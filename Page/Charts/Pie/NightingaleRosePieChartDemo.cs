using SkiaSharp;
using TCYM.UI.Chart.Components.Legend;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Pie
{
    /// <summary>展示等角度、按数值映射半径的南丁格尔玫瑰图。</summary>
    internal sealed class NightingaleRosePieChartDemo : PieChartDemoPage
    {
        /// <summary>创建南丁格尔玫瑰图页面。</summary>
        internal NightingaleRosePieChartDemo()
            : base(
                "基础南丁格尔玫瑰图",
                "每个扇区使用相同角度，数值越大，扇区半径越长。",
                "RoseType.Area 负责等角面积布局；Radius 同时使用像素内外半径，圆角由 itemStyle.borderRadius 控制。固定预览高度下将官网 250px 外半径收敛为 190px，避免图例和标签被裁剪。",
                CreateOption())
        {
        }

        /// <summary>创建南丁格尔玫瑰图的完整配置。</summary>
        /// <returns>包含底部图例和八个玫瑰扇区的配置。</returns>
        private static ChartOption CreateOption()
        {
            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 800,
                    UpdateDuration = 500,
                },
                Legend = new ChartLegendOption
                {
                    Id = "nightingale-legend",
                    Show = true,
                    Left = ChartLength.FromKeyword("center"),
                    Bottom = "8px",
                    Orient = ChartLegendOrient.Horizontal,
                    ItemGap = 18f,
                },
                Series =
                [
                    new ChartPieSeriesOption
                    {
                        Id = "nightingale-rose",
                        Name = "Nightingale Chart",
                        Center = ChartPieCenter.Percent(50f, 47f),
                        Radius = new ChartPieRadius(
                            ChartLength.Pixels(50f),
                            ChartLength.Pixels(190f)),
                        RoseType = ChartPieRoseType.Area,
                        ItemStyle = new()
                        {
                            BorderRadius = 8f,
                            BorderColor = ChartBrush.Solid(SKColors.White),
                            BorderWidth = 2f,
                        },
                        Label = new()
                        {
                            Show = true,
                            Position = ChartPieLabelPosition.Outer,
                            Formatter = "{b}: {c}",
                            FontSize = 12f,
                            Color = ChartBrush.Solid(new SKColor(67, 74, 89)),
                        },
                        DataSource = CreateData(),
                    },
                ],
            };
        }

        /// <summary>创建八个带稳定 Key 的玫瑰扇区数据。</summary>
        /// <returns>从 rose 1 到 rose 8 的完整 Pie 数据。</returns>
        private static ChartPieData CreateData()
        {
            var values = new[] { 40d, 38d, 32d, 30d, 28d, 26d, 22d, 18d };
            var data = new ChartPieData();
            for (var index = 0; index < values.Length; index++)
            {
                var name = $"rose {index + 1}";
                data.Add(new ChartPieDataItem(values[index], name)
                {
                    Key = $"nightingale-{index + 1}",
                });
            }

            return data;
        }
    }
}
