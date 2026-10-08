using System.Globalization;
using System.Text.Json;
using SkiaSharp;
using TCYM.UI.Chart.Components.Brush;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Components.DataZoom;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Chart.Text;
using TCYM.UI.Elements;

namespace Page.Charts.Candlestick
{
    /// <summary>DJI 日 K、四条均线和成交量的联动刷选案例。</summary>
    internal sealed class BrushCandlestickChartDemo : CandlestickChartDemoPage
    {
        private static readonly SKColor UpColor = SKColor.Parse("#00da3c");
        private static readonly SKColor DownColor = SKColor.Parse("#ec0000");
        private static readonly ChartBrush VolumeUpBrush = ChartBrush.Solid(UpColor);
        private static readonly ChartBrush VolumeDownBrush = ChartBrush.Solid(DownColor);
        private const string BrushId = "dji-brush";
        private bool _restoreOnNextLayout;

        internal BrushCandlestickChartDemo()
            : base(
                "K 线图刷选",
                "Dow-Jones index、MA5 / MA10 / MA20 / MA30 与成交量联动。",
                "默认选择 2016-06-02—2016-06-20。点击“横向刷选”后在图中拖动选择日期，区间外透明度降至 10%；切换“平移”可拖动行情窗口。上下图共用日期和 DataZoom。",
                CreateOption())
        {
            // 当前 toolbox 尚无完整 UI，使用普通按钮调用已经支持的公共 action。
            AddToolbarButton("横向刷选", () => Chart.DispatchAction(new ChartBrushTakeGlobalCursorAction(
                ChartBrushType.LineX, ChartBrushMode.Single)));
            AddToolbarButton("平移", () => Chart.DispatchAction(new ChartBrushTakeGlobalCursorAction(null)));
            AddToolbarButton("清除刷选", () => Chart.DispatchAction(new ChartBrushAction(Array.Empty<ChartBrushArea>())));
            AddToolbarButton("恢复示例", RestoreExample);
        }

        public override void OnRouteEnter(string? fromPath)
        {
            _ = fromPath;
            _restoreOnNextLayout = true;
        }

        public override void Layout(float x, float y, float width, float height)
        {
            base.Layout(x, y, width, height);
            if (!_restoreOnNextLayout || Chart.Width <= 0f || Chart.Height <= 0f)
            {
                return;
            }

            // 默认Brush/DataZoom必须出现在首个可见画面；等真实画布尺寸确定后再执行，
            // 避免OnRouteEnter在0x0内容区提前触发完整构建和随后一次尺寸重建。
            _restoreOnNextLayout = false;
            RestoreExample();
        }

        private void RestoreExample()
        {
            Chart.DispatchAction(new ChartDataZoomAction(98d, 100d));
            Chart.DispatchAction(new ChartBrushAction([
                new ChartBrushLineXArea(("2016-06-02", "2016-06-20"))
                {
                    Id = "dji-default-area",
                    Target = new ChartBrushCartesianTarget(gridIndex: 0, xAxisIndex: 0, yAxisIndex: 0),
                },
            ], new ChartBrushComponentSelector(id: BrushId)));
            Chart.DispatchAction(new ChartBrushTakeGlobalCursorAction(ChartBrushType.LineX, ChartBrushMode.Single));
        }

        private static ChartOption CreateOption()
        {
            var rows = ReadHistory();
            var candleData = new ChartCandlestickData();
            var volumeData = new ChartDataTable()
                .AddDimension("x")
                .AddDimension("volume")
                .AddDimension("sign");
            foreach (var (row, index) in rows.Select((row, index) => (row, index)))
            {
                // 三类数据使用相同日期 Key，BrushLink 才能按身份联动，而不是依赖当前可见索引。
                candleData.AddOhlc(
                    row.Open,
                    row.Close,
                    row.Low,
                    row.High,
                    name: row.Date,
                    key: row.Date);
                var sign = row.Open > row.Close ? 1d : -1d;
                volumeData.AddItem(
                    [index, row.Volume, sign],
                    name: row.Date,
                    key: row.Date);
            }

            var candle = new ChartCandlestickSeriesOption
            {
                Id = "dji-candles", Name = "Dow-Jones index",
                DataSource = candleData,
                Large = false, Progressive = 0,
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(UpColor), Color0 = ChartBrush.Solid(DownColor),
                    BorderColor = ChartBrush.Solid(UpColor), BorderColor0 = ChartBrush.Solid(DownColor),
                },
            };
            var volume = new ChartBarSeriesOption
            {
                Id = "dji-volume", Name = "Volume",
                XAxisIndex = 1, YAxisIndex = 1,
                DataSource = volumeData,
                ItemStyle = new() { Color = ChartBrush.Solid(SKColor.Parse("#7fbe9e")) },
            };
            // 第三维sign只决定成交量颜色；复用静态画刷避免为每个日期创建富Bar数据项。
            volume.ItemStyle.ColorCallback = static (in ChartFormatterParameters parameters) =>
                parameters.DimensionValues.Count > 2
                && parameters.DimensionValues[2].TryGetDouble(out var sign)
                    ? sign > 0d ? VolumeDownBrush : VolumeUpBrush
                    : null;
            volume.Encode.Set("x", 0).Set("y", 1);

            var option = new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new() { Enabled = false },
                Legend = new ChartLegendOption
                {
                    Left = "center", Bottom = "8px",
                    Data = { "Dow-Jones index", "MA5", "MA10", "MA20", "MA30" },
                },
                Tooltip = new ChartTooltipOption
                {
                    Trigger = ChartTooltipTrigger.Axis, Confine = true,
                    BorderWidth = 1f, BorderColor = ChartBrush.Solid(SKColor.Parse("#cccccc")),
                    Padding = new ChartInsets(10f),
                    TextStyle = new() { Color = ChartBrush.Solid(SKColors.Black) },
                    AxisPointer = new() { Type = ChartTooltipAxisPointerType.Cross, Precision = 2 },
                    Position = ChartTooltipPosition.FromCallback((in ChartTooltipPositionContext context) =>
                        ChartTooltipPosition.At(
                            context.Point.X < context.ViewSize.Width / 2f
                                ? Math.Max(30f, context.ViewSize.Width - context.ContentSize.Width - 30f)
                                : 30f,
                            10f)),
                    FormatterCallback = (in ChartTooltipFormatterContext context) =>
                        FormatTooltip(in context, rows),
                },
                Grid = new ChartGridOption
                {
                    Id = "dji-price-grid",
                    Left = "10%", Right = "8%", Top = "45px", Height = "48%",
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                },
                XAxis =
                [
                    new ChartXAxisOption
                    {
                        Id = "dji-price-x", Type = ChartAxisType.Category,
                        Data = rows.Select(row => ChartValue.From(row.Date)).ToArray(),
                        BoundaryGap = false, AxisLine = new() { OnZero = false },
                        SplitLine = new() { Show = false },
                        Minimum = ChartAxisBound.DataMinimum, Maximum = ChartAxisBound.DataMaximum,
                        AxisLabel = new() { HideOverlap = true, FontSize = 11f },
                    },
                    new ChartXAxisOption
                    {
                        Id = "dji-volume-x", Type = ChartAxisType.Category, GridIndex = 1,
                        Data = rows.Select(row => ChartValue.From(row.Date)).ToArray(),
                        BoundaryGap = false, AxisLine = new() { OnZero = false },
                        AxisTick = new() { Show = false }, AxisLabel = new() { Show = false },
                        SplitLine = new() { Show = false },
                        Minimum = ChartAxisBound.DataMinimum, Maximum = ChartAxisBound.DataMaximum,
                    },
                ],
                YAxis =
                [
                    new ChartYAxisOption
                    {
                        Type = ChartAxisType.Value, Scale = true, SplitArea = new() { Show = true },
                        AxisLabel = new() { FontSize = 11f },
                    },
                    new ChartYAxisOption
                    {
                        Type = ChartAxisType.Value, GridIndex = 1, Scale = true, SplitNumber = 2,
                        Minimum = ChartAxisBound.Fixed(0d),
                        AxisLabel = new() { Show = false }, AxisLine = new() { Show = false },
                        AxisTick = new() { Show = false }, SplitLine = new() { Show = false },
                    },
                ],
                DataZoom =
                [
                    new ChartDataZoomInsideOption
                    {
                        Id = "dji-inside", XAxisIndex = [0, 1], Start = 98d, End = 100d,
                        ZoomOnMouseWheel = ChartDataZoomInteractionTrigger.Ctrl,
                    },
                    new ChartDataZoomSliderOption
                    {
                        Id = "dji-slider", XAxisIndex = [0, 1], Start = 98d, End = 100d,
                        Left = "10%", Right = "8%", Top = "85%", Height = "30px",
                        Detail = new() { Show = false },
                    },
                ],
                Brush = new ChartBrushOption
                {
                    Id = BrushId, BrushType = ChartBrushType.LineX, BrushMode = ChartBrushMode.Single,
                    SeriesIndex = [0, 5], BrushLink = [0, 1, 2, 3, 4, 5],
                    OutOfBrush = new() { Opacity = 0.1f },
                },
                Series =
                [
                    candle,
                    CreateMovingAverage(rows, 5, SKColor.Parse("#5470c6")),
                    CreateMovingAverage(rows, 10, SKColor.Parse("#91cc75")),
                    CreateMovingAverage(rows, 20, SKColor.Parse("#fac858")),
                    CreateMovingAverage(rows, 30, SKColor.Parse("#ee6666")),
                    volume,
                ],
            };
            option.Components.Add(new ChartGridOption
            {
                Id = "dji-volume-grid", Left = "10%", Right = "8%", Top = "63%", Height = "16%",
                OuterBoundsMode = ChartGridOuterBoundsMode.None,
            });
            return option;
        }

        private static ChartLineSeriesOption CreateMovingAverage(HistoryRow[] rows, int days, SKColor color)
        {
            var data = new ChartLineData();
            for (var index = 0; index < rows.Length; index++)
            {
                var value = ChartValue.Null;
                if (index >= days)
                {
                    var total = 0d;
                    for (var offset = 0; offset < days; offset++) total += rows[index - offset].Close;
                    value = Math.Round(total / days, 3);
                }
                data.AddValue(value, name: rows[index].Date, key: rows[index].Date);
            }
            return new ChartLineSeriesOption
            {
                Id = $"dji-ma{days}", Name = $"MA{days}",
                DataSource = data, Smooth = ChartLineSmooth.Automatic, ShowSymbol = false,
                LineStyle = new() { Color = ChartBrush.Solid(color), Opacity = 0.5f, Width = 1.5f },
                ItemStyle = new() { Color = ChartBrush.Solid(color) },
            };
        }

        private static ChartTooltipContent? FormatTooltip(
            in ChartTooltipFormatterContext context, HistoryRow[] rows)
        {
            if (context.Parameters.Count == 0) return null;
            var first = context.Parameters[0];
            var index = first.DataIndex;
            if (index < 0 || index >= rows.Length) return null;
            var row = rows[index];
            var lines = new List<ChartTooltipContentLine>();
            // 用同一天的原始 OHLC 生成价格提示，均线或成交量命中时也不会丢失价格字段。
            foreach (var (name, value) in new[] { ("开盘", row.Open), ("收盘", row.Close), ("最低", row.Low), ("最高", row.High) })
                lines.Add(new ChartTooltipContentLine(name, [value.ToString("F2", context.Culture)],
                    ChartBrush.Solid(row.Close >= row.Open ? UpColor : DownColor)));

            foreach (var item in context.Parameters)
            {
                if (item.SeriesType == "line")
                {
                    lines.Add(new ChartTooltipContentLine(item.SeriesName,
                        [item.Value.Primary.TryGetDouble(out var value) ? value.ToString("F3", context.Culture) : "-"], item.Color));
                }
            }
            lines.Add(new ChartTooltipContentLine("Volume", [row.Volume.ToString("N0", context.Culture)],
                ChartBrush.Solid(row.Close >= row.Open ? UpColor : DownColor)));
            return ChartTooltipContent.FromSections([new ChartTooltipContentSection(row.Date, lines)]);
        }

        /// <summary>数据来自 Apache ECharts stock-DJI.json；JsonDocument 解析无需反射，支持 NativeAOT。</summary>
        private static HistoryRow[] ReadHistory()
        {
            using var stream = typeof(BrushCandlestickChartDemo).Assembly.GetManifestResourceStream(
                "TCYM.UI.Example.Page.Charts.Candlestick.stock-DJI.json")
                ?? throw new InvalidOperationException("缺少内嵌 DJI 历史数据。");
            using var document = JsonDocument.Parse(stream);
            return document.RootElement.EnumerateArray()
                .Select(row => new HistoryRow(
                    row[0].GetString()!, row[1].GetDouble(), row[2].GetDouble(),
                    row[3].GetDouble(), row[4].GetDouble(), row[5].GetDouble()))
                .ToArray();
        }

        private readonly record struct HistoryRow(
            string Date, double Open, double Close, double Low, double High, double Volume);
    }
}
