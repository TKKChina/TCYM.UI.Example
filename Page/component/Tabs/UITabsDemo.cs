using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Message;
using TCYM.UI.Elements.Tabs;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Tabs
{
    internal class UITabsDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Tabs.style.css";

        internal UITabsDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "tabs-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Tabs 标签页组件",
                    ClassName = new List<string> { "tabs-demo-title" },
                },
                new UILabel
                {
                    Text = "选项卡切换组件。",
                    ClassName = new List<string> { "tabs-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "Tabs 组件用于在同一上下文中切换不同的内容视图，常用于分组展示相关内容或功能。",
                    ClassName = new List<string> { "tabs-demo-desc" },
                },
                new BasicTabsDemo(),
                new CustomStyleTabsDemo(),
                new StateTabsDemo(),
                new IconTabsDemo(),
                new PositionTabsDemo(),
            };
        }

        private static UIView CreatePane(string title, string desc, string stat, SKColor background)
        {
            return new UIView
            {
                ClassName = new List<string> { "tabs-pane-card" },
                Style = new DefaultUIStyle
                {
                    BackgroundColor = background,
                },
                Children = new()
                {
                    new UILabel
                    {
                        Text = title,
                        ClassName = new List<string> { "tabs-pane-title" },
                    },
                    new UILabel
                    {
                        Text = desc,
                        ClassName = new List<string> { "tabs-pane-desc" },
                    },
                    new UILabel
                    {
                        Text = stat,
                        ClassName = new List<string> { "tabs-pane-stat" },
                    }
                }
            };
        }

        private class BasicTabsDemo : UIView
        {
            internal BasicTabsDemo()
            {
                ClassName = new List<string> { "tabs-demo-card" };

                var activeLabel = new UILabel
                {
                    Text = "当前标签：概览",
                    ClassName = new List<string> { "tabs-value-label" }
                };

                var tabs = new UITabs
                {
                    ClassName = new List<string> { "tabs-showcase" },
                    DefaultActiveKey = "overview",
                    IndicatorColor = SKColor.Parse("#1677FF"),
                    ActiveTextColor = SKColor.Parse("#1677FF"),
                    OnTabChanged = item => activeLabel.Text = $"当前标签：{item.Label}",
                    ContentStyle = new DefaultUIStyle
                    {
                        Width = "100%",
                        PaddingTop = 12,
                        PaddingRight = 12,
                        PaddingBottom = 12,
                        PaddingLeft = 12,
                    },
                    Items = new List<TabItem>
                    {
                        new TabItem("overview", "概览", CreatePane("项目概览", "通过标签组织同级信息内容。", "活跃模块 12", SKColor.Parse("#F3F9FF"))),
                        new TabItem("analytics", "分析", CreatePane("数据分析", "切换不同内容区域而无需离开页面。", "今日新增 8", SKColor.Parse("#F6FFF7"))),
                        new TabItem("history", "历史", CreatePane("操作历史", "适合分组呈现多个上下文视图。", "记录 124条", SKColor.Parse("#FFF9F2"))),
                    }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "基础用法",
                        ClassName = new List<string> { "tabs-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "最常见的标签页场景。切换标签时只更新内容区域，不打断当前页面上下文。",
                        ClassName = new List<string> { "tabs-card-desc" }
                    },
                    tabs,
                    activeLabel,
                };
            }
        }

        private class CustomStyleTabsDemo : UIView
        {
            internal CustomStyleTabsDemo()
            {
                ClassName = new List<string> { "tabs-demo-card" };

                var tabs = new UITabs
                {
                    ClassName = new List<string> { "tabs-showcase", "tabs-custom-theme" },
                    DefaultActiveKey = "design",
                    TabTextColor = SKColor.Parse("#C7D2E5"),
                    ActiveTextColor = SKColors.White,
                    HoverTextColor = SKColor.Parse("#FFFFFF"),
                    IndicatorColor = SKColor.Parse("#7B61FF"),
                    ShowBaseLine = false,
                    ShowIndicator = false,
                    ContentStyle = new DefaultUIStyle
                    {
                        Width = "100%",
                        PaddingTop = 12,
                        PaddingRight = 12,
                        PaddingBottom = 12,
                        PaddingLeft = 12,
                    },
                    Items = new List<TabItem>
                    {
                        new TabItem("design", "设计", CreatePane("视觉设计", "通过 ClassName 使用外部 CSS 自定义 Tabs 风格。", "Theme: Aurora", SKColor.Parse("#F7F7FF"))),
                        new TabItem("motion", "动效", CreatePane("动效规范", "标签栏、内容面板、选中态都可以被外部样式覆盖。", "Duration: 240ms", SKColor.Parse("#F5FFFD"))),
                        new TabItem("token", "令牌", CreatePane("设计令牌", "适合做主题风格、品牌色与间距规范展示。", "Tokens: 32", SKColor.Parse("#FFF8F8"))),
                    }
                };

                var tabs_theme = new UITabs
                {
                    ClassName = new List<string> { "tabs-showcase", "tab_style_theme" },
                    DefaultActiveKey = "design",
                    ShowIndicator = false,
                    ShowBaseLine = false,
                    Items = new List<TabItem>
                    {
                        new TabItem("design", "设计", CreatePane("视觉设计", "通过 ClassName 使用外部 CSS 自定义 Tabs 风格。", "Theme: Aurora", SKColor.Parse("#F7F7FF"))),
                        new TabItem("motion", "动效", CreatePane("动效规范", "标签栏、内容面板、选中态都可以被外部样式覆盖。", "Duration: 240ms", SKColor.Parse("#F5FFFD"))),
                        new TabItem("token", "令牌", CreatePane("设计令牌", "适合做主题风格、品牌色与间距规范展示。", "Tokens: 32", SKColor.Parse("#FFF8F8"))),
                    }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义样式",
                        ClassName = new List<string> { "tabs-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "这一组演示外部 CSS 定制。通过给 UITabs 设置 ClassName，再使用 .tcym-tabs-nav、.tcym-tabs-tab、.tcym-tabs-tab-active 做主题覆盖。",
                        ClassName = new List<string> { "tabs-card-desc" }
                    },
                    tabs,
                    tabs_theme
                };
            }
        }

        private class StateTabsDemo : UIView
        {
            internal StateTabsDemo()
            {
                ClassName = new List<string> { "tabs-demo-card" };

                var tabs = new UITabs
                {
                    ClassName = new List<string> { "tabs-showcase", "tabs-state-theme" },
                    DefaultActiveKey = "message",
                    IndicatorColor = SKColor.Parse("#13A36E"),
                    ActiveTextColor = SKColor.Parse("#13A36E"),
                    BadgeColor = SKColor.Parse("#FF4D4F"),
                    DisabledTextColor = SKColor.Parse("#BBBBBB"),
                    ContentStyle = new DefaultUIStyle
                    {
                        Width = "100%",
                        Height = 220,
                        PaddingTop = 12,
                        PaddingRight = 12,
                        PaddingBottom = 12,
                        PaddingLeft = 12,
                    },
                    Items = new List<TabItem>
                    {
                        new TabItem("message", "消息", CreatePane("消息中心", "角标可以表示通知数量。", "未读 12 条", SKColor.Parse("#F3FFF8"))) { Badge = 12 },
                        new TabItem("task", "任务", CreatePane("待办任务", "禁用前也可以参与排序与分组。", "待处理 3 项", SKColor.Parse("#FFF9F2"))) { Badge = 3 },
                        new TabItem("archive", "归档", CreatePane("归档区", "禁用状态不会响应点击。", "Locked", SKColor.Parse("#F5F5F5"))) { Disabled = true },
                    }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "角标与禁用",
                        ClassName = new List<string> { "tabs-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "通过 Badge 表示数量，通过 Disabled 控制不可交互标签。适合消息中心、待办列表等场景。",
                        ClassName = new List<string> { "tabs-card-desc" }
                    },
                    tabs,
                };
            }
        }

        private class IconTabsDemo : UIView
        {
            internal IconTabsDemo()
            {
                ClassName = new List<string> { "tabs-demo-card" };

                var tabs = new UITabs
                {
                    ClassName = new List<string> { "tabs-showcase", "tabs-icon-theme" },
                    DefaultActiveKey = "home",
                    IndicatorColor = SKColor.Parse("#1677FF"),
                    ActiveTextColor = SKColor.Parse("#1677FF"),
                    IconFontFamily = UIFontManager.Get("IconFontExample"),
                    ContentStyle = new DefaultUIStyle
                    {
                        Width = "100%",
                        PaddingTop = 12,
                        PaddingRight = 12,
                        PaddingBottom = 12,
                        PaddingLeft = 12,
                    },
                    Items = new List<TabItem>
                    {
                        new TabItem("home", "主页", CreatePane("主页内容", "Tab 支持图标显示，配合文字或独立使用。", "&#xe68a;", SKColor.Parse("#F3F9FF"))) { IconContent = "&#xe973;" },
                        new TabItem("search", "搜索", CreatePane("搜索界面", "适合做功能分区，如主页、搜索、设置等。", "&#xe63f;", SKColor.Parse("#F6FFF7"))) { IconFontFamily = UIFontManager.Get("TCYMIconFont"), IconContent = "&#xe63f;" },
                        new TabItem("settings", "设置", CreatePane("设置中心", "也适合做工具面板的标签导航。", "&#xe60f;", SKColor.Parse("#FFF9F2"))) { Icon =new UIIcon { Content = "&#xe60f;", Style = new DefaultUIStyle
                                {
                                    FontFamily = UIFontManager.Get("TCYMIconFont"),
                                    Color = ColorHelper.ParseColor("#1677FF"),
                                    Display = "flex",
                                    AlignItems = "center",
                                    JustifyContent = "center",
                                    FontSize = 18,
                                    Width = 20,
                                    Height = 20,
                                }, } },
                    }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "图标标签",
                        ClassName = new List<string> { "tabs-card-title", "label-cany" }
                    },
                    new UILabel
                    {
                        Text = "这一组演示通过 IconFont 给 Tabs 添加图标。只需要在 TabItem 设置 Icon 属性，并给 UITabs 设置 IconFontFamily。",
                        ClassName = new List<string> { "tabs-card-desc" }
                    },
                    tabs,
                };
            }
        }

        private class PositionTabsDemo : UIView
        {
            internal PositionTabsDemo()
            {
                ClassName = new List<string> { "tabs-demo-card" };

                var grid = new UIView
                {
                    ClassName = new List<string> { "tabs-position-grid" },
                    Children = new()
                    {
                        CreatePositionDemo("顶部", TabPosition.Top, "top"),
                        CreatePositionDemo("底部", TabPosition.Bottom, "bottom"),
                        CreatePositionDemo("左侧", TabPosition.Left, "left"),
                        CreatePositionDemo("右侧", TabPosition.Right, "right"),
                    }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "标签位置",
                        ClassName = new List<string> { "tabs-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "TabPosition 支持 Top、Bottom、Left、Right 四种布局方向，适合不同类型的内容编排。",
                        ClassName = new List<string> { "tabs-card-desc" }
                    },
                    grid,
                };
            }

            private static UIView CreatePositionDemo(string title, TabPosition position, string keyPrefix)
            {
                return new UIView
                {
                    ClassName = new List<string> { "tabs-position-card" },
                    Children = new()
                    {
                        new UILabel
                        {
                            Text = title,
                            ClassName = new List<string> { "tabs-position-title" },
                        },
                        new UITabs
                        {
                            ClassName = new List<string> { "tabs-showcase", "tabs-position-widget" },
                            TabPosition = position,
                            DefaultActiveKey = keyPrefix + "-one",
                            IndicatorColor = SKColor.Parse("#7C4DFF"),
                            ActiveTextColor = SKColor.Parse("#7C4DFF"),
                            ContentStyle = new DefaultUIStyle
                            {
                                // Width = "100%",
                                // Height = 160,
                                PaddingTop = 10,
                                PaddingRight = 10,
                                PaddingBottom = 10,
                                PaddingLeft = 10,
                            },
                            Items = new List<TabItem>
                            {
                                new TabItem(keyPrefix + "-one", "概览", CreatePane("布局位置", "标签栏可切换到四个方向。", "Position Demo", SKColor.Parse("#F7F3FF"))),
                                new TabItem(keyPrefix + "-two", "配置", CreatePane("配置面板", "左右布局适合做工具面板。", "Adaptive", SKColor.Parse("#F2FBFF"))),
                                new TabItem(keyPrefix + "-three", "记录", CreatePane("记录区域", "上下布局更适合传统导航。", "Logs 28", SKColor.Parse("#FFF8F2"))),
                            }
                        }
                    }
                };
            }
        }
    }
}
