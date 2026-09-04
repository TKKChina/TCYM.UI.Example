using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using Page.Charts;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Message;

namespace TCYM.UI.Example.Page.Layout
{
    /// <summary>承载主菜单和组件示例内容，并在页面稳定后预加载图表路由。</summary>
    internal class Layout : UIView, IUIRouteLifecycle
    {
        private const int ChartsPreloadDelayMilliseconds = 300;
        private CancellationTokenSource? _chartsPreloadCancellation;

        internal Layout()
        {
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
                            "treeSelect" => "/demo/tree-select",
                            "checkbox" => "/demo/checkbox",
                            "radio" => "/demo/radio",
                            "datepicker" => "/demo/datepicker",
                            "timePicker" => "/demo/time-picker",
                            "colorPicker" => "/demo/color-picker",
                            "switch" => "/demo/switch",
                            "input" => "/demo/input",
                            "binding" => "/demo/binding",
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

        /// <summary>进入主界面后延迟预加载图表页，避免阻塞当前路由的首帧显示。</summary>
        /// <param name="fromPath">进入主界面前所在的路由路径。</param>
        public void OnRouteEnter(string? fromPath)
        {
            _ = fromPath;
            CancelChartsPreload();
            var cancellation = CancellationTokenSource.CreateLinkedTokenSource(LifetimeToken);
            _chartsPreloadCancellation = cancellation;
            _ = PreloadChartsAsync(cancellation);
        }

        /// <summary>离开主界面时取消尚未开始的图表预加载。</summary>
        /// <param name="toPath">主界面离开后进入的目标路由路径。</param>
        public void OnRouteLeave(string? toPath)
        {
            _ = toPath;
            CancelChartsPreload();
        }

        /// <summary>等待主界面首帧完成后，在 UI 线程创建一次性图表预加载页。</summary>
        private async Task PreloadChartsAsync(CancellationTokenSource cancellation)
        {
            try
            {
                await Task.Delay(ChartsPreloadDelayMilliseconds, cancellation.Token);
                cancellation.Token.ThrowIfCancellationRequested();
                await UIRouterNavigator.PreloadByIdAsync("charts");
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
            }
            finally
            {
                if (ReferenceEquals(_chartsPreloadCancellation, cancellation))
                {
                    _chartsPreloadCancellation = null;
                }
                cancellation.Dispose();
            }
        }

        /// <summary>取消当前尚未完成的图表预加载任务。</summary>
        private void CancelChartsPreload()
        {
            var cancellation = _chartsPreloadCancellation;
            _chartsPreloadCancellation = null;
            cancellation?.Cancel();
        }

        /// <summary>退出登录，并在登录页显示后释放尚未使用的图表预加载页。</summary>
        private void Logout()
        {
            CancelChartsPreload();
            UISystem.Manager?.GetElementById<UICaptionBar>("demo-caption-bar")?.AddClass("login-caption-bar");
            UIRouterNavigator.NavigateById("login", replace: true);
            UIDispatcher.Post(() => UIRouterNavigator.DiscardPreloadedById("charts"));
            UISystem.SetWindowSize(1200, 800);
            UIMessage.Info("已退出登录");
        }
    }
}
