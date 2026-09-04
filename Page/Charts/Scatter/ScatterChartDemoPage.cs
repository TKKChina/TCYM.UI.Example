using TCYM.UI.Chart;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace Page.Charts.Scatter
{
    /// <summary>为散点图示例提供统一的标题、说明、预览卡片和 UIChart 生命周期。</summary>
    internal abstract class ScatterChartDemoPage : UIScrollView, IUIRouteLifecycle
    {
        /// <summary>散点图示例共用的嵌入式样式资源。</summary>
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.Charts.Scatter.style.css";

        /// <summary>当前页面拥有的图表控件。</summary>
        private readonly UIChart _chart;

        /// <summary>获取供具体示例更新配置或订阅事件的图表控件。</summary>
        protected UIChart Chart => _chart;

        /// <summary>指示页面是否已经释放图表资源。</summary>
        private bool _disposed;

        /// <summary>创建采用组件 Demo 排版的散点图页面并提交完整配置。</summary>
        /// <param name="title">页面主标题。</param>
        /// <param name="subtitle">页面副标题。</param>
        /// <param name="description">当前示例能力说明。</param>
        /// <param name="option">要提交给 UIChart 的完整配置。</param>
        /// <param name="registry">可选模块注册表；为空时使用默认注册表。</param>
        protected ScatterChartDemoPage(
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
                ClassName = "scatter-chart-canvas",
            };

            ClassName = "scatter-chart-demo";
            Children = new()
            {
                new UILabel
                {
                    Text = title,
                    ClassName = "scatter-chart-demo-title",
                },
                new UILabel
                {
                    Text = subtitle,
                    ClassName = "scatter-chart-demo-subtitle",
                },
                new UILabel
                {
                    Text = description,
                    ClassName = "scatter-chart-demo-description",
                },
                new UIView
                {
                    ClassName = "scatter-chart-preview-card",
                    Children = new()
                    {
                        new UILabel
                        {
                            Text = "效果预览",
                            ClassName = "scatter-chart-card-title",
                        },
                        new UILabel
                        {
                            Text = "图表使用 100% × 590 预览画布；较窄窗口中可横向滚动查看完整坐标轴与图例。",
                            ClassName = "scatter-chart-preview-help",
                        },
                        new UIScrollView
                        {
                            ClassName = "scatter-chart-preview-scroll",
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

        /// <summary>创建基础散点图示例。</summary>
        internal static ScatterChartDemoPage CreateBasic() => new BasicScatterChartDemo();

        /// <summary>创建安斯库姆四重奏示例。</summary>
        internal static ScatterChartDemoPage CreateAnscombeQuartet() => new AnscombeQuartetScatterChartDemo();

        /// <summary>创建数据聚合散点图示例。</summary>
        internal static ScatterChartDemoPage CreateClustering() => new ClusteringScatterChartDemo();

        /// <summary>创建涟漪特效散点图示例。</summary>
        internal static ScatterChartDemoPage CreateEffect() => new EffectScatterChartDemo();

        /// <summary>创建特殊关系气泡散点图示例。</summary>
        internal static ScatterChartDemoPage CreateSpecialRelation() => new SpecialRelationScatterChartDemo();

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
