using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Graph;

/// <summary>24 个机房 / 区域与 53 个部门组成的双层分布拓扑。</summary>
internal sealed class DistributedTopologyGraphDemo : GraphChartDemoPage
{
    private static readonly string[] Groups =
    [
        "南开区",
        "河北区",
        "西青区",
        "空港IDC",
        "武清区",
        "市电子政务中心",
        "东丽区",
        "宝坻区",
        "和平区",
        "蓟州区",
        "河东区",
        "河西区",
        "静海区",
        "北辰区",
        "红桥区",
        "滨海新区",
        "宁河区",
        "烟台道联通机房",
        "市政府",
        "市人大",
        "市政协",
        "津南区",
        "华苑IDC",
        "科技信息所",
    ];
    private static readonly (string Name, string Parent)[] Departments =
    [
        ("市监狱局", "南开区"),
        ("南开区政府", "南开区"),
        ("市税务局", "河北区"),
        ("河北区政府", "河北区"),
        ("市公安局", "西青区"),
        ("西青区政府", "西青区"),
        ("市税务局数据中心", "空港IDC"),
        ("空港数据中心", "空港IDC"),
        ("武清区政府", "武清区"),
        ("市应急局", "市电子政务中心"),
        ("市气象局", "市电子政务中心"),
        ("市合作交流办", "市电子政务中心"),
        ("市交通运输委", "市电子政务中心"),
        ("市司法局", "市电子政务中心"),
        ("市高法", "市电子政务中心"),
        ("东丽区政府", "东丽区"),
        ("宝坻区政府", "宝坻区"),
        ("市科技局", "和平区"),
        ("市统计局", "和平区"),
        ("和平区政府", "和平区"),
        ("蓟州区政府", "蓟州区"),
        ("河东区政府", "河东区"),
        ("市人防办", "河东区"),
        ("市粮食和物资储备局", "河东区"),
        ("市场监管委", "河东区"),
        ("河西区政府", "河西区"),
        ("市戒毒局", "河西区"),
        ("市安全局", "河西区"),
        ("市农委", "河西区"),
        ("市文旅局", "河西区"),
        ("市纪检委", "河西区"),
        ("静海区政府", "静海区"),
        ("北辰区政府", "北辰区"),
        ("红桥区政府", "红桥区"),
        ("滨海新区政府", "滨海新区"),
        ("宁河区政府", "宁河区"),
        ("市医保局", "烟台道联通机房"),
        ("市信访办", "烟台道联通机房"),
        ("市住建局", "烟台道联通机房"),
        ("市商务局", "烟台道联通机房"),
        ("市编办", "市政府"),
        ("市机要局", "市政府"),
        ("市政府办公厅", "市政府"),
        ("市委网信办", "市政府"),
        ("市委办公厅", "市政府"),
        ("市财政局数据中心", "市人大"),
        ("市政协办公厅", "市政协"),
        ("津南区政府", "津南区"),
        ("市人大办公厅", "津南区"),
        ("华苑数据中心", "华苑IDC"),
        ("市民政局", "华苑IDC"),
        ("市审计局", "科技信息所"),
        ("市国资委", "科技信息所"),
    ];

    internal DistributedTopologyGraphDemo() : base(
        "多节点分布拓扑图", "内圈展示机房 / 区域，外圈展示所属部门，连线表示数据流向。",
        "保留附件中的 24 个分组、53 个部门和云图。原 Cartesian 坐标映射为 Graph 逻辑像素；通过本示例的 View 扩展绘制流动 pin，不反复 SetOption，也不占用后台定时器。",
        CreateOption(), GraphEdgeEffects.CreateRegistry(flows: CreateFlows()), "cloud.png") { }

    private static ChartOption CreateOption(SKSize viewport = default)
    {
        // 原图的 [0, 0] 汇聚点没有可见图标，这里用不可见节点承载连线端点。
        var data = new ChartGraphData()
        {
            new ChartGraphNode { Id = "hub", Name = "汇聚中心", X = viewport.Width / 2, Y = viewport.Height / 2, Symbol = "none" }
        };
        for (var index = 0; index < Groups.Length; index++)
        {
            var name = Groups[index];
            var point = RingPoint(index, Groups.Length, 0.72, viewport);
            data.Add(new ChartGraphNode
            {
                Id = name, Name = name, X = point.X, Y = point.Y,
                Symbol = "image://cloud.png", SymbolSize = new ChartSymbolSize(45, 28), Value = "区域 / 机房",
                Label = new()
                {
                    Show = true, Position = "bottom", Distance = 8, FontSize = 10,
                    Formatter = "{cloud|{b}}",
                    Rich = new()
                    {
                        ["cloud"] = new ChartTextStyle
                        {
                            Color = ChartBrush.Solid(SKColors.White), BorderColor = ChartBrush.Solid(new SKColor(89, 197, 238)),
                            BorderWidth = 1, BorderRadius = 9, Padding = new ChartInsets(3, 5, 3, 5),
                            BackgroundColor = ChartBrush.Solid(new SKColor(0, 0, 0, 200)),
                        },
                    },
                },
            });
            data.AddLink(new ChartGraphEdge("hub", name)
            {
                Id = "to-" + name, Name = "中心 → " + name,
                LineStyle = new() { Color = ChartBrush.Solid(name == "河北区" ? SKColors.Red : SKColors.White) },
            });
        }
        for (var index = 0; index < Departments.Length; index++)
        {
            var (name, parent) = Departments[index];
            var point = RingPoint(index, Departments.Length, 1, viewport);
            var angle = MathF.Atan2(point.Y - viewport.Height / 2, point.X - viewport.Width / 2) * 180 / MathF.PI;
            var right = point.X >= viewport.Width / 2;
            data.Add(new ChartGraphNode
            {
                Id = name, Name = name, Value = parent, X = point.X, Y = point.Y,
                Symbol = "circle", SymbolSize = 9,
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(SKColor.Parse("#0ceffd")),
                    BorderColor = ChartBrush.Solid(name == "市监狱局" ? new SKColor(217, 0, 27) : new SKColor(4, 242, 28)),
                    BorderWidth = 2,
                },
                Label = new()
                {
                    Show = true, Position = right ? "right" : "left", Distance = 8,
                    Rotate = right ? angle : angle + 180, FontSize = 10,
                    Align = right ? ChartTextAlign.Left : ChartTextAlign.Right,
                    Color = ChartBrush.Solid(new SKColor(24, 163, 239)), Formatter = "{b}",
                },
            });
            data.AddLink(new ChartGraphEdge(parent, name)
            {
                Id = "to-" + name, Name = parent + " → " + name,
                LineStyle = new() { Color = ChartBrush.Solid(name == "市监狱局" ? SKColors.Red : SKColors.White) },
            });
        }
        return new ChartOption
        {
            Animation = new() { Enabled = false },
            Background = ChartBrush.Solid(SKColors.Black),
            Tooltip = new()
            {
                Trigger = ChartTooltipTrigger.Item, Confine = true,
                FormatterCallback = (in ChartTooltipFormatterContext context) =>
                {
                    if (context.Parameters.Count == 0) return ChartTooltipContent.FromText("");
                    var item = context.Parameters[0];
                    return ChartTooltipContent.FromText(item.DataType == "edge" ? item.Name
                        : $"{item.Name}\r\n{item.Value.Primary.ToObject()}");
                },
            },
            Series = new ChartGraphSeriesOption
            {
                Id = "distributed-topology", Layout = ChartGraphLayout.None, DataSource = data,
                SymbolSize = 9, LineStyle = new() { Color = ChartBrush.Solid(SKColors.White), Width = 1.2f, Opacity = 0.75f },
            },
        };
    }

    protected override void OnViewportChanged(SKSize size) =>
        Chart.SetOption(new ChartOption { Series = CreateOption(size).Series });

    private static SKPoint RingPoint(int index, int count, double radius, SKSize viewport)
    {
        var angle = -Math.PI / 2 + Math.PI * 2 * (index + 1) / count;
        return new SKPoint((float)(viewport.Width / 2 + Math.Cos(angle) * viewport.Width / 3 * radius),
            (float)(viewport.Height / 2 + Math.Sin(angle) * viewport.Height * 0.3 * radius));
    }

    private static IReadOnlyDictionary<string, GraphEdgeEffects.Flow> CreateFlows()
    {
        var flows = new Dictionary<string, GraphEdgeEffects.Flow>(StringComparer.Ordinal);
        foreach (var name in Groups)
            flows.Add("to-" + name, new GraphEdgeEffects.Flow(name == "科技信息所" ? 2 : 6, 4, 16));
        foreach (var (name, _) in Departments)
            flows.Add("to-" + name, name switch
            {
                "市监狱局" => new GraphEdgeEffects.Flow(2, 6, 20),
                "市审计局" => new GraphEdgeEffects.Flow(4, 4, 8),
                _ => new GraphEdgeEffects.Flow(6, 4, 8),
            });
        return flows;
    }
}
