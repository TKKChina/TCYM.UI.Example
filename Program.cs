using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Example.Page.Layout;
using TCYM.UI.Helpers;

internal class Program
{
    public static void Main()
    {
        // 是否启用帧时间日志（输出每帧的CPU和GPU时间，单位毫秒），可用于性能分析和调优。启用后会在控制台输出每帧的渲染时间信息，帮助开发者了解UI渲染的性能瓶颈。
        UISystem.EnableFrameTimingLog = false;
        // 帧时间日志的阈值和输出频率设置（仅在启用帧时间日志时生效）。FrameTimingLogThresholdMs 设置了日志输出的时间阈值，只有当某帧的CPU或GPU渲染时间超过这个值时才会输出日志。FrameTimingLogIntervalFrames 设置了日志输出的频率，表示每隔多少帧输出一次日志。合理设置这两个参数可以帮助开发者聚焦于性能问题较严重的帧，同时避免过多的日志输出干扰分析。
        UISystem.FrameTimingLogThresholdMs = 1;
        // 设置帧时间日志的输出频率（单位：帧）。例如，设置为1表示每帧都输出日志，设置为10表示每10帧输出一次日志。合理设置这个参数可以帮助开发者在性能分析时获得足够的数据，同时避免过多的日志输出干扰分析。
        UISystem.FrameTimingLogIntervalFrames = 1;
        // 是否启用GPU初始化日志（输出GPU相关的初始化信息和错误日志）。启用后会在控制台输出GPU设备的相关信息、驱动版本、支持的功能等，以及在GPU初始化过程中遇到的任何错误。这对于调试和优化GPU渲染性能非常有帮助，尤其是在不同平台和设备上运行时。
        UISystem.EnableGpuInitLog = true;
        UISystem.Initialize("TCYM", 1620, 800, true, 30);

        var manager = UISystem.Manager;
        if (manager == null) return;
        // 支持AOT 环境下的属性访问
        TCYM.UI.Binding.Generated.GeneratedBindingAccessors_TCYM_UI.InitGenerated();
        TCYM.UI.Binding.Generated.GeneratedBindingAccessors_TCYM_UI_Example.InitGenerated();

        // === 全局通配符默认样式：字体16px，颜色 #000000 ===
        new DefaultUIStyle().ParseFromCss("*{font-size:16px;color: rgba(0,0,0,1);}");
        UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.com.css");
        var root = manager.Root;
        root.SetStyle(new DefaultUIStyle
        {
            BackgroundColor = ColorHelper.ParseColor("rgb(252, 252, 252)"),
        });
        root.Id = "root";
        root.AddChild(new UICaptionBar()
        {
            CaptionTitle = "TCYM.UI.Demo",
            CaptionBoxStyle = new UpdateUIStyle
            {
                BoxShadowColor = ColorHelper.ParseColor("rgba(140,139,139,0.25)"),
                BoxShadowSpread = 10,
                BoxShadowOffsetY = 2,
                BoxShadowBlur = 4,
            },
        });
        root.AddChild(new Layout());

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