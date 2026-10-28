using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Autocomplete;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Autocomplete
{
    /// <summary>自动完成框示例：保留自由输入，并在输入时提供可选择的建议。</summary>
    internal class UIAutocompleteDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Autocomplete.style.css";

        internal UIAutocompleteDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = "autocomplete-demo-view";
            Children = new()
            {
                new UILabel { Text = "Autocomplete 自动完成框", ClassName = "autocomplete-demo-title" },
                new UILabel
                {
                    Text = "输入时显示建议，支持自由填写、键盘选择和动态搜索。",
                    ClassName = "autocomplete-demo-title-sub",
                },
                new UILabel
                {
                    Text = "输入框沿用 Input 的样式与编辑行为，建议面板保持与 Select 一致的视觉风格。使用 ↑ / ↓ 切换建议，Enter 选择，Esc 关闭。",
                    ClassName = "autocomplete-demo-desc",
                },
                CreateBasicSection(),
                CreateEmailSection(),
                CreateCustomSection(),
                new AsyncSearchSection(),
                CreateSizeSection(),
                CreateStateSection(),
                CreateEmptySection(),
                CreateEventSection(),
            };
        }

        private static List<AutocompleteOption> CreateLanguageOptions() => new()
        {
            "C#", "C++", "Java", "JavaScript", "TypeScript", "Python", "Rust",
            new("legacy", "旧版本（不可选择）") { Disabled = true },
        };

        /// <summary>用一致的卡片结构承载各个示例。</summary>
        private static UIView Card(string title, string description, params UIElement[] examples) => new()
        {
            ClassName = "autocomplete-demo-card",
            Children = new()
            {
                new UILabel { Text = title, ClassName = "autocomplete-card-title" },
                new UILabel { Text = description, ClassName = "autocomplete-card-desc" },
                new UIView
                {
                    ClassName = "autocomplete-showcase",
                    Children = examples.ToList(),
                },
            },
        };

        private static UIView CreateBasicSection() => Card(
            "基本用法与过滤",
            "Options 支持字符串或 AutocompleteOption。默认按输入内容筛选建议，仍可填写列表中不存在的内容；第二个输入框演示自定义前缀匹配。",
            new UIAutocomplete
            {
                Placeholder = "输入语言名称，如 Java",
                Options = CreateLanguageOptions(),
                AllowClear = true,
                Style = new UpdateUIStyle { Width = 300 },
            },
            new UIAutocomplete
            {
                Placeholder = "前缀匹配，输入 Type",
                Options = CreateLanguageOptions(),
                FilterOption = (text, option) => option.Label.StartsWith(text, StringComparison.OrdinalIgnoreCase),
                DefaultActiveFirstOption = true,
                Style = new UpdateUIStyle { Width = 300 },
            });

        /// <summary>监听用户输入，组合常用邮箱后缀；候选已由业务生成，因此关闭内置过滤。</summary>
        private static UIView CreateEmailSection()
        {
            var email = new UIAutocomplete
            {
                Placeholder = "输入邮箱账号，例如 TCYM",
                Filter = false,
                AllowClear = true,
                Style = new UpdateUIStyle { Width = 320 },
                OnSearch = (sender, text) =>
                {
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        sender.Options = new();
                        return;
                    }

                    int separator = text.IndexOf('@');
                    string account = separator < 0 ? text : text[..separator];
                    sender.Options = new[] { "qq.com", "163.com", "outlook.com", "gmail.com" }
                        .Select(domain => $"{account}@{domain}")
                        .Where(address => address.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                        .Select(address => new AutocompleteOption(address))
                        .ToList();
                },
            };

            return Card("动态邮箱建议", "回调首参 sender 是当前组件，可通过 OnSearch 根据用户输入更新 Options，选择后会填入完整邮箱。", email);
        }

        private static UIView CreateCustomSection()
        {
            var result = new UILabel { Text = "尚未选择用户", ClassName = "autocomplete-status" };
            var user = new UIAutocomplete
            {
                Placeholder = "按姓名或账号搜索用户",
                Options = new()
                {
                    new("zhangsan", "张三") { Tag = "研发部" },
                    new("lisi", "李四") { Tag = "设计部" },
                    new("wangwu", "王五") { Tag = "产品部" },
                },
                OptionHeight = 48,
                Style = new UpdateUIStyle { Width = 320 },
                FilterOption = (text, option) =>
                    option.Value.Contains(text, StringComparison.OrdinalIgnoreCase) || option.Label.Contains(text),
                // 候选内容可包含多行，选择时只将姓名填回输入框。
                OptionRender = (option, _) => new UIView
                {
                    ClassName = "autocomplete-user-option",
                    Children = new()
                    {
                        new UILabel { Text = option.Label, ClassName = "autocomplete-user-name" },
                        new UILabel { Text = $"{option.Value} · {option.Tag}", ClassName = "autocomplete-user-detail" },
                    },
                },
                SelectedTextRender = option => option.Label,
                OnSelect = (_, option) => result.Text = $"姓名：{option.Label}；账号：{option.Value}；部门：{option.Tag}",

            };

            return Card("自定义候选与回填文本", "通过 OptionRender 展示姓名和部门，通过 SelectedTextRender 控制回填文本；OnSelect 可获取完整业务数据。", user, result);
        }

        /// <summary>异步请求通过版本号屏蔽过期结果，路由离开并释放后不再更新界面。</summary>
        private sealed class AsyncSearchSection : UIView, IDisposable
        {
            private readonly UIAutocomplete _input;
            private readonly UILabel _status;
            private int _searchVersion;
            private volatile bool _disposed;

            internal AsyncSearchSection()
            {
                ClassName = "autocomplete-demo-card";
                _status = new UILabel { Text = "输入关键词开始搜索", ClassName = "autocomplete-status" };
                _input = new UIAutocomplete
                {
                    Placeholder = "模拟远程搜索，如 tcym",
                    Filter = false,
                    AllowClear = true,
                    EmptyText = "请输入关键词",
                    Style = new UpdateUIStyle { Width = 320 },
                    OnSearchAsync = SearchAsync,
                };
                Children = new()
                {
                    new UILabel { Text = "异步搜索", ClassName = "autocomplete-card-title" },
                    new UILabel
                    {
                        Text = "OnSearchAsync 可直接绑定异步方法，Loading 展示加载状态。快速输入时仅接纳最后一次结果。",
                        ClassName = "autocomplete-card-desc",
                    },
                    new UIView { ClassName = "autocomplete-showcase", Children = new() { _input, _status } },
                };
            }

            private async Task SearchAsync(UIAutocomplete sender, string text)
            {
                int version = Interlocked.Increment(ref _searchVersion);
                if (_disposed) return;

                sender.Options = new();
                if (string.IsNullOrWhiteSpace(text))
                {
                    sender.Loading = false;
                    sender.EmptyText = "请输入关键词";
                    _status.Text = "输入关键词开始搜索";
                    return;
                }

                sender.Loading = true;
                _status.Text = $"正在搜索：{text}";
                await Task.Delay(450).ConfigureAwait(false);
                UIDispatcher.Invoke(() =>
                {
                    if (_disposed || version != Volatile.Read(ref _searchVersion)) return;

                    sender.Options = new()
                    {
                        new($"{text} 文档"),
                        new($"{text} 示例"),
                        new($"{text} 教程"),
                    };
                    sender.Loading = false;
                    sender.EmptyText = "暂无搜索结果";
                    _status.Text = $"已返回“{text}”的 3 条建议";
                });
            }

            public void Dispose()
            {
                _disposed = true;
                Interlocked.Increment(ref _searchVersion);
            }
        }

        private static UIView CreateSizeSection() => Card(
            "不同尺寸",
            "与 Input 一样，使用 Style 调整宽度、高度及字体；建议面板与输入框宽度对齐。",
            new UIAutocomplete
            {
                Placeholder = "小尺寸",
                Options = CreateLanguageOptions(),
                Style = new UpdateUIStyle { Width = 200, Height = 24, FontSize = 12 },
            },
            new UIAutocomplete { Placeholder = "默认尺寸", Options = CreateLanguageOptions() },
            new UIAutocomplete
            {
                Placeholder = "大尺寸",
                Options = CreateLanguageOptions(),
                Style = new UpdateUIStyle { Width = 320, Height = 40, FontSize = 18 },
            });

        private static UIView CreateStateSection() => Card(
            "禁用、只读、前后缀与清除",
            "Disabled 和 ReadOnly 沿用 Input 的行为。Prefix / Suffix 可放入文字或图标，AllowClear 开启内容清除。",
            new UIAutocomplete
            {
                Value = "禁用状态",
                Disabled = true,
                Options = CreateLanguageOptions(),
                Style = new UpdateUIStyle { Width = 300 },
            },
            new UIAutocomplete
            {
                Value = "只读状态，可选择复制文本",
                ReadOnly = true,
                Options = CreateLanguageOptions(),
                Style = new UpdateUIStyle { Width = 300 },
            },
            new UIAutocomplete
            {
                Placeholder = "输入文档关键词",
                Value = "TCYM",
                Options = new() { "TCYM", "TCYM.UI", "TCYM.UI.Chart" },
                AllowClear = true,
                // 复用 UIInput 示例的搜索图标，保持前缀尺寸、字体与颜色一致。
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
                Suffix = new UILabel { Text = "文档", ClassName = "autocomplete-affix" },
                Style = new UpdateUIStyle { Width = 320 },
            });

        private static UIView CreateEmptySection() => Card(
            "空状态与弹出方向",
            "EmptyText / EmptyContent 可设置空状态；Placement 指定弹出方向，AutoAdjustOverflow 默认开启以适应窗口边缘。",
            new UIAutocomplete
            {
                Placeholder = "输入不存在的语言，查看空状态",
                Options = CreateLanguageOptions(),
                EmptyText = "未找到匹配语言，可以直接输入",
                Style = new UpdateUIStyle { Width = 320 },
            },
            new UIAutocomplete
            {
                Placeholder = "自定义空内容",
                EmptyContent = new UILabel
                {
                    Text = "暂无建议，输入内容仍会保留",
                    Style = new UpdateUIStyle { Padding = [12], FontSize = 13 },
                },
                Style = new UpdateUIStyle { Width = 320 },
            },
            new UIAutocomplete
            {
                Placeholder = "向上显示候选",
                Options = CreateLanguageOptions(),
                Placement = AutocompletePlacement.Top,
                DropdownMaxHeight = 160,
                Style = new UpdateUIStyle { Width = 320 },
            });

        private static UIView CreateEventSection()
        {
            var change = new UILabel { Text = "OnChange：等待输入", ClassName = "autocomplete-status" };
            var select = new UILabel { Text = "OnSelect：等待选择", ClassName = "autocomplete-status" };
            var open = new UILabel { Text = "OnOpenChange：关闭", ClassName = "autocomplete-status" };
            return Card(
                "输入与选择事件",
                "OnSearch 在用户编辑时触发；OnChange 在编辑和选择时触发；OnSelect 仅在选择建议时触发。",
                new UIAutocomplete
                {
                    Placeholder = "输入或选择一个语言",
                    Options = CreateLanguageOptions(),
                    AllowClear = true,
                    Style = new UpdateUIStyle { Width = 320 },
                    OnChange = (_, text) => change.Text = $"OnChange：{text}",
                    OnSelect = (_, option) => select.Text = $"OnSelect：{option.Label}（{option.Value}）",
                    OnOpenChange = (_, isOpen) => open.Text = $"OnOpenChange：{(isOpen ? "展开" : "关闭")}",
                },
                change,
                select,
                open);
        }
    }
}
