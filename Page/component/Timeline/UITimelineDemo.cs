using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Timeline;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Timeline
{
    /// <summary>Timeline 时间轴组件示例页。</summary>
    internal sealed class UITimelineDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Timeline.style.css";

        internal UITimelineDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = "timeline-demo-view";
            Children =
            [
                new UILabel
                {
                    Text = "Timeline 时间轴",
                    ClassName = "timeline-demo-title",
                },
                new UILabel
                {
                    Text = "按时间顺序展示一系列事件，并使用轨道在视觉上串联进程。",
                    ClassName = "timeline-demo-title-sub",
                },
                new UILabel
                {
                    Text = "Items 支持文本或任意 UIElement；可组合颜色、Loading、自定义节点、标题双栏、反向排序、交替布局和横向布局。",
                    ClassName = "timeline-demo-desc",
                },
                new BasicSection(),
                new LoadingAndReverseSection(),
                new AlternateSection(),
                new HorizontalSection(),
                new ScaleSection(),
            ];
        }

        private sealed class BasicSection : UIView
        {
            internal BasicSection()
            {
                ClassName = "timeline-demo-card";
                Children =
                [
                    CreateSectionTitle("基础用法与颜色"),
                    CreateSectionDescription("默认使用 Outlined 节点；每一项都可以通过 Color 设置独立的状态颜色。Filled 则使用实心节点。"),
                    new UIView
                    {
                        ClassName = "timeline-demo-grid",
                        Children =
                        [
                            CreatePanel(
                                "Outlined",
                                new UITimeline
                                {
                                    ClassName = new() { UITimeline.RootClassName, "timeline-component" },
                                    Items =
                                    [
                                        new TimelineItem("创建服务站点 2026-08-17"),
                                        new TimelineItem("解决初始网络问题 2026-08-17")
                                        {
                                            Color = ColorHelper.ParseColor("#52c41a"),
                                        },
                                        new TimelineItem("技术测试")
                                        {
                                            Color = ColorHelper.ParseColor("#faad14"),
                                        },
                                        new TimelineItem("网络问题已处理")
                                        {
                                            Color = ColorHelper.ParseColor("#ff4d4f"),
                                        },
                                    ],
                                }),
                            CreatePanel(
                                "Filled / End",
                                new UITimeline
                                {
                                    ClassName = new() { UITimeline.RootClassName, "timeline-component" },
                                    Mode = TimelineMode.End,
                                    Variant = TimelineVariant.Filled,
                                    PrimaryColor = ColorHelper.ParseColor("#722ed1"),
                                    Items =
                                    [
                                        new TimelineItem("收集需求"),
                                        new TimelineItem("完成交互设计"),
                                        new TimelineItem("进入开发阶段"),
                                        new TimelineItem("交付验收"),
                                    ],
                                }),
                        ],
                    },
                ];
            }
        }

        private sealed class LoadingAndReverseSection : UIView
        {
            internal LoadingAndReverseSection()
            {
                ClassName = "timeline-demo-card";

                var timeline = new UITimeline
                {
                    ClassName = new() { UITimeline.RootClassName, "timeline-component", "timeline-pending" },
                    Items =
                    [
                        new TimelineItem("初始化部署环境"),
                        new TimelineItem("上传并校验应用包")
                        {
                            Color = ColorHelper.ParseColor("#52c41a"),
                        },
                        new TimelineItem("等待节点完成滚动发布")
                        {
                            Loading = true,
                        },
                    ],
                };
                var state = new UILabel
                {
                    Text = "当前顺序：正序",
                    ClassName = "timeline-state-label",
                };
                var reverseButton = new UIButton
                {
                    Text = "切换排序",
                    ClassName = "timeline-action-button",
                };
                reverseButton.OnClick += _ =>
                {
                    timeline.Reverse = !timeline.Reverse;
                    state.Text = timeline.Reverse ? "当前顺序：倒序" : "当前顺序：正序";
                };

                Children =
                [
                    CreateSectionTitle("等待状态与反向排序", "label-green"),
                    CreateSectionDescription("Loading 绘制持续旋转的等待节点；Reverse 只改变显示顺序，不修改 Items 原始列表。"),
                    new UIView
                    {
                        ClassName = "timeline-reverse-layout",
                        Children =
                        [
                            timeline,
                            new UIView
                            {
                                ClassName = "timeline-reverse-actions",
                                Children = [reverseButton, state],
                            },
                        ],
                    },
                ];
            }
        }

        private sealed class AlternateSection : UIView
        {
            internal AlternateSection()
            {
                ClassName = "timeline-demo-card";
                Children =
                [
                    CreateSectionTitle("标题、交替布局与自定义节点", "label-purple"),
                    CreateSectionDescription("Title 单独展示时间；Alternate 让内容在两侧交替。Icon 可以使用任意 UIElement 替换默认圆点。"),
                    new UITimeline
                    {
                        ClassName = new() { UITimeline.RootClassName, "timeline-component", "timeline-alternate" },
                        Mode = TimelineMode.Alternate,
                        ItemPaddingBottom = 26,
                        Items =
                        [
                            new TimelineItem(CreateEventCard(
                                "需求确认",
                                "产品、设计和研发共同确认本次发布范围。",
                                "#e6f4ff"))
                            {
                                Title = "08:30",
                                Icon = CreateCustomIcon("✓", "timeline-icon-blue"),
                            },
                            new TimelineItem(CreateEventCard(
                                "构建完成",
                                "流水线已生成 Windows、Linux 与 macOS 制品。",
                                "#f6ffed"))
                            {
                                Title = "10:05",
                                Icon = CreateCustomIcon("2", "timeline-icon-green"),
                                Placement = TimelineItemPlacement.End,
                            },
                            new TimelineItem(CreateEventCard(
                                "灰度验证",
                                "正在观察性能指标和异常日志。",
                                "#fff7e6"))
                            {
                                Title = "11:20",
                                Loading = true,
                                Color = ColorHelper.ParseColor("#fa8c16"),
                            },
                            new TimelineItem("等待正式发布窗口")
                            {
                                Title = "14:00",
                                Color = ColorHelper.ParseColor("#8c8c8c"),
                            },
                        ],
                    },
                ];
            }
        }

        private sealed class HorizontalSection : UIView
        {
            internal HorizontalSection()
            {
                ClassName = "timeline-demo-card";
                Children =
                [
                    CreateSectionTitle("横向时间轴", "label-orange"),
                    CreateSectionDescription("Orientation=Horizontal 时项目从左到右平均分布，适合展示流程阶段和短时间跨度的进度。"),
                    new UITimeline
                    {
                        ClassName = new() { UITimeline.RootClassName, "timeline-component", "timeline-horizontal-demo" },
                        Orientation = TimelineOrientation.Horizontal,
                        Variant = TimelineVariant.Filled,
                        Items =
                        [
                            new TimelineItem("初始化")
                            {
                                Title = "09:00",
                                Color = ColorHelper.ParseColor("#1677ff"),
                            },
                            new TimelineItem("构建")
                            {
                                Title = "09:08",
                                Color = ColorHelper.ParseColor("#52c41a"),
                            },
                            new TimelineItem("测试")
                            {
                                Title = "09:16",
                                Color = ColorHelper.ParseColor("#722ed1"),
                            },
                            new TimelineItem("发布")
                            {
                                Title = "09:30",
                                Icon = CreateCustomIcon("✓", "timeline-icon-orange"),
                            },
                        ],
                    },
                ];
            }
        }

        private sealed class ScaleSection : UIView
        {
            internal ScaleSection()
            {
                ClassName = "timeline-demo-card";

                float[] zoomLevels = [0.75f, 1f, 1.25f, 1.5f];
                int zoomIndex = 1;
                var timeline = new UITimeline
                {
                    ClassName = new()
                    {
                        UITimeline.RootClassName,
                        "timeline-component",
                        "timeline-zoom-preview",
                    },
                    Orientation = TimelineOrientation.Horizontal,
                    Variant = TimelineVariant.Filled,
                    Items =
                    [
                        new TimelineItem("采集")
                        {
                            Title = "Step 1",
                            Color = ColorHelper.ParseColor("#1677ff"),
                        },
                        new TimelineItem("处理")
                        {
                            Title = "Step 2",
                            Color = ColorHelper.ParseColor("#722ed1"),
                        },
                        new TimelineItem("输出")
                        {
                            Title = "Step 3",
                            Color = ColorHelper.ParseColor("#52c41a"),
                        },
                    ],
                };
                var state = new UILabel
                {
                    Text = "当前 Zoom：100%",
                    ClassName = "timeline-state-label timeline-zoom-state",
                };
                var zoomButton = new UIButton
                {
                    Text = "切换 Zoom",
                    ClassName = "timeline-action-button timeline-zoom-button",
                };
                zoomButton.OnClick += _ =>
                {
                    zoomIndex = (zoomIndex + 1) % zoomLevels.Length;
                    float zoom = zoomLevels[zoomIndex];
                    timeline.Style = new UpdateUIStyle
                    {
                        Width = "58%",
                        Zoom = zoom,
                    };
                    state.Text = $"当前 Zoom：{zoom * 100:0}%";
                };

                Children =
                [
                    CreateSectionTitle("高 DPI 与视觉缩放", "label-cany"),
                    CreateSectionDescription("组件会跟随窗口所在显示器的布局 DPI/DPR；Zoom 会统一缩放轨道、节点、标题和内容，同时保留原始布局尺寸。"),
                    new UIView
                    {
                        ClassName = "timeline-zoom-stage",
                        Children = [timeline],
                    },
                    new UIView
                    {
                        ClassName = "timeline-zoom-actions",
                        Children = [zoomButton, state],
                    },
                ];
            }
        }

        private static UIView CreatePanel(string title, UITimeline timeline)
        {
            return new UIView
            {
                ClassName = "timeline-demo-panel",
                Children =
                [
                    new UILabel
                    {
                        Text = title,
                        ClassName = "timeline-panel-title",
                    },
                    timeline,
                ],
            };
        }

        private static UIView CreateEventCard(string title, string description, string background)
        {
            return new UIView
            {
                ClassName = "timeline-event-card",
                Style = new UpdateUIStyle
                {
                    BackgroundColor = ColorHelper.ParseColor(background),
                },
                Children =
                [
                    new UILabel
                    {
                        Text = title,
                        ClassName = "timeline-event-title",
                    },
                    new UILabel
                    {
                        Text = description,
                        Wrap = true,
                        ClassName = "timeline-event-description",
                    },
                ],
            };
        }

        private static UIView CreateCustomIcon(string text, string accentClass)
        {
            return new UIView
            {
                ClassName = new() { "timeline-custom-icon", accentClass },
                Children =
                [
                    new UILabel
                    {
                        Text = text,
                        TextAlign = "center",
                        ClassName = "timeline-custom-icon-text",
                    },
                ],
            };
        }

        private static UILabel CreateSectionTitle(string text, string accentClass = "label-title")
        {
            return new UILabel
            {
                Text = text,
                ClassName = new() { "timeline-card-title", accentClass },
            };
        }

        private static UILabel CreateSectionDescription(string text)
        {
            return new UILabel
            {
                Text = text,
                Wrap = true,
                ClassName = "timeline-card-desc",
            };
        }
    }
}
