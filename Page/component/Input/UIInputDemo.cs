using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Input;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Input
{
    internal class UIInputDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Input.style.css";

        internal UIInputDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "input-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Input 输入框",
                    ClassName = new List<string> { "input-demo-title" },
                },
                new UILabel
                {
                    Text = "通过鼠标或键盘输入内容，是最基础的表单域的包装。",
                    ClassName = new List<string> { "input-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "需要用户输入表单域内容时使用。提供组合型输入框，带搜索的输入框，还可以进行大小选择。",
                    ClassName = new List<string> { "input-demo-desc" },
                },
                new BasicSection(),
                new TypeSection(),
                new SizeSection(),
                new DisabledReadOnlySection(),
                new PrefixSuffixSection(),
                new AllowClearSection(),
                new PasswordSection(),
                new CountSection(),
                new ValidationSection(),
                new CustomColorSection(),
                new TextAreaSection(),
                new TextAreaAutoSizeSection(),
                new EventSection(),
            };
        }

        /// <summary>
        /// 基本用法
        /// </summary>
        private class BasicSection : UIView
        {
            internal BasicSection()
            {
                ClassName = new List<string> { "input-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "基本用法",
                        ClassName = new List<string> { "input-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "基本使用方式。通过 Placeholder 设置占位文本，通过 Text 设置默认值。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase" },
                        Children = new()
                        {
                            new UIInput
                            {
                                Placeholder = "请输入内容",
                            },
                            new UIInput
                            {
                                Text = "已有默认内容",
                            },
                            new UIInput
                            {
                                Placeholder = "带自定义占位颜色",
                                PlaceholderColor = ColorHelper.ParseColor("#bfbfbf"),
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 输入框类型
        /// </summary>
        private class TypeSection : UIView
        {
            internal TypeSection()
            {
                ClassName = new List<string> { "input-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "输入框类型",
                        ClassName = new List<string> { "input-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "通过 Type 属性设置输入框类型，支持 Text、Password、Number、Email、Search。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase" },
                        Children = new()
                        {
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "Text：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Type = UIInputType.Text,
                                        Placeholder = "普通文本输入",
                                    },
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "Password：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Type = UIInputType.Password,
                                        Placeholder = "密码输入",
                                    },
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "Number：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Type = UIInputType.Number,
                                        Placeholder = "仅限数字输入",
                                    },
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "Email：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Type = UIInputType.Email,
                                        Placeholder = "请输入邮箱",
                                    },
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "Search：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Type = UIInputType.Search,
                                        Placeholder = "搜索...",
                                    },
                                }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 不同尺寸
        /// </summary>
        private class SizeSection : UIView
        {
            internal SizeSection()
            {
                ClassName = new List<string> { "input-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "不同尺寸",
                        ClassName = new List<string> { "input-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "通过 Style 调整输入框的宽度、高度和字体大小。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase" },
                        Children = new()
                        {
                            new UIInput
                            {
                                Placeholder = "小尺寸",
                                Style = new UpdateUIStyle
                                {
                                    Width = 160,
                                    Height = 24,
                                    FontSize = 12,
                                },
                            },
                            new UIInput
                            {
                                Placeholder = "默认尺寸 (200×30)",
                            },
                            new UIInput
                            {
                                Placeholder = "大尺寸",
                                Style = new UpdateUIStyle
                                {
                                    Width = 300,
                                    Height = 40,
                                    FontSize = 18,
                                },
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 禁用和只读
        /// </summary>
        private class DisabledReadOnlySection : UIView
        {
            internal DisabledReadOnlySection()
            {
                ClassName = new List<string> { "input-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "禁用与只读",
                        ClassName = new List<string> { "input-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "Disabled 禁用后不可交互也不可获取焦点；ReadOnly 只读时允许聚焦、选择和复制，但不能编辑。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase" },
                        Children = new()
                        {
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "禁用：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Text = "禁用状态的输入框",
                                        Disabled = true,
                                    },
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "只读：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Text = "只读状态，可选中复制",
                                        ReadOnly = true,
                                    },
                                }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 前缀和后缀
        /// </summary>
        private class PrefixSuffixSection : UIView
        {
            internal PrefixSuffixSection()
            {
                ClassName = new List<string> { "input-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "前缀与后缀",
                        ClassName = new List<string> { "input-card-title", "label-purple" }
                    },
                    new UILabel
                    {
                        Text = "通过 Prefix 和 Suffix 在输入框内部添加前缀/后缀元素，如图标或文字标签。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase" },
                        Children = new()
                        {
                            new UIInput
                            {
                                Placeholder = "请输入金额",
                                Prefix = new UILabel
                                {
                                    Text = "￥",
                                    Style = new DefaultUIStyle
                                    {
                                        Height = "100%",
                                        Display = "flex",
                                        AlignItems = "center",
                                        JustifyContent = "center",
                                        FontSize = 14,
                                        Color = ColorHelper.ParseColor("rgba(0,0,0,0.45)"),
                                    }
                                },
                            },
                            new UIInput
                            {
                                Placeholder = "请输入网址",
                                Prefix = new UILabel
                                {
                                    Text = "https://",
                                    Style = new DefaultUIStyle
                                    {
                                        Height = "100%",
                                        Display = "flex",
                                        AlignItems = "center",
                                        JustifyContent = "center",
                                        FontSize = 13,
                                        Color = ColorHelper.ParseColor("rgba(0,0,0,0.45)"),
                                    }
                                },
                                Suffix = new UILabel
                                {
                                    Text = ".com",
                                    Style = new DefaultUIStyle
                                    {
                                        Height = "100%",
                                        Display = "flex",
                                        AlignItems = "center",
                                        JustifyContent = "center",
                                        FontSize = 13,
                                        Color = ColorHelper.ParseColor("rgba(0,0,0,0.45)"),
                                    }
                                },
                            },
                            new UIInput
                            {
                                Placeholder = "搜索图标前缀",
                                Prefix = new UIIcon
                                {
                                    Content = "&#xe63f;",
                                    Style = new DefaultUIStyle
                                    {
                                        Width = 20,
                                         Height = "100%",
                                         Display = "flex",
                                         AlignItems = "center",
                                         JustifyContent = "center",
                                        FontSize = 16,
                                        Color = ColorHelper.ParseColor("rgba(0,0,0,0.45)"),
                                        FontFamily = UIFontManager.Get("TCYMIconFont"),
                                    }
                                },
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 可清除
        /// </summary>
        private class AllowClearSection : UIView
        {
            internal AllowClearSection()
            {
                ClassName = new List<string> { "input-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "可清除",
                        ClassName = new List<string> { "input-card-title", "label-cany" }
                    },
                    new UILabel
                    {
                        Text = "设置 AllowClear 后，当输入框有内容时会显示清除图标，点击即可清空。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase" },
                        Children = new()
                        {
                            new UIInput
                            {
                                Placeholder = "输入内容后可清除",
                                AllowClear = true,
                            },
                            new UIInput
                            {
                                Text = "已有内容，点 × 清除",
                                AllowClear = true,
                            },
                            new UIInput
                            {
                                Text = "自定义清除图标颜色",
                                AllowClear = true,
                                ClearIconColor = ColorHelper.ParseColor("#ff4d4f"),
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 密码输入
        /// </summary>
        private class PasswordSection : UIView
        {
            internal PasswordSection()
            {
                ClassName = new List<string> { "input-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "密码输入",
                        ClassName = new List<string> { "input-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "Type=Password 时自动显示密码掩码。PasswordVisibilityToggle 控制是否显示切换图标，PasswordVisible 控制明文/密文状态。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase" },
                        Children = new()
                        {
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "带切换：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Type = UIInputType.Password,
                                        Placeholder = "请输入密码",
                                        PasswordVisibilityToggle = true,
                                    },
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "无切换：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Type = UIInputType.Password,
                                        Placeholder = "隐藏切换图标",
                                        PasswordVisibilityToggle = false,
                                    },
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "默认明文：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Type = UIInputType.Password,
                                        Text = "visible123",
                                        PasswordVisible = true,
                                        PasswordVisibilityToggle = true,
                                    },
                                }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 字符计数
        /// </summary>
        private class CountSection : UIView
        {
            internal CountSection()
            {
                ClassName = new List<string> { "input-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "字符计数",
                        ClassName = new List<string> { "input-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "通过 Count 配置显示字符计数。可设置 Max 最大值、Strategy 自定义计数策略、ExceedFormatter 超出截断逻辑。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase" },
                        Children = new()
                        {
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "基本计数：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Placeholder = "最多输入 20 个字符",
                                        Count = new CountConfig
                                        {
                                            Show = true,
                                            Max = 20,
                                        },
                                    },
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "超出截断：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Placeholder = "超过 10 字符自动截断",
                                        Count = new CountConfig
                                        {
                                            Show = true,
                                            Max = 10,
                                            ExceedFormatter = (text, max) => text.Substring(0, max),
                                        },
                                    },
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "仅显示计数：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Placeholder = "只显示已输入字数",
                                        Count = new CountConfig
                                        {
                                            Show = true,
                                        },
                                    },
                                }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 长度校验
        /// </summary>
        private class ValidationSection : UIView
        {
            internal ValidationSection()
            {
                ClassName = new List<string> { "input-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "长度校验",
                        ClassName = new List<string> { "input-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "通过 MinLength 和 MaxLength 设置长度限制。MinLength 校验失败时失焦后边框变红，MaxLength 超出时自动截断。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase" },
                        Children = new()
                        {
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "MinLength=5：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Placeholder = "至少 5 个字符",
                                        MinLength = 5,
                                    },
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "MaxLength=10：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Placeholder = "最多 10 个字符",
                                        MaxLength = 10,
                                    },
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "Min=3 Max=15：", ClassName = new List<string> { "input-type-label" } },
                                    new UIInput
                                    {
                                        Placeholder = "3 ~ 15 个字符",
                                        MinLength = 3,
                                        MaxLength = 15,
                                        ValidationErrorBorderColor = ColorHelper.ParseColor("#ff7875"),
                                    },
                                }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 自定义颜色
        /// </summary>
        private class CustomColorSection : UIView
        {
            internal CustomColorSection()
            {
                ClassName = new List<string> { "input-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义颜色",
                        ClassName = new List<string> { "input-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "通过各种颜色属性自定义输入框外观：边框颜色、聚焦边框颜色、背景颜色、文本颜色、光标颜色等。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase" },
                        Children = new()
                        {
                            new UIInput
                            {
                                Text = "自定义边框颜色",
                                InputBorderColor = ColorHelper.ParseColor("#52c41a"),
                                InputFocusBorderColor = ColorHelper.ParseColor("#73d13d"),
                            },
                            new UIInput
                            {
                                Text = "深色背景模式",
                                InputBackground = ColorHelper.ParseColor("#1f1f1f"),
                                InputTextColor = SKColors.White,
                                InputBorderColor = ColorHelper.ParseColor("#434343"),
                                InputFocusBorderColor = ColorHelper.ParseColor("#177ddc"),
                                CursorColor = SKColors.White,
                                PlaceholderColor = ColorHelper.ParseColor("rgba(255,255,255,0.35)"),
                            },
                            new UIInput
                            {
                                Text = "粉色主题",
                                InputBorderColor = ColorHelper.ParseColor("#eb2f96"),
                                InputFocusBorderColor = ColorHelper.ParseColor("#f759ab"),
                                CursorColor = ColorHelper.ParseColor("#eb2f96"),
                                SelectionBackgroundColor = ColorHelper.ParseColor("rgba(235,47,150,0.2)"),
                            },
                            new UIInput
                            {
                                Placeholder = "仅显示底部边框",
                                InputBorderTop = false,
                                InputBorderLeft = false,
                                InputBorderRight = false,
                                InputBorderBottom = true,
                                InputBorderBottomColor = ColorHelper.ParseColor("#1890ff"),
                                Style = new UpdateUIStyle
                                {
                                    BorderRadius = 0,
                                },
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 多行文本框
        /// </summary>
        private class TextAreaSection : UIView
        {
            internal TextAreaSection()
            {
                ClassName = new List<string> { "input-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "多行文本框 TextArea",
                        ClassName = new List<string> { "input-card-title", "label-purple" }
                    },
                    new UILabel
                    {
                        Text = "UITextArea 用于多行文本输入，通过 Rows 设置可见行数，支持 AllowClear、Count 等与 UIInput 一致的功能。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase" },
                        Children = new()
                        {
                            new UITextArea
                            {
                                Placeholder = "请输入多行内容...",
                                Rows = 3,
                            },
                            new UITextArea
                            {
                                Placeholder = "带字符计数的文本框",
                                Rows = 4,
                                Count = new CountConfig
                                {
                                    Show = true,
                                    Max = 200,
                                },
                            },
                            new UITextArea
                            {
                                Text = "禁用状态的多行文本框",
                                Rows = 3,
                                Disabled = true,
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 自适应高度
        /// </summary>
        private class TextAreaAutoSizeSection : UIView
        {
            internal TextAreaAutoSizeSection()
            {
                ClassName = new List<string> { "input-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自适应高度",
                        ClassName = new List<string> { "input-card-title", "label-cany" }
                    },
                    new UILabel
                    {
                        Text = "设置 AutoSize 后，UITextArea 的高度会随内容自动增长（但不小于 Rows 计算的最小高度）。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase" },
                        Children = new()
                        {
                            new UITextArea
                            {
                                Placeholder = "自适应高度，试试输入多行文字",
                                AutoSize = true,
                                Rows = 2,
                                Style = new UpdateUIStyle
                                {
                                    Width = 300,
                                },
                            },
                            new UITextArea
                            {
                                Placeholder = "自适应 + 最大字数",
                                AutoSize = true,
                                Rows = 2,
                                AllowClear = true,
                                Count = new CountConfig
                                {
                                    Show = true,
                                    Max = 100,
                                },
                                Style = new UpdateUIStyle
                                {
                                    Width = 300,
                                },
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 事件回调
        /// </summary>
        private class EventSection : UIView
        {
            internal EventSection()
            {
                ClassName = new List<string> { "input-demo-card" };

                var valueLabel = new UILabel
                {
                    Text = "输入值：(未输入)",
                    ClassName = new List<string> { "input-event-label" }
                };
                var focusLabel = new UILabel
                {
                    Text = "焦点状态：未聚焦",
                    ClassName = new List<string> { "input-event-label" }
                };
                var passwordLabel = new UILabel
                {
                    Text = "密码可见：false",
                    ClassName = new List<string> { "input-event-label" }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "事件回调",
                        ClassName = new List<string> { "input-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "通过 OnValuesChange 监听内容变化，OnFocus / OnBlur 监听焦点变化，OnPasswordVisibleChange 监听密码可见性切换。",
                        ClassName = new List<string> { "input-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "input-showcase", "input-showcase-column" },
                        Children = new()
                        {
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "输入监听：", ClassName = new List<string> { "input-type-label" } },
                                    CreateValueInput(valueLabel, focusLabel),
                                }
                            },
                            new UIView
                            {
                                ClassName = new List<string> { "input-showcase-row" },
                                Children = new()
                                {
                                    new UILabel { Text = "密码监听：", ClassName = new List<string> { "input-type-label" } },
                                    CreatePasswordInput(passwordLabel),
                                }
                            },
                            valueLabel,
                            focusLabel,
                            passwordLabel,
                        }
                    },
                };
            }

            private static UIInput CreateValueInput(UILabel valueLabel, UILabel focusLabel)
            {
                var input = new UIInput { Placeholder = "输入内容观察变化" };
                input.OnValuesChange += text => { valueLabel.Text = $"输入值：{text}"; };
                input.OnFocus += () => { focusLabel.Text = "焦点状态：已聚焦"; };
                input.OnBlur += () => { focusLabel.Text = "焦点状态：已失焦"; };
                return input;
            }

            private static UIInput CreatePasswordInput(UILabel passwordLabel)
            {
                var input = new UIInput
                {
                    Type = UIInputType.Password,
                    Placeholder = "点击眼睛图标切换",
                };
                input.OnPasswordVisibleChange += visible => { passwordLabel.Text = $"密码可见：{visible}"; };
                return input;
            }
        }
    }
}
