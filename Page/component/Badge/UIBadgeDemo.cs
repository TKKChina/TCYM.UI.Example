using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Badge
{
    internal class UIBadgeDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Badge.style.css";

        internal UIBadgeDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "badge-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Badge 徽标数",
                    ClassName = new List<string> { "badge-demo-title" },
                },
                new UILabel
                {
                    Text = "图标右上角的圆形徽标数字 / 红点。",
                    ClassName = new List<string> { "badge-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "一般出现在通知图标或头像的右上角，用于显示需要处理的消息条数，通过醒目视觉形式吸引用户处理。",
                    ClassName = new List<string> { "badge-demo-desc" },
                },
                new BasicSection(),
                new OverflowAndDotSection(),
                new OffsetAndSizeSection(),
                new StatusSection(),
                new ColorSection(),
                new RibbonSection(),
            };
        }

        // ─── 工具方法 ───

        private static UIView CreateTargetBox(string text)
        {
            return new UIView
            {
                Style = new DefaultUIStyle
                {
                    Width = 48,
                    Height = 48,
                    BorderRadius = 10,
                    BackgroundColor = ColorHelper.ParseColor("#e6f4ff"),
                    BorderWidth = 1,
                    BorderColor = ColorHelper.ParseColor("#91caff"),
                    Display = "flex",
                    AlignItems = "center",
                    JustifyContent = "center",
                },
                Children = new()
                {
                    new UILabel
                    {
                        Text = text,
                        Style = new DefaultUIStyle
                        {
                            FontSize = 12,
                            Color = ColorHelper.ParseColor("#1677ff"),
                        }
                    }
                }
            };
        }

        private static UIView CreateBadgeCard(UIElement badge, string label)
        {
            return new UIView
            {
                ClassName = new List<string> { "badge-item-card" },
                Children = new()
                {
                    badge,
                    new UILabel
                    {
                        Text = label,
                        TextAlign = "center",
                        ClassName = new List<string> { "badge-item-label" },
                    }
                }
            };
        }

        private static UIView CreateRibbonTarget(string title, string description)
        {
            return new UIView
            {
                ClassName = new List<string> { "badge-ribbon-target" },
                Children = new()
                {
                    new UILabel
                    {
                        Text = title,
                        ClassName = new List<string> { "badge-ribbon-target-title" },
                    },
                    new UILabel
                    {
                        Text = description,
                        ClassName = new List<string> { "badge-ribbon-target-desc" },
                    }
                }
            };
        }

        private static UIView CreateRibbonCard(UIElement ribbon, string label)
        {
            return new UIView
            {
                ClassName = new List<string> { "badge-ribbon-item" },
                Children = new()
                {
                    ribbon,
                    new UILabel
                    {
                        Text = label,
                        TextAlign = "center",
                        ClassName = new List<string> { "badge-item-label" },
                    }
                }
            };
        }

        /// <summary>
        /// 基础与独立使用
        /// </summary>
        private class BasicSection : UIView
        {
            internal BasicSection()
            {
                ClassName = new List<string> { "badge-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "基础与独立使用",
                        ClassName = new List<string> { "badge-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "包裹子元素或独立使用。支持 Count 数字、CountText 自定义文本、ShowZero 显示零值。",
                        ClassName = new List<string> { "badge-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "badge-showcase" },
                        Children = new()
                        {
                            CreateBadgeCard(new UIBadge
                            {
                                Count = 5,
                                ShowOutline = false,
                                Children = new() { CreateTargetBox("通知") }
                            }, "包裹内容"),

                            CreateBadgeCard(new UIBadge
                            {
                                Count = 0,
                                OutlineColor = SKColors.Green,
                                ShowZero = true,
                                Children = new() { CreateTargetBox("待办") }
                            }, "0 也显示"),

                            CreateBadgeCard(new UIBadge
                            {
                                Count = 12
                            }, "独立徽标"),

                            CreateBadgeCard(new UIBadge
                            {
                                CountText = "NEW",
                                ShowOutline = false,
                                BadgeColor = ColorHelper.ParseColor("#1677ff")
                            }, "自定义文本"),
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 封顶数字与红点
        /// </summary>
        private class OverflowAndDotSection : UIView
        {
            internal OverflowAndDotSection()
            {
                ClassName = new List<string> { "badge-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "封顶数字与红点",
                        ClassName = new List<string> { "badge-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "通过 OverflowCount 设置封顶值（默认 99），超出显示为 99+。Dot 模式只显示一个小红点。",
                        ClassName = new List<string> { "badge-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "badge-showcase" },
                        Children = new()
                        {
                            CreateBadgeCard(new UIBadge
                            {
                                Count = 120,
                                OverflowCount = 99,
                                Children = new() { CreateTargetBox("消息") }
                            }, "99+ 封顶"),

                            CreateBadgeCard(new UIBadge
                            {
                                Dot = true,
                                Children = new() { CreateTargetBox("头像") }
                            }, "小红点"),

                            CreateBadgeCard(new UIBadge
                            {
                                Dot = true,
                                BadgeColor = ColorHelper.ParseColor("#52c41a"),
                                Children = new() { CreateTargetBox("在线") }
                            }, "自定义红点"),

                            CreateBadgeCard(new UIBadge
                            {
                                Count = 1000,
                                OverflowCount = 999
                            }, "999+ 独立"),
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 偏移与大小
        /// </summary>
        private class OffsetAndSizeSection : UIView
        {
            internal OffsetAndSizeSection()
            {
                ClassName = new List<string> { "badge-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "偏移与大小",
                        ClassName = new List<string> { "badge-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "通过 OffsetX / OffsetY 调整徽标位置，Size 支持 Default 和 Small 两种尺寸。",
                        ClassName = new List<string> { "badge-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "badge-showcase" },
                        Children = new()
                        {
                            CreateBadgeCard(new UIBadge
                            {
                                Count = 8,
                                OffsetX = -8,
                                OffsetY = 6,
                                Children = new() { CreateTargetBox("偏移") }
                            }, "偏移位置"),

                            CreateBadgeCard(new UIBadge
                            {
                                Count = 5,
                                Size = BadgeSize.Small,
                                Children = new() { CreateTargetBox("小号") }
                            }, "Small"),

                            CreateBadgeCard(new UIBadge
                            {
                                Count = 5,
                                Size = BadgeSize.Default,
                                Children = new() { CreateTargetBox("默认") }
                            }, "Default"),

                            CreateBadgeCard(new UIBadge
                            {
                                CountText = "HOT",
                                Size = BadgeSize.Small,
                                BadgeColor = ColorHelper.ParseColor("#722ed1")
                            }, "小号文本"),
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 状态点
        /// </summary>
        private class StatusSection : UIView
        {
            internal StatusSection()
            {
                ClassName = new List<string> { "badge-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "状态点",
                        ClassName = new List<string> { "badge-card-title", "label-purple" }
                    },
                    new UILabel
                    {
                        Text = "用于表示状态的小圆点，支持 Success / Processing / Default / Error / Warning 五种状态。",
                        ClassName = new List<string> { "badge-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "badge-showcase" },
                        Children = new()
                        {
                            CreateStatusBadge(BadgeStatus.Success, "Success"),
                            CreateStatusBadge(BadgeStatus.Processing, "Processing"),
                            CreateStatusBadge(BadgeStatus.Default, "Default"),
                            CreateStatusBadge(BadgeStatus.Error, "Error"),
                            CreateStatusBadge(BadgeStatus.Warning, "Warning"),
                        }
                    },
                };
            }

            private static UIView CreateStatusBadge(BadgeStatus status, string text)
            {
                return new UIView
                {
                    ClassName = new List<string> { "badge-status-row" },
                    Children = new()
                    {
                        new UIBadge
                        {
                            Status = status,
                            StatusText = text,
                        }
                    }
                };
            }
        }

        /// <summary>
        /// 多彩徽标
        /// </summary>
        private class ColorSection : UIView
        {
            internal ColorSection()
            {
                ClassName = new List<string> { "badge-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "多彩徽标",
                        ClassName = new List<string> { "badge-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "通过 BadgeColor 自定义徽标颜色，适用于不同业务场景的视觉区分。",
                        ClassName = new List<string> { "badge-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "badge-showcase" },
                        Children = new()
                        {
                            CreateBadgeCard(new UIBadge
                            {
                                Count = 6,
                                BadgeColor = ColorHelper.ParseColor("#eb2f96"),
                                Children = new() { CreateTargetBox("粉") }
                            }, "Pink"),

                            CreateBadgeCard(new UIBadge
                            {
                                Count = 8,
                                BadgeColor = ColorHelper.ParseColor("#13c2c2"),
                                Children = new() { CreateTargetBox("青") }
                            }, "Cyan"),

                            CreateBadgeCard(new UIBadge
                            {
                                CountText = "VIP",
                                BadgeColor = ColorHelper.ParseColor("#faad14")
                            }, "Gold"),

                            CreateBadgeCard(new UIBadge
                            {
                                Dot = true,
                                BadgeColor = ColorHelper.ParseColor("#722ed1"),
                                Children = new() { CreateTargetBox("紫") }
                            }, "Purple"),
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 缎带徽标
        /// </summary>
        private class RibbonSection : UIView
        {
            internal RibbonSection()
            {
                ClassName = new List<string> { "badge-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "缎带",
                        ClassName = new List<string> { "badge-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "使用 UIBadge.Ribbon 包裹内容。支持 Start / End 两侧位置、Color 快速换色，以及通过 RibbonStyle 自定义尺寸、渐变、圆角、边框和阴影。",
                        ClassName = new List<string> { "badge-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "badge-showcase", "badge-ribbon-showcase" },
                        Children = new()
                        {
                            CreateRibbonCard(new UIBadge.Ribbon
                            {
                                Text = "Hippies",
                                Children = new()
                                {
                                    CreateRibbonTarget("默认缎带", "默认位于内容右上角")
                                }
                            }, "Placement.End"),

                            CreateRibbonCard(new UIBadge.Ribbon
                            {
                                Text = "START",
                                Placement = BadgeRibbonPlacement.Start,
                                Color = ColorHelper.ParseColor("#eb2f96"),
                                Children = new()
                                {
                                    CreateRibbonTarget("起始侧", "颜色可以直接通过 Color 设置")
                                }
                            }, "Placement.Start + Color"),

                            CreateRibbonCard(new UIBadge.Ribbon
                            {
                                Text = "PRO",
                                OffsetY = 12,
                                FoldSize = 10,
                                FoldColor = ColorHelper.ParseColor("#30156f"),
                                RibbonStyle = new UpdateUIStyle
                                {
                                    Height = 30,
                                    PaddingLeft = 16,
                                    PaddingRight = 16,
                                    FontSize = 13,
                                    BackgroundGradient = "linear-gradient(135deg, #9254de, #531dab)",
                                    Color = SKColors.White,
                                    BorderRadius = 6,
                                    BorderWidth = 1,
                                    BorderColor = ColorHelper.ParseColor("rgba(255, 255, 255, 0.42)"),
                                    BoxShadowColor = ColorHelper.ParseColor("rgba(83, 29, 171, 0.35)"),
                                    BoxShadowBlur = 8,
                                    BoxShadowOffsetY = 3,
                                },
                                Children = new()
                                {
                                    CreateRibbonTarget("完整自定义", "RibbonStyle、折角和偏移均可调整")
                                }
                            }, "自定义 RibbonStyle"),
                        }
                    },
                };
            }
        }
    }
}
