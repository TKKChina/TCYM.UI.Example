using SkiaSharp;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.TreeMap;

/// <summary>使用官方磁盘树数据展示各文件和目录的占用面积。</summary>
internal sealed class DiskUsageTreemapDemo : TreeMapChartDemoPage
{
    internal DiskUsageTreemapDemo() : base(
        "磁盘占用",
        "通过矩形面积比较磁盘目录与文件的大小。",
        "使用嵌入的官方数据；Tooltip 显示完整路径和原始 KB。小于 300 像素面积的块隐藏；颜色饱和度和边框色按各层配置自动映射。",
        CreateOption()) { }

    private static ChartOption CreateOption()
    {
        // 1. 数据只解析一次；DataSource 是不可变快照，允许两个磁盘案例安全复用。
        var data = DiskUsageTreemapData.Instance;
        // 2. 开启 ECharts 层级语义：Levels[0] 是虚拟根，Levels[1] 起是业务节点。
        var option = new ChartOption
        {
            Background = ChartBrush.Solid(SKColors.White),
            Animation = new() { Enabled = true, Duration = 700, UpdateDuration = 450 },
            Title = new() { Text = "Disk Usage", Left = "center" },
            Tooltip = new() { Trigger = ChartTooltipTrigger.Item, Confine = true, FormatterCallback = DiskUsageTreemapData.FormatTooltip },
            Series = new ChartTreemapSeriesOption
            {
                Id = "disk-usage", Name = "Disk Usage", DataSource = data.Source,
                VisibleMin = 300, NodeClick = ChartTreemapNodeClick.ZoomToNode, Roam = true,
                LevelsIncludeVirtualRoot = true,
                Label = new() { Show = true, Formatter = "{b}" },
                UpperLabel = new() { Show = false },
                ItemStyle = new()
                {
                    BorderColor = ChartBrush.Solid(SKColors.White),
                    // ECharts 的默认 borderWidth/gapWidth 为 0；具体层级在 Levels 中覆盖。
                    BorderWidth = 0, GapWidth = 0,
                },
                Levels =
                [
                    new() { ItemStyle = new() { BorderWidth = 0, GapWidth = 5 } },
                    new() { ItemStyle = new() { GapWidth = 1 } },
                    new()
                    {
                        ColorSaturation = [0.35f, 0.5f],
                        ItemStyle = new() { GapWidth = 1, BorderColorSaturation = 0.6f },
                    },
                ],
                Breadcrumb = new() { Show = true, RootName = "Disk Usage" },
            },
        };
        // 调色板属于本案例的 Options；与共享数据里的边框色保持同一组颜色。
        foreach (var color in DiskUsageTreemapData.Palette)
            option.Color.Add(ChartBrush.Solid(color));
        return option;
    }
}
