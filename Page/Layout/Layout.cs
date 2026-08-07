using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Message;

namespace TCYM.UI.Example.Page.Layout
{
    internal class Layout : UIView
    {
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
                            "svg" => "/demo/svg",
                            "message" => "/demo/message",
                            "modal" => "/demo/modal",
                            "progress" => "/demo/progress",
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

        private static void Logout()
        {
            UISystem.Manager?.GetElementById<UICaptionBar>("demo-caption-bar")?.AddClass("login-caption-bar");
            UIRouterNavigator.NavigateById("login", replace: true);
            UISystem.SetWindowSize(1200, 800);
            UIMessage.Info("已退出登录");
        }
    }
}
