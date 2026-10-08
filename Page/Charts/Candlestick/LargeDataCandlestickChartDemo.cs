using System.Globalization;
using SkiaSharp;
using TCYM.UI.Chart.Components.DataZoom;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Elements;

namespace Page.Charts.Candlestick
{
    /// <summary>共享 Dataset 驱动的 100,000 条分钟 K 线和红绿成交量。</summary>
    internal sealed class LargeDataCandlestickChartDemo : CandlestickChartDemoPage
    {
        private const int DataCount = 100_000;
        private static readonly SKColor UpColor = SKColor.Parse("#ec0000");
        private static readonly SKColor DownColor = SKColor.Parse("#00da3c");
        private readonly BackgroundCandlestickChart _backgroundChart;

        internal LargeDataCandlestickChartDemo()
            : this(new BackgroundCandlestickChart(CreateOption))
        {
        }

        private LargeDataCandlestickChartDemo(BackgroundCandlestickChart backgroundChart)
            : base(
                "大数据量 K 线图",
                "100,000 条可复现分钟行情，上方 K 线、下方成交量。",
                "后台生成并构建完整 100,000 条数据，加载时可继续切换页面。K 线使用 Large + Progressive，成交量启用高密度批次缓存；Ctrl + 滚轮和底部滑块可查看细节。",
                backgroundChart)
        {
            _backgroundChart = backgroundChart;
            var recentButton = AddToolbarButton("查看最近 500 条", () => _backgroundChart.DispatchAction(new ChartDataZoomAction(
                100d * (DataCount - 500) / (DataCount - 1), 100d)));
            var allButton = AddToolbarButton("恢复全部数据", () => _backgroundChart.DispatchAction(new ChartDataZoomAction(0d, 100d)));
            recentButton.Disabled = allButton.Disabled = true;
            _backgroundChart.Loaded += () => recentButton.Disabled = allButton.Disabled = false;
            _backgroundChart.LoadingFailed += _ => recentButton.Disabled = allButton.Disabled = true;
        }

        /// <summary>页面真正进入后启动任务；路由预创建不会提前分配十万条数据。</summary>
        public override void OnRouteEnter(string? fromPath) => _backgroundChart.Start();

        internal static ChartOption CreateOption(CancellationToken cancellationToken)
        {
            var source = GenerateOhlc(DataCount, cancellationToken);
            var candle = new ChartCandlestickSeriesOption
            {
                Id = "large-candles", Name = "分钟K",
                DatasetId = "large-ohlc",
                Large = true, LargeThreshold = 1_000,
                Progressive = 10_000, ProgressiveThreshold = 10_000,
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(UpColor),
                    Color0 = ChartBrush.Solid(DownColor),
                    BorderColor = ChartBrush.Solid(SKColor.Parse("#8a0000")),
                    BorderColor0 = ChartBrush.Solid(SKColor.Parse("#008f28")),
                },
            };
            // 原始列为 time/open/highest/lowest/close，y 编码重新排成 K 线的 OHLC 顺序。
            candle.Encode.Set("x", "time").Set("y", "open", "close", "lowest", "highest")
                .Set("itemName", "time");

            var option = new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new() { Enabled = false },
                Title = new ChartTitleOption
                {
                    Text = "Data Amount: 100,000", Left = "10px", Top = "10px",
                },
                Tooltip = new ChartTooltipOption
                {
                    Trigger = ChartTooltipTrigger.Axis, Confine = true,
                    AxisPointer = new() { Type = ChartTooltipAxisPointerType.Line },
                    FormatterCallback = (in ChartTooltipFormatterContext context) =>
                    {
                        if (context.Parameters.Count == 0) return null;
                        var item = context.Parameters[0];
                        var values = item.Value.Values;
                        if (values.Count < 7) return null;
                        var lines = new List<ChartTooltipContentLine>();
                        foreach (var (name, dimension) in new[] { ("开盘", 1), ("收盘", 4), ("最低", 3), ("最高", 2), ("成交量", 5) })
                        {
                            var text = values[dimension].TryGetDouble(out var value)
                                ? value.ToString(dimension == 5 ? "N0" : "F2", context.Culture) : "-";
                            lines.Add(new ChartTooltipContentLine(name, [text], item.Color));
                        }
                        return ChartTooltipContent.FromSections([
                            new ChartTooltipContentSection(item.Name, lines),
                        ]);
                    },
                },
                Grid = new ChartGridOption
                {
                    Id = "large-price-grid", Left = "8%", Right = "6%", Top = "64px", Bottom = "200px",
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                },
                XAxis =
                [
                    new ChartXAxisOption
                    {
                        Id = "large-price-x", Type = ChartAxisType.Category,
                        BoundaryGap = false,
                        AxisLine = new() { OnZero = false },
                        SplitLine = new() { Show = false },
                        Minimum = ChartAxisBound.DataMinimum, Maximum = ChartAxisBound.DataMaximum,
                        AxisLabel = new() { FontSize = 11f, HideOverlap = true },
                    },
                    new ChartXAxisOption
                    {
                        Id = "large-volume-x", Type = ChartAxisType.Category, GridIndex = 1,
                        BoundaryGap = false,
                        AxisLine = new() { OnZero = false },
                        AxisTick = new() { Show = false },
                        AxisLabel = new() { Show = false },
                        SplitLine = new() { Show = false },
                        Minimum = ChartAxisBound.DataMinimum, Maximum = ChartAxisBound.DataMaximum,
                    },
                ],
                YAxis =
                [
                    new ChartYAxisOption
                    {
                        Type = ChartAxisType.Value, Scale = true,
                        SplitArea = new() { Show = true },
                        AxisLabel = new() { FormatterCallback = (in ChartAxisLabelFormatterContext context) =>
                            context.Value.TryGetDouble(out var value) ? value.ToString("0.##", CultureInfo.InvariantCulture) : "" },
                    },
                    new ChartYAxisOption
                    {
                        Type = ChartAxisType.Value, GridIndex = 1, Scale = true,
                        Minimum = ChartAxisBound.Fixed(0d), SplitNumber = 2,
                        AxisLabel = new() { Show = false }, AxisLine = new() { Show = false },
                        AxisTick = new() { Show = false }, SplitLine = new() { Show = false },
                    },
                ],
                DataZoom =
                [
                    new ChartDataZoomInsideOption
                    {
                        Id = "large-inside", XAxisIndex = [0, 1], Start = 10, End = 100,
                        ZoomOnMouseWheel = ChartDataZoomInteractionTrigger.Ctrl,
                    },
                    new ChartDataZoomSliderOption
                    {
                        Id = "large-slider", XAxisIndex = [0, 1], Start = 10, End = 100,
                        Left = "8%", Right = "6%", Top = "91%", Height = "34px",
                        Realtime = false, Detail = new() { Show = false },
                    },
                ],
                Series =
                [
                    candle,
                    CreateVolumeSeries("large-up-volume", "上涨成交量", "upVolume", UpColor),
                    CreateVolumeSeries("large-down-volume", "下跌成交量", "downVolume", DownColor),
                ],
            };
            option.Dataset.Add(new ChartDatasetOption { Id = "large-ohlc", Source = source });
            option.Components.Add(new ChartGridOption
            {
                Id = "large-volume-grid", Left = "8%", Right = "6%", Height = "80px", Bottom = "90px",
                OuterBoundsMode = ChartGridOuterBoundsMode.None,
            });
            return option;
        }

        private static ChartBarSeriesOption CreateVolumeSeries(
            string id, string name, string valueDimension, SKColor color)
        {
            // Large Bar 不支持逐项 VisualMap。两个批次占用同一柱位，空值一侧不绘制。
            var series = new ChartBarSeriesOption
            {
                Id = id, Name = name, DatasetId = "large-ohlc",
                XAxisIndex = 1, YAxisIndex = 1,
                Large = true, LargeThreshold = 1_000, Progressive = 0,
                BarGap = ChartBarGap.Percent(-100f),
                ItemStyle = new() { Color = ChartBrush.Solid(color) },
            };
            series.Encode.Set("x", "time").Set("y", valueDimension).Set("itemName", "time");
            return series;
        }

        /// <summary>使用固定种子生成数据，每分钟一条；数值排序后选取开盘和收盘。</summary>
        private static ChartDataTable GenerateOhlc(int count, CancellationToken cancellationToken)
        {
            var table = new ChartDataTable()
                .AddDimension("time", ChartDimensionType.Ordinal)
                .AddDimension("open").AddDimension("highest").AddDimension("lowest")
                .AddDimension("close").AddDimension("volume").AddDimension("sign")
                .AddDimension("upVolume").AddDimension("downVolume");
            var random = new Random(20110101);
            var start = new DateTimeOffset(2011, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var baseValue = 6000d;
            var previousClose = baseValue;
            var prices = new double[4];
            for (var index = 0; index < count; index++)
            {
                if ((index & 255) == 0) cancellationToken.ThrowIfCancellationRequested();
                baseValue += random.NextDouble() * 20d - 10d;
                for (var price = 0; price < prices.Length; price++)
                    prices[price] = baseValue + (random.NextDouble() - 0.5d) * 12d;
                Array.Sort(prices); // 必须按数字排序，不能按字符串字典序排序。
                var openIndex = random.Next(4);
                var closeIndex = random.Next(3);
                if (closeIndex >= openIndex) closeIndex++;
                var open = Math.Round(prices[openIndex], 2);
                var close = Math.Round(prices[closeIndex], 2);
                var high = Math.Round(prices[3], 2);
                var low = Math.Round(prices[0], 2);
                var volume = Math.Round(prices[3] * (1000d + random.NextDouble() * 500d));
                var sign = open < close ? 1 : open > close ? -1 : previousClose <= close ? 1 : -1;
                var time = start.AddMinutes(index + 1).ToString("yyyy-MM-dd\nHH:mm:ss", CultureInfo.InvariantCulture);
                table.AddItem([
                    time, open, high, low, close, volume, sign,
                    sign > 0 ? ChartValue.From(volume) : ChartValue.Null,
                    sign < 0 ? ChartValue.From(volume) : ChartValue.Null,
                ], key: index.ToString(CultureInfo.InvariantCulture), name: time);
                previousClose = close;
            }
            return table;
        }
    }
}
