using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Input;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Radio
{
    internal class UIRadioDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Radio.style.css";

        internal UIRadioDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "radio-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Radio 单选框",
                    ClassName = new List<string> { "radio-demo-title" },
                },
                new UILabel
                {
                    Text = "用于在多个备选项中选中单个状态。",
                    ClassName = new List<string> { "radio-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "单选框允许用户从一组互斥的选项中选择一个选项。当一个选项被选中时，同组的其他选项会自动取消选中。",
                    ClassName = new List<string> { "radio-demo-desc" },
                },
                new BasicSection(),
                new DisabledSection(),
                new GroupSection(),
                new CheckedStyleSection(),
                new CustomStyleSection(),
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
                ClassName = new List<string> { "radio-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "基本用法",
                        ClassName = new List<string> { "radio-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "简单的 Radio，支持设置默认选中状态。",
                        ClassName = new List<string> { "radio-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "radio-showcase" },
                        Children = new()
                        {
                            new UIRadio
                            {
                                Label = "选项 A",
                                GroupName = "basic-demo",
                                DefaultChecked = true,
                            },
                            new UIRadio
                            {
                                Label = "选项 B",
                                GroupName = "basic-demo",
                            },
                            new UIRadio
                            {
                                Label = "选项 C",
                                GroupName = "basic-demo",
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
                ClassName = new List<string> { "radio-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "禁用状态",
                        ClassName = new List<string> { "radio-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "通过 Disabled 属性禁用单选框，禁用后不可交互且样式置灰。",
                        ClassName = new List<string> { "radio-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "radio-showcase" },
                        Children = new()
                        {
                            new UIRadio
                            {
                                Label = "禁用未选中",
                                GroupName = "disabled-demo",
                                Disabled = true,
                            },
                            new UIRadio
                            {
                                Label = "禁用已选中",
                                GroupName = "disabled-demo-2",
                                DefaultChecked = true,
                                Disabled = true,
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 分组互斥
        /// </summary>
        private class GroupSection : UIView
        {
            internal GroupSection()
            {
                ClassName = new List<string> { "radio-demo-card" };

                var resultLabel = new UILabel
                {
                    Text = "当前选中：苹果",
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
                        Text = "分组互斥",
                        ClassName = new List<string> { "radio-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "通过 GroupName 属性将多个单选框归为一组，同组只能选中一个。",
                        ClassName = new List<string> { "radio-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "radio-showcase" },
                        Children = new()
                        {
                            new UIRadio
                            {
                                Label = "苹果",
                                GroupName = "fruit-group-demo",
                                DefaultChecked = true,
                                OnCheckedChanged = isChecked => { if (isChecked) resultLabel.Text = "当前选中：苹果"; },
                            },
                            new UIRadio
                            {
                                Label = "香蕉",
                                GroupName = "fruit-group-demo",
                                OnCheckedChanged = isChecked => { if (isChecked) resultLabel.Text = "当前选中：香蕉"; },
                            },
                            new UIRadio
                            {
                                Label = "橘子",
                                GroupName = "fruit-group-demo",
                                OnCheckedChanged = isChecked => { if (isChecked) resultLabel.Text = "当前选中：橘子"; },
                            },
                            new UIRadio
                            {
                                Label = "葡萄",
                                GroupName = "fruit-group-demo",
                                OnCheckedChanged = isChecked => { if (isChecked) resultLabel.Text = "当前选中：葡萄"; },
                            },
                            resultLabel,
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 选中态样式
        /// </summary>
        private class CheckedStyleSection : UIView
        {
            internal CheckedStyleSection()
            {
                ClassName = new List<string> { "radio-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "选中态样式",
                        ClassName = new List<string> { "radio-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "通过 CheckedStyle 属性自定义选中时的背景、边框、阴影等视觉表现（按钮式单选框）。",
                        ClassName = new List<string> { "radio-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "radio-showcase" },
                        Children = new()
                        {
                            new UIRadio
                            {
                                Label = "北京",
                                GroupName = "checked-style-demo",
                                RenderCircle = false,
                                DefaultChecked = true,
                                Style = new DefaultUIStyle
                                {
                                    PaddingLeft = 12,
                                    PaddingRight = 12,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderRadius = 6,
                                    BorderWidth = 1,
                                    BorderColor = ColorHelper.ParseColor("rgba(0,0,0,0.15)"),
                                },
                                CheckedStyle = new DefaultUIStyle
                                {
                                    Color = ColorHelper.ParseColor("#1677ff"),
                                    BackgroundColor = ColorHelper.ParseColor("#e6f4ff"),
                                    BorderColor = ColorHelper.ParseColor("#1677ff"),
                                    BorderWidth = 1,
                                    BorderRadius = 6,
                                    PaddingLeft = 12,
                                    PaddingRight = 12,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                },
                            },
                            new UIRadio
                            {
                                Label = "上海",
                                GroupName = "checked-style-demo",
                                RenderCircle = false,
                                Style = new DefaultUIStyle
                                {
                                    PaddingLeft = 12,
                                    PaddingRight = 12,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderRadius = 6,
                                    BorderWidth = 1,
                                    BorderColor = ColorHelper.ParseColor("rgba(0,0,0,0.15)"),
                                },
                                CheckedStyle = new DefaultUIStyle
                                {
                                    Color = ColorHelper.ParseColor("#1677ff"),
                                    BackgroundColor = ColorHelper.ParseColor("#e6f4ff"),
                                    BorderColor = ColorHelper.ParseColor("#1677ff"),
                                    BorderWidth = 1,
                                    BorderRadius = 6,
                                    PaddingLeft = 12,
                                    PaddingRight = 12,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                },
                            },
                            new UIRadio
                            {
                                Label = "广州",
                                GroupName = "checked-style-demo",
                                RenderCircle = false,
                                Style = new DefaultUIStyle
                                {
                                    PaddingLeft = 12,
                                    PaddingRight = 12,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderRadius = 6,
                                    BorderWidth = 1,
                                    BorderColor = ColorHelper.ParseColor("rgba(0,0,0,0.15)"),
                                },
                                CheckedStyle = new DefaultUIStyle
                                {
                                    Color = ColorHelper.ParseColor("#1677ff"),
                                    BackgroundColor = ColorHelper.ParseColor("#e6f4ff"),
                                    BorderColor = ColorHelper.ParseColor("#1677ff"),
                                    BorderWidth = 1,
                                    BorderRadius = 6,
                                    PaddingLeft = 12,
                                    PaddingRight = 12,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                },
                            },
                            new UIRadio
                            {
                                Label = "深圳",
                                GroupName = "checked-style-demo",
                                RenderCircle = false,
                                Style = new DefaultUIStyle
                                {
                                    PaddingLeft = 12,
                                    PaddingRight = 12,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
                                    BorderRadius = 6,
                                    BorderWidth = 1,
                                    BorderColor = ColorHelper.ParseColor("rgba(0,0,0,0.15)"),
                                },
                                CheckedStyle = new DefaultUIStyle
                                {
                                    Color = ColorHelper.ParseColor("#1677ff"),
                                    BackgroundColor = ColorHelper.ParseColor("#e6f4ff"),
                                    BorderColor = ColorHelper.ParseColor("#1677ff"),
                                    BorderWidth = 1,
                                    BorderRadius = 6,
                                    PaddingLeft = 12,
                                    PaddingRight = 12,
                                    PaddingTop = 6,
                                    PaddingBottom = 6,
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
        private class CustomStyleSection : UIView
        {
            internal CustomStyleSection()
            {
                ClassName = new List<string> { "radio-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义样式",
                        ClassName = new List<string> { "radio-card-title", "label-purple" }
                    },
                    new UILabel
                    {
                        Text = "自定义圆圈尺寸、颜色、内点大小等视觉属性。",
                        ClassName = new List<string> { "radio-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "radio-showcase" },
                        Children = new()
                        {
                            new UIRadio
                            {
                                Label = "大号圆圈",
                                GroupName = "custom-style-demo",
                                Radius = 12f,
                                CircleCheckedBorderColor = ColorHelper.ParseColor("#52c41a"),
                                CheckedColor = ColorHelper.ParseColor("#52c41a"),
                                DefaultChecked = true,
                            },
                            new UIRadio
                            {
                                Label = "粗边框",
                                GroupName = "custom-style-demo",
                                CircleStroke = 3f,
                                CircleCheckedStroke = 3f,
                                CircleCheckedBorderColor = ColorHelper.ParseColor("#eb2f96"),
                                CheckedColor = ColorHelper.ParseColor("#eb2f96"),
                            },
                            new UIRadio
                            {
                                Label = "小内点",
                                GroupName = "custom-style-demo",
                                DotRadiusFactor = 0.35f,
                                CircleCheckedBorderColor = ColorHelper.ParseColor("#722ed1"),
                                CheckedColor = ColorHelper.ParseColor("#722ed1"),
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
                ClassName = new List<string> { "radio-demo-card" };

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
                        ClassName = new List<string> { "radio-card-title", "label-cany" }
                    },
                    new UILabel
                    {
                        Text = "通过 OnCheckedChanged 监听选中状态变化。",
                        ClassName = new List<string> { "radio-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "radio-showcase" },
                        Children = new()
                        {
                            new UIRadio
                            {
                                Label = "选项 X",
                                GroupName = "event-demo",
                                DefaultChecked = true,
                                OnCheckedChanged = isChecked =>
                                {
                                    if (isChecked) statusLabel.Text = "状态：选中了 X";
                                },
                            },
                            new UIRadio
                            {
                                Label = "选项 Y",
                                GroupName = "event-demo",
                                OnCheckedChanged = isChecked =>
                                {
                                    if (isChecked) statusLabel.Text = "状态：选中了 Y";
                                },
                            },
                            new UIRadio
                            {
                                Label = "选项 Z",
                                GroupName = "event-demo",
                                OnCheckedChanged = isChecked =>
                                {
                                    if (isChecked) statusLabel.Text = "状态：选中了 Z";
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
