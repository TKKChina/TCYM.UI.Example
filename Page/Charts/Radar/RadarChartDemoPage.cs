using TCYM.UI.Chart;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace Page.Charts.Radar
{
    /// <summary>沿用 Lines 的标题、说明、590px 预览卡片和图表资源生命周期。</summary>
    internal abstract class RadarChartDemoPage : UIScrollView, IUIRouteLifecycle
    {
        private bool _disposed;
        protected UIChart Chart { get; }

        protected RadarChartDemoPage(string title, string subtitle, string description, ChartOption option)
        {
            UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.Charts.Radar.style.css");
            Chart = new UIChart(ChartModules.CreateDefaultRegistry())
            {
                ClassName = "radar-chart-canvas",
            };
            ClassName = "radar-chart-demo";
            Children =
            [
                new UILabel { Text = title, ClassName = "radar-chart-demo-title" },
                new UILabel { Text = subtitle, ClassName = "radar-chart-demo-subtitle" },
                new UILabel { Text = description, ClassName = "radar-chart-demo-description" },
                new UIView
                {
                    ClassName = "radar-chart-preview-card",
                    Children =
                    [
                        new UILabel { Text = "效果预览", ClassName = "radar-chart-card-title" },
                        new UILabel
                        {
                            Text = "预览画布为 100% × 590；窄窗口可横向滚动。悬停查看各维数值，点击图例切换数据。",
                            ClassName = "radar-chart-preview-help",
                        },
                        new UIScrollView
                        {
                            ClassName = "radar-chart-preview-scroll",
                            Children = [Chart],
                        },
                    ],
                },
            ];
            Chart.SetOption(option, new ChartSetOptionOptions { NotMerge = true });
        }

        internal static RadarChartDemoPage CreateBasic() => new BasicRadarChartDemo();
        internal static RadarChartDemoPage CreateCustomized() => new CustomizedRadarChartDemo();
        internal static RadarChartDemoPage CreateGradient() => new GradientRadarChartDemo();
        internal static RadarChartDemoPage CreateMultidimensional() => new MultidimensionalRadarChartDemo();

        public void OnRouteEnter(string? fromPath) { }

        public void OnRouteLeave(string? toPath)
        {
            if (_disposed) return;
            Chart.Clear();
            Chart.Dispose();
            _disposed = true;
        }
    }
}
