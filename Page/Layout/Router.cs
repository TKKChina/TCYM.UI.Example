using TCYM.UI.Core.Routing;
using TCYM.UI.Example.Page.component.Button;
using TCYM.UI.Example.Page.component.Dropdown;
using TCYM.UI.Example.Page.component.Icon;
using TCYM.UI.Example.Page.component.Menu;
using TCYM.UI.Example.Page.component.Slider;
using TCYM.UI.Example.Page.component.Tabs;
using TCYM.UI.Example.Page.component.Pagination;
using TCYM.UI.Example.Page.component.Select;
using TCYM.UI.Example.Page.component.Checkbox;
using TCYM.UI.Example.Page.component.Radio;
using TCYM.UI.Example.Page.component.DatePicker;
using TCYM.UI.Example.Page.component.Switch;
using TCYM.UI.Example.Page.component.Input;
using TCYM.UI.Example.Page.component.Tree;
using TCYM.UI.Example.Page.component.Tooltip;
using TCYM.UI.Example.Page.component.Badge;
using TCYM.UI.Example.Page.component.FloatButton;
using TCYM.UI.Example.Page.component.Tag;
using TCYM.UI.Example.Page.component.Svg;
using TCYM.UI.Example.Page.component.Table;
using TCYM.UI.Example.Page.component.Message;
using TCYM.UI.Example.Page.component.Label;
using TCYM.UI.Example.Page.component.UsbCamera;
using TCYM.UI.Example.Page.component.VirtualScrollView;
using TCYM.UI.Example.Page.component.FilePicker;
using TCYM.UI.Example.Page.component.Image;
using TCYM.UI.Example.Page.component.CodeEditor;
using TCYM.UI.Example.Page.component.Splitter;

namespace TCYM.UI.Example.Page.Layout
{
    internal static class Router
    {
        public static UIRouter Create()
        {
            var router = new UIRouter()
            .Register("/demo/button", () => new UIButtonDemo())
            .Register("/demo/float-button", () => new UIFloatButtonDemo())
            .Register("/demo/icon", () => new UIIconDemo())
            .Register("/demo/label", () => new UILabelDemo())
            .Register("/demo/image", () => new UIImageDemo(),false)
            .Register("/demo/menu", () => new UIMenuDemo())
            .Register("/demo/slider", () => new UISliderDemo())
            .Register("/demo/splitter", () => new UISplitterDemo())
            .Register("/demo/tabs", () => new UITabsDemo())
            .Register("/demo/pagination", () => new UIPaginationDemo())
            .Register("/demo/select", () => new UISelectDemo())
            .Register("/demo/dropdown", () => new UIDropdownDemo())
            .Register("/demo/checkbox", () => new UICheckboxDemo())
            .Register("/demo/radio", () => new UIRadioDemo())
            .Register("/demo/datepicker", () => new UIDatePickerDemo())
            .Register("/demo/switch", () => new UISwitchDemo())
            .Register("/demo/input", () => new UIInputDemo())
            .Register("/demo/code-editor", () => new UICodeEditorDemo())
            .Register("/demo/tree", () => new UITreeDemo())
            .Register("/demo/tooltip", () => new UITooltipDemo())
            .Register("/demo/badge", () => new UIBadgeDemo())
            .Register("/demo/tag", () => new UITagDemo())
            .Register("/demo/svg", () => new UISvgDemo())
            .Register("/demo/table", () => new UITableDemo())
            .Register("/demo/message", () => new UIMessageDemo())
            .Register("/demo/virtual-scroll-view", () => new UIVirtualScrollViewDemo())
            .Register("/demo/file-picker", () => new UIFilePickerDemo())
            .Register("/demo/usb-camera", () => new UIUsbCameraDemo());
            
            return router;
        }
    }
}
