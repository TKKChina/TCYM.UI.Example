using SkiaSharp;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Graph;

/// <summary>每 200ms 添加一个节点，后台 force 重新计算，离页自动取消。</summary>
internal sealed class DynamicNodesGraphDemo : GraphChartDemoPage
{
    private const int MaximumNodes = 300;
    private ChartGraphData _data;
    private Random _random = new(20260911);
    private bool _paused;
    private SKSize _layoutSize;
    protected override TimeSpan? UpdateInterval => TimeSpan.FromMilliseconds(200);

    internal DynamicNodesGraphDemo() : base(
        "动态增加图节点", "每 200ms 加入一个节点，并随机连接两个不同节点。",
        "中心节点固定；force 在后台执行，已有节点延续当前位置。可拖动、平移、缩放；到 300 个节点暂停增长，可点击重置，离页停止更新。",
        CreateOption(out var data))
    {
        _data = data;
        AddButton("暂停 / 继续", () => _paused = !_paused);
        AddButton("重置", () =>
        {
            _random = new Random(20260911);
            _paused = false;
            var option = CreateOption(out _data);
            _data.Nodes[0].X = _layoutSize.Width / 2;
            _data.Nodes[0].Y = _layoutSize.Height / 2;
            Chart.SetOption(option, new ChartSetOptionOptions { NotMerge = true });
            Status.Text = "节点：1　连线：0";
        });
        Status.Text = "节点：1　连线：0";
    }

    private static ChartOption CreateOption(out ChartGraphData data)
    {
        data = new ChartGraphData().Add(new ChartGraphNode
        {
            Id = "-1", Name = "固定中心", Fixed = true, SymbolSize = 20, Value = 0,
        });
        return new ChartOption
        {
            Animation = new() { Enabled = false },
            Background = ChartBrush.Solid(SKColors.White),
            Tooltip = new() { Trigger = ChartTooltipTrigger.Item, Confine = true },
            Series = new ChartGraphSeriesOption
            {
                Id = "growing-graph", Name = "动态节点", Layout = ChartGraphLayout.Force,
                Roam = true, Draggable = true, SymbolSize = 10,
                DataSource = data,
                Force = new() { Repulsion = 100, EdgeLength = 5, Gravity = 0.1, Iterations = 100, Seed = 1 },
                ItemStyle = new() { Color = ChartBrush.Solid(new SKColor(80, 112, 221)) },
                LineStyle = new() { Color = ChartBrush.Solid(new SKColor(145, 160, 183)), Opacity = 0.65f },
            },
        };
    }

    protected override bool UpdateDemo()
    {
        if (_paused || _data.Count >= MaximumNodes) return true;
        var index = _data.Count;
        _data.Add(new ChartGraphNode
        {
            Id = index.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Name = "节点 " + index, Value = index,
            X = _layoutSize.Width / 2 + _random.Next(-100, 101),
            Y = _layoutSize.Height / 2 + _random.Next(-100, 101),
        });
        var source = _random.Next(_data.Count);
        var target = _random.Next(_data.Count);
        if (source != target)
            _data.AddLink(new ChartGraphEdge(source, target) { Id = "edge-" + index, Name = "随机连接" });

        // Graph 拓扑统一提交。保留稳定 Id，后续更新不会重建 UIChart 控件。
        Chart.SetOption(new ChartOption
        {
            Series = new ChartGraphSeriesOption { Id = "growing-graph", DataSource = _data },
        });
        Status.Text = $"节点：{_data.Count}　连线：{_data.Links.Count}" +
            (_data.Count == MaximumNodes ? "　已到演示上限，点击重置可重新开始。" : "");
        return true;
    }

    protected override void OnViewportChanged(SKSize size)
    {
        var dx = (size.Width - _layoutSize.Width) / 2;
        var dy = (size.Height - _layoutSize.Height) / 2;
        foreach (var node in _data.Nodes)
        {
            node.X = (node.X ?? _layoutSize.Width / 2) + dx;
            node.Y = (node.Y ?? _layoutSize.Height / 2) + dy;
        }
        _layoutSize = size;
        Chart.SetOption(new ChartOption
        {
            Series = new ChartGraphSeriesOption { Id = "growing-graph", DataSource = _data },
        });
    }
}
