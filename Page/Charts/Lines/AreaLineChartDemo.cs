using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Lines
{
    /// <summary>展示带渐变填充、平滑路径和数值标签的面积折线图。</summary>
    internal sealed class AreaLineChartDemo : LineChartDemoPage
    {
        /// <summary>面积折线图使用的蓝色。</summary>
        private static readonly SKColor LineColor = new(22, 96, 235);

        /// <summary>创建面积折线图示例页面。</summary>
        internal AreaLineChartDemo()
            : base(
                "面积折线图",
                "在平滑折线下方添加由蓝色渐隐到透明的面积区域。",
                "适合展示具有连续趋势的时序数据；示例同时开启可切换 Legend 和节点数值标签。",
                CreateOption())
        {
        }

        /// <summary>创建面积折线图的完整配置。</summary>
        /// <returns>包含 Legend、平滑折线、空心节点和渐变面积的图表配置。</returns>
        private static ChartOption CreateOption()
        {
            // 1. 创建图表、图例和绘图区。
            var option = new ChartOption
            {
                //Background = ChartBrush.Solid(SKColors.White),
                Legend = new()
                {
                    Left = ChartLength.FromKeyword("center"),
                    Top = "10px",
                    Orient = ChartLegendOrient.Horizontal,
                    SelectedMode = ChartLegendSelectedMode.Multiple,
                    ItemWidth = 28f,
                    ItemHeight = 12f,
                    ItemGap = 26f,
                    TextStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(35, 42, 54)),
                        FontSize = 15f,
                    },
                },
                Tooltip = new()
                {
                    Trigger = ChartTooltipTrigger.Axis,
                    BackgroundColor = ChartBrush.Solid(new SKColor(255, 255, 255, 200)),
                    AxisPointer = new()
                    {
                        Type = ChartTooltipAxisPointerType.Line,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(22, 96, 235)),
                            Width = 1.5f,
                        },
                        Animation = ChartTooltipAxisPointerAnimation.Auto
                    },
                    FormatterCallback = (in ChartTooltipFormatterContext context) =>
                    {
                        if (context.Parameters.Count == 0)
                        {
                            return null;
                        }

                        var parameter = context.Parameters[0];
                        var valueText =
                            parameter.Value.Primary.AsString(context.Culture) ?? "-";
                        return ChartTooltipContent.FromSections(
                        [
                            new ChartTooltipContentSection(
                            header: $"时间: {parameter.Name ?? "-"}",
                            lines:
                            [
                                new ChartTooltipContentLine(
                                    name: parameter.SeriesName ?? "温度",
                                    values: [$"{valueText}°C"],
                                    markerColor: parameter.Color
                                        ?? ChartBrush.Solid(LineColor),
                                    markerType: "circle")
                            ])
                        ]);
                    }
                },
                Animation = new()
                {
                    Enabled = true,
                    Duration = 1000,
                },
                Grid = new ChartGridOption
                {
                    Show = true,
                    Left = "10px",
                    Top = "52px",
                    Right = ChartLength.Pixels(24f),
                    Bottom = "30px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                    BackgroundColor = ChartBrush.Solid(SKColors.White),
                },
                XAxis = new ChartXAxisOption
                {
                    Name = "时间",
                    NameLocation = ChartAxisNameLocation.Middle,
                    NameGap = 38f,
                    Type = ChartAxisType.Category,
                    BoundaryGap = false,
                    Data = ["00:00", "02:00", "04:00", "06:00", "08:00", "10:00", "12:00","14:00", "16:00", "18:00", "20:00", "22:00", "24:00",],
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
                        Length = 6f,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(145, 153, 166)),
                            Width = 1f,
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
                    SplitLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(224, 230, 239)),
                            Type = ChartAxisLineType.Dashed,
                            Width = 1f,
                        },
                    }
                },
                YAxis = new ChartYAxisOption
                {
                    Name = "°C",
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
                            Width = 1f,
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
                    }
                },
                Series = new ChartLineSeriesOption
                {
                    Name = "温度",
                    ShowSymbol = true,
                    ShowAllSymbol = ChartLineShowAllSymbol.All,
                    Symbol = "emptyCircle",
                    SymbolSize = 10f,
                    Clip = true,
                    DataSource = new ChartLineData(10d, 20d, 35d, 55d, 80d, 100d, 110d, 100d, 80d, 55d, 35d, 20d, 10d),
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
                        Formatter = "{c}°C",
                        Color = ChartBrush.Solid(LineColor),
                        FontSize = 14f,
                    },
                    Emphasis = new()
                    {
                        Focus = "series",
                        BlurScope = "coordinateSystem",
                    },
                    Smooth = ChartLineSmooth.FromTension(0.25f),
                    // 4. 开启平滑曲线，并为折线下方添加纵向渐变面积。
                    // ECharts 的 smooth=true 固定使用 0.5 强度；本示例显式降低到 0.25，
                    // 避免 100→110 的短峰在到达 110 前过早形成近似平台，同时保留连续切线。
                    AreaStyle = new()
                    {
                        Color = new ChartLinearGradientBrush(
                                new SKPoint(0f, 0f),
                                new SKPoint(0f, 1f),
                                [LineColor.WithAlpha(112), LineColor.WithAlpha(0)],
                                [0f, 1f]),
                        Origin = ChartLineAreaOrigin.Start,
                        Opacity = 1f,
                    }
                }
            };
            return option;
        }
    }
}
