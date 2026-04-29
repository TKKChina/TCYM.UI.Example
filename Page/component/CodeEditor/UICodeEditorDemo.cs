using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.CodeEditor;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.CodeEditor
{
    internal class UICodeEditorDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.CodeEditor.style.css";

        private static readonly Dictionary<CodeEditorLanguage, string> CodeSamples = new()
        {
            [CodeEditorLanguage.CSharp] = "using System;\n\nnamespace Demo\n{\n    public class Program\n    {\n        public static void Main()\n        {\n            Console.WriteLine(\"Hello TCYM.UI CodeEditor\");\n        }\n    }\n}",
            [CodeEditorLanguage.Java] = "package demo;\n\npublic class App {\n    public static void main(String[] args) {\n        System.out.println(\"Hello TCYM.UI CodeEditor\");\n    }\n}",
            [CodeEditorLanguage.JavaScript] = "const greeting = 'Hello TCYM.UI CodeEditor';\n\nfunction runDemo() {\n  console.log(greeting);\n}\n\nrunDemo();",
            [CodeEditorLanguage.TypeScript] = "type User = {\n  id: number;\n  name: string;\n};\n\nconst currentUser: User = { id: 1, name: 'TCYM' };\nconsole.log(currentUser.name);",
            [CodeEditorLanguage.Css] = ".editor-card {\n  display: flex;\n  flex-direction: column;\n  gap: 12px;\n  padding: 16px;\n  border-radius: 12px;\n}",
            [CodeEditorLanguage.Json] = "{\n  \"name\": \"TCYM.UI\",\n  \"framework\": \"net8.0\",\n  \"features\": [\n    \"syntax-highlight\",\n    \"find-replace\",\n    \"minimap\"\n  ]\n}"
        };

        internal UICodeEditorDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);

            ClassName = new List<string> { "code-editor-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "CodeEditor 代码编辑器",
                    ClassName = new List<string> { "code-editor-demo-title", "label-title" }
                },
                new UILabel
                {
                    Text = "基于 UICodeEditor 的语法高亮、查找替换、最小地图和只读预览示例。",
                    ClassName = new List<string> { "code-editor-demo-title-sub" }
                },
                new UILabel
                {
                    Text = "支持 C#、Java、JavaScript、TypeScript、CSS、JSON。内置快捷键包括 Ctrl+F 查找、Ctrl+H 替换、F3/Shift+F3 匹配跳转，以及 Alt+0 系列代码折叠。",
                    ClassName = new List<string> { "code-editor-demo-desc" }
                },
                new PlaygroundSection(),
                new ReadOnlyPreviewSection(),
                new FeatureHintSection()
            };
        }

        private sealed class PlaygroundSection : UIView
        {
            internal PlaygroundSection()
            {
                ClassName = new List<string> { "code-editor-demo-card" };

                var statusLabel = new UILabel
                {
                    ClassName = new List<string> { "code-editor-status" }
                };

                var editor = new UICodeEditor
                {
                    Language = CodeEditorLanguage.CSharp,
                    Text = CodeSamples[CodeEditorLanguage.CSharp],
                    ShowLineNumbers = true,
                    ShowMinimap = true,
                    AutoFormatOnInitialize = false,
                    Style = new DefaultUIStyle
                    {
                        Width = "100%",
                        Height = 420,
                        FontSize = 14,
                    }
                };

                editor.OnValueChanged += _ => UpdateStatus(statusLabel, editor, "编辑中");
                UpdateStatus(statusLabel, editor, "默认载入 C# 示例");

                Children = new()
                {
                    CreateSectionTitle("基础编辑与语言切换"),
                    CreateSectionDescription("这一块直接对应 TCYM.UI 内已有的 UICodeEditorDome，用于演示语言高亮、文本编辑、行号与最小地图切换。"),
                    CreateToolbar(
                        CreateLanguageButton("C#", CodeEditorLanguage.CSharp, editor, statusLabel),
                        CreateLanguageButton("Java", CodeEditorLanguage.Java, editor, statusLabel),
                        CreateLanguageButton("JavaScript", CodeEditorLanguage.JavaScript, editor, statusLabel),
                        CreateLanguageButton("TypeScript", CodeEditorLanguage.TypeScript, editor, statusLabel),
                        CreateLanguageButton("CSS", CodeEditorLanguage.Css, editor, statusLabel),
                        CreateLanguageButton("JSON", CodeEditorLanguage.Json, editor, statusLabel),
                        CreateToggleButton("切换行号", () =>
                        {
                            editor.ShowLineNumbers = !editor.ShowLineNumbers;
                            RefreshEditor(editor);
                            UpdateStatus(statusLabel, editor, editor.ShowLineNumbers ? "已显示行号" : "已隐藏行号");
                        }),
                        CreateToggleButton("切换 Minimap", () =>
                        {
                            editor.ShowMinimap = !editor.ShowMinimap;
                            RefreshEditor(editor);
                            UpdateStatus(statusLabel, editor, editor.ShowMinimap ? "已显示 Minimap" : "已隐藏 Minimap");
                        })
                    ),
                    statusLabel,
                    new UIView
                    {
                        ClassName = new List<string> { "code-editor-shell" },
                        Children = new()
                        {
                            editor
                        }
                    }
                };
            }
        }

        private sealed class ReadOnlyPreviewSection : UIView
        {
            internal ReadOnlyPreviewSection()
            {
                ClassName = new List<string> { "code-editor-demo-card" };

                var preview = new UICodeEditor
                {
                    Language = CodeEditorLanguage.Json,
                    Text = CodeSamples[CodeEditorLanguage.Json],
                    ReadOnly = true,
                    ShowLineNumbers = true,
                    ShowMinimap = false,
                    AutoFormatOnInitialize = false,
                    EditorBackground = ColorHelper.ParseColor("#fffdf7"),
                    EditorBorder = ColorHelper.ParseColor("#eadfbe"),
                    EditorText = ColorHelper.ParseColor("#3b2f1f"),
                    IdentifierColor = ColorHelper.ParseColor("#7c3aed"),
                    KeywordColor = ColorHelper.ParseColor("#155eef"),
                    StringColor = ColorHelper.ParseColor("#b54708"),
                    NumberColor = ColorHelper.ParseColor("#027a48"),
                    LineNumberText = ColorHelper.ParseColor("#8c6d46"),
                    GutterBackground = ColorHelper.ParseColor("#f7f1e1"),
                    CurrentLineBackground = ColorHelper.ParseColor("#fff4cc"),
                    CursorColor = ColorHelper.ParseColor("#7a4b00"),
                    Style = new DefaultUIStyle
                    {
                        Width = "100%",
                        Height = 280,
                        FontSize = 14,
                    }
                };

                Children = new()
                {
                    CreateSectionTitle("只读预览与主题定制"),
                    CreateSectionDescription("ReadOnly=true 适合日志、配置、脚本预览等场景。颜色属性可以直接覆盖，快速做出不同主题。"),
                    new UILabel
                    {
                        Text = "这个示例关闭了 Minimap，并切换为浅色主题，便于展示编辑器的配色覆盖能力。",
                        ClassName = new List<string> { "code-editor-inline-note" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "code-editor-shell", "code-editor-shell-light" },
                        Children = new()
                        {
                            preview
                        }
                    }
                };
            }
        }

        private sealed class FeatureHintSection : UIView
        {
            internal FeatureHintSection()
            {
                ClassName = new List<string> { "code-editor-demo-card" };
                Children = new()
                {
                    CreateSectionTitle("能力说明"),
                    CreateSectionDescription("当前示例页主要覆盖已经在框架里实现完成的编辑器能力，便于在 Example 工程里直接回归验证。"),
                    CreateHintItem("语法高亮：内置 C# / Java / JavaScript / TypeScript / CSS / JSON 六种语言。"),
                    CreateHintItem("编辑能力：支持键盘输入、选区、剪贴板、撤销栈以及粘贴后自动格式化。"),
                    CreateHintItem("辅助能力：支持查找替换、代码折叠、最小地图、横向滚动和只读模式。"),
                    CreateHintItem("适用场景：代码片段编辑、脚本配置、日志预览、DSL 输入和结果回显。")
                };
            }
        }

        private static UIButton CreateLanguageButton(string text, CodeEditorLanguage language, UICodeEditor editor, UILabel statusLabel)
        {
            return CreateActionButton(text, () =>
            {
                editor.Language = language;
                editor.Text = CodeSamples[language];
                RefreshEditor(editor);
                UpdateStatus(statusLabel, editor, $"已切换到 {GetLanguageLabel(language)} 示例");
            }, "code-editor-lang-button");
        }

        private static UIButton CreateToggleButton(string text, Action action)
        {
            return CreateActionButton(text, action, "code-editor-toggle-button");
        }

        private static UIButton CreateActionButton(string text, Action action, params string[] extraClasses)
        {
            var classNames = new List<string> { "code-editor-action-button" };
            classNames.AddRange(extraClasses);

            return new UIButton
            {
                Text = text,
                ClassName = classNames,
                Events = new()
                {
                    Click = _ => action()
                }
            };
        }

        private static UIView CreateToolbar(params UIElement[] children)
        {
            return new UIView
            {
                ClassName = new List<string> { "code-editor-toolbar" },
                Children = children.ToList(),
            };
        }

        private static UILabel CreateSectionTitle(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "code-editor-card-title", "label-title" }
            };
        }

        private static UILabel CreateSectionDescription(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "code-editor-card-desc" }
            };
        }

        private static UILabel CreateHintItem(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "code-editor-hint-item" }
            };
        }

        private static void UpdateStatus(UILabel label, UICodeEditor editor, string prefix)
        {
            int lineCount = editor.Text.Split('\n').Length;
            label.Text = $"{prefix} | 语言：{GetLanguageLabel(editor.Language)} | {lineCount} 行 | {editor.Text.Length} 字符 | 行号：{(editor.ShowLineNumbers ? "开" : "关")} | Minimap：{(editor.ShowMinimap ? "开" : "关")}";
            label.RequestLayout();
            label.RequestRedraw();
        }

        private static string GetLanguageLabel(CodeEditorLanguage language)
        {
            return language switch
            {
                CodeEditorLanguage.CSharp => "C#",
                CodeEditorLanguage.Java => "Java",
                CodeEditorLanguage.JavaScript => "JavaScript",
                CodeEditorLanguage.TypeScript => "TypeScript",
                CodeEditorLanguage.Css => "CSS",
                CodeEditorLanguage.Json => "JSON",
                _ => "PlainText"
            };
        }

        private static void RefreshEditor(UICodeEditor editor)
        {
            editor.RequestLayout();
            editor.RequestRedraw();
            UISystem.Manager?.MarkDirty();
        }
    }
}