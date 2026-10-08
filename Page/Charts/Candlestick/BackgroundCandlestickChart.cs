using SkiaSharp;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Loading;
using TCYM.UI.Events;

namespace Page.Charts.Candlestick
{
    /// <summary>
    /// 大数据案例专用画布：后台线程独占 ChartEngine，UI 线程只显示已完成的不可变 SKImage。
    /// 数据生成、SetOption、缩放重建和 Render 都不在 UI 线程执行。
    /// </summary>
    internal sealed class BackgroundCandlestickChart : UIView, IDisposable
    {
        private Func<CancellationToken, ChartOption>? _createOption;
        private readonly CancellationTokenSource _cancellation = new();
        private readonly AutoResetEvent _wake = new(false);
        private readonly object _gate = new();
        private readonly Queue<Func<ChartEngine, bool>> _commands = new();
        private readonly UIView _loadingLayer;
        private readonly NeonLoading _spinner;
        private readonly UILabel _message;

        private Viewport? _viewport;
        private Viewport? _displayedViewport;
        private PendingFrame? _pendingFrame;
        private Func<ChartEngine, bool>? _pendingMove;
        private string? _pendingMessage;
        private string? _pendingError;
        private SKImage? _image; // 只由 UI 线程读取、替换和释放。
        private Task? _worker;
        private int _disposed;
        private int _memoryCleanupScheduled;
        private bool _pointerPressed; // 只由后台图表线程访问。

        internal bool IsReady { get; private set; }
        internal string? Error { get; private set; }
        /// <summary>后台线程完全退出的任务；加载首帧完成通过 Loaded 通知。</summary>
        internal Task Completion => _worker ?? Task.CompletedTask;
        internal event Action? Loaded;
        internal event Action<string>? LoadingFailed;

        // 所有鼠标变化均由后台完成绘制后主动请求重绘。
        protected override Type? ExplicitPointerMoveInvalidationOwner => typeof(BackgroundCandlestickChart);

        internal BackgroundCandlestickChart(Func<CancellationToken, ChartOption> createOption)
        {
            _createOption = createOption;
            _spinner = new NeonLoading
            {
                SizeLoader = 72f,
                Style = new DefaultUIStyle { Width = 72, Height = 72 },
            };
            _message = new UILabel
            {
                Text = "正在后台加载 K 线数据…",
                Style = new DefaultUIStyle
                {
                    Width = "100%", Height = 32,
                    FontSize = 14, TextAlign = "center", Color = SKColor.Parse("#666666"),
                },
            };
            _loadingLayer = new UIView
            {
                Style = new DefaultUIStyle
                {
                    Position = "absolute", Left = 0, Top = 0,
                    Width = "100%", Height = "100%",
                    Display = "flex", FlexDirection = "column",
                    AlignItems = "center", JustifyContent = "center",
                    BackgroundColor = SKColors.White,
                },
                Children = [_spinner, _message],
            };
            Children = [_loadingLayer];
        }

        /// <summary>只在页面进入时启动一次。复制字体后由后台独立拥有，避免借用已释放的 UI 字体。</summary>
        internal void Start()
        {
            if (_worker is not null || Volatile.Read(ref _disposed) != 0) return;
            var createOption = _createOption;
            if (createOption is null) return;
            SKData? fontData = null;
            try
            {
                using var fontStream = Font.OpenStream(out var faceIndex);
                fontData = SKData.Create(fontStream);
                var ownedFontData = fontData;
                // LongRunning 保证整个同步内核始终在同一工作线程，而不是在 await 后换线程。
                _worker = Task.Factory.StartNew(
                    () => Run(createOption, ownedFontData, faceIndex),
                    CancellationToken.None,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
                fontData = null;
            }
            catch (Exception exception)
            {
                fontData?.Dispose();
                PublishError(exception.Message);
            }
        }

        /// <summary>快捷按钮只排队 action，不在 UI 线程调用 SetOption 或访问后台内核。</summary>
        internal void DispatchAction(ChartAction action)
        {
            if (!IsReady) return;
            Enqueue(engine => { engine.DispatchAction(action); return true; });
        }

        public override void Layout(float x, float y, float width, float height)
        {
            base.Layout(x, y, width, height);
            QueueViewport();
        }

        public override void Update(float deltaSeconds)
        {
            if (Volatile.Read(ref _disposed) != 0) return;
            QueueViewport();
            ApplyPendingFrame();
            base.Update(deltaSeconds);
        }

        protected override void RenderContent(SKCanvas canvas)
        {
            if (Volatile.Read(ref _disposed) != 0) return;
            if (_image is not null)
            {
                canvas.DrawImage(_image, ContentRect(),
                    new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None));
            }
            base.RenderContent(canvas);
        }

        /// <summary>把窗口坐标转换为逻辑图表坐标，鼠标事件在后台按顺序执行。</summary>
        public override void OnMouseEvent(UIMouseEvent e)
        {
            base.OnMouseEvent(e);
            if (!IsReady || Volatile.Read(ref _disposed) != 0
                || _displayedViewport != _viewport
                || !TryScreenToLocal(e.X, e.Y, out var local))
                return;

            var content = ContentRect();
            var point = new SKPoint(
                (local.X - content.Left) / EffectiveLayoutDpiScale,
                (local.Y - content.Top) / EffectiveLayoutDpiScale);
            var button = e.Button;
            var modifiers = e.Modifiers;
            var wheel = e.WheelDelta;
            switch (e.Type)
            {
                case UIMouseEventType.Move:
                    Enqueue(engine => engine.PointerMove(point.X, point.Y, modifiers), isMove: true);
                    break;
                case UIMouseEventType.Down:
                    Enqueue(engine =>
                    {
                        _pointerPressed = true;
                        return engine.PointerDown(point.X, point.Y, button, modifiers);
                    });
                    break;
                case UIMouseEventType.Up:
                    Enqueue(engine =>
                    {
                        _pointerPressed = false;
                        return engine.PointerUp(point.X, point.Y, button, modifiers);
                    });
                    break;
                case UIMouseEventType.Wheel:
                    Enqueue(engine => engine.PointerWheel(point.X, point.Y, wheel, modifiers));
                    break;
                case UIMouseEventType.DoubleClick:
                    Enqueue(engine => engine.DoubleClick(point.X, point.Y, button, modifiers));
                    break;
                case UIMouseEventType.Leave:
                    Enqueue(engine =>
                    {
                        engine.PointerLeave(point.X, point.Y);
                        if (_pointerPressed)
                        {
                            engine.PointerCancel(point.X, point.Y);
                            _pointerPressed = false;
                        }
                        return true;
                    });
                    break;
            }
        }

        private SKRect ContentRect() => new(
            BorderWidth + PaddingLeft,
            BorderWidth + PaddingTop,
            Math.Max(BorderWidth + PaddingLeft, Width - BorderWidth - PaddingRight),
            Math.Max(BorderWidth + PaddingTop, Height - BorderWidth - PaddingBottom));

        private void QueueViewport()
        {
            var content = ContentRect();
            if (content.Width <= 0 || content.Height <= 0) return;
            var scale = EffectiveLayoutDpiScale;
            var density = Math.Max(scale, EffectiveDevicePixelRatio);
            var viewport = new Viewport(
                content.Width / scale, content.Height / scale,
                Math.Max(1, (int)Math.Ceiling(content.Width / scale * density)),
                Math.Max(1, (int)Math.Ceiling(content.Height / scale * density)));
            lock (_gate)
            {
                if (_viewport == viewport || _disposed != 0) return;
                _viewport = viewport;
            }
            Wake();
        }

        private void Enqueue(Func<ChartEngine, bool> command, bool isMove = false)
        {
            lock (_gate)
            {
                if (_disposed != 0) return;
                if (isMove)
                {
                    // 重建期间只保留最新鼠标位置，防止旧 Tooltip 事件越积越多。
                    _pendingMove = command;
                }
                else
                {
                    if (_pendingMove is not null) _commands.Enqueue(_pendingMove);
                    _pendingMove = null;
                    _commands.Enqueue(command);
                }
            }
            Wake();
        }

        /// <summary>从生成数据到释放场景，所有内核调用都在本工作线程内完成。</summary>
        private void Run(
            Func<CancellationToken, ChartOption> createOption,
            SKData fontData,
            int faceIndex)
        {
            var token = _cancellation.Token;
            try
            {
                using (fontData)
                using (var font = SKTypeface.FromData(fontData, faceIndex)
                    ?? throw new InvalidOperationException("无法复制图表字体。"))
                using (var engine = new ChartEngine(theme: new ChartTheme { Typeface = font }))
                {
                    var option = createOption(token);
                    token.ThrowIfCancellationRequested();
                    PublishMessage("数据已生成，正在后台构建图表…");

                    Viewport viewport;
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        lock (_gate)
                        {
                            if (_viewport is { } available) { viewport = available; break; }
                        }
                        _wake.WaitOne();
                    }

                    engine.EnableLargeBatchRasterCache = true;
                    engine.Resize(viewport.Width, viewport.Height);
                    engine.SetOption(option, new ChartSetOptionOptions { NotMerge = true });
                    // Engine 已捕获独立快照，不再让工作线程栈同时保留原始十万条 Option/Dataset。
                    option = null!;
                    token.ThrowIfCancellationRequested();
                    ThrowOnBuildError(engine);
                    var initial = true;
                    var dirty = true;
                    while (!token.IsCancellationRequested)
                    {
                        Func<ChartEngine, bool>[] commands;
                        lock (_gate)
                        {
                            if (_viewport is { } latest && latest != viewport)
                            {
                                viewport = latest;
                                dirty = true;
                            }
                            if (_pendingMove is not null) _commands.Enqueue(_pendingMove);
                            _pendingMove = null;
                            commands = _commands.ToArray();
                            _commands.Clear();
                        }

                        engine.Resize(viewport.Width, viewport.Height);
                        foreach (var command in commands)
                        {
                            token.ThrowIfCancellationRequested();
                            dirty |= command(engine);
                        }

                        if (dirty || engine.NeedsAnotherFrame)
                        {
                            using var surface = SKSurface.Create(new SKImageInfo(
                                viewport.PixelWidth, viewport.PixelHeight, SKColorType.Bgra8888, SKAlphaType.Premul))
                                ?? throw new InvalidOperationException("无法创建后台图表画布。");
                            surface.Canvas.Clear(SKColors.White);
                            surface.Canvas.Scale(viewport.PixelWidth / viewport.Width, viewport.PixelHeight / viewport.Height);
                            engine.Render(surface.Canvas);
                            token.ThrowIfCancellationRequested();
                            ThrowOnBuildError(engine);
                            // 首次等 Progressive 完成后再隐藏 loading；后续交互可连续发布动画帧。
                            if (!initial || !engine.NeedsAnotherFrame)
                            {
                                PublishFrame(new PendingFrame(surface.Snapshot(), viewport));
                                initial = false;
                            }
                            dirty = false;
                        }
                        _wake.WaitOne(engine.NeedsAnotherFrame ? 16 : Timeout.Infinite);
                    }
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { }
            catch (Exception exception)
            {
                if (!token.IsCancellationRequested) PublishError(exception.Message);
            }
            finally
            {
                // 不在 UI Dispose 中等待这里，否则用户切页仍会被后台构建阻塞。
                _wake.Dispose();
                _cancellation.Dispose();
            }
        }

        private static void ThrowOnBuildError(ChartEngine engine)
        {
            foreach (var diagnostic in engine.Diagnostics.Items)
            {
                if (diagnostic.Severity == ChartDiagnosticSeverity.Error)
                    throw new InvalidOperationException(diagnostic.Message, diagnostic.Exception);
            }
        }

        private void PublishFrame(PendingFrame frame)
        {
            lock (_gate)
            {
                if (_disposed != 0) { frame.Image.Dispose(); return; }
                _pendingFrame?.Image.Dispose();
                _pendingFrame = frame;
            }
            RequestRedraw(); // 框架明确允许从后台线程请求重绘；UI 对象变更留到 Update。
        }

        private void PublishMessage(string message)
        {
            lock (_gate)
            {
                if (_disposed != 0) return;
                _pendingMessage = message;
            }
            RequestRedraw();
        }

        private void PublishError(string message)
        {
            lock (_gate)
            {
                if (_disposed != 0) return;
                _pendingError = message;
            }
            RequestRedraw();
        }

        /// <summary>只在 UI Update 中交换画面、更新 loading 和启用按钮。</summary>
        private void ApplyPendingFrame()
        {
            PendingFrame? frame;
            string? message;
            string? error;
            lock (_gate)
            {
                frame = _pendingFrame;
                message = _pendingMessage;
                error = _pendingError;
                _pendingFrame = null;
                _pendingMessage = _pendingError = null;
            }
            if (message is not null && !IsReady) _message.Text = message;
            if (error is not null)
            {
                frame?.Image.Dispose();
                Error = error;
                IsReady = false;
                if (_spinner.Parent == _loadingLayer) _loadingLayer.RemoveChildAndDispose(_spinner);
                var errorText = "加载失败：" + error + "；可返回后重试。";
                if (_loadingLayer.Parent is not null)
                {
                    _message.Text = errorText;
                    _message.TextColor = SKColor.Parse("#d4380d");
                }
                else
                {
                    AddChild(new UILabel
                    {
                        Text = errorText,
                        Style = new DefaultUIStyle
                        {
                            Position = "absolute", Left = 0, Top = 0,
                            Width = "100%", Height = 40,
                            BackgroundColor = SKColors.White, Color = SKColor.Parse("#d4380d"),
                        },
                    });
                }
                LoadingFailed?.Invoke(error);
                return;
            }
            if (frame is null) return;
            if (frame.Viewport != _viewport) { frame.Image.Dispose(); return; }
            var old = _image;
            _image = frame.Image;
            _displayedViewport = frame.Viewport;
            old?.Dispose();
            InvalidateRenderCache();
            if (!IsReady)
            {
                IsReady = true;
                RemoveChildAndDispose(_loadingLayer);
                Loaded?.Invoke();
            }
        }

        private void Wake()
        {
            try { _wake.Set(); }
            catch (ObjectDisposedException) { }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
            IsReady = false;
            _createOption = null;
            try { _cancellation.Cancel(); }
            catch (ObjectDisposedException) { }
            Wake();
            lock (_gate)
            {
                _commands.Clear();
                _pendingMove = null;
                _pendingFrame?.Image.Dispose();
                _pendingFrame = null;
                _pendingMessage = _pendingError = null;
            }
            _image?.Dispose();
            _image = null;
            Loaded = null;
            LoadingFailed = null;
            _spinner.Dispose();
            if (_worker is null)
            {
                _wake.Dispose();
                _cancellation.Dispose();
            }
            else
            {
                ScheduleMemoryCleanupAfterWorker(_worker);
            }
        }

        /// <summary>
        /// 不阻塞 UI 线程等待大数据构建退出；工作线程真正释放 Engine 与 Dataset 后，
        /// 再合并请求一次托管堆和 Windows 工作集整理。后续路由导航会取消尚未开始的整理，
        /// 避免在连续切换页面时强制 GC。
        /// </summary>
        private void ScheduleMemoryCleanupAfterWorker(Task worker)
        {
            if (Interlocked.Exchange(ref _memoryCleanupScheduled, 1) != 0) return;
            _ = worker.ContinueWith(
                static _ => UISystem.RequestMemoryCleanup(
                    trimProcessWorkingSet: true,
                    delayMilliseconds: 100),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        private sealed record Viewport(float Width, float Height, int PixelWidth, int PixelHeight);
        private sealed record PendingFrame(SKImage Image, Viewport Viewport);
    }
}
