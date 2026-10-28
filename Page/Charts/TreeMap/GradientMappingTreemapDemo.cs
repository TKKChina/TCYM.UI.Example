using System.Globalization;
using System.Text.Json;
using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.TreeMap;

/// <summary>按预算增长率映射红、灰、绿的矩形树图。</summary>
internal sealed class GradientMappingTreemapDemo : TreeMapChartDemoPage
{
    // 原始数据：https://echarts.apache.org/examples/data/asset/data/obama_budget_proposal_2012.json
    private const string ResourceName =
        "TCYM.UI.Example.Page.Charts.TreeMap.Assets.obama_budget_proposal_2012.json";

    private static readonly SKColor NegativeColor = SKColor.Parse("#942e38");
    private static readonly SKColor NeutralColor = SKColor.Parse("#aaaaaa");
    private static readonly SKColor PositiveColor = SKColor.Parse("#269f3c");

    internal GradientMappingTreemapDemo() : base(
        "映射为渐变色",
        "使用 2012 年预算金额决定矩形面积，以相对 2011 年的变化率决定颜色。",
        "增长为绿色、下降为红色、零值或缺失变化率为灰色。原始年度金额和变化率保留在多维值中；点击矩形放大，面包屑返回上层。",
        CreateOption()) { }

    /// <summary>读取嵌入数据，创建完整数据源、颜色、Tooltip 和 Options。</summary>
    private static ChartOption CreateOption()
    {
        // 1. 保留原始三个维度，再按原示例计算第四维；第一维仍用于面积。
        using var stream = typeof(GradientMappingTreemapDemo).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException("缺少嵌入的预算数据：" + ResourceName);
        using var document = JsonDocument.Parse(stream);
        if (document.RootElement.ValueKind != JsonValueKind.Array)
            throw new InvalidDataException("预算数据的根节点必须是数组。");

        var ids = new HashSet<string>(StringComparer.Ordinal);
        var data = new ChartTreemapData();
        foreach (var node in ConvertLevel(document.RootElement, ids)) data.Add(node);

        // 2. 颜色由原生 Treemap 按第四维和 Levels 调色板映射。
        return new ChartOption
        {
            Background = ChartBrush.Solid(SKColors.White),
            Animation = new() { Enabled = true, Duration = 700, UpdateDuration = 450 },
            Title = new()
            {
                Text = "Gradient Mapping",
                Subtext = "Growth > 0: green; Growth < 0: red; Growth = 0: grey",
                Left = "center",
            },
            Tooltip = new()
            {
                Trigger = ChartTooltipTrigger.Item,
                Confine = true,
                FormatterCallback = static (in ChartTooltipFormatterContext context) =>
                {
                    if (context.Parameters.Count == 0) return ChartTooltipContent.FromText("");
                    var item = context.Parameters[0];
                    var values = item.Value.Values;
                    return ChartTooltipContent.FromText(
                        $"{item.Name}\r\n" +
                        $"2012 Amount: {FormatAmount(Number(values, 0))}\r\n" +
                        $"2011 Amount: {FormatAmount(Number(values, 1))}\r\n" +
                        $"Change From 2011: {FormatChange(Number(values, 2))}");
                },
            },
            Series = new ChartTreemapSeriesOption
            {
                Id = "budget-gradient",
                Name = "ALL",
                DataSource = data,
                Top = ChartLength.Pixels(80),
                LevelsIncludeVirtualRoot = true,
                VisualDimension = 3,
                VisualMin = -100,
                VisualMax = 100,
                Label = new() { Show = true, Formatter = "{b}" },
                ItemStyle = new()
                {
                    BorderColor = ChartBrush.Solid(SKColors.Black),
                    BorderWidth = 0, GapWidth = 0,
                },
                // 与原示例一致：第 0 层配置虚拟根，第 1 层配置颜色范围。
                Levels =
                [
                    new() { ItemStyle = new() { BorderWidth = 3, BorderColor = ChartBrush.Solid(SKColor.Parse("#333333")), GapWidth = 3 } },
                    new()
                    {
                        Color = [ChartBrush.Solid(NegativeColor), ChartBrush.Solid(NeutralColor), ChartBrush.Solid(PositiveColor)],
                        ColorMappingBy = ChartTreemapColorMappingBy.Value,
                        ItemStyle = new() { GapWidth = 1 },
                    },
                ],
                NodeClick = ChartTreemapNodeClick.ZoomToNode,
                Roam = true,
                Breadcrumb = new() { Show = true, RootName = "ALL" },
            },
        };
    }

    /// <summary>每一层分别计算极值，避免不同父节点的增长率互相影响。</summary>
    private static IReadOnlyList<ChartTreemapNode> ConvertLevel(
        JsonElement siblings, HashSet<string> ids)
    {
        if (siblings.ValueKind != JsonValueKind.Array)
            throw new InvalidDataException("预算节点的 children 必须是数组。");

        var entries = siblings.EnumerateArray().Where(item => item.ValueKind != JsonValueKind.Null).ToArray();
        var min = double.PositiveInfinity;
        var max = double.NegativeInfinity;
        foreach (var item in entries)
        {
            var change = ReadNumber(item.GetProperty("value"), 2);
            if (change is not { } number) continue;
            min = Math.Min(min, number);
            max = Math.Max(max, number);
        }

        var result = new List<ChartTreemapNode>(entries.Length);
        foreach (var item in entries)
        {
            var id = item.GetProperty("id").GetString();
            var name = item.GetProperty("name").GetString();
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
                throw new InvalidDataException("预算节点必须包含非空 id 和 name。");

            var vector = item.GetProperty("value");
            var amount2012 = ReadNumber(vector, 0);
            var amount2011 = ReadNumber(vector, 1);
            var change = ReadNumber(vector, 2);
            if (amount2012 < 0)
                throw new InvalidDataException("预算面积不能为负数：" + id);

            var mapped = MapChange(change, min, max);
            if (!ids.Add(id))
                throw new InvalidDataException("预算节点 id 重复：" + id);

            var node = new ChartTreemapNode
            {
                Id = id,
                Name = name,
                Values =
                [
                    amount2012 is { } amount ? ChartValue.From(amount) : ChartValue.Null,
                    amount2011 is { } previous ? ChartValue.From(previous) : ChartValue.Null,
                    change is { } growth ? ChartValue.From(growth) : ChartValue.Null,
                    ChartValue.From(mapped),
                ],
            };
            if (item.TryGetProperty("children", out var children) && children.ValueKind != JsonValueKind.Null)
                foreach (var child in ConvertLevel(children, ids)) node.Children.Add(child);
            result.Add(node);
        }
        return result;
    }

    private static double? ReadNumber(JsonElement vector, int index)
    {
        if (vector.ValueKind != JsonValueKind.Array || vector.GetArrayLength() <= index)
            throw new InvalidDataException("预算节点的 value 必须包含 2012、2011 和变化率三个维度。");
        var value = vector[index];
        if (value.ValueKind == JsonValueKind.Null) return null;
        if (value.ValueKind != JsonValueKind.Number || !value.TryGetDouble(out var number) || !double.IsFinite(number))
            throw new InvalidDataException("预算维度必须是有限数字或 null。");
        return number;
    }

    private static double MapChange(double? change, double min, double max)
    {
        if (change is > 0 && max > 0) return 40 + change.Value / max * 60;
        if (change is < 0 && min < 0) return -40 + change.Value / -min * 60;
        return 0;
    }

    private static double? Number(IReadOnlyList<ChartValue> values, int index) =>
        index < values.Count && values[index].TryGetDouble(out var number) && double.IsFinite(number)
            ? number : null;

    private static string FormatAmount(double? value) =>
        value is { } amount ? (amount * 1000).ToString("N0", CultureInfo.InvariantCulture) + "$" : "-";

    private static string FormatChange(double? value) =>
        value is { } change ? change.ToString("F2", CultureInfo.InvariantCulture) + "%" : "-";
}
