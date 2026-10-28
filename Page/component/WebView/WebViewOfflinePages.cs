using System.Text;

namespace TCYM.UI.Example.Page.component.WebView;

/// <summary>
/// 从 Example 程序集读取两份自包含 HTML。文档直接交给 NavigateToString，不创建磁盘文件、
/// 不启动本地 HTTP 服务，也不要求部署时复制额外 HTML 或引入资源管理包。
/// </summary>
internal sealed class WebViewOfflinePages
{
    private const string ResourcePrefix = "TCYM.UI.Example.Page.component.WebView.";
    private const string MarkerPlaceholder = "__TCYM_DEMO_DOCUMENT__";
    // about:blank 不是可信来源标识；每个页面实例用独立标记核对实际完成的文档。
    private readonly string _instanceMarker = Guid.NewGuid().ToString("N");
    private readonly string _homeHtml;
    private readonly string _nextHtml;

    /// <summary>只读加载明确列出的内嵌资源，缺少资源时直接报告配置问题，不回退到临时文件。</summary>
    internal WebViewOfflinePages()
    {
        _homeHtml = ReadHtml("home");
        _nextHtml = ReadHtml("next");
    }

    /// <summary>取得当前示例允许的两页之一；不将网页传入的任意名称拼成资源路径。</summary>
    internal string GetHtml(string page) => page switch
    {
        "home" => _homeHtml,
        "next" => _nextHtml,
        _ => throw new ArgumentOutOfRangeException(nameof(page)),
    };

    /// <summary>显示资源的稳定逻辑地址，避免把 WebView2 的 about:blank 误显示成磁盘路径。</summary>
    internal static string GetResourceUri(string page) => page switch
    {
        "home" => "res://TCYM.UI.Example/Page.component.WebView.home.html",
        "next" => "res://TCYM.UI.Example/Page.component.WebView.next.html",
        _ => throw new ArgumentOutOfRangeException(nameof(page)),
    };

    /// <summary>该标记只用于确认本次显式加载的资源文档，不把任意 about:blank 自动纳入消息通道。</summary>
    internal string GetDocumentMarker(string page) => page switch
    {
        "home" or "next" => _instanceMarker + "-" + page,
        _ => throw new ArgumentOutOfRangeException(nameof(page)),
    };

    /// <summary>
    /// 某些 WebView2 Runtime 在 NavigateToString 开始事件中报告 data:text/html;base64，
    /// 完成后的 Source 才是 about:blank。只接受与本次明确请求的资源 UTF-8 字节完全相同的内容；
    /// 不以 data: 前缀或 HTML MIME 类型本身授予桥接许可。
    /// </summary>
    internal bool IsMatchingNavigationDataUri(string page, string navigationUri)
    {
        int separator = navigationUri.IndexOf(',');
        if (separator < 0) return false;
        ReadOnlySpan<char> header = navigationUri.AsSpan(0, separator);
        if (!header.Equals("data:text/html;charset=utf-8;base64", StringComparison.OrdinalIgnoreCase)
            && !header.Equals("data:text/html;base64", StringComparison.OrdinalIgnoreCase)) return false;
        byte[] expected = Encoding.UTF8.GetBytes(GetHtml(page));
        // 先限定为这份资源的编码长度，避免为任意网页提交的超长 data URI 额外分配解码缓冲。
        if (navigationUri.Length - separator - 1 != ((expected.Length + 2) / 3) * 4) return false;
        try
        {
            byte[] actual = Convert.FromBase64String(navigationUri[(separator + 1)..]);
            return actual.AsSpan().SequenceEqual(expected);
        }
        catch (FormatException) { return false; }
    }

    /// <summary>GetManifestResourceStream 在 JIT 和 NativeAOT 下都按固定名称读取，无运行时类型反射。</summary>
    private string ReadHtml(string page)
    {
        string resource = ResourcePrefix + page + ".html";
        using Stream stream = typeof(WebViewOfflinePages).Assembly.GetManifestResourceStream(resource)
            ?? throw new InvalidOperationException("缺少 WebView 内嵌文档：" + resource);
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return reader.ReadToEnd().Replace(MarkerPlaceholder, GetDocumentMarker(page), StringComparison.Ordinal);
    }
}
