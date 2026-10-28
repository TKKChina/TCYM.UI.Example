using System.Text.Json;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Input;
using TCYM.UI.Elements.Loading;
using TCYM.UI.Events;
using TCYM.UI.WebView;

namespace TCYM.UI.Example.Page.component.WebView;

/// <summary>
/// 原生网页控件示例。路由使用 KeepAlive=false；离页立即取消业务操作、解除通知并关闭浏览器，
/// 返回页面时重新创建实例。默认直接加载程序集内嵌 HTML，也可手动打开在线网页；不创建 HTML 临时文件。
/// </summary>
internal sealed class UIWebViewDemo : UIScrollView, IUIRouteLifecycle, IDisposable
{
    private const int MaximumLogEntries = 40;
    private const int MaximumLogEntryLength = 320;
    // 页面拥有独立取消源；基础元素生命周期终止或路由离开都能使挂起命令失效。
    private readonly CancellationTokenSource _lifetime;
    private readonly CancellationToken _token;
    // 日志只更新单个标签，不为每条网页消息创建新元素。
    private readonly Queue<string> _logLines = new();
    private WebViewOfflinePages? _pages;
    // disposed 为通知和 await continuation 的共同入口门；busy 防止多个工具栏命令并发提交。
    private bool _disposed;
    private bool _busy;
    // 只保存演示选项，加载组件仍声明创建并通过浏览器 Id 设置，不缓存子元素引用。
    private bool _customLoading;
    // 路由挂载后再订阅通知；只记录生命周期状态，不保存子控件引用。
    private bool _eventsAttached;
    // NavigateToString 的地址为 about:blank，必须同时核对显式加载意图、导航编号和实际文档标记。
    // 任何来自在线页、历史或自定义脚本的普通 about:blank 导航均不能自动恢复桥接。
    private bool _trustedDocumentReady;
    private string? _pendingEmbeddedPage;
    private string? _candidateEmbeddedPage;
    private string? _trustedEmbeddedPage;
    private ulong _candidateNavigationId;
    private bool _initializationFailed;
    private ulong _latestNavigationId;

    /// <summary>声明式创建页面；构造阶段只读取资源和预设 HTML，不依赖尚未注册的元素 Id。</summary>
    internal UIWebViewDemo()
    {
        UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.component.WebView.style.css");
        Id = "webview-demo-page";
        ClassName = "webview-demo-view";
        _lifetime = CancellationTokenSource.CreateLinkedTokenSource(LifetimeToken);
        _token = _lifetime.Token;

        string? initialHtml = null;
        string initialStatus = "正在等待原生网页视图初始化……";
        string initialSource = "地址：等待导航";
        string initialRuntime = "运行时：尚未初始化";
        string initialLog = "等待网页事件……";
        if (!UIWebView.IsPlatformSupported)
        {
            _initializationFailed = true;
            initialStatus = UnsupportedPlatformText;
            initialRuntime = "运行时：当前窗口后端暂无可用实现。";
        }
        else
        {
            try
            {
                _pages = new WebViewOfflinePages();
                initialHtml = _pages.GetHtml("home");
                // 浏览器尚未挂载，只登记首屏请求；事件订阅在 OnRouteEnter 中完成。
                _pendingEmbeddedPage = "home";
                initialSource = "地址：内嵌资源 " + WebViewOfflinePages.GetResourceUri("home");
                initialLog = "已读取两份内嵌 HTML，等待加载首页。";
            }
            catch (Exception exception)
            {
                _initializationFailed = true;
                initialStatus = "无法读取内嵌页面：" + exception.Message;
                initialLog = "资源错误：" + Limit(exception.Message, MaximumLogEntryLength);
            }
        }
        Children =
        [
            new UILabel { Id = "webview-demo-title", Text = "WebView 网页视图", ClassName = "webview-demo-title" },
            new UILabel
            {
                Id = "webview-demo-description", ClassName = "webview-demo-description",
                Text = "原生网页视图：内嵌 HTML、在线网页、JavaScript 与字符串 / JSON 双向消息。",
            },
            new UILabel
            {
                Id = "webview-demo-hint", ClassName = "webview-demo-hint",
                Text = "默认直接加载程序集内嵌 HTML，无需联网。也可输入 HTTP / HTTPS 地址浏览在线网页；脚本和消息演示仅对内嵌页面启用。",
            },
            new UIView
            {
                ClassName = "webview-demo-card",
                Children =
                [
                    new UILabel { Id = "webview-demo-navigation-title", Text = "网页导航", ClassName = "webview-demo-section-title" },
                    new UIInput
                    {
                        Id = "webview-demo-url", ClassName = "webview-demo-input", Value = "https://tcym.top:8035/tcym/UI/Doc/index.html",
                        MaxLength = 4096, Disabled = true,
                    },
                    new UIView
                    {
                        ClassName = "webview-demo-toolbar",
                        Children =
                        [
                            new UIButton
                            {
                                Id = "webview-demo-open-url", Text = "打开在线网页", ClassName = "webview-demo-button", Disabled = true,
                                Events = new() { ClickAsync = value => ExecuteButtonAsync(value, OpenUrlAsync) },
                            },
                        ],
                    },
                    new UIView
                    {
                        ClassName = "webview-demo-toolbar",
                        Children =
                        [
                            new UIButton
                            {
                                Id = "webview-demo-home", Text = "回到内嵌首页", ClassName = "webview-demo-button", Disabled = true,
                                Events = new() { ClickAsync = value => ExecuteButtonAsync(value, (_, token) => LoadEmbeddedPageAsync("home", token)) },
                            },
                            new UIButton
                            {
                                Id = "webview-demo-next", Text = "打开内嵌第二页", ClassName = "webview-demo-button", Disabled = true,
                                Events = new() { ClickAsync = value => ExecuteButtonAsync(value, (_, token) => LoadEmbeddedPageAsync("next", token)) },
                            },
                            new UIButton
                            {
                                Id = "webview-demo-back", Text = "后退", ClassName = "webview-demo-button", Disabled = true,
                                Events = new() { ClickAsync = value => ExecuteButtonAsync(value, (_, token) => NavigateHistoryAsync(back: true, token)) },
                            },
                            new UIButton
                            {
                                Id = "webview-demo-forward", Text = "前进", ClassName = "webview-demo-button", Disabled = true,
                                Events = new() { ClickAsync = value => ExecuteButtonAsync(value, (_, token) => NavigateHistoryAsync(back: false, token)) },
                            },
                            new UIButton
                            {
                                Id = "webview-demo-reload", Text = "刷新", ClassName = "webview-demo-button", Disabled = true,
                                Events = new() { ClickAsync = value => ExecuteButtonAsync(value, ReloadAsync) },
                            },
                            new UIButton
                            {
                                Id = "webview-demo-stop", Text = "停止", ClassName = "webview-demo-button", Disabled = true,
                                Events = new() { ClickAsync = value => ExecuteButtonAsync(value, (_, token) => Element<UIWebView>("browser").StopAsync(token)) },
                            },
                        ],
                    },
                    new UILabel
                    {
                        Id = "webview-demo-navigation-hint", ClassName = "webview-demo-description",
                        Text = "内嵌页直接读取 HTML；后退 / 前进使用浏览器真实历史。弹层期间显示网页快照，关闭后恢复原页面。",
                    },
                    new UILabel { Id = "webview-demo-status", Text = initialStatus, ClassName = "webview-demo-status" },
                    new UILabel { Id = "webview-demo-source", Text = initialSource, ClassName = "webview-demo-metadata" },
                    new UILabel { Id = "webview-demo-runtime", Text = initialRuntime, ClassName = "webview-demo-metadata" },
                    new UIView
                    {
                        ClassName = "webview-demo-toolbar",
                        Children =
                        [
                            new UIButton
                            {
                                Id = "webview-demo-loading", Text = "使用自定义加载内容", ClassName = "webview-demo-button",
                                Events = new() { ClickAsync = value => ExecuteButtonAsync(value, ChangeLoadingAsync) },
                            },
                            new UILabel
                            {
                                Text = "默认显示 TomatoNovelLoading；切换后打开在线网页可观察加载提示。",
                                ClassName = "webview-demo-description",
                            },
                        ],
                    },
                    // 原生区域保持矩形和不透明背景；预设资源直接交给 Html，不生成磁盘文件。
                    new UIView
                    {
                        ClassName = "webview-demo-browser-shell",
                        Children =
                        [
                            // Linux Demo 默认使用软件策略改善外层滚动曝光；类库默认仍保留系统策略。
                            new UIWebView(new UIWebViewOptions { UseSoftwareRenderingOnLinux = OperatingSystem.IsLinux() })
                            {
                                Id = "webview-demo-browser",
                                ClassName = "webview-demo-browser",
                                Html = initialHtml,
                                //Source = "https://baidu.com" //--直接打开在线网页，演示脚本和消息功能将被禁用。
                            },
                        ],
                    },
                ],
            },
            new UIView
            {
                ClassName = "webview-demo-card",
                Children =
                [
                    new UILabel { Id = "webview-demo-script-title", Text = "执行 JavaScript", ClassName = "webview-demo-section-title" },
                    new UILabel
                    {
                        Id = "webview-demo-script-hint", ClassName = "webview-demo-description",
                        Text = "内嵌页面可执行脚本，结果保留原生引擎返回的 JSON 文本。打开在线网页或从脚本跳转后，宿主暂停此演示的脚本和消息操作。",
                    },
                    new UITextArea
                    {
                        Id = "webview-demo-script", ClassName = "webview-demo-textarea", Rows = 3, Disabled = true,
                        Value = "({ title: document.title, page: document.documentElement.dataset.page, ready: document.readyState })",
                        MaxLength = 8192,
                    },
                    new UIView
                    {
                        ClassName = "webview-demo-toolbar",
                        Children =
                        [
                            new UIButton
                            {
                                Id = "webview-demo-execute", Text = "执行脚本", ClassName = "webview-demo-button", Disabled = true,
                                Events = new() { ClickAsync = value => ExecuteButtonAsync(value, ExecuteScriptAsync) },
                            },
                        ],
                    },
                    new UILabel { Id = "webview-demo-result", Text = "执行结果将以原始 JSON 显示。", ClassName = "webview-demo-output" },
                ],
            },
            new UIView
            {
                ClassName = "webview-demo-card",
                Children =
                [
                    new UILabel { Id = "webview-demo-message-title", Text = "双向消息", ClassName = "webview-demo-section-title" },
                    new UILabel
                    {
                        Id = "webview-demo-message-hint", ClassName = "webview-demo-description",
                        Text = "宿主发送后，网页会展示收到的内容并回传确认。网页中的两个按钮可主动发送字符串或对象；所有结果显示在下方日志。",
                    },
                    new UIInput
                    {
                        Id = "webview-demo-message", ClassName = "webview-demo-input", Value = "你好，离线网页！",
                        MaxLength = 1024, Disabled = true,
                    },
                    new UIView
                    {
                        ClassName = "webview-demo-toolbar",
                        Children =
                        [
                            new UIButton
                            {
                                Id = "webview-demo-send-string", Text = "宿主发送字符串", ClassName = "webview-demo-button", Disabled = true,
                                Events = new() { ClickAsync = value => ExecuteButtonAsync(value, SendStringAsync) },
                            },
                        ],
                    },
                    new UIInput
                    {
                        Id = "webview-demo-json", ClassName = "webview-demo-input", Value = "{\"type\":\"host-json\",\"value\":42}",
                        MaxLength = 4096, Disabled = true,
                    },
                    new UIView
                    {
                        ClassName = "webview-demo-toolbar",
                        Children =
                        [
                            new UIButton
                            {
                                Id = "webview-demo-send-json", Text = "宿主发送 JSON", ClassName = "webview-demo-button", Disabled = true,
                                Events = new() { ClickAsync = value => ExecuteButtonAsync(value, SendJsonAsync) },
                            },
                        ],
                    },
                    new UIView
                    {
                        ClassName = "webview-demo-toolbar",
                        Children =
                        [
                            new UIButton
                            {
                                Id = "webview-demo-clear-log", Text = "清空日志", ClassName = "webview-demo-button",
                                Events = new() { ClickAsync = value => ExecuteButtonAsync(value, ClearLogAsync) },
                            },
                        ],
                    },
                    new UILabel { Id = "webview-demo-log", Text = initialLog, ClassName = "webview-demo-log" },
                ],
            },
        ];
    }

    /// <summary>页面仍可接收通知或更新界面；不以浏览器就绪替代页面生命周期。</summary>
    private bool IsActive => !_disposed && !_token.IsCancellationRequested;
    /// <summary>桥接可用性只由真实文档状态决定；忙碌门仅防止命令重入，不改变整排控件颜色。</summary>
    private bool CanBridge => HasTrustedDocument();

    /// <summary>使用事件携带的当前按钮；不捕获声明阶段控件引用，且拒绝来自其它页面的同 Id 元素。</summary>
    private Task ExecuteButtonAsync(UIMouseEvent value, Func<UIButton, CancellationToken, Task> action) => value.Element is UIButton sender && OwnsElement(sender) ? RunActionAsync(sender, action) : Task.CompletedTask;

    /// <summary>串行提交按钮操作并观察取消、失败；离页后 finally 也不得继续读写旧控件。</summary>
    private async Task RunActionAsync(UIButton sender, Func<UIButton, CancellationToken, Task> action)
    {
        if (!IsActive || sender.Disabled || _busy) return;
        // 不把短暂命令忙碌映射成全局 Disabled，避免所有按钮变半透明、输入框失焦后再恢复。
        _busy = true;
        try
        {
            await action(sender, _token);
        }
        catch (OperationCanceledException) when (_token.IsCancellationRequested) { }
        catch (ObjectDisposedException) when (!IsActive) { }
        catch (Exception exception)
        {
            if (!IsActive) return;
            Element<UILabel>("status").Text = "操作失败：" + exception.Message;
            AppendLog("操作失败", exception.Message);
        }
        finally
        {
            // 页面已被拥有型释放时不再读写子控件，避免迟到的 continuation 访问新路由。
            if (IsActive)
            {
                _busy = false;
                UpdateControls();
            }
        }
    }

    /// <summary>传入任意 UIElement 作为加载内容；null 恢复默认动画，控件负责释放被替换的内容。</summary>
    private Task ChangeLoadingAsync(UIButton sender, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        _customLoading = !_customLoading;
        Element<UIWebView>("browser").LoadingElement = _customLoading ? new UIView
        {
            Id = "webview-demo-loading-content",
            Style = new DefaultUIStyle
            {
                Width = 260, Height = 120, Display = "flex", FlexDirection = "column",
                AlignItems = "center", JustifyContent = "center", Gap = 8,
            },
            Children =
            [
                new TomatoNovelLoading { SizeLoader = 54, Style = new DefaultUIStyle { Width = 54, Height = 54 } },
                new UILabel { Text = "网页正在加载，请稍候……" },
            ],
        } : null;
        sender.Text = _customLoading ? "恢复默认加载动画" : "使用自定义加载内容";
        return Task.CompletedTask;
    }

    /// <summary>显式加载内嵌页之前关闭旧文档桥接，并登记只允许消费一次的加载意图。</summary>
    private void PrepareEmbeddedRequest(string page)
    {
        ResetDocumentTrust();
        _pendingEmbeddedPage = page;
        Element<UILabel>("source").Text = "地址：内嵌资源 " + WebViewOfflinePages.GetResourceUri(page);
        UpdateControls();
    }

    /// <summary>两份资源已经在内存中，直接调用原生 NavigateToString，无临时目录和本地服务器。</summary>
    private async Task LoadEmbeddedPageAsync(string page, CancellationToken token)
    {
        if (_pages == null) return;
        string html = _pages.GetHtml(page);
        PrepareEmbeddedRequest(page);
        try { await Element<UIWebView>("browser").NavigateToStringAsync(html, token); }
        catch
        {
            // 提交前取消或失败不能留下待消费的 about:blank 许可。
            ResetDocumentTrust();
            throw;
        }
    }

    /// <summary>在线导航只接受明确的 HTTP / HTTPS 绝对地址，输入文本不拼成脚本或文件路径。</summary>
    private async Task OpenUrlAsync(UIButton sender, CancellationToken token)
    {
        string text = Element<UIInput>("url").Value.Trim();
        if (!Uri.TryCreate(text, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            || string.IsNullOrEmpty(uri.Host))
            throw new ArgumentException("请输入完整的 HTTP / HTTPS 网址，例如 https://example.com。");
        ResetDocumentTrust();
        await Element<UIWebView>("browser").NavigateAsync(uri.AbsoluteUri, token);
    }

    /// <summary>使用浏览器真实历史；即使历史项恰好是 about:blank，也不推断它属于本次内嵌加载。</summary>
    private Task NavigateHistoryAsync(bool back, CancellationToken token)
    {
        ResetDocumentTrust();
        return back ? Element<UIWebView>("browser").GoBackAsync(token) : Element<UIWebView>("browser").GoForwardAsync(token);
    }

    /// <summary>
    /// 内嵌页重新提交原始资源，避免原生 Reload 将 NavigateToString 的 about:blank 刷成空页。
    /// 提交导航、收到导航事件及完成 DOM 校验之间都可能接受点击，因此待加载和待确认的资源也走此路径；
    /// 这只决定重新加载哪份固定资源，不为尚未确认的文档开放脚本或消息。
    /// </summary>
    private Task ReloadAsync(UIButton sender, CancellationToken token)
    {
        string? page = _pendingEmbeddedPage ?? _candidateEmbeddedPage ?? _trustedEmbeddedPage;
        if (page != null) return LoadEmbeddedPageAsync(page, token);
        ResetDocumentTrust();
        return Element<UIWebView>("browser").ReloadAsync(token);
    }

    /// <summary>统一撤销当前文档及待提交文档的桥接状态，不影响网页继续正常浏览。</summary>
    private void ResetDocumentTrust()
    {
        _trustedDocumentReady = false;
        _trustedEmbeddedPage = null;
        _pendingEmbeddedPage = null;
        _candidateEmbeddedPage = null;
        _candidateNavigationId = 0;
        UpdateControls();
    }

    /// <summary>只接受 NavigateToString 原生报告的精确空白地址，不接受片段、查询或其它 about 页面。</summary>
    private static bool IsBlankDocument(string? source) => string.Equals(source, "about:blank", StringComparison.OrdinalIgnoreCase);

    /// <summary>执行输入的 JavaScript 并原样展示 JSON 结果；结果返回时再次确认页面和来源仍有效。</summary>
    private async Task ExecuteScriptAsync(UIButton sender, CancellationToken token)
    {
        if (!HasTrustedDocument()) return;
        string result = await Element<UIWebView>("browser").ExecuteScriptAsync(Element<UITextArea>("script").Value, token);
        if (!IsActive || !HasTrustedDocument()) return;
        Element<UILabel>("result").Text = "JSON 结果：\n" + Limit(result, 4096);
        AppendLog(sender.Text, result);
    }

    /// <summary>通过原生字符串消息接口发送输入文本，不把文本拼接成 JavaScript。</summary>
    private async Task SendStringAsync(UIButton sender, CancellationToken token)
    {
        if (!HasTrustedDocument()) return;
        string message = Element<UIInput>("message").Value;
        await Element<UIWebView>("browser").PostWebMessageAsStringAsync(message, token);
        if (IsActive) AppendLog("宿主 → 网页（字符串）", message);
    }

    /// <summary>先解析输入 JSON，再通过原生 JSON 消息接口发送；非法输入进入统一错误提示。</summary>
    private async Task SendJsonAsync(UIButton sender, CancellationToken token)
    {
        if (!HasTrustedDocument()) return;
        string json = Element<UIInput>("json").Value;
        // JsonDocument 仅解析现成文本，不反射序列化匿名对象，因此 NativeAOT 无额外类型元数据要求。
        using var document = JsonDocument.Parse(json);
        await Element<UIWebView>("browser").PostWebMessageAsJsonAsync(document.RootElement.GetRawText(), token);
        if (IsActive) AppendLog("宿主 → 网页（JSON）", json);
    }

    /// <summary>操作执行前再次验证来源；控件的视觉禁用不能替代命令边界的来源检查。</summary>
    private bool HasTrustedDocument() => IsActive
        && FindElement<UIWebView>("browser") is { IsInitialized: true, Disabled: false, IsLoading: false } browser
        && _trustedDocumentReady && _trustedEmbeddedPage != null && IsBlankDocument(browser.Source);

    /// <summary>路由挂载后按 Id 获取浏览器，使用具名处理器订阅可解除的通知。</summary>
    private void AttachBrowserEvents(UIWebView browser)
    {
        browser.Initialized += BrowserInitialized;
        browser.NavigationStarting += BrowserNavigationStarting;
        browser.NavigationCompletedAsync += BrowserNavigationCompletedAsync;
        browser.StateChanged += BrowserStateChanged;
        browser.WebMessageReceivedAsync += BrowserWebMessageReceivedAsync;
        browser.Error += BrowserError;
        _eventsAttached = true;
    }

    /// <summary>显示实际 Runtime 版本；初始化完成不代表首页已经加载完成。</summary>
    private void BrowserInitialized(UIWebView sender)
    {
        if (!IsCurrentBrowser(sender)) return;
        // Windows、X11 与原生 Wayland 共用声明式页面与消息协议，只展示本机真正使用的引擎。
        string engine = OperatingSystem.IsLinux() ? "WebKitGTK" : "WebView2";
        string windowSystem = sender.NativeHostState.WindowContext.Platform switch
        {
            UINativeWindowPlatform.Wayland => "Wayland",
            UINativeWindowPlatform.X11 => "X11",
            UINativeWindowPlatform.Win32 => "Windows",
            _ => "当前窗口",
        };
        Element<UILabel>("runtime").Text = "运行时：" + engine + " " + (sender.RuntimeVersion ?? "已就绪") + "（" + windowSystem + "）";
        AppendLog("初始化", "原生网页视图已就绪。");
        UpdateControls();
    }

    /// <summary>记录最新导航编号并暂停脚本、消息，避免旧文档与新地址交错。</summary>
    private void BrowserNavigationStarting(UIWebView sender, UIWebViewNavigationStartingEventArgs value)
    {
        if (!IsCurrentBrowser(sender)) return;
        string? requestedPage = _pendingEmbeddedPage;
        ResetDocumentTrust();
        _latestNavigationId = value.NavigationId;
        // 消费显式请求只允许一次。Runtime 可能先报告整份 HTML 的 data URI，再以 about:blank 完成；
        // data 内容必须精确匹配此次资源，最终仍需核对同一导航的 Source 和 DOM 实例标记。
        if (requestedPage != null && !value.IsRedirected
            && (IsBlankDocument(value.Uri) || _pages?.IsMatchingNavigationDataUri(requestedPage, value.Uri) == true))
        {
            _candidateEmbeddedPage = requestedPage;
            _candidateNavigationId = value.NavigationId;
        }
        Element<UILabel>("status").Text = "正在加载页面……";
        AppendLog("开始导航", _candidateEmbeddedPage is string page ? WebViewOfflinePages.GetResourceUri(page) : value.Uri);
        UpdateSourceLabel();
        UpdateControls();
    }

    /// <summary>
    /// 只有显式内嵌加载对应的导航成功，且实际 DOM 保留本实例资源标记时恢复桥接。
    /// 异步读回后再次核对导航编号，避免检查旧文档期间新导航已经发生。
    /// </summary>
    private async Task BrowserNavigationCompletedAsync(UIWebView sender, UIWebViewNavigationCompletedEventArgs value)
    {
        if (!IsCurrentBrowser(sender) || value.NavigationId != _latestNavigationId) return;
        string? page = _candidateNavigationId == value.NavigationId ? _candidateEmbeddedPage : null;
        bool verified = false;
        if (value.IsSuccess && page != null && _pages != null && IsBlankDocument(sender.Source))
        {
            try
            {
                // 标记只包含本类生成的 GUID 与固定页名，不把用户输入或网页消息拼入脚本。
                string expected = _pages.GetDocumentMarker(page);
                string result = await sender.ExecuteScriptAsync(
                    "document.querySelector('meta[name=\"tcym-demo-document\"]')?.content === '" + expected + "'", _token);
                verified = result == "true";
            }
            catch (OperationCanceledException) when (_token.IsCancellationRequested) { return; }
            catch (ObjectDisposedException) when (!IsActive) { return; }
            catch (Exception exception)
            {
                if (IsActive) AppendLog("文档校验", exception.Message);
            }
        }
        if (!IsActive || value.NavigationId != _latestNavigationId) return;
        _trustedDocumentReady = verified && IsBlankDocument(sender.Source) && _candidateNavigationId == value.NavigationId;
        _trustedEmbeddedPage = _trustedDocumentReady ? page : null;
        _candidateEmbeddedPage = null;
        _candidateNavigationId = 0;
        Element<UILabel>("status").Text = !value.IsSuccess ? $"页面加载失败（错误代码 {value.WebErrorStatus}），可返回内嵌首页重试。"
            : _trustedDocumentReady ? "内嵌页面已就绪，可以执行脚本和交换消息。"
            : "网页已加载，可正常浏览；此演示的脚本和消息仅对内嵌页面启用。";
        AppendLog("导航完成", value.IsSuccess ? (sender.DocumentTitle.Length == 0 ? sender.Source ?? "" : sender.DocumentTitle) : $"失败：{value.WebErrorStatus}");
        UpdateSourceLabel();
        UpdateControls();
    }

    /// <summary>同步地址、历史和加载状态；状态值直接来自公开快照，不访问原生接口。</summary>
    private void BrowserStateChanged(UIWebView sender, UIWebViewState value)
    {
        if (!IsCurrentBrowser(sender)) return;
        UpdateSourceLabel();
        UpdateControls();
    }

    /// <summary>等待原生导航时显示资源逻辑地址，其余情况显示真实网址，不暴露机器上的文件路径。</summary>
    private void UpdateSourceLabel()
    {
        string? page = _pendingEmbeddedPage ?? _candidateEmbeddedPage ?? _trustedEmbeddedPage;
        Element<UILabel>("source").Text = page != null ? "地址：内嵌资源 " + WebViewOfflinePages.GetResourceUri(page)
            : "地址：" + Limit(Element<UIWebView>("browser").Source ?? "等待导航", 240);
    }

    /// <summary>同时验证已确认的当前内嵌文档与原生消息来源，在线网页不会获得演示命令通道。</summary>
    private async Task BrowserWebMessageReceivedAsync(UIWebView sender, UIWebViewMessageEventArgs value)
    {
        if (!IsCurrentBrowser(sender)) return;
        if (!HasTrustedDocument() || !IsBlankDocument(value.Source))
        {
            AppendLog("已忽略消息", "当前文档尚未确认，或来源不属于本次内嵌示例。");
            return;
        }
        if (value.Text == null)
        {
            using var document = JsonDocument.Parse(value.Json);
            var root = document.RootElement;
            // 只识别两个固定内嵌页的切换消息，不执行网页传入的脚本、网址、文件名或任意命令。
            if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("type", out var type)
                && type.ValueKind == JsonValueKind.String && type.GetString() == "demo-navigate"
                && root.TryGetProperty("page", out var destination) && destination.ValueKind == JsonValueKind.String)
            {
                string? page = destination.GetString();
                if (page is "home" or "next")
                    await RunActionAsync(page == "home" ? Element<UIButton>("home") : Element<UIButton>("next"), (_, token) => LoadEmbeddedPageAsync(page, token));
                return;
            }
        }
        // 只展示文本，不把网页传入的 JSON 当作命令执行，也不将文本拼接到 JavaScript。
        AppendLog(value.Text == null ? "网页 → 宿主（JSON）" : "网页 → 宿主（字符串）", value.Text ?? value.Json);
    }

    /// <summary>区分平台不支持、初始化失败与运行中错误；不在示例里自动安装或下载 Runtime。</summary>
    private void BrowserError(UIWebView sender, UIWebViewErrorEventArgs value)
    {
        if (!IsCurrentBrowser(sender)) return;
        if (value.Operation == UIWebViewErrorOperation.Initialization)
        {
            _initializationFailed = true;
            // 初始化还可能因线程、宿主或几何状态失败，不能把所有 Linux 错误都提示成缺少运行库。
            // 原生后端已提供具体异常；在页面展示有界原因，完整业务错误仍记录到下方日志。
            Element<UILabel>("status").Text = sender.IsSupported
                ? OperatingSystem.IsLinux()
                    ? "原生网页初始化失败：" + Limit(value.Exception.Message, 220)
                    : "原生网页初始化失败。若未安装 WebView2 Runtime，请先安装微软官方 Runtime 后重新进入本页。"
                : UnsupportedPlatformText;
            Element<UILabel>("runtime").Text = sender.IsSupported
                ? "运行时：初始化未完成；可查看下方错误日志，本示例不会自动下载或安装。"
                : "运行时：当前平台无可用实现。";
        }
        else Element<UILabel>("status").Text = "网页操作失败：" + Limit(value.Exception.Message, 220);
        AppendLog("网页错误", value.Exception.Message);
        UpdateControls();
    }

    /// <summary>按实际支持范围提示；原生 Wayland 不会被静默当作 X11 后端。</summary>
    private static string UnsupportedPlatformText => OperatingSystem.IsLinux()
        ? "当前 Linux 窗口后端不支持原生网页；请使用 SDL X11 或原生 Wayland，后者需在 UI 初始化前准备原生宿主。"
        : "当前平台尚不支持原生网页；请使用 Windows x64 / ARM64，或 Linux X11 / 原生 Wayland 运行此示例。";

    /// <summary>仅按初始化、导航能力和可信文档更新可用性，短暂命令执行不会使整个工具栏闪烁。</summary>
    private void UpdateControls()
    {
        if (!IsActive || FindElement<UIWebView>("browser") is not { } browser) return;
        bool ready = !_initializationFailed && _pages != null && browser.IsInitialized && !browser.Disabled;
        SetButtonDisabled("open-url", !ready);
        SetButtonDisabled("home", !ready);
        SetButtonDisabled("next", !ready);
        SetButtonDisabled("reload", !ready);
        SetButtonDisabled("back", !ready || !browser.CanGoBack);
        SetButtonDisabled("forward", !ready || !browser.CanGoForward);
        SetButtonDisabled("stop", !ready || !browser.IsLoading);
        bool bridge = CanBridge;
        SetButtonDisabled("execute", !bridge);
        SetButtonDisabled("send-string", !bridge);
        SetButtonDisabled("send-json", !bridge);
        Element<UITextArea>("script").Disabled = !bridge;
        Element<UIInput>("message").Disabled = !bridge;
        Element<UIInput>("json").Disabled = !bridge;
        Element<UIInput>("url").Disabled = !ready;
    }

    /// <summary>通过元素 Id 更新按钮；不维护另一份控件列表，避免脱离声明式元素树。</summary>
    private void SetButtonDisabled(string id, bool disabled)
    {
        var button = FindElement<UIButton>(id);
        if (button != null && button.Disabled != disabled) button.Disabled = disabled;
    }

    /// <summary>清空有界日志；通过 Id 获取输出标签，不保留标签字段。</summary>
    private Task ClearLogAsync(UIButton sender, CancellationToken token)
    {
        _logLines.Clear();
        Element<UILabel>("log").Text = "日志已清空。";
        return Task.CompletedTask;
    }

    /// <summary>日志条数和单条长度都受限，消息风暴或超长数据不会令示例控件树持续增长。</summary>
    private void AppendLog(string category, string text)
    {
        if (!IsActive) return;
        _logLines.Enqueue($"[{DateTime.Now:HH:mm:ss}] {category}：{Limit(text.Replace('\r', ' ').Replace('\n', ' '), MaximumLogEntryLength)}");
        while (_logLines.Count > MaximumLogEntries) _logLines.Dequeue();
        Element<UILabel>("log").Text = string.Join('\n', _logLines);
    }

    /// <summary>本路由不保活；每次进入创建全新的资源标记和浏览器实例，不保留前一文档的消息状态。</summary>
    public void OnRouteEnter(string? fromPath)
    {
        if (!IsActive || _eventsAttached || FindElement<UIWebView>("browser") is not { } browser) return;
        // 路由器先挂载并登记整棵树，再调用进入通知；此时尚未进行首轮布局和原生自动初始化。
        AttachBrowserEvents(browser);
        UpdateControls();
    }

    /// <summary>路由先通知离开再拆树，必须在这里关掉接收门，不能等孩子释放后的页面 Dispose。</summary>
    public void OnRouteLeave(string? toPath) => Dispose();

    /// <summary>
    /// 同步关闭业务生命周期，不在 UI 线程阻塞原生释放或窗口呈现。HTML 来自只读内嵌资源，
    /// 没有需要删除的文件；原生控件的异步关闭由其已有 Dispatcher 生命周期负责跟踪。
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        // 离页通知先于摘树，先取得本页实例；子树已拆除时浏览器由拥有型释放流程关闭。
        var browser = FindElement<UIWebView>("browser");
        _disposed = true;
        ResetDocumentTrust();
        _lifetime.Cancel();
        if (browser != null)
        {
            if (_eventsAttached)
            {
                browser.Initialized -= BrowserInitialized;
                browser.NavigationStarting -= BrowserNavigationStarting;
                browser.NavigationCompletedAsync -= BrowserNavigationCompletedAsync;
                browser.StateChanged -= BrowserStateChanged;
                browser.WebMessageReceivedAsync -= BrowserWebMessageReceivedAsync;
                browser.Error -= BrowserError;
            }
            browser.Dispose();
        }
        _eventsAttached = false;
        _lifetime.Dispose();
    }

    /// <summary>运行时按 Id 获取组件，并验证其仍属于本页；旧回调不能误操作重入路由的新实例。</summary>
    private T? FindElement<T>(string id) where T : UIElement
    {
        var element = UISystem.Manager?.GetElementById<T>("webview-demo-" + id);
        return element != null && OwnsElement(element) ? element : null;
    }

    /// <summary>当前操作要求目标仍在本页树内；未挂载或已拆树时及时停止，不退回跨页面查找。</summary>
    private T Element<T>(string id) where T : UIElement => FindElement<T>(id)
        ?? throw new InvalidOperationException("当前 WebView 示例中找不到组件：" + id);

    /// <summary>只按父链验证归属，不缓存全局 Id 查询结果。</summary>
    private bool OwnsElement(UIElement element)
    {
        for (UIElement? current = element; current != null; current = current.Parent)
            if (ReferenceEquals(current, this)) return true;
        return false;
    }

    /// <summary>浏览器通知必须同时通过页面生命周期、当前 Id 注册和父链归属检查。</summary>
    private bool IsCurrentBrowser(UIWebView sender)
        => IsActive && ReferenceEquals(sender, FindElement<UIWebView>("browser"));

    /// <summary>限制显示长度；脚本与消息的实际发送内容不因日志截断而改变。</summary>
    private static string Limit(string value, int maximum) => value.Length <= maximum ? value : value[..maximum] + "…";
}
