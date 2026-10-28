using System.Globalization;
using System.Text.Json;
using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.TreeMap;

/// <summary>两个磁盘案例共享原始数据；只解析一次，并缓存不可变快照。</summary>
internal sealed class DiskUsageTreemapData
{
    // 原始数据：https://echarts.apache.org/examples/data/asset/data/disk.tree.json
    private const string ResourceName = "TCYM.UI.Example.Page.Charts.TreeMap.Assets.disk.tree.json";
    private static readonly Lazy<DiskUsageTreemapData> Shared = new(Load, isThreadSafe: true);
    private readonly Dictionary<string, string> _paths;

    // ECharts 示例使用的主题调色板。
    internal static IReadOnlyList<SKColor> Palette { get; } = Array.AsReadOnly(new[]
    {
        SKColor.Parse("#5070dd"), SKColor.Parse("#b6d634"), SKColor.Parse("#505372"),
        SKColor.Parse("#ff994d"), SKColor.Parse("#0ca8df"), SKColor.Parse("#ffd10a"),
        SKColor.Parse("#fb628b"), SKColor.Parse("#785db0"), SKColor.Parse("#3fbe95"),
    });

    private DiskUsageTreemapData(ChartDataSnapshot source, Dictionary<string, string> paths)
    {
        Source = source;
        _paths = paths;
    }

    internal static DiskUsageTreemapData Instance => Shared.Value;
    internal ChartDataSnapshot Source { get; }

    private static DiskUsageTreemapData Load()
    {
        using var stream = typeof(DiskUsageTreemapData).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException("缺少嵌入的磁盘数据：" + ResourceName);
        using var document = JsonDocument.Parse(stream, new JsonDocumentOptions { MaxDepth = 512 });
        if (document.RootElement.ValueKind != JsonValueKind.Array)
            throw new InvalidDataException("磁盘数据的根必须是数组。");

        var data = new ChartTreemapData();
        var paths = new Dictionary<string, string>(StringComparer.Ordinal);
        var serial = 0;
        foreach (var node in ReadLevel(document.RootElement, "", ref serial, paths))
            data.Add(node);

        // 全树一次性捕获，SetOption 可共享不可变快照；两个案例不复用可变节点。
        return new DiskUsageTreemapData(ChartDataSnapshot.Capture(data), paths);
    }

    private static IReadOnlyList<ChartTreemapNode> ReadLevel(
        JsonElement level, string parentPath, ref int serial, Dictionary<string, string> paths)
    {
        if (level.ValueKind != JsonValueKind.Array)
            throw new InvalidDataException("磁盘数据的 children 必须是数组。");
        var entries = level.EnumerateArray().Where(item => item.ValueKind != JsonValueKind.Null).ToArray();
        var result = new List<ChartTreemapNode>(entries.Length);

        for (var index = 0; index < entries.Length; index++)
        {
            var entry = entries[index];
            var id = "disk-" + serial.ToString(CultureInfo.InvariantCulture);
            serial++;
            var name = entry.TryGetProperty("name", out var nameElement) && nameElement.ValueKind == JsonValueKind.String
                ? nameElement.GetString() : null;
            var segment = string.IsNullOrEmpty(name) ? id : name;
            var path = string.IsNullOrEmpty(parentPath) ? segment : parentPath + "/" + segment;
            var value = ReadValue(entry);
            var node = new ChartTreemapNode
            {
                Id = id, Name = name,
                Value = value.HasValue ? ChartValue.From(value.Value) : ChartValue.Null,
            };

            paths.Add(id, path);
            if (entry.TryGetProperty("children", out var children) && children.ValueKind != JsonValueKind.Null)
                foreach (var child in ReadLevel(children, path, ref serial, paths))
                    node.Children.Add(child);
            result.Add(node);
        }
        return result;
    }

    private static double? ReadValue(JsonElement item)
    {
        if (item.ValueKind != JsonValueKind.Object)
            throw new InvalidDataException("磁盘节点必须是对象。");
        if (!item.TryGetProperty("value", out var value) || value.ValueKind == JsonValueKind.Null) return null;
        if (value.ValueKind != JsonValueKind.Number || !value.TryGetDouble(out var number)
            || !double.IsFinite(number) || number < 0)
            throw new InvalidDataException("磁盘节点大小必须是有限非负数。");
        return number;
    }

    /// <summary>使用稳定 Id 查找原始路径；实际显示原始值，避免父值被有效面积修正后改变。</summary>
    internal static ChartTooltipContent FormatTooltip(in ChartTooltipFormatterContext context)
    {
        if (context.Parameters.Count == 0) return ChartTooltipContent.Empty;
        var item = context.Parameters[0];
        var snapshot = item.RawData as ChartTreemapNodeSnapshot;
        var path = snapshot?.Id is { } id && Instance._paths.TryGetValue(id, out var valuePath)
            ? valuePath : item.Name ?? "";
        var original = snapshot?.Value ?? ChartValue.Null;
        var value = original.TryGetDouble(out var raw) ? raw
            : item.Value.Primary.TryGetDouble(out var effective) ? effective : (double?)null;
        var formatted = value?.ToString("#,0.################", CultureInfo.InvariantCulture) ?? "-";
        return ChartTooltipContent.FromText($"{path}\r\nDisk Usage: {formatted} KB");
    }

}
