using TCYM.UI.Core.Routing;
using TCYM.UI.Example.Page.component.Button;
using TCYM.UI.Example.Page.component.Dropdown;
using TCYM.UI.Example.Page.component.Icon;
using TCYM.UI.Example.Page.component.Menu;
using TCYM.UI.Example.Page.component.Segmented;
using TCYM.UI.Example.Page.component.Slider;
using TCYM.UI.Example.Page.component.Tabs;
using TCYM.UI.Example.Page.component.Pagination;
using TCYM.UI.Example.Page.component.Select;
using TCYM.UI.Example.Page.component.Checkbox;
using TCYM.UI.Example.Page.component.Radio;
using TCYM.UI.Example.Page.component.DatePicker;
using TCYM.UI.Example.Page.component.TimePicker;
using TCYM.UI.Example.Page.component.Switch;
using TCYM.UI.Example.Page.component.Input;
using TCYM.UI.Example.Page.component.Tree;
using TCYM.UI.Example.Page.component.TreeSelect;
using TCYM.UI.Example.Page.component.Tooltip;
using TCYM.UI.Example.Page.component.Badge;
using TCYM.UI.Example.Page.component.FloatButton;
using TCYM.UI.Example.Page.component.Tag;
using TCYM.UI.Example.Page.component.Svg;
using TCYM.UI.Example.Page.component.Table;
using TCYM.UI.Example.Page.component.Message;
using TCYM.UI.Example.Page.component.Notification;
using TCYM.UI.Example.Page.component.Modal;
using TCYM.UI.Example.Page.component.Label;
using TCYM.UI.Example.Page.component.UsbCamera;
using TCYM.UI.Example.Page.component.VirtualScrollView;
using TCYM.UI.Example.Page.component.FilePicker;
using TCYM.UI.Example.Page.component.Image;
using TCYM.UI.Example.Page.component.CodeEditor;
using TCYM.UI.Example.Page.component.Splitter;
using TCYM.UI.Example.Page.component.Watermark;
using TCYM.UI.Example.Page.component.Sdl3;
using TCYM.UI.Example.Page.component.Gamepad;
using TCYM.UI.Example.Page.component.Carousel;
using TCYM.UI.Example.Page.component.Progress;
using TCYM.UI.Example.Page.component.ColorPicker;
using TCYM.UI.Example.Page.component.Anchor;
using TCYM.UI.Example.Page.component.Glass;
using TCYM.UI.Example.Page.component.Timeline;
using TCYM.UI.Elements;

namespace TCYM.UI.Example.Page.Layout
{
    internal static class Router
    {
        public static UIRouter Create()
        {
            UIRouteRecord[] routes =
            [
                new()
                {
                    Id = "demo",
                    Path = "/demo",
                    Name = "组件示例",
                    Element = UIRoute.LazyLoad(() => new UIView()),
                    Children =
                    [
                        new() { Id = "demo_button", Path = "button", Name = "Button", Element = UIRoute.LazyLoad(() => new UIButtonDemo()) },
                        new() { Id = "demo_anchor", Path = "anchor", Name = "Anchor", Element = UIRoute.LazyLoad(() => new UIAnchorDemo()) },
                        new() { Id = "demo_float_button", Path = "float-button", Name = "FloatButton", Element = UIRoute.LazyLoad(() => new UIFloatButtonDemo()) },
                        new() { Id = "demo_icon", Path = "icon", Name = "Icon", Element = UIRoute.LazyLoad(() => new UIIconDemo()) },
                        new() { Id = "demo_label", Path = "label", Name = "Label", Element = UIRoute.LazyLoad(() => new UILabelDemo()) },
                        new() { Id = "demo_glass", Path = "glass", Name = "CSS Glass", Element = UIRoute.LazyLoad(() => new UIGlassDemo()), KeepAlive = false },
                        new() { Id = "demo_image", Path = "image", Name = "Image", Element = UIRoute.LazyLoad(() => new UIImageDemo()), KeepAlive = false },
                        new() { Id = "demo_menu", Path = "menu", Name = "Menu", Element = UIRoute.LazyLoad(() => new UIMenuDemo()) },
                        new() { Id = "demo_segmented", Path = "segmented", Name = "Segmented", Element = UIRoute.LazyLoad(() => new UISegmentedDemo()) },
                        new() { Id = "demo_slider", Path = "slider", Name = "Slider", Element = UIRoute.LazyLoad(() => new UISliderDemo()) },
                        new() { Id = "demo_splitter", Path = "splitter", Name = "Splitter", Element = UIRoute.LazyLoad(() => new UISplitterDemo()) },
                        new() { Id = "demo_tabs", Path = "tabs", Name = "Tabs", Element = UIRoute.LazyLoad(() => new UITabsDemo()) },
                        new() { Id = "demo_carousel", Path = "carousel", Name = "Carousel", Element = UIRoute.LazyLoad(() => new UICarouselDemo()), KeepAlive = false },
                        new() { Id = "demo_pagination", Path = "pagination", Name = "Pagination", Element = UIRoute.LazyLoad(() => new UIPaginationDemo()) },
                        new() { Id = "demo_select", Path = "select", Name = "Select", Element = UIRoute.LazyLoad(() => new UISelectDemo()) },
                        new() { Id = "demo_tree_select", Path = "tree-select", Name = "TreeSelect", Element = UIRoute.LazyLoad(() => new UITreeSelectDemo()) },
                        new() { Id = "demo_dropdown", Path = "dropdown", Name = "Dropdown", Element = UIRoute.LazyLoad(() => new UIDropdownDemo()) },
                        new() { Id = "demo_checkbox", Path = "checkbox", Name = "Checkbox", Element = UIRoute.LazyLoad(() => new UICheckboxDemo()) },
                        new() { Id = "demo_radio", Path = "radio", Name = "Radio", Element = UIRoute.LazyLoad(() => new UIRadioDemo()) },
                        new() { Id = "demo_datepicker", Path = "datepicker", Name = "DatePicker", Element = UIRoute.LazyLoad(() => new UIDatePickerDemo()) },
                        new() { Id = "demo_timepicker", Path = "time-picker", Name = "TimePicker", Element = UIRoute.LazyLoad(() => new UITimePickerDemo()) },
                        new() { Id = "demo_color_picker", Path = "color-picker", Name = "ColorPicker", Element = UIRoute.LazyLoad(() => new UIColorPickerDemo()) },
                        new() { Id = "demo_switch", Path = "switch", Name = "Switch", Element = UIRoute.LazyLoad(() => new UISwitchDemo()) },
                        new() { Id = "demo_input", Path = "input", Name = "Input", Element = UIRoute.LazyLoad(() => new UIInputDemo()) },
                        new() { Id = "demo_code_editor", Path = "code-editor", Name = "CodeEditor", Element = UIRoute.LazyLoad(() => new UICodeEditorDemo()) },
                        new() { Id = "demo_tree", Path = "tree", Name = "Tree", Element = UIRoute.LazyLoad(() => new UITreeDemo()) },
                        new() { Id = "demo_tooltip", Path = "tooltip", Name = "Tooltip", Element = UIRoute.LazyLoad(() => new UITooltipDemo()) },
                        new() { Id = "demo_badge", Path = "badge", Name = "Badge", Element = UIRoute.LazyLoad(() => new UIBadgeDemo()) },
                        new() { Id = "demo_tag", Path = "tag", Name = "Tag", Element = UIRoute.LazyLoad(() => new UITagDemo()) },
                        new() { Id = "demo_svg", Path = "svg", Name = "Svg", Element = UIRoute.LazyLoad(() => new UISvgDemo()) },
                        new() { Id = "demo_table", Path = "table", Name = "Table", Element = UIRoute.LazyLoad(() => new UITableDemo()) },
                        new() { Id = "demo_timeline", Path = "timeline", Name = "Timeline", Element = UIRoute.LazyLoad(() => new UITimelineDemo()) },
                        new() { Id = "demo_message", Path = "message", Name = "Message", Element = UIRoute.LazyLoad(() => new UIMessageDemo()) },
                        new() { Id = "demo_notification", Path = "notification", Name = "Notification", Element = UIRoute.LazyLoad(() => new UINotificationDemo()), KeepAlive = false },
                        new() { Id = "demo_modal", Path = "modal", Name = "Modal", Element = UIRoute.LazyLoad(() => new UIModalDemo()) },
                        new() { Id = "demo_progress", Path = "progress", Name = "Progress", Element = UIRoute.LazyLoad(() => new UIProgressDemo()) },
                        new() { Id = "demo_watermark", Path = "watermark", Name = "Watermark", Element = UIRoute.LazyLoad(() => new UIWatermarkDemo()) },
                        new() { Id = "demo_virtual_scroll_view", Path = "virtual-scroll-view", Name = "VirtualScrollView", Element = UIRoute.LazyLoad(() => new UIVirtualScrollViewDemo()) },
                        new() { Id = "demo_file_picker", Path = "file-picker", Name = "FilePicker", Element = UIRoute.LazyLoad(() => new UIFilePickerDemo()) },
                        new() { Id = "demo_sdl3", Path = "sdl3", Name = "SDL3", Element = UIRoute.LazyLoad(() => new UISdl3FeaturesDemo()) },
                        new() { Id = "demo_gamepad", Path = "gamepad", Name = "Gamepad", Element = UIRoute.LazyLoad(() => new UIGamepadDemo()) },
                        new() { Id = "demo_usb_camera", Path = "usb-camera", Name = "UsbCamera", Element = UIRoute.LazyLoad(() => new UIUsbCameraDemo()) }
                    ]
                }
            ];

            return new UIRouter(routes);
        }
    }
}
