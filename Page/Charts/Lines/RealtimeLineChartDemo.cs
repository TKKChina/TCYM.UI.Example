using System.Globalization;
using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.CoordinateSystems.Scales;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Extensions;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Core;
using TCYM.UI.Elements.Message;

namespace Page.Charts.Lines
{
    /// <summary>展示按固定时间间隔实时追加监测值的折线图。</summary>
    internal sealed class RealtimeLineChartDemo : LineChartDemoPage
    {
        /// <summary>图表初次显示的历史数据点数量。</summary>
        private const int InitialPointCount = 20;

        /// <summary>相邻模拟数据之间的时间跨度。</summary>
        private static readonly TimeSpan SampleInterval = TimeSpan.FromSeconds(1);

        /// <summary>实时折线图使用的蓝色。</summary>
        private static readonly SKColor LineColor = new(22, 119, 255);

        /// <summary>下一次追加数据使用的时间。</summary>
        private DateTimeOffset _nextSampleTime;

        /// <summary>下一次追加数据使用的递增序号。</summary>
        private int _nextSampleIndex;

        /// <summary>控制实时追加循环的取消源。</summary>
        private CancellationTokenSource? _updateCancellation;

        /// <summary>创建实时追加折线图示例页面。</summary>
        internal RealtimeLineChartDemo()
            : base(
                "实时追加折线图",
                "每秒生成一个模拟监测值，并通过 AppendData 增量追加到折线系列。",
                "页面先展示 20 个历史点，进入后每秒追加并保留一个新点；时间轴随数据自动扩展，Y 轴保持固定范围，离开页面时实时任务立即停止。",
                CreateOption(out var nextSampleTime, out var nextSampleIndex))
        {
            _nextSampleTime = nextSampleTime;
            _nextSampleIndex = nextSampleIndex;
        }

        /// <summary>页面显示后启动实时追加循环。</summary>
        /// <param name="fromPath">来源路由路径。</param>
        protected override void OnDemoRouteEnter(string? fromPath)
        {
            _ = fromPath;
            if (_updateCancellation is not null)
            {
                return;
            }

            _updateCancellation = CancellationTokenSource.CreateLinkedTokenSource(LifetimeToken);
            _ = RunRealtimeUpdatesAsync(_updateCancellation.Token);
        }

        /// <summary>页面离开时先停止追加循环，再由基类释放图表。</summary>
        /// <param name="toPath">目标路由路径。</param>
        protected override void OnDemoRouteLeave(string? toPath)
        {
            _ = toPath;
            var cancellation = Interlocked.Exchange(ref _updateCancellation, null);
            if (cancellation is null)
            {
                return;
            }

            cancellation.Cancel();
            cancellation.Dispose();
        }

        /// <summary>按固定间隔生成数据，并把图表修改投递回 UI 线程。</summary>
        /// <param name="cancellationToken">页面离开或释放时触发的取消令牌。</param>
        private async Task RunRealtimeUpdatesAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(SampleInterval);
            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    await UIDispatcher.InvokeAsync(() => AppendNextSample(cancellationToken));
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // 路由离开属于预期结束，不需要上报异常。
            }
            catch (Exception exception)
            {
                await UIDispatcher.InvokeAsync(() =>
                    UIMessage.Error(
                        $"实时追加已停止：{exception.Message}",
                        duration: 3f,
                        key: "realtime-line-update-error"));
            }
        }

        /// <summary>创建一个新模拟点，并通过公开 AppendData API 增量提交。</summary>
        /// <param name="cancellationToken">用于阻止已经排队的 UI 更新在页面离开后执行。</param>
        private void AppendNextSample(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Chart.AppendData(0, CreateDataTable().AddRow(
                _nextSampleTime,
                CreateSimulationValue(_nextSampleIndex)));
            _nextSampleTime = _nextSampleTime.Add(SampleInterval);
            _nextSampleIndex++;
        }

        /// <summary>创建实时折线图的完整配置与首批历史数据。</summary>
        /// <param name="nextSampleTime">返回第一次实时追加使用的时间。</param>
        /// <param name="nextSampleIndex">返回第一次实时追加使用的序号。</param>
        /// <returns>包含时间轴、Tooltip、初始数据和更新动画的完整配置。</returns>
        private static ChartOption CreateOption(
            out DateTimeOffset nextSampleTime,
            out int nextSampleIndex)
        {
            // 1. 使用固定起点生成可复现的历史数据，便于理解和重复运行示例。
            var now = DateTimeOffset.UtcNow;
            var firstSampleTime = now
                .AddSeconds(-(InitialPointCount - 1))
                .AddTicks(-(now.Ticks % TimeSpan.TicksPerSecond));
            var data = CreateDataTable();
            for (var index = 0; index < InitialPointCount; index++)
            {
                data.AddRow(
                    firstSampleTime.AddSeconds(index),
                    CreateSimulationValue(index));
            }

            nextSampleTime = firstSampleTime.AddSeconds(InitialPointCount);
            nextSampleIndex = InitialPointCount;

            // 2. 每个 Demo 的完整 Option 都保留在自己的类中，复制本文件即可看懂配置。
            var series = new ChartLineSeriesOption
            {
                Id = "realtime-value",
                Name = "实时值",
                ShowSymbol = true,
                ShowAllSymbol = ChartLineShowAllSymbol.All,
                Symbol = "circle",
                SymbolSize = 7f,
                Clip = true,
                DataSource = data,
                AnimationDurationUpdate = 450,
                AnimationEasingUpdate = "cubicOut",
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
                    BorderColor = ChartBrush.Solid(SKColors.White),
                    BorderWidth = 1.5f,
                },
                Label = new()
                {
                    Show = false,
                },
                EndLabel = new()
                {
                    Show = false,
                },
                Emphasis = new()
                {
                    Focus = "series",
                    BlurScope = "coordinateSystem",
                },
            };
            series.Encode
                .Set("x", "time")
                .Set("y", "value")
                .Set("tooltip", "value");

            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 600,
                    UpdateDuration = 450,
                    UpdateEasing = "cubicOut",
                },
                Tooltip = new()
                {
                    Trigger = ChartTooltipTrigger.Axis,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove,
                    ShowContent = true,
                    Confine = true,
                    ValueFormatter = static (value, _) =>
                    {
                        var displayValue = value.Values.Count > 1
                            ? value.Values[^1]
                            : value.Primary;
                        return displayValue.TryGetDouble(out var number)
                            ? number.ToString("F2", CultureInfo.InvariantCulture)
                            : "-";
                    },
                    AxisPointer = new()
                    {
                        Type = ChartTooltipAxisPointerType.Line,
                        Axis = ChartTooltipAxisPointerAxis.X,
                        Snap = true,
                        Animation = ChartTooltipAxisPointerAnimation.Disabled,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(100, 108, 120, 170)),
                            Width = 1f,
                            Type = "dashed",
                        },
                    },
                },
                Grid = new ChartGridOption
                {
                    Show = true,
                    Left = "12px",
                    Top = "24px",
                    Right = "56px",
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
                    Type = ChartAxisType.Time,
                    Scale = true,
                    AxisLabel = new()
                    {
                        Show = true,
                        Color = ChartBrush.Solid(new SKColor(45, 52, 65)),
                        FontSize = 13f,
                        FormatterCallback = static (in ChartAxisLabelFormatterContext context) =>
                            FormatTime(context.Value, "HH:mm:ss"),
                    },
                    SplitLine = new()
                    {
                        Show = false,
                    },
                },
                YAxis = new ChartYAxisOption
                {
                    Name = "模拟值",
                    NameLocation = ChartAxisNameLocation.Start,
                    NameGap = 12f,
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(20d),
                    Maximum = ChartAxisBound.Fixed(80d),
                    Interval = 10d,
                    Scale = true,
                    AxisLabel = new()
                    {
                        Show = true,
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
                Series = series,
            };
        }

        /// <summary>创建 AppendData 所需的两列数据表。</summary>
        /// <returns>包含 time 与 value 维度的列布局表格。</returns>
        private static ChartDataTable CreateDataTable() =>
            new ChartDataTable()
                .AddDimension("time", ChartDimensionType.Time)
                .AddDimension("value", ChartDimensionType.Number);

        /// <summary>根据递增序号生成包含趋势和周期波动的确定性模拟值。</summary>
        /// <param name="index">从零开始的数据序号。</param>
        /// <returns>保留两位小数且落在固定 Y 轴范围内的模拟值。</returns>
        private static double CreateSimulationValue(int index)
        {
            var value = 48d
                + 10d * Math.Sin(index * 0.42d)
                + 4d * Math.Sin(index * 0.13d)
                + 0.08d * (index % 20);
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>把 Time 轴值格式化为指定的本地时间文本。</summary>
        /// <param name="value">时间轴刻度值。</param>
        /// <param name="format">DateTimeOffset 格式字符串。</param>
        /// <returns>格式化后的本地时间，无法读取时返回短横线。</returns>
        private static string FormatTime(ChartValue value, string format)
        {
            if (value.ToObject() is DateTimeOffset offset)
            {
                return offset.ToLocalTime().ToString(format, CultureInfo.InvariantCulture);
            }

            if (value.ToObject() is DateTime dateTime)
            {
                return dateTime.ToLocalTime().ToString(format, CultureInfo.InvariantCulture);
            }

            return value.Kind == ChartValueKind.Number
                && value.TryGetDouble(out var unixMilliseconds)
                && ChartTimeValue.TryFromUnixMilliseconds(unixMilliseconds, out var parsed)
                    ? parsed.ToLocalTime().ToString(format, CultureInfo.InvariantCulture)
                    : "-";
        }
    }
}
