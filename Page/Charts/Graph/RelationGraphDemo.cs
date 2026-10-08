using SkiaSharp;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Graph;

/// <summary>使用力导向布局展示资源数量及其属性、关联对象。</summary>
internal sealed class RelationGraphDemo : GraphChartDemoPage
{
    private const float NodeSize = 100;
    private static readonly SKColor PropertyColor = SKColor.Parse("#F5BB4A");
    private static readonly SKColor RelatedColor = SKColor.Parse("#62C6F8");

    internal RelationGraphDemo() : base(
        "关系图",
        "用知识图谱展示资源对象，以及展览、专题、资讯、报纸、期刊等关联资源。",
        "节点内显示名称，连线上显示资源数量。支持拖动节点、平移和缩放；NodeScaleRatio = 0 让圆点保持 100px，画布宽度随容器自适应。",
        CreateOption()) { }

    /// <summary>本案例的完整 Options、节点和连线都在这里创建。</summary>
    private static ChartOption CreateOption()
    {
        // 1. Name 是节点身份和 Tooltip 名称，Value 保存节点内需要换行的显示名称。
        var data = new ChartGraphData();
        (string Name, string ShowName, string Count, int Category)[] resources =
        [
            ("展览", "展览", "100部", 0),
            ("专题", "专题", "200个", 0),
            ("资讯", "资讯", "100条", 0),
            ("报纸", "报纸", "120条", 1),
            ("主题订阅", "主题\n订阅", "90条", 1),
            ("期刊", "期刊", "100种", 1),
            ("视频", "视频", "200条", 1),
        ];
        foreach (var resource in resources)
        {
            data.Add(new ChartGraphNode
            {
                Id = resource.Name,
                Name = resource.Name,
                Value = resource.ShowName,
                CategoryIndex = resource.Category,
            });
            data.AddLink(new ChartGraphEdge(resource.Name, "对象")
            {
                Id = resource.Name + "-对象",
                Name = resource.Name + " → 对象",
                Value = resource.Count,
                // 对应 ECharts 的 color: 'source'，采用来源节点的分类颜色。
                LineStyle = new()
                {
                    Color = ChartBrush.Solid(resource.Category == 0 ? PropertyColor : RelatedColor),
                },
            });
        }
        data.Add(new ChartGraphNode
        {
            Id = "对象", Name = "对象", Value = "资源\n数量", CategoryIndex = 3,
        });

        // 2. 使用普通 Graph 的后台 force；不提交目前尚未消费的 radius / 过程动画字段。
        return new ChartOption
        {
            Background = ChartBrush.Solid(SKColors.White),
            Animation = new() { Enabled = false },
            Color = { ChartBrush.Solid(PropertyColor), ChartBrush.Solid(RelatedColor) },
            Tooltip = new() { Trigger = ChartTooltipTrigger.Item, Formatter = "{b}", Confine = true },
            Series = new ChartGraphSeriesOption
            {
                Id = "resource-relations",
                Name = "知识图谱",
                Layout = ChartGraphLayout.Force,
                DataSource = data,
                Categories =
                [
                    new() { Name = "对象属性" },
                    new() { Name = "关联对象" },
                    new() { Name = "所属类别" },
                    new() { Name = "对象" },
                ],
                Symbol = "circle",
                SymbolSize = NodeSize,
                Roam = true,
                Draggable = true,
                NodeScaleRatio = 0,
                FocusNodeAdjacency = false,
                Force = new()
                {
                    Repulsion = 100,
                    Gravity = 0.02,
                    // 同时拉开节点间距，放大整张关系图，并给数量标签留出空间。
                    EdgeLength = NodeSize + 80,
                    Iterations = 240,
                    Seed = 7,
                },
                Label = new()
                {
                    Show = true,
                    Position = "inside",
                    Color = ChartBrush.Solid(SKColors.White),
                    FontSize = 22,
                    FormatterCallback = context => context.PrimaryValue.AsString(context.Culture) ?? context.Name ?? "",
                },
                ItemStyle = new()
                {
                    BorderColor = ChartBrush.Solid(SKColors.White),
                    BorderWidth = 4,
                    ShadowBlur = 12,
                    ShadowColor = ChartBrush.Solid(new SKColor(98, 198, 248, 128)),
                },
                LineStyle = new() { Opacity = 0.5f, Width = 2 },
                Curveness = 0,
                EdgeLabel = new()
                {
                    Show = true, FontSize = 14, Formatter = "{c}",
                    Color = ChartBrush.Solid(new SKColor(85, 85, 85)),
                },
            },
        };
    }
}
