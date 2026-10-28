using TCYM.UI.Chart;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace Page.Charts.TreeMap;

/// <summary>沿用 Lines 的页面排版和图表生命周期；各案例自己创建完整 Options。</summary>
internal abstract class TreeMapChartDemoPage : UIScrollView, IUIRouteLifecycle
{
    private bool _disposed;
    protected UIChart Chart { get; }

    protected TreeMapChartDemoPage(string title, string subtitle, string description, ChartOption option)
    {
        UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.Charts.TreeMap.style.css");
        ShowScrollbar = false;
        Chart = new UIChart(ChartModules.CreateDefaultRegistry()) { ClassName = "treemap-chart-canvas" };
        ClassName = "treemap-chart-demo";
        Children =
        [
            new UILabel { Text = title, ClassName = "treemap-chart-demo-title" },
            new UILabel { Text = subtitle, ClassName = "treemap-chart-demo-subtitle" },
            new UILabel { Text = description, ClassName = "treemap-chart-demo-description" },
            new UIView
            {
                ClassName = "treemap-chart-preview-card",
                Children =
                [
                    new UILabel { Text = "效果预览", ClassName = "treemap-chart-card-title" },
                    new UILabel
                    {
                        Text = "画布为 100% × 590；点击矩形缩放或下钻，点击面包屑返回；Ctrl＋滚轮或触屏双指缩放，普通滚轮滚动页面。",
                        ClassName = "treemap-chart-preview-help",
                    },
                    new UIScrollView
                    {
                        ClassName = "treemap-chart-preview-scroll",
                        ShowScrollbar = false,
                        Children = [Chart],
                    },
                ],
            },
        ];
        // 首次布局后再构建图表，避免先按 0 × 0 计算一次大数据。
        Chart.SetOption(option, new ChartSetOptionOptions { NotMerge = true, LazyUpdate = true });
    }

    public void OnRouteEnter(string? fromPath) { }

    public void OnRouteLeave(string? toPath)
    {
        if (_disposed) return;
        _disposed = true;
        Chart.Clear();
        Chart.Dispose();
    }

    internal static TreeMapChartDemoPage CreateDisk() => new DiskUsageTreemapDemo();
    internal static TreeMapChartDemoPage CreateParent() => new ParentHierarchyTreemapDemo();
    internal static TreeMapChartDemoPage CreateGradient() => new GradientMappingTreemapDemo();
}
