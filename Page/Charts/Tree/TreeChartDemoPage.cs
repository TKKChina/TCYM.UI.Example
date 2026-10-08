using SkiaSharp;
using TCYM.UI.Chart;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace Page.Charts.Tree;

/// <summary>沿用 Lines 的页面结构；各具体案例自己创建完整 Options 和数据。</summary>
internal abstract class TreeChartDemoPage : UIScrollView, IUIRouteLifecycle
{
    private bool _disposed;
    private SKSize _chartSize;
    protected UIChart Chart { get; }

    protected TreeChartDemoPage(string title, string subtitle, string description, ChartOption option)
    {
        UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.Charts.Tree.style.css");
        ShowScrollbar = false;
        Chart = new UIChart(ChartModules.CreateDefaultRegistry()) { ClassName = "tree-chart-canvas" };
        ClassName = "tree-chart-demo";
        Children =
        [
            new UILabel { Text = title, ClassName = "tree-chart-demo-title" },
            new UILabel { Text = subtitle, ClassName = "tree-chart-demo-subtitle" },
            new UILabel { Text = description, ClassName = "tree-chart-demo-description" },
            new UIView
            {
                ClassName = "tree-chart-preview-card",
                Children =
                [
                    new UILabel { Text = "效果预览", ClassName = "tree-chart-card-title" },
                    new UILabel
                    {
                        Text = "预览画布为 100% × 590；点击分支折叠/展开。Ctrl＋滚轮或触屏双指缩放，拖动平移；普通滚轮滚动页面。",
                        ClassName = "tree-chart-preview-help",
                    },
                    new UIScrollView
                    {
                        ClassName = "tree-chart-preview-scroll",
                        ShowScrollbar = false,
                        Children = [Chart],
                    },
                ],
            },
        ];
        Chart.SetOption(option, new ChartSetOptionOptions { NotMerge = true, LazyUpdate = true });
    }

    protected override void RenderContent(SKCanvas canvas)
    {
        if (!_disposed)
        {
            var inset = MathF.Max(0, Chart.BorderWidth);
            var size = new SKSize(
                MathF.Max(0, Chart.Width - inset * 2 - Chart.PaddingLeft - Chart.PaddingRight) / EffectiveLayoutDpiScale,
                MathF.Max(0, Chart.Height - inset * 2 - Chart.PaddingTop - Chart.PaddingBottom) / EffectiveLayoutDpiScale);
            if (size.Width > 0 && size.Height > 0 && size != _chartSize)
            {
                _chartSize = size;
                OnChartSizeChanged(size);
            }
        }
        base.RenderContent(canvas);
    }

    /// <summary>普通 Tree 由原生布局自动适应；公司案例在这里按可用宽度调整卡片。</summary>
    protected virtual void OnChartSizeChanged(SKSize size) { }

    public void OnRouteEnter(string? fromPath) { }
    public void OnRouteLeave(string? toPath)
    {
        if (_disposed) return;
        _disposed = true;
        Chart.Clear();
        Chart.Dispose();
    }

    internal static TreeChartDemoPage CreateCompany() => new CompanyTreeChartDemo();
    internal static TreeChartDemoPage CreateLeftToRight() => new LeftToRightTreeChartDemo();
    internal static TreeChartDemoPage CreateTopToBottom() => new TopToBottomTreeChartDemo();
    internal static TreeChartDemoPage CreateRightToLeft() => new RightToLeftTreeChartDemo();
}
