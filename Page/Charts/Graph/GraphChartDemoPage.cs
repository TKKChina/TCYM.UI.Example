using SkiaSharp;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace Page.Charts.Graph;

/// <summary>只共用 Lines 风格的页面外壳和生命周期；每个案例自己创建完整 Options。</summary>
internal abstract class GraphChartDemoPage : UIScrollView, IUIRouteLifecycle
{
    private readonly ChartOption _initialOption;
    private readonly string[] _imageNames;
    private GraphImageAssets? _images;
    private CancellationTokenSource? _running;
    private bool _disposed;
    private bool _ready;
    protected GraphDemoChart Chart { get; }
    protected UIView Toolbar { get; }
    protected UILabel Status { get; }
    protected virtual TimeSpan? UpdateInterval => null;

    protected GraphChartDemoPage(string title, string subtitle, string description, ChartOption option,
        ChartRegistry? registry = null, params string[] imageNames)
    {
        _initialOption = option;
        _imageNames = imageNames;
        UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.Charts.Graph.style.css");
        Chart = new GraphDemoChart(registry ?? ChartModules.CreateDefaultRegistry()) { ClassName = "graph-chart-canvas" };
        Chart.ViewportChanged += size => { if (_ready && !_disposed) OnViewportChanged(size); };
        Toolbar = new UIView { ClassName = "graph-chart-toolbar" };
        var className = imageNames.Length == 0 ? "graph-chart-status-hidden" : "";
        Status = new UILabel
        {
            Text = imageNames.Length > 0 ? "Loading… 正在后台加载图片资源" : "",
            ClassName = $"graph-chart-status {className}",
        };
        ClassName = "graph-chart-demo";
        Children =
        [
            new UILabel { Text = title, ClassName = "graph-chart-demo-title" },
            new UILabel { Text = subtitle, ClassName = "graph-chart-demo-subtitle" },
            new UILabel { Text = description, ClassName = "graph-chart-demo-description" },
            new UIView
            {
                ClassName = "graph-chart-preview-card",
                Children =
                [
                    new UILabel { Text = "效果预览", ClassName = "graph-chart-card-title" },
                    new UILabel
                    {
                        Text = "预览画布与 Lines 一样为 100% × 590；宽度随容器自适应。悬停查看节点/连线信息；PC 按 Ctrl＋滚轮缩放，触屏双指缩放，普通滚轮保留页面滚动。",
                        ClassName = "graph-chart-preview-help",
                    },
                    Toolbar,
                    Status,
                    new UIScrollView
                    {
                        ClassName = "graph-chart-preview-scroll",
                        ShowScrollbar = false,
                        Children = [Chart],
                    }
                ],
            },
        ];
        if (imageNames.Length == 0) ApplyInitialOption();
    }

    private void ApplyInitialOption()
    {
        Chart.SetOption(_initialOption, new ChartSetOptionOptions { NotMerge = true });
        _ready = true;
        // 图片可能在首次布局后才加载完；此时也要按当前内容区完成定位。
        if (Chart.ViewportSize.Width > 0 && Chart.ViewportSize.Height > 0)
            OnViewportChanged(Chart.ViewportSize);
    }

    /// <summary>尺寸均为去除边框、内边距和布局 DPI 后的逻辑像素。</summary>
    protected virtual void OnViewportChanged(SKSize size) { }

    protected void AddButton(string text, Action action)
    {
        var button = new UIButton { Text = text, ClassName = "graph-chart-toolbar-button" };
        button.OnClick += _ => { if (!_disposed) action(); };
        Toolbar.AddChild(button);
    }

    public void OnRouteEnter(string? fromPath)
    {
        if (_disposed || _running is not null) return;
        _running = CancellationTokenSource.CreateLinkedTokenSource(LifetimeToken);
        _ = RunAsync(_running.Token);
    }

    private async Task RunAsync(CancellationToken token)
    {
        GraphImageAssets? prepared = null;
        try
        {
            if (_imageNames.Length > 0)
            {
                prepared = await Task.Run(() => GraphImageAssets.Load(_imageNames, token), token);
                await UIDispatcher.InvokeAsync(() =>
                {
                    if (token.IsCancellationRequested || _disposed) return;
                    _images = prepared;
                    prepared = null;
                    Chart.SetImageResolver(_images);
                    ApplyInitialOption();
                    Status.AddClass("graph-chart-status-hidden");
                });
            }
            token.ThrowIfCancellationRequested();
            if (UpdateInterval is not { } interval) return;
            using var timer = new PeriodicTimer(interval);
            while (await timer.WaitForNextTickAsync(token))
            {
                var keepRunning = false;
                await UIDispatcher.InvokeAsync(() =>
                {
                    if (!token.IsCancellationRequested && !_disposed) keepRunning = UpdateDemo();
                });
                if (!keepRunning) break;
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception exception)
        {
            await UIDispatcher.InvokeAsync(() =>
            {
                if (!token.IsCancellationRequested && !_disposed) Status.Text = $"案例已停止：{exception.Message}";
            });
        }
        finally { prepared?.Dispose(); }
    }

    /// <summary>仅由 UI 线程定时调用，返回 false 停止本次循环。</summary>
    protected virtual bool UpdateDemo() => false;

    public void OnRouteLeave(string? toPath)
    {
        if (_disposed) return;
        _disposed = true;
        var running = Interlocked.Exchange(ref _running, null);
        running?.Cancel();
        running?.Dispose();
        Chart.Clear();
        Chart.Dispose();
        _images?.Dispose();
        _images = null;
    }

    internal static GraphChartDemoPage CreateRelation() => new RelationGraphDemo();
    internal static GraphChartDemoPage CreateDynamic() => new DynamicNodesGraphDemo();
    internal static GraphChartDemoPage CreateComplex() => new ComplexRelationGraphDemo();
    internal static GraphChartDemoPage CreateDistributed() => new DistributedTopologyGraphDemo();
}
