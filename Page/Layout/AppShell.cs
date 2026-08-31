using Page.Charts;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace TCYM.UI.Example.Page.Layout
{
    /// <summary>
    /// 应用最外层路由：登录页与主布局是两个同级页面。
    /// </summary>
    internal sealed class AppShell : UIView
    {
        internal AppShell()
        {
            UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.Layout.style.css");

            ClassName = "app-shell";
            var router = new UIRouter();
            router.Register(
            [
                new UIRouteRecord
                {
                    Id = "login",
                    Path = "/login",
                    Name = "登录",
                    KeepAlive = false,
                    Element = UIRoute.LazyLoad(() => new LoginPage())
                },
                new UIRouteRecord
                {
                    Id = "app",
                    Path = "/app",
                    Name = "主界面",
                    KeepAlive = false,
                    ReclaimMemoryOnLeave = true,
                    Element = UIRoute.LazyLoad(() => new Layout())
                },
                new UIRouteRecord
                {
                    Id = "charts",
                    Path = "/charts",
                    Name = "图表",
                    KeepAlive = false,
                    ReclaimMemoryOnLeave = true,
                    Element = UIRoute.LazyLoad(() => new ChartPage())
                }
            ]);

            Children =
            [
                new UIRouterView(router)
                {
                    ClassName = "app-router-view"
                }
            ];
            UISystem.Manager?.GetElementById<UICaptionBar>("demo-caption-bar")?.AddClass("login-caption-bar");
            UIRouterNavigator.NavigateById("login", replace: true);
        }
    }
}
