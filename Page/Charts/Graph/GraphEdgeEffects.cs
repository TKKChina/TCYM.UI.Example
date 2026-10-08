using SkiaSharp;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Interaction;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Chart.Series;

namespace Page.Charts.Graph;

/// <summary>
/// 示例专用的边装饰：复用原生 Graph 的已呈现路径，补充箭头和流动标记。
/// 不读取私有字段，不修改节点/边数据，不声明框架已原生支持 ECharts lines。
/// </summary>
internal static class GraphEdgeEffects
{
    internal sealed record Flow(double Period = 6, float Width = 4, float Length = 12);

    internal static ChartRegistry CreateRegistry(bool arrows = false, IReadOnlyDictionary<string, Flow>? flows = null)
    {
        var registry = ChartModules.CreateDefaultRegistry();
        registry.RegisterSeriesView(new ChartGraphSeriesViewFactory((context, option, root) =>
        {
            var overlay = root.Children.OfType<Overlay>().FirstOrDefault();
            if (overlay is null)
            {
                overlay = new Overlay(root, arrows, flows) { Id = "graph-demo-edge-effects", Silent = true, Z2 = 5 };
                root.Add(overlay);
            }
            overlay.DefaultNodeSize = option.SymbolSize ?? ChartSymbolSize.Square(16);
            overlay.ClipRect = context.Viewport;
            overlay.Task?.Cancel();
            if (flows is null || flows.Count == 0 || option.Show == false) return;
            var commands = context.Commands;
            overlay.Task = context.FrameScheduler.Schedule(elapsed =>
            {
                if (overlay.Disposed || !root.Visible) return false;
                overlay.Seconds = elapsed.TotalSeconds;
                commands.RequestRender();
                return true;
            });
        }), replace: true);
        return registry;
    }

    private sealed class Overlay(ChartGroup root, bool arrows, IReadOnlyDictionary<string, Flow>? flows) : ChartElement
    {
        internal double Seconds;
        internal ChartFrameTask? Task;
        internal bool Disposed;
        internal ChartSymbolSize DefaultNodeSize;
        protected override void DisposeCore() { Disposed = true; Task?.Cancel(); base.DisposeCore(); }
        protected override bool Contains(float localX, float localY) => false;

        protected override void RenderCore(SKCanvas canvas, ChartStyle style, float inheritedOpacity)
        {
            var elements = Enumerate(root).ToArray();
            var nodes = elements.Where(e => e.Metadata?.DataType == "node").ToArray();
            var byIndex = nodes.ToDictionary(e => e.Metadata!.DataIndex);
            var byId = new Dictionary<string, ChartElement>(StringComparer.Ordinal);
            foreach (var node in nodes)
            {
                if (node.Metadata?.Payload is not ChartGraphNodeSnapshot item) continue;
                if (item.Id is { } id) byId.TryAdd(id, node);
                if (item.Name is { } name) byId.TryAdd(name, node);
            }
            using var paint = new SKPaint { IsAntialias = true };
            foreach (var edge in elements.OfType<ChartPathElement>())
            {
                if (edge.Metadata is not { DataType: "edge", Payload: ChartGraphEdgeSnapshot data }) continue;
                using var measure = new SKPathMeasure(edge.Path, false);
                if (measure.Length <= 0) continue;
                var start = Radius(Resolve(data.Source), DefaultNodeSize);
                var end = measure.Length - Radius(Resolve(data.Target), DefaultNodeSize);
                if (end <= start) continue;
                var edgeStyle = CurrentStyle(edge);
                var color = (edgeStyle.Stroke as ChartSolidBrush)?.Color ?? SKColors.White;
                paint.Color = color.WithAlpha((byte)Math.Clamp((int)(color.Alpha * edgeStyle.Opacity * inheritedOpacity), 0, 255));
                if (arrows)
                {
                    if (measure.GetPositionAndTangent(start, out var source, out _)) canvas.DrawCircle(source, 2, paint);
                    if (measure.GetPositionAndTangent(end, out var target, out var direction))
                        Mark(canvas, paint, target, direction, 8, 8, pin: false);
                }
                if (data.Id is { } key && flows?.TryGetValue(key, out var flow) == true)
                {
                    var progress = Math.Max(0, Seconds - 0.002) % flow.Period / flow.Period;
                    var distance = start + (end - start) * (float)progress;
                    if (measure.GetPositionAndTangent(distance, out var point, out var tangent))
                        Mark(canvas, paint, point, tangent, flow.Length, flow.Width, pin: true);
                }
            }

            ChartElement? Resolve(ChartGraphEndpoint endpoint) =>
                endpoint.Index is { } index ? byIndex.GetValueOrDefault(index)
                : endpoint.Id is { } id ? byId.GetValueOrDefault(id) : null;
        }

        private static float Radius(ChartElement? element, ChartSymbolSize defaultSize)
        {
            if (element?.Metadata?.Payload is not ChartGraphNodeSnapshot node || node.Symbol == "none") return 0;
            var size = node.SymbolSize ?? defaultSize;
            return Math.Max(size.Width, size.Height) * element.ScaleX / 2 + 2;
        }

        private static ChartStyle CurrentStyle(ChartElement element)
        {
            var style = element.Style;
            if ((element.InteractionState & ChartInteractionState.Blur) != 0 && element.BlurStyle is { } blur)
                style = blur.MergeOnto(style);
            if ((element.InteractionState & ChartInteractionState.Select) != 0 && element.SelectStyle is { } select)
                style = select.MergeOnto(style);
            if ((element.InteractionState & ChartInteractionState.Emphasis) != 0 && element.EmphasisStyle is { } emphasis)
                style = emphasis.MergeOnto(style);
            return style;
        }

        private static IEnumerable<ChartElement> Enumerate(ChartGroup group)
        {
            foreach (var child in group.Children)
            {
                if (!child.Visible || child is Overlay) continue;
                if (child.Metadata?.SeriesType == "graph") yield return child;
                if (child is ChartGroup nested)
                    foreach (var descendant in Enumerate(nested)) yield return descendant;
            }
        }

        private static void Mark(SKCanvas canvas, SKPaint paint, SKPoint point, SKPoint tangent,
            float length, float width, bool pin)
        {
            canvas.Save();
            canvas.Translate(point.X, point.Y);
            canvas.RotateDegrees(MathF.Atan2(tangent.Y, tangent.X) * 180 / MathF.PI);
            using var path = new SKPathBuilder();
            path.MoveTo(0, 0);
            if (pin)
            {
                path.QuadTo(-length, -width, -length, 0);
                path.QuadTo(-length, width, 0, 0);
            }
            else
            {
                path.LineTo(-length, -width / 2);
                path.LineTo(-length, width / 2);
            }
            path.Close();
            using var shape = path.Detach();
            canvas.DrawPath(shape, paint);
            canvas.Restore();
        }
    }
}
