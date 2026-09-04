using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Chart.Text;

namespace Page.Charts.Scatter
{
    /// <summary>展示两类关系数据、第三维气泡大小和顶部 T 形辅助标记的散点图。</summary>
    internal sealed class SpecialRelationScatterChartDemo : ScatterChartDemoPage
    {
        private const string BuyCategory = "买房人数";
        private const string RentCategory = "租房人数";
        private const string XAxisId = "special-relation-x";
        private const string LeftYAxisId = "special-relation-y";
        private const string RightYAxisId = "special-relation-marker-y";
        private const double YAxisMaximum = 700d;

        private const string TopMarkerPath =
            "path://M851.968 167.936l0 109.568-281.6 0 0 587.776-116.736 0 0-587.776-281.6 0 0-109.568 679.936 0z";

        /// <summary>原示例的十六条业务数据，Order 只负责确定数据和顶部标记的顺序。</summary>
        private static readonly RelationPoint[] SourceData =
        [
            new(BuyCategory, 1000d, 7086d, 10d, 1556d),
            new(BuyCategory, 2000d, 7686d, 52d, 1956d),
            new(RentCategory, 2000d, 1986d, 102d, 3556d),
            new(RentCategory, 4000d, 6786d, 384d, 5556d),
            new(BuyCategory, 3000d, 1586d, 200d, 4556d),
            new(RentCategory, 3000d, 2886d, 250d, 4856d),
            new(BuyCategory, 4000d, 3686d, 334d, 5056d),
            new(BuyCategory, 5000d, 2186d, 390d, 4056d),
            new(RentCategory, 5000d, 2956d, 540d, 5156d),
            new(BuyCategory, 6000d, 5486d, 330d, 4056d),
            new(RentCategory, 1000d, 1686d, 60d, 2556d),
            new(RentCategory, 6000d, 7586d, 380d, 4756d),
            new(BuyCategory, 7000d, 3486d, 220d, 3256d),
            new(RentCategory, 7000d, 3816d, 270d, 3856d),
            new(BuyCategory, 8000d, 1486d, 110d, 1056d),
            new(RentCategory, 8000d, 5656d, 160d, 1956d),
        ];

        internal SpecialRelationScatterChartDemo()
            : base(
                "特殊关系图",
                "用 X、Y 和气泡值三个维度同时表达两类住房关系数据。",
                "买房人数和租房人数分别形成一条 Scatter；第三维决定气泡大小，渐变和阴影增强层次。隐藏右侧 Y 轴承载顶部 path:// T 形标记，辅助系列不会进入图例或 Tooltip。",
                CreateOption())
        {
        }

        /// <summary>创建深色背景、两条气泡系列和一条顶部辅助系列。</summary>
        private static ChartOption CreateOption()
        {
            var legend = new ChartLegendOption
            {
                Top = "14px",
                Right = "28px",
                Orient = ChartLegendOrient.Horizontal,
                ItemWidth = 12f,
                ItemHeight = 12f,
                ItemGap = 22f,
                TextStyle = new()
                {
                    Color = ChartBrush.Solid(SKColors.White),
                    FontSize = 14f,
                },
            };
            // 只列两个业务系列，固定 T 辅助系列不会污染图例。
            legend.Data.Add(BuyCategory);
            legend.Data.Add(RentCategory);

            return new ChartOption
            {
                Background = ChartBrush.Solid(new SKColor(7, 20, 53)),
                Animation = new() { Enabled = true, Duration = 800 },
                Tooltip = new ChartTooltipOption
                {
                    Trigger = ChartTooltipTrigger.Item,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                    FormatterCallback = FormatTooltip,
                },
                Legend = legend,
                Grid = new ChartGridOption
                {
                    Left = "38px",
                    Top = "62px",
                    Right = "42px",
                    Bottom = "34px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                    BackgroundColor = ChartBrush.Solid(new SKColor(7, 20, 53)),
                },
                XAxis = new ChartXAxisOption
                {
                    Id = XAxisId,
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(1000d),
                    Scale = true,
                    AxisLine = new() { Show = false },
                    AxisTick = new() { Show = false },
                    AxisLabel = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(255, 255, 255, 204)),
                        FontSize = 12f,
                        Margin = 15f,
                    },
                    SplitLine = CreateSplitLine(true),
                },
                YAxis =
                [
                    CreateYAxis(LeftYAxisId, ChartYAxisPosition.Left, false, true),
                    CreateYAxis(RightYAxisId, ChartYAxisPosition.Right, true, false),
                ],
                Series =
                [
                    CreateBubbleSeries(
                        "special-relation-buy",
                        BuyCategory,
                        new SKColor(143, 253, 222),
                        new SKColor(10, 180, 132)),
                    CreateBubbleSeries(
                        "special-relation-rent",
                        RentCategory,
                        new SKColor(87, 144, 244),
                        new SKColor(19, 85, 199)),
                    CreateTopMarkerSeries(),
                ],
            };
        }

        /// <summary>创建可见左轴或完全隐藏的右侧辅助轴。</summary>
        private static ChartYAxisOption CreateYAxis(
            string id,
            ChartYAxisPosition position,
            bool hidden,
            bool showSplitLine) => new()
        {
            Id = id,
            Type = ChartAxisType.Value,
            Position = position,
            Minimum = ChartAxisBound.Fixed(0d),
            Maximum = ChartAxisBound.Fixed(YAxisMaximum),
            Interval = 100d,
            Scale = true,
            AxisLine = new() { Show = false },
            AxisTick = new() { Show = false },
            AxisLabel = new()
            {
                Show = !hidden,
                Color = ChartBrush.Solid(new SKColor(255, 255, 255, 204)),
                FontSize = 12f,
                Margin = 15f,
            },
            SplitLine = CreateSplitLine(showSplitLine),
        };

        /// <summary>创建买房或租房对应的一条三维气泡系列。</summary>
        private static ChartScatterSeriesOption CreateBubbleSeries(
            string id,
            string category,
            SKColor startColor,
            SKColor endColor)
        {
            var data = new ChartScatterData();
            foreach (var point in SourceData
                .Where(item => item.Category == category)
                .OrderBy(item => item.Order))
            {
                data.Add(new ChartScatterDataItem(
                    new ChartValue[] { point.X, point.Y, point.Quantity })
                {
                    Key = id + "-" + point.Order.ToString("0"),
                    Name = category + " " + point.Order.ToString("0"),
                });
            }

            return new ChartScatterSeriesOption
            {
                Id = id,
                Name = category,
                XAxisId = XAxisId,
                YAxisId = LeftYAxisId,
                Symbol = "circle",
                SymbolSize = 16f,
                SymbolSizeCallback = ResolveBubbleSize,
                Clip = true,
                DataSource = data,
                ItemStyle = new()
                {
                    Color = new ChartLinearGradientBrush(
                        new SKPoint(0.4f, 0.3f),
                        new SKPoint(0f, 1f),
                        [startColor, endColor],
                        [0f, 1f]),
                    ShadowBlur = 20f,
                    ShadowColor = ChartBrush.Solid(endColor.WithAlpha(210)),
                    ShadowOffsetY = 4f,
                    Opacity = 0.6f,
                },
                Emphasis = new()
                {
                    Focus = "series",
                    BlurScope = "coordinateSystem",
                    Scale = 1.12f,
                },
            };
        }

        /// <summary>创建位于隐藏右轴顶部的 path:// T 形标记。</summary>
        private static ChartScatterSeriesOption CreateTopMarkerSeries()
        {
            var data = new ChartScatterData();
            foreach (var order in SourceData
                .Select(item => item.Order)
                .Distinct()
                .OrderBy(value => value))
            {
                data.Add(new ChartScatterDataItem(order, YAxisMaximum)
                {
                    Key = "top-marker-" + order.ToString("0"),
                });
            }

            return new ChartScatterSeriesOption
            {
                Id = "special-relation-top-markers",
                XAxisId = XAxisId,
                YAxisId = RightYAxisId,
                Silent = true,
                Clip = false,
                Symbol = TopMarkerPath,
                SymbolKeepAspect = true,
                SymbolSize = 12f,
                SymbolOffset = new ChartSymbolOffset(
                    ChartLength.Pixels(0f),
                    ChartLength.Percent(40f)),
                DataSource = data,
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(new SKColor(99, 190, 248)),
                },
                Tooltip = new() { Show = false },
            };
        }

        /// <summary>第三维除以 100 得到气泡直径，并限制极端尺寸。</summary>
        private static ChartSymbolSize? ResolveBubbleSize(
            in ChartFormatterParameters parameters)
        {
            if (!parameters.TryGetDimension(2, out var quantity)
                || !quantity.TryGetDouble(out var value))
            {
                return ChartSymbolSize.Square(16f);
            }

            return ChartSymbolSize.Square(
                (float)Math.Clamp(value / 100d, 10d, 56d));
        }

        private static ChartTooltipContent? FormatTooltip(
            in ChartTooltipFormatterContext context)
        {
            if (context.Parameters.Count == 0
                || context.Parameters[0].Value.Values.Count < 3)
            {
                return null;
            }

            var parameter = context.Parameters[0];
            var values = parameter.Value.Values;
            return ChartTooltipContent.FromSections(
            [
                new ChartTooltipContentSection(
                    parameter.SeriesName ?? "关系数据",
                    [
                        new ChartTooltipContentLine(
                            "横轴值", [Format(values[0], context)],
                            parameter.Color, "circle"),
                        new ChartTooltipContentLine(
                            "纵轴值", [Format(values[1], context)]),
                        new ChartTooltipContentLine(
                            "气泡值", [Format(values[2], context)]),
                    ]),
            ]);
        }

        private static string Format(
            ChartValue value,
            in ChartTooltipFormatterContext context) =>
            value.TryGetDouble(out var number)
                ? number.ToString("0.##", context.Culture)
                : "-";

        private static ChartAxisSplitLineOption CreateSplitLine(bool show) => new()
        {
            Show = show,
            LineStyle = new()
            {
                Color = ChartBrush.Solid(new SKColor(255, 255, 255, 26)),
                Width = 1f,
            },
        };

        private readonly record struct RelationPoint(
            string Category,
            double Order,
            double X,
            double Y,
            double Quantity);
    }
}
