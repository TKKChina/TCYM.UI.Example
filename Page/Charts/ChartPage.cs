using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace Page.Charts
{
    /// <summary>承载图表示例二级导航和独立内容路由的页面。</summary>
    internal sealed class ChartPage : UIView
    {
        /// <summary>创建 Line/Bar/Pie/Gauge 导航、路由视图并打开默认的基础折线图。</summary>
        internal ChartPage()
        {
            UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.Charts.style.css");

            var router = ChartRouter.Create();
            ClassName = "charts-page";
            Children = new()
            {
                new ChartMenu(),
                new UIRouterView(router)
                {
                    ClassName = "charts-router-view",
                },
            };

            if (!router.ReplaceById("chart_line_basic"))
            {
                throw new InvalidOperationException("无法创建默认的基础折线图页面。");
            }
        }
    }
}
