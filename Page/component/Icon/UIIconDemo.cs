using System.Text.Json;
using System.Text.Json.Serialization;
using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.CodeEditor;
using TCYM.UI.Elements.Message;
using TCYM.UI.Elements.Tabs;
using TCYM.UI.Elements.Tooltip;
using TCYM.UI.Enums;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Icon
{
    internal class IconFontItem
    {
        public string icon_id { get; set; } = "";
        public string name { get; set; } = "";
        public string font_class { get; set; } = "";
        public string unicode { get; set; } = "";
        public int unicode_decimal { get; set; }
    }

    [JsonSerializable(typeof(List<IconFontItem>))]
    internal partial class IconDemoJsonContext : JsonSerializerContext { }


    internal class UIIconDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Icon.style.css";

        private const string CSharpCode = "new UIView\n{\n    ClassName = new List<string> { \"icon-demo-card\" },\n    Children = new()\n    {\n        new UIView\n        {\n            Style = new DefaultUIStyle\n            {\n                Width = 130,\n                Height = 130,\n                Display = \"flex\",\n                JustifyContent = \"center\",\n                AlignItems = \"center\",\n                BorderColor = ColorHelper.ParseColor(\"#8b8989\"),\n                BorderWidth = 1,\n                BorderRadius = 8,\n                BorderStyle = \"dashed\",\n                PointerEvents = \"box-only\",\n            },\n            Children = new()\n            {\n                new UIIcon\n                {\n                    Content = \"&#xe612;\",\n                    Style = new DefaultUIStyle\n                    {\n                        FontFamily = UIFontManager.Get(\"TCYMIconFont\"),\n                        Color = ColorHelper.ParseColor(\"#555\"),\n                        FontSize = 20,\n                        Width = 30,\n                        Height = 30,\n                    }\n                }\n            }\n        }\n    }\n};";

        private const string CssCode = ".icon-demo-card {\n    width: 100%;\n    padding: 10px;\n    display: flex;\n    gap: 10px;\n    flex-wrap: wrap;\n    border: 1px solid rgba(0,0,0,0.06);\n    border-radius: 6px;\n    margin-top: 10px;\n}";

        private bool _codeVisible = false;
        private UIView _codePanel;

        internal UIIconDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "icon-demo-view" };

            var header = new UIView
            {
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Display = "flex",
                    AlignItems = "center",
                    JustifyContent = "space-between",
                    PaddingRight= 10
                },
                Children = new()
                {
                    new UILabel
                    {
                        Text = "图标列表",
                        ClassName = new List<string> { "label-title" }
                    },
                    new UITooltip
                    {
                        Title = "查看代码",
                        Children = new()
                        {
                            new UIIcon
                            {
                                Content = "&#xe61c;",
                                ClassName = new List<string> { "code-toolbar-icon" },
                                Style = new DefaultUIStyle
                                {
                                    FontFamily = UIFontManager.Get("TCYMIconFont"),
                                    FontSize = 20,
                                    Cursor = UICursor.Pointer,
                                },
                                Events = new()
                                {
                                    Click = _ =>
                                    {
                                        _codeVisible = !_codeVisible;
                                        var code = UISystem.Manager?.GetElementById("icon-demo-codePanel") as UIView;
                                        code?.SetClassNames(new List<string> { "code-panel" });
                                        code?.SetStyle(new DefaultUIStyle
                                        {
                                            Display = _codeVisible ? "block" : "none"
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _codePanel = new UIView
            {
                Id = "icon-demo-codePanel",
                ClassName = new List<string> { "icon-code-panel" },
                Children = new()
                {
                    new UITabs
                    {
                        Style = new DefaultUIStyle { Width = "100%", Height = "100%" },
                        TabBarStyle = new DefaultUIStyle { Height = 40 },
                        ContentStyle = new DefaultUIStyle { Width = "100%", Height = "calc(100% - 40px)" },
                        IndicatorColor = SKColor.Parse("#1677FF"),
                        ActiveTextColor = SKColor.Parse("#1677FF"),
                        DefaultActiveKey = "cs",
                        Items = new List<TabItem>
                        {
                            new TabItem("cs", "C#", new UICodeEditor
                            {
                                Text = CSharpCode,
                                Language = CodeEditorLanguage.CSharp,
                                ReadOnly = true,
                                ShowMinimap = true,
                                ShowLineNumbers = true,
                                Style = new DefaultUIStyle { Width = "100%", Height = "100%" }
                            }),
                            new TabItem("css", "CSS", new UICodeEditor
                            {
                                Text = CssCode,
                                Language = CodeEditorLanguage.Css,
                                ReadOnly = true,
                                ShowMinimap = true,
                                ShowLineNumbers = true,
                                Style = new DefaultUIStyle { Width = "100%", Height = "100%" }
                            })
                        }
                    }
                }
            };

            Children = new()
            {
                new UILabel
                {
                    Text = "Icon 图标",
                    ClassName = new List<string> { "icon-card-title" }
                },
                new UILabel
                {
                    Text = "语义化的矢量图形。",
                    ClassName = new List<string> { "icon-card-subtitle" }
                },
                new UILabel
                {
                    Text = "使用图标组件，你需要安装对应的图标字体并通过 UIFontManager 注册。\n\n图标的命令式使用方式：\n• 通过 UIIcon 组件展示图标，支持自定义字体、大小、颜色等。\n• 可以嵌套在 UIButton 等组件中作为前缀/后缀图标。\n• 支持通过 CSS 动画实现旋转、缩放等效果。\n• 支持 Unicode 字符引用（如 &#xe612;）和转义序列（如 \\ue612）。",
                    ClassName = new List<string> { "icon-card-desc" }
                },
                header,
                _codePanel,
                new IconDemo()
            };
        }

        class IconDemo : UIView
        {
            internal IconDemo()
            {
                string json = UIFileHelper.ReadFileAsString("res://TCYM.UI.Example/Page.component.Icon.iconfont.json");
                List<IconFontItem> IconMap = JsonSerializer.Deserialize(json, IconDemoJsonContext.Default.ListIconFontItem)
                    ?? new List<IconFontItem>();

                ClassName = new List<string> { "icon-demo-card" };
                foreach (var icon in IconMap)
                {
                    AddChild(new UIView
                    {
                        Style = new DefaultUIStyle
                        {
                            //Width = UIStyleParser.ParseUnitPublic("calc((100% - 15px) / 4 )"),
                            Width = 130,
                            Height = 130,
                            Display = "flex",
                            JustifyContent = "center",
                            AlignItems = "center",
                            Position = "relative",
                            BorderColor = ColorHelper.ParseColor("#8b8989"),
                            BorderWidth = 1,
                            BorderRadius = 8,
                            BorderStyle = "dashed",
                            PointerEvents = "box-only",
                            Before = new DefaultUIStyle
                            {
                               Content = icon.name,
                               Color = ColorHelper.ParseColor("#8b8989"),
                               Opacity = 0.1f,
                               Position = "absolute",
                               FontSize = 14,
                               Top= "30%",
                               Left = "50%",
                               TransformCss = "translateX(-50%)",
                               Bottom = 8
                            },
                            Hover = new DefaultUIStyle
                            {
                                BackgroundColor = ColorHelper.ParseColor("#1677ff"),
                            }
                        },
                        Children = new()
                        {
                            new UIIcon
                            {
                                Content = $"&#x{icon.unicode};", // 解析为 Unicode 字符引用 或者 $"\\u{icon.unicode}" 转义序列
                                Style = new DefaultUIStyle
                                {
                                    FontFamily = UIFontManager.Get("TCYMIconFont"),
                                    Color = ColorHelper.ParseColor("#555"),
                                    Display = "flex",
                                    AlignItems = "center",
                                    JustifyContent = "center",
                                    FontSize = 20,
                                    Width = 30,
                                    Height = 30,
                                },
                            }
                        },
                        Events = new()
                        {
                            MouseEnter = (e) =>
                            {
                                if (e.Element?.Children != null)
                                {
                                    foreach (var item in e.Element.Children)
                                    {
                                        item.UpdateStyle(new UpdateUIStyle
                                        {
                                            Color = SKColors.White,
                                            TransformCss = "scale(1.5)",
                                            Transition = "transform 0.1s ease-out",
                                        });
                                    }
                                }
                                if (e.Element?.Style?.Before != null)
                                {
                                    e.Element.Style.Before.Color = SKColors.White;
                                }
                            },
                            MouseLeave = (e) =>
                            {
                                if (e.Element?.Children != null)
                                {
                                    foreach (var item in e.Element.Children)
                                    {
                                        item.UpdateStyle(new UpdateUIStyle
                                        {
                                            Color = ColorHelper.ParseColor("#555"),
                                            TransformCss = "scale(1)",
                                            Transition = "transform 0.2s ease-out",
                                        });
                                    }
                                }
                                if (e.Element?.Style?.Before != null)
                                {
                                    e.Element.Style.Before.Color = ColorHelper.ParseColor("#8b8989");
                                }
                            },
                            Click = (e) =>
                            {
                                // 点击图标时赋值到剪贴板
                                UIClipboard.SetText($"&#x{icon.unicode};");
                                // 显示提示消息
                                UIMessage.Success($"{icon.name} 已复制 &#x{icon.unicode}; 到剪贴板");
                            }
                        }
                    });
                }
            }
        }
    }
}