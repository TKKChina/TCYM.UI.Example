using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;

namespace TCYM.UI.Example.Page.component.Label
{
    internal partial class UILabelDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Label.style.css";
        private const string LongText = "TCYM.UI 的 UILabel 支持内容测量、自动换行、文本对齐、多行省略以及高亮片段，适合承载标题、描述、状态和辅助说明等文本场景。";
        private const string AlignmentPreviewText = "第一行文本较长，用于观察对齐效果。\n第二行较短。\n第三行中等长度。";
        // 所有空白策略使用同一份原文：四个连续空格、显式换行，以及一段足以跨行的尾部空格。
        private const string WhiteSpacePreviewText = "开始    连续空格\n第二行：这段中文比较长，用于观察固定宽度下的自动换行。\n行尾空格                                                \n结束。";
        private const string DirectionPreviewText = "ABC 123 שלום";
        private const string WritingModePreviewText = "春江花月夜\n山水映长空\nTCYM.UI 123";

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
                    Text = "当前 Demo 展示文本样式、对齐换行、CSS white-space / text-overflow / direction / writing-mode / user-select、多行省略、高亮片段，以及 flex 行布局下与子元素组合的能力。",
                    ClassName = new List<string> { "label-demo-desc" },
                },
                CreateBasicSection(),
                CreatePrioritySection(),
                CreateAlignmentSection(),
                CreateWhiteSpaceSection(),
                CreateTextOverflowSection(),
                CreateDirectionSection(),
                CreateWritingModeSection(),
                CreateUserSelectSection(),
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
                    ClassName = new List<string> { "label-showcase" },
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

        /// <summary>用相同原文对比六种 CSS 空白与换行策略，固定预览尺寸便于观察差异。</summary>
        private static UIView CreateWhiteSpaceSection()
        {
            return CreateSectionCard(
                "CSS white-space：空白与换行",
                "六张卡片使用同一原文：开始后有四个空格，包含显式换行，“行尾空格”后有一段连续空格。预览区固定宽高，超出的内容直接裁剪；观察空格是否保留、换行是否生效，以及“结束。”的位置。",
                new UIView
                {
                    ClassName = new List<string> { "label-flow-grid" },
                    Children = new()
                    {
                        CreateTextFlowPreview("white-space: normal", "折叠连续空格与换行，按可用宽度自动换行。", WhiteSpacePreviewText, "label-white-normal"),
                        CreateTextFlowPreview("white-space: nowrap", "折叠连续空格与换行，整段保持单行；超出右侧的内容裁剪。", WhiteSpacePreviewText, "label-white-nowrap"),
                        CreateTextFlowPreview("white-space: pre", "保留空格和原文换行，不自动折行；长行超出右侧时裁剪。", WhiteSpacePreviewText, "label-white-pre"),
                        CreateTextFlowPreview("white-space: pre-wrap", "保留空格和原文换行，同时允许自动折行；行尾空格可悬挂。", WhiteSpacePreviewText, "label-white-pre-wrap"),
                        CreateTextFlowPreview("white-space: pre-line", "折叠连续空格，保留原文换行，同时允许自动折行。", WhiteSpacePreviewText, "label-white-pre-line"),
                        CreateTextFlowPreview("white-space: break-spaces", "保留每个空格并计入换行宽度；行尾空格可占用额外一行。", WhiteSpacePreviewText, "label-white-break-spaces"),
                    }
                });
        }

        /// <summary>在相同受限宽度下对比裁剪与省略；实际行为完全由 CSS 声明控制。</summary>
        private static UIView CreateTextOverflowSection()
        {
            return CreateSectionCard(
                "CSS text-overflow：裁剪与省略",
                "两项均设置 width: 100%、white-space: nowrap 和 overflow: hidden。原始 Text 保持完整，clip 直接裁剪，ellipsis 在可见文字末尾显示省略号。",
                new UIView
                {
                    ClassName = new List<string> { "label-flow-grid" },
                    Children = new()
                    {
                        CreateTextFlowPreview("text-overflow: clip", "超出内容区的文字直接裁掉，不显示省略号。", LongText, "label-overflow-sample", "label-overflow-clip"),
                        CreateTextFlowPreview("text-overflow: ellipsis", "为末尾的“…”预留空间，缩短或放宽容器后重新计算。", LongText, "label-overflow-sample", "label-overflow-ellipsis"),
                    }
                },
                new UILabel
                {
                    Text = ".label-title {\n    width: 200px;\n    white-space: nowrap;\n    overflow: hidden;\n    text-overflow: ellipsis;\n}",
                    ClassName = new List<string> { "label-flow-code" },
                });
        }

        /// <summary>对比基础方向和内联起点，并展示 RTL 溢出时左侧的省略号。</summary>
        private static UIView CreateDirectionSection()
        {
            return CreateSectionCard(
                "CSS direction：文字基础方向",
                "前两项使用相同的英文、数字和希伯来文，并设置 text-align: start，让起始位置跟随 direction。英文和数字保持各自顺序；第三项演示 RTL 文本超出宽度后的省略效果。",
                new UIView
                {
                    ClassName = new List<string> { "label-flow-grid" },
                    Children = new()
                    {
                        CreateTextFlowPreview("direction: ltr", "从左侧开始，观察英文、数字和希伯来文的排列。", DirectionPreviewText, "label-direction-sample", "label-direction-ltr"),
                        CreateTextFlowPreview("direction: rtl", "从右侧开始，混合文字按基础方向排列。", DirectionPreviewText, "label-direction-sample", "label-direction-rtl"),
                        CreateTextFlowPreview("RTL + text-overflow", "超出时在左侧显示“…”；右侧的英文和数字不会倒序。", "ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789", "label-direction-sample", "label-direction-rtl", "label-direction-ellipsis"),
                    }
                },
                new UILabel
                {
                    Text = ".rtl-label {\n    width: 200px;\n    direction: rtl;\n    text-align: start;\n    white-space: nowrap;\n    overflow: hidden;\n    text-overflow: ellipsis;\n}",
                    ClassName = new List<string> { "label-flow-code" },
                });
        }

        /// <summary>用同一段中英文对比三种书写模式，并补充竖排 RTL 与省略组合。</summary>
        private static UIView CreateWritingModeSection()
        {
            return CreateSectionCard(
                "CSS writing-mode：横排与竖排",
                "前三项使用相同原文：蓝色是第一行（列），绿色是第二行（列）。竖排中文保持直立，英文和数字旋转显示；第四项展示 direction: rtl 从底端开始、溢出时顶部显示省略号。",
                new UIView
                {
                    ClassName = new List<string> { "label-flow-grid" },
                    Children = new()
                    {
                        CreateWritingModePreview("horizontal-tb", "横向书写，各行从上往下排列。", "label-writing-horizontal"),
                        CreateWritingModePreview("vertical-rl", "纵向书写，蓝色首列在右侧，随后向左分列。", "label-writing-rl"),
                        CreateWritingModePreview("vertical-lr", "纵向书写，蓝色首列在左侧，随后向右分列。", "label-writing-lr"),
                        CreateTextFlowPreview("vertical-rl + rtl", "固定高度下不换列；观察顶部省略号和底端对齐。", "甲乙丙丁戊己庚辛壬癸子丑寅卯辰巳午未申酉戌亥", "label-writing-sample", "label-writing-rtl-ellipsis"),
                    }
                },
                new UILabel
                {
                    Text = ".vertical-label {\n    height: 200px;\n    writing-mode: vertical-rl;\n    direction: ltr;\n    text-align: start;\n    white-space: pre;\n}",
                    ClassName = new List<string> { "label-flow-code" },
                });
        }

        /// <summary>为前两行设置原文高亮，直观标出横排与两种竖排列顺序。</summary>
        private static UIView CreateWritingModePreview(string title, string description, string modeClass)
        {
            var card = CreateTextFlowPreview(title, description, WritingModePreviewText, "label-writing-sample", modeClass);
            var sample = card.Children.OfType<UILabel>().Last();
            sample.Highlights = new List<TextHighlight>
            {
                new() { Value = "春江花月夜", Color = "#1677ff" },
                new() { Value = "山水映长空", Color = "#389e0d" },
            };
            return card;
        }

        /// <summary>创建 CSS 文本对比卡片，保留 UILabel 默认 API 配置，让样式表决定排版。</summary>
        private static UIView CreateTextFlowPreview(string title, string description, string text, params string[] sampleClasses)
        {
            var classes = new List<string> { "label-flow-sample" };
            classes.AddRange(sampleClasses);
            return new UIView
            {
                ClassName = new List<string> { "label-flow-box" },
                Children = new()
                {
                    new UILabel { Text = title, ClassName = new List<string> { "label-preview-title" } },
                    new UILabel { Text = description, ClassName = new List<string> { "label-flow-note" } },
                    new UILabel { Text = text, ClassName = classes },
                }
            };
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
                "Highlights 默认按 Value 精确匹配；设置 IsRegex=true 后，Value 将作为正则表达式，适合搜索结果、高风险关键词、编号和命中规则提示等场景。",
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
                            Text = "风控提示：订单 ERR-1042 金额异常、设备 WARN-77 指纹异常、登录地区异常。",
                            ClassName = new List<string> { "label-highlight-sample" },
                            Highlights = new List<TextHighlight>
                            {
                                new() { Value = @"\b(?:ERR|WARN)-\d+\b", Color = "#1677ff", IsRegex = true },
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
