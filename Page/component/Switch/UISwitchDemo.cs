using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Input;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Switch
{
    internal class UISwitchDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Switch.style.css";

        internal UISwitchDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "switch-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Switch 开关",
                    ClassName = new List<string> { "switch-demo-title" },
                },
                new UILabel
                {
                    Text = "开关选择器。",
                    ClassName = new List<string> { "switch-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "需要表示开关状态/两种状态之间的切换时使用。和 Checkbox 的区别是，切换 Switch 会直接触发状态变更，而 Checkbox 一般用于状态标记。",
                    ClassName = new List<string> { "switch-demo-desc" },
                },
                new BasicSection(),
                new DisabledSection(),
                new CustomTextSection(),
                new IconSection(),
                new StyleSection(),
                new EventSection(),
            };
        }

        /// <summary>
        /// 基本用法
        /// </summary>
        private class BasicSection : UIView
        {
            internal BasicSection()
            {
                ClassName = new List<string> { "switch-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "基本用法",
                        ClassName = new List<string> { "switch-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "最简单的用法，通过 DefaultChecked 设置默认选中状态。",
                        ClassName = new List<string> { "switch-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "switch-showcase" },
                        Children = new()
                        {
                            new UISwitch
                            {
                                DefaultChecked = true,
                                RenderChildrenContent = false,
                            },
                            new UISwitch
                            {
                                DefaultChecked = false,
                                RenderChildrenContent = false,
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 禁用状态
        /// </summary>
        private class DisabledSection : UIView
        {
            internal DisabledSection()
            {
                ClassName = new List<string> { "switch-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "禁用状态",
                        ClassName = new List<string> { "switch-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "通过 Disabled 属性禁用开关。",
                        ClassName = new List<string> { "switch-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "switch-showcase" },
                        Children = new()
                        {
                            new UISwitch
                            {
                                DefaultChecked = true,
                                RenderChildrenContent = false,
                                Disabled = true,
                            },
                            new UISwitch
                            {
                                DefaultChecked = false,
                                RenderChildrenContent = false,
                                Disabled = true,
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 自定义文字
        /// </summary>
        private class CustomTextSection : UIView
        {
            internal CustomTextSection()
            {
                ClassName = new List<string> { "switch-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义文字",
                        ClassName = new List<string> { "switch-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "通过 CheckedChildren 和 UnCheckedChildren 自定义开关内的文字内容。",
                        ClassName = new List<string> { "switch-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "switch-showcase" },
                        Children = new()
                        {
                            new UISwitch
                            {
                                DefaultChecked = true,
                                CheckedChildren = "开启",
                                UnCheckedChildren = "关闭",
                            },
                            new UISwitch
                            {
                                DefaultChecked = false,
                                CheckedChildren = "ON",
                                UnCheckedChildren = "OFF",
                            },
                            new UISwitch
                            {
                                DefaultChecked = true,
                                CheckedChildren = "开启测试",
                                UnCheckedChildren = "关闭测试",
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 图标内容
        /// </summary>
        private class IconSection : UIView
        {
            internal IconSection()
            {
                ClassName = new List<string> { "switch-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "图标内容",
                        ClassName = new List<string> { "switch-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "通过 CheckedChildren / UnCheckedChildren 传入 UIElement，或使用 CheckedElement / UnCheckedElement 工厂方法自定义图标。",
                        ClassName = new List<string> { "switch-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "switch-showcase" },
                        Children = new()
                        {
                            new UISwitch
                            {
                                DefaultChecked = true,
                                RenderChildrenContent = true,
                                CheckedChildren = new UIIcon
                                {
                                    Content = "&#xe605;",
                                    Style = new DefaultUIStyle
                                    {
                                        Color = SKColors.Orange,
                                        FontFamily = UIFontManager.Get("TCYMIconFont"),
                                    }
                                },
                                UnCheckedChildren = new UIIcon
                                {
                                    Content = "&#xe62a;",
                                    Style = new DefaultUIStyle
                                    {
                                        Color = SKColors.Black,
                                        FontFamily = UIFontManager.Get("TCYMIconFont"),
                                    }
                                },
                            },
                            new UISwitch
                            {
                                DefaultChecked = true,
                                RenderChildrenContent = false,
                                CheckedElement = () => new UIIcon
                                {
                                    Content = "&#xe605;",
                                    Style = new DefaultUIStyle
                                    {
                                        Color = SKColors.Orange,
                                        FontFamily = UIFontManager.Get("TCYMIconFont"),
                                    }
                                },
                                UnCheckedElement = () => new UIIcon
                                {
                                    Content = "&#xe62a;",
                                    Style = new DefaultUIStyle
                                    {
                                        Color = SKColors.Black,
                                        FontFamily = UIFontManager.Get("TCYMIconFont"),
                                    }
                                },
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 自定义样式
        /// </summary>
        private class StyleSection : UIView
        {
            internal StyleSection()
            {
                ClassName = new List<string> { "switch-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义样式",
                        ClassName = new List<string> { "switch-card-title", "label-purple" }
                    },
                    new UILabel
                    {
                        Text = "通过 CheckedStyle / UnCheckedStyle 自定义选中和未选中状态的轨道颜色等样式。",
                        ClassName = new List<string> { "switch-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "switch-showcase" },
                        Children = new()
                        {
                            new UISwitch
                            {
                                DefaultChecked = true,
                                RenderChildrenContent = false,
                                TrackColorChecked = ColorHelper.ParseColor("#52c41a"),
                            },
                            new UISwitch
                            {
                                DefaultChecked = true,
                                RenderChildrenContent = false,
                                TrackColorChecked = ColorHelper.ParseColor("#eb2f96"),
                            },
                            new UISwitch
                            {
                                DefaultChecked = true,
                                RenderChildrenContent = false,
                                UnCheckedStyle = new DefaultUIStyle
                                {
                                    BackgroundColor = SKColors.Black,
                                },
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
                ClassName = new List<string> { "switch-demo-card" };

                var statusLabel = new UILabel
                {
                    Text = "状态：未操作",
                    Style = new DefaultUIStyle
                    {
                        FontSize = 13,
                        Color = ColorHelper.ParseColor("rgba(0,0,0,0.65)"),
                    }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "事件回调",
                        ClassName = new List<string> { "switch-card-title", "label-cany" }
                    },
                    new UILabel
                    {
                        Text = "通过 OnChange 监听开关状态变化。",
                        ClassName = new List<string> { "switch-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "switch-showcase" },
                        Children = new()
                        {
                            new UISwitch
                            {
                                DefaultChecked = false,
                                CheckedChildren = "开",
                                UnCheckedChildren = "关",
                                OnChange = isChecked =>
                                {
                                    statusLabel.Text = $"状态：{(isChecked ? "已开启" : "已关闭")}";
                                },
                            },
                            statusLabel,
                        }
                    },
                };
            }
        }
    }
}
