using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using Page.Charts;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Message;

namespace TCYM.UI.Example.Page.Layout
{
    internal class Layout : UIView, IUIRouteLifecycle
    {
        private readonly UIRouter _appRouter;
        // 0=未开始，1=后台预热或等待 UI 空闲提交，2=页面预加载完成。
        private int _chartsPreloadState;

        internal Layout(UIRouter appRouter)
        {
            _appRouter = appRouter ?? throw new ArgumentNullException(nameof(appRouter));
            var router = Router.Create();
            ClassName = "main-view";
            Children = new()
            {
                new Menu
                {
                    SelectChange = (keys, item) =>
                    {
                        if (item.Key == "logout")
                        {
                            Logout();
                            return;
                        }
                        var path = item.Key switch
                        {
                            "button" => "/demo/button",
                            "icon" => "/demo/icon",
                            "label" => "/demo/label",
                            "glass" => "/demo/glass",
                            "image" => "/demo/image",
                            "menu" => "/demo/menu",
                            "segmented" => "/demo/segmented",
                            "slider" => "/demo/slider",
                            "splitter" => "/demo/splitter",
                            "tabs" => "/demo/tabs",
                            "carousel" => "/demo/carousel",
                            "pagination" => "/demo/pagination",
                            "dropdown" => "/demo/dropdown",
                            "select" => "/demo/select",
                            "checkbox" => "/demo/checkbox",
                            "radio" => "/demo/radio",
                            "datepicker" => "/demo/datepicker",
                            "timePicker" => "/demo/time-picker",
                            "colorPicker" => "/demo/color-picker",
                            "switch" => "/demo/switch",
                            "input" => "/demo/input",
                            "codeEditor" => "/demo/code-editor",
                            "badge" => "/demo/badge",
                            "tooltip" => "/demo/tooltip",
                            "floatbutton" => "/demo/float-button",
                            "tag" => "/demo/tag",
                            "tree" => "/demo/tree",
                            "table" => "/demo/table",
                            "timeline" => "/demo/timeline",
                            "svg" => "/demo/svg",
                            "message" => "/demo/message",
                            "notification" => "/demo/notification",
                            "modal" => "/demo/modal",
                            "progress" => "/demo/progress",
                            "chart" => "/charts",
                            "virtualScrollView" => "/demo/virtual-scroll-view",
                            "filePicker" => "/demo/file-picker",
                            "sdl3" => "/demo/sdl3",
                            "gamepad" => "/demo/gamepad",
                            "usbCamera" => "/demo/usb-camera",
                            "watermark" => "/demo/watermark",
                            "anchor" => "/demo/anchor",
                            _ => "/demo/button"
                        };
                        UIRouterNavigator.Navigate(path);
                    }
                },
                new UIRouterView(router)
                {
                   ClassName = "router-view"
                }
            };

            //router.ReplaceById("demo_button");
        }

        /// <summary>
        /// 主界面显示后先在线程池预热纯 Chart 管线，再把真实页面创建加入 UI 空闲队列。
        /// </summary>
        public void OnRouteEnter(string? fromPath)
        {
            _ = fromPath;
            if (Interlocked.CompareExchange(ref _chartsPreloadState, 1, 0) != 0) return;
            _ = WarmUpAndQueueChartsAsync();
        }

        /// <summary>
        /// 后台阶段不访问 UI 树；完成后只排队一次较短的 UI 页面构造与宿主尺寸准备。
        /// </summary>
        private async Task WarmUpAndQueueChartsAsync()
        {
            try
            {
                await ChartPage.WarmUpInBackgroundAsync().ConfigureAwait(false);
                if (!UIDispatcher.PostIdle(() =>
                    {
                        bool loaded = _appRouter.PreloadById("charts");
                        Volatile.Write(ref _chartsPreloadState, loaded ? 2 : 0);
                    }))
                {
                    Volatile.Write(ref _chartsPreloadState, 0);
                }
            }
            catch (Exception exception)
            {
                Volatile.Write(ref _chartsPreloadState, 0);
                // 预热失败只影响性能，不影响功能；保留调试信息并让下一次进入重新尝试。
                System.Diagnostics.Debug.WriteLine(
                    $"[Charts.BackgroundPreload] {exception}");
            }
        }

        /// <summary>
        /// KeepAlive 主界面离开时不释放页面；隐藏子树由框架自动停止布局、渲染与动画扫描。
        /// </summary>
        public void OnRouteLeave(string? toPath)
        {
            _ = toPath;
        }

        private static void Logout()
        {
            UISystem.Manager?.GetElementById<UICaptionBar>("demo-caption-bar")?.AddClass("login-caption-bar");
            UIRouterNavigator.NavigateById("login", replace: true);
            UISystem.SetWindowSize(1200, 800);
            UIMessage.Info("已退出登录");
        }
    }
}
