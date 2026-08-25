using SkiaSharp;
using TCYM.UI.Chart.Components.Legend;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Pie
{
    /// <summary>展示通过 startAngle 与 endAngle 限制到半周范围的环形图。</summary>
    internal sealed class HalfDonutPieChartDemo : PieChartDemoPage
    {
        /// <summary>创建半环形图页面。</summary>
        internal HalfDonutPieChartDemo()
            : base(
                "半环形图",
                "把完整环形图限制在 180° 到 360° 的半周范围。",
                "Center 下移到画布 70% 高度，StartAngle=180、EndAngle=360 只绘制上半环；Tooltip 和 Legend 仍使用原始五项数据。",
                CreateOption())
        {
        }

        /// <summary>创建半环形图的完整配置。</summary>
        /// <returns>包含半周角度范围、图例和 Tooltip 的配置。</returns>
        private static ChartOption CreateOption()
        {
            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 750,
                    UpdateDuration = 450,
                },
                Tooltip = new ChartTooltipOption
                {
                    Id = "half-donut-tooltip",
                    Trigger = ChartTooltipTrigger.Item,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                },
                Legend = new ChartLegendOption
                {
                    Id = "half-donut-legend",
                    Show = true,
                    Top = "5%",
                    Left = ChartLength.FromKeyword("center"),
                    Orient = ChartLegendOrient.Horizontal,
                    ItemGap = 18f,
                },
                Series =
                [
                    new ChartPieSeriesOption
                    {
                        Id = "half-donut",
                        Name = "Access From",
                        Radius = new ChartPieRadius(
                            ChartLength.Percent(40f),
                            ChartLength.Percent(70f)),
                        Center = ChartPieCenter.Percent(50f, 70f),
                        StartAngle = 180f,
                        EndAngle = 360f,
                        Label = new()
                        {
                            Show = true,
                            Position = ChartPieLabelPosition.Outer,
                            Formatter = "{b}: {d}%",
                            FontSize = 12f,
                        },
                        DataSource = CreateData(),
                    },
                ],
            };
        }

        /// <summary>创建五个访问来源数据项。</summary>
        /// <returns>带稳定 Key 的半环形图数据。</returns>
        private static ChartPieData CreateData()
        {
            var values = new (string Name, double Value)[]
            {
                ("Search Engine", 1048d),
                ("Direct", 735d),
                ("Email", 580d),
                ("Union Ads", 484d),
                ("Video Ads", 300d),
            };

            var data = new ChartPieData();
            for (var index = 0; index < values.Length; index++)
            {
                var (name, value) = values[index];
                data.Add(new ChartPieDataItem(value, name)
                {
                    Key = $"half-donut-{index}",
                });
            }

            return data;
        }
    }
}
