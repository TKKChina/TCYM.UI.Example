using SkiaSharp;
using TCYM.UI.Chart.Components.Mark;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Interaction;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Elements.Message;

namespace Page.Charts.Lines
{
    /// <summary>展示按时间类目连接离散值的基础折线图。</summary>
    internal sealed class BasicLineChartDemo : LineChartDemoPage
    {
        /// <summary>基础折线系列的稳定标识。</summary>
        private const string SeriesId = "basic-value";

        /// <summary>基础折线图使用的蓝色。</summary>
        private static readonly SKColor LineColor = new(22, 96, 235);

        /// <summary>平均值标记线使用的橙色。</summary>
        private static readonly SKColor AverageLineColor = new(250, 84, 28);

        /// <summary>创建基础折线图示例页面。</summary>
        internal BasicLineChartDemo()
            : base(
                "基础折线图",
                "使用圆形数据节点连接一组全天时序数据。",
                "折线保留原始拐点，在每个节点上方展示对应数值，并使用橙色虚线标出整组数据的平均值；点击任意圆形节点可查看对应时间和数值。",
                CreateOption(),
                CreateRegistry())
        {
            Chart.ChartEvent += OnChartEvent;
        }

        /// <summary>响应基础折线数据节点的点击事件。</summary>
        /// <param name="sender">触发事件的图表控件。</param>
        /// <param name="eventArgs">包含节点时间、数值和系列标识的图表事件参数。</param>
        private static void OnChartEvent(object? _, ChartEventArgs eventArgs)
        {
            if (eventArgs.Type != ChartEventType.Click
                || eventArgs.ComponentType != "series"
                || eventArgs.SeriesId != SeriesId
                || eventArgs.DataIndex < 0
                || !eventArgs.PrimaryValue.TryGetDouble(out var value))
            {
                return;
            }

            UIMessage.Info(
                $"点击时间：{eventArgs.Name ?? "-"}，数值：{value:0.##}°C",
                duration: 2f,
                key: "basic-line-point-click");
        }

        /// <summary>创建包含 Mark 模块的图表注册表。</summary>
        /// <returns>可绘制平均值标记线的图表注册表。</returns>
        private static ChartRegistry CreateRegistry()
        {
            var registry = ChartModules.CreateDefaultRegistry();
            ChartMarkModule.Register(registry);
            return registry;
        }

        /// <summary>创建基础折线图的完整配置。</summary>
        /// <returns>包含普通折线、圆形节点、数值标签和平均值标记线的图表配置。</returns>
        private static ChartOption CreateOption()
        {
            // 1. 创建图表和绘图区。基础折线图不需要图例。
            var option = new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 1000,
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
                // 2. 轴可像 ECharts JSON 一样直接放在 ChartOption 下，category Data 也可整体赋值。
                // 单 Grid/单 X/Y 轴默认按索引 0 绑定，无需重复声明关联 ID。
                XAxis = new ChartXAxisOption
                {
                    Name = "时间",
                    NameLocation = ChartAxisNameLocation.Middle,
                    NameGap = 38f,
                    Type = ChartAxisType.Category,
                    BoundaryGap = true,
                    AxisLabel = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(45, 52, 65)),
                        FontSize = 14f,
                    },
                    Data = ["00:00", "02:00", "04:00", "06:00", "08:00", "10:00", "12:00", "14:00", "16:00", "18:00", "20:00", "22:00", "24:00"],
                },
                YAxis = new ChartYAxisOption
                {
                    Name = "数值",
                    NameLocation = ChartAxisNameLocation.Start,
                    NameGap = 12f,
                    Type = ChartAxisType.Value,
                    Interval = 20d,
                    Scale = true,
                    SplitLine = new()
                    {
                        LineStyle = new()
                        {

                            Color = ChartBrush.Solid(new SKColor(224, 230, 239)),
                            Type = ChartAxisLineType.Dashed,
                            Width = 1f,
                        }
                    }
                },
                // 3. Series 与标量 DataSource 都可在对象初始化器中直接声明。
                Series = new ChartLineSeriesOption
                {
                    Id = SeriesId,
                    Name = "数值",
                    ShowSymbol = true,
                    ShowAllSymbol = ChartLineShowAllSymbol.All,
                    Symbol = "circle",
                    SymbolSize = 10f,
                    Clip = true,
                    DataSource = new ChartLineData(23d,34d,28d,45d,67d,58d,79d,72d,96d,85d,62d,41d,31d),
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
                    // 4. MarkLine 根据当前系列自动计算平均值，无需手工维护常量。
                    MarkLine = new()
                    {
                        Symbol = ChartMarkSymbolPair.Uniform("none"),
                        Precision = 2,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(AverageLineColor),
                            Width = 1.5f,
                            Type = ChartLineStyleType.Dashed,
                        },
                        Label = new()
                        {
                            Show = true,
                            // 标签放在线段右端内侧并向上偏移，避免长文字超出画布边界。
                            Position = "insideRight",
                            Distance = 8f,
                            Offset = new SKPoint(0f, -12f),
                            Color = ChartBrush.Solid(AverageLineColor),
                            FontSize = 13f,
                            FormatterCallback = static context =>
                                context.PrimaryValue.TryGetDouble(out var average)
                                    ? $"平均值：{average.ToString("F2", context.Culture)}°C"
                                    : "平均值",
                        },
                        Data =
                        {
                            new ChartMarkLineAxisDataItem(new ChartMarkLineAxisPositionOption
                            {
                                Statistic = ChartMarkStatisticType.Average,
                            })
                            {
                                Key = "basic-average",
                                Name = "平均值",
                            },
                        },
                    },
                },
            };

            var xAxis = option.XAxis.SingleOrDefault()
                ?? throw new InvalidOperationException("基础折线图缺少 X 轴配置。");

            var yAxis = option.YAxis.SingleOrDefault()
                ?? throw new InvalidOperationException("基础折线图缺少 Y 轴配置。");
            //ConfigureAxis(yAxis, showSplitLine: true);

            return option;
        }
    }
}
