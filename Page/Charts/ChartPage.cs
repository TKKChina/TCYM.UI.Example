using Page.Charts.Lines;
using TCYM.UI.Chart;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace Page.Charts
{
    /// <summary>承载图表示例二级导航和独立内容路由的页面。</summary>
    internal sealed class ChartPage : UIView, IUIRoutePreload
    {
        // 全进程只执行一次后台预热；Lazy 保证并发进入 app 时不会启动重复管线。
        private static readonly Lazy<Task> BackgroundWarmup = new(
            static () => Task.Run(static () =>
            {
                _ = ChartRouter.Create();
                BasicLineChartDemo.WarmUpPipeline();
            }),
            LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly UIRouter _router;

        /// <summary>
        /// 在线程池中预热纯 Chart 管线。此阶段不创建或挂载任何 UIElement。
        /// </summary>
        internal static Task WarmUpInBackgroundAsync() => BackgroundWarmup.Value;

        /// <summary>创建 Line/Bar/Pie 导航、路由视图并打开默认的基础折线图。</summary>
        internal ChartPage()
        {
            UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.Charts.style.css");

            _router = ChartRouter.Create();
            ClassName = "charts-page";
            Children = new()
            {
                new ChartMenu(),
                new UIRouterView(_router)
                {
                    ClassName = "charts-router-view",
                },
            };

            if (!_router.ReplaceById("chart_line_basic"))
            {
                throw new InvalidOperationException("无法创建默认的基础折线图页面。");
            }
        }

        /// <summary>
        /// 在页面仍未挂载时按路由宿主尺寸完成首轮布局，并提前构建默认图表场景。
        /// </summary>
        public void OnRoutePreload(float availableWidth, float availableHeight)
        {
            if (availableWidth <= 0f || availableHeight <= 0f) return;

            Measure(availableWidth, availableHeight);
            Layout(0f, 0f, availableWidth, availableHeight);
            PrepareCharts(this);
        }

        /// <summary>
        /// 递归准备页面当前已创建的图表；未来默认页增加多个图表时无需扩展专用字段。
        /// </summary>
        private static void PrepareCharts(UIElement element)
        {
            if (element is UIChart chart)
            {
                chart.Prepare();
            }

            foreach (var child in element.Children)
            {
                PrepareCharts(child);
            }
        }
    }
}
