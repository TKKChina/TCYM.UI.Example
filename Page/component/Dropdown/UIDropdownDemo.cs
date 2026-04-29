using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Enums;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Dropdown
{
    internal class UIDropdownDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Dropdown.style.css";

        internal UIDropdownDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "dropdown-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Dropdown 下拉菜单",
                    ClassName = new List<string> { "dropdown-demo-title" },
                },
                new UILabel
                {
                    Text = "向下弹出的菜单列表。",
                    ClassName = new List<string> { "dropdown-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "当页面上的操作命令过多时，用此组件可以收纳操作元素。点击触点，会出现一个下拉菜单。可在列表中进行选择，并执行相应的命令。",
                    ClassName = new List<string> { "dropdown-demo-desc" },
                },
                new BasicSection(),
                new ArrowSection(),
                new CustomStyleSection(),
                new EventSection(),
                new PlacementSection(),
            };
        }

        private static List<UIElement> CreateBasicMenu()
        {
            return new()
            {
                CreateMenuItem("菜单项一"),
                CreateMenuItem("菜单项二"),
                CreateMenuItem("菜单项三"),
            };
        }

        private static UILabel CreateMenuItem(string text, UICursor cursor = UICursor.Pointer)
        {
            return new UILabel
            {
                Text = text,
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = 30,
                    PaddingLeft = 8,
                    PaddingRight = 8,
                    Display = "flex",
                    AlignItems = "center",
                    Cursor = cursor,
                    BorderRadius = 4,
                    Hover = new DefaultUIStyle
                    {
                        BackgroundColor = ColorHelper.ParseColor("rgba(0, 0, 0, 0.06)")
                    }
                }
            };
        }

        private static UIView CreateTriggerButton(string text, string colorHex = "#1677ff")
        {
            var color = ColorHelper.ParseColor(colorHex);
            return new UIView
            {
                Style = new DefaultUIStyle
                {
                    Display = "flex",
                    FlexDirection = "row",
                    AlignItems = "center",
                    Gap = 4,
                    PaddingLeft = 12,
                    PaddingRight = 12,
                    PaddingTop = 5,
                    PaddingBottom = 5,
                    BorderRadius = 6,
                    Cursor = UICursor.Pointer,
                    BackgroundColor = color,
                },
                Children = new()
                {
                    new UILabel
                    {
                        Text = text,
                        Style = new DefaultUIStyle
                        {
                            Color = SKColors.White,
                            FontSize = 14,
                            PointerEvents = "none",
                        }
                    },
                    new UIIcon
                    {
                        Content = "&#xe601;",
                        Style = new DefaultUIStyle
                        {
                            FontFamily = UIFontManager.Get("TCYMIconFont"),
                            FontSize = 12,
                            Color = SKColors.White,
                            PointerEvents = "none",
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 基本使用
        /// </summary>
        private class BasicSection : UIView
        {
            internal BasicSection()
            {
                ClassName = new List<string> { "dropdown-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "基本使用",
                        ClassName = new List<string> { "dropdown-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "最简单的下拉菜单，点击触发按钮弹出菜单列表。",
                        ClassName = new List<string> { "dropdown-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "dropdown-showcase" },
                        Children = new()
                        {
                            new UIDropdown
                            {
                                Menu = CreateBasicMenu(),
                                MenuWidth = 140,
                                MenuHeight = 108,
                                Children = new() { CreateTriggerButton("基础下拉") },
                            },
                            new UIDropdown
                            {
                                Menu = new()
                                {
                                    CreateMenuItem("操作一"),
                                    CreateMenuItem("操作二"),
                                    CreateMenuItem("操作三"),
                                    CreateMenuItem("操作四"),
                                    CreateMenuItem("操作五"),
                                },
                                MenuWidth = 140,
                                MenuHeight = 168,
                                CloseOnContentClick = false,
                                Children = new()
                                {
                                    CreateTriggerButton("点击菜单不关闭", "#52c41a"),
                                },
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 箭头控制
        /// </summary>
        private class ArrowSection : UIView
        {
            internal ArrowSection()
            {
                ClassName = new List<string> { "dropdown-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "箭头控制",
                        ClassName = new List<string> { "dropdown-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "通过 IsArrow 属性控制是否显示下拉菜单的指示箭头。",
                        ClassName = new List<string> { "dropdown-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "dropdown-showcase" },
                        Children = new()
                        {
                            new UIDropdown
                            {
                                Menu = CreateBasicMenu(),
                                IsArrow = true,
                                MenuWidth = 140,
                                MenuHeight = 108,
                                Children = new() { CreateTriggerButton("显示箭头") },
                            },
                            new UIDropdown
                            {
                                Menu = CreateBasicMenu(),
                                IsArrow = false,
                                MenuWidth = 140,
                                MenuHeight = 108,
                                Children = new() { CreateTriggerButton("隐藏箭头", "#fa541c") },
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 自定义样式
        /// </summary>
        private class CustomStyleSection : UIView
        {
            internal CustomStyleSection()
            {
                ClassName = new List<string> { "dropdown-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义样式",
                        ClassName = new List<string> { "dropdown-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "通过 DropdownStyle 和 ArrowStyle 自定义下拉面板和箭头的样式。",
                        ClassName = new List<string> { "dropdown-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "dropdown-showcase" },
                        Children = new()
                        {
                            new UIDropdown
                            {
                                Menu = new()
                                {
                                    new UILabel
                                    {
                                        Text = "自定义菜单",
                                        Style = new DefaultUIStyle
                                        {
                                            Color = SKColors.White,
                                            FontSize = 13,
                                            Width = "100%",
                                            Height = 30,
                                            Display = "flex",
                                            AlignItems = "center",
                                            PaddingLeft = 8,
                                        }
                                    },
                                    new UIView
                                    {
                                        Style = new DefaultUIStyle
                                        {
                                            Width = "100%",
                                            Height = 1,
                                            BackgroundColor = ColorHelper.ParseColor("rgba(255,255,255,0.2)"),
                                            MarginTop = 4,
                                            MarginBottom = 4,
                                        }
                                    },
                                    new UILabel
                                    {
                                        Text = "深色主题项一",
                                        Style = new DefaultUIStyle
                                        {
                                            Color = ColorHelper.ParseColor("rgba(255,255,255,0.85)"),
                                            Width = "100%",
                                            Height = 30,
                                            PaddingLeft = 8,
                                            Display = "flex",
                                            AlignItems = "center",
                                            Cursor = UICursor.Pointer,
                                            BorderRadius = 4,
                                            Hover = new DefaultUIStyle
                                            {
                                                BackgroundColor = ColorHelper.ParseColor("rgba(255,255,255,0.1)")
                                            }
                                        }
                                    },
                                    new UILabel
                                    {
                                        Text = "深色主题项二",
                                        Style = new DefaultUIStyle
                                        {
                                            Color = ColorHelper.ParseColor("rgba(255,255,255,0.85)"),
                                            Width = "100%",
                                            Height = 30,
                                            PaddingLeft = 8,
                                            Display = "flex",
                                            AlignItems = "center",
                                            Cursor = UICursor.Pointer,
                                            BorderRadius = 4,
                                            Hover = new DefaultUIStyle
                                            {
                                                BackgroundColor = ColorHelper.ParseColor("rgba(255,255,255,0.1)")
                                            }
                                        }
                                    },
                                },
                                MenuWidth = 160,
                                MenuHeight = 140,
                                DropdownStyle = new DefaultUIStyle
                                {
                                    Width = "100%",
                                    Height = "100%",
                                    BackgroundColor = ColorHelper.ParseColor("#1f1f1f"),
                                    BorderRadius = 8,
                                    BorderWidth = 0,
                                    PaddingLeft = 8,
                                    PaddingRight = 8,
                                    PaddingTop = 5,
                                    PaddingBottom = 8,
                                    Display = "flex",
                                    FlexDirection = "column",
                                },
                                ArrowStyle = new DefaultUIStyle
                                {
                                    Width = 10,
                                    Height = 10,
                                    Position = "absolute",
                                    Top = -5,
                                    Left = 16,
                                    BackgroundColor = ColorHelper.ParseColor("#1f1f1f"),
                                    TransformCss = "rotate(45deg)",
                                    ZIndex = 9990,
                                },
                                Children = new() { CreateTriggerButton("深色主题", "#722ed1") },
                            },
                            new UIDropdown
                            {
                                Menu = CreateBasicMenu(),
                                MenuWidth = 160,
                                MenuHeight = 108,
                                IsArrow = false,
                                DropdownStyle = new DefaultUIStyle
                                {
                                    Width = "100%",
                                    Height = "100%",
                                    BackgroundColor = ColorHelper.ParseColor("#fffbe6"),
                                    BorderRadius = 8,
                                    BorderColor = ColorHelper.ParseColor("#ffe58f"),
                                    BorderWidth = 1,
                                    PaddingLeft = 8,
                                    PaddingRight = 8,
                                    PaddingTop = 5,
                                    PaddingBottom = 8,
                                    Display = "flex",
                                    FlexDirection = "column",
                                },
                                Children = new() { CreateTriggerButton("暖色面板", "#faad14") },
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 事件回调
        /// </summary>
        private class EventSection : UIView
        {
            internal EventSection()
            {
                ClassName = new List<string> { "dropdown-demo-card" };

                var statusLabel = new UILabel
                {
                    Text = "状态：已关闭",
                    Style = new DefaultUIStyle
                    {
                        FontSize = 13,
                        Color = ColorHelper.ParseColor("rgba(0,0,0,0.65)"),
                    }
                };

                var clickLabel = new UILabel
                {
                    Text = "点击：无",
                    Style = new DefaultUIStyle
                    {
                        FontSize = 13,
                        Color = ColorHelper.ParseColor("rgba(0,0,0,0.65)"),
                    }
                };

                UILabel MakeClickableItem(string text)
                {
                    var item = CreateMenuItem(text);
                    item.OnClick += _ =>
                    {
                        clickLabel.Text = $"点击：{text}";
                    };
                    return item;
                }

                Children = new()
                {
                    new UILabel
                    {
                        Text = "事件回调",
                        ClassName = new List<string> { "dropdown-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "通过 OnOpenChange 监听菜单显示状态变化，点击菜单项可触发自定义操作。",
                        ClassName = new List<string> { "dropdown-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "dropdown-showcase" },
                        Children = new()
                        {
                            new UIDropdown
                            {
                                Menu = new()
                                {
                                    MakeClickableItem("新建文件"),
                                    MakeClickableItem("打开项目"),
                                    MakeClickableItem("保存全部"),
                                },
                                MenuWidth = 140,
                                MenuHeight = 108,
                                OnOpenChange = open =>
                                {
                                    statusLabel.Text = $"状态：{(open ? "已打开" : "已关闭")}";
                                },
                                Children = new() { CreateTriggerButton("事件监听", "#eb2f96") },
                            },
                            statusLabel,
                            clickLabel,
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 溢出翻转
        /// </summary>
        private class PlacementSection : UIView
        {
            internal PlacementSection()
            {
                ClassName = new List<string> { "dropdown-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "溢出自动翻转",
                        ClassName = new List<string> { "dropdown-card-title", "label-cany" }
                    },
                    new UILabel
                    {
                        Text = "当下拉菜单超出窗口底部时，AutoAdjustOverflow 会自动将菜单翻转到触发器上方显示。",
                        ClassName = new List<string> { "dropdown-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "dropdown-showcase" },
                        Children = new()
                        {
                            new UIDropdown
                            {
                                Menu = new()
                                {
                                    CreateMenuItem("选项 A"),
                                    CreateMenuItem("选项 B"),
                                    CreateMenuItem("选项 C"),
                                    CreateMenuItem("选项 D"),
                                    CreateMenuItem("选项 E"),
                                    CreateMenuItem("选项 F"),
                                },
                                MenuWidth = 140,
                                MenuHeight = 200,
                                AutoAdjustOverflow = true,
                                Children = new() { CreateTriggerButton("自动翻转", "#13c2c2") },
                            },
                            new UIDropdown
                            {
                                Menu = new()
                                {
                                    CreateMenuItem("选项 A"),
                                    CreateMenuItem("选项 B"),
                                    CreateMenuItem("选项 C"),
                                    CreateMenuItem("选项 D"),
                                    CreateMenuItem("选项 E"),
                                    CreateMenuItem("选项 F"),
                                },
                                MenuWidth = 140,
                                MenuHeight = 200,
                                AutoAdjustOverflow = false,
                                Children = new() { CreateTriggerButton("不翻转", "#595959") },
                            },
                        }
                    },
                };
            }
        }
    }
}
