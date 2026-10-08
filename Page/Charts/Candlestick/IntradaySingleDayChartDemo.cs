using System.Globalization;
using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Candlestick
{
    /// <summary>把午休 11:30—13:00 映射到同一个 X 坐标的单日走势。</summary>
    internal sealed class IntradaySingleDayChartDemo : CandlestickChartDemoPage
    {
        private static readonly DateTimeOffset OpenTime = new(2024, 4, 9, 9, 30, 0, TimeSpan.Zero);

        internal IntradaySingleDayChartDemo()
            : base(
                "断轴上的日内走势图 (II)",
                "09:30—11:30 与 13:00—15:00 两个交易时段。",
                "用交易分钟数值轴实现零间距午休压缩；11:30 与 13:00 共用一个刻度并合并显示。每项仍保留原始 UTC 时间，悬停提示不会变成压缩后的分钟序号。",
                CreateOption())
        {
        }

        private static ChartOption CreateOption()
        {
            var data = new ChartLineData();
            var random = new Random(20240410);
            var value = 1669d;
            for (var minute = 0; minute <= 330; minute++)
            {
                if (minute > 120 && minute < 210) continue; // 跳过午休。
                var time = OpenTime.AddMinutes(minute);
                var tradingMinute = minute >= 210 ? minute - 90 : minute;
                value = Math.Round(value +
                    Math.Floor((random.NextDouble() - 0.5 * Math.Sin(value / 1000d)) * 2000d) / 100d, 2);
                data.Add(new ChartLineDataItem(tradingMinute, value)
                {
                    Name = time.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                    Key = time.ToString("O", CultureInfo.InvariantCulture),
                });
            }

            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new() { Enabled = false },
                Title = new ChartTitleOption
                {
                    Text = "Intraday Chart with Breaks (Single Day)",
                    Left = "center", Top = "16px",
                    TextStyle = new() { FontSize = 18f },
                },
                Grid = new ChartGridOption
                {
                    Left = "8%", Right = "6%", Top = "80px", Bottom = "115px",
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                },
                XAxis = new ChartXAxisOption
                {
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(0d),
                    Maximum = ChartAxisBound.Fixed(240d),
                    Interval = 30d,
                    AxisLine = new() { OnZero = false },
                    SplitLine = new() { Show = false },
                    AxisLabel = new()
                    {
                        ShowMinLabel = true, ShowMaxLabel = true, HideOverlap = true,
                        FormatterCallback = (in ChartAxisLabelFormatterContext context) =>
                        {
                            if (!context.Value.TryGetDouble(out var minute)) return "";
                            if (Math.Abs(minute - 120d) < 0.001d) return "11:30/13:00";
                            var actualMinute = minute > 120d ? minute + 90d : minute;
                            return OpenTime.AddMinutes(actualMinute).ToString("HH:mm", CultureInfo.InvariantCulture);
                        },
                    },
                },
                YAxis = new ChartYAxisOption
                {
                    Type = ChartAxisType.Value, Scale = true,
                    Minimum = ChartAxisBound.DataMinimum,
                    SplitLine = new() { LineStyle = new() { Color = ChartBrush.Solid(SKColor.Parse("#e5eaf2")) } },
                },
                Tooltip = new ChartTooltipOption
                {
                    Trigger = ChartTooltipTrigger.Axis, Confine = true,
                    AxisPointer = new() { Type = ChartTooltipAxisPointerType.Line },
                    FormatterCallback = (in ChartTooltipFormatterContext context) =>
                    {
                        if (context.Parameters.Count == 0) return null;
                        var point = context.Parameters[0];
                        if (point.Value.Values.Count < 2 || !point.Value.Values[1].TryGetDouble(out var price)) return null;
                        return ChartTooltipContent.FromSections([
                            new ChartTooltipContentSection(point.Name + " UTC", [
                                new ChartTooltipContentLine("指数", [price.ToString("F2", context.Culture)], point.Color),
                            ]),
                        ]);
                    },
                },
                DataZoom =
                [
                    new ChartDataZoomInsideOption
                    {
                        Id = "single-day-inside", XAxisIndex = 0,
                        ZoomOnMouseWheel = ChartDataZoomInteractionTrigger.Ctrl,
                    },
                    new ChartDataZoomSliderOption
                    {
                        Id = "single-day-slider", XAxisIndex = 0,
                        Left = "8%", Right = "6%", Top = "90%", Height = "34px",
                        // Slider 端点没有回调接口；隐藏分钟序号，由主轴/Tooltip 显示实际时间。
                        Detail = new() { Show = false },
                    },
                ],
                Series = new ChartLineSeriesOption
                {
                    Id = "single-day-price", Name = "指数",
                    DataSource = data, ShowSymbol = false,
                    LineStyle = new() { Color = ChartBrush.Solid(SKColor.Parse("#5470c6")), Width = 1.6f },
                    ItemStyle = new() { Color = ChartBrush.Solid(SKColor.Parse("#5470c6")) },
                },
            };
        }
    }
}
