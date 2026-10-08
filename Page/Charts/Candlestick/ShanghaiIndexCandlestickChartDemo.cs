using SkiaSharp;
using TCYM.UI.Chart.Components.Mark;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Candlestick
{
    /// <summary>上证指数日 K：红涨绿跌、四条收盘均线、最高/最低与收盘统计标记。</summary>
    internal sealed class ShanghaiIndexCandlestickChartDemo : CandlestickChartDemoPage
    {
        private static readonly SKColor UpColor = SKColor.Parse("#ec0000");
        private static readonly SKColor DownColor = SKColor.Parse("#00da3c");

        internal ShanghaiIndexCandlestickChartDemo()
            : base(
                "上证指数",
                "日 K、MA5 / MA10 / MA20 / MA30 与高低值标记。",
                "保留附件中 88 个交易日的原始 OHLC 数据。红色上涨、绿色下跌；均线由收盘价计算，底部滑块和 Ctrl + 滚轮联动缩放，悬停查看当日价格。",
                CreateOption(),
                CreateRegistry())
        {
        }

        private static ChartRegistry CreateRegistry()
        {
            var registry = ChartModules.CreateDefaultRegistry();
            ChartMarkModule.Register(registry);
            return registry;
        }

        /// <summary>每个案例在自己的类中组装完整 Options。</summary>
        private static ChartOption CreateOption()
        {
            var rows = CreateHistory();
            var candles = new ChartCandlestickData();
            foreach (var row in rows)
            {
                // K 线前四维顺序固定：开盘、收盘、最低、最高。
                candles.AddOhlc(
                    row.Open,
                    row.Close,
                    row.Low,
                    row.High,
                    name: row.Date,
                    key: row.Date);
            }

            var daily = new ChartCandlestickSeriesOption
            {
                Id = "shanghai-daily",
                Name = "日K",
                DataSource = candles,
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(UpColor),
                    Color0 = ChartBrush.Solid(DownColor),
                    BorderColor = ChartBrush.Solid(SKColor.Parse("#8a0000")),
                    BorderColor0 = ChartBrush.Solid(SKColor.Parse("#008f28")),
                },
                MarkPoint = new ChartMarkPointOption
                {
                    Symbol = "pin",
                    SymbolSize = 48f,
                    Precision = 0,
                    Label = new()
                    {
                        Show = true,
                        Position = "inside",
                        Color = ChartBrush.Solid(SKColors.White),
                        FontSize = 11f,
                        FormatterCallback = context => context.PrimaryValue.TryGetDouble(out var value)
                            ? Math.Round(value).ToString("0", context.Culture)
                            : "",
                    },
                    Data =
                    {
                        new ChartMarkPointDataItem(new ChartMarkPositionOption
                        {
                            XAxis = "2013/5/31", YAxis = 2300d, Value = 2300d,
                        })
                        {
                            Key = "manual-mark", Name = "Mark",
                            ItemStyle = new() { Color = ChartBrush.Solid(SKColor.Parse("#293c55")) },
                        },
                        CreateStatisticPoint("highest", "最高价", ChartMarkStatisticType.Max, "highest"),
                        CreateStatisticPoint("lowest", "最低价", ChartMarkStatisticType.Min, "lowest"),
                        CreateStatisticPoint("average", "平均收盘价", ChartMarkStatisticType.Average, "close"),
                    },
                    Tooltip = new()
                    {
                        FormatterCallback = (in ChartTooltipFormatterContext context) =>
                        {
                            if (context.Parameters.Count == 0) return null;
                            var item = context.Parameters[0];
                            return ChartTooltipContent.FromText(
                                $"{item.Name}\n{item.Value.Primary.AsString(context.Culture)}");
                        },
                    },
                },
                MarkLine = new ChartMarkLineOption
                {
                    Symbol = ChartMarkSymbolPair.Uniform("none"),
                    Precision = 2,
                    Label = new() { Show = false, Position = "end", FontSize = 11f },
                    Data =
                    {
                        new ChartMarkLineEndpointsDataItem(
                            CreateExtremeEndpoint(ChartMarkStatisticType.Min, "lowest"),
                            CreateExtremeEndpoint(ChartMarkStatisticType.Max, "highest"))
                        {
                            Key = "low-to-high", Name = "最低至最高",
                        },
                        new ChartMarkLineAxisDataItem(new ChartMarkLineAxisPositionOption
                        {
                            Statistic = ChartMarkStatisticType.Min, ValueDimension = "close",
                        }) { Key = "minimum-close", Name = "最低收盘价", Label = new() { Show = true } },
                        new ChartMarkLineAxisDataItem(new ChartMarkLineAxisPositionOption
                        {
                            Statistic = ChartMarkStatisticType.Max, ValueDimension = "close",
                        }) { Key = "maximum-close", Name = "最高收盘价", Label = new() { Show = true } },
                    },
                },
            };

            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new() { Enabled = false },
                Title = new ChartTitleOption { Text = "上证指数", Left = "10px", Top = "8px" },
                Legend = new ChartLegendOption
                {
                    Left = "center", Top = "16px",
                    Data = { "日K", "MA5", "MA10", "MA20", "MA30" },
                },
                Tooltip = new ChartTooltipOption
                {
                    Trigger = ChartTooltipTrigger.Axis,
                    Confine = true,
                    AxisPointer = new() { Type = ChartTooltipAxisPointerType.Cross, Precision = 2 },
                    FormatterCallback = (in ChartTooltipFormatterContext context) => FormatTooltip(in context, rows),
                },
                Grid = new ChartGridOption
                {
                    Left = "8%", Right = "8%", Top = "70px", Bottom = "125px",
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                },
                XAxis = new ChartXAxisOption
                {
                    Type = ChartAxisType.Category,
                    Data = rows.Select(row => ChartValue.From(row.Date)).ToArray(),
                    BoundaryGap = false,
                    AxisLine = new() { OnZero = false },
                    SplitLine = new() { Show = false },
                    Minimum = ChartAxisBound.DataMinimum,
                    Maximum = ChartAxisBound.DataMaximum,
                    AxisLabel = new() { FontSize = 11f, HideOverlap = true },
                },
                YAxis = new ChartYAxisOption
                {
                    Type = ChartAxisType.Value,
                    Scale = true,
                    SplitArea = new() { Show = true },
                    AxisLabel = new()
                    {
                        FormatterCallback = (in ChartAxisLabelFormatterContext context) =>
                            context.Value.TryGetDouble(out var value) ? value.ToString("0.##") : "",
                    },
                },
                DataZoom =
                [
                    new ChartDataZoomInsideOption
                    {
                        Id = "shanghai-inside", XAxisIndex = 0, Start = 50, End = 100,
                        ZoomOnMouseWheel = ChartDataZoomInteractionTrigger.Ctrl,
                    },
                    new ChartDataZoomSliderOption
                    {
                        Id = "shanghai-slider", XAxisIndex = 0, Start = 50, End = 100,
                        Left = "8%", Right = "8%", Top = "90%", Height = "32px",
                        Detail = new() { Show = false },
                    },
                ],
                Series =
                [
                    daily,
                    CreateMovingAverage(rows, 5, SKColor.Parse("#5470c6")),
                    CreateMovingAverage(rows, 10, SKColor.Parse("#91cc75")),
                    CreateMovingAverage(rows, 20, SKColor.Parse("#fac858")),
                    CreateMovingAverage(rows, 30, SKColor.Parse("#ee6666")),
                ],
            };
        }

        private static ChartMarkPointDataItem CreateStatisticPoint(
            string key, string name, ChartMarkStatisticType statistic, string dimension) =>
            new(new ChartMarkPositionOption { Statistic = statistic, ValueDimension = dimension })
            {
                Key = key, Name = name,
            };

        private static ChartMarkLineEndpointOption CreateExtremeEndpoint(
            ChartMarkStatisticType statistic, string dimension) =>
            new(new ChartMarkPositionOption { Statistic = statistic, ValueDimension = dimension })
            {
                Symbol = "circle",
                SymbolSize = 10f,
                Label = new() { Show = false },
                Emphasis = new() { Label = new() { Show = false } },
            };

        /// <summary>按附件口径留空最初 dayCount 项，随后求含当日在内的收盘价平均值。</summary>
        private static ChartLineSeriesOption CreateMovingAverage(
            (string Date, double Open, double Close, double Low, double High)[] rows,
            int dayCount,
            SKColor color)
        {
            var data = new ChartLineData();
            for (var index = 0; index < rows.Length; index++)
            {
                var average = ChartValue.Null;
                if (index >= dayCount)
                {
                    var sum = 0d;
                    for (var offset = 0; offset < dayCount; offset++)
                        sum += rows[index - offset].Close;
                    average = sum / dayCount;
                }
                data.AddValue(average, name: rows[index].Date, key: rows[index].Date);
            }

            return new ChartLineSeriesOption
            {
                Id = $"shanghai-ma{dayCount}", Name = $"MA{dayCount}",
                DataSource = data, Smooth = ChartLineSmooth.Automatic,
                // 每个有效均线值都显示空心圆，不随类目轴标签采样而隐藏。
                ShowSymbol = true,
                ShowAllSymbol = ChartLineShowAllSymbol.All,
                Symbol = "emptyCircle",
                SymbolSize = 6f,
                LineStyle = new() { Color = ChartBrush.Solid(color), Opacity = 0.5f, Width = 1.5f },
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(color),
                    BorderColor = ChartBrush.Solid(color),
                    BorderWidth = 1.5f,
                },
            };
        }

        private static ChartTooltipContent? FormatTooltip(
            in ChartTooltipFormatterContext context,
            (string Date, double Open, double Close, double Low, double High)[] rows)
        {
            if (context.Parameters.Count == 0) return null;
            var index = context.Parameters[0].DataIndex;
            if (index < 0 || index >= rows.Length) return null;
            var row = rows[index];
            var lines = new List<ChartTooltipContentLine>();
            var priceColor = ChartBrush.Solid(row.Close >= row.Open ? UpColor : DownColor);
            // 按原始数据索引取 OHLC；即使指针最近的图元是均线，也显示同一交易日的价格。
            foreach (var (name, value) in new[] { ("开盘", row.Open), ("收盘", row.Close), ("最低", row.Low), ("最高", row.High) })
                lines.Add(new ChartTooltipContentLine(name, [value.ToString("F2", context.Culture)], priceColor));

            foreach (var item in context.Parameters)
            {
                if (item.SeriesType == "line")
                {
                    lines.Add(new ChartTooltipContentLine(item.SeriesName,
                        [item.Value.Primary.TryGetDouble(out var value) ? value.ToString("F2", context.Culture) : "-"],
                        item.Color));
                }
            }
            return ChartTooltipContent.FromSections([new ChartTooltipContentSection(row.Date, lines)]);
        }

        /// <summary>附件提供的 2013-01-24 至 2013-06-13 历史数据。</summary>
        private static (string Date, double Open, double Close, double Low, double High)[] CreateHistory() =>
            [
                ("2013/1/24", 2320.26d, 2320.26d, 2287.3d, 2362.94d),
                ("2013/1/25", 2300d, 2291.3d, 2288.26d, 2308.38d),
                ("2013/1/28", 2295.35d, 2346.5d, 2295.35d, 2346.92d),
                ("2013/1/29", 2347.22d, 2358.98d, 2337.35d, 2363.8d),
                ("2013/1/30", 2360.75d, 2382.48d, 2347.89d, 2383.76d),
                ("2013/1/31", 2383.43d, 2385.42d, 2371.23d, 2391.82d),
                ("2013/2/1", 2377.41d, 2419.02d, 2369.57d, 2421.15d),
                ("2013/2/4", 2425.92d, 2428.15d, 2417.58d, 2440.38d),
                ("2013/2/5", 2411d, 2433.13d, 2403.3d, 2437.42d),
                ("2013/2/6", 2432.68d, 2434.48d, 2427.7d, 2441.73d),
                ("2013/2/7", 2430.69d, 2418.53d, 2394.22d, 2433.89d),
                ("2013/2/8", 2416.62d, 2432.4d, 2414.4d, 2443.03d),
                ("2013/2/18", 2441.91d, 2421.56d, 2415.43d, 2444.8d),
                ("2013/2/19", 2420.26d, 2382.91d, 2373.53d, 2427.07d),
                ("2013/2/20", 2383.49d, 2397.18d, 2370.61d, 2397.94d),
                ("2013/2/21", 2378.82d, 2325.95d, 2309.17d, 2378.82d),
                ("2013/2/22", 2322.94d, 2314.16d, 2308.76d, 2330.88d),
                ("2013/2/25", 2320.62d, 2325.82d, 2315.01d, 2338.78d),
                ("2013/2/26", 2313.74d, 2293.34d, 2289.89d, 2340.71d),
                ("2013/2/27", 2297.77d, 2313.22d, 2292.03d, 2324.63d),
                ("2013/2/28", 2322.32d, 2365.59d, 2308.92d, 2366.16d),
                ("2013/3/1", 2364.54d, 2359.51d, 2330.86d, 2369.65d),
                ("2013/3/4", 2332.08d, 2273.4d, 2259.25d, 2333.54d),
                ("2013/3/5", 2274.81d, 2326.31d, 2270.1d, 2328.14d),
                ("2013/3/6", 2333.61d, 2347.18d, 2321.6d, 2351.44d),
                ("2013/3/7", 2340.44d, 2324.29d, 2304.27d, 2352.02d),
                ("2013/3/8", 2326.42d, 2318.61d, 2314.59d, 2333.67d),
                ("2013/3/11", 2314.68d, 2310.59d, 2296.58d, 2320.96d),
                ("2013/3/12", 2309.16d, 2286.6d, 2264.83d, 2333.29d),
                ("2013/3/13", 2282.17d, 2263.97d, 2253.25d, 2286.33d),
                ("2013/3/14", 2255.77d, 2270.28d, 2253.31d, 2276.22d),
                ("2013/3/15", 2269.31d, 2278.4d, 2250d, 2312.08d),
                ("2013/3/18", 2267.29d, 2240.02d, 2239.21d, 2276.05d),
                ("2013/3/19", 2244.26d, 2257.43d, 2232.02d, 2261.31d),
                ("2013/3/20", 2257.74d, 2317.37d, 2257.42d, 2317.86d),
                ("2013/3/21", 2318.21d, 2324.24d, 2311.6d, 2330.81d),
                ("2013/3/22", 2321.4d, 2328.28d, 2314.97d, 2332d),
                ("2013/3/25", 2334.74d, 2326.72d, 2319.91d, 2344.89d),
                ("2013/3/26", 2318.58d, 2297.67d, 2281.12d, 2319.99d),
                ("2013/3/27", 2299.38d, 2301.26d, 2289d, 2323.48d),
                ("2013/3/28", 2273.55d, 2236.3d, 2232.91d, 2273.55d),
                ("2013/3/29", 2238.49d, 2236.62d, 2228.81d, 2246.87d),
                ("2013/4/1", 2229.46d, 2234.4d, 2227.31d, 2243.95d),
                ("2013/4/2", 2234.9d, 2227.74d, 2220.44d, 2253.42d),
                ("2013/4/3", 2232.69d, 2225.29d, 2217.25d, 2241.34d),
                ("2013/4/8", 2196.24d, 2211.59d, 2180.67d, 2212.59d),
                ("2013/4/9", 2215.47d, 2225.77d, 2215.47d, 2234.73d),
                ("2013/4/10", 2224.93d, 2226.13d, 2212.56d, 2233.04d),
                ("2013/4/11", 2236.98d, 2219.55d, 2217.26d, 2242.48d),
                ("2013/4/12", 2218.09d, 2206.78d, 2204.44d, 2226.26d),
                ("2013/4/15", 2199.91d, 2181.94d, 2177.39d, 2204.99d),
                ("2013/4/16", 2169.63d, 2194.85d, 2165.78d, 2196.43d),
                ("2013/4/17", 2195.03d, 2193.8d, 2178.47d, 2197.51d),
                ("2013/4/18", 2181.82d, 2197.6d, 2175.44d, 2206.03d),
                ("2013/4/19", 2201.12d, 2244.64d, 2200.58d, 2250.11d),
                ("2013/4/22", 2236.4d, 2242.17d, 2232.26d, 2245.12d),
                ("2013/4/23", 2242.62d, 2184.54d, 2182.81d, 2242.62d),
                ("2013/4/24", 2187.35d, 2218.32d, 2184.11d, 2226.12d),
                ("2013/4/25", 2213.19d, 2199.31d, 2191.85d, 2224.63d),
                ("2013/4/26", 2203.89d, 2177.91d, 2173.86d, 2210.58d),
                ("2013/5/2", 2170.78d, 2174.12d, 2161.14d, 2179.65d),
                ("2013/5/3", 2179.05d, 2205.5d, 2179.05d, 2222.81d),
                ("2013/5/6", 2212.5d, 2231.17d, 2212.5d, 2236.07d),
                ("2013/5/7", 2227.86d, 2235.57d, 2219.44d, 2240.26d),
                ("2013/5/8", 2242.39d, 2246.3d, 2235.42d, 2255.21d),
                ("2013/5/9", 2246.96d, 2232.97d, 2221.38d, 2247.86d),
                ("2013/5/10", 2228.82d, 2246.83d, 2225.81d, 2247.67d),
                ("2013/5/13", 2247.68d, 2241.92d, 2231.36d, 2250.85d),
                ("2013/5/14", 2238.9d, 2217.01d, 2205.87d, 2239.93d),
                ("2013/5/15", 2217.09d, 2224.8d, 2213.58d, 2225.19d),
                ("2013/5/16", 2221.34d, 2251.81d, 2210.77d, 2252.87d),
                ("2013/5/17", 2249.81d, 2282.87d, 2248.41d, 2288.09d),
                ("2013/5/20", 2286.33d, 2299.99d, 2281.9d, 2309.39d),
                ("2013/5/21", 2297.11d, 2305.11d, 2290.12d, 2305.3d),
                ("2013/5/22", 2303.75d, 2302.4d, 2292.43d, 2314.18d),
                ("2013/5/23", 2293.81d, 2275.67d, 2274.1d, 2304.95d),
                ("2013/5/24", 2281.45d, 2288.53d, 2270.25d, 2292.59d),
                ("2013/5/27", 2286.66d, 2293.08d, 2283.94d, 2301.7d),
                ("2013/5/28", 2293.4d, 2321.32d, 2281.47d, 2322.1d),
                ("2013/5/29", 2323.54d, 2324.02d, 2321.17d, 2334.33d),
                ("2013/5/30", 2316.25d, 2317.75d, 2310.49d, 2325.72d),
                ("2013/5/31", 2320.74d, 2300.59d, 2299.37d, 2325.53d),
                ("2013/6/3", 2300.21d, 2299.25d, 2294.11d, 2313.43d),
                ("2013/6/4", 2297.1d, 2272.42d, 2264.76d, 2297.1d),
                ("2013/6/5", 2270.71d, 2270.93d, 2260.87d, 2276.86d),
                ("2013/6/6", 2264.43d, 2242.11d, 2240.07d, 2266.69d),
                ("2013/6/7", 2242.26d, 2210.9d, 2205.07d, 2250.63d),
                ("2013/6/13", 2190.1d, 2148.35d, 2126.22d, 2190.1d),
            ];
    }
}
