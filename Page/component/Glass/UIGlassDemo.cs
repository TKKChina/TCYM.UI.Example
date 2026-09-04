using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.CodeEditor;

namespace TCYM.UI.Example.Page.component.Glass
{
    /// <summary>
    /// 展示 CSS backdrop-filter 与 TCYM.UI 液体玻璃边缘折射。
    /// </summary>
    internal sealed class UIGlassDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Glass.style.css";

        private const string GlassCss = """
            .frosted-glass {
                backdrop-filter: blur(18px) saturate(145%) brightness(105%);
                background: rgba(255, 255, 255, 0.13);
                border: 1px solid rgba(255, 255, 255, 0.30);
                border-radius: 24px;
                corner-curve: continuous;
                box-shadow: 0 18px 46px rgba(3, 10, 30, 0.24);
            }

            .liquid-glass {
                backdrop-filter: blur(18px) saturate(145%) brightness(105%);
                -tcym-backdrop-refraction: 12px;
                background: rgba(255, 255, 255, 0.13);
                border: 1px solid rgba(255, 255, 255, 0.30);
                border-radius: 24px;
                corner-curve: continuous;
                box-shadow: 0 18px 46px rgba(3, 10, 30, 0.24);
            }

            .liquid-glass:hover {
                -tcym-backdrop-refraction: 18px;
            }

            .glass-panel {
                animation: glass-demo-panel-float 7s ease-in-out infinite;
            }

            @keyframes glass-demo-panel-float {
                0% { transform: translateY(0); }
                50% { transform: translateY(-12px); }
                100% { transform: translateY(0); }
            }

            .dark-glass {
                backdrop-filter: blur(16px) saturate(125%) brightness(82%) contrast(110%);
                background: rgba(7, 15, 38, 0.48);
                border: 1px solid rgba(255, 255, 255, 0.18);
                border-radius: 20px;
                corner-curve: continuous;
                box-shadow: 0 14px 34px rgba(7, 15, 38, 0.24);
            }

            .glass-hover-surface:hover {
                background-color: #2563eb;
                border-color: #1d4ed8;
                color: #ffffff;
            }

            .glass-active-surface:active {
                background-color: #ea580c;
                border-color: #c2410c;
                color: #ffffff;
                opacity: 0.82;
            }

            .glass-adjacent-source + .glass-adjacent-target {
                background-color: #2563eb;
                color: #ffffff;
            }

            .glass-general-source ~ .glass-general-target {
                background-color: #059669;
                color: #ffffff;
            }

            .glass-first-child-item:first-child {
                border-color: #7c3aed;
                background-color: #8b5cf6;
                color: #ffffff;
            }

            .glass-nth-child-item:nth-child(2n) {
                border-color: #be185d;
                background-color: #ec4899;
                color: #ffffff;
            }

            .glass-not-item:not(.glass-not-disabled) {
                border-color: #0f766e;
                background-color: #14b8a6;
                color: #ffffff;
            }

            .glass-is-item:is(.glass-is-primary, .glass-is-success) {
                border-color: #0369a1;
                background-color: #0ea5e9;
                color: #ffffff;
            }

            .glass-has-surface:has(.glass-has-indicator) {
                border-color: #7c3aed;
                background-color: #ede9fe;
                color: #4c1d95;
            }
            """;

        internal UIGlassDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);

            ClassName = "glass-demo-view";
            Children = new()
            {
                new UILabel
                {
                    Text = "CSS",
                    ClassName = "glass-demo-title",
                },
                new UILabel
                {
                    Text = "Backdrop Filter · Frosted Glass · Liquid Refraction",
                    ClassName = "glass-demo-title-sub",
                },
                new UILabel
                {
                    Text = "透明背景由 backdrop-filter 负责模糊和调色；背景保持静止，玻璃面板会持续缓慢移动，便于观察实时采样。液体玻璃额外使用 -tcym-backdrop-refraction 产生边缘折射。",
                    ClassName = "glass-demo-desc",
                },
                CreateHeroSection(),
                CreateComparisonSection(),
                CreateSelectorSection(),
                CreateCodeSection(),
            };
        }

        private static UIView CreateHeroSection()
        {
            return new UIView
            {
                ClassName = "glass-demo-card",
                Children = new()
                {
                    CreateSectionTitle("磨砂与液体玻璃"),
                    CreateSectionDescription("两个场景使用完全相同的静态背景、滤镜链和卡片尺寸，玻璃面板以相同节奏缓慢上下移动；右侧仅增加边缘折射。将鼠标移到液体玻璃上可增强折射。"),
                    new UIView
                    {
                        ClassName = "glass-scene-row",
                        Children = new()
                        {
                            CreateGlassScene(liquid: false),
                            CreateGlassScene(liquid: true),
                        },
                    },
                },
            };
        }

        private static UIView CreateComparisonSection()
        {
            return new UIView
            {
                ClassName = "glass-demo-card",
                Children = new()
                {
                    CreateSectionTitle("滤镜链对比"),
                    CreateSectionDescription("同一背景依次展示透明填充、组合磨砂滤镜和深色玻璃。文字与彩色条纹位于玻璃后方，便于观察真实采样结果。"),
                    new UIView
                    {
                        ClassName = "glass-compare-stage",
                        Children = new()
                        {
                            new UIView { ClassName = new List<string> { "glass-ribbon", "glass-ribbon-blue" } },
                            new UIView { ClassName = new List<string> { "glass-ribbon", "glass-ribbon-pink" } },
                            new UIView { ClassName = new List<string> { "glass-ribbon", "glass-ribbon-green" } },
                            new UILabel
                            {
                                Text = "BACKGROUND  /  COLOR  /  DETAIL  /  CONTRAST",
                                ClassName = "glass-compare-watermark",
                            },
                            new UIView
                            {
                                ClassName = "glass-compare-row",
                                Children = new()
                                {
                                    CreateComparisonTile("plain-glass", "仅透明填充", "没有 backdrop-filter", "rgba(255,255,255,.10)"),
                                    CreateComparisonTile("compact-frosted-glass", "组合磨砂", "blur + saturate + brightness", "blur(12px)"),
                                    CreateComparisonTile("dark-glass", "深色玻璃", "brightness + contrast", "dark material"),
                                },
                            },
                        },
                    },
                },
            };
        }

        /// <summary>
        /// 创建交互伪类、结构伪类、函数伪类与兄弟选择器演示区域。
        /// </summary>
        private static UIView CreateSelectorSection()
        {
            return new UIView
            {
                ClassName = "glass-demo-card",
                Children = new()
                {
                    CreateSectionTitle("CSS 交互与高级选择器"),
                    CreateSectionDescription("前两项观察 :hover / :active；接着依次展示 :first-child、:nth-child(2n)、:not()、:is() 与 :has()；最后两项展示相邻兄弟 + 与通用兄弟 ~。"),
                    new UIView
                    {
                        ClassName = "glass-selector-grid",
                        Children = new()
                        {
                            CreateSelectorStateTile(
                                ":hover",
                                "鼠标进入时应用悬停样式",
                                "glass-hover-surface",
                                "将鼠标移入此区域"),
                            CreateSelectorStateTile(
                                ":active",
                                "鼠标左键或触摸按住时应用",
                                "glass-active-surface",
                                "按住此区域查看状态"),
                            CreateChildPositionSelectorTile(
                                ":first-child",
                                "同一父元素的 4 个直接子元素中，仅第 1 项命中。",
                                "glass-first-child-item"),
                            CreateChildPositionSelectorTile(
                                ":nth-child(2n)",
                                "按真实 Children 序号匹配偶数项，第 2、4 项命中。",
                                "glass-nth-child-item"),
                            CreateFunctionalSelectorTile(
                                ":not(.disabled)",
                                "排除带 disabled 类的元素，第 1、3 项命中。",
                                new (string Text, string ClassName)[]
                                {
                                    ("默认 · 命中", "glass-sibling-pill glass-not-item"),
                                    ("disabled", "glass-sibling-pill glass-not-item glass-not-disabled"),
                                    ("默认 · 命中", "glass-sibling-pill glass-not-item"),
                                    ("disabled", "glass-sibling-pill glass-not-item glass-not-disabled"),
                                }),
                            CreateFunctionalSelectorTile(
                                ":is(.primary, .success)",
                                "匹配 primary 或 success 任一分支，第 1、3 项命中。",
                                new (string Text, string ClassName)[]
                                {
                                    ("primary", "glass-sibling-pill glass-is-item glass-is-primary"),
                                    ("neutral", "glass-sibling-pill glass-is-item"),
                                    ("success", "glass-sibling-pill glass-is-item glass-is-success"),
                                    ("neutral", "glass-sibling-pill glass-is-item"),
                                }),
                            CreateHasSelectorTile(),
                            new UIView
                            {
                                ClassName = "glass-selector-example",
                                Children = new()
                                {
                                    new UILabel { Text = "相邻兄弟 +", ClassName = "glass-selector-example-title" },
                                    new UILabel
                                    {
                                        Text = "只有紧跟在源元素后的目标会命中。",
                                        ClassName = "glass-selector-example-desc",
                                    },
                                    new UIView
                                    {
                                        ClassName = "glass-sibling-row",
                                        Children = new()
                                        {
                                            new UILabel
                                            {
                                                Text = "源元素",
                                                ClassName = new List<string> { "glass-sibling-pill", "glass-adjacent-source" },
                                            },
                                            new UILabel
                                            {
                                                Text = "+ 命中",
                                                ClassName = new List<string> { "glass-sibling-pill", "glass-adjacent-target" },
                                            },
                                            new UILabel
                                            {
                                                Text = "间隔",
                                                ClassName = "glass-sibling-pill",
                                            },
                                            new UILabel
                                            {
                                                Text = "未命中",
                                                ClassName = new List<string> { "glass-sibling-pill", "glass-adjacent-target" },
                                            },
                                        },
                                    },
                                },
                            },
                            new UIView
                            {
                                ClassName = "glass-selector-example",
                                Children = new()
                                {
                                    new UILabel { Text = "通用兄弟 ~", ClassName = "glass-selector-example-title" },
                                    new UILabel
                                    {
                                        Text = "源元素之后的全部目标都会命中，即使中间存在其它兄弟。",
                                        ClassName = "glass-selector-example-desc",
                                    },
                                    new UIView
                                    {
                                        ClassName = "glass-sibling-row",
                                        Children = new()
                                        {
                                            new UILabel
                                            {
                                                Text = "源元素",
                                                ClassName = new List<string> { "glass-sibling-pill", "glass-general-source" },
                                            },
                                            new UILabel { Text = "间隔", ClassName = "glass-sibling-pill" },
                                            new UILabel
                                            {
                                                Text = "~ 命中 A",
                                                ClassName = new List<string> { "glass-sibling-pill", "glass-general-target" },
                                            },
                                            new UILabel
                                            {
                                                Text = "~ 命中 B",
                                                ClassName = new List<string> { "glass-sibling-pill", "glass-general-target" },
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            };
        }

        /// <summary>
        /// 创建单个交互伪类演示卡片。
        /// </summary>
        private static UIView CreateSelectorStateTile(
            string title,
            string description,
            string stateClass,
            string content)
        {
            return new UIView
            {
                ClassName = "glass-selector-example",
                Children = new()
                {
                    new UILabel { Text = title, ClassName = "glass-selector-example-title" },
                    new UILabel { Text = description, ClassName = "glass-selector-example-desc" },
                    new UILabel
                    {
                        Text = content,
                        ClassName = new List<string> { "glass-selector-surface", stateClass },
                    },
                },
            };
        }

        /// <summary>
        /// 创建结构伪类演示卡片，四个目标元素共享同一个直接父元素。
        /// </summary>
        private static UIView CreateChildPositionSelectorTile(
            string title,
            string description,
            string itemClass)
        {
            return new UIView
            {
                ClassName = "glass-selector-example",
                Children = new()
                {
                    new UILabel { Text = title, ClassName = "glass-selector-example-title" },
                    new UILabel { Text = description, ClassName = "glass-selector-example-desc" },
                    new UIView
                    {
                        ClassName = "glass-sibling-row",
                        Children = new()
                        {
                            new UILabel
                            {
                                Text = "第 1 项",
                                ClassName = new List<string> { "glass-sibling-pill", itemClass },
                            },
                            new UILabel
                            {
                                Text = "第 2 项",
                                ClassName = new List<string> { "glass-sibling-pill", itemClass },
                            },
                            new UILabel
                            {
                                Text = "第 3 项",
                                ClassName = new List<string> { "glass-sibling-pill", itemClass },
                            },
                            new UILabel
                            {
                                Text = "第 4 项",
                                ClassName = new List<string> { "glass-sibling-pill", itemClass },
                            },
                        },
                    },
                },
            };
        }

        /// <summary>
        /// 创建函数伪类演示卡片，并按传入顺序构造同一父元素下的目标项。
        /// </summary>
        private static UIView CreateFunctionalSelectorTile(
            string title,
            string description,
            IReadOnlyList<(string Text, string ClassName)> items)
        {
            var itemElements = new List<UIElement>(items.Count);
            foreach (var item in items)
            {
                itemElements.Add(new UILabel
                {
                    Text = item.Text,
                    ClassName = item.ClassName,
                });
            }

            return new UIView
            {
                ClassName = "glass-selector-example",
                Children = new()
                {
                    new UILabel { Text = title, ClassName = "glass-selector-example-title" },
                    new UILabel { Text = description, ClassName = "glass-selector-example-desc" },
                    new UIView
                    {
                        ClassName = "glass-sibling-row",
                        Children = itemElements,
                    },
                },
            };
        }

        /// <summary>
        /// 创建 <c>:has()</c> 演示卡片，由父容器根据内部状态子项决定自身样式。
        /// </summary>
        private static UIView CreateHasSelectorTile()
        {
            return new UIView
            {
                ClassName = "glass-selector-example",
                Children = new()
                {
                    new UILabel
                    {
                        Text = ":has(.status)",
                        ClassName = "glass-selector-example-title",
                    },
                    new UILabel
                    {
                        Text = "父容器包含 status 子项，因此父容器自身命中并高亮。",
                        ClassName = "glass-selector-example-desc",
                    },
                    new UIView
                    {
                        ClassName = "glass-has-surface",
                        Children = new()
                        {
                            new UILabel
                            {
                                Text = "普通子项",
                                ClassName = "glass-sibling-pill",
                            },
                            new UILabel
                            {
                                Text = "status",
                                ClassName = "glass-sibling-pill glass-has-indicator",
                            },
                            new UILabel
                            {
                                Text = "普通子项",
                                ClassName = "glass-sibling-pill",
                            },
                        },
                    },
                },
            };
        }

        private static UIView CreateCodeSection()
        {
            return new UIView
            {
                ClassName = "glass-demo-card",
                Children = new()
                {
                    CreateSectionTitle("CSS 用法"),
                    CreateSectionDescription("backdrop-filter 支持按声明顺序组合与重复函数；-webkit-backdrop-filter 也可作为兼容别名。折射强度为 0 时自动退化为标准磨砂。"),
                    new UICodeEditor
                    {
                        Text = GlassCss,
                        Language = CodeEditorLanguage.Css,
                        ReadOnly = true,
                        ClassName = "glass-code-editor",
                    },
                },
            };
        }

        private static UIView CreateGlassScene(bool liquid)
        {
            return new UIView
            {
                ClassName = "glass-scene",
                Children = new()
                {
                    new UIView { ClassName = new List<string> { "glass-orb", "glass-orb-blue" } },
                    new UIView { ClassName = new List<string> { "glass-orb", "glass-orb-violet" } },
                    new UIView { ClassName = new List<string> { "glass-orb", "glass-orb-warm" } },
                    new UIView { ClassName = "glass-guide-x" },
                    new UIView { ClassName = "glass-guide-y" },
                    new UILabel
                    {
                        Text = "REAL BACKDROP",
                        ClassName = "glass-stage-watermark",
                    },
                    CreateGlassPanel(
                        liquid ? "liquid-glass" : "frosted-glass",
                        liquid ? "LIQUID-LITE" : "FROSTED",
                        liquid ? "液体边缘折射" : "标准磨砂玻璃",
                        liquid
                            ? "相同磨砂链，仅增加圆角边缘位移；中心区域保持稳定。"
                            : "高斯模糊与饱和度增强，不附加框架硬编码白色蒙层。",
                        liquid ? "refraction 12px" : "blur 18px"),
                },
            };
        }

        private static UIView CreateGlassPanel(
            string variantClass,
            string eyebrow,
            string title,
            string description,
            string metric)
        {
            return new UIView
            {
                ClassName = new List<string> { "glass-panel", variantClass },
                Children = new()
                {
                    new UILabel { Text = eyebrow, ClassName = "glass-panel-eyebrow" },
                    new UILabel { Text = title, ClassName = "glass-panel-title" },
                    new UILabel { Text = description, ClassName = "glass-panel-description" },
                    new UIView
                    {
                        ClassName = "glass-panel-footer",
                        Children = new()
                        {
                            new UILabel { Text = "CSS BACKDROP", ClassName = "glass-panel-chip" },
                            new UILabel { Text = metric, ClassName = "glass-panel-metric" },
                        },
                    },
                },
            };
        }

        private static UIView CreateComparisonTile(
            string variantClass,
            string title,
            string description,
            string value)
        {
            return new UIView
            {
                ClassName = new List<string> { "glass-compare-tile", variantClass },
                Children = new()
                {
                    new UILabel { Text = title, ClassName = "glass-compare-title" },
                    new UILabel { Text = description, ClassName = "glass-compare-description" },
                    new UILabel { Text = value, ClassName = "glass-compare-value" },
                },
            };
        }

        private static UILabel CreateSectionTitle(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = "glass-card-title",
            };
        }

        private static UILabel CreateSectionDescription(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = "glass-card-desc",
            };
        }
    }
}
