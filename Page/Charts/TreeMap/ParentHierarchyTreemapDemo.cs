using SkiaSharp;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.TreeMap;

/// <summary>用父节点标题带显示磁盘目录的层级。</summary>
internal sealed class ParentHierarchyTreemapDemo : TreeMapChartDemoPage
{
    internal ParentHierarchyTreemapDemo() : base(
        "显示父层级",
        "在磁盘矩形树图中显示父目录标题。",
        "父目录预留 30px 标题带，业务根使用深灰边框，悬停时边框变亮；下钻后可通过面包屑返回。各层饱和度和边框颜色由配置自动映射。",
        CreateOption()) { }

    private static ChartOption CreateOption()
    {
        var data = DiskUsageTreemapData.Instance;
        // 与原示例的三个层级一一对应；第 0 层只作用于虚拟根。
        ChartTreemapLevelOption[] levels =
        [
            new()
            {
                ItemStyle = new() { BorderColor = ChartBrush.Solid(SKColor.Parse("#777777")), BorderWidth = 0, GapWidth = 1 },
                UpperLabel = new() { Show = false },
            },
            new()
            {
                ItemStyle = new() { BorderColor = ChartBrush.Solid(SKColor.Parse("#555555")), BorderWidth = 5, GapWidth = 1 },
                Emphasis = new() { ItemStyle = new() { BorderColor = ChartBrush.Solid(SKColor.Parse("#dddddd")) } },
            },
            new()
            {
                ColorSaturation = [0.35f, 0.5f],
                ItemStyle = new() { BorderWidth = 5, GapWidth = 1, BorderColorSaturation = 0.6f },
            },
        ];
        var option = new ChartOption
        {
            Background = ChartBrush.Solid(SKColors.White),
            Animation = new() { Enabled = true, Duration = 700, UpdateDuration = 450 },
            Title = new() { Text = "Disk Usage", Left = "center" },
            Tooltip = new() { Trigger = ChartTooltipTrigger.Item, Confine = true, FormatterCallback = DiskUsageTreemapData.FormatTooltip },
            Series = new ChartTreemapSeriesOption
            {
                Id = "disk-parent", Name = "Disk Usage", DataSource = data.Source,
                VisibleMin = 300,
                NodeClick = ChartTreemapNodeClick.ZoomToNode, Roam = true,
                LevelsIncludeVirtualRoot = true,
                Label = new() { Show = true, Formatter = "{b}" },
                UpperLabel = new() { Show = true, Height = 30, Formatter = "{b}" },
                ItemStyle = new()
                {
                    BorderColor = ChartBrush.Solid(SKColors.White),
                    BorderWidth = 0, GapWidth = 0,
                },
                Levels = levels,
                Breadcrumb = new() { Show = true, RootName = "Disk Usage" },
            },
        };
        // 当前案例自行设置调色板；共用色值保证父边框与填充颜色协调。
        foreach (var color in DiskUsageTreemapData.Palette)
            option.Color.Add(ChartBrush.Solid(color));
        return option;
    }
}
