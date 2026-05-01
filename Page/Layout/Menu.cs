using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Menu;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.Layout
{
    internal class Menu : UIScrollView
    {
        public Action<string[], MenuItem>? SelectChange { get; set; }
        internal Menu()
        {
            Style = new DefaultUIStyle
            {
                Width = 260,
                Height = "100%",
                OverflowX = "hidden",
            };
            Children = new()
            {
                new UIMenu
                {
                    Mode = MenuMode.Inline,
                    Theme = MenuTheme.Light,
                    Style = new UpdateUIStyle
                    {
                        Width = "100%",
                    },
                    IconFontFamily = UIFontManager.Get("IconFontExample"),
                    Items = new List<MenuItem>
                    {
                        MenuItem.Group("group-universal", "通用", new List<MenuItem>
                        {
                            MenuItem.Divider("group-universal-divider-top"),
                            new("button", "按钮"){ Icon = "&#xe690;" },
                            new("floatbutton", "悬浮按钮") { Icon = "&#xe649;",Badge = new UIBadge { CountText = "更新",ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#28b5ec")} },
                            new("icon", "Icon 图标") { Icon = "&#xe60a;" },
                            new("label", "文本") { Icon = "&#xe651;"},
                        }),
                        MenuItem.Group("group-layout", "布局", new List<MenuItem>
                        {
                            MenuItem.Divider("group-layout-divider-top"),
                            new("splitter", "分隔面板") { Icon = "&#xe6cc;", Badge = new UIBadge { CountText = "NEW", ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#34f50d") } },
                        }),
                        MenuItem.Group("group-navigation", "导航", new List<MenuItem>
                        {
                            MenuItem.Divider("group-navigation-divider-top"),
                            new("menu", "导航菜单") { Icon = "&#xe607;"},
                            new("pagination", "分页") { Icon = "&#xe697;" },
                            new ("dropdown", "下拉菜单") { Icon = "&#xe695;" },
                            new ("tabs", "标签页") { Icon = "&#xe6d4;" }
                        }),
                        MenuItem.Group("group-data-entry", "数据录入", new List<MenuItem>
                        {
                            MenuItem.Divider("group-data-entry-divider-top"),
                            new("slider", "滑动条") { Icon = "&#xe6d1;" },
                            new("select", "选择器") { Icon = "&#xe70b;" },
                            new("checkbox", "多选框") { Icon = "&#xe66d;" },
                            new("radio", "单选框") { Icon = "&#xe71f;" },
                            new("datepicker", "日期选择框") { Icon = "&#xe629;" },
                            new("switch", "开关") { Icon = "&#xed5f;" },
                            new("input", "输入框") { Icon = "&#xe790;" },
                            new("codeEditor", "代码编辑器") { Icon = "&#xe61d;"},
                        }),
                        MenuItem.Group("group-data-display", "数据展示", new List<MenuItem>
                        {
                            MenuItem.Divider("group-data-display-divider-top"),
                            new("badge", "徽标数") { Icon = "&#xe61c;" },
                            new("image", "图片") { Icon = "&#xe60d;", Badge = new UIBadge { CountText = "更新",ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#28b5ec")} },
                            new("virtualScrollView", "虚拟滚动") { Icon = "&#xe610;"},
                            new("tooltip", "文字提示") { Icon = "&#xe6e0;" },
                            new("tag", "标签") { Icon = "&#xe6a7;" },
                            new("table", "表格") { Icon = "&#xe6a9;",Badge = new UIBadge { CountText = "更新",ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#28b5ec")} },
                            new("tree", "树形") { Icon = "&#xe67b;" },
                            new("svg", "SVG 矢量图") { Icon = "&#xeba4;" },
                        }),
                        MenuItem.Group("group-feedback", "反馈", new List<MenuItem>
                        {
                            MenuItem.Divider("group-feedback-divider-top"),
                            new("message", "消息") { Icon = "&#xe671;" },
                        }),
                        MenuItem.Group("group-other", "其他", new List<MenuItem>
                        {
                            MenuItem.Divider("group-other-divider-top"),
                            new("filePicker", "文件选择") { Icon = "&#xea3e;"},
                            new("usbCamera", "USB 摄像头") { Icon = "&#xe965;" },
                        })
                    },
                    OnSelect = (keys, item) =>
                    {
                        SelectChange?.Invoke(keys, item);
                    }
                }
            };
        }
    }
}
