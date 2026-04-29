using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;

namespace TCYM.UI.Example.Page.component.Label
{
    internal class UILabelDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Label.style.css";
        private const string LongText = "TCYM.UI 的 UILabel 支持内容测量、自动换行、文本对齐、多行省略以及高亮片段，适合承载标题、描述、状态和辅助说明等文本场景。";
        private const string AlignmentPreviewText = "第一行文本较长，用于观察对齐效果。\n第二行较短。\n第三行中等长度。";

        internal UILabelDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);

            ClassName = new List<string> { "label-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Label 文本",
                    ClassName = new List<string> { "label-demo-title" },
                },
                new UILabel
                {
                    Text = "基础文本显示控件，覆盖常见的标题、正文、辅助信息和状态标签场景。",
                    ClassName = new List<string> { "label-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "当前 Demo 重点展示 UILabel 的文本样式、对齐换行、多行省略、高亮片段，以及在 flex 行布局下与子元素组合的能力。",
                    ClassName = new List<string> { "label-demo-desc" },
                },
                CreateBasicSection(),
                CreatePrioritySection(),
                CreateAlignmentSection(),
                CreateEllipsisSection(),
                CreateHighlightSection(),
                CreateFlexSection(),
            };
        }

        private static UIView CreateBasicSection()
        {
            return CreateSectionCard(
                "基础文本样式",
                "UILabel 可直接承载标题、正文、辅助说明和强调信息，样式完全由 UIStyle 控制。",
                new UIView
                {
                    ClassName = new List<string> { "label-showcase", "label-showcase-column" },
                    Children = new()
                    {
                        new UILabel
                        {
                            Text = "页面主标题 / 30px Bold",
                            ClassName = new List<string> { "label-sample-title" },
                        },
                        new UILabel
                        {
                            Text = "正文内容通常承载较长描述，强调可读性、间距和层级关系。",
                            ClassName = new List<string> { "label-sample-body" },
                        },
                        new UILabel
                        {
                            Text = "辅助说明文本用于解释上下文或帮助用户理解当前状态。",
                            ClassName = new List<string> { "label-sample-subtle" },
                        },
                        new UILabel
                        {
                            Text = "高优先级提醒：当前环境检测到 2 项待处理配置。",
                            ClassName = new List<string> { "label-sample-warning" },
                        }
                    }
                }
            );
        }

        private static UIView CreatePrioritySection()
        {
            return CreateSectionCard(
                "优先级与 Flex",
                "普通文本渲染时，优先级为 UILabel.TextAlign > Style.TextAlign > CSS 的 text-align > 全局 * 的文本对齐兜底。进入 display:flex 且 flex-direction:row 并带子元素后，justify-content / align-items 控制的是整组内容的位置。",
                new UIView
                {
                    ClassName = new List<string> { "label-priority-list" },
                    Children = new()
                    {
                        CreatePriorityItem("1", "UILabel.TextAlign 直接属性最高。"),
                        CreatePriorityItem("2", "Style.TextAlign 次之，当没有直接属性时覆盖 CSS。"),
                        CreatePriorityItem("3", "CSS 的 text-align 再次之，适用于类样式和选择器规则。"),
                        CreatePriorityItem("4", "全局 * 的文本对齐只作为最低优先级兜底。"),
                        CreatePriorityItem("5", "当 UILabel 变成 flex 行容器并带子元素时，justify-content / align-items 决定整组位置，TextAlign 不再主导整组排布。"),
                    }
                },
                new UIView
                {
                    ClassName = new List<string> { "label-priority-grid" },
                    Children = new()
                    {
                        CreatePriorityPreview(
                            "直接属性最高",
                            "同时设置 UILabel.TextAlign=right、Style.TextAlign=center、CSS text-align:left，最终以 UILabel.TextAlign 为准。",
                            new UILabel
                            {
                                Text = AlignmentPreviewText,
                                TextAlign = "right",
                                ClassName = new List<string> { "label-priority-sample", "label-priority-css-left" },
                                Style = new DefaultUIStyle
                                {
                                    Width = "100%",
                                    TextAlign = "center",
                                }
                            }
                        ),
                        CreatePriorityPreview(
                            "Style 次之",
                            "不设置 UILabel.TextAlign，只设置 Style.TextAlign=center 和 CSS text-align:right，最终以 Style 为准。",
                            new UILabel
                            {
                                Text = AlignmentPreviewText,
                                ClassName = new List<string> { "label-priority-sample", "label-priority-css-right" },
                                Style = new DefaultUIStyle
                                {
                                    Width = "100%",
                                    TextAlign = "center",
                                }
                            }
                        ),
                        CreatePriorityPreview(
                            "CSS 生效",
                            "没有直接属性和 Style.TextAlign 时，类样式里的 text-align:right 生效。",
                            new UILabel
                            {
                                Text = AlignmentPreviewText,
                                ClassName = new List<string> { "label-priority-sample", "label-priority-css-right" },
                            }
                        ),
                        CreatePriorityPreview(
                            "全局 * 兜底",
                            "这里临时模拟全局 * { text-align: center; }。控件本身没有设置直接属性、Style.TextAlign 或 CSS text-align，因此取全局兜底值。",
                            CreateGlobalFallbackPreviewLabel()
                        ),
                        CreatePriorityPreview(
                            "Flex 单独看",
                            "这里故意同时设置 UILabel.TextAlign=right 和 justify-content:center。由于 Label 已进入 flex 行布局，最终由 justify-content:center 控制整组内容居中。",
                            CreateFlexPriorityPreviewLabel()
                        ),
                    }
                }
            );
        }

        private static UIView CreateAlignmentSection()
        {
            return CreateSectionCard(
                "对齐与换行",
                "通过 TextAlign 控制左中右对齐；默认支持自动换行，适合固定宽度信息区和卡片说明区。",
                new UIView
                {
                    ClassName = new List<string> { "label-align-grid" },
                    Children = new()
                    {
                        CreateAlignPreview("左对齐", "left"),
                        CreateAlignPreview("居中", "center"),
                        CreateAlignPreview("右对齐", "right"),
                    }
                }
            );
        }

        private static UIView CreateEllipsisSection()
        {
            return CreateSectionCard(
                "多行省略",
                "MaxLines 和 Ellipsize 适合概览卡片、列表摘要和表格单元格内容预览。",
                new UIView
                {
                    ClassName = new List<string> { "label-ellipsis-grid" },
                    Children = new()
                    {
                        CreateEllipsisPreview("单行省略", 1, true),
                        CreateEllipsisPreview("两行省略", 2, true),
                        CreateEllipsisPreview("两行完整换行", 2, false),
                    }
                }
            );
        }

        private static UIView CreateHighlightSection()
        {
            return CreateSectionCard(
                "文本高亮",
                "通过 Highlights 指定文本片段和颜色，适合搜索结果、高风险关键词、命中规则提示等场景。",
                new UIView
                {
                    ClassName = new List<string> { "label-showcase", "label-showcase-column" },
                    Children = new()
                    {
                        new UILabel
                        {
                            Text = "搜索命中：Label Demo 支持 Highlight、Wrap 和 Ellipsize。",
                            ClassName = new List<string> { "label-highlight-sample" },
                            Highlights = new List<TextHighlight>
                            {
                                new() { Value = "Label Demo", Color = "#1677ff" },
                                new() { Value = "Highlight", Color = "#fa541c" },
                                new() { Value = "Ellipsize", Color = "#52c41a" },
                            }
                        },
                        new UILabel
                        {
                            Text = "风控提示：订单金额异常、设备指纹异常、登录地区异常。",
                            ClassName = new List<string> { "label-highlight-sample" },
                            Highlights = new List<TextHighlight>
                            {
                                new() { Value = "异常", Color = "#cf1322" },
                                new() { Value = "风控提示", Color = "#722ed1" },
                            }
                        }
                    }
                }
            );
        }

        private static UIView CreateFlexSection()
        {
            return CreateSectionCard(
                "行内组合",
                "当 UILabel 设为 display:flex 且 flex-direction:row 时，文本可以和子元素水平排列，适合状态标签和组合信息。",
                new UIView
                {
                    ClassName = new List<string> { "label-flex-grid" },
                    Children = new()
                    {
                        CreateFlexLabel(
                            "服务状态",
                            "运行中",
                            "label-chip-success",
                            new SKColor(82, 196, 26)
                        ),
                        CreateFlexLabel(
                            "发布渠道",
                            "Beta",
                            "label-chip-beta",
                            new SKColor(22, 119, 255)
                        ),
                        CreateFlexLabel(
                            "风险等级",
                            "需复核",
                            "label-chip-warning",
                            new SKColor(250, 173, 20)
                        ),
                    }
                }
            );
        }

        private static UIView CreateSectionCard(string title, string description, params UIElement[] children)
        {
            var sectionChildren = new List<UIElement>
            {
                new UILabel
                {
                    Text = title,
                    ClassName = new List<string> { "label-card-title","label-title" },
                },
                new UILabel
                {
                    Text = description,
                    ClassName = new List<string> { "label-card-desc" },
                }
            };

            sectionChildren.AddRange(children);

            return new UIView
            {
                ClassName = new List<string> { "label-demo-card" },
                Children = sectionChildren,
            };
        }

        private static UIView CreatePriorityItem(string index, string text)
        {
            return new UIView
            {
                ClassName = new List<string> { "label-priority-item" },
                Children = new()
                {
                    new UILabel
                    {
                        Text = index,
                        ClassName = new List<string> { "label-priority-index" },
                    },
                    new UILabel
                    {
                        Text = text,
                        ClassName = new List<string> { "label-priority-text" },
                    }
                }
            };
        }

        private static UIView CreatePriorityPreview(string title, string note, UIElement sample)
        {
            return new UIView
            {
                ClassName = new List<string> { "label-priority-box" },
                Children = new()
                {
                    new UILabel
                    {
                        Text = title,
                        ClassName = new List<string> { "label-priority-box-title" },
                    },
                    new UILabel
                    {
                        Text = note,
                        ClassName = new List<string> { "label-priority-box-note" },
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "label-priority-shell" },
                        Children = new() { sample }
                    }
                }
            };
        }

        private static UILabel CreateGlobalFallbackPreviewLabel()
        {
            var previous = StyleGlobals.UniversalTextAlign;
            try
            {
                StyleGlobals.UniversalTextAlign = "center";
                var label = new UILabel
                {
                    Text = AlignmentPreviewText,
                };
                label.SetStyle(new DefaultUIStyle
                {
                    Width = "100%",
                    FontSize = 14,
                });
                return label;
            }
            finally
            {
                StyleGlobals.UniversalTextAlign = previous;
            }
        }

        private static UILabel CreateFlexPriorityPreviewLabel()
        {
            return new UILabel
            {
                Text = "当前环境",
                TextAlign = "right",
                ClassName = new List<string> { "label-priority-flex-row" },
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Display = "flex",
                    FlexDirection = "row",
                    AlignItems = "center",
                    JustifyContent = "center",
                    Gap = 8,
                },
                Children = new()
                {
                    new UIView
                    {
                        Style = new DefaultUIStyle
                        {
                            Width = 8,
                            Height = 8,
                            BorderRadius = 999,
                            BackgroundColor = new SKColor(22, 119, 255),
                        }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "label-inline-chip", "label-chip-beta" },
                        Children = new()
                        {
                            new UILabel
                            {
                                Text = "居中排列",
                                ClassName = new List<string> { "label-inline-chip-text" },
                            }
                        }
                    }
                }
            };
        }

        private static UIView CreateAlignPreview(string title, string align)
        {
            return new UIView
            {
                ClassName = new List<string> { "label-preview-box" },
                Children = new()
                {
                    new UILabel
                    {
                        Text = title,
                        ClassName = new List<string> { "label-preview-title" },
                    },
                    new UILabel
                    {
                        Text = AlignmentPreviewText,
                        TextAlign = align,
                        ClassName = new List<string> { "label-preview-content" },
                        
                    }
                }
            };
        }

        private static UIView CreateEllipsisPreview(string title, int maxLines, bool ellipsize)
        {
            return new UIView
            {
                ClassName = new List<string> { "label-preview-box" },
                Children = new()
                {
                    new UILabel
                    {
                        Text = title,
                        ClassName = new List<string> { "label-preview-title" },
                    },
                    new UILabel
                    {
                        Text = LongText,
                        MaxLines = maxLines,
                        Ellipsize = ellipsize,
                        ClassName = new List<string> { "label-preview-content" },
                    }
                }
            };
        }

        private static UIView CreateFlexLabel(string prefix, string chipText, string chipClassName, SKColor dotColor)
        {
            return new UIView
            {
                ClassName = new List<string> { "label-flex-card" },
                Children = new()
                {
                    new UILabel
                    {
                        Text = prefix,
                        ClassName = new List<string> { "label-flex-title" },
                    },
                    new UILabel
                    {
                        Text = "当前环境",
                        ClassName = new List<string> { "label-inline-row" },
                        Style = new DefaultUIStyle
                        {
                            Display = "flex",
                            FlexDirection = "row",
                            AlignItems = "center",
                            Gap = 8,
                        },
                        Children = new()
                        {
                            new UIView
                            {
                                Style = new DefaultUIStyle
                                {
                                    Width = 8,
                                    Height = 8,
                                    BorderRadius = 999,
                                    BackgroundColor = dotColor,
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "label-inline-chip", chipClassName },
                                Children = new()
                                {
                                    new UILabel
                                    {
                                        Text = chipText,
                                        ClassName = new List<string> { "label-inline-chip-text" },
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}