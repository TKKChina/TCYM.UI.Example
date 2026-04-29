using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Menu;
using TCYM.UI.Elements.Message;
using TCYM.UI.Elements.Tooltip;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Menu
{
    public class UIMenuDemo : UIScrollView
    {
        const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Menu.style.css";
        internal UIMenuDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "menu-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "菜单组件",
                    ClassName = new List<string> { "menu-demo-title" }
                },
                new UILabel
                {
                    Text = "为页面和功能提供导航的菜单列表。",
                    ClassName = new List<string> { "menu-demo-title-sub" }
                },
                new UILabel
                {
                    Text = "菜单组件由 Menu 和 MenuItem 组成，Menu 组件是菜单的容器，MenuItem 组件是菜单项，可以嵌套使用。",
                    ClassName = new List<string> { "menu-demo-desc" }
                },
                new BaseMenuDemo(),
                new CustomMenuDemo()
            };
        }

        class BaseMenuDemo : UIView
        {
            internal BaseMenuDemo()
            {
                // 已经在 com.css 中注册了 IconFont 字体，这里不需要再次注册
                // UIFontFaceRegistry.Register("IconFont",UIFileHelper.ResolveAssetPath("Assets", "Fonts", "iconfont.ttf"));
                ClassName = new List<string> { "menu-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "基础菜单",
                        ClassName = new List<string> { "menu-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "基础菜单展示了最简单的菜单使用方式。",
                        ClassName = new List<string> { "menu-card-desc" }
                    },
                    new UIView
                    {
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            MarginTop = 10,
                            Display = "flex",
                            Gap = 8,
                            PaddingLeft = 10,
                        },
                        Children = new()
                        {
                            new UIMenu
                            {
                                Mode = MenuMode.Inline,
                                Theme = MenuTheme.Light,
                                Style = new UpdateUIStyle
                                {
                                    Width = 260
                                },
                                OpenKeys = new[] { "products" },
                                SelectedKeys = new[] { "button" },
                                Items = new List<MenuItem>
                                {
                                    MenuItem.Group("group-universal", "通用", new List<MenuItem>
                                    {
                                        MenuItem.Divider("group-universal-divider-top"),
                                        new("button", "按钮"){ Badge = new UIBadge { CountText = "更新",ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#28b5ec")} },
                                        new("icon", "Icon 图标"),
                                        new ("FloatButton",  "悬浮按钮"),
                                    }),
                                    MenuItem.Group("group-navigation", "导航", new List<MenuItem>
                                    {
                                            MenuItem.Divider("group-navigation-divider-top"),
                                            new("menu", "导航菜单"),
                                            new("Pagination", "分页"),
                                            new ("Splitter", "分隔面板"),
                                            new ("瀑布流", "瀑布流"),
                                            new ("Steps", "步骤条"),
                                            new ("Dropdown", "下拉菜单"),
                                    }),
                                    MenuItem.Group("group-setting", "系统设置", new List<MenuItem>
                                    {
                                        new("setting-user", "用户管理"),
                                        new("setting-role", "角色权限")
                                    }, addDivider: false),
                                    MenuItem.Divider(),
                                    new MenuItem("logout", "退出登录") { Icon = "🚪", Danger = true }
                                },
                                OnSelect = (keys, item) =>
                                {
                                    UIMessage.Info($"选中: {item.Key}, 选中的lable:{item.Label}");
                                }
                            },
                            new UIMenu
                            {
                                Id = "theme-menu-dome",
                                Mode= MenuMode.Inline,
                                Theme = MenuTheme.Dark,
                                Style = new UpdateUIStyle
                                {
                                    Width = 260
                                },
                                OpenKeys = new[] {"system-public","process-platform","Integrated-platform","business-platform" },
                                IconFontFamily = UIFontManager.Get("IconFontExample"),
                                Items = new()
                                {
                                    new MenuItem("system-public", "系统公共")
                                    {
                                        Icon = new UIIcon{
                                            Content = "&#xeb84;",
                                            Style = new DefaultUIStyle
                                            {
                                                FontFamily = UIFontManager.Get("IconFontExample"),
                                                Color = SKColors.DodgerBlue,
                                                FontSize = 16
                                            }
                                        },
                                        Children = new List<MenuItem>
                                        {
                                            new("system-manager", "系统管理"){ Icon = "&#xe65e;"},
                                            new("info-center", "消息中心"){ Icon = "&#xe9bb;"},
                                            new("system-config", "系统配置"){ Icon = "&#xe61b;"}
                                        }
                                    },
                                    new MenuItem("process-platform","流程平台")
                                    {
                                        Icon = new UIIcon
                                        {
                                            Content = "&#xe6c4;",
                                            Style = new DefaultUIStyle
                                            {
                                                FontFamily = UIFontManager.Get("IconFontExample"),
                                                Color = SKColors.Orange,
                                                FontSize = 16
                                            }
                                        },
                                        Children = new List<MenuItem>
                                        {
                                            new("process-manager", "工作流平台"){ Icon = "&#xe609;"},
                                            new("model-center", "任务中心"){ Icon = "&#xe608;"}
                                        }
                                    },
                                    new MenuItem("Integrated-platform", "集成平台")
                                    {
                                        Icon = new UIIcon
                                        {
                                            Content = "&#xe701;",
                                            Style = new DefaultUIStyle
                                            {
                                                FontFamily = UIFontManager.Get("IconFontExample"),
                                                Color = SKColors.Green,
                                                FontSize = 16
                                            }
                                        },
                                        Children = new List<MenuItem>
                                        {
                                            new("product-manager", "事件总线"){ Icon = "&#xe602;"},
                                            new("category-manager", "服务网关"){ Icon = "&#xe67a;"},
                                            new("brand-manager", "数据集成"){ Icon = "&#xe618;"},
                                        }
                                    },
                                    new MenuItem("business-platform", "大模型平台")
                                    {
                                        Icon = new UIIcon
                                        {
                                            Content = "&#xe75a;",
                                            Style = new DefaultUIStyle
                                            {
                                                FontFamily = UIFontManager.Get("IconFontExample"),
                                                Color = SKColors.Purple,
                                                FontSize = 16
                                            }
                                        },
                                        Children = new List<MenuItem>
                                        {
                                            new("event-manager", "能力纳管"){ Icon = "&#xe66a;"},
                                            new("task-manager", "智能对话平台"){ Icon = "&#xe606;"},
                                            new("report-manager", "智能体平台"){ Icon = "&#xe8be;"},
                                        }
                                    }
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "menu-theme-hint" },
                                Children = new()
                                {
                                    new UIButton
                                    {
                                        Text = "切换主题",
                                        Style = new DefaultUIStyle
                                        {
                                            Width = 120,
                                            Height = 32,
                                            BorderRadius = 4,
                                            BackgroundColor = ColorHelper.ParseColor("#1677ff"),
                                            Color = SKColors.White,
                                        },
                                        Events = new()
                                        {
                                            Click = (e) =>
                                            {
                                                var menu = UISystem.Manager?.GetElementById<UIMenu>("theme-menu-dome");
                                                if(menu != null)
                                                {
                                                    if(menu.Theme == MenuTheme.Light)
                                                    {
                                                        menu.Theme = MenuTheme.Dark;
                                                    }
                                                    else
                                                    {
                                                        menu.Theme = MenuTheme.Light;
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new UITooltip
                                    {
                                        Id = "theme-menu-tooltip",
                                        Title = "收起菜单",
                                        Children = new()
                                        {
                                            new UIIcon
                                            {
                                                Id = "theme-menu-toggle-icon",
                                                Content = "&#xe61d;",
                                                Style = new DefaultUIStyle
                                                {
                                                    Width = 32,
                                                    Height = 32,
                                                    Display = "flex",
                                                    AlignItems = "center",
                                                    JustifyContent = "center",
                                                    FontFamily = UIFontManager.Get("TCYMIconFont"),
                                                    Cursor = Enums.UICursor.Pointer,
                                                    BackgroundColor = ColorHelper.ParseColor("#1677ff"),
                                                    Color = SKColors.White,
                                                },
                                                Events = new()
                                                {
                                                    Click = _ =>
                                                    {
                                                        var menu = UISystem.Manager?.GetElementById<UIMenu>("theme-menu-dome");
                                                        var tooltip = UISystem.Manager?.GetElementById<UITooltip>("theme-menu-tooltip");
                                                        var icon = UISystem.Manager?.GetElementById<UIIcon>("theme-menu-toggle-icon");
                                                        if(menu != null && tooltip != null && icon != null)                                                        {
                                                            menu.InlineCollapsed = !menu.InlineCollapsed;
                                                            tooltip.Title = menu.InlineCollapsed ? "展开菜单" : "收起菜单";
                                                             icon.Content = menu.InlineCollapsed ? "&#xe621;" : "&#xe61d;";
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }

        class CustomMenuDemo : UIElement
        {
            internal CustomMenuDemo()
            {
                ClassName = new List<string> { "menu-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义样式菜单",
                        ClassName = new List<string> { "menu-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "自定义样式菜单展示了如何通过 MenuItem 的 Children 属性来自定义菜单内容。",
                        ClassName = new List<string> { "menu-card-desc" }
                    },
                    new UIView
                    {
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            MarginTop = 10,
                            Display = "flex",
                            Gap = 8,
                            PaddingLeft = 10,
                        },
                        Children = new()
                        {
                            new UIMenu
                            {
                                Mode= MenuMode.Inline,
                                Theme = MenuTheme.Light,
                                SelectStyle = new UpdateUIStyle
                                {
                                    BackgroundColor = ColorHelper.ParseColor("rgba(0,0,0,0.3)"),
                                    BorderRadius = 0,
                                    Color = SKColors.White,
                                    Hover = new DefaultUIStyle
                                    {
                                        BackgroundColor = ColorHelper.ParseColor("rgba(0,0,0,0.3)"),
                                        Color = SKColors.Yellow,
                                    },
                                    Before = new DefaultUIStyle
                                    {
                                        Content = "",
                                        Position = "absolute",
                                        Left = 0,
                                        Top = 0,
                                        Width = 5,
                                        Height = "100%",
                                        BackgroundColor = ColorHelper.ParseColor("#1677ff"),
                                        BorderRadius = 2.5f
                                    }
                                },
                                Style = new UpdateUIStyle
                                {
                                    Width = 260,
                                    BackgroundGradient = "linear-gradient( 314deg, #35C1FB 0%, #00DFE0 100%)",
                                },
                                OpenKeys = new[] { "process-platform" },
                                IconFontFamily = UIFontManager.Get("IconFontExample"),
                                Items = new()
                                {
                                    new MenuItem("system-public", "系统公共")
                                    {
                                        Icon = new UIIcon
                                        {
                                            Content = "&#xeb84;",
                                            Style = new DefaultUIStyle
                                            {
                                                FontFamily = UIFontManager.Get("IconFontExample"),
                                                Color = SKColors.DodgerBlue,
                                                FontSize = 16
                                            }
                                        },
                                        Children = new List<MenuItem>
                                        {
                                            new("system-manager", "系统管理"){ Icon = "&#xe65e;"},
                                            new("info-center", "消息中心"){ Icon = "&#xe9bb;"},
                                            new("system-config", "系统配置"){ Icon = "&#xe61b;"}
                                        }
                                    },
                                    new MenuItem("process-platform","流程平台")
                                    {
                                        Icon = new UIIcon
                                        {
                                            Content = "&#xe6c4;",
                                            Style = new DefaultUIStyle
                                            {
                                                FontFamily = UIFontManager.Get("IconFontExample"),
                                                Color = SKColors.Orange,
                                                FontSize = 16
                                            }
                                        },
                                        Children = new List<MenuItem>
                                        {
                                            new("process-manager", "工作流平台"){ Icon = "&#xe609;"},
                                            new("model-center", "任务中心"){ Icon = "&#xe608;"}
                                        }
                                    },
                                    new MenuItem("Integrated-platform", "集成平台")
                                    {
                                        Icon = new UIIcon
                                        {
                                            Content = "&#xe701;",
                                            Style = new DefaultUIStyle
                                            {
                                                FontFamily = UIFontManager.Get("IconFontExample"),
                                                Color = SKColors.Green,
                                                FontSize = 16
                                            }
                                        },
                                        Children = new List<MenuItem>
                                        {
                                            new("product-manager", "事件总线"){ Icon = "&#xe602;"},
                                            new("category-manager", "服务网关"){ Icon = "&#xe67a;"},
                                            new("brand-manager", "数据集成"){ Icon = "&#xe618;"},
                                        }
                                    },
                                    new MenuItem("business-platform", "大模型平台")
                                    {
                                        Icon = new UIIcon
                                        {
                                            Content = "&#xe75a;",
                                            Style = new DefaultUIStyle
                                            {
                                                FontFamily = UIFontManager.Get("IconFontExample"),
                                                Color = SKColors.Purple,
                                                FontSize = 16
                                            }
                                        },
                                        Children = new List<MenuItem>
                                        {
                                            new("event-manager", "能力纳管"){ Icon = "&#xe66a;"},
                                            new("task-manager", "智能对话平台"){ Icon = "&#xe606;"},
                                            new("report-manager", "智能体平台"){ Icon = "&#xe8be;"},
                                        }
                                    }
                                }
                            },
                            new UIMenu
                            {
                                Mode = MenuMode.Inline,
                                Theme = MenuTheme.Dark,
                                Style = new UpdateUIStyle
                                {
                                    Width = 260,
                                },
                                IconFontFamily = UIFontManager.Get("IconFontExample"),
                                Items = new()
                                {
                                    new MenuItem("custom", "自定义项")
                                    {
                                        ItemRender = (item, menu) =>
                                        {
                                            Console.WriteLine($"当前选中项: {menu.SelectedKeys.FirstOrDefault()}, 当前展开项: {string.Join(",", menu.OpenKeys)}");
                                            return new UIView
                                            {
                                                Style = new DefaultUIStyle
                                                {
                                                    Width = "100%",
                                                    Height = "100%",
                                                    Display = "flex",
                                                    AlignItems = "center",
                                                    JustifyContent = "space-between",
                                                    Gap = 6,
                                                    PaddingTop = 4,
                                                    PaddingBottom = 4,
                                                    PaddingLeft = 10,
                                                    PaddingRight = 30,
                                                    Cursor = Enums.UICursor.Pointer,
                                                },
                                                Children = new()
                                                {
                                                    new UIView
                                                    {
                                                        Style = new DefaultUIStyle
                                                        {
                                                            Height = "100%",
                                                            Display = "flex",
                                                            AlignItems = "center",
                                                        },
                                                        Children = new()
                                                        {
                                                            new UIIcon
                                                            {
                                                                Content = "&#xeb84;",
                                                                Style = new DefaultUIStyle
                                                                {
                                                                    FontFamily = UIFontManager.Get("IconFontExample"),
                                                                    Color = SKColors.DodgerBlue,
                                                                    FontSize = 16,
                                                                    MarginRight = 10
                                                                }
                                                            },
                                                            new UILabel
                                                            {
                                                                Text = item.Label,
                                                                Style = new DefaultUIStyle
                                                                {
                                                                    Color = SKColors.White,
                                                                }
                                                            }
                                                        }
                                                    },
                                                    new UIBadge
                                                    {
                                                        Count = 3,
                                                        ShowOutline = false,
                                                        BadgeColor = ColorHelper.ParseColor("#13c2c2"),
                                                        Children = new()
                                                        {
                                                            new UILabel
                                                            {
                                                                Text = "New",
                                                                Style = new DefaultUIStyle
                                                                {
                                                                    Color = SKColors.Green,
                                                                    FontSize = 11,
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            };
                                        },
                                        Children = new List<MenuItem>
                                        {
                                            new("system-manager", "系统管理"){ Icon = "&#xe65e;"},
                                            new("info-center", "消息中心"){ Icon = "&#xe9bb;"},
                                            new("system-config", "系统配置"){ Icon = "&#xe61b;"}
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}