using SkiaSharp;
using System.Linq;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Tooltip;
using TCYM.UI.Enums;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Tooltip
{
    internal class UITooltipDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Tooltip.style.css";

        internal UITooltipDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "tooltip-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Tooltip 文字提示",
                    ClassName = new List<string> { "tooltip-demo-title" },
                },
                new UILabel
                {
                    Text = "简单的文字提示气泡框。",
                    ClassName = new List<string> { "tooltip-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "鼠标移入则显示提示，移出消失，气泡浮层不承载复杂文本和操作。可用来代替系统默认的 title 提示，提供一个按钮/文字/操作的文案解释。",
                    ClassName = new List<string> { "tooltip-demo-desc" },
                },
                new BasicSection(),
                new PlacementSection(),
                new TriggerSection(),
                new CustomContentSection(),
                new ControlledSection(),
            };
        }

        // ─── 工具方法 ───
        private static UIView CreateTriggerButton(string text)
        {
            return new UIView
            {
                Style = new DefaultUIStyle
                {
                    Height = 34,
                    PaddingLeft = 14,
                    PaddingRight = 14,
                    BorderWidth = 1,
                    BorderRadius = 6,
                    BorderColor = ColorHelper.ParseColor("#d9d9d9"),
                    BackgroundColor = SKColors.White,
                    Display = "flex",
                    AlignItems = "center",
                    JustifyContent = "center",
                    Cursor = UICursor.Pointer,
                },
                Children = new()
                {
                    new UILabel
                    {
                        Text = text,
                        Style = new DefaultUIStyle
                        {
                            Color = ColorHelper.ParseColor("#333"),
                            FontSize = 13,
                            PointerEvents = "none",
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 基础用法
        /// </summary>
        private class BasicSection : UIView
        {
            internal BasicSection()
            {
                ClassName = new List<string> { "tooltip-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "基础用法",
                        ClassName = new List<string> { "tooltip-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "最简单的用法，鼠标移入显示提示。支持自定义颜色、偏移量和延迟隐藏。",
                        ClassName = new List<string> { "tooltip-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tooltip-showcase" },
                        Children = new()
                        {
                            new UITooltip
                            {
                                Title = "这里是基础文字提示。",
                                Children = new() { CreateTriggerButton("Hover 我") }
                            },
                            new UITooltip
                            {
                                Title = "支持自定义颜色与偏移。",
                                Color = ColorHelper.ParseColor("#1677ff"),
                                OffsetX = 8,
                                Placement = TooltipPlacement.Bottom,
                                Children = new() { CreateTriggerButton("蓝色提示") }
                            },
                            new UITooltip
                            {
                                Title = "离开时有延迟，避免闪烁。",
                                MouseLeaveDelay = 0.18f,
                                MaxPopupWidth = 220,
                                Children = new() { CreateTriggerButton("延迟隐藏") }
                            },
                            new UITooltip
                            {
                                Title = "不显示箭头的提示。",
                                Arrow = false,
                                Children = new() { CreateTriggerButton("无箭头") }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 12 个方向
        /// </summary>
        private class PlacementSection : UIView
        {
            internal PlacementSection()
            {
                var placements = new[]
                {
                    TooltipPlacement.TopLeft, TooltipPlacement.Top, TooltipPlacement.TopRight,
                    TooltipPlacement.LeftTop, TooltipPlacement.RightTop,
                    TooltipPlacement.Left, TooltipPlacement.Right,
                    TooltipPlacement.LeftBottom, TooltipPlacement.RightBottom,
                    TooltipPlacement.BottomLeft, TooltipPlacement.Bottom, TooltipPlacement.BottomRight,
                };

                ClassName = new List<string> { "tooltip-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "12 个方向",
                        ClassName = new List<string> { "tooltip-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "通过 Placement 设置气泡弹出方向，共 12 个方向。支持 AutoAdjustOverflow 自动调整避免溢出。",
                        ClassName = new List<string> { "tooltip-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tooltip-showcase" },
                        Children = placements.Select(p => (UIElement)new UITooltip
                        {
                            Title = p.ToString(),
                            Placement = p,
                            Children = new()
                            {
                                new UIView
                                {
                                    Style = new DefaultUIStyle
                                    {
                                        Width = 88,
                                        Height = 34,
                                        BorderWidth = 1,
                                        BorderRadius = 6,
                                        BorderColor = ColorHelper.ParseColor("#d9d9d9"),
                                        Display = "flex",
                                        AlignItems = "center",
                                        JustifyContent = "center",
                                        Cursor = UICursor.Pointer,
                                        BackgroundColor = SKColors.White,
                                    },
                                    Children = new()
                                    {
                                        new UILabel
                                        {
                                            Text = p.ToString(),
                                            Style = new DefaultUIStyle
                                            {
                                                FontSize = 11,
                                                Color = ColorHelper.ParseColor("#555"),
                                                PointerEvents = "none",
                                            }
                                        }
                                    }
                                }
                            }
                        }).ToList()
                    },
                };
            }
        }

        /// <summary>
        /// 触发方式
        /// </summary>
        private class TriggerSection : UIView
        {
            internal TriggerSection()
            {
                ClassName = new List<string> { "tooltip-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "触发方式",
                        ClassName = new List<string> { "tooltip-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "支持 Hover 和 Click 两种触发方式，也可同时设置两者。Click 模式下点击外部自动关闭。",
                        ClassName = new List<string> { "tooltip-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tooltip-showcase" },
                        Children = new()
                        {
                            new UITooltip
                            {
                                Title = "Hover 触发（默认）",
                                Trigger = new() { TooltipTrigger.Hover },
                                Children = new() { CreateTriggerButton("Hover 触发") }
                            },
                            new UITooltip
                            {
                                Title = "Click 触发，点击外部关闭。",
                                Trigger = new() { TooltipTrigger.Click },
                                Placement = TooltipPlacement.Bottom,
                                Children = new() { CreateTriggerButton("Click 触发") }
                            },
                            new UITooltip
                            {
                                Title = "Hover 可显示，Click 可固定打开。",
                                Trigger = new() { TooltipTrigger.Hover, TooltipTrigger.Click },
                                Placement = TooltipPlacement.Right,
                                Children = new() { CreateTriggerButton("Hover + Click") }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 自定义内容
        /// </summary>
        private class CustomContentSection : UIView
        {
            internal CustomContentSection()
            {
                ClassName = new List<string> { "tooltip-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义内容",
                        ClassName = new List<string> { "tooltip-card-title", "label-purple" }
                    },
                    new UILabel
                    {
                        Text = "通过 TooltipContent 设置任意 UIElement 作为提示内容，支持复杂布局。也可通过 MaxPopupWidth 限制最大宽度。",
                        ClassName = new List<string> { "tooltip-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tooltip-showcase" },
                        Children = new()
                        {
                            new UITooltip
                            {
                                Placement = TooltipPlacement.Right,
                                Trigger = new() { TooltipTrigger.Click },
                                MaxPopupWidth = 260,
                                Title = "操作提示",
                                TooltipContent = new UIView
                                {
                                    Style = new DefaultUIStyle
                                    {
                                        Width = 200,
                                        Display = "flex",
                                        FlexDirection = "column",
                                        Gap = 8,
                                    },
                                    Children = new()
                                    {
                                        new UILabel
                                        {
                                            Text = "点击外部区域会自动关闭。",
                                            Wrap = true,
                                            Style = new DefaultUIStyle
                                            {
                                                Width = "100%",
                                                Color = SKColors.White,
                                                FontSize = 12,
                                            }
                                        },
                                        new UIView
                                        {
                                            Style = new DefaultUIStyle
                                            {
                                                Width = "100%",
                                                Height = 1,
                                                BackgroundColor = ColorHelper.ParseColor("rgba(255,255,255,0.18)"),
                                            }
                                        },
                                        new UILabel
                                        {
                                            Text = "Tooltip 支持承载任意 UIElement 内容，类似 Popover 的用法。",
                                            Wrap = true,
                                            Style = new DefaultUIStyle
                                            {
                                                Width = "100%",
                                                Color = ColorHelper.ParseColor("rgba(255,255,255,0.78)"),
                                                FontSize = 12,
                                            }
                                        }
                                    }
                                },
                                Children = new() { CreateTriggerButton("点击打开") }
                            },
                            new UITooltip
                            {
                                Placement = TooltipPlacement.Bottom,
                                MaxPopupWidth = 200,
                                TooltipContent = new UIView
                                {
                                    Style = new DefaultUIStyle
                                    {
                                        Width = 180,
                                        Display = "flex",
                                        FlexDirection = "column",
                                        Gap = 6,
                                    },
                                    Children = new()
                                    {
                                        new UILabel
                                        {
                                            Text = "多行自定义内容展示",
                                            Wrap = true,
                                            Style = new DefaultUIStyle
                                            {
                                                Width = "100%",
                                                Color = SKColors.White,
                                                FontSize = 13,
                                            }
                                        },
                                        new UILabel
                                        {
                                            Text = "Hover 显示，支持多种布局。",
                                            Wrap = true,
                                            Style = new DefaultUIStyle
                                            {
                                                Width = "100%",
                                                Color = ColorHelper.ParseColor("rgba(255,255,255,0.75)"),
                                                FontSize = 12,
                                            }
                                        }
                                    }
                                },
                                Children = new() { CreateTriggerButton("Hover 自定义") }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 受控模式
        /// </summary>
        private class ControlledSection : UIView
        {
            internal ControlledSection()
            {
                ClassName = new List<string> { "tooltip-demo-card" };

                var stateLabel = new UILabel
                {
                    Text = "当前状态：关闭",
                    ClassName = new List<string> { "tooltip-event-label" }
                };

                bool isOpen = false;
                UITooltip? tooltip = null;
                tooltip = new UITooltip
                {
                    Open = isOpen,
                    Trigger = new() { TooltipTrigger.Click },
                    Title = "这是受控 Tooltip，显示状态由外部按钮驱动。",
                    Placement = TooltipPlacement.Top,
                    OnOpenChange = next =>
                    {
                        isOpen = next;
                        if (tooltip != null) tooltip.Open = isOpen;
                        stateLabel.Text = isOpen ? "当前状态：打开" : "当前状态：关闭";
                    },
                    Children = new() { CreateTriggerButton("受控 Trigger") }
                };

                var toggleButton = new UIButton
                {
                    Text = "切换状态",
                    Style = new DefaultUIStyle
                    {
                        Width = 96,
                        Height = 34,
                        BackgroundColor = ColorHelper.ParseColor("#1677ff"),
                        Color = SKColors.White,
                        BorderRadius = 6,
                        Cursor = UICursor.Pointer,
                    }
                };
                toggleButton.OnClick += _ =>
                {
                    isOpen = !isOpen;
                    tooltip!.Open = isOpen;
                    stateLabel.Text = isOpen ? "当前状态：打开" : "当前状态：关闭";
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "受控模式",
                        ClassName = new List<string> { "tooltip-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "通过 Open 属性控制 Tooltip 的显隐状态。配合 OnOpenChange 回调同步状态，实现完全受控。",
                        ClassName = new List<string> { "tooltip-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tooltip-showcase" },
                        Children = new()
                        {
                            tooltip,
                            toggleButton,
                        }
                    },
                    stateLabel,
                };
            }
        }
    }
}
