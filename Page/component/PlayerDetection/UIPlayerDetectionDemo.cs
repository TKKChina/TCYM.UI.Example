using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Input;
using TCYM.UI.Pro.Elements.Player;
using TCYM.UI.Pro.Vision;

namespace TCYM.UI.Example.Page.component.PlayerDetection
{
    /// <summary>
    /// 视频识别示例：在 <see cref="UIPlayer"/> 上叠加 YOLOv8（ONNX Runtime）目标检测框。
    /// 演示加载本地 ONNX 模型、切换“仅预览叠加 / 烧录进帧 / 关闭”三种绘制模式，
    /// 并实时显示最近一次检测到的目标类别与置信度。
    /// </summary>
    internal class UIPlayerDetectionDemo : UIScrollView, IUIRouteLifecycle
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.PlayerDetection.style.css";

        // 训练好的 YOLOv8 模型默认路径；不存在时仅提示，不影响页面打开。
        private const string DefaultModelPath = @"G:\C#\训练模型\best.onnx";
        private const string DefaultSourceUrl = @"";

        private readonly UIPlayer _player;
        private readonly UIInput _sourceInput;
        private readonly UIInput _modelInput;
        private readonly UILabel _modelStatusLabel;
        private readonly UILabel _detectionResultLabel;
        private readonly UILabel _playbackStatusLabel;

        private UIButton? _loadModelButton;
        private UIButton? _modeOffButton;
        private UIButton? _modeOverlayButton;
        private UIButton? _modeBurnButton;

        private Yolov8OnnxDetector? _detector;
        private bool _modelLoading;

        // 检测结果在后台推理线程更新，这里用 volatile + 版本号在 Update 中安全刷新到 UI。
        private volatile string _detectionSummary = "尚未加载模型，检测未开始。";
        private long _detectionVersion;
        private long _renderedDetectionVersion;

        internal UIPlayerDetectionDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            EnableDeferredChildLoading = false;

            _sourceInput = CreateInput(DefaultSourceUrl, "请输入本地视频路径、HTTP 或 RTSP 地址");
            _modelInput = CreateInput(DefaultModelPath, "请输入 YOLOv8 ONNX 模型路径（best.onnx）");

            _modelStatusLabel = CreateText("待加载模型。", "pd-status-label");
            _modelStatusLabel.MaxLines = 3;
            _detectionResultLabel = CreateText(_detectionSummary, "pd-detection-result");
            _detectionResultLabel.MaxLines = 4;
            _playbackStatusLabel = CreateText("待播放。", "pd-status-label");
            _playbackStatusLabel.Wrap = false;
            _playbackStatusLabel.MaxLines = 1;
            _playbackStatusLabel.Ellipsize = true;

            _player = new UIPlayer
            {
                Id = "detection-player",
                SourceUrl = DefaultSourceUrl,
                AutoPlay = false,
                MaintainAspectRatio = true,
                // 默认仅在预览上叠加检测框，不改动原始帧（不影响录制/拍照）。
                DetectionRenderMode = DetectionRenderMode.PreviewOverlay,
                // 每隔 3 帧推理一次，兼顾实时性与开销。
                DetectionFrameInterval = 3,
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = "100%",
                }
            };
            _player.DetectionsUpdated += OnDetectionsUpdated;

            ClassName = new List<string> { "pd-demo-view" };
            Children = new()
            {
                CreateText("Player 视频识别", "pd-demo-title"),
                CreateText("基于 TCYM.UI.Pro 的 UIPlayer + YOLOv8（ONNX Runtime）目标检测示例。", "pd-demo-title-sub"),
                CreateText("加载训练好的 best.onnx 后，检测框会按所选模式叠加到视频画面；预览叠加不会改动原始视频，烧录进帧则会写入录制与拍照。", "pd-demo-desc"),
                CreateModelSection(),
                CreatePlaybackSection(),
                CreatePreviewSection(),
            };
        }

        public void OnRouteEnter(string? fromPath)
        {
            SetPlaybackStatus("待播放。");
        }

        public void OnRouteLeave(string? toPath)
        {
            _player.DetectionsUpdated -= OnDetectionsUpdated;
            _player.DisposePlayer();
            _detector?.Dispose();
            _detector = null;
        }

        /// <summary>
        /// 每帧把后台线程产生的检测摘要安全刷新到 UI（避免跨线程直接改 UI）。
        /// </summary>
        public override void Update(float deltaSeconds)
        {
            base.Update(deltaSeconds);

            long version = Interlocked.Read(ref _detectionVersion);
            if (version != _renderedDetectionVersion)
            {
                _renderedDetectionVersion = version;
                _detectionResultLabel.Text = _detectionSummary;
            }
        }

        private UIView CreateModelSection()
        {
            _loadModelButton = CreateButton("加载模型", LoadModel, "pd-btn-primary");
            _modeOffButton = CreateButton("关闭检测", () => SetRenderMode(DetectionRenderMode.None), "pd-btn-neutral");
            _modeOverlayButton = CreateButton("预览叠加", () => SetRenderMode(DetectionRenderMode.PreviewOverlay), "pd-btn-secondary");
            _modeBurnButton = CreateButton("烧录进帧", () => SetRenderMode(DetectionRenderMode.BurnIn), "pd-btn-secondary");
            UpdateModeButtons();

            return CreateSectionCard(
                "识别模型",
                "填写 YOLOv8 导出的 ONNX 模型路径并加载；加载在后台进行，自动优先使用 GPU(DirectML)，失败回退 CPU。",
                CreateField("模型路径", _modelInput),
                new UIView
                {
                    ClassName = new List<string> { "pd-button-row" },
                    Children = new() { _loadModelButton }
                },
                CreateText("检测绘制模式", "pd-field-label"),
                new UIView
                {
                    ClassName = new List<string> { "pd-button-row" },
                    Children = new() { _modeOffButton, _modeOverlayButton, _modeBurnButton }
                },
                _modelStatusLabel,
                _detectionResultLabel,
                CreateHintLabel("预览叠加：仅控件预览显示框，不改原始帧；烧录进帧：框写入帧内存，预览、录制与拍照均可见。")
            );
        }

        private UIView CreatePlaybackSection()
        {
            return CreateSectionCard(
                "播放控制",
                "填写播放源后开始播放；识别会在播放过程中自动进行。",
                CreateField("播放源", _sourceInput),
                new UIView
                {
                    ClassName = new List<string> { "pd-button-row" },
                    Children = new()
                    {
                        CreateButton("开始播放", StartPlayback, "pd-btn-primary"),
                        CreateButton("停止播放", StopPlayback, "pd-btn-neutral"),
                        CreateButton("切换播放源", SwitchSource, "pd-btn-secondary"),
                    }
                },
                _playbackStatusLabel
            );
        }

        private UIView CreatePreviewSection()
        {
            return CreateSectionCard(
                "预览画面",
                "视频内容由 UIPlayer 绘制；检测框按所选模式叠加。",
                new UIView
                {
                    ClassName = new List<string> { "pd-preview-shell" },
                    Children = new() { _player }
                }
            );
        }

        private async void LoadModel()
        {
            if (_modelLoading) return;

            string modelPath = _modelInput.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(modelPath))
            {
                SetModelStatus("请输入模型路径后再加载。");
                return;
            }

            if (!File.Exists(modelPath))
            {
                SetModelStatus($"未找到模型文件：\n{modelPath}");
                return;
            }

            _modelLoading = true;
            UpdateModeButtons();
            SetModelStatus("正在加载模型……");

            try
            {
                // 在后台创建检测器，避免初始化 ONNX 会话阻塞 UI 线程。
                // classNames 传 null 使用模型内置 16 类英文缩写；后续可替换为中文映射。
                var detector = await Task.Run(() => new Yolov8OnnxDetector(modelPath));

                _detector?.Dispose();
                _detector = detector;
                _player.Detector = detector;
                if (_player.DetectionRenderMode == DetectionRenderMode.None)
                {
                    SetRenderMode(DetectionRenderMode.PreviewOverlay);
                }

                string backend = detector.IsUsingGpu ? "GPU / DirectML" : "CPU";
                string status = $"模型已加载（{backend}），输入 {detector.InputWidth}×{detector.InputHeight}，类别数 {detector.ClassNames.Count}。";
                if (!detector.IsUsingGpu && detector.GpuInitError != null)
                {
                    status += $"\nGPU 初始化失败，已回退 CPU：{detector.GpuInitError}";
                }
                SetModelStatus(status);
            }
            catch (Exception ex)
            {
                _detector = null;
                _player.Detector = null;
                SetModelStatus($"模型加载失败：{ex.Message}");
            }
            finally
            {
                _modelLoading = false;
                UpdateModeButtons();
            }
        }

        private void SetRenderMode(DetectionRenderMode mode)
        {
            _player.DetectionRenderMode = mode;
            UpdateModeButtons();

            string text = mode switch
            {
                DetectionRenderMode.None => "已关闭检测绘制。",
                DetectionRenderMode.PreviewOverlay => "已切换为：预览叠加（不改原始帧）。",
                DetectionRenderMode.BurnIn => "已切换为：烧录进帧（写入录制与拍照）。",
                _ => string.Empty,
            };
            SetModelStatus(text);
        }

        private void StartPlayback()
        {
            string? source = NormalizeSource();
            if (source == null) return;

            _player.SourceUrl = source;
            _player.Play();
            SetPlaybackStatus($"正在播放：{FormatSourceForStatus(source)}");
        }

        private void StopPlayback()
        {
            _player.Stop();
            SetPlaybackStatus("已停止播放。");
        }

        private void SwitchSource()
        {
            string? source = NormalizeSource();
            if (source == null) return;

            _player.SwitchUrl(source);
            SetPlaybackStatus($"已切换播放源：{FormatSourceForStatus(source)}");
        }

        private void OnDetectionsUpdated(IReadOnlyList<DetectionBox> detections)
        {
            // 后台推理线程回调：只更新 volatile 字段与版本号，UI 在 Update 中刷新。
            if (detections.Count == 0)
            {
                _detectionSummary = "本帧未检测到目标。";
            }
            else
            {
                var items = detections
                    .OrderByDescending(d => d.Confidence)
                    .Take(6)
                    .Select(d => $"{d.Label} {d.Confidence:P0}");
                _detectionSummary = $"检测到 {detections.Count} 个目标：" + string.Join("、", items);
            }

            Interlocked.Increment(ref _detectionVersion);
        }

        private string? NormalizeSource()
        {
            string source = _sourceInput.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(source))
            {
                SetPlaybackStatus("请输入播放源后再操作。");
                return null;
            }

            return source;
        }

        private void UpdateModeButtons()
        {
            if (_loadModelButton != null)
            {
                _loadModelButton.Disabled = _modelLoading;
                _loadModelButton.Text = _modelLoading ? "加载中…" : "加载模型";
            }

            DetectionRenderMode mode = _player.DetectionRenderMode;
            SetModeButtonActive(_modeOffButton, mode == DetectionRenderMode.None);
            SetModeButtonActive(_modeOverlayButton, mode == DetectionRenderMode.PreviewOverlay);
            SetModeButtonActive(_modeBurnButton, mode == DetectionRenderMode.BurnIn);
        }

        private static void SetModeButtonActive(UIButton? button, bool active)
        {
            if (button == null) return;

            var classes = new List<string> { "pd-action-btn" };
            if (active)
            {
                classes.Add("pd-btn-active");
            }
            button.ClassName = classes;
        }

        private void SetModelStatus(string text) => _modelStatusLabel.Text = text;

        private void SetPlaybackStatus(string text) => _playbackStatusLabel.Text = text;

        private static string FormatSourceForStatus(string source)
        {
            if (Uri.TryCreate(source, UriKind.Absolute, out var uri) && !uri.IsFile)
            {
                string host = string.IsNullOrWhiteSpace(uri.Host) ? source : uri.Host;
                return string.IsNullOrWhiteSpace(uri.AbsolutePath) || uri.AbsolutePath == "/"
                    ? $"{uri.Scheme}://{host}"
                    : $"{uri.Scheme}://{host}{uri.AbsolutePath}";
            }

            try
            {
                string fileName = Path.GetFileName(source);
                return string.IsNullOrWhiteSpace(fileName) ? source : fileName;
            }
            catch
            {
                return source;
            }
        }

        private UIView CreateField(string label, UIInput input)
        {
            return new UIView
            {
                ClassName = new List<string> { "pd-field" },
                Children = new()
                {
                    CreateText(label, "pd-field-label"),
                    input
                }
            };
        }

        private static UIInput CreateInput(string text, string placeholder)
        {
            return new UIInput
            {
                Text = text,
                Placeholder = placeholder,
                AllowClear = true
            };
        }

        private static UIButton CreateButton(string text, Action action, string className)
        {
            return new UIButton
            {
                Text = text,
                ClassName = new List<string> { "pd-action-btn", className },
                CornerCurve = "continuous",
                Events = new()
                {
                    Click = _ => action()
                }
            };
        }

        private static UIView CreateSectionCard(string title, string description, params UIElement[] children)
        {
            var sectionChildren = new List<UIElement>
            {
                CreateText(title, "pd-card-title"),
                CreateText(description, "pd-card-desc")
            };

            sectionChildren.AddRange(children);

            return new UIView
            {
                ClassName = new List<string> { "pd-demo-card" },
                Children = sectionChildren,
            };
        }

        private static UILabel CreateHintLabel(string text) => CreateText(text, "pd-demo-hint");

        private static UILabel CreateText(string text, string className)
        {
            return new UILabel
            {
                Text = text,
                Wrap = true,
                ClassName = new List<string> { className }
            };
        }
    }
}
