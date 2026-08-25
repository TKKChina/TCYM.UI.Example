using SkiaSharp;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Lines
{
    /// <summary>展示沿 X 方向保持稳定趋势的平滑折线图。</summary>
    internal sealed class SmoothLineChartDemo : LineChartDemoPage
    {
        /// <summary>平滑折线图使用的蓝色。</summary>
        private static readonly SKColor LineColor = new(22, 96, 235);

        /// <summary>创建平滑折线图示例页面。</summary>
        internal SmoothLineChartDemo()
            : base(
                "平滑折线图",
                "用 Cubic 平滑路径连接全天数据节点。",
                "平滑曲线保留原始节点值，同时通过 X 方向单调约束减少不必要的回折，适合强调总体趋势。",
                CreateOption())
        {
        }

        /// <summary>创建平滑折线图的完整配置。</summary>
        /// <returns>包含自动平滑和 X 方向单调约束的图表配置。</returns>
        private static ChartOption CreateOption()
        {
            var option = new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = false,
                },
                Grid = new ChartGridOption
                {
                    Show = true,
                    Left = "10px",
                    Top = "24px",
                    Right = "24px",
                    Bottom = "30px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                    BackgroundColor = ChartBrush.Solid(SKColors.White),
                },
                // 单 Grid、单 X/Y 轴时默认按索引 0 绑定，不需要再声明关联 ID。
                XAxis = new ChartXAxisOption
                {
                    Name = "时间",
                    NameLocation = ChartAxisNameLocation.Middle,
                    NameGap = 38f,
                    Type = ChartAxisType.Category,
                    BoundaryGap = false,
                    Data =
                    [
                        "00:00", "02:00", "04:00", "06:00", "08:00", "10:00", "12:00",
                        "14:00", "16:00", "18:00", "20:00", "22:00", "24:00",
                    ],
                    AxisLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(145, 153, 166)),
                            Width = 1f,
                        },
                    },
                    AxisTick = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(145, 153, 166)),
                        },
                    },
                    AxisLabel = new()
                    {
                        Show = true,
                        Color = ChartBrush.Solid(new SKColor(45, 52, 65)),
                        FontSize = 14f,
                        Interval = 0,
                        HideOverlap = false,
                    },
                    NameTextStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(45, 52, 65)),
                        FontSize = 14f,
                    },
                    SplitLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(224, 230, 239)),
                            Type = ChartAxisLineType.Dashed,
                            Width = 1f,
                        },
                    },
                },
                YAxis = new ChartYAxisOption
                {
                    Name = "数值",
                    NameLocation = ChartAxisNameLocation.Start,
                    NameGap = 12f,
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(0d),
                    Maximum = ChartAxisBound.Fixed(120d),
                    Interval = 20d,
                    Scale = true,
                    AxisLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(145, 153, 166)),
                            Width = 1f,
                        },
                    },
                    AxisTick = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(145, 153, 166)),
                        },
                    },
                    AxisLabel = new()
                    {
                        Show = true,
                        Color = ChartBrush.Solid(new SKColor(45, 52, 65)),
                        FontSize = 14f,
                    },
                    NameTextStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(45, 52, 65)),
                        FontSize = 14f,
                    },
                    SplitLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(224, 230, 239)),
                            Type = ChartAxisLineType.Dashed,
                            Width = 1f,
                        },
                    },
                },
                // 平滑相关配置和数据都集中在同一个 Series 初始化器中。
                Series = new ChartLineSeriesOption
                {
                    Id = "smooth-value",
                    Name = "数值",
                    ShowSymbol = true,
                    ShowAllSymbol = ChartLineShowAllSymbol.All,
                    Symbol = "circle",
                    SymbolSize = 10f,
                    Clip = true,
                    DataSource = new ChartLineData(
                        21d, 32d, 27d, 44d, 63d, 56d, 74d,
                        82d, 97d, 86d, 64d, 38d, 26d),
                    LineStyle = new()
                    {
                        Color = ChartBrush.Solid(LineColor),
                        Width = 2.5f,
                        Cap = SKStrokeCap.Round,
                        Join = SKStrokeJoin.Round,
                    },
                    ItemStyle = new()
                    {
                        Color = ChartBrush.Solid(LineColor),
                        BorderColor = ChartBrush.Solid(LineColor),
                        BorderWidth = 1.5f,
                    },
                    Label = new()
                    {
                        Show = true,
                        Position = "top",
                        Distance = 7f,
                        Formatter = "{c}",
                        Color = ChartBrush.Solid(LineColor),
                        FontSize = 14f,
                    },
                    Emphasis = new()
                    {
                        Focus = "series",
                        BlurScope = "coordinateSystem",
                    },
                    Smooth = ChartLineSmooth.Automatic,
                    SmoothMonotone = ChartLineSmoothMonotone.X,
                },
            };

            return option;
        }
    }
}
