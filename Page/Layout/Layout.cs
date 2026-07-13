using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace TCYM.UI.Example.Page.Layout
{
    internal class Layout : UIView
    {
        internal Layout()
        {
            UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.Layout.style.css");
            var router = Router.Create();
            //router.Push("/demo/button");
            ClassName = new List<string> { "main-view" };
            Children = new()
            {
                new Menu
                {
                    SelectChange = (keys, item) =>  
                    { 
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
                            "watermark" => "/demo/watermark",
                            "virtualScrollView" => "/demo/virtual-scroll-view",
                            "filePicker" => "/demo/file-picker",
                            "usbCamera" => "/demo/usb-camera",
                            "sdl3" => "/demo/sdl3",
                            "gamepad" => "/demo/gamepad",
                            _ => "/demo/button"
                        };
                        router.Push(path);
                    }
                },
                new UIRouterView(router)
                {
                   ClassName = new List<string> { "router-view" }
                }
            };
        }
    }
}
