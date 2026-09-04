using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Modal;
using TCYM.UI.Enums;
using TCYM.UI.Events;
using TCYM.UI.Example.Page.Layout;
using TCYM.UI.Helpers;

internal static class Program
{
    /// <summary>初始化 UI 系统、挂接默认页面并运行桌面消息循环。</summary>
    public static void Main()
    {
        // 是否启用帧时间日志（输出每帧的CPU和GPU时间，单位毫秒），可用于性能分析和调优。启用后会在控制台输出每帧的渲染时间信息，帮助开发者了解UI渲染的性能瓶颈。
        UISystem.EnableFrameTimingLog = true;
        // 是否启用渲染类型分析日志（输出每帧的渲染类型分析信息）。启用后会在控制台输出每帧的渲染类型分析结果，包括不同渲染类型的占比和性能数据。这对于优化UI渲染性能非常有帮助，尤其是在复杂UI场景下，可以帮助开发者识别哪些渲染类型可能导致性能问题。
        UISystem.EnableRenderTypeProfile = true;
        // 示例调试时不隐藏累计耗时低于默认 0.5ms 的轻量组件，确保新增组件和 Chart 内部图元也会出现在统计中。
        UISystem.RenderTypeProfileMinimumMilliseconds = 0;
        // 帧时间日志的阈值和输出频率设置（仅在启用帧时间日志时生效）。FrameTimingLogThresholdMs 设置了日志输出的时间阈值，只有当某帧的CPU或GPU渲染时间超过这个值时才会输出日志。FrameTimingLogIntervalFrames 设置了日志输出的频率，表示每隔多少帧输出一次日志。合理设置这两个参数可以帮助开发者聚焦于性能问题较严重的帧，同时避免过多的日志输出干扰分析。
        UISystem.FrameTimingLogThresholdMs = 1;
        // 设置帧时间日志的输出频率（单位：帧）。例如，设置为1表示每帧都输出日志，设置为10表示每10帧输出一次日志。合理设置这个参数可以帮助开发者在性能分析时获得足够的数据，同时避免过多的日志输出干扰分析。
        UISystem.FrameTimingLogIntervalFrames = 60;

        // 是否启用GPU初始化日志（输出GPU相关的初始化信息和错误日志）。启用后会在控制台输出GPU设备的相关信息、驱动版本、支持的功能等，以及在GPU初始化过程中遇到的任何错误。这对于调试和优化GPU渲染性能非常有帮助，尤其是在不同平台和设备上运行时。
        UISystem.EnableGpuInitLog = true;
        // Windows 11 向 DWM 提交原生圆角偏好；Windows 10 使用 Region 与分层外阴影回退。
        // macOS/Linux 当前保留窗口管理器的默认圆角与阴影外观。
        UISystem.DefaultWindowCornerPreference = UIWindowCornerPreference.Round;
        UISystem.DefaultWindowShadowPreference = UIWindowShadowPreference.Enabled;
        UISystem.Initialize("TCYM", 1200, 800, true, 30, resizable: true);

        var manager = UISystem.Manager;
        if (manager == null) return;
        //manager.OnAnyJoystickEvent += (e) =>
        //{
        //    switch (e.Type)
        //    {
        //        case UIJoystickEventType.DeviceAdded:
        //            Console.WriteLine($"[{e.DeviceType}] 设备 {e.DeviceId} 已连接");
        //            break;
        //        case UIJoystickEventType.DeviceRemoved:
        //            Console.WriteLine($"[{e.DeviceType}] 设备 {e.DeviceId} 已断开");
        //            break;
        //        case UIJoystickEventType.AxisMotion:
        //            // AxisNormalized 已归一化到 -1.0 ~ 1.0，直接使用
        //            Console.WriteLine($"[{e.DeviceType}] 设备 {e.DeviceId} 轴 {e.Axis} = {e.AxisNormalized:F3}");
        //            break;
        //        case UIJoystickEventType.HatMotion:
        //            Console.WriteLine($"[{e.DeviceType}] 设备 {e.DeviceId} 帽子 {e.Hat} = {e.HatState}");
        //            break;
        //        case UIJoystickEventType.ButtonDown:
        //            Console.WriteLine($"[{e.DeviceType}] 设备 {e.DeviceId} 按钮 {e.Button} 按下");
        //            break;
        //        case UIJoystickEventType.ButtonUp:
        //            Console.WriteLine($"[{e.DeviceType}] 设备 {e.DeviceId} 按钮 {e.Button} 抬起");
        //            break;
        //    }
        //    Console.Error.WriteLine($"[{e.DeviceType}] 设备 {e.DeviceId} 事件 {e.Type}");
        //};
        UISystem.UnhandledException += (_, e) =>
        {
            UIModal.Confirm("发生未处理异常", e.Exception.ToString(), onOk: () =>
            {
                Environment.Exit(1);
            }, okType: ModalOkType.Danger, okText: "退出程序", cancelText: "忽略");
        };

        // === 全局通配符默认样式：字体16px，颜色 #000000 ===
        UISystem.RegisterGlobalDefaultsCss("*{font-size:16px;color: rgba(0,0,0,1);}");
        UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.com.css");
        var root = manager.Root;
        root.SetStyle(new DefaultUIStyle
        {
            BackgroundColor = ColorHelper.ParseColor("rgb(252, 252, 252)"),
        });
        root.Id = "root";
        root.AddChild(new UICaptionBar()
        {
            Id = "demo-caption-bar",
            CaptionTitle = "TCYM.UI.Demo",
            CaptionBoxStyle = new UpdateUIStyle
            {
                BoxShadowColor = ColorHelper.ParseColor("rgba(140,139,139,0.25)"),
                BoxShadowSpread = 10,
                BoxShadowOffsetY = 2,
                BoxShadowBlur = 4,
                Cursor = UICursor.Grab
            },
        });
        root.AddChild(new AppShell());

        try
        {
            UISystem.Run();
        }
        finally
        {
            UISystem.Shutdown();
        }
    }
}
