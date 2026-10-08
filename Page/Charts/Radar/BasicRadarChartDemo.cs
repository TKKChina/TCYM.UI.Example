using SkiaSharp;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Radar
{
    /// <summary>六个独立指标上的预算与实际支出对比。</summary>
    internal sealed class BasicRadarChartDemo : RadarChartDemoPage
    {
        internal BasicRadarChartDemo()
            : base(
                "基础雷达图",
                "使用六边形网格对比 Allocated Budget 与 Actual Spending。",
                "每个 Indicator 都有自己的最大值；一项 Radar 数据就是按指标顺序排列的完整向量。图例对应两项预算数据，悬停可查看六个维度的原始数值。",
                CreateOption())
        {
        }

        private static ChartOption CreateOption()
        {
            // 数据中的六个值依次对应下面的六个 Indicator。
            var data = new ChartRadarData()
                .Add(new ChartRadarDataItem([4200, 3000, 20000, 35000, 50000, 18000])
                {
                    Key = "allocated-budget", Name = "Allocated Budget",
                })
                .Add(new ChartRadarDataItem([5000, 14000, 28000, 26000, 42000, 21000])
                {
                    Key = "actual-spending", Name = "Actual Spending",
                });

            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true, Duration = 1000, UpdateDuration = 600,
                    Easing = "cubicOut", UpdateEasing = "cubicInOut",
                },
                Title = new ChartTitleOption
                {
                    Text = "Basic Radar Chart", Left = "12px", Top = "12px",
                    TextStyle = new() { FontSize = 18f },
                },
                Legend = new ChartLegendOption
                {
                    Left = "center", Top = "48px",
                    Data = { "Allocated Budget", "Actual Spending" },
                },
                Tooltip = new ChartTooltipOption
                {
                    Trigger = ChartTooltipTrigger.Item, Confine = true,
                },
                Radar = new ChartRadarOption
                {
                    Id = "basic-radar",
                    Center = new ChartPolarCenter(ChartLength.Percent(50f), ChartLength.Percent(55f)),
                    Radius = ChartPolarRadius.FromOuter(ChartLength.Percent(65f)),
                    Shape = ChartRadarShape.Polygon,
                    AxisName = new() { FontSize = 13f },
                    Indicator =
                    [
                        new() { Name = "Sales", Max = 6500 },
                        new() { Name = "Administration", Max = 16000 },
                        new() { Name = "Information Technology", Max = 30000 },
                        new() { Name = "Customer Support", Max = 38000 },
                        new() { Name = "Development", Max = 52000 },
                        new() { Name = "Marketing", Max = 25000 },
                    ],
                },
                Series = new ChartRadarSeriesOption
                {
                    Id = "budget-spending", Name = "Budget vs spending",
                    RadarId = "basic-radar",
                    ColorBy = ChartRadarColorBy.Data,
                    Symbol = "circle", SymbolSize = 5f,
                    DataSource = data,
                },
            };
        }
    }
}
