using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Chart.Text;

namespace Page.Charts.Gauge
{
    /// <summary>展示带圆帽进度弧、自定义指针和富文本速度值的半圆仪表盘。</summary>
    internal sealed class SpeedGaugeChartDemo : GaugeChartDemoPage
    {
        /// <summary>速度仪表盘用于增量匹配和场景复用的稳定系列标识。</summary>
        private const string SeriesId = "speed-gauge";

        /// <summary>速度数据项用于动画匹配的稳定标识。</summary>
        private const string DataKey = "speed-gauge-value";

        /// <summary>复刻 ECharts 示例中窄长针形指针的 SVG Path。</summary>
        private const string PointerIcon =
            "path://M2090.36389,615.30999 L2090.36389,615.30999 " +
            "C2091.48372,615.30999 2092.40383,616.194028 " +
            "2092.44859,617.312956 L2096.90698,728.755929 " +
            "C2097.05155,732.369577 2094.2393,735.416212 " +
            "2090.62566,735.56078 C2090.53845,735.564269 " +
            "2090.45117,735.566014 2090.36389,735.566014 " +
            "C2086.74736,735.566014 2083.81557,732.63423 " +
            "2083.81557,729.017692 C2083.81557,728.930412 " +
            "2083.81732,728.84314 2083.82081,728.755929 " +
            "L2088.2792,617.312956 C2088.32396,616.194028 " +
            "2089.24407,615.30999 2090.36389,615.30999 Z";

        /// <summary>创建速度仪表盘示例页面。</summary>
        internal SpeedGaugeChartDemo()
            : base(
                "速度仪表盘",
                "使用半圆刻度、圆帽进度弧和自定义 Path 指针显示车辆速度。",
                "量程为 0～240 km/h；Detail.FormatterCallback 将数值和单位拆成两个 Rich 样式，方便分别控制字号、粗细和颜色。固定 490px 宽屏预览使用 130% 半径和 66% 圆心，以展开标签和 Detail。",
                CreateOption())
        {
        }

        /// <summary>创建速度仪表盘的完整配置。</summary>
        /// <returns>包含刻度、进度、指针、详情和稳定数据标识的配置。</returns>
        private static ChartOption CreateOption()
        {
            var accent = ChartBrush.Solid(new SKColor(88, 217, 249));
            var scaleColor = ChartBrush.Solid(new SKColor(153, 153, 153));

            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 800,
                    UpdateDuration = 500,
                    UpdateEasing = "cubicOut",
                },
                Series =
                [
                    new ChartGaugeSeriesOption
                    {
                        Id = SeriesId,
                        Name = "速度",
                        Center = ChartGaugeCenter.Percent(50f, 66f),
                        Radius = ChartLength.Percent(130f),
                        StartAngle = 180f,
                        EndAngle = 0f,
                        Minimum = 0d,
                        Maximum = 240d,
                        SplitNumber = 12,
                        AxisLine = new()
                        {
                            RoundCap = true,
                            LineStyle = new()
                            {
                                Width = 18f,
                            },
                        },
                        Progress = new()
                        {
                            Show = true,
                            RoundCap = true,
                            Width = 18f,
                            ItemStyle = new()
                            {
                                Color = accent,
                            },
                        },
                        Pointer = new()
                        {
                            Icon = PointerIcon,
                            KeepAspect = true,
                            Length = ChartLength.Percent(75f),
                            Width = 16f,
                            OffsetCenter = ChartGaugeCenter.Percent(0f, 5f),
                            ItemStyle = new()
                            {
                                Color = accent,
                                ShadowColor = ChartBrush.Solid(new SKColor(0, 138, 255, 115)),
                                ShadowBlur = 10f,
                                ShadowOffsetX = 2f,
                                ShadowOffsetY = 2f,
                            },
                        },
                        AxisTick = new()
                        {
                            SplitNumber = 2,
                            LineStyle = new()
                            {
                                Width = 2f,
                                Color = scaleColor,
                            },
                        },
                        SplitLine = new()
                        {
                            Length = ChartLength.Pixels(12f),
                            LineStyle = new()
                            {
                                Width = 3f,
                                Color = scaleColor,
                            },
                        },
                        AxisLabel = new()
                        {
                            Distance = 30f,
                            Color = scaleColor,
                            FontSize = 20f,
                        },
                        Title = new()
                        {
                            Show = false,
                        },
                        Detail = new()
                        {
                            BackgroundColor = ChartBrush.Solid(SKColors.White),
                            Width = ChartLength.Percent(60f),
                            Height = ChartLength.Pixels(40f),
                            LineHeight = 40f,
                            OffsetCenter = ChartGaugeCenter.Percent(0f, 0f),
                            ValueAnimation = true,
                            FormatterCallback = static value => $"{{value|{value:F0}}}{{unit|km/h}}",
                            Rich = new()
                            {
                                ["value"] = new ChartTextStyle
                                {
                                    FontSize = 50f,
                                    FontWeight = 700,
                                    Color = ChartBrush.Solid(new SKColor(119, 119, 119)),
                                },
                                ["unit"] = new ChartTextStyle
                                {
                                    FontSize = 20f,
                                    Color = scaleColor,
                                    Padding = new ChartInsets(left: 10f, top: 10f, right: 0f, bottom: 0f),
                                },
                            },
                        },
                        DataSource = CreateData(100d),
                    },
                ],
            };
        }

        /// <summary>创建带稳定 Key 的单项速度数据。</summary>
        /// <param name="value">当前速度，单位为 km/h。</param>
        /// <returns>可以直接赋给 Gauge 系列的数据源。</returns>
        private static ChartGaugeData CreateData(double value)
        {
            return new ChartGaugeData().Add(new ChartGaugeDataItem(value, "速度")
            {
                Id = DataKey,
                Key = DataKey,
            });
        }
    }
}
