using TCYM.UI.Chart;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace Page.Charts.Bars
{
    /// <summary>为柱状图示例提供统一的标题、说明、预览卡片和 UIChart 生命周期。</summary>
    internal abstract class BarChartDemoPage : UIScrollView, IUIRouteLifecycle
    {
        /// <summary>柱状图示例共用的嵌入式样式资源。</summary>
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.Charts.Bars.style.css";

        /// <summary>当前页面拥有的图表控件。</summary>
        private readonly UIChart _chart;

        /// <summary>指示页面是否已经释放图表资源。</summary>
        private bool _disposed;

        /// <summary>创建采用组件 Demo 排版的柱状图页面并提交完整配置。</summary>
        /// <param name="title">页面主标题。</param>
        /// <param name="subtitle">页面副标题。</param>
        /// <param name="description">当前示例能力说明。</param>
        /// <param name="option">要提交给 UIChart 的完整配置。</param>
        /// <param name="registry">可选模块注册表；为空时使用默认注册表。</param>
        protected BarChartDemoPage(
            string title,
            string subtitle,
            string description,
            ChartOption option,
            ChartRegistry? registry = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            ArgumentException.ThrowIfNullOrWhiteSpace(subtitle);
            ArgumentException.ThrowIfNullOrWhiteSpace(description);
            ArgumentNullException.ThrowIfNull(option);

            UISystem.LoadStyleFile(DemoCssPath);

            _chart = new UIChart(registry ?? ChartModules.CreateDefaultRegistry())
            {
                ClassName = "bar-chart-canvas",
            };

            ClassName = "bar-chart-demo";
            Children = new()
            {
                new UILabel
                {
                    Text = title,
                    ClassName = "bar-chart-demo-title",
                },
                new UILabel
                {
                    Text = subtitle,
                    ClassName = "bar-chart-demo-subtitle",
                },
                new UILabel
                {
                    Text = description,
                    ClassName = "bar-chart-demo-description",
                },
                new UIView
                {
                    ClassName = "bar-chart-preview-card",
                    Children = new()
                    {
                        new UILabel
                        {
                            Text = "效果预览",
                            ClassName = "bar-chart-card-title",
                        },
                        new UILabel
                        {
                            Text = "图表使用 100% × 490 预览画布；较窄窗口中可横向滚动查看完整坐标轴。",
                            ClassName = "bar-chart-preview-help",
                        },
                        new UIScrollView
                        {
                            ClassName = "bar-chart-preview-scroll",
                            Children = new() { _chart },
                        },
                    },
                },
            };

            _chart.SetOption(option, new ChartSetOptionOptions
            {
                NotMerge = true,
            });
        }

        /// <summary>创建折柱混合示例。</summary>
        /// <returns>包含堆积柱与总量折线的示例页面。</returns>
        internal static BarChartDemoPage CreateMixed() => new MixedBarLineChartDemo();

        /// <summary>创建带圆角的堆积柱状图示例。</summary>
        /// <returns>包含两个 stack 与逐项顶部圆角的示例页面。</returns>
        internal static BarChartDemoPage CreateRoundedStack() => new RoundedStackBarChartDemo();

        /// <summary>创建柱状渐变示例。</summary>
        /// <returns>包含渐变、阴影和 rich 标签的示例页面。</returns>
        internal static BarChartDemoPage CreateGradient() => new GradientBarChartDemo();

        /// <summary>创建多 Y 轴示例。</summary>
        /// <returns>包含三条 Y 轴和折柱组合的示例页面。</returns>
        internal static BarChartDemoPage CreateMultipleYAxis() => new MultipleYAxisBarChartDemo();

        /// <summary>创建横向柱状图示例。</summary>
        /// <returns>包含双 Y 类目轴与重叠背景轨道的示例页面。</returns>
        internal static BarChartDemoPage CreateHorizontal() => new HorizontalBarChartDemo();

        /// <summary>创建横向堆叠条形图示例。</summary>
        /// <returns>包含五个 total stack 来源系列的示例页面。</returns>
        internal static BarChartDemoPage CreateHorizontalStack() => new HorizontalStackBarChartDemo();

        /// <summary>创建极坐标堆积柱状图示例。</summary>
        /// <returns>包含 Radius category 与 Angle value stack 的示例页面。</returns>
        internal static BarChartDemoPage CreatePolarStack() => new PolarStackBarChartDemo();

        /// <summary>创建极坐标玫瑰柱状图示例。</summary>
        /// <returns>包含 Angle category 与 Radius value 彩色柱的示例页面。</returns>
        internal static BarChartDemoPage CreatePolarRose() => new PolarRoseBarChartDemo();

        /// <summary>静态示例进入页面时无需额外启动任务。</summary>
        /// <param name="fromPath">来源路由路径。</param>
        public void OnRouteEnter(string? fromPath)
        {
            _ = fromPath;
        }

        /// <summary>离开页面时清空场景并释放 UIChart。</summary>
        /// <param name="toPath">目标路由路径。</param>
        public void OnRouteLeave(string? toPath)
        {
            _ = toPath;
            if (_disposed)
            {
                return;
            }

            _chart.Clear();
            _chart.Dispose();
            _disposed = true;
        }
    }
}
