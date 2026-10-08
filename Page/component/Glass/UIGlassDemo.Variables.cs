using TCYM.UI.Core;
using TCYM.UI.Elements;

namespace TCYM.UI.Example.Page.component.Glass
{
    internal sealed partial class UIGlassDemo
    {
        // 与实际 style.css 保持相同的变量声明，便于直接复制核心用法。
        private const string VariablesCss = """
            /* 局部变量只影响此容器及其后代，不污染其他演示。 */
            .css-vars-scope {
                --accent: #2563eb;
                --gap: 8px;
                --alias: var(--accent);
                --Color: #7c3aed;
                --color: #ea580c;
            }

            .css-vars-value {
                color: var(--accent);
                border: 1px solid var(--accent);
                padding: var(--gap) calc(var(--gap) * 2);
            }

            .css-vars-shadow { --accent: #059669; }

            .css-vars-composite {
                border: 1px solid var(--accent);
                padding: var(--gap) calc(var(--gap) * 2);
            }

            .css-vars-meter {
                width: calc(var(--gap) * 12);
                background: var(--accent);
            }

            .css-vars-fallback {
                color: var(--missing, var(--fallback, var(--accent, #334155)));
            }

            .css-vars-alias-child {
                --accent: #059669;
                color: var(--accent);
                /* alias 在父作用域计算，继承后不会被子元素的 accent 改写。 */
                border-color: var(--alias);
            }

            .css-vars-case-upper { color: var(--Color); border-color: var(--Color); }
            .css-vars-case-lower { color: var(--color); border-color: var(--color); }
            .css-vars-case-missing { color: var(--COLOR, #64748b); border-color: var(--COLOR, #64748b); }
            """;

        /// <summary>
        /// 创建变量作用域和动态更新演示，仅在点击时修改变量，不使用轮询或循环动画。
        /// </summary>
        private static UIView CreateVariableSection()
        {
            var inlineValue = CreateVariableValue("css-vars-inline", "元素变量：固定红色，不受父级切换影响");
            inlineValue.Style = new DefaultUIStyle
            {
                Variables = new() { ["--accent"] = "#be123c" },
            };

            var scope = new UIView
            {
                Id = "css-vars-demo-scope",
                ClassName = "css-vars-scope",
                Children = new()
                {
                    CreateVariableCard("局部变量与继承", "父级蓝色可切换为紫色；子作用域和元素变量独立覆盖。",
                        CreateVariableValue("css-vars-inherited", "继承父级 --accent：随按钮切换"),
                        new UIView
                        {
                            ClassName = "css-vars-shadow",
                            Children = new()
                            {
                                CreateVariableValue("css-vars-shadow", "子作用域：固定绿色"),
                            },
                        },
                        inlineValue,
                        CreateVariableValue("css-vars-sibling", "兄弟元素：仍继承父级，不被绿色污染")),
                    CreateVariableCard("复合属性 + 多个 var() + calc()", "点击间距按钮：8px / 14px；色条宽度为 gap × 12。",
                        new UIView
                        {
                            Id = "css-vars-composite",
                            ClassName = "css-vars-composite",
                            Children = new()
                            {
                                new UILabel
                                {
                                    Text = "边框使用 accent；上下留白 gap，左右留白 gap × 2。",
                                    ClassName = "css-vars-explanation",
                                },
                                new UIView { Id = "css-vars-meter", ClassName = "css-vars-meter" },
                            },
                        },
                        new UILabel
                        {
                            Text = "border: 1px solid var(--accent);\npadding: var(--gap) calc(var(--gap) * 2);\nwidth: calc(var(--gap) * 12);",
                            ClassName = "css-vars-code-preview",
                        }),
                    CreateVariableCard("嵌套回退与计算值继承", "缺失变量逐层回退；父级 alias 计算后再继承给子元素。",
                        CreateVariableValue("css-vars-fallback", "嵌套回退：最终采用父级 accent", "css-vars-fallback"),
                        CreateVariableValue("css-vars-alias-child", "子级文字绿色；边框使用父级 alias", "css-vars-alias-child"),
                        new UILabel
                        {
                            Text = "var(--missing, var(--fallback, var(--accent, #334155)))\n父级：--alias: var(--accent);\n子级：--accent: #059669; border-color: var(--alias);",
                            ClassName = "css-vars-code-preview",
                        }),
                    CreateVariableCard("变量名区分大小写", "--Color、--color 是两个变量；未定义的 --COLOR 使用回退值。",
                        CreateVariableValue("css-vars-case-upper", "--Color → 紫色 #7c3aed", "css-vars-case-upper"),
                        CreateVariableValue("css-vars-case-lower", "--color → 橙色 #ea580c", "css-vars-case-lower"),
                        CreateVariableValue("css-vars-case-missing", "--COLOR 未定义 → 灰色 #64748b", "css-vars-case-missing")),
                },
            };

            bool purple = false;
            bool largeGap = false;
            var status = new UILabel
            {
                Id = "css-vars-status",
                Text = "当前变量：--accent = #2563eb；--gap = 8px；色条宽度 = 96px",
                ClassName = "css-vars-status",
            };

            // 从实际计算后的作用域读取值，只在按钮操作后刷新说明文字。
            void RefreshStatus()
            {
                status.Text = $"当前变量：--accent = {scope.GetCssVariable("--accent")}；--gap = {scope.GetCssVariable("--gap")}；色条宽度 = {(largeGap ? 168 : 96)}px";
            }

            // 按钮放在变量作用域外，调整间距时不会改变按钮的布局和命中区域。
            return new UIView
            {
                Id = "css-vars-demo-section",
                ClassName = "glass-demo-card",
                Children = new()
                {
                    CreateSectionTitle("CSS 变量：作用域、组合替换与动态更新"),
                    CreateSectionDescription("以下四张卡片共享局部变量。点击按钮，观察文字、边框、内边距和色条宽度同步更新；其他演示不受影响。"),
                    new UIView
                    {
                        ClassName = "css-vars-controls",
                        Children = new()
                        {
                            new UIButton
                            {
                                Id = "css-vars-theme-button",
                                Text = "切换蓝色 / 紫色",
                                ClassName = "css-vars-button",
                                Events = new()
                                {
                                    Click = _ =>
                                    {
                                        purple = !purple;
                                        scope.SetCssVariable("--accent", purple ? "#9333ea" : "#2563eb");
                                        RefreshStatus();
                                    },
                                },
                            },
                            new UIButton
                            {
                                Id = "css-vars-gap-button",
                                Text = "间距 8px / 14px",
                                ClassName = "css-vars-button",
                                Events = new()
                                {
                                    Click = _ =>
                                    {
                                        largeGap = !largeGap;
                                        scope.SetCssVariable("--gap", largeGap ? "14px" : "8px");
                                        RefreshStatus();
                                    },
                                },
                            },
                            new UIButton
                            {
                                Id = "css-vars-reset-button",
                                Text = "恢复 CSS 初始值",
                                ClassName = "css-vars-button",
                                Events = new()
                                {
                                    Click = _ =>
                                    {
                                        // 移除运行时覆盖，让样式表中的原始变量重新生效。
                                        scope.RemoveCssVariable("--accent");
                                        scope.RemoveCssVariable("--gap");
                                        purple = false;
                                        largeGap = false;
                                        RefreshStatus();
                                    },
                                },
                            },
                        },
                    },
                    status,
                    scope,
                    new UILabel
                    {
                        Text = "C# 动态更新：scope.SetCssVariable(\"--accent\", \"#9333ea\");\n恢复样式表：scope.RemoveCssVariable(\"--accent\");\n元素局部变量：Style = new DefaultUIStyle { Variables = new() { [\"--accent\"] = \"#be123c\" } };",
                        ClassName = "css-vars-code-preview",
                    },
                },
            };
        }

        /// <summary>
        /// 复用选择器演示的卡片布局，组织变量标题、说明和实际预览元素。
        /// </summary>
        private static UIView CreateVariableCard(string title, string description, params UIElement[] content)
        {
            var card = new UIView
            {
                ClassName = new List<string> { "glass-selector-example", "css-vars-card" },
                Children = new()
                {
                    new UILabel { Text = title, ClassName = "glass-selector-example-title" },
                    new UILabel { Text = description, ClassName = "glass-selector-example-desc" },
                },
            };
            foreach (var element in content)
                card.Children.Add(element);
            return card;
        }

        /// <summary>
        /// 创建消费变量的文字样本，额外类用于覆盖局部变量或指定回退表达式。
        /// </summary>
        private static UILabel CreateVariableValue(string id, string text, string extraClass = "")
        {
            var classes = new List<string> { "css-vars-value" };
            if (!string.IsNullOrEmpty(extraClass))
                classes.Add(extraClass);
            return new UILabel { Id = id, Text = text, ClassName = classes };
        }
    }
}
