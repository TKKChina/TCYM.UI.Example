using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Graph;

/// <summary>图片节点、三类图例、富文本标注及节点/边不同 Tooltip。</summary>
internal sealed class ComplexRelationGraphDemo : GraphChartDemoPage
{
    private sealed record PersonInfo(int Num, int Type) : IChartImmutableDataValue;
    private readonly ChartGraphData _data;
    private readonly SKPoint[] _positions;
    internal ComplexRelationGraphDemo() : base(
        "复杂关系图", "展示人员分类、密接人数和家人 / 同事 / 朋友关系。",
        "保留附件中的 11 人、10 条关系和原始图片。图片在后台预加载；节点可拖动，悬停人员和连线显示不同信息。边箭头由本示例的公开 View 扩展绘制。",
        CreateOption(out var data), GraphEdgeEffects.CreateRegistry(arrows: true),
        "person-healthy.png", "person-contact.png", "person-positive.png", "person-confirmed.png",
        "legend-healthy.png", "legend-asymptomatic.png", "legend-confirmed.png")
    {
        _data = data;
        _positions = data.Nodes.Select(node => new SKPoint((float)node.X!.Value, (float)node.Y!.Value)).ToArray();
    }

    private static ChartOption CreateOption(out ChartGraphData data)
    {
        // 原片段分类与图标颜色不一致；修正三类图例归属，Num/Type/日期与原值一致。
        (string Name, int Type, int Category, double X, double Y, string Image)[] people =
        [
            ("张三三", 0, 2, 20, 40, "person-positive.png"),
            ("赵五", 1, 2, -20, 30, "person-confirmed.png"),
            ("刘六", 2, 1, -10, 50, "person-contact.png"),
            ("光头强", 1, 2, 0, 30, "person-confirmed.png"),
            ("熊大", 2, 1, 36, 39, "person-contact.png"),
            ("张芳芳", 3, 0, 10, 50, "person-healthy.png"),
            ("李四1", 3, 0, 36, 25, "person-healthy.png"),
            ("李四2", 3, 0, 45, 48, "person-healthy.png"),
            ("李四3", 3, 0, 35, 52, "person-healthy.png"),
            ("李四4", 3, 0, 34, 62, "person-healthy.png"),
            ("李四5", 3, 0, 18, 56, "person-healthy.png"),
        ];
        data = new ChartGraphData();
        foreach (var person in people)
            data.Add(new ChartGraphNode
            {
                Id = person.Name, Name = person.Name, CategoryIndex = person.Category,
                // 先保存 0~1 的位置比例，布局时再换算为当前画布的逻辑像素。
                X = 0.08 + (person.X + 20) / 65 * 0.74,
                // 末端多留 30px，避免李四3的名字与李四4的人数徽标重叠。
                Y = (100 + (person.Y - 25) * 10 + (person.Name == "李四4" ? 30 : 0)) / 590,
                Symbol = "image://" + person.Image,
                Value = ChartValue.FromObject(new PersonInfo(55, person.Type)),
                // FormatterCallback 返回纯文本；富文本使用 Formatter 模板，避免显示原始标记。
                Label = new() { Formatter = $"{{a{person.Type}|密接人数：55}}\n\n\n\n\n{{b}}" },
            });
        (int Source, int Target, string Name)[] links =
        [
            (1, 2, "家人1"), (2, 3, "家人2"), (3, 0, "同事1"), (0, 4, "朋友1"),
            (0, 5, "同事2"), (0, 6, "朋友2"), (0, 7, "家人3"), (0, 8, "家人4"),
            (0, 9, "同事4"), (0, 10, "同事5"),
        ];
        foreach (var link in links)
            data.AddLink(new ChartGraphEdge(link.Source, link.Target)
            {
                Id = "contact-" + data.Links.Count, Name = link.Name, Value = link.Name,
            });

        return new ChartOption
        {
            Animation = new() { Enabled = false },
            Background = ChartBrush.Solid(new SKColor(51, 51, 51)),
            Tooltip = new()
            {
                Trigger = ChartTooltipTrigger.Item, Confine = true,
                FormatterCallback = (in ChartTooltipFormatterContext context) =>
                {
                    if (context.Parameters.Count == 0) return ChartTooltipContent.FromText("");
                    var item = context.Parameters[0];
                    if (item.DataType == "edge")
                        return ChartTooltipContent.FromText($"{item.Name}\r\n接触：2021-01-24");
                    if (item.Value.Primary.ToObject() is not PersonInfo person)
                        return ChartTooltipContent.FromText(item.Name);
                    var dates = person.Type switch
                    {
                        0 => "检测阳性：2021-02-17\r\n出院：2021-03-03",
                        1 => "发病：2021-01-31\r\n确诊：2021-01-31\r\n出院：2021-01-31",
                        _ => "暂无检测记录",
                    };
                    return ChartTooltipContent.FromText($"{item.Name}\r\n密接人数：{person.Num}\r\n{dates}");
                },
            },
            Legend = new()
            {
                Bottom = "5%", Right = "2%", Orient = ChartLegendOrient.Vertical,
                ItemWidth = 20, ItemHeight = 20,
                TextStyle = new() { Color = ChartBrush.Solid(SKColors.White) },
                Data =
                {
                    new ChartLegendDataItem("未感染") { Icon = "image://legend-healthy.png" },
                    new ChartLegendDataItem("无症状感染") { Icon = "image://legend-asymptomatic.png" },
                    new ChartLegendDataItem("确诊病例") { Icon = "image://legend-confirmed.png" },
                },
            },
            Series = new ChartGraphSeriesOption
            {
                Id = "complex-relations", Layout = ChartGraphLayout.None,
                Draggable = true, Roam = true, SymbolSize = 70, DataSource = data,
                Categories =
                [
                    new() { Name = "未感染" }, new() { Name = "无症状感染" }, new() { Name = "确诊病例" },
                ],
                Label = new()
                {
                    Show = true, Position = "inside", Align = ChartTextAlign.Center,
                    Color = ChartBrush.Solid(SKColors.White), FontSize = 12,
                    Rich = new()
                    {
                        ["a0"] = Badge(new SKColor(255, 86, 76, 115)),
                        ["a1"] = Badge(new SKColor(255, 86, 76, 89)),
                        ["a2"] = Badge(new SKColor(239, 255, 55, 89)),
                        ["a3"] = Badge(new SKColor(50, 233, 255, 89)),
                    },
                },
                EdgeLabel = new() { Show = true, FontSize = 10, Formatter = "{c}", Color = ChartBrush.Solid(SKColors.White) },
                LineStyle = new() { Color = ChartBrush.Solid(SKColors.White), Width = 1, Opacity = 0.9f },
            },
        };
    }

    private static ChartTextStyle Badge(SKColor background) => new()
    {
        Color = ChartBrush.Solid(SKColors.White), BackgroundColor = ChartBrush.Solid(background),
        Padding = new ChartInsets(4, 5, 4, 5),
    };

    protected override void OnViewportChanged(SKSize size)
    {
        for (var index = 0; index < _data.Count; index++)
        {
            _data.Nodes[index].X = _positions[index].X * size.Width;
            _data.Nodes[index].Y = _positions[index].Y * size.Height;
        }
        Chart.SetOption(new ChartOption
        {
            Series = new ChartGraphSeriesOption { Id = "complex-relations", DataSource = _data },
        });
    }
}
