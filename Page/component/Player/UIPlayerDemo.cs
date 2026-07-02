using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Input;
using TCYM.UI.Pro.Elements.Player;

namespace TCYM.UI.Example.Page.component.Player
{
    internal class UIPlayerDemo : UIScrollView, IUIRouteLifecycle
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Player.style.css";
        private const string DefaultSourceUrl = @"";

        private readonly UIPlayer _player;
        private readonly UIInput _sourceInput;
        private readonly UIInput _watermarkInput;
        private readonly UILabel _statusLabel;
        private readonly UILabel _recordingResultLabel;
        private readonly UIView _previewSection;
        private UIButton? _startRecordingButton;
        private UIButton? _stopRecordingButton;
        private string? _currentRecordingFilePath;
        private bool _recordingOperationPending;

        internal UIPlayerDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            EnableDeferredChildLoading = false;

            _sourceInput = CreateInput(DefaultSourceUrl, "请输入本地视频路径、HTTP 或 RTSP 地址");
            _watermarkInput = CreateInput("测试水印", "请输入水印文本");
            _statusLabel = CreateText("待播放", "player-status-label");
            _statusLabel.Wrap = false;
            _statusLabel.MaxLines = 1;
            _statusLabel.Ellipsize = true;
            _recordingResultLabel = CreateText("尚未开始录制。", "player-recording-result");
            _recordingResultLabel.MaxLines = 6;
            _player = new UIPlayer
            {
                Id = "player",
                SourceUrl = DefaultSourceUrl,
                AutoPlay = false,
                MaintainAspectRatio = true,
                Watermarks = new()
                {
                    new WatermarkSpec
                    {
                      TextProvider = () => _watermarkInput.Text,
                      FontSize = 16,
                      Color = SKColors.Red.WithAlpha(200),
                      Position = new SKPoint(30, 60),
                    }
                },
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = "100%",
                }
            };
            _previewSection = CreatePreviewSection();

            ClassName = new List<string> { "player-demo-view" };
            Children = new()
             {
                CreateText("Player 视频播放", "player-demo-title"),
                CreateText("基于 TCYM.UI.Pro 的 FFmpeg 视频播放控件示例。", "player-demo-title-sub"),
                CreateText("支持本地文件、HTTP 与 RTSP 播放源，示例页同时演示动态水印、播放源切换和 MP4 录制。", "player-demo-desc"),
                CreateSupportedFormatsSection(),
                CreateControlSection(),
                _previewSection,
             };
        }

        public void OnRouteEnter(string? fromPath)
        {
            SetStatus("待播放");
        }

        public void OnRouteLeave(string? toPath)
        {
            bool wasRecording = _player.IsRecording;
            _player.DisposePlayer();
            if (wasRecording)
            {
                SetRecordingResult(BuildRecordingResultMessage());
            }
            SetStatus("已离开播放器示例页，播放已停止");
        }

        private UIView CreateControlSection()
        {
            return CreateSectionCard(
                "播放控制",
                "填写播放源和水印文本后，可以手动开始、停止、切换当前播放源，并支持拍照保存与 MP4 录制。",
                new UIView
                {
                    ClassName = new List<string> { "player-control-panel" },
                    Children = new()
                    {
                        CreateField("播放源", _sourceInput),
                        CreateField("水印文本", _watermarkInput),
                        CreateToolbar(),
                        CreateRecordingToolbar(),
                        _statusLabel
                    }
                },
                _recordingResultLabel,
                CreateHintLabel("水印文本会在视频帧内按原始分辨率坐标绘制。")
            );
        }

        private UIView CreatePreviewSection()
        {
            return CreateSectionCard(
                "预览画面",
                "播放器区域保持固定高度，视频内容由 UIPlayer 根据缩放模式绘制到画布。",
                new UIView
                {
                    ClassName = new List<string> { "player-preview-shell" },
                    Children = new()
                  {
                _player
                  }
                }
            );
        }

        private static UIView CreateSupportedFormatsSection()
        {
            return CreateSectionCard(
                "支持格式",
                "UIPlayer 使用 FFmpeg 读取媒体源，实际支持范围取决于随 TCYM.UI.Pro 发布的 FFmpeg 编译能力。",
                CreateFormatText("播放输入：本地文件 MP4、MOV、MKV、AVI、FLV、WebM、MPEG-TS/TS、M3U8/HLS 等。\n网络源：HTTP/HTTPS、RTSP、RTMP、HLS/M3U8 等。\n常见视频编码：H.264/AVC、H.265/HEVC、MPEG-4、VP8、VP9 等。\n录制输出：MP4，默认 H.264 编码。")
            );
        }

        private UIView CreateToolbar()
        {
            return new UIView
            {
                ClassName = new List<string> { "player-button-row" },
                Children = new()
        {
          CreateButton("开始播放", StartPlayback, "player-btn-primary"),
          CreateButton("停止播放", StopPlayback, "player-btn-neutral"),
          CreateButton("切换播放源", SwitchSource, "player-btn-secondary"),
          CreateButton("拍照保存", CapturePhoto, "player-btn-secondary"),
        }
            };
        }

        private UIView CreateRecordingToolbar()
        {
            _startRecordingButton = CreateButton("开始录制", StartRecording, "player-btn-record");
            _stopRecordingButton = CreateButton("停止录制", StopRecording, "player-btn-neutral");
            UpdateRecordingActionButtons();

            return new UIView
            {
                ClassName = new List<string> { "player-button-row" },
                Children = new()
        {
            _startRecordingButton,
            _stopRecordingButton,
        }
            };
        }

        private UIView CreateField(string label, UIInput input)
        {
            return new UIView
            {
                ClassName = new List<string> { "player-field" },
                Children = new()
        {
            CreateText(label, "player-field-label"),
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
                ClassName = new List<string> { "player-action-btn", className },
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
                CreateText(title, "player-card-title"),
                CreateText(description, "player-card-desc")
            };

            sectionChildren.AddRange(children);

            return new UIView
            {
                ClassName = new List<string> { "player-demo-card" },
                Children = sectionChildren,
            };
        }

        private static UILabel CreateHintLabel(string text)
        {
            return CreateText(text, "player-demo-hint");
        }

        private static UILabel CreateText(string text, string className)
        {
            return new UILabel
            {
                Text = text,
                Wrap = true,
                ClassName = new List<string> { className }
            };
        }

        private static UILabel CreateFormatText(string text)
        {
            var label = CreateText(text, "player-format-list");
            label.MaxLines = 8;
            return label;
        }

        private void StartPlayback()
        {
            string? source = NormalizeSource();
            if (source == null) return;

            _player.SourceUrl = source;
            _player.Play();
            SetStatus($"正在播放：{FormatSourceForStatus(source)}");
        }

        private async void StopPlayback()
        {
            if (_recordingOperationPending) return;
            if (_player.IsRecording)
            {
                await StopRecordingInternalAsync();
            }

            _player.Stop();
            SetStatus("已停止播放");
        }

        private async void SwitchSource()
        {
            string? source = NormalizeSource();
            if (source == null) return;

            if (_recordingOperationPending) return;
            if (_player.IsRecording)
            {
                await StopRecordingInternalAsync();
            }

            _player.SwitchUrl(source);
            SetStatus($"已切换播放源：{FormatSourceForStatus(source)}");
        }

        private async void StartRecording()
        {
            if (_recordingOperationPending || _player.IsRecording) return;

            string? source = NormalizeSource();
            if (source == null) return;

            _recordingOperationPending = true;
            UpdateRecordingActionButtons();
            try
            {
                if (!_player.IsPlaying)
                {
                    _player.SourceUrl = source;
                    _player.Play();
                }

                string filePath = BuildRecordingPath();
                _currentRecordingFilePath = filePath;
                await _player.StartRecordingAsync(filePath);
                SetStatus($"正在播放并录制：{FormatSourceForStatus(source)}");
                SetRecordingResult($"录制已开始：\n{filePath}\n停止录制后会在这里显示最终文件大小。");
            }
            catch (Exception ex)
            {
                _currentRecordingFilePath = null;
                SetRecordingResult($"开始录制失败：{ex.Message}");
            }
            finally
            {
                _recordingOperationPending = false;
                UpdateRecordingActionButtons();
            }
        }

        private async void CapturePhoto()
        {
            if (_recordingOperationPending) return;

            try
            {
                string filePath = BuildPhotoPath();
                await _player.CapturePhotoAsync(filePath);
                SetRecordingResult($"拍照已保存：\n{filePath}");
                SetStatus(_player.IsPlaying ? "已保存当前播放画面" : "已保存当前静态画面");
            }
            catch (Exception ex)
            {
                SetRecordingResult($"拍照失败：{ex.Message}");
            }
        }

        private async void StopRecording()
        {
            if (_recordingOperationPending || !_player.IsRecording) return;
            await StopRecordingInternalAsync();
        }

        private async Task StopRecordingInternalAsync()
        {
            _recordingOperationPending = true;
            UpdateRecordingActionButtons();
            try
            {
                await _player.StopRecordingAsync();
                SetRecordingResult(BuildRecordingResultMessage());
                SetStatus(_player.IsPlaying ? "录制已停止，播放仍在继续" : "录制已停止");
            }
            catch (Exception ex)
            {
                SetRecordingResult($"停止录制失败：{ex.Message}");
            }
            finally
            {
                _recordingOperationPending = false;
                UpdateRecordingActionButtons();
            }
        }

        private string? NormalizeSource()
        {
            string source = _sourceInput.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(source))
            {
                SetStatus("请输入播放源后再操作");
                return null;
            }

            return source;
        }

        private void SetStatus(string text)
        {
            _statusLabel.Text = text;
            // _statusLabel.RequestRedraw();
        }

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

        private void SetRecordingResult(string text)
        {
            _recordingResultLabel.Text = text;
            _recordingResultLabel.RequestLayout();
            _recordingResultLabel.RequestRedraw();
        }

        private void UpdateRecordingActionButtons()
        {
            if (_startRecordingButton != null)
            {
                _startRecordingButton.Disabled = _recordingOperationPending || _player.IsRecording;
                _startRecordingButton.RequestRedraw();
            }

            if (_stopRecordingButton != null)
            {
                _stopRecordingButton.Disabled = _recordingOperationPending || !_player.IsRecording;
                _stopRecordingButton.RequestRedraw();
            }
        }

        private string BuildRecordingResultMessage()
        {
            string? filePath = _currentRecordingFilePath;
            _currentRecordingFilePath = null;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return "录制已停止。未记录到本次输出路径，请查看视频目录下的 TCYM.UI.Example/Player。";
            }

            if (!File.Exists(filePath))
            {
                return $"录制已停止，但当前没有找到输出文件：\n{filePath}";
            }

            long size = new FileInfo(filePath).Length;
            return $"录制已保存：\n{filePath}\n文件大小：{FormatFileSize(size)}";
        }

        private static string BuildRecordingPath()
        {
            string root = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            if (string.IsNullOrWhiteSpace(root))
            {
                root = AppContext.BaseDirectory;
            }

            string directory = Path.Combine(root, "TCYM.UI.Example", "Player");
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, $"player_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");
        }

        private static string BuildPhotoPath()
        {
            string root = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (string.IsNullOrWhiteSpace(root))
            {
                root = AppContext.BaseDirectory;
            }

            string directory = Path.Combine(root, "TCYM.UI.Example", "Player");
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, $"player_{DateTime.Now:yyyyMMdd_HHmmss}.png");
        }

        private static string FormatFileSize(long size)
        {
            if (size < 1024) return $"{size} B";
            if (size < 1024 * 1024) return $"{size / 1024.0:F1} KB";
            if (size < 1024L * 1024 * 1024) return $"{size / 1024.0 / 1024.0:F2} MB";
            return $"{size / 1024.0 / 1024.0 / 1024.0:F2} GB";
        }
    }
}