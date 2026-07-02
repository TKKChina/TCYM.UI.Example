using System.Globalization;
using System.IO;
using SkiaSharp;
using TCYM.FFmpeg.MVS;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Input;
using TCYM.UI.Elements.Select;
using TCYM.UI.Pro.Elements.MVS;
using TCYM.UI.Pro.Elements.Player;

namespace TCYM.UI.Example.Page.component.MvGigECamera
{
    internal class UIMvGigECameraDemo : UIScrollView, IUIRouteLifecycle
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.MvGigECamera.style.css";
        private const string ConnectionModeSerialValue = "serial";
        private const string ConnectionModeIpValue = "ip";

        private readonly UIMvGigECamera _camera;
        private readonly UISelect _deviceSelect;
        private readonly UISelect _connectionModeSelect;
        private readonly UIInput _watermarkInput;
        private readonly UIInput _exposureInput;
        private readonly UILabel _deviceLabel;
        private readonly UILabel _statusLabel;
        private readonly UILabel _exposureLabel;
        private readonly UILabel _resultLabel;
        private readonly object _deviceLoadSync = new();

        private DemoDeviceOption[] _devices;
        private string? _deviceEnumerationError;
        private DemoDeviceOption[]? _pendingDevices;
        private string? _pendingDeviceEnumerationError;
        private bool _deviceLoadInProgress;
        private bool _deviceLoadCompleted;
        private bool _hasPendingDeviceLoadResult;

        private UIButton? _startRecordingButton;
        private UIButton? _stopRecordingButton;
        private bool _recordingOperationPending;
        private string? _currentRecordingFilePath;
        private string? _selectedDeviceKey;
        private DeviceConnectionMode _connectionMode;

        private bool IsDeviceListLoading => _deviceLoadInProgress && !_deviceLoadCompleted;

        private enum DeviceConnectionMode
        {
            SerialNumber,
            DeviceIp,
        }

        internal UIMvGigECameraDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            EnableDeferredChildLoading = false;

            _devices = Array.Empty<DemoDeviceOption>();
            _connectionMode = DeviceConnectionMode.SerialNumber;

            _watermarkInput = CreateInput("TCYM GigE", "请输入预览水印文本");
            _exposureInput = CreateInput("15000", "请输入手动曝光时间（微秒）");
            _connectionModeSelect = CreateConnectionModeSelect(ConnectionModeSerialValue);
            _deviceSelect = CreateDeviceSelect(null);
            _deviceLabel = CreateText("正在检测可访问的 GigE 相机，请稍候。", "mvs-camera-status-label");
            _statusLabel = CreateText("设备列表加载中。", "mvs-camera-status-label");
            _exposureLabel = CreateText("曝光信息将在打开相机后显示。", "mvs-camera-status-label");
            _resultLabel = CreateText("尚未执行操作。", "mvs-camera-result");
            _resultLabel.MaxLines = 8;

            _camera = new UIMvGigECamera
            {
                AutoPlay = false,
                MaintainAspectRatio = true,
                //PreferredDeviceIp = "192.168.0.234",
                Watermarks = new()
                 {
                    new WatermarkSpec
                    {
                        TextProvider = () => $"{_watermarkInput.Text }\r\nTCYM 测试"?? string.Empty,
                        FontSize = 30,
                        Color = SKColors.White,
                        Position = new SKPoint(30, 60),
                    }
                 },
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = "100%",
                }
            };

            ApplyPreferredDevice(null);

            ClassName = new List<string> { "mvs-camera-demo-view" };
            Children = new()
            {
                CreateText("MvGigE 工业相机", "mvs-camera-demo-title"),
                CreateText("基于 TCYM.UI.Pro.Elements.MVS.UIMvGigECamera 的预览、拍照、录像与曝光控制示例。", "mvs-camera-demo-title-sub"),
                CreateText("该页面依赖 Windows 以及可访问的 GigE Vision 相机。预览、水印、拍照和录像结果保持一致。", "mvs-camera-demo-desc"),
                CreateDeviceSection(),
                CreateExposureSection(),
                CreateControlSection(),
                CreatePreviewSection(),
            };

            RefreshDeviceInfo();
            RefreshStatus();
            RefreshExposureInfo();
            SetResult("页面已打开，正在后台加载 GigE 相机列表。拍照默认保存到图片目录，录像默认保存到视频目录。", 6);
        }

        public void OnRouteEnter(string? fromPath)
        {
            EnsureDevicesLoaded();
            RefreshDeviceInfo();
            RefreshStatus();
            RefreshExposureInfo();
        }

        public override void Update(float deltaSeconds)
        {
            base.Update(deltaSeconds);
            ApplyPendingDeviceLoadResult();
        }

        public void OnRouteLeave(string? toPath)
        {
            bool wasRecording = _camera.IsRecording;
            try
            {
                _camera.Stop();
            }
            catch
            {
            }

            if (wasRecording)
            {
                SetResult("已离开 MvGigE 相机示例页，录像与预览已自动停止。", 6);
            }

            _currentRecordingFilePath = null;
            RefreshStatus();
            RefreshExposureInfo();
        }

        private void EnsureDevicesLoaded()
        {
            lock (_deviceLoadSync)
            {
                if (_deviceLoadCompleted || _deviceLoadInProgress)
                {
                    return;
                }

                _deviceLoadInProgress = true;
            }

            RefreshDeviceInfo();
            RefreshStatus();

            _ = Task.Run(() =>
            {
                (DemoDeviceOption[] devices, string? errorMessage) = LoadDevices();
                lock (_deviceLoadSync)
                {
                    _pendingDevices = devices;
                    _pendingDeviceEnumerationError = errorMessage;
                    _hasPendingDeviceLoadResult = true;
                }

                try
                {
                    UISystem.Manager?.RequestRedraw();
                }
                catch
                {
                }
            });
        }

        private void ApplyPendingDeviceLoadResult()
        {
            DemoDeviceOption[] devices;
            string? errorMessage;

            lock (_deviceLoadSync)
            {
                if (!_hasPendingDeviceLoadResult)
                {
                    return;
                }

                devices = _pendingDevices ?? Array.Empty<DemoDeviceOption>();
                errorMessage = _pendingDeviceEnumerationError;
                _pendingDevices = null;
                _pendingDeviceEnumerationError = null;
                _hasPendingDeviceLoadResult = false;
                _deviceLoadInProgress = false;
                _deviceLoadCompleted = true;
            }

            ApplyLoadedDevices(devices, errorMessage);
        }

        private void ApplyLoadedDevices(DemoDeviceOption[] devices, string? errorMessage)
        {
            _devices = devices ?? Array.Empty<DemoDeviceOption>();
            _deviceEnumerationError = errorMessage;

            DemoDeviceOption? selectedDevice = null;
            if (TryGetDeviceByKey(_selectedDeviceKey, out DemoDeviceOption existingDevice))
            {
                selectedDevice = existingDevice;
            }
            else
            {
                selectedDevice = _devices.FirstOrDefault();
                _selectedDeviceKey = selectedDevice?.Key;
            }

            _connectionMode = ResolveInitialConnectionMode(selectedDevice);
            _connectionModeSelect.SelectedValue = GetConnectionModeValue(_connectionMode);
            _deviceSelect.Options = _devices
                .Select(device => new SelectOption(device.Key, device.DisplayName))
                .ToList();
            _deviceSelect.SelectedValue = _selectedDeviceKey;
            _deviceSelect.Placeholder = _devices.Length == 0 ? "当前没有可用设备" : "请选择 GigE 相机";
            _deviceSelect.ShowSearch = _devices.Length > 6;

            ApplyPreferredDevice(selectedDevice);
            RefreshDeviceInfo();
            RefreshStatus();
            RefreshExposureInfo();

            if (!string.IsNullOrWhiteSpace(_deviceEnumerationError))
            {
                SetResult($"当前无法枚举 GigE 相机：{_deviceEnumerationError}", 8);
            }
            else if (_devices.Length == 0)
            {
                SetResult("未检测到可访问的 GigE 相机。请确认设备已连接、同网段且当前账号具备访问权限。", 6);
            }
            else
            {
                SetResult("设备列表已加载完成。请选择设备并点击“打开预览”。", 4);
            }
        }

        private UIView CreateDeviceSection()
        {
            return CreateSectionCard(
                "设备与水印",
                "可在当前可访问的 GigE 相机中切换目标设备，并在序列号/IP 两种匹配方式之间切换；设备列表会直接显示 IP。",
                CreateField("连接方式", _connectionModeSelect),
                CreateField("设备列表", _deviceSelect),
                CreateField("水印文本", _watermarkInput),
                _deviceLabel,
                CreateHintLabel("切换设备或连接方式时，如当前正在录像，会先结束当前录像并重新绑定选中的相机。")
            );
        }

        private UIView CreateExposureSection()
        {
            return CreateSectionCard(
                "曝光控制",
                "支持查看当前曝光状态、切换自动曝光模式，以及在关闭自动曝光后应用手动曝光时间。单位为微秒。",
                CreateField("手动曝光（us）", _exposureInput),
                CreateButtonRow(
                    CreateButton("刷新曝光", RefreshExposureButton, "mvs-camera-btn-neutral"),
                    CreateButton("自动关闭", () => SetExposureMode(MvGigECameraExposureAutoMode.Off, "已切换到手动曝光模式。"), "mvs-camera-btn-secondary"),
                    CreateButton("单次自动", () => SetExposureMode(MvGigECameraExposureAutoMode.Once, "已触发一次自动曝光。"), "mvs-camera-btn-secondary"),
                    CreateButton("连续自动", () => SetExposureMode(MvGigECameraExposureAutoMode.Continuous, "已切换到连续自动曝光。"), "mvs-camera-btn-secondary")
                ),
                CreateButtonRow(
                    CreateButton("应用手动曝光", ApplyManualExposure, "mvs-camera-btn-primary")
                ),
                _exposureLabel
            );
        }

        private UIView CreateControlSection()
        {
            return CreateSectionCard(
                "操作面板",
                "手动打开或关闭预览，拍照保存 PNG，录像输出 MP4。录像期间会继续保持实时预览。",
                CreateButtonRow(
                    CreateButton("打开预览", OpenPreview, "mvs-camera-btn-primary"),
                    CreateButton("关闭预览", ClosePreview, "mvs-camera-btn-neutral"),
                    CreateButton("重新打开", RestartPreview, "mvs-camera-btn-secondary"),
                    CreateButton("拍照保存", CapturePhoto, "mvs-camera-btn-secondary")
                ),
                CreateRecordingToolbar(),
                _statusLabel,
                _resultLabel
            );
        }

        private UIView CreatePreviewSection()
        {
            return CreateSectionCard(
                "预览画面",
                "控件通过 Skia 渲染最新帧。若相机未打开或当前设备不可用，会在这里显示占位状态文本。",
                new UIView
                {
                    ClassName = new List<string> { "mvs-camera-preview-shell" },
                    Children = new()
                    {
                        _camera
                    }
                }
            );
        }

        private UIView CreateRecordingToolbar()
        {
            _startRecordingButton = CreateButton("开始录像", StartRecording, "mvs-camera-btn-record");
            _stopRecordingButton = CreateButton("停止录像", StopRecording, "mvs-camera-btn-neutral");
            UpdateRecordingActionButtons();

            return CreateButtonRow(_startRecordingButton, _stopRecordingButton);
        }

        private UISelect CreateDeviceSelect(string? defaultKey)
        {
            return new UISelect
            {
                Options = _devices
                    .Select(device => new SelectOption(device.Key, device.DisplayName))
                    .ToList(),
                SelectedValue = defaultKey,
                Placeholder = _devices.Length == 0 ? "当前没有可用设备" : "请选择 GigE 相机",
                ShowSearch = _devices.Length > 6,
                ClassName = new List<string> { "mvs-camera-select" },
                Style = new UpdateUIStyle
                {
                    Width = 300,
                    Height = 34,
                },
                OnSelect = option => SelectDeviceByKey(option.Value?.ToString())
            };
        }

        private UISelect CreateConnectionModeSelect(string defaultValue)
        {
            return new UISelect
            {
                Options = new List<SelectOption>
                {
                    new(ConnectionModeSerialValue, "按序列号匹配"),
                    new(ConnectionModeIpValue, "按 IP 匹配"),
                },
                SelectedValue = defaultValue,
                ShowSearch = false,
                ClassName = new List<string> { "mvs-camera-select" },
                Style = new UpdateUIStyle
                {
                    Height = 34,
                },
                OnSelect = option => ChangeConnectionMode(option.Value?.ToString())
            };
        }

        private static UIInput CreateInput(string text, string placeholder)
        {
            return new UIInput
            {
                Text = text,
                Placeholder = placeholder,
                AllowClear = true,
                ClassName = new List<string> { "mvs-camera-input" },
            };
        }

        private static UIView CreateSectionCard(string title, string description, params UIElement[] children)
        {
            var sectionChildren = new List<UIElement>
            {
                CreateText(title, "mvs-camera-card-title"),
                CreateText(description, "mvs-camera-card-desc")
            };

            sectionChildren.AddRange(children);

            return new UIView
            {
                ClassName = new List<string> { "mvs-camera-demo-card" },
                Children = sectionChildren,
            };
        }

        private static UIView CreateField(string label, UIElement input)
        {
            return new UIView
            {
                ClassName = new List<string> { "mvs-camera-field" },
                Children = new()
                {
                    CreateText(label, "mvs-camera-field-label"),
                    input
                }
            };
        }

        private static UIView CreateButtonRow(params UIElement[] buttons)
        {
            return new UIView
            {
                ClassName = new List<string> { "mvs-camera-button-row" },
                Children = buttons.ToList()
            };
        }

        private static UIButton CreateButton(string text, Action action, string className)
        {
            return new UIButton
            {
                Text = text,
                ClassName = new List<string> { "mvs-camera-action-btn", className },
                CornerCurve = "continuous",
                Events = new()
                {
                    Click = _ => action()
                }
            };
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

        private static UILabel CreateHintLabel(string text)
        {
            return CreateText(text, "mvs-camera-demo-hint");
        }

        private void OpenPreview()
        {
            EnsureDevicesLoaded();
            if (IsDeviceListLoading)
            {
                RefreshStatus();
                SetResult("正在后台枚举 GigE 相机，请稍后再打开预览。", 4);
                return;
            }

            if (_devices.Length == 0)
            {
                SetResult(string.IsNullOrWhiteSpace(_deviceEnumerationError) ? "当前没有可访问的 GigE 相机。" : _deviceEnumerationError, 8);
                return;
            }

            try
            {
                _camera.Play();
                RefreshStatus();
                RefreshExposureInfo();
                SetResult("已请求打开 GigE 相机预览。若设备与网络配置正常，画面会开始显示。", 6);
            }
            catch (Exception ex)
            {
                RefreshStatus();
                RefreshExposureInfo();
                SetResult($"打开预览失败：{ex.Message}", 8);
            }
        }

        private void ClosePreview()
        {
            bool wasRecording = _camera.IsRecording;
            try
            {
                _camera.Stop();
                RefreshStatus();
                RefreshExposureInfo();
                SetResult(wasRecording ? BuildRecordingResultMessage() : "GigE 相机预览已停止。", 6);
            }
            catch (Exception ex)
            {
                RefreshStatus();
                RefreshExposureInfo();
                SetResult($"关闭预览失败：{ex.Message}", 8);
            }
        }

        private void RestartPreview()
        {
            if (_devices.Length == 0)
            {
                SetResult("当前没有可重新打开的 GigE 相机。", 6);
                return;
            }

            try
            {
                _camera.Restart();
                RefreshStatus();
                RefreshExposureInfo();
                SetResult("已重新初始化 GigE 相机连接。", 4);
            }
            catch (Exception ex)
            {
                RefreshStatus();
                RefreshExposureInfo();
                SetResult($"重新打开失败：{ex.Message}", 8);
            }
        }

        private void CapturePhoto()
        {
            try
            {
                string filePath = BuildPhotoPath();
                _camera.CapturePhoto(filePath);
                RefreshStatus();
                SetResult($"照片已保存：\n{filePath}", 6);
            }
            catch (Exception ex)
            {
                RefreshStatus();
                SetResult($"拍照失败：{ex.Message}", 8);
            }
        }

        private async void StartRecording()
        {
            if (_recordingOperationPending || _camera.IsRecording)
            {
                return;
            }

            _recordingOperationPending = true;
            UpdateRecordingActionButtons();
            try
            {
                if (!_camera.IsPlaying)
                {
                    _camera.Play();
                }

                string filePath = BuildRecordingPath();
                _currentRecordingFilePath = filePath;
                await _camera.StartRecordingAsync(filePath).ConfigureAwait(false);
                SetResult($"录像已开始：\n{filePath}\n停止录像后会在这里显示最终文件大小。", 6);
            }
            catch (Exception ex)
            {
                _currentRecordingFilePath = null;
                SetResult($"开始录像失败：{ex.Message}", 8);
            }
            finally
            {
                _recordingOperationPending = false;
                RefreshStatus();
            }
        }

        private async void StopRecording()
        {
            if (_recordingOperationPending || !_camera.IsRecording)
            {
                return;
            }

            _recordingOperationPending = true;
            UpdateRecordingActionButtons();
            try
            {
                await _camera.StopRecordingAsync().ConfigureAwait(false);
                SetResult(BuildRecordingResultMessage(), 6);
            }
            catch (Exception ex)
            {
                SetResult($"停止录像失败：{ex.Message}", 8);
            }
            finally
            {
                _recordingOperationPending = false;
                RefreshStatus();
            }
        }

        private void RefreshExposureButton()
        {
            RefreshExposureInfo();
        }

        private void SetExposureMode(MvGigECameraExposureAutoMode autoMode, string successMessage)
        {
            try
            {
                _camera.SetExposureAutoMode(autoMode);
                RefreshExposureInfo();
                SetResult(successMessage, 5);
            }
            catch (Exception ex)
            {
                RefreshExposureInfo();
                SetResult($"切换曝光模式失败：{ex.Message}", 8);
            }
        }

        private void ApplyManualExposure()
        {
            if (!TryParseExposure(out float exposureTime, out string errorMessage))
            {
                SetResult(errorMessage, 6);
                return;
            }

            try
            {
                _camera.SetExposureAutoMode(MvGigECameraExposureAutoMode.Off);
                _camera.SetExposureTime(exposureTime);
                RefreshExposureInfo();
                SetResult($"已应用手动曝光：{exposureTime:F2} us", 5);
            }
            catch (Exception ex)
            {
                RefreshExposureInfo();
                SetResult($"应用手动曝光失败：{ex.Message}", 8);
            }
        }

        private bool TryParseExposure(out float exposureTime, out string errorMessage)
        {
            string raw = _exposureInput.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(raw))
            {
                exposureTime = 0;
                errorMessage = "请输入手动曝光时间。";
                return false;
            }

            if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out exposureTime)
                || float.TryParse(raw, NumberStyles.Float, CultureInfo.CurrentCulture, out exposureTime))
            {
                errorMessage = string.Empty;
                return true;
            }

            errorMessage = "曝光时间格式无效，请输入数字。";
            return false;
        }

        private void SelectDeviceByKey(string? deviceKey)
        {
            if (string.IsNullOrWhiteSpace(deviceKey))
            {
                return;
            }

            if (!TryGetDeviceByKey(deviceKey, out DemoDeviceOption selectedDevice))
            {
                return;
            }

            if (string.Equals(_selectedDeviceKey, selectedDevice.Key, StringComparison.Ordinal))
            {
                RefreshDeviceInfo();
                return;
            }

            bool reopen = _camera.IsPlaying;
            bool wasRecording = _camera.IsRecording;
            if (reopen || wasRecording)
            {
                try
                {
                    _camera.Stop();
                }
                catch
                {
                }
            }

            _currentRecordingFilePath = null;
            _selectedDeviceKey = selectedDevice.Key;
            ApplyPreferredDevice(selectedDevice);
            _deviceSelect.SelectedValue = _selectedDeviceKey;
            RefreshDeviceInfo();

            if (reopen)
            {
                try
                {
                    _camera.Play();
                    SetResult(
                        wasRecording
                            ? $"已停止当前录像并切换到设备：{selectedDevice.DisplayName}"
                            : $"已切换到设备：{selectedDevice.DisplayName}",
                        6);
                }
                catch (Exception ex)
                {
                    SetResult($"切换设备后重新打开失败：{ex.Message}", 8);
                }
            }
            else
            {
                SetResult($"已切换到设备：{selectedDevice.DisplayName}", 4);
            }

            RefreshStatus();
            RefreshExposureInfo();
        }

        private bool TryGetDeviceByKey(string? deviceKey, out DemoDeviceOption device)
        {
            foreach (DemoDeviceOption candidate in _devices)
            {
                if (string.Equals(candidate.Key, deviceKey, StringComparison.OrdinalIgnoreCase))
                {
                    device = candidate;
                    return true;
                }
            }

            device = default!;
            return false;
        }

        private void ChangeConnectionMode(string? connectionModeValue)
        {
            DeviceConnectionMode nextMode = ParseConnectionMode(connectionModeValue);
            if (_connectionMode == nextMode)
            {
                RefreshDeviceInfo();
                RefreshStatus();
                return;
            }

            bool reopen = _camera.IsPlaying;
            bool wasRecording = _camera.IsRecording;
            if (reopen || wasRecording)
            {
                try
                {
                    _camera.Stop();
                }
                catch
                {
                }
            }

            _currentRecordingFilePath = null;
            _connectionMode = nextMode;
            _connectionModeSelect.SelectedValue = GetConnectionModeValue(_connectionMode);

            DemoDeviceOption? selectedDevice = TryGetSelectedDevice(out DemoDeviceOption device) ? device : null;
            ApplyPreferredDevice(selectedDevice);
            RefreshDeviceInfo();

            if (reopen)
            {
                try
                {
                    _camera.Play();
                    SetResult(
                        wasRecording
                            ? $"已停止当前录像，并切换为{BuildBindingDisplayText(selectedDevice)}。"
                            : $"已切换为{BuildBindingDisplayText(selectedDevice)}。",
                        6);
                }
                catch (Exception ex)
                {
                    SetResult($"切换连接方式后重新打开失败：{ex.Message}", 8);
                }
            }
            else
            {
                SetResult($"已切换为{BuildBindingDisplayText(selectedDevice)}。", 4);
            }

            RefreshStatus();
            RefreshExposureInfo();
        }

        private bool TryGetSelectedDevice(out DemoDeviceOption device)
        {
            return TryGetDeviceByKey(_selectedDeviceKey, out device);
        }

        private void ApplyPreferredDevice(DemoDeviceOption? device)
        {
            _camera.PreferredSerialNumber = null;
            _camera.PreferredDeviceIp = null;
            _camera.PreferredDeviceName = null;

            if (device == null)
            {
                return;
            }

            if (_connectionMode == DeviceConnectionMode.DeviceIp)
            {
                if (!string.IsNullOrWhiteSpace(device.DeviceIp))
                {
                    _camera.PreferredDeviceIp = device.DeviceIp;
                    return;
                }

                if (!string.IsNullOrWhiteSpace(device.SerialNumber))
                {
                    _camera.PreferredSerialNumber = device.SerialNumber;
                    return;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(device.SerialNumber))
                {
                    _camera.PreferredSerialNumber = device.SerialNumber;
                    return;
                }

                if (!string.IsNullOrWhiteSpace(device.DeviceIp))
                {
                    _camera.PreferredDeviceIp = device.DeviceIp;
                    return;
                }
            }

            _camera.PreferredDeviceName = string.IsNullOrWhiteSpace(device.PreferredName)
                ? device.DisplayName
                : device.PreferredName;
        }

        private void RefreshDeviceInfo()
        {
            if (IsDeviceListLoading && _devices.Length == 0)
            {
                _deviceLabel.Text = "正在检测可访问的 GigE 相机，请稍候。\n页面主体会先显示，设备列表加载完成后会自动刷新。";
            }
            else if (_devices.Length == 0)
            {
                _deviceLabel.Text = string.IsNullOrWhiteSpace(_deviceEnumerationError)
                    ? "当前未检测到可访问的 GigE 相机。"
                    : $"当前未检测到可访问的 GigE 相机。\n{_deviceEnumerationError}";
            }
            else if (TryGetDeviceByKey(_selectedDeviceKey, out DemoDeviceOption selectedDevice))
            {
                _deviceLabel.Text = $"当前绑定：{selectedDevice.DisplayName}\n{BuildBindingDisplayText(selectedDevice)}\n{selectedDevice.Summary}\n设备总数：{_devices.Length}";
            }
            else
            {
                _deviceLabel.Text = $"当前设备选择无效。\n检测到设备：{string.Join(" / ", _devices.Select(device => device.DisplayName))}\n设备总数：{_devices.Length}";
            }

            _deviceLabel.RequestLayout();
            _deviceLabel.RequestRedraw();
        }

        private void RefreshStatus()
        {
            string playingText = IsDeviceListLoading && !_camera.IsPlaying
                ? "设备列表加载中"
                : _camera.IsPlaying
                ? (_camera.IsRecording ? "正在预览并录像" : "正在预览")
                : "待预览";
            DemoDeviceOption? selectedDevice = TryGetSelectedDevice(out DemoDeviceOption device) ? device : null;
            string deviceText = BuildStatusDeviceText(_camera.CurrentDevice, selectedDevice);
            string frameText = _camera.LatestFrameInfo == null
                ? "尚未收到图像帧"
                : $"最新帧：{_camera.LatestFrameInfo.Width}x{_camera.LatestFrameInfo.Height} / Frame #{_camera.LatestFrameInfo.FrameNumber}";

            _statusLabel.Text = $"当前状态：{playingText}\n{BuildBindingDisplayText(selectedDevice)}\n当前设备：{deviceText}\n{frameText}";
            _statusLabel.RequestLayout();
            _statusLabel.RequestRedraw();
            UpdateRecordingActionButtons();
        }

        private void RefreshExposureInfo()
        {
            try
            {
                var settings = _camera.GetExposureSettings();
                _exposureInput.Text = settings.ExposureTime.ToString("F2", CultureInfo.InvariantCulture);
                _exposureLabel.Text = $"模式：{FormatExposureAutoMode(settings.AutoMode)}\n当前曝光：{settings.ExposureTime:F2} us\n范围：{settings.MinExposureTime:F2} - {settings.MaxExposureTime:F2} us";
            }
            catch (Exception ex)
            {
                _exposureLabel.Text = $"曝光信息不可用：{ex.Message}";
            }

            _exposureLabel.RequestLayout();
            _exposureLabel.RequestRedraw();
        }

        private void UpdateRecordingActionButtons()
        {
            if (_startRecordingButton != null)
            {
                _startRecordingButton.Disabled = _recordingOperationPending || _camera.IsRecording || _devices.Length == 0 || IsDeviceListLoading;
                _startRecordingButton.RequestRedraw();
            }

            if (_stopRecordingButton != null)
            {
                _stopRecordingButton.Disabled = _recordingOperationPending || !_camera.IsRecording;
                _stopRecordingButton.RequestRedraw();
            }
        }

        private void SetResult(string message, int maxLines)
        {
            _resultLabel.MaxLines = maxLines;
            _resultLabel.Text = message;
            _resultLabel.RequestLayout();
            _resultLabel.RequestRedraw();
        }

        private string BuildRecordingResultMessage()
        {
            string? filePath = _currentRecordingFilePath;
            _currentRecordingFilePath = null;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return "录像已停止。\n未记录到本次输出路径，请查看视频目录下的 TCYM.UI.Example/MvGigECamera。";
            }

            if (!File.Exists(filePath))
            {
                return $"录像已停止，但当前没有找到输出文件：\n{filePath}";
            }

            long size = new FileInfo(filePath).Length;
            return $"录像已保存：\n{filePath}\n文件大小：{FormatFileSize(size)}";
        }

        private static string FormatFileSize(long size)
        {
            if (size < 1024) return $"{size} B";
            if (size < 1024 * 1024) return $"{size / 1024.0:F1} KB";
            if (size < 1024L * 1024 * 1024) return $"{size / 1024.0 / 1024.0:F2} MB";
            return $"{size / 1024.0 / 1024.0 / 1024.0:F2} GB";
        }

        private static string BuildPhotoPath()
        {
            string directory = GetOutputDirectory(Environment.SpecialFolder.MyPictures);
            return Path.Combine(directory, $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.png");
        }

        private static string BuildRecordingPath()
        {
            string directory = GetOutputDirectory(Environment.SpecialFolder.MyVideos);
            return Path.Combine(directory, $"record_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");
        }

        private static string GetOutputDirectory(Environment.SpecialFolder folder)
        {
            string root = Environment.GetFolderPath(folder);
            if (string.IsNullOrWhiteSpace(root))
            {
                root = AppContext.BaseDirectory;
            }

            string directory = Path.Combine(root, "TCYM.UI.Example", "MvGigECamera");
            Directory.CreateDirectory(directory);
            return directory;
        }

        private static string FormatExposureAutoMode(MvGigECameraExposureAutoMode autoMode)
        {
            return autoMode switch
            {
                MvGigECameraExposureAutoMode.Off => "手动",
                MvGigECameraExposureAutoMode.Once => "单次自动",
                MvGigECameraExposureAutoMode.Continuous => "连续自动",
                _ => autoMode.ToString(),
            };
        }

        private static (DemoDeviceOption[] devices, string? errorMessage) LoadDevices()
        {
            if (!OperatingSystem.IsWindows())
            {
                return (Array.Empty<DemoDeviceOption>(), "当前运行环境不是 Windows，GigE 相机示例不可用。");
            }

            try
            {
                DemoDeviceOption[] devices = UIMvGigECamera
                    .EnumerateDevices(onlyAccessible: true)
                    .Select(device => new DemoDeviceOption(
                        BuildDeviceKey(device.SerialNumber, device.CurrentIp, device.UserDefinedName, device.ModelName),
                        BuildDeviceDisplayName(device.UserDefinedName, device.ModelName, device.CurrentIp, device.SerialNumber),
                        BuildPreferredDeviceName(device.UserDefinedName, device.ModelName),
                        device.SerialNumber ?? string.Empty,
                        device.CurrentIp ?? string.Empty,
                        $"型号：{(string.IsNullOrWhiteSpace(device.ModelName) ? "未知" : device.ModelName)}\nIP：{(string.IsNullOrWhiteSpace(device.CurrentIp) ? "未知" : device.CurrentIp)}\n序列号：{(string.IsNullOrWhiteSpace(device.SerialNumber) ? "未知" : device.SerialNumber)}"))
                    .ToArray();
                return (devices, null);
            }
            catch (Exception ex)
            {
                return (Array.Empty<DemoDeviceOption>(), ex.Message);
            }
        }

        private static string BuildDeviceKey(string? serialNumber, string? deviceIp, string? userDefinedName, string? modelName)
        {
            if (!string.IsNullOrWhiteSpace(serialNumber))
            {
                return $"sn:{serialNumber}";
            }

            if (!string.IsNullOrWhiteSpace(deviceIp))
            {
                return $"ip:{deviceIp}";
            }

            if (!string.IsNullOrWhiteSpace(userDefinedName))
            {
                return $"name:{userDefinedName}";
            }

            return $"model:{modelName}";
        }

        private static string BuildDeviceDisplayName(string? userDefinedName, string? modelName, string? deviceIp, string? serialNumber)
        {
            string primaryName = BuildPreferredDeviceName(userDefinedName, modelName);
            if (!string.IsNullOrWhiteSpace(primaryName))
            {
                if (!string.IsNullOrWhiteSpace(deviceIp) && !string.Equals(primaryName, deviceIp, StringComparison.OrdinalIgnoreCase))
                {
                    return $"{primaryName} ({deviceIp})";
                }

                return primaryName;
            }

            if (!string.IsNullOrWhiteSpace(deviceIp))
            {
                return deviceIp;
            }

            if (!string.IsNullOrWhiteSpace(serialNumber))
            {
                return serialNumber;
            }

            return "GigE 相机";
        }

        private static string BuildPreferredDeviceName(string? userDefinedName, string? modelName)
        {
            if (!string.IsNullOrWhiteSpace(userDefinedName))
            {
                return userDefinedName;
            }

            if (!string.IsNullOrWhiteSpace(modelName))
            {
                return modelName;
            }

            return string.Empty;
        }

        private static DeviceConnectionMode ResolveInitialConnectionMode(DemoDeviceOption? device)
        {
            if (device != null)
            {
                if (!string.IsNullOrWhiteSpace(device.SerialNumber))
                {
                    return DeviceConnectionMode.SerialNumber;
                }

                if (!string.IsNullOrWhiteSpace(device.DeviceIp))
                {
                    return DeviceConnectionMode.DeviceIp;
                }
            }

            return DeviceConnectionMode.SerialNumber;
        }

        private static DeviceConnectionMode ParseConnectionMode(string? connectionModeValue)
        {
            return string.Equals(connectionModeValue, ConnectionModeIpValue, StringComparison.OrdinalIgnoreCase)
                ? DeviceConnectionMode.DeviceIp
                : DeviceConnectionMode.SerialNumber;
        }

        private static string GetConnectionModeValue(DeviceConnectionMode connectionMode)
        {
            return connectionMode == DeviceConnectionMode.DeviceIp
                ? ConnectionModeIpValue
                : ConnectionModeSerialValue;
        }

        private static string GetConnectionModeLabel(DeviceConnectionMode connectionMode)
        {
            return connectionMode == DeviceConnectionMode.DeviceIp ? "按 IP 匹配" : "按序列号匹配";
        }

        private string BuildBindingDisplayText(DemoDeviceOption? device)
        {
            if (device == null)
            {
                return $"连接方式：{GetConnectionModeLabel(_connectionMode)}";
            }

            if (_connectionMode == DeviceConnectionMode.DeviceIp)
            {
                if (!string.IsNullOrWhiteSpace(device.DeviceIp))
                {
                    return $"连接方式：按 IP 匹配（{device.DeviceIp}）";
                }

                if (!string.IsNullOrWhiteSpace(device.SerialNumber))
                {
                    return $"连接方式：按 IP 匹配（当前回退到序列号 {device.SerialNumber}）";
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(device.SerialNumber))
                {
                    return $"连接方式：按序列号匹配（{device.SerialNumber}）";
                }

                if (!string.IsNullOrWhiteSpace(device.DeviceIp))
                {
                    return $"连接方式：按序列号匹配（当前回退到 IP {device.DeviceIp}）";
                }
            }

            if (!string.IsNullOrWhiteSpace(device.PreferredName))
            {
                return $"连接方式：按名称匹配（{device.PreferredName}）";
            }

            return $"连接方式：{GetConnectionModeLabel(_connectionMode)}";
        }

        private static string BuildStatusDeviceText(MvGigECameraDeviceInfo? currentDevice, DemoDeviceOption? selectedDevice)
        {
            if (currentDevice != null)
            {
                return BuildDeviceDisplayName(currentDevice.UserDefinedName, currentDevice.ModelName, currentDevice.CurrentIp, currentDevice.SerialNumber);
            }

            return selectedDevice?.DisplayName ?? "未选择设备";
        }

        private sealed record DemoDeviceOption(string Key, string DisplayName, string PreferredName, string SerialNumber, string DeviceIp, string Summary);
    }
}