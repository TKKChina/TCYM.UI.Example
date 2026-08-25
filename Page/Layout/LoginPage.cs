using TCYM.UI.Binding;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Input;
using TCYM.UI.Elements.Message;

namespace TCYM.UI.Example.Page.Layout
{
    /// <summary>
    /// 登录演示页。验证通过后由 AppShell 切换到主布局。
    /// </summary>
    internal sealed class LoginPage : UIView
    {
        private readonly UIInput _usernameInput;
        private readonly UIInput _passwordInput;

        internal LoginPage()
        {
            UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.Layout.login.css");

            ClassName = "login-page";
            _usernameInput = new UIInput
            {
                Text = "admin",
                Prefix = new UIIcon
                {
                    Style = new DefaultUIStyle
                    {
                        FontFamily = UIFontManager.Get("IconFontExample"),
                        FontSize = 16,
                    },
                    Content = "&#xe604;"
                },
                Placeholder = "请输入账号",
                AllowClear = true,
                ClassName = "login-input",
                IconFontSize = 16,
            };
            _passwordInput = new UIInput
            {
                Text = "123456",
                Prefix = new UIIcon
                {
                    Style = new DefaultUIStyle
                    {
                        FontFamily = UIFontManager.Get("IconFontExample"),
                        FontSize = 16,
                    },
                    Content = "&#xe636;"
                },
                Type = UIInputType.Password,
                Placeholder = "请输入密码",
                PasswordVisibilityToggle = true,
                ClassName = "login-input",
                IconFontSize = 16,
            };

            Children =
            [
                new UIView
                {
                    ClassName = "login-card",
                    Children =
                    [
                        CreateBrandPanel(),
                        CreateFormPanel()
                    ]
                }
            ];
        }

        private UIView CreateBrandPanel()
        {
            return new UIView
            {
                ClassName = "login-brand",
                Children =
                [
                    new UIView { ClassName = "login-orb login-orb-one" },
                    new UIView { ClassName = "login-orb login-orb-two" },
                    new UIView
                    {
                        ClassName = "login-brand-content",
                        Children =
                        [
                            new UIView
                            {
                                ClassName = "login-brand-badge",
                                Children =
                                [
                                    new UILabel { Text = "◆", ClassName = "login-brand-mark" },
                                    new UILabel { Text = "TCYM DESKTOP UI", ClassName = "login-brand-kicker" }
                                ]
                            },
                            new UILabel
                            {
                                Text = "让桌面应用界面\n更清晰，也更高效",
                                ClassName = "login-brand-title"
                            },
                            new UILabel
                            {
                                Text = "一套面向现代桌面应用的跨平台 UI 框架，兼顾开发体验与原生性能。",
                                ClassName = "login-brand-description"
                            },
                            new UIView
                            {
                                ClassName = "login-feature-list",
                                Children =
                                [
                                    CreateFeature("声明式路由", "清晰组织登录页、主布局与业务页面"),
                                    CreateFeature("丰富组件", "常用交互组件开箱即用"),
                                    CreateFeature("跨平台渲染", "同一套代码覆盖多个桌面平台")
                                ]
                            },
                            new UIView { ClassName = "login-brand-spacer" },
                            new UIView
                            {
                                ClassName = "login-brand-footer",
                                Children =
                                [
                                    new UILabel { Text = "●", ClassName = "login-status-dot" },
                                    new UILabel { Text = "TCYM.UI Example is ready", ClassName = "login-brand-footer-text" }
                                ]
                            }
                        ]
                    }
                ]
            };
        }

        private UIView CreateFormPanel()
        {
            return new UIView
            {
                ClassName = "login-form-panel",
                Children =
                [
                    new UILabel { Text = "WELCOME BACK", ClassName = "login-form-kicker" },
                    new UILabel { Text = "欢迎回来", ClassName = "login-title" },
                    new UILabel
                    {
                        Text = "登录后进入 TCYM.UI 组件示例",
                        ClassName = "login-subtitle"
                    },
                    new UIView
                    {
                        ClassName = "login-fields",
                        Children =
                        [
                            new UIView
                            {
                                ClassName = "login-field",
                                Children =
                                [
                                    new UILabel { Text = "账号", ClassName = "login-label" },
                                    _usernameInput
                                ]
                            },
                            new UIView
                            {
                                ClassName = "login-field",
                                Children =
                                [
                                    new UILabel { Text = "密码", ClassName = "login-label" },
                                    _passwordInput
                                ]
                            }
                        ]
                    },
                    new UIView
                    {
                        ClassName = "login-demo-account",
                        Children =
                        [
                            new UILabel { Text = "演示账号", ClassName = "login-demo-label" },
                            new UILabel { Text = "admin  /  123456", ClassName = "login-demo-value" }
                        ]
                    },
                    new UIButton
                    {
                        Text = "登录并进入主界面  →",
                        ClassName = "login-button",
                        Events = new UIEventBindings
                        {
                            Click = _ => Login()
                        }
                    },
                    new UILabel
                    {
                        Text = "登录成功后才会创建 Menu 和业务页面",
                        ClassName = "login-security-note"
                    }
                ]
            };
        }

        private static UIView CreateFeature(string title, string description)
        {
            return new UIView
            {
                ClassName = "login-feature",
                Children =
                [
                    new UILabel { Text = "✓", ClassName = "login-feature-icon" },
                    new UIView
                    {
                        ClassName = "login-feature-copy",
                        Children =
                        [
                            new UILabel { Text = title, ClassName = "login-feature-title" },
                            new UILabel { Text = description, ClassName = "login-feature-description" }
                        ]
                    }
                ]
            };
        }

        private void Login()
        {
            var username = _usernameInput.Text;
            var password = _passwordInput.Text;
            // 仅用于演示。实际项目应在这里调用登录服务。
            if (!string.Equals(username.Trim(), "admin", StringComparison.OrdinalIgnoreCase)
                || password != "123456")
            {
                UIMessage.Warning("账号或密码错误，请使用 admin / 123456");
                return;
            }

            UISystem.Manager?.GetElementById<UICaptionBar>("demo-caption-bar")?.RemoveClass("login-caption-bar");
            UIMessage.Success("登录成功");
            UIRouterNavigator.Navigate("/app/demo", replace: true);
            UISystem.SetWindowSize(1620, 800);
        }
    }
}
