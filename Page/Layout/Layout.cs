using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using Page.Charts;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Message;

namespace TCYM.UI.Example.Page.Layout
{
    /// <summary>承载主菜单和组件示例内容。</summary>
    internal class Layout : UIView
    {
        internal Layout()
        {
            var router = Router.Create();
            ClassName = "main-view";
            Children =
            [
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
                            "cascader" => "/demo/cascader",
                            "checkbox" => "/demo/checkbox",
                            "radio" => "/demo/radio",
                            "datepicker" => "/demo/datepicker",
                            "timePicker" => "/demo/time-picker",
                            "colorPicker" => "/demo/color-picker",
                            "switch" => "/demo/switch",
                            "input" => "/demo/input",
                            "autocomplete" => "/demo/autocomplete",
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
                            "loading" => "/demo/loading",
                            "notification" => "/demo/notification",
                            "popconfirm" => "/demo/popconfirm",
                            "modal" => "/demo/modal",
                            "progress" => "/demo/progress",
                            "chart" => "/charts",
                            "virtualScrollView" => "/demo/virtual-scroll-view",
                            "filePicker" => "/demo/file-picker",
                            "sdl3" => "/demo/sdl3",
                            "webView" => "/demo/web-view",
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
            ];

            //router.ReplaceById("demo_button");
        }

        /// <summary>退出登录，并在登录页显示后释放尚未使用的图表预加载页。</summary>
        private void Logout()
        {
            UISystem.Manager?.GetElementById<UICaptionBar>("demo-caption-bar")?.AddClass("login-caption-bar");
            UIRouterNavigator.NavigateById("login", replace: true);
            UIDispatcher.Post(() => UIRouterNavigator.DiscardPreloadedById("charts"));
            UISystem.SetWindowSize(1200, 800);
            UIMessage.Info("已退出登录");
        }
    }
}
