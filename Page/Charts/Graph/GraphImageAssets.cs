using SkiaSharp;
using TCYM.UI.Chart.Rendering.Symbols;

namespace Page.Charts.Graph;

/// <summary>只解析已在后台解码的本地嵌入资源；Resolve 不进行网络、文件读取或图片解码。</summary>
internal sealed class GraphImageAssets : IChartImageResolver, IDisposable
{
    private readonly Dictionary<string, SKBitmap> _bitmaps = new(StringComparer.Ordinal);
    internal static Stream Open(string name) => typeof(GraphImageAssets).Assembly.GetManifestResourceStream(
        "TCYM.UI.Example.Page.Charts.Graph.Assets." + name)
        ?? throw new InvalidOperationException("缺少 Graph 嵌入资源：" + name);

    internal static GraphImageAssets Load(IEnumerable<string> names, CancellationToken token = default)
    {
        var assets = new GraphImageAssets();
        try
        {
            foreach (var name in names.Distinct(StringComparer.Ordinal))
            {
                token.ThrowIfCancellationRequested();
                using var stream = Open(name);
                var bitmap = SKBitmap.Decode(stream) ?? throw new InvalidOperationException("图片解码失败：" + name);
                bitmap.SetImmutable();
                assets._bitmaps.Add(name, bitmap);
            }
            token.ThrowIfCancellationRequested();
            return assets;
        }
        catch { assets.Dispose(); throw; }
    }

    public ChartImageResolveResult Resolve(string source) =>
        _bitmaps.TryGetValue(source, out var bitmap)
            ? ChartImageResolveResult.Success(SKImage.FromBitmap(bitmap))
            : ChartImageResolveResult.Failure("GRAPH_DEMO_IMAGE_MISSING", "图片未预加载：" + source);

    public void Dispose()
    {
        foreach (var bitmap in _bitmaps.Values) bitmap.Dispose();
        _bitmaps.Clear();
    }
}
