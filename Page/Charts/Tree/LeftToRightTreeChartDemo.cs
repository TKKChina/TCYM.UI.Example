using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Tree;

/// <summary>从左到右树状图；完整 Options 和数据均在本类中创建。</summary>
internal sealed class LeftToRightTreeChartDemo : TreeChartDemoPage
{
    internal LeftToRightTreeChartDemo() : base(
        "从左到右树状图",
        "展示 ROOT、ALPHA、BETA、GAMMA 的层级关系，以及最深层的 DEEP。",
        "蓝色分支配深蓝标签，橙色叶子配浅黄色标签；采用直角折线，在父子节点中间分叉。初始全部展开，点击分支可折叠/展开。",
        CreateOption()) { }

    private static ChartOption CreateOption()
    {
        // 本类保留完整原始数据，便于单独复制这个案例。
        var root = new ChartTreeNode
        {
            Id = "root", Name = "ROOT",
            Children =
            {
                new ChartTreeNode
                {
                    Id = "alpha", Name = "ALPHA",
                    Children =
                    {
                        new ChartTreeNode { Id = "a-one", Name = "A ONE" },
                        new ChartTreeNode { Id = "a-two", Name = "A TWO" },
                    },
                },
                new ChartTreeNode
                {
                    Id = "beta", Name = "BETA",
                    Children =
                    {
                        new ChartTreeNode
                        {
                            Id = "b-one", Name = "B ONE",
                            Children = { new ChartTreeNode { Id = "deep", Name = "DEEP" } },
                        },
                        new ChartTreeNode { Id = "b-two", Name = "B TWO" },
                    },
                },
                new ChartTreeNode { Id = "gamma", Name = "GAMMA" },
            },
        };
        return new ChartOption
        {
            Background = ChartBrush.Solid(SKColors.White),
            Animation = new() { Enabled = true },
            Tooltip = new() { Trigger = ChartTooltipTrigger.Item, Formatter = "{b}", Confine = true },
            Series = new ChartTreeSeriesOption
            {
                Id = "tree-lr", Name = "从左到右树状图",
                DataSource = new ChartTreeData().Add(root),
                Layout = ChartTreeLayout.Orthogonal,
                Orient = ChartTreeOrient.LR,
                EdgeShape = ChartTreeEdgeShape.Polyline,
                EdgeForkPosition = ChartLength.Percent(50),
                InitialTreeDepth = -1,
                ExpandAndCollapse = true,
                Roam = true,
                Left = ChartLength.Pixels(80), Right = ChartLength.Pixels(120),
                Top = ChartLength.Pixels(65), Bottom = ChartLength.Pixels(65),
                Symbol = "circle", SymbolSize = 22,
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(new SKColor(40, 111, 199)),
                    BorderColor = ChartBrush.Solid(SKColors.White), BorderWidth = 2,
                },
                LineStyle = new() { Color = ChartBrush.Solid(new SKColor(116, 143, 172)), Width = 2 },
                Label = new()
                {
                    Show = true, Position = "top", Distance = 10, FontSize = 16,
                    Color = ChartBrush.Solid(new SKColor(35, 49, 68)),
                    Formatter = "{branch|{b}}",
                    Rich = new()
                    {
                        ["branch"] = new ChartTextStyle
                        {
                            FontSize = 15, Color = ChartBrush.Solid(SKColors.White),
                            BackgroundColor = ChartBrush.Solid(new SKColor(38, 70, 116)),
                            Padding = new ChartInsets(4, 6, 4, 6), BorderRadius = 4,
                        },
                    },
                },
                Leaves = new()
                {
                    ItemStyle = new() { Color = ChartBrush.Solid(new SKColor(224, 135, 39)) },
                    Label = new()
                    {
                        Show = true, Position = "right", Distance = 10, FontSize = 16,
                        Color = ChartBrush.Solid(new SKColor(35, 49, 68)),
                        Formatter = "{leaf|{b}}",
                        Rich = new()
                        {
                            ["leaf"] = new ChartTextStyle
                            {
                                FontSize = 15, Color = ChartBrush.Solid(new SKColor(79, 49, 11)),
                                BackgroundColor = ChartBrush.Solid(new SKColor(255, 231, 188)),
                                Padding = new ChartInsets(3, 6, 3, 6), BorderRadius = 3,
                            },
                        },
                    },
                },
            },
        };
    }
}
