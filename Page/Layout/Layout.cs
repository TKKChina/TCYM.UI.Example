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
                            "slider" => "/demo/slider",
                            "tabs" => "/demo/tabs",
                            "pagination" => "/demo/pagination",
                            "dropdown" => "/demo/dropdown",
                            "select" => "/demo/select",
                            "checkbox" => "/demo/checkbox",
                            "radio" => "/demo/radio",
                            "datepicker" => "/demo/datepicker",
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
                            "virtualScrollView" => "/demo/virtual-scroll-view",
                            "filePicker" => "/demo/file-picker",
                            "usbCamera" => "/demo/usb-camera",
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
