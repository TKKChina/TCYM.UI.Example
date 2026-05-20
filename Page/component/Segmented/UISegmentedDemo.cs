using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Segmented;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Segmented
{
    internal class UISegmentedDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Segmented.style.css";

        internal UISegmentedDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "segmented-demo-view" };

            Children = new()
            {
                new UILabel
                {
                    Text = "Segmented 分段选择器",
                    ClassName = new List<string> { "segmented-demo-title" },
                },
                new UILabel
                {
                    Text = "在几个互斥选项之间快速切换，可同时支持文本和自定义 UIElement 内容。",
                    ClassName = new List<string> { "segmented-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "Segmented 适合在少量互斥状态之间快速切换。这里分别演示字符串选项、自定义 UIElement 选项，以及可自定义选中态颜色和样式的主题化写法。",
                    ClassName = new List<string> { "segmented-demo-desc" },
                },
                CreateBasicSection(),
                CreateCustomContentSection(),
                CreateThemedSection(),
            };
        }

        private static UIView CreateBasicSection()
        {
            var currentValue = CreateStatusLabel("当前值：进行中");

            var segmented = new UISegmented
            {
                Options = new List<object?> { "进行中", "待处理", "已完成" },
                DefaultValue = "进行中",
                ItemHeight = 34,
                SelectionAnimationDuration = 0.3f,
                SelectionAnimationTimingFunction = "linear",
                ItemStyle = new UpdateUIStyle
                {
                    Width = 80,
                    BorderRadius = 8,
                    BorderColor = SKColors.Transparent,
                },
                SelectedItemStyle = new UpdateUIStyle
                {
                    Color = ColorHelper.ParseColor("#1677ff"),
                },
                ThumbStyle = new UpdateUIStyle
                {
                    BackgroundColor = SKColors.White,
                    BorderColor = ColorHelper.ParseColor("#d5def0"),
                },

                OnChange = value => currentValue.Text = $"当前值：{value}",
                //Style = new UpdateUIStyle
                //{
                //    Width = 320,
                //}
            };

            return new UIView
            {
                ClassName = new List<string> { "segmented-demo-card" },
                Children = new()
                {
                    new UILabel
                    {
                        Text = "基础用法",
                        ClassName = new List<string> { "segmented-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "Options 可直接传字符串列表。默认会自动选中第一项或 DefaultValue 对应项。",
                        ClassName = new List<string> { "segmented-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "segmented-showcase" },
                        Children = new()
                        {
                            segmented,
                        }
                    },
                    currentValue,
                }
            };
        }

        private static UIView CreateCustomContentSection()
        {
            var currentValue = CreateStatusLabel("当前值：weekly / 周报");

            var segmented = new UISegmented
            {
                DefaultValue = "weekly",
                Options = new List<object?>
                {
                    new UISegmentedOption(CreateMetricOption("日报", "12 条", "#1677ff"), "daily")
                    {
                        Label = "日报",
                    },
                    new UISegmentedOption(CreateMetricOption("周报", "7 份", "#16a34a"), "weekly")
                    {
                        Label = "周报",
                    },
                    new UISegmentedOption(CreateMetricOption("月报", "3 项", "#f59e0b"), "monthly")
                    {
                        Label = "月报",
                        SelectedStyle = new UpdateUIStyle
                        {
                            BackgroundColor = ColorHelper.ParseColor("#fff7ed"),
                            BorderColor = ColorHelper.ParseColor("#fdba74"),
                        }
                    }
                },
                ItemHeight = 56,
                ItemHorizontalPadding = 10,
                ItemVerticalPadding = 8,
                ItemStyle = new UpdateUIStyle
                {
                    Width = 118,
                },
                ThumbStyle = new UpdateUIStyle
                {
                    BorderRadius = 12,
                },
                OnSelectionChanged = (_, option) => currentValue.Text = $"当前值：{option.Value} / {option.Label}",
            };

            return new UIView
            {
                ClassName = new List<string> { "segmented-demo-card" },
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义内容",
                        ClassName = new List<string> { "segmented-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "Options 也可以传 UIElement。这里每个选项都是一个紧凑的信息块，保留了和其他 Demo 一致的展示结构。",
                        ClassName = new List<string> { "segmented-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "segmented-showcase" },
                        Children = new()
                        {
                            segmented,
                        }
                    },
                    currentValue,
                }
            };
        }

        private static UIView CreateThemedSection()
        {
            var currentValue = CreateStatusLabel("当前值：activity");
            currentValue.ClassName = new List<string> { "segmented-value-label", "segmented-value-label-dark" };

            var segmented = new UISegmented
            {
                Block = true,
                DefaultValue = "activity",
                Options = new List<object?>
                {
                    new UISegmentedOption("Activity", "activity"),
                    new UISegmentedOption("Design", "design"),
                    new UISegmentedOption("Deploy", "deploy"),
                },
                Style = new UpdateUIStyle
                {
                    BackgroundColor = ColorHelper.ParseColor("#101828"),
                    BorderColor = ColorHelper.ParseColor("#101828"),
                },
                ItemStyle = new UpdateUIStyle
                {
                    Width = 120,
                    FontWeight = 600,
                    BorderRadius = 14,
                    Color = ColorHelper.ParseColor("#cbd5e1"),
                    Hover = new DefaultUIStyle
                    {
                        BackgroundColor = ColorHelper.ParseColor("#1f2937"),
                        BorderColor = SKColors.Transparent,
                        Color = ColorHelper.ParseColor("#cbd5e1"),
                    },
                },
                SelectedItemStyle = new UpdateUIStyle
                {
                    Color = SKColors.White,
                },
                ThumbStyle = new UpdateUIStyle
                {
                    BackgroundColor = ColorHelper.ParseColor("#2563eb"),
                    BorderColor = ColorHelper.ParseColor("#60a5fa"),
                    BorderRadius = 14,
                    BoxShadowColor = ColorHelper.ParseColor("rgba(37,99,235,0.45)"),
                    BoxShadowBlur = 14,
                    BoxShadowOffsetY = 4,
                    BoxShadowSpread = 0,
                },
                OnChange = value => currentValue.Text = $"当前值：{value}",
            };

            return new UIView
            {
                ClassName = new List<string> { "segmented-demo-card" },
                Children = new()
                {
                    new UILabel
                    {
                        Text = "选中态颜色与样式",
                        ClassName = new List<string> { "segmented-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "通过 ItemStyle、SelectedItemStyle 与 ThumbStyle 可以分别调整默认态、选中内容和滑道外观。",
                        ClassName = new List<string> { "segmented-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "segmented-showcase", "segmented-showcase-dark" },
                        Children = new()
                        {
                            segmented,
                        }
                    },
                    currentValue,
                }
            };
        }

        private static UILabel CreateStatusLabel(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "segmented-value-label" }
            };
        }

        private static UIView CreateMetricOption(string title, string stat, string dotColor)
        {
            return new UIView
            {
                ClassName = new List<string> { "segmented-metric-option" },
                Children = new()
                {
                    new UIView
                    {
                        ClassName = new List<string> { "segmented-metric-dot" },
                        Style = new DefaultUIStyle
                        {
                            BackgroundColor = ColorHelper.ParseColor(dotColor),
                        }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "segmented-metric-copy" },
                        Children = new()
                        {
                            new UILabel
                            {
                                Text = title,
                                ClassName = new List<string> { "segmented-metric-title" },
                            },
                            new UILabel
                            {
                                Text = stat,
                                ClassName = new List<string> { "segmented-metric-stat" },
                            }
                        }
                    }
                }
            };
        }
    }
}
