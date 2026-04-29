using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Input;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Checkbox
{
    internal class UICheckboxDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Checkbox.style.css";

        internal UICheckboxDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "checkbox-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Checkbox 多选框",
                    ClassName = new List<string> { "checkbox-demo-title" },
                },
                new UILabel
                {
                    Text = "收集用户的多项选择。",
                    ClassName = new List<string> { "checkbox-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "在一组可选项中进行多项选择时使用。单独使用可以表示两种状态之间的切换，和 Switch 类似。",
                    ClassName = new List<string> { "checkbox-demo-desc" },
                },
                new BasicSection(),
                new DisabledSection(),
                new GroupSection(),
                new IndeterminateSection(),
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
                ClassName = new List<string> { "checkbox-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "基本用法",
                        ClassName = new List<string> { "checkbox-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "简单的 Checkbox，支持设置默认选中状态。",
                        ClassName = new List<string> { "checkbox-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "checkbox-showcase" },
                        Children = new()
                        {
                            new UICheckbox
                            {
                                Label = "未选中",
                            },
                            new UICheckbox
                            {
                                Label = "默认选中",
                                DefaultChecked = true,
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
                ClassName = new List<string> { "checkbox-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "禁用状态",
                        ClassName = new List<string> { "checkbox-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "通过 Disabled 属性禁用复选框，禁用后不可交互且样式置灰。",
                        ClassName = new List<string> { "checkbox-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "checkbox-showcase" },
                        Children = new()
                        {
                            new UICheckbox
                            {
                                Label = "禁用未选中",
                                Disabled = true,
                            },
                            new UICheckbox
                            {
                                Label = "禁用已选中",
                                DefaultChecked = true,
                                Disabled = true,
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 分组
        /// </summary>
        private class GroupSection : UIView
        {
            internal GroupSection()
            {
                ClassName = new List<string> { "checkbox-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "Checkbox 分组",
                        ClassName = new List<string> { "checkbox-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "通过 GroupName 属性将多个复选框归为一组，方便管理。",
                        ClassName = new List<string> { "checkbox-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "checkbox-showcase" },
                        Children = new()
                        {
                            new UICheckbox
                            {
                                Label = "苹果",
                                GroupName = "fruits-demo",
                                DefaultChecked = true,
                            },
                            new UICheckbox
                            {
                                Label = "香蕉",
                                GroupName = "fruits-demo",
                            },
                            new UICheckbox
                            {
                                Label = "橘子",
                                GroupName = "fruits-demo",
                            },
                            new UICheckbox
                            {
                                Label = "葡萄",
                                GroupName = "fruits-demo",
                                DefaultChecked = true,
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 半选状态
        /// </summary>
        private class IndeterminateSection : UIView
        {
            internal IndeterminateSection()
            {
                ClassName = new List<string> { "checkbox-demo-card" };

                var child1 = new UICheckbox { Label = "选项 A", GroupName = "indeterminate-demo" };
                var child2 = new UICheckbox { Label = "选项 B", GroupName = "indeterminate-demo" };
                var child3 = new UICheckbox { Label = "选项 C", GroupName = "indeterminate-demo" };
                var children = new List<UICheckbox> { child1, child2, child3 };

                var parentCheckbox = new UICheckbox
                {
                    Label = "全选",
                    Style = new DefaultUIStyle { FontWeight = 700 },
                };

                void UpdateParentState()
                {
                    int checkedCount = 0;
                    foreach (var c in children) if (c.IsChecked) checkedCount++;
                    if (checkedCount == 0)
                        parentCheckbox.SetChecked(false, false);
                    else if (checkedCount == children.Count)
                        parentCheckbox.SetChecked(true, false);
                    else
                        parentCheckbox.SetChecked(false, true);
                }

                parentCheckbox.OnCheckedChanged = (isChecked, indeterminate) =>
                {
                    if (indeterminate) return;
                    foreach (var c in children)
                        c.SetChecked(isChecked);
                };

                foreach (var c in children)
                {
                    c.OnCheckedChanged = (_, _) => UpdateParentState();
                }

                Children = new()
                {
                    new UILabel
                    {
                        Text = "全选 / 半选",
                        ClassName = new List<string> { "checkbox-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "通过 Indeterminate 属性实现半选状态，常用于全选联动场景。",
                        ClassName = new List<string> { "checkbox-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "checkbox-showcase", "checkbox-showcase-col" },
                        Children = new()
                        {
                            parentCheckbox,
                            new UIView
                            {
                                Style = new DefaultUIStyle
                                {
                                    Width = "100%",
                                    Height = 1,
                                    BackgroundColor = ColorHelper.ParseColor("rgba(0,0,0,0.06)"),
                                    MarginTop = 4,
                                    MarginBottom = 4,
                                },
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "checkbox-showcase" },
                                Children = new() { child1, child2, child3 },
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
                ClassName = new List<string> { "checkbox-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义样式",
                        ClassName = new List<string> { "checkbox-card-title", "label-purple" }
                    },
                    new UILabel
                    {
                        Text = "自定义方框尺寸、颜色、圆角等视觉属性。",
                        ClassName = new List<string> { "checkbox-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "checkbox-showcase" },
                        Children = new()
                        {
                            new UICheckbox
                            {
                                Label = "大号复选框",
                                Size = 22f,
                                CornerRadius = 4f,
                                BorderColorChecked = ColorHelper.ParseColor("#52c41a"),
                                CheckColor = ColorHelper.ParseColor("#52c41a"),
                                DefaultChecked = true,
                            },
                            new UICheckbox
                            {
                                Label = "圆形样式",
                                Size = 18f,
                                CornerRadius = 9f,
                                BorderColorChecked = ColorHelper.ParseColor("#eb2f96"),
                                CheckColor = ColorHelper.ParseColor("#eb2f96"),
                            },
                            new UICheckbox
                            {
                                Label = "填充背景",
                                Size = 16f,
                                CornerRadius = 3f,
                                BorderColorChecked = ColorHelper.ParseColor("#722ed1"),
                                BoxColorChecked = ColorHelper.ParseColor("#722ed1"),
                                CheckColor = SKColors.White,
                                DefaultChecked = true,
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
                ClassName = new List<string> { "checkbox-demo-card" };

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
                        ClassName = new List<string> { "checkbox-card-title", "label-cany" }
                    },
                    new UILabel
                    {
                        Text = "通过 OnCheckedChanged 监听选中状态变化。",
                        ClassName = new List<string> { "checkbox-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "checkbox-showcase" },
                        Children = new()
                        {
                            new UICheckbox
                            {
                                Label = "点击我试试",
                                OnCheckedChanged = (isChecked, indeterminate) =>
                                {
                                    statusLabel.Text = $"状态：{(isChecked ? "已选中" : "未选中")}";
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
