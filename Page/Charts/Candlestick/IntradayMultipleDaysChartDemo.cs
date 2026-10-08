using System.Globalization;
using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Chart.Text;

namespace Page.Charts.Candlestick
{
    /// <summary>通过交易分钟类目压缩夜间休市区间，展示四天的日内面积走势。</summary>
    internal sealed class IntradayMultipleDaysChartDemo : CandlestickChartDemoPage
    {
        internal IntradayMultipleDaysChartDemo()
            : base(
                "断轴上的日内走势图",
                "四个交易日的 09:30—16:00 行情，交易日之间保留窄间隔。",
                "示例层用交易分钟类目压缩夜间休市时间（当前没有原生 axis.breaks）；日间数据完整保留，跨日插入空值断开折线，Tooltip 显示真实 UTC 日期和时间。",
                CreateOption())
        {
        }

        private static ChartOption CreateOption()
        {
            var (categories, data) = CreateTradingData();
            var minimumSpan = 60d / (categories.Length - 1) * 100d;
            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new() { Enabled = false },
                Title = new ChartTitleOption
                {
                    Text = "Intraday Chart with Breaks (Multiple Days)",
                    Left = "center", Top = "16px",
                    TextStyle = new() { FontSize = 18f },
                },
                Grid = new ChartGridOption
                {
                    Left = "8%", Right = "6%", Top = "100px", Bottom = "155px",
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                },
                XAxis = new ChartXAxisOption
                {
                    Type = ChartAxisType.Category,
                    Data = categories.Select(ChartValue.From).ToArray(),
                    BoundaryGap = false,
                    Minimum = ChartAxisBound.DataMinimum,
                    Maximum = ChartAxisBound.DataMaximum,
                    AxisTick = new() { Show = false },
                    SplitLine = new() { Show = false },
                    AxisLabel = new()
                    {
                        Interval = 0,
                        FontSize = 11f,
                        HideOverlap = true,
                        ShowMinLabel = true,
                        ShowMaxLabel = true,
                        Rich = new()
                        {
                            ["weak"] = new ChartTextStyle { Color = ChartBrush.Solid(SKColor.Parse("#999999")) },
                        },
                        TextFormatter = (in ChartFormatterParameters parameters) =>
                        {
                            var text = parameters.PrimaryValue.AsString(CultureInfo.InvariantCulture);
                            if (!DateTimeOffset.TryParseExact(text, "yyyy-MM-dd HH:mm",
                                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var time)
                                || time.Minute % 30 != 0)
                                return ChartTextContent.Empty;

                            // 只在开盘/收盘处增加淡色日期，其余刻度仅显示小时和分钟。
                            var boundary = time.Hour == 9 && time.Minute == 30
                                || time.Hour == 16 && time.Minute == 0;
                            return boundary
                                ? ChartTextContent.FromRuns([
                                    new ChartTextRun(time.ToString("HH:mm", CultureInfo.InvariantCulture)),
                                    new ChartTextRun("\n" + time.ToString("dd'd'", CultureInfo.InvariantCulture), "weak"),
                                ])
                                : new ChartTextContent(time.ToString("HH:mm", CultureInfo.InvariantCulture));
                        },
                    },
                },
                YAxis = new ChartYAxisOption
                {
                    Type = ChartAxisType.Value,
                    Scale = true,
                    Minimum = ChartAxisBound.DataMinimum,
                    AxisLabel = new() { Formatter = "{value}", FontSize = 12f },
                    SplitLine = new()
                    {
                        LineStyle = new() { Color = ChartBrush.Solid(SKColor.Parse("#e5eaf2")) },
                    },
                },
                Tooltip = new ChartTooltipOption
                {
                    Trigger = ChartTooltipTrigger.Axis,
                    Confine = true,
                    AxisPointer = new() { Type = ChartTooltipAxisPointerType.Line },
                    FormatterCallback = (in ChartTooltipFormatterContext context) =>
                    {
                        if (context.Parameters.Count == 0) return null;
                        var point = context.Parameters[0];
                        if (!point.Value.Primary.TryGetDouble(out var value)) return null;
                        return ChartTooltipContent.FromSections([
                            new ChartTooltipContentSection(point.Name + " UTC", [
                                new ChartTooltipContentLine("指数", [value.ToString("F2", context.Culture)], point.Color),
                            ]),
                        ]);
                    },
                },
                DataZoom =
                [
                    new ChartDataZoomInsideOption
                    {
                        Id = "multi-day-inside", XAxisIndex = 0,
                        MinSpan = minimumSpan, ZoomOnMouseWheel = ChartDataZoomInteractionTrigger.Ctrl,
                    },
                    new ChartDataZoomSliderOption
                    {
                        Id = "multi-day-slider", XAxisIndex = 0,
                        Left = "8%", Right = "6%", Top = "83%", Height = "34px",
                        MinSpan = minimumSpan,
                        Detail = new() { Show = false },
                    },
                ],
                Series = new ChartLineSeriesOption
                {
                    Id = "multi-day-price", Name = "指数",
                    DataSource = data,
                    ShowSymbol = false,
                    ConnectNulls = false,
                    LineStyle = new() { Color = ChartBrush.Solid(SKColor.Parse("#5470c6")), Width = 1.5f },
                    ItemStyle = new() { Color = ChartBrush.Solid(SKColor.Parse("#5470c6")) },
                    AreaStyle = new() { Color = ChartBrush.Solid(SKColor.Parse("#5470c6")), Opacity = 0.22f },
                },
            };
        }

        /// <summary>每天保留 391 个分钟数据；15 个空类目约占完整宽度的 1%，用于跨日断线。</summary>
        private static (string[] Categories, ChartLineData Data) CreateTradingData()
        {
            var categories = new List<string>();
            var data = new ChartLineData();
            var random = new Random(20240409);
            var value = 1669d;
            var firstDay = new DateTimeOffset(2024, 4, 9, 9, 30, 0, TimeSpan.Zero);
            for (var day = 0; day < 4; day++)
            {
                if (day > 0)
                {
                    for (var gap = 0; gap < 15; gap++)
                    {
                        var key = $"休市-{day}-{gap}";
                        categories.Add(key);
                        data.Add(new ChartLineDataItem(ChartValue.Null) { Key = key, Name = key });
                    }
                }

                for (var minute = 0; minute <= 390; minute++)
                {
                    var scale = day > 0 && minute == 0 ? 10d : 1d;
                    value = Math.Round(value +
                        Math.Floor((random.NextDouble() - 0.5 * Math.Sin(value / 1000d)) * 2000d) / 100d * scale, 2);
                    var time = firstDay.AddDays(day).AddMinutes(minute);
                    var label = time.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    categories.Add(label);
                    data.AddValue(value, name: label, key: label);
                }
            }
            return (categories.ToArray(), data);
        }
    }
}
