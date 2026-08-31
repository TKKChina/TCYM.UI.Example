using TCYM.UI.Chart;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace Page.Charts.Gauge
{
    /// <summary>为仪表盘示例提供统一的标题、说明、预览卡片和 UIChart 生命周期。</summary>
    internal abstract class GaugeChartDemoPage : UIScrollView, IUIRouteLifecycle
    {
        /// <summary>仪表盘示例共用的嵌入式样式资源。</summary>
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.Charts.Gauge.style.css";

        /// <summary>当前页面拥有的图表控件。</summary>
        private readonly UIChart _chart;

        /// <summary>获取供具体示例更新配置或订阅事件的图表控件。</summary>
        protected UIChart Chart => _chart;

        /// <summary>指示页面是否已经释放图表资源。</summary>
        private bool _disposed;

        /// <summary>创建采用组件 Demo 排版的仪表盘页面并提交完整配置。</summary>
        /// <param name="title">页面主标题。</param>
        /// <param name="subtitle">页面副标题。</param>
        /// <param name="description">当前示例能力说明。</param>
        /// <param name="option">要提交给 UIChart 的完整配置。</param>
        /// <param name="registry">可选模块注册表；为空时使用默认注册表。</param>
        protected GaugeChartDemoPage(
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
                ClassName = "gauge-chart-canvas",
            };

            ClassName = "gauge-chart-demo";
            Children = new()
            {
                new UILabel
                {
                    Text = title,
                    ClassName = "gauge-chart-demo-title",
                },
                new UILabel
                {
                    Text = subtitle,
                    ClassName = "gauge-chart-demo-subtitle",
                },
                new UILabel
                {
                    Text = description,
                    ClassName = "gauge-chart-demo-description",
                },
                new UIView
                {
                    ClassName = "gauge-chart-preview-card",
                    Children = new()
                    {
                        new UILabel
                        {
                            Text = "效果预览",
                            ClassName = "gauge-chart-card-title",
                        },
                        new UILabel
                        {
                            Text = "图表使用 100% × 490 预览画布；窗口变窄时可以横向滚动查看完整仪表盘。",
                            ClassName = "gauge-chart-preview-help",
                        },
                        new UIScrollView
                        {
                            ClassName = "gauge-chart-preview-scroll",
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

        /// <summary>创建速度仪表盘示例。</summary>
        internal static GaugeChartDemoPage CreateSpeed() => new SpeedGaugeChartDemo();

        /// <summary>创建动态阶段速度仪表盘示例。</summary>
        internal static GaugeChartDemoPage CreateStageSpeed() => new StageSpeedGaugeChartDemo();

        /// <summary>创建动态气温仪表盘示例。</summary>
        internal static GaugeChartDemoPage CreateTemperature() => new TemperatureGaugeChartDemo();

        /// <summary>创建实时时钟仪表盘示例。</summary>
        internal static GaugeChartDemoPage CreateClock() => new ClockGaugeChartDemo();

        /// <summary>创建自定义正负区间仪表盘示例。</summary>
        internal static GaugeChartDemoPage CreateCustom() => new CustomGaugeChartDemo();

        /// <summary>创建由 Pie 与 Gauge 多层组合的饼仪图示例。</summary>
        internal static GaugeChartDemoPage CreatePieGauge() => new PieGaugeChartDemo();

        /// <summary>页面进入时允许具体示例启动自身的动态行为。</summary>
        /// <param name="fromPath">来源路由路径。</param>
        public void OnRouteEnter(string? fromPath)
        {
            OnDemoRouteEnter(fromPath);
        }

        /// <summary>允许具体示例在页面显示后启动自身的动态行为。</summary>
        /// <param name="fromPath">来源路由路径。</param>
        protected virtual void OnDemoRouteEnter(string? fromPath)
        {
            _ = fromPath;
        }

        /// <summary>离开页面时停止动态行为、清空场景并释放 UIChart。</summary>
        /// <param name="toPath">目标路由路径。</param>
        public void OnRouteLeave(string? toPath)
        {
            if (_disposed)
            {
                return;
            }

            OnDemoRouteLeave(toPath);
            _chart.Clear();
            _chart.Dispose();
            _disposed = true;
        }

        /// <summary>允许具体示例在图表清理和释放前停止自身的动态行为。</summary>
        /// <param name="toPath">目标路由路径。</param>
        protected virtual void OnDemoRouteLeave(string? toPath)
        {
            _ = toPath;
        }
    }
}
