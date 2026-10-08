using SkiaSharp;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Radar
{
    /// <summary>圆形虚线网格中的两个年度多维数据对比。</summary>
    internal sealed class MultidimensionalRadarChartDemo : RadarChartDemoPage
    {
        private static readonly SKColor Green = new(86, 199, 60);
        private static readonly SKColor Blue = new(0, 183, 238);

        internal MultidimensionalRadarChartDemo()
            : base(
                "多维雷达图",
                "对比 2016 年、2017 年在六个指标上的数值。",
                "两个年度共用最大值为 88 的圆形网格；面积保持半透明，重叠时仍可看到另一年度和网格。数据从中心展开，右侧图例可单独隐藏某一年。",
                CreateOption())
        {
        }

        private static ChartOption CreateOption() => new()
        {
            Background = ChartBrush.Solid(SKColors.White),
            Animation = new()
            {
                Enabled = true, Duration = 1000, UpdateDuration = 600,
                Easing = "cubicOut", UpdateEasing = "cubicInOut",
            },
            Tooltip = new ChartTooltipOption { Show = true, Trigger = ChartTooltipTrigger.Item, Confine = true },
            Legend = new ChartLegendOption
            {
                Show = true, Icon = "circle",
                Right = "20%", Top = "center", Orient = ChartLegendOrient.Vertical,
                ItemGap = 30f, Data = { "2016年", "2017年" },
                TextStyle = new() { FontSize = 15f, Color = ChartBrush.Solid(SKColors.Black) },
            },
            Radar = new ChartRadarOption
            {
                Id = "year-radar",
                Center = new ChartPolarCenter(ChartLength.Percent(35f), ChartLength.Percent(50f)),
                Radius = ChartPolarRadius.FromOuter(ChartLength.Percent(70f)),
                StartAngle = 90, SplitNumber = 4, Shape = ChartRadarShape.Circle,
                Indicator =
                [
                    new() { Name = "三无、五保", Max = 88 },
                    new() { Name = "低保", Max = 88 },
                    new() { Name = "残疾", Max = 88 },
                    new() { Name = "失独", Max = 88 },
                    new() { Name = "高龄", Max = 88 },
                    new() { Name = "突出贡献", Max = 88 },
                ],
                AxisName = new() { FontSize = 14f, Color = ChartBrush.Solid(SKColors.Black) },
                AxisLabel = new() { Show = false },
                SplitArea = new() { AreaStyle = new() { Color = { ChartBrush.Solid(SKColors.Transparent) } } },
                AxisLine = new()
                {
                    Show = true,
                    LineStyle = new() { Type = ChartAxisLineType.Dashed, Color = ChartBrush.Solid(SKColors.Black) },
                },
                SplitLine = new()
                {
                    Show = true,
                    LineStyle = new() { Type = ChartAxisLineType.Dashed, Color = ChartBrush.Solid(SKColors.Black) },
                },
            },
            Series =
            [
                new ChartRadarSeriesOption
                {
                    Id = "year-2016", Name = "2016年", RadarId = "year-radar",
                    Symbol = "circle", SymbolSize = 10f,
                    ItemStyle = new()
                    {
                        Color = ChartBrush.Solid(Green),
                        BorderColor = ChartBrush.Solid(Green.WithAlpha(77)), BorderWidth = 10f,
                    },
                    LineStyle = new() { Color = ChartBrush.Solid(Green), Width = 2f },
                    // 原颜色 alpha=0.4，再乘面积 opacity=0.7，最终透明度为 0.28。
                    AreaStyle = new() { Color = ChartBrush.Solid(Green.WithAlpha(102)), Opacity = 0.7f },
                    DataSource = new ChartRadarData().Add(new ChartRadarDataItem([80, 50, 55, 80, 50, 80])
                    {
                        Key = "2016", Name = "2016年",
                    }),
                },
                new ChartRadarSeriesOption
                {
                    Id = "year-2017", Name = "2017年", RadarId = "year-radar",
                    Symbol = "circle", SymbolSize = 10f,
                    ItemStyle = new()
                    {
                        Color = ChartBrush.Solid(Blue),
                        BorderColor = ChartBrush.Solid(Blue.WithAlpha(102)), BorderWidth = 10f,
                    },
                    LineStyle = new() { Color = ChartBrush.Solid(Blue), Width = 2f },
                    AreaStyle = new() { Color = ChartBrush.Solid(Blue), Opacity = 0.7f },
                    DataSource = new ChartRadarData().Add(new ChartRadarDataItem([60, 60, 65, 60, 70, 40])
                    {
                        Key = "2017", Name = "2017年",
                    }),
                },
            ],
        };
    }
}
