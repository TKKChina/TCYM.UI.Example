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
