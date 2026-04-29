using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Select;
using TCYM.UI.Elements.UsbCamera;
using TCYM.UI.Enums;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.UsbCamera
{
  /// <summary>
  /// USB 摄像头组件示例页，演示预览、拍照和录像这三个主要能力。
  /// </summary>
  internal class UIUsbCameraDemo : UIScrollView, IUIRouteLifecycle
  {
    /// <summary>
    /// 实际执行摄像头预览和媒体操作的核心控件。
    /// </summary>
    private readonly UICameraPreview _preview;

    /// <summary>
    /// 用于展示当前检测到的摄像头设备信息。
    /// </summary>
    private readonly UILabel _deviceLabel;

    /// <summary>
    /// 用于切换当前示例页绑定的摄像头设备。
    /// </summary>
    private readonly UISelect _deviceSelect;

    /// <summary>
    /// 用于展示摄像头当前运行状态。
    /// </summary>
    private readonly UILabel _statusLabel;

    /// <summary>
    /// 用于展示拍照、录像等操作结果。
    /// </summary>
    private readonly UILabel _resultLabel;

    /// <summary>
    /// 当前系统检测到的摄像头设备列表。
    /// </summary>
    private readonly UICameraDeviceInfo[] _devices;

    /// <summary>
    /// 记录当前录像输出路径，停止录像时用于回显结果。
    /// </summary>
    private string? _currentRecordingFilePath;

    /// <summary>
    /// 当前绑定到预览控件的摄像头稳定标识。
    /// </summary>
    private string? _selectedDeviceId;

    /// <summary>
    /// 录像开始按钮；录像进行中或录像操作未完成时会被禁用。
    /// </summary>
    private UIButton? _startRecordingButton;

    /// <summary>
    /// 停止录像按钮；未录像或录像操作未完成时会被禁用。
    /// </summary>
    private UIButton? _stopRecordingButton;

    /// <summary>
    /// 标记当前是否有录像开始/停止操作尚未完成，用于阻止重复点击。
    /// </summary>
    private bool _recordingOperationPending;

    /// <summary>
    /// 初始化 USB 摄像头示例页，并构建页面上的信息区、预览区和操作区。
    /// </summary>
    internal UIUsbCameraDemo()
    {
      _devices = UICameraPreview.GetVideoInputDeviceInfos().ToArray();
      UICameraDeviceInfo? defaultDevice = _devices.Length > 0 ? _devices[0] : null;
      _selectedDeviceId = defaultDevice?.DeviceId;
      _preview = new UICameraPreview
      {
        PreferredDeviceId = _selectedDeviceId,
        Style = new DefaultUIStyle
        {
          Width = "100%",
          Height = "100%"
        }
      };
      _deviceLabel = CreateInfoLabel();
      _deviceSelect = CreateDeviceSelect(defaultDevice?.DeviceId);
      _statusLabel = CreateInfoLabel();
      _resultLabel = CreateInfoLabel();

      Style = new DefaultUIStyle
      {
        Width = "100%",
        Height = "100%",
        OverflowX = "hidden",
        PaddingTop = 24,
        PaddingRight = 24,
        PaddingBottom = 32,
        PaddingLeft = 24,
      };

      Children = new()
      {
          new UILabel
          {
              Text = "USB 摄像头",
              Style = new DefaultUIStyle
              {
                  FontSize = 30,
                  FontWeight = 700,
                  Color = ColorHelper.ParseColor("#0f172a")
              }
          },
          new UILabel
          {
              Text = "基于 DirectShow 的实时预览、拍照与 AVI 录像示例。",
              Style = new DefaultUIStyle
              {
                  FontSize = 16,
                  Color = ColorHelper.ParseColor("#334155")
              }
          },
          new UILabel
          {
              Text = "这个 Demo 不会自动打开摄像头。请手动点击按钮开始预览；录像输出为 AVI，停止录像后结果区会显示实际保存路径和文件大小。",
              Wrap = true,
              Style = new DefaultUIStyle
              {
                  Width = "100%",
                  FontSize = 14,
                  Color = ColorHelper.ParseColor("#475569")
              }
          },
                CreateInfoCard("设备信息", _deviceSelect, _deviceLabel),
          CreatePreviewCard(),
          CreateInfoCard("运行状态", _statusLabel, _resultLabel)
      };

      RefreshDeviceInfo();
      RefreshStatus();
      RefreshResult("拍照默认保存到图片目录，录像默认保存到视频目录。输出目录为 TCYM.UI.Example/UsbCamera。\n如需生成 AVI，请先点击“打开摄像头”，再开始录像。", 6);
    }

    /// <summary>
    /// 创建设备切换选择器，并把选中值绑定到预览控件的 PreferredDeviceId。
    /// </summary>
    private UISelect CreateDeviceSelect(string? defaultDeviceId)
    {
      return new UISelect
      {
        Options = _devices
            .Select(device => new SelectOption(device.DeviceId, device.DisplayName))
            .ToList(),
        SelectedValue = defaultDeviceId,
        Placeholder = _devices.Length == 0 ? "当前没有可用设备" : "请选择摄像头设备",
        ShowSearch = _devices.Length > 6,
        Disabled = _devices.Length == 0,
        Style = new UpdateUIStyle
        {
          Width = 320,
          Height = 34
        },
        OnSelect = option => SelectDeviceById(option.Value?.ToString())
      };
    }

    /// <summary>
    /// 页面被切入时刷新一次状态文本，确保展示当前控件状态。
    /// </summary>
    /// <param name="fromPath">来源路由。</param>
    public void OnRouteEnter(string? fromPath)
    {
      RefreshDeviceInfo();
      RefreshStatus();
    }

    /// <summary>
    /// 页面被切出时停止预览，避免离开路由后仍然占用摄像头设备。
    /// </summary>
    /// <param name="toPath">目标路由。</param>
    public void OnRouteLeave(string? toPath)
    {
      bool wasRecording = _preview.IsRecording;
      try
      {
        _preview.Stop();
      }
      catch { }

      _currentRecordingFilePath = null;
      RefreshStatus();
      if (wasRecording)
      {
        RefreshResult("已离开 USB 摄像头示例页，录像已自动停止。", 4);
      }
    }

    /// <summary>
    /// 按设备稳定标识切换当前示例页绑定的摄像头。
    /// 若正在录像，会先结束当前录像再切换设备。
    /// </summary>
    private void SelectDeviceById(string? deviceId)
    {
      if (string.IsNullOrWhiteSpace(deviceId))
      {
        return;
      }

      if (!TryGetDeviceById(deviceId, out UICameraDeviceInfo selectedDevice))
      {
        return;
      }

      if (string.Equals(_selectedDeviceId, selectedDevice.DeviceId, StringComparison.Ordinal))
      {
        RefreshDeviceInfo();
        return;
      }

      bool wasRecording = _preview.IsRecording;
      if (wasRecording)
      {
        _preview.StopRecordingAsync().GetAwaiter().GetResult();
      }

      _selectedDeviceId = selectedDevice.DeviceId;
      _preview.PreferredDeviceId = _selectedDeviceId;
      _deviceSelect.SelectedValue = _selectedDeviceId;

      RefreshDeviceInfo();
      RefreshStatus();
      RefreshResult(
          wasRecording
              ? $"已停止当前录像并切换到设备：{selectedDevice.DisplayName}\n{BuildRecordingResultMessage()}"
              : $"已切换到设备：{selectedDevice.DisplayName}",
          6);
    }

    /// <summary>
    /// 按稳定标识查找设备。
    /// </summary>
    private bool TryGetDeviceById(string? deviceId, out UICameraDeviceInfo device)
    {
      foreach (UICameraDeviceInfo candidate in _devices)
      {
        if (string.Equals(candidate.DeviceId, deviceId, StringComparison.OrdinalIgnoreCase))
        {
          device = candidate;
          return true;
        }
      }

      device = default;
      return false;
    }

    /// <summary>
    /// 构建预览区卡片，包含画面容器和拍照、录像相关操作按钮。
    /// </summary>
    private UIView CreatePreviewCard()
    {
      _startRecordingButton = CreateActionButton("开始录像", async () =>
      {
        if (_recordingOperationPending || _preview.IsRecording)
        {
          return;
        }

        _recordingOperationPending = true;
        UpdateRecordingActionButtons();
        try
        {
          string filePath = BuildVideoPath();
          _currentRecordingFilePath = filePath;
          await _preview.StartRecordingAsync(filePath);
          RefreshResult($"录像已开始：\n{filePath}\n停止录像后会在这里显示最终文件大小。", 6);
        }
        catch (Exception ex)
        {
          _currentRecordingFilePath = null;
          RefreshResult($"开始录像失败：{ex.Message}", 6);
        }
        finally
        {
          _recordingOperationPending = false;
          RefreshStatus();
        }
      });

      _stopRecordingButton = CreateActionButton("停止录像", async () =>
      {
        if (_recordingOperationPending || !_preview.IsRecording)
        {
          return;
        }

        _recordingOperationPending = true;
        UpdateRecordingActionButtons();
        try
        {
          await _preview.StopRecordingAsync();
          RefreshResult(BuildRecordingResultMessage(), 6);
        }
        catch (Exception ex)
        {
          RefreshResult($"停止录像失败：{ex.Message}", 6);
        }
        finally
        {
          _recordingOperationPending = false;
          RefreshStatus();
        }
      });

      return CreateInfoCard(
          "交互演示",
          new UIView
          {
            Style = new DefaultUIStyle
            {
              Width = "100%",
              Height = 600,
              BorderWidth = 1,
              BorderColor = ColorHelper.ParseColor("#cbd5e1"),
              BorderRadius = 16,
              BackgroundColor = ColorHelper.ParseColor("#0f172a")
            },
            Children = new()
            {
              _preview
            }
          },
          CreateButtonRow(
              CreateActionButton("打开摄像头", () =>
              {
                try
                {
                  _preview.Play();
                  RefreshStatus();
                  RefreshResult("已请求打开摄像头。若设备正常，预览画面会开始显示。", 4);
                }
                catch (Exception ex)
                {
                  RefreshStatus();
                  RefreshResult($"打开失败：{ex.Message}", 6);
                }
              }),
              CreateActionButton("关闭摄像头", () =>
              {
                bool wasRecording = _preview.IsRecording;
                try
                {
                  _preview.Stop();
                  RefreshStatus();
                  RefreshResult(wasRecording ? BuildRecordingResultMessage() : "摄像头预览已停止。", 6);
                }
                catch (Exception ex)
                {
                  RefreshStatus();
                  RefreshResult($"关闭失败：{ex.Message}", 6);
                }
              }),
              CreateActionButton("重新打开", () =>
              {
                try
                {
                  _preview.Restart();
                  RefreshStatus();
                  //RefreshResult("已重新初始化摄像头图形。", 4);
                }
                catch (Exception ex)
                {
                  RefreshStatus();
                  RefreshResult($"重开失败：{ex.Message}", 6);
                }
              })
          ),
          CreateButtonRow(
              CreateActionButton("拍照保存", () =>
              {
                try
                {
                  string filePath = BuildPhotoPath();
                  _preview.CapturePhoto(filePath);
                  RefreshStatus();
                  RefreshResult($"照片已保存：\n{filePath}", 6);
                }
                catch (Exception ex)
                {
                  RefreshStatus();
                  RefreshResult($"拍照失败：{ex.Message}", 6);
                }
              }),
              _startRecordingButton,
              _stopRecordingButton
          )
      );
    }

    /// <summary>
    /// 创建一个带标题的说明卡片，用于承载同一块演示内容。
    /// </summary>
    /// <param name="title">卡片标题。</param>
    /// <param name="content">卡片内容。</param>
    /// <returns>组装完成的卡片视图。</returns>
    private UIView CreateInfoCard(string title, params UIElement[] content)
    {
      var children = new List<UIElement>
            {
                new UILabel
                {
                    Text = title,
                    Style = new DefaultUIStyle
                    {
                        FontSize = 18,
                        FontWeight = 600,
                        Color = ColorHelper.ParseColor("#0f172a")
                    }
                }
            };
      children.AddRange(content);

      return new UIView
      {
        Style = new DefaultUIStyle
        {
          Width = "100%",
          MarginTop = 18,
          PaddingTop = 18,
          PaddingRight = 18,
          PaddingBottom = 18,
          PaddingLeft = 18,
          BorderWidth = 1,
          BorderColor = ColorHelper.ParseColor("#e2e8f0"),
          BorderRadius = 18,
          BackgroundColor = ColorHelper.ParseColor("rgba(255,255,255,0.96)"),
          Display = "flex",
          FlexDirection = "column",
          Gap = 12,
        },
        Children = children
      };
    }

    /// <summary>
    /// 创建一行按钮容器，统一横向排列当前操作按钮。
    /// </summary>
    /// <param name="buttons">要放入容器的按钮元素。</param>
    /// <returns>承载按钮的行容器。</returns>
    private UIView CreateButtonRow(params UIElement[] buttons)
    {
      return new UIView
      {
        Style = new DefaultUIStyle
        {
          Width = "100%",
          Display = "flex",
          FlexDirection = "row",
          Gap = 8,
        },
        Children = buttons.ToList()
      };
    }

    /// <summary>
    /// 创建统一样式的操作按钮，并绑定点击回调。
    /// </summary>
    /// <param name="text">按钮文本。</param>
    /// <param name="onClick">点击后执行的动作。</param>
    /// <returns>配置完成的按钮。</returns>
    private UIButton CreateActionButton(string text, Action onClick)
    {
      return new UIButton
      {
        Text = text,
        Style = new DefaultUIStyle
        {
          Width = 120,
          Height = 38,
          BorderRadius = 10,
          Cursor = UICursor.Pointer,
          BackgroundColor = ColorHelper.ParseColor("#0ea5e9"),
          Color = ColorHelper.ParseColor("#ffffff"),
          FontSize = 14,
        },
        Events = new()
        {
          Click = _ => onClick()
        }
      };
    }

    /// <summary>
    /// 创建统一样式的信息标签，用于展示设备、状态和结果文本。
    /// </summary>
    /// <returns>配置完成的标签。</returns>
    private UILabel CreateInfoLabel()
    {
      return new UILabel
      {
        Wrap = true,
        MaxLines = 6,
        Style = new DefaultUIStyle
        {
          Width = "100%",
          FontSize = 14,
          Color = ColorHelper.ParseColor("#334155")
        }
      };
    }

    /// <summary>
    /// 刷新设备信息标签，展示当前检测到的摄像头列表和默认设备。
    /// </summary>
    private void RefreshDeviceInfo()
    {
      bool hasSelectedDevice = TryGetDeviceById(_selectedDeviceId, out UICameraDeviceInfo selectedDevice);
      _deviceLabel.Text = _devices.Length == 0
          ? "当前未检测到可用的 USB 摄像头设备。"
        : $"当前绑定：{(hasSelectedDevice ? selectedDevice.DisplayName : _devices[0].DisplayName)}\n检测到设备：{string.Join(" / ", _devices.Select(device => device.DisplayName))}\n设备总数：{_devices.Length}";
      _deviceLabel.RequestLayout();
      _deviceLabel.RequestRedraw();
    }

    /// <summary>
    /// 刷新状态标签，展示预览控件返回的最新状态文本。
    /// </summary>
    private void RefreshStatus()
    {
      _statusLabel.Text = $"当前状态：{_preview.StatusText}";
      _statusLabel.RequestLayout();
      _statusLabel.RequestRedraw();
      UpdateRecordingActionButtons();
    }

    /// <summary>
    /// 根据当前录像状态同步开始/停止按钮的可点击性。
    /// </summary>
    private void UpdateRecordingActionButtons()
    {
      if (_startRecordingButton != null)
      {
        _startRecordingButton.Disabled = _recordingOperationPending || _preview.IsRecording;
        _startRecordingButton.RequestRedraw();
      }

      if (_stopRecordingButton != null)
      {
        _stopRecordingButton.Disabled = _recordingOperationPending || !_preview.IsRecording;
        _stopRecordingButton.RequestRedraw();
      }
    }

    /// <summary>
    /// 刷新结果标签，并同步限制最大显示行数。
    /// </summary>
    /// <param name="message">要显示的结果文本。</param>
    /// <param name="maxLines">标签允许显示的最大行数。</param>
    private void RefreshResult(string message, int maxLines)
    {
      _resultLabel.MaxLines = maxLines;
      _resultLabel.Text = message;
      _resultLabel.RequestLayout();
      _resultLabel.RequestRedraw();
    }

    /// <summary>
    /// 根据当前录像输出路径生成用户可读的结果文本。
    /// </summary>
    /// <returns>录像停止后的提示信息。</returns>
    private string BuildRecordingResultMessage()
    {
      string? filePath = _currentRecordingFilePath;
      _currentRecordingFilePath = null;

      if (string.IsNullOrWhiteSpace(filePath))
      {
        return "录像已停止。\n未记录到本次输出路径，请查看视频目录下的 TCYM.UI.Example/UsbCamera。";
      }

      if (!File.Exists(filePath))
      {
        return $"录像已停止，但当前没有找到输出文件：\n{filePath}";
      }

      long size = new FileInfo(filePath).Length;
      return $"录像已保存：\n{filePath}\n文件大小：{FormatFileSize(size)}";
    }

    /// <summary>
    /// 把字节数转换成适合界面展示的文件大小字符串。
    /// </summary>
    /// <param name="size">原始字节数。</param>
    /// <returns>格式化后的文件大小文本。</returns>
    private static string FormatFileSize(long size)
    {
      if (size < 1024) return $"{size} B";
      if (size < 1024 * 1024) return $"{size / 1024.0:F1} KB";
      if (size < 1024L * 1024 * 1024) return $"{size / 1024.0 / 1024.0:F2} MB";
      return $"{size / 1024.0 / 1024.0 / 1024.0:F2} GB";
    }

    /// <summary>
    /// 生成拍照文件的默认输出路径。
    /// </summary>
    /// <returns>图片文件完整路径。</returns>
    private static string BuildPhotoPath()
    {
      string directory = GetOutputDirectory(Environment.SpecialFolder.MyPictures);
      return Path.Combine(directory, $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.png");
    }

    /// <summary>
    /// 生成录像文件的默认输出路径。
    /// </summary>
    /// <returns>AVI 文件完整路径。</returns>
    private static string BuildVideoPath()
    {
      string directory = GetOutputDirectory(Environment.SpecialFolder.MyVideos);
      return Path.Combine(directory, $"record_{DateTime.Now:yyyyMMdd_HHmmss}.avi");
    }

    /// <summary>
    /// 获取拍照或录像输出目录，不存在时自动创建。
    /// </summary>
    /// <param name="folder">输出所基于的系统特殊目录。</param>
    /// <returns>最终用于保存文件的目录。</returns>
    private static string GetOutputDirectory(Environment.SpecialFolder folder)
    {
      string root = Environment.GetFolderPath(folder);
      if (string.IsNullOrWhiteSpace(root))
      {
        root = AppContext.BaseDirectory;
      }

      string directory = Path.Combine(root, "TCYM.UI.Example", "UsbCamera");
      Directory.CreateDirectory(directory);
      return directory;
    }
  }
}