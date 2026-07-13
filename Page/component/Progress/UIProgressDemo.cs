using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Progress
{
    /// <summary>
    /// Progress 进度组件示例页面。
    /// 页面按照“基础条形进度、成功子进度与文字位置、圆形与仪表盘、
    /// 分段与渐变、动态更新”的顺序组织，便于逐项查看 UIProgress 的核心能力。
    /// </summary>
    internal class UIProgressDemo : UIScrollView
    {
        /// <summary>
        /// 当前 Demo 使用的嵌入式 CSS 资源路径。
        /// res:// 路径会由 UISystem 从 TCYM.UI.Example 程序集中读取。
        /// </summary>
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Progress.style.css";

        /// <summary>
        /// 初始化 Progress Demo 页面并依次添加页面说明与各功能演示卡片。
        /// UIScrollView 允许示例数量超出窗口高度时继续纵向滚动查看。
        /// </summary>
        internal UIProgressDemo()
        {
            // 先加载本页样式，再设置根节点 ClassName，确保首次布局即可命中 CSS 规则。
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = "progress-demo-view";
            Children = new()
            {
                // 页面标题。
                new UILabel
                {
                    Text = "Progress 进度条",
                    ClassName = "progress-demo-title",
                },
                // 一句话概括组件用途。
                new UILabel
                {
                    Text = "展示操作的当前进度和状态。",
                    ClassName = "progress-demo-title-sub",
                },
                // 罗列本页覆盖的主要功能，帮助使用者快速确认示例范围。
                new UILabel
                {
                    Text = "支持条形、圆形、仪表盘、分段、状态、成功子进度、渐变色、自定义文字与文字位置。",
                    ClassName = "progress-demo-desc",
                },
                // 各分区独立封装，避免所有示例堆叠在页面构造函数中。
                new BasicLineSection(),
                new PercentPositionSection(),
                new CircleSection(),
                new StepsAndGradientSection(),
                new DynamicSection(),
            };
        }

        /// <summary>
        /// 创建演示卡片标题。
        /// </summary>
        /// <param name="text">标题文字。</param>
        /// <param name="accentClass">标题强调色 CSS 类，默认使用通用标题色。</param>
        /// <returns>已经配置公共标题样式的 UILabel。</returns>
        private static UILabel CreateSectionTitle(string text, string accentClass = "label-title")
        {
            return new UILabel
            {
                Text = text,
                ClassName = new UIClassNameCollection { "progress-card-title", accentClass },
            };
        }

        /// <summary>
        /// 创建演示卡片的功能说明文字。
        /// </summary>
        /// <param name="text">需要展示的说明。</param>
        /// <returns>使用统一说明样式的 UILabel。</returns>
        private static UILabel CreateSectionDescription(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = "progress-card-desc",
            };
        }

        /// <summary>
        /// 创建一项条形进度示例，由属性说明标签和 UIProgress 组成。
        /// </summary>
        /// <param name="label">当前进度条演示的属性组合说明。</param>
        /// <param name="progress">需要放入页面的 UIProgress 实例。</param>
        /// <returns>占满卡片宽度的条形进度示例容器。</returns>
        private static UIView CreateLineItem(string label, UIProgress progress)
        {
            // 条形进度默认跟随父容器宽度，便于观察百分比和外部文字布局。
            progress.Style = new UpdateUIStyle
            {
                Width = "100%",
            };

            return new UIView
            {
                ClassName = "progress-line-item",
                Children = new()
                {
                    new UILabel
                    {
                        Text = label,
                        ClassName = "progress-line-label",
                    },
                    progress,
                },
            };
        }

        /// <summary>
        /// 创建一项圆形或仪表盘示例，并在图形下方显示说明标签。
        /// </summary>
        /// <param name="label">当前圆形进度的说明文字。</param>
        /// <param name="progress">Circle 或 Dashboard 类型的 UIProgress。</param>
        /// <returns>带统一边框、间距和标签的圆形示例卡片。</returns>
        private static UIView CreateCircleItem(string label, UIProgress progress)
        {
            return new UIView
            {
                ClassName = "progress-circle-item",
                Children = new()
                {
                    progress,
                    new UILabel
                    {
                        Text = label,
                        TextAlign = "center",
                        ClassName = "progress-circle-label",
                    },
                },
            };
        }

        /// <summary>
        /// 基础条形进度演示。
        /// 覆盖默认状态、Active 动画、Exception、自动 Success、隐藏信息和 Small 尺寸。
        /// </summary>
        private sealed class BasicLineSection : UIView
        {
            /// <summary>
            /// 构建基础条形进度演示卡片。
            /// </summary>
            internal BasicLineSection()
            {
                ClassName = "progress-demo-card";
                Children = new()
                {
                    CreateSectionTitle("进度条与状态"),
                    CreateSectionDescription("Percent 设置当前百分比；Status 支持 Normal、Active、Success 和 Exception，ShowInfo 可以隐藏进度信息。"),
                    new UIView
                    {
                        ClassName = "progress-line-stack",
                        Children = new()
                        {
                            // Status 默认为 Auto；30% 时按 Normal 状态绘制。
                            CreateLineItem("基础进度", new UIProgress { Percent = 30 }),
                            // Active 只对 Line 类型显示循环高光动画。
                            CreateLineItem("进行中动画", new UIProgress
                            {
                                Percent = 50,
                                Status = ProgressStatus.Active,
                            }),
                            // Exception 会把已完成轨道和状态图标切换为异常色。
                            CreateLineItem("异常状态", new UIProgress
                            {
                                Percent = 70,
                                Status = ProgressStatus.Exception,
                            }),
                            // Auto 状态在 Percent 达到 100 时自动解析为 Success。
                            CreateLineItem("完成状态", new UIProgress
                            {
                                Percent = 100,
                            }),
                            // Small 使用较细轨道；ShowInfo=false 时不预留百分比文字空间。
                            CreateLineItem("隐藏信息 / Small", new UIProgress
                            {
                                Percent = 66,
                                ShowInfo = false,
                                Size = ProgressSize.Small,
                            }),
                        },
                    },
                };
            }
        }

        /// <summary>
        /// 成功子进度和百分比文字位置演示。
        /// 展示 Success、PercentPosition、Format 以及自定义轨道高度的组合用法。
        /// </summary>
        private sealed class PercentPositionSection : UIView
        {
            /// <summary>
            /// 构建成功子进度与文字位置演示卡片。
            /// </summary>
            internal PercentPositionSection()
            {
                ClassName = "progress-demo-card";
                Children = new()
                {
                    CreateSectionTitle("成功子进度与文字位置", "label-green"),
                    CreateSectionDescription("Success 单独标识已成功完成的区段；PercentPosition 可将信息放在轨道外部、内部或下方。"),
                    new UIView
                    {
                        ClassName = "progress-line-stack",
                        Children = new()
                        {
                            // 总进度为 70%，其中前 30% 使用 Success 的独立颜色覆盖显示。
                            CreateLineItem("Success.Percent = 30", new UIProgress
                            {
                                Percent = 70,
                                Success = new ProgressSuccess
                                {
                                    Percent = 30,
                                    StrokeColor = ColorHelper.ParseColor("#52c41a"),
                                },
                            }),
                            // Inner + End 把百分比放进轨道内部末尾；22 表示条形轨道高度。
                            CreateLineItem("内部末尾", new UIProgress
                            {
                                Percent = 75,
                                PercentPosition = new ProgressPercentPosition(
                                    ProgressPercentAlign.End,
                                    ProgressPercentPositionType.Inner),
                                Size = new ProgressSize(22),
                            }),
                            // Format 接管默认“百分比%”文字，可返回任意业务说明字符串。
                            CreateLineItem("内部居中 + 自定义格式", new UIProgress
                            {
                                Percent = 58,
                                StrokeColor = ColorHelper.ParseColor("#722ed1"),
                                PercentPosition = new ProgressPercentPosition(
                                    ProgressPercentAlign.Center,
                                    ProgressPercentPositionType.Inner),
                                Format = (percent, _) => $"已完成 {percent:0}%",
                                Size = new ProgressSize(24),
                            }),
                            // Bottom 会将信息放到轨道下方，Center 负责水平居中。
                            CreateLineItem("下方居中", new UIProgress
                            {
                                Percent = 45,
                                PercentPosition = new ProgressPercentPosition(
                                    ProgressPercentAlign.Center,
                                    ProgressPercentPositionType.Bottom),
                            }),
                        },
                    },
                };
            }
        }

        /// <summary>
        /// 圆形进度与仪表盘演示。
        /// 展示 Circle、Dashboard、尺寸预设、自定义文字、缺口方向和端点样式。
        /// </summary>
        private sealed class CircleSection : UIView
        {
            /// <summary>
            /// 构建圆形进度与仪表盘演示卡片。
            /// </summary>
            internal CircleSection()
            {
                ClassName = "progress-demo-card";
                Children = new()
                {
                    CreateSectionTitle("进度圈与仪表盘", "label-purple"),
                    CreateSectionDescription("Type 切换 Circle / Dashboard；Dashboard 支持 GapDegree 与 GapPlacement，StrokeLinecap 控制端点形状。"),
                    new UIView
                    {
                        ClassName = "progress-circle-grid",
                        Children = new()
                        {
                            // 自定义单值 Size 对 Circle 表示直径。
                            CreateCircleItem("Circle", new UIProgress
                            {
                                Type = ProgressType.Circle,
                                Percent = 75,
                                Size = new ProgressSize(120),
                            }),
                            // 使用 Small 预设快速创建紧凑型进度圈。
                            CreateCircleItem("Small", new UIProgress
                            {
                                Type = ProgressType.Circle,
                                Percent = 30,
                                Size = ProgressSize.Small,
                            }),
                            // Format 中的换行符会在圆心区域按多行文本居中绘制。
                            CreateCircleItem("自定义文字", new UIProgress
                            {
                                Type = ProgressType.Circle,
                                Percent = 75,
                                Format = (percent, _) => $"{percent:0}\nDays",
                                StrokeColor = ColorHelper.ParseColor("#13c2c2"),
                                Size = new ProgressSize(120),
                            }),
                            // Circle 达到 100% 后，Auto 状态会显示成功色和对勾图标。
                            CreateCircleItem("Success", new UIProgress
                            {
                                Type = ProgressType.Circle,
                                Percent = 100,
                                Size = new ProgressSize(120),
                            }),
                            // Dashboard 默认在底部保留缺口，GapDegree 控制缺口角度。
                            CreateCircleItem("Dashboard", new UIProgress
                            {
                                Type = ProgressType.Dashboard,
                                Percent = 68,
                                GapDegree = 75,
                                GapPlacement = ProgressGapPlacement.Bottom,
                                Size = new ProgressSize(120),
                            }),
                            // 将缺口移到顶部，并用 Butt 生成平直的弧线端点。
                            CreateCircleItem("Butt / Top", new UIProgress
                            {
                                Type = ProgressType.Dashboard,
                                Percent = 68,
                                GapDegree = 95,
                                GapPlacement = ProgressGapPlacement.Top,
                                StrokeLinecap = ProgressStrokeLinecap.Butt,
                                StrokeColor = ColorHelper.ParseColor("#fa8c16"),
                                Size = new ProgressSize(120),
                            }),
                        },
                    },
                };
            }
        }

        /// <summary>
        /// 分段进度与渐变色演示。
        /// 同时覆盖 Line、Circle 和 Dashboard 三种类型下的 Steps 行为。
        /// </summary>
        private sealed class StepsAndGradientSection : UIView
        {
            /// <summary>
            /// 构建分段与渐变演示卡片。
            /// </summary>
            internal StepsAndGradientSection()
            {
                // 同一个渐变配置可复用于条形、圆形及分段进度。
                var gradient = new ProgressGradient(
                    ColorHelper.ParseColor("#108ee9"),
                    ColorHelper.ParseColor("#87d068"));

                ClassName = "progress-demo-card";
                Children = new()
                {
                    CreateSectionTitle("分段与渐变", "label-orange"),
                    CreateSectionDescription("Steps 启用分段进度；StepColors 可逐段配色，StrokeGradient 提供线性渐变，Rounding 控制完成段数取整。"),
                    new UIView
                    {
                        ClassName = "progress-line-stack",
                        Children = new()
                        {
                            // Steps > 0 时不再绘制连续轨道，而是绘制指定数量的独立分段。
                            CreateLineItem("五段进度", new UIProgress
                            {
                                Percent = 50,
                                Steps = 5,
                            }),
                            // StepColors 按索引设置每个已完成分段的颜色。
                            CreateLineItem("逐段配色", new UIProgress
                            {
                                Percent = 66,
                                Steps = 8,
                                StepColors = new SKColor[]
                                {
                                    ColorHelper.ParseColor("#1677ff"),
                                    ColorHelper.ParseColor("#13c2c2"),
                                    ColorHelper.ParseColor("#52c41a"),
                                    ColorHelper.ParseColor("#a0d911"),
                                    ColorHelper.ParseColor("#faad14"),
                                    ColorHelper.ParseColor("#fa8c16"),
                                    ColorHelper.ParseColor("#f5222d"),
                                    ColorHelper.ParseColor("#eb2f96"),
                                },
                            }),
                            // 未启用 Steps 时，StrokeGradient 直接填充连续进度轨道。
                            CreateLineItem("渐变轨道", new UIProgress
                            {
                                Percent = 93,
                                StrokeGradient = gradient,
                            }),
                            // Rounding 接收理论完成段数；此处向下取整，避免未完全达到时提前点亮下一段。
                            CreateLineItem("向下取整", new UIProgress
                            {
                                Percent = 48,
                                Steps = 7,
                                Rounding = value => (int)Math.Floor(value),
                                StrokeColor = ColorHelper.ParseColor("#722ed1"),
                            }),
                        },
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "progress-circle-grid", "progress-circle-grid-compact" },
                        Children = new()
                        {
                            // Circle 的每个分段使用渐变采样色，StepGap 控制相邻圆弧的像素间距。
                            CreateCircleItem("Circle Steps", new UIProgress
                            {
                                Type = ProgressType.Circle,
                                Percent = 60,
                                Steps = 12,
                                StepGap = 4,
                                StrokeGradient = gradient,
                                Size = new ProgressSize(120),
                            }),
                            // Dashboard 也支持 Steps，并继续保留 GapDegree 定义的整体缺口。
                            CreateCircleItem("Dashboard Steps", new UIProgress
                            {
                                Type = ProgressType.Dashboard,
                                Percent = 50,
                                Steps = 10,
                                StepGap = 4,
                                GapDegree = 80,
                                StrokeColor = ColorHelper.ParseColor("#722ed1"),
                                Size = new ProgressSize(120),
                            }),
                        },
                    },
                };
            }
        }

        /// <summary>
        /// 动态进度演示。
        /// 两个 UIProgress 共用同一数值，通过按钮实时更新以验证属性重绘与状态切换。
        /// </summary>
        private sealed class DynamicSection : UIView
        {
            /// <summary>
            /// 构建动态演示卡片并连接按钮事件。
            /// </summary>
            internal DynamicSection()
            {
                // 条形进度使用 Active 状态，以便数值变化时同时观察进行中动画。
                var line = new UIProgress
                {
                    Percent = 30,
                    Status = ProgressStatus.Active,
                };
                // 圆形进度与条形进度保持数值同步，用于验证不同 Type 的运行时更新。
                var circle = new UIProgress
                {
                    Type = ProgressType.Circle,
                    Percent = 30,
                    Size = new ProgressSize(104),
                    StrokeColor = ColorHelper.ParseColor("#13c2c2"),
                };

                // 所有入口统一通过此方法更新，确保两个组件始终使用相同百分比。
                void Change(float delta)
                {
                    // UIProgress.Percent 自身也会限制范围；这里先 Clamp 便于明确 Demo 的边界行为。
                    float next = Math.Clamp(line.Percent + delta, 0f, 100f);
                    line.Percent = next;
                    circle.Percent = next;
                }

                ClassName = "progress-demo-card";
                Children = new()
                {
                    CreateSectionTitle("动态展示", "label-red"),
                    CreateSectionDescription("Percent 是可更新属性；设置后组件会立即重绘，并在 Auto 状态下于 100% 自动切换为成功图标。"),
                    new UIView
                    {
                        ClassName = "progress-dynamic-row",
                        Children = new()
                        {
                            // 左侧区域放置条形进度和操作按钮。
                            new UIView
                            {
                                ClassName = "progress-dynamic-main",
                                Children = new()
                                {
                                    CreateLineItem("当前进度", line),
                                    new UIView
                                    {
                                        ClassName = "progress-actions",
                                        Children = new()
                                        {
                                            CreateActionButton("− 10", () => Change(-10)),
                                            CreateActionButton("+ 10", () => Change(10)),
                                        },
                                    },
                                },
                            },
                            // 右侧圆形进度用于直观看到与条形进度同步的结果。
                            CreateCircleItem("同步进度", circle),
                        },
                    },
                };
            }

            /// <summary>
            /// 创建动态演示使用的操作按钮。
            /// </summary>
            /// <param name="text">按钮显示文字。</param>
            /// <param name="click">按钮点击后执行的进度更新逻辑。</param>
            /// <returns>带统一样式和 Click 事件的 UIButton。</returns>
            private static UIButton CreateActionButton(string text, Action click)
            {
                return new UIButton
                {
                    Text = text,
                    ClassName = "progress-action-button",
                    Events = new()
                    {
                        Click = _ => click(),
                    },
                };
            }
        }
    }
}
