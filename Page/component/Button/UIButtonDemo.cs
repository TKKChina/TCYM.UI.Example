using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.CodeEditor;
using TCYM.UI.Elements.Message;
using TCYM.UI.Elements.Tabs;
using TCYM.UI.Elements.Tooltip;

namespace TCYM.UI.Example.Page.component.Button
{
    internal class UIButtonDemo : UIView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Button.style.css";

        internal UIButtonDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);

            Style = new DefaultUIStyle
            {
                Width = "100%",
                Height = "100%",
            };

            Children = new()
            {
                new UILabel
                {
                    Text = "Button 按钮",
                    ClassName = new List<string> { "btn-card-title" }
                },
                new UILabel
                {
                    Text = "按钮用于开始一个即时操作。",
                    ClassName = new List<string> { "btn-card-subtitle" }
                },
                new UILabel
                {
                    Text = "何时使用 标记了一个（或封装一组）操作命令，响应用户点击行为，触发相应的业务逻辑。",
                    ClassName = new List<string> { "btn-card-desc" }
                },
                new UIScrollView
                {
                    ClassName = new List<string> { "btn-demo-scroll" },
                    Children = new()
                    {
                        CreateSectionTitle("基础按钮-代码演示", "label-title"),
                        new BaseButton(),
                        CreateSectionTitle("图标按钮-代码演示", "label-green"),
                        new IconButton(),
                        CreateSectionTitle("禁用状态-代码演示", "label-orange"),
                        new DisableButton()
                    }
                }
            };
        }

        private static UILabel CreateSectionTitle(string text, string className)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { className }
            };
        }

        class BaseButton : UIView
        {
            private const string CSharpCode = "new UIView\n{\n    ClassName = new List<string> { \"btn-box\" },\n    Children = new()\n    {\n        new UIButton\n        {\n            Text = \"Primary Button\",\n            ClassName = new List<string> { \"btn\", \"btn-primary\" },\n            Events = new()\n            {\n                Click = _ =>\n                {\n                    UIMessage.Info(\"点击了 Primary Button\");\n                }\n            }\n        },\n        new UIButton\n        {\n            Text = \"Default Button\",\n            ClassName = new List<string> { \"btn\", \"btn-default\" },\n            Events = new()\n            {\n                Click = _ =>\n                {\n                    UIMessage.Warning(\"点击了 Default Button\");\n                }\n            }\n        },\n        new UIButton\n        {\n            Text = \"Dashed Button\",\n            ClassName = new List<string> { \"btn\", \"btn-dashed\" }\n        },\n        new UIButton\n        {\n            Text = \"Text Button\",\n            ClassName = new List<string> { \"btn\", \"btn-text\" }\n        },\n        new UIButton\n        {\n            Text = \"Link Button\",\n            ClassName = new List<string> { \"btn\", \"btn-link\" }\n        },\n    }\n};";

            private const string CssCode = ".btn-box {\n    width: 100%;\n    padding: 5px;\n    display: flex;\n    gap: 5px;\n}\n\n.btn {\n    padding: 5px;\n    border-radius: 6px;\n    cursor: pointer;\n}\n\n.btn-primary {\n    color: #FFFFFF;\n    background: #1677FF;\n}\n\n.btn-default {\n    color: #000000;\n    background: #FFFFFF;\n}\n\n.btn-dashed {\n    color: #000000;\n    border: 1px dashed rgba(0,0,0,0.2);\n}\n\n.btn-text {\n    color: #000000;\n}\n\n.btn-text:hover {\n    background: rgba(0,0,0,0.1);\n}\n\n.btn-link {\n    color: #1677FF;\n}\n\n.btn-link:hover {\n    text-decoration: underline;\n}";

            private bool _codeVisible = false;
            private UIView _codePanel;

            internal BaseButton()
            {
                Style = new DefaultUIStyle { Width = "100%" };

                var buttonRow = new UIView
                {
                    ClassName = new List<string> { "btn-box" },
                    Children = new()
                    {
                        new UIButton
                        { 
                            Text = "Primary Button", 
                            ClassName = new List<string> { "btn", "btn-primary" },
                            Events = new()
                            {
                                Click = _ =>
                                {
                                    var info = UISystem.GetCurrentDisplayInfo();
                                     Console.WriteLine($"当前屏幕: {info.Index}");
                                    Console.WriteLine($"分辨率: {info.Width}x{info.Height}");
                                    Console.WriteLine($"可用区域: {info.UsableWidth}x{info.UsableHeight}");
                                    Console.WriteLine($"DPI: {info.Dpi}, HDPI: {info.Hdpi}, VDPI: {info.Vdpi}");
                                    Console.WriteLine($"Scale: {info.Scale}");
                                    Console.WriteLine($"IsTablet: {UISystem.IsTabletDevice()}");
                                    Console.WriteLine($"Orientation: {UISystem.GetCurrentTabletOrientation()}");
                                    UIMessage.Info("点击了 Primary Button");
                                }
                            }
                        },
                        new UIButton
                        { 
                            Text = "Default Button",
                            ClassName = new List<string> { "btn", "btn-default" },
                            Events = new()
                            {
                                Click = _ =>
                                {
                                    UIMessage.Warning("点击了 Default Button");
                                }
                            }
                        },
                        new UIButton { Text = "Dashed Button", ClassName = new List<string> { "btn", "btn-dashed" } },
                        new UIButton { Text = "Text Button", ClassName = new List<string> { "btn", "btn-text" } },
                        new UIButton { Text = "Link Button", ClassName = new List<string> { "btn", "btn-link" } },
                    }
                };


                var toolbar = new UIView
                {
                    ClassName = new List<string> { "code-toolbar" },
                    Children = new()
                    {
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
                                        Cursor = Enums.UICursor.Pointer,
                                    },
                                    Events = new()
                                    {
                                        Click = _ =>
                                        {
                                            _codeVisible = !_codeVisible;
                                            var code = UISystem.Manager?.GetElementById("base-btn-codePanel") as UIView;
                                            code?.SetClassNames(new List<string> { "code-panel" });
                                            code?.SetStyle( new DefaultUIStyle
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
                    Id = "base-btn-codePanel",
                    ClassName = new List<string> { "code-panel" },
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

                Children = new() { buttonRow, toolbar, _codePanel };
            }
        }

        class IconButton : UIView
        {
            private const string CSharpCode = "new UIView\n{\n    ClassName = new List<string> { \"btn-box\" },\n    Children = new()\n    {\n        new UIButton\n        {\n            Text = \"\\ue612\",\n            ClassName = new List<string> { \"btn\", \"btn-icon-button\" },\n            Style = new DefaultUIStyle\n            {\n                FontFamily = UIFontManager.Get(\"TCYMIconFont\"),\n                FontSize = 16,\n            }\n        },\n        new UIButton\n        {\n            Text = \"搜索\",\n            ClassName = new List<string> { \"btn\", \"btn-icon-search\" },\n            Children = new()\n            {\n                new UIIcon\n                {\n                    Content = \"&#xe63f;\",\n                    ClassName = new List<string> { \"btn-search-icon\" },\n                }\n            }\n        },\n        new UIButton\n        {\n            Text = \"加载中...\",\n            ClassName = new List<string> { \"btn\", \"btn-icon-loading\" },\n            Children = new()\n            {\n                new UIIcon\n                {\n                    Content = \"&#xec72;\",\n                    ClassName = new List<string> { \"btn-loading-icon\" },\n                }\n            }\n        }\n    }\n};";

            private const string CssCode = ".btn-icon-button {\n    border-radius: 50%;\n    width: 30px;\n    height: 30px;\n    background: #1677FF;\n    color: #FFFFFF;\n}\n\n.btn-icon-button:hover {\n    color: #ff0000;\n}\n\n.btn-icon-search {\n    padding: 5px 15px;\n    border-radius: 50%;\n    background: #1677FF;\n    color: #FFFFFF;\n    padding-left: 25px;\n    position: relative;\n}\n\n.btn-search-icon {\n    pointer-events: none;\n    font-family: \"TCYMIconFont\";\n    position: absolute;\n    left: -18px;\n    top: 50%;\n    font-size: 18px;\n    color: #FFFFFF;\n    transform: translateY(-50%);\n}\n\n.btn-icon-loading {\n    padding: 5px 15px;\n    background: #1677FF;\n    color: #FFFFFF;\n    padding-left: 25px;\n    position: relative;\n}\n\n.btn-loading-icon {\n    pointer-events: none;\n    font-family: \"TCYMIconFont\";\n    position: absolute;\n    left: -18px;\n    top: 50%;\n    font-size: 18px;\n    color: #FFFFFF;\n    transform: translateY(-50%);\n    animation: spin 1s linear infinite;\n}\n\n@keyframes spin {\n    0% { transform: rotate(0deg); }\n    100% { transform: rotate(360deg); }\n}";

            private bool _codeVisible = false;
            private UIView _codePanel;

            internal IconButton()
            {
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                };
                var buttonRow = new UIView
                {
                    ClassName = new List<string> { "btn-box" },
                    Children = new()
                    {
                        new UIButton
                        {
                            Text = "\ue612",
                            ClassName = new List<string> { "btn", "btn-icon-button" },
                            Style = new DefaultUIStyle
                            {
                                FontFamily = UIFontManager.Get("TCYMIconFont"),
                                FontSize = 16,
                            }
                        },
                        new UIButton
                        {
                            Text = "搜索",
                            ClassName = new List<string> { "btn", "btn-icon-search" },
                            Children = new()
                            {
                                new UIIcon
                                {
                                    Content = "&#xe63f;",
                                    ClassName = new List<string> { "btn-search-icon" },
                                }
                            }
                        },
                        new UIButton
                        {
                            Text = "加载中...",
                            ClassName = new List<string> { "btn", "btn-icon-loading" },
                            Children = new()
                            {
                                new UIIcon
                                {
                                    Content = "&#xec72;",
                                    ClassName = new List<string> { "btn-loading-icon" },
                                }
                            }
                        }
                    }
                };

                var toolbar = new UIView
                {
                    ClassName = new List<string> { "code-toolbar" },
                    Children = new()
                    {
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
                                        Cursor = Enums.UICursor.Pointer,
                                    },
                                    Events = new()
                                    {
                                        Click = _ =>
                                        {
                                            _codeVisible = !_codeVisible;
                                            var code = UISystem.Manager?.GetElementById("icon-btn-codePanel") as UIView;
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
                    Id = "icon-btn-codePanel",
                    ClassName = new List<string> { "code-panel" },
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

                Children = new() { buttonRow, toolbar, _codePanel };
            }
        }

        class DisableButton : UIView
        {
            private const string CSharpCode = "new UIView\n{\n    ClassName = new List<string> { \"btn-box\" },\n    Children = new()\n    {\n        new UIButton\n        {\n            Text = \"Primary Button\",\n            ClassName = new List<string> { \"btn\", \"btn-primary\" },\n            Disabled = true,\n        },\n        new UIButton\n        {\n            Text = \"Default Button\",\n            ClassName = new List<string> { \"btn\", \"btn-default\" },\n            Disabled = true,\n        },\n        new UIButton\n        {\n            Text = \"Dashed Button\",\n            ClassName = new List<string> { \"btn\", \"btn-dashed\" },\n            Disabled = true,\n        },\n        new UIButton\n        {\n            Text = \"Text Button\",\n            ClassName = new List<string> { \"btn\", \"btn-text\" },\n            Disabled = true,\n        },\n        new UIButton\n        {\n            Text = \"Link Button\",\n            ClassName = new List<string> { \"btn\", \"btn-link\" },\n            Disabled = true,\n        },\n    }\n};";

            private const string CssCode = ".btn-box {\n    width: 100%;\n    padding: 5px;\n    display: flex;\n    gap: 5px;\n}\n\n.btn {\n    padding: 5px;\n    border-radius: 6px;\n    cursor: pointer;\n}\n\n.btn-primary {\n    color: #FFFFFF;\n    background: #1677FF;\n}\n\n.btn-default {\n    color: #000000;\n    background: #FFFFFF;\n}\n\n.btn-dashed {\n    color: #000000;\n    border: 1px dashed rgba(0,0,0,0.2);\n}\n\n.btn-text {\n    color: #000000;\n}\n\n.btn-link {\n    color: #1677FF;\n}";

            private bool _codeVisible = false;
            private UIView _codePanel;

            internal DisableButton()
            {
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                };
                var buttonRow = new UIView
                {
                    ClassName = new List<string> { "btn-box" },
                    Children = new()
                    {
                        new UIButton
                        {
                            Text = "Primary Button",
                            ClassName = new List<string> { "btn", "btn-primary" },
                            Disabled = true,
                        },
                        new UIButton
                        {
                            Text = "Default Button",
                            ClassName = new List<string> { "btn", "btn-default" },
                            Disabled = true,
                        },
                        new UIButton
                        {
                            Text = "Dashed Button",
                            ClassName = new List<string> { "btn", "btn-dashed" },
                            Disabled = true,
                        },
                        new UIButton
                        {
                            Text = "Text Button",
                            ClassName = new List<string> { "btn", "btn-text" },
                            Disabled = true,
                        },
                        new UIButton
                        {
                            Text = "Link Button",
                            ClassName = new List<string> { "btn", "btn-link" },
                            Disabled = true,
                        },
                    }
                };

                var toolbar = new UIView
                {
                    ClassName = new List<string> { "code-toolbar" },
                    Children = new()
                    {
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
                                        Cursor = Enums.UICursor.Pointer,
                                    },
                                    Events = new()
                                    {
                                        Click = _ =>
                                        {
                                            _codeVisible = !_codeVisible;
                                            var code = UISystem.Manager?.GetElementById("disable-btn-codePanel") as UIView;
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
                    Id = "disable-btn-codePanel",
                    ClassName = new List<string> { "code-panel" },
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

                Children = new() { buttonRow, toolbar, _codePanel };
            }
        }
    }
}
