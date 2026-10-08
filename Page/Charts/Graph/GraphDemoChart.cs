using SkiaSharp;
using TCYM.UI.Chart;
using TCYM.UI.Chart.Core;

namespace Page.Charts.Graph;

/// <summary>在图表绘制前通知逻辑内容区尺寸变化，供各案例调整自己的布局。</summary>
internal sealed class GraphDemoChart(ChartRegistry registry) : UIChart(registry)
{
    internal SKSize ViewportSize { get; private set; }
    internal event Action<SKSize>? ViewportChanged;

    protected override void RenderContent(SKCanvas canvas)
    {
        if (!Engine.IsDisposed)
        {
            var inset = MathF.Max(0, BorderWidth);
            var size = new SKSize(
                MathF.Max(0, Width - inset * 2 - PaddingLeft - PaddingRight) / EffectiveLayoutDpiScale,
                MathF.Max(0, Height - inset * 2 - PaddingTop - PaddingBottom) / EffectiveLayoutDpiScale);
            if (size.Width > 0 && size.Height > 0 && size != ViewportSize)
            {
                ViewportSize = size;
                ViewportChanged?.Invoke(size);
            }
        }
        base.RenderContent(canvas);
    }
}
