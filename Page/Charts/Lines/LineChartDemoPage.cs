using TCYM.UI.Chart;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace Page.Charts.Lines
{
    /// <summary>
    /// 为折线图示例提供统一的标题、说明、预览卡片和 UIChart 资源生命周期。
    /// </summary>
    internal abstract class LineChartDemoPage : UIScrollView, IUIRouteLifecycle
    {
        /// <summary>图表示例页面共用的嵌入式样式资源。</summary>
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.Charts.Lines.style.css";

        /// <summary>当前页面拥有的图表控件。</summary>
        private readonly UIChart _chart;

        /// <summary>获取供具体示例订阅事件的图表控件。</summary>
        protected UIChart Chart => _chart;

        /// <summary>指示页面是否已经释放图表原生资源。</summary>
        private bool _disposed;

        /// <summary>创建采用组件 Demo 排版的折线图页面并提交首个完整配置。</summary>
        /// <param name="title">页面主标题。</param>
        /// <param name="subtitle">页面副标题。</param>
        /// <param name="description">当前折线图能力说明。</param>
        /// <param name="option">要提交给 UIChart 的完整配置。</param>
        /// <param name="registry">示例所需的可选扩展模块注册表；为空时使用默认注册表。</param>
        protected LineChartDemoPage(
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
                //Width = ChartWidth,
                //Height = ChartHeight,
                ClassName = "line-chart-canvas",
                //Style = new DefaultUIStyle
                //{
                //    Width = "100%",
                //    Height = 550,
                //},
            };

            ClassName = "line-chart-demo";
            Children = new()
            {
                new UILabel
                {
                    Text = title,
                    ClassName = "line-chart-demo-title",
                },
                new UILabel
                {
                    Text = subtitle,
                    ClassName = "line-chart-demo-subtitle",
                },
                new UILabel
                {
                    Text = description,
                    ClassName = "line-chart-demo-description",
                },
                new UIView
                {
                    ClassName = "line-chart-preview-card",
                    Children = new()
                    {
                        new UILabel
                        {
                            Text = "效果预览",
                            ClassName = "line-chart-card-title",
                        },
                        new UILabel
                        {
                            Text = "图表使用固定 100% ×490  预览画布；较窄窗口中可横向滚动查看完整时间轴。",
                            ClassName = "line-chart-preview-help",
                        },
                        new UIScrollView
                        {
                            ClassName = "line-chart-preview-scroll",
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

        /// <summary>创建面积折线图示例页面。</summary>
        /// <returns>包含渐变面积和平滑折线的示例页面。</returns>
        internal static LineChartDemoPage CreateArea()
        {
            return new AreaLineChartDemo();
        }

        /// <summary>创建基础折线图示例页面。</summary>
        /// <returns>包含普通折线和离散节点的示例页面。</returns>
        internal static LineChartDemoPage CreateBasic()
        {
            return new BasicLineChartDemo();
        }

        /// <summary>创建多系列折线图示例页面。</summary>
        /// <returns>包含两个对比系列的示例页面。</returns>
        internal static LineChartDemoPage CreateMultiple()
        {
            return new MultipleLineChartDemo();
        }

        /// <summary>创建阶梯折线图示例页面。</summary>
        /// <returns>包含 End 阶梯折线的示例页面。</returns>
        internal static LineChartDemoPage CreateStep()
        {
            return new StepLineChartDemo();
        }

        /// <summary>创建平滑折线图示例页面。</summary>
        /// <returns>包含 X 方向单调平滑折线的示例页面。</returns>
        internal static LineChartDemoPage CreateSmooth()
        {
            return new SmoothLineChartDemo();
        }

        /// <summary>创建 10K 模拟数据折线图示例页面。</summary>
        /// <returns>包含连续数值轴和 LTTB 降采样的示例页面。</returns>
        internal static LineChartDemoPage CreateLargeData()
        {
            return new LargeDataLineChartDemo();
        }

        /// <summary>创建实时追加折线图示例页面。</summary>
        /// <returns>持续通过 AppendData 追加时间和值的示例页面。</returns>
        internal static LineChartDemoPage CreateRealtime()
        {
            return new RealtimeLineChartDemo();
        }

        /// <summary>页面进入时保留已经构建完成的静态图表。</summary>
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

        /// <summary>页面离开时清理场景并释放 UIChart 拥有的原生资源。</summary>
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
