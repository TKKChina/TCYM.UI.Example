using SkiaSharp;
using TCYM.UI.Chart.Components.Mark;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Lines
{
    /// <summary>展示两个系列共用 Cartesian 坐标系的对比折线图。</summary>
    internal sealed class MultipleLineChartDemo : LineChartDemoPage
    {
        /// <summary>系列 A 使用的蓝色。</summary>
        private static readonly SKColor SeriesAColor = new(22, 96, 235);

        /// <summary>系列 B 使用的橙色。</summary>
        private static readonly SKColor SeriesBColor = new(250, 84, 28);

        /// <summary>需要把标签放到另一侧以减少重叠的数据索引。</summary>
        private static readonly HashSet<int> AlternateLabelIndices =
        [
            0, 2, 5, 10, 11, 12,
        ];

        /// <summary>创建多系列折线图示例页面。</summary>
        internal MultipleLineChartDemo()
            : base(
                "多系列折线图",
                "在同一时间轴上对比系列 A 与系列 B。",
                "两个系列分别使用蓝色和橙色，并通过 Legend 控制显隐；标签分列在节点上下方以减少遮挡，同时用同系列颜色的图钉标出各自最大值和最小值。",
                CreateOption(),
                CreateRegistry())
        {
        }

        /// <summary>创建包含 Mark 模块的图表注册表。</summary>
        /// <returns>可绘制最大值、最小值标记的图表注册表。</returns>
        private static ChartRegistry CreateRegistry()
        {
            var registry = ChartModules.CreateDefaultRegistry();
            ChartMarkModule.Register(registry);
            return registry;
        }

        /// <summary>创建多系列折线图的完整配置。</summary>
        /// <returns>包含 Legend 以及蓝色、橙色两个系列的图表配置。</returns>
        private static ChartOption CreateOption()
        {
            var option = new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = false,
                },
                Legend = new ChartLegendOption
                {
                    Id = "multiple-legend",
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
                Grid = new ChartGridOption
                {
                    Show = true,
                    Left = "10px",
                    Top = "52px",
                    Right = "24px",
                    Bottom = "30px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                    BackgroundColor = ChartBrush.Solid(SKColors.White),
                },
                // 两个系列共用同一组坐标轴，默认按索引 0 绑定。
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
                // 强类型 collection expression 的顺序就是 seriesIndex 顺序。
                Series =
                [
                    CreateSeries(
                        "multiple-a",
                        "系列A",
                        [18d, 32d, 27d, 46d, 64d, 53d, 71d, 68d, 92d, 78d, 57d, 39d, 25d],
                        SeriesAColor,
                        labelPosition: "top",
                        alternateLabelIndices: AlternateLabelIndices),
                    CreateSeries(
                        "multiple-b",
                        "系列B",
                        [28d, 22d, 35d, 38d, 55d, 60d, 66d, 50d, 63d, 75d, 70d, 48d, 30d],
                        SeriesBColor,
                        labelPosition: "bottom",
                        alternateLabelIndices: AlternateLabelIndices),
                ],
            };

            return option;
        }

        /// <summary>创建多系列示例中的单个折线系列。</summary>
        /// <param name="id">系列稳定 ID。</param>
        /// <param name="name">图例显示名称。</param>
        /// <param name="values">按时间顺序排列的系列数据。</param>
        /// <param name="color">折线、节点和标签颜色。</param>
        /// <param name="labelPosition">默认标签位置。</param>
        /// <param name="alternateLabelIndices">需要把标签放到相反侧的数据索引。</param>
        /// <returns>绑定多系列示例坐标轴的折线系列。</returns>
        private static ChartLineSeriesOption CreateSeries(
            string id,
            string name,
            IReadOnlyList<double> values,
            SKColor color,
            string labelPosition,
            IReadOnlySet<int> alternateLabelIndices)
        {
            var (minimumIndex, maximumIndex) = ResolveExtremaIndices(values);
            var data = new ChartLineData();
            for (var index = 0; index < values.Count; index++)
            {
                var item = new ChartLineDataItem(values[index]);
                if (alternateLabelIndices.Contains(index))
                {
                    item.Label.Position = string.Equals(
                        labelPosition,
                        "top",
                        StringComparison.Ordinal)
                        ? "bottom"
                        : "top";
                }

                if (index == minimumIndex || index == maximumIndex)
                {
                    // 极值由 pin 内标签显示，隐藏宿主 Line 标签避免同一数值重复绘制。
                    item.Label.Show = false;
                }

                data.Add(item);
            }

            return new ChartLineSeriesOption
            {
                Id = id,
                Name = name,
                ShowSymbol = true,
                ShowAllSymbol = ChartLineShowAllSymbol.All,
                Symbol = "circle",
                SymbolSize = 10f,
                Clip = true,
                DataSource = data,
                LineStyle = new()
                {
                    Color = ChartBrush.Solid(color),
                    Width = 2.5f,
                    Cap = SKStrokeCap.Round,
                    Join = SKStrokeJoin.Round,
                },
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(color),
                    BorderColor = ChartBrush.Solid(color),
                    BorderWidth = 1.5f,
                },
                Label = new()
                {
                    Show = true,
                    Position = labelPosition,
                    Distance = 7f,
                    Formatter = "{c}",
                    Color = ChartBrush.Solid(color),
                    FontSize = 14f,
                },
                Emphasis = new()
                {
                    Focus = "series",
                    BlurScope = "coordinateSystem",
                },
                MarkPoint = new ChartMarkPointOption
                {
                    Symbol = "pin",
                    SymbolSize = 50f,
                    Precision = 0,
                    ItemStyle = new()
                    {
                        Color = ChartBrush.Solid(color),
                        BorderColor = ChartBrush.Solid(SKColors.White),
                        BorderWidth = 2f,
                    },
                    Label = new ChartLabelOption
                    {
                        Show = true,
                        Position = "inside",
                        Color = ChartBrush.Solid(SKColors.White),
                        FontSize = 14f,
                    },
                    Data =
                    {
                        new ChartMarkPointDataItem(new ChartMarkPositionOption
                        {
                            Statistic = ChartMarkStatisticType.Max,
                        })
                        {
                            Key = $"{id}-maximum",
                            Name = "最大值",
                        },
                        new ChartMarkPointDataItem(new ChartMarkPositionOption
                        {
                            Statistic = ChartMarkStatisticType.Min,
                        })
                        {
                            Key = $"{id}-minimum",
                            Name = "最小值",
                        },
                    },
                },
            };
        }

        /// <summary>查找 MarkPoint 统计会命中的首个有限最小值和最大值宿主索引。</summary>
        /// <param name="values">按系列数据顺序排列的原始数值。</param>
        /// <returns>没有有限值时均为 -1；否则返回首个最小值和最大值的索引。</returns>
        private static (int MinimumIndex, int MaximumIndex) ResolveExtremaIndices(
            IReadOnlyList<double> values)
        {
            var minimumIndex = -1;
            var maximumIndex = -1;
            var minimumValue = double.PositiveInfinity;
            var maximumValue = double.NegativeInfinity;
            for (var index = 0; index < values.Count; index++)
            {
                var value = values[index];
                if (!double.IsFinite(value))
                {
                    continue;
                }

                if (value < minimumValue)
                {
                    minimumValue = value;
                    minimumIndex = index;
                }

                if (value > maximumValue)
                {
                    maximumValue = value;
                    maximumIndex = index;
                }
            }

            return (minimumIndex, maximumIndex);
        }
    }
}
