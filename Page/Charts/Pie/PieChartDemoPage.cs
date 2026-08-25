using TCYM.UI.Chart;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace Page.Charts.Pie
{
    /// <summary>为饼图示例提供统一的标题、说明、预览卡片和 UIChart 生命周期。</summary>
    internal abstract class PieChartDemoPage : UIScrollView, IUIRouteLifecycle
    {
        /// <summary>饼图示例共用的嵌入式样式资源。</summary>
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.Charts.Pie.style.css";

        /// <summary>当前页面拥有的图表控件。</summary>
        private readonly UIChart _chart;

        /// <summary>获取供具体示例更新配置或订阅事件的图表控件。</summary>
        protected UIChart Chart => _chart;

        /// <summary>指示页面是否已经释放图表资源。</summary>
        private bool _disposed;

        /// <summary>创建采用组件 Demo 排版的饼图页面并提交完整配置。</summary>
        /// <param name="title">页面主标题。</param>
        /// <param name="subtitle">页面副标题。</param>
        /// <param name="description">当前示例能力说明。</param>
        /// <param name="option">要提交给 UIChart 的完整配置。</param>
        /// <param name="registry">可选模块注册表；为空时使用默认注册表。</param>
        protected PieChartDemoPage(
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
                ClassName = "pie-chart-canvas",
            };

            ClassName = "pie-chart-demo";
            Children = new()
            {
                new UILabel
                {
                    Text = title,
                    ClassName = "pie-chart-demo-title",
                },
                new UILabel
                {
                    Text = subtitle,
                    ClassName = "pie-chart-demo-subtitle",
                },
                new UILabel
                {
                    Text = description,
                    ClassName = "pie-chart-demo-description",
                },
                new UIView
                {
                    ClassName = "pie-chart-preview-card",
                    Children = new()
                    {
                        new UILabel
                        {
                            Text = "效果预览",
                            ClassName = "pie-chart-card-title",
                        },
                        new UILabel
                        {
                            Text = "图表使用 100% × 490 预览画布；窗口变窄时可以横向滚动查看完整标签。",
                            ClassName = "pie-chart-preview-help",
                        },
                        new UIScrollView
                        {
                            ClassName = "pie-chart-preview-scroll",
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

        /// <summary>创建嵌套饼图示例。</summary>
        /// <returns>包含内外两层访问来源的示例页面。</returns>
        internal static PieChartDemoPage CreateNested() => new NestedPieChartDemo();

        /// <summary>创建圆角环形图示例。</summary>
        /// <returns>包含间隔角和中心 emphasis 标签的示例页面。</returns>
        internal static PieChartDemoPage CreateRoundedDonut() => new RoundedDonutPieChartDemo();

        /// <summary>创建柱饼进度图示例。</summary>
        /// <returns>包含 Polar Bar、刻度环和中心标题的示例页面。</returns>
        internal static PieChartDemoPage CreatePolarProgress() => new PolarProgressPieChartDemo();

        /// <summary>创建南丁格尔玫瑰图示例。</summary>
        /// <returns>包含等角面积玫瑰布局的示例页面。</returns>
        internal static PieChartDemoPage CreateNightingaleRose() => new NightingaleRosePieChartDemo();

        /// <summary>创建半环形图示例。</summary>
        /// <returns>仅绘制上半环角度范围的示例页面。</returns>
        internal static PieChartDemoPage CreateHalfDonut() => new HalfDonutPieChartDemo();

        /// <summary>创建半圆增强展示示例。</summary>
        /// <returns>包含间隔扇区、双层标签和中心指标的示例页面。</returns>
        internal static PieChartDemoPage CreateEnhancedHalfDonut() => new EnhancedHalfDonutPieChartDemo();

        /// <summary>创建动态数据饼图示例。</summary>
        /// <returns>每秒更新一组饼图数据并展示平滑过渡的示例页面。</returns>
        internal static PieChartDemoPage CreateDynamicData() => new DynamicDataPieChartDemo();

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

        /// <summary>离开页面时清空场景并释放 UIChart。</summary>
        /// <param name="toPath">目标路由路径。</param>
        public void OnRouteLeave(string? toPath)
        {
            _ = toPath;
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
