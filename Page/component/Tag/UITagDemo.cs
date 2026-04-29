using SkiaSharp;
using System.Linq;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Tag
{
    internal class UITagDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Tag.style.css";

        private static readonly TagClassColor[] AllClassColors = new[]
        {
            TagClassColor.Magenta, TagClassColor.Red, TagClassColor.Volcano,
            TagClassColor.Orange, TagClassColor.Gold, TagClassColor.Lime,
            TagClassColor.Green, TagClassColor.Cyan, TagClassColor.Blue,
            TagClassColor.GeekBlue, TagClassColor.Purple,
        };

        internal UITagDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "tag-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Tag 标签",
                    ClassName = new List<string> { "tag-demo-title" },
                },
                new UILabel
                {
                    Text = "进行标记和分类的小标签。",
                    ClassName = new List<string> { "tag-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "用于标记事物的属性和维度，或者进行分类。支持 Filled、Solid、Outlined 三种变体，以及 11 种预设颜色。",
                    ClassName = new List<string> { "tag-demo-desc" },
                },
                new FilledSection(),
                new SolidSection(),
                new OutlinedSection(),
                new CustomColorSection(),
                new IconSection(),
            };
        }

        /// <summary>
        /// Filled 填充变体
        /// </summary>
        private class FilledSection : UIView
        {
            internal FilledSection()
            {
                ClassName = new List<string> { "tag-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "Filled 填充样式",
                        ClassName = new List<string> { "tag-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "默认变体，浅色背景搭配深色文字，适用于大多数场景。支持 11 种 Ant Design 预设颜色。",
                        ClassName = new List<string> { "tag-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tag-showcase" },
                        Children = AllClassColors.Select(c => (UIElement)new UITag
                        {
                            Text = c.ToString(),
                            Variant = TagVariant.Filled,
                            ClassColor = c,
                        }).ToList()
                    },
                };
            }
        }

        /// <summary>
        /// Solid 实心变体
        /// </summary>
        private class SolidSection : UIView
        {
            internal SolidSection()
            {
                ClassName = new List<string> { "tag-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "Solid 实心样式",
                        ClassName = new List<string> { "tag-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "实心背景搭配白色文字，视觉更醒目，适合需要强调的标签。",
                        ClassName = new List<string> { "tag-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tag-showcase" },
                        Children = AllClassColors.Select(c => (UIElement)new UITag
                        {
                            Text = c.ToString(),
                            Variant = TagVariant.Solid,
                            ClassColor = c,
                        }).ToList()
                    },
                };
            }
        }

        /// <summary>
        /// Outlined 描边变体
        /// </summary>
        private class OutlinedSection : UIView
        {
            internal OutlinedSection()
            {
                ClassName = new List<string> { "tag-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "Outlined 描边样式",
                        ClassName = new List<string> { "tag-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "带边框的样式，背景色透明或浅色，适合在白色背景上使用。",
                        ClassName = new List<string> { "tag-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tag-showcase" },
                        Children = AllClassColors.Select(c => (UIElement)new UITag
                        {
                            Text = c.ToString(),
                            Variant = TagVariant.Outlined,
                            ClassColor = c,
                        }).ToList()
                    },
                };
            }
        }

        /// <summary>
        /// 自定义颜色
        /// </summary>
        private class CustomColorSection : UIView
        {
            internal CustomColorSection()
            {
                ClassName = new List<string> { "tag-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义颜色",
                        ClassName = new List<string> { "tag-card-title", "label-purple" }
                    },
                    new UILabel
                    {
                        Text = "不设置 ClassColor 时，可通过 Style.Color 自定义颜色，组件会自动计算背景色和边框色。",
                        ClassName = new List<string> { "tag-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tag-showcase" },
                        Children = new()
                        {
                            new UITag
                            {
                                Text = "#f50",
                                Variant = TagVariant.Filled,
                                Style = new DefaultUIStyle
                                {
                                    Color = ColorHelper.ParseColor("#f50"),
                                    PaddingLeft = 4, PaddingRight = 4,
                                    PaddingTop = 2, PaddingBottom = 2,
                                    BorderRadius = 5,
                                }
                            },
                            new UITag
                            {
                                Text = "#2db7f5",
                                Variant = TagVariant.Filled,
                                Style = new DefaultUIStyle
                                {
                                    Color = ColorHelper.ParseColor("#2db7f5"),
                                    PaddingLeft = 4, PaddingRight = 4,
                                    PaddingTop = 2, PaddingBottom = 2,
                                    BorderRadius = 5,
                                }
                            },
                            new UITag
                            {
                                Text = "#87d068",
                                Variant = TagVariant.Filled,
                                Style = new DefaultUIStyle
                                {
                                    Color = ColorHelper.ParseColor("#87d068"),
                                    PaddingLeft = 4, PaddingRight = 4,
                                    PaddingTop = 2, PaddingBottom = 2,
                                    BorderRadius = 5,
                                }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 带图标
        /// </summary>
        private class IconSection : UIView
        {
            internal IconSection()
            {
                ClassName = new List<string> { "tag-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "带图标标签",
                        ClassName = new List<string> { "tag-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "通过 Icon 属性添加图标，IconPosition 控制图标在文字左侧或右侧。支持与所有变体和颜色组合使用。",
                        ClassName = new List<string> { "tag-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tag-showcase" },
                        Children = new()
                        {
                            new UITag
                            {
                                Text = "左侧图标",
                                Variant = TagVariant.Filled,
                                ClassColor = TagClassColor.Blue,
                                IconPosition = TagIconPosition.Left,
                                Icon = new UIIcon
                                {
                                    Content = "\ue8e8",
                                    Style = new DefaultUIStyle
                                    {
                                        FontFamily = UIFontManager.Get("TCYMIconFont"),
                                        FontSize = 14,
                                        Width = 14,
                                        Height = 14,
                                    }
                                }
                            },
                            new UITag
                            {
                                Text = "右侧图标",
                                IconPosition = TagIconPosition.Right,
                                Icon = new UIIcon
                                {
                                    Content = "\ue614",
                                    Style = new DefaultUIStyle
                                    {
                                        FontFamily = UIFontManager.Get("TCYMIconFont"),
                                        FontSize = 14,
                                    }
                                }
                            },
                            new UITag
                            {
                                Text = "Lime 图标",
                                IconPosition = TagIconPosition.Right,
                                Variant = TagVariant.Filled,
                                ClassColor = TagClassColor.Lime,
                                Icon = new UIIcon
                                {
                                    Content = "\ue707",
                                    Style = new DefaultUIStyle
                                    {
                                        FontFamily = UIFontManager.Get("TCYMIconFont"),
                                        FontSize = 16,
                                    }
                                }
                            },
                            new UITag
                            {
                                Text = "Outlined 图标",
                                IconPosition = TagIconPosition.Right,
                                Variant = TagVariant.Outlined,
                                ClassColor = TagClassColor.Green,
                                Icon = new UIIcon
                                {
                                    Content = "\ue610",
                                    Style = new DefaultUIStyle
                                    {
                                        FontFamily = UIFontManager.Get("TCYMIconFont"),
                                        FontSize = 16,
                                    }
                                }
                            },
                        }
                    },
                };
            }
        }
    }
}
