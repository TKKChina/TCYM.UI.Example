using SkiaSharp;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Lines
{
    /// <summary>展示在下一类目位置切换数值的 End 阶梯折线图。</summary>
    internal sealed class StepLineChartDemo : LineChartDemoPage
    {
        /// <summary>阶梯折线图使用的蓝色。</summary>
        private static readonly SKColor LineColor = new(22, 96, 235);

        /// <summary>创建阶梯折线图示例页面。</summary>
        internal StepLineChartDemo()
            : base(
                "阶梯折线图",
                "使用水平区间和垂直跳变表达离散状态变化。",
                "End 阶梯模式会维持当前值直到下一个时间节点，适合展示档位、阈值或阶段性配置。",
                CreateOption())
        {
        }

        /// <summary>创建阶梯折线图的完整配置。</summary>
        /// <returns>包含 End 阶梯模式和数值标签的图表配置。</returns>
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
                // 阶梯类型和数据都集中在同一个 Series 初始化器中。
                Series = new ChartLineSeriesOption
                {
                    Id = "step-value",
                    Name = "数值",
                    ShowSymbol = true,
                    ShowAllSymbol = ChartLineShowAllSymbol.All,
                    Symbol = "circle",
                    SymbolSize = 10f,
                    Clip = true,
                    DataSource = new ChartLineData(
                        20d, 20d, 35d, 35d, 55d, 55d, 75d,
                        65d, 95d, 95d, 70d, 40d, 25d),
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
                    // End 表示到达下一个时间点时再垂直切换到新值。
                    Step = ChartLineStep.End,
                },
            };

            return option;
        }
    }
}
