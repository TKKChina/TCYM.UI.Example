using System.Globalization;
using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Core;
using TCYM.UI.Elements.Message;

namespace Page.Charts.Gauge
{
    /// <summary>展示由三个同心 Gauge 系列组成并按系统时间更新的模拟时钟。</summary>
    internal sealed class ClockGaugeChartDemo : GaugeChartDemoPage
    {
        private const string HourSeriesId = "clock-gauge-hour";
        private const string MinuteSeriesId = "clock-gauge-minute";
        private const string SecondSeriesId = "clock-gauge-second";
        private const string HourDataKey = "clock-hour-value";
        private const string MinuteDataKey = "clock-minute-value";
        private const string SecondDataKey = "clock-second-value";
        private const int NormalUpdateDuration = 300;

        /// <summary>三根表针共同使用的紧凑 path 符号。</summary>
        private const string PointerPath =
            "path://M2.9,0.7L2.9,0.7c1.4,0,2.6,1.2,2.6,2.6v115"
            + "c0,1.4-1.2,2.6-2.6,2.6l0,0c-1.4,0-2.6-1.2-2.6-2.6V3.3"
            + "C0.3,1.9,1.4,0.7,2.9,0.7z";

        /// <summary>小时层使用的原始 ECHARTS Logo path。</summary>
        private const string AnchorLogoPath =
            "path://M532.8,70.8C532.8,70.8,532.8,70.8,532.8,70.8L532.8,70.8"
            + "C532.7,70.8,532.8,70.8,532.8,70.8z "
            + "M456.1,49.6c-2.2-6.2-8.1-10.6-15-10.6h-37.5v10.6h37.5l0,0"
            + "c2.9,0,5.3,2.4,5.3,5.3c0,2.9-2.4,5.3-5.3,5.3v0h-22.5"
            + "c-1.5,0.1-3,0.4-4.3,0.9c-4.5,1.6-8.1,5.2-9.7,9.8"
            + "c-0.6,1.7-0.9,3.4-0.9,5.3v16h10.6v-16l0,0l0,0"
            + "c0-2.7,2.1-5,4.7-5.3h10.3l10.4,21.2h11.8l-10.4-21.2h0"
            + "c6.9,0,12.8-4.4,15-10.6c0.6-1.7,0.9-3.5,0.9-5.3"
            + "C457,53,456.7,51.2,456.1,49.6z "
            + "M388.9,92.1h11.3L381,39h-3.6h-11.3L346.8,92v0h11.3"
            + "l3.9-10.7h7.3h7.7l3.9-10.6h-7.7h-7.3l7.7-21.2v0L388.9,92.1z "
            + "M301,38.9h-10.6v53.1H301V70.8h28.4l3.7-10.6H301V38.9z"
            + "M333.2,38.9v10.6v10.7v31.9h10.6V38.9H333.2z "
            + "M249.5,81.4L249.5,81.4L249.5,81.4c-2.9,0-5.3-2.4-5.3-5.3h0"
            + "V54.9h0l0,0c0-2.9,2.4-5.3,5.3-5.3l0,0l0,0h33.6l3.9-10.6"
            + "h-37.5c-1.9,0-3.6,0.3-5.3,0.9c-4.5,1.6-8.1,5.2-9.7,9.7"
            + "c-0.6,1.7-0.9,3.5-0.9,5.3l0,0v21.3c0,1.9,0.3,3.6,0.9,5.3"
            + "c1.6,4.5,5.2,8.1,9.7,9.7c1.7,0.6,3.5,0.9,5.3,0.9h33.6"
            + "l3.9-10.6H249.5z "
            + "M176.8,38.9v10.6h49.6l3.9-10.6H176.8z "
            + "M192.7,81.4L192.7,81.4L192.7,81.4c-2.9,0-5.3-2.4-5.3-5.3l0,0"
            + "v-5.3h38.9l3.9-10.6h-53.4v10.6v5.3l0,0c0,1.9,0.3,3.6,0.9,5.3"
            + "c1.6,4.5,5.2,8.1,9.7,9.7c1.7,0.6,3.4,0.9,5.3,0.9h23.4h10.2"
            + "l3.9-10.6l0,0H192.7z "
            + "M460.1,38.9v10.6h21.4v42.5h10.6V49.6h17.5l3.8-10.6H460.1z "
            + "M541.6,68.2c-0.2,0.1-0.4,0.3-0.7,0.4"
            + "C541.1,68.4,541.4,68.3,541.6,68.2L541.6,68.2z "
            + "M554.3,60.2h-21.6v0l0,0c-2.9,0-5.3-2.4-5.3-5.3"
            + "c0-2.9,2.4-5.3,5.3-5.3l0,0l0,0h33.6l3.8-10.6h-37.5l0,0"
            + "c-6.9,0-12.8,4.4-15,10.6c-0.6,1.7-0.9,3.5-0.9,5.3"
            + "c0,1.9,0.3,3.7,0.9,5.3c2.2,6.2,8.1,10.6,15,10.6h21.6l0,0"
            + "c2.9,0,5.3,2.4,5.3,5.3c0,2.9-2.4,5.3-5.3,5.3l0,0h-37.5"
            + "v10.6h37.5c6.9,0,12.8-4.4,15-10.6c0.6-1.7,0.9-3.5,0.9-5.3"
            + "c0-1.9-0.3-3.7-0.9-5.3C567.2,64.6,561.3,60.2,554.3,60.2z";

        private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(1d);
        private static readonly SKColor HandColor = new(192, 145, 31);
        private static readonly SKColor AnchorLogoColor = new(112, 113, 119);
        private static readonly SKColor HandShadowColor = new(0, 0, 0, 77);

        /// <summary>控制每秒系统时间更新循环的取消源。</summary>
        private CancellationTokenSource? _updateCancellation;

        /// <summary>创建三针时钟仪表盘页面。</summary>
        internal ClockGaugeChartDemo()
            : base(
                "时钟仪表盘",
                "三个全圆 Gauge 分别映射小时、分钟和秒钟，并每秒读取本机系统时间。",
                "小时、分钟、秒钟各自拥有稳定系列 Id 与数据 Key；小时层保留附件中的原始 ECHARTS Logo Anchor，三根表针使用相同的短 path。任一表针回绕到 0 时该次更新立即完成，其余更新使用 300ms 动画。",
                CreateOption(DateTime.Now))
        {
        }

        /// <summary>页面显示后启动系统时钟更新循环。</summary>
        /// <param name="fromPath">来源路由路径。</param>
        protected override void OnDemoRouteEnter(string? fromPath)
        {
            _ = fromPath;
            if (_updateCancellation is not null)
            {
                return;
            }

            _updateCancellation = CancellationTokenSource.CreateLinkedTokenSource(LifetimeToken);
            _ = RunClockUpdatesAsync(_updateCancellation.Token);
        }

        /// <summary>页面离开时停止时钟任务，防止释放后继续更新 UIChart。</summary>
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

        /// <summary>按一秒间隔读取系统时间，并将 Gauge 更新投递到 UI 线程。</summary>
        private async Task RunClockUpdatesAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(UpdateInterval);
            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    var now = DateTime.Now;
                    await UIDispatcher.InvokeAsync(() => UpdateClock(now, cancellationToken));
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // 路由离开属于预期结束，不向用户显示错误。
            }
            catch (Exception exception)
            {
                await UIDispatcher.InvokeAsync(() =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    UIMessage.Error(
                        $"时钟仪表盘更新已停止：{exception.Message}",
                        duration: 3f,
                        key: "clock-gauge-update-error");
                });
            }
        }

        /// <summary>根据同一个系统时间快照同步更新三根表针。</summary>
        private void UpdateClock(DateTime now, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var (hour, minute, second) = GetClockValues(now);
            Chart.SetOption(new ChartOption
            {
                Series =
                [
                    CreateClockPatch(
                        HourSeriesId, "hour", HourDataKey, hour, "cubicOut"),
                    CreateClockPatch(
                        MinuteSeriesId, "minute", MinuteDataKey, minute, "cubicOut"),
                    CreateClockPatch(
                        SecondSeriesId, "second", SecondDataKey, second, "cubicOut"),
                ],
            }, new ChartSetOptionOptions
            {
                Silent = true,
            });
        }

        /// <summary>创建三层完整 Gauge 配置和当前系统时间数据。</summary>
        private static ChartOption CreateOption(DateTime now)
        {
            var (hour, minute, second) = GetClockValues(now);
            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 1000,
                    Easing = "cubicOut",
                    UpdateDuration = NormalUpdateDuration,
                    UpdateEasing = "cubicOut",
                },
                Series =
                [
                    CreateHourSeries(hour),
                    CreateMinuteSeries(minute),
                    CreateSecondSeries(second),
                ],
            };
        }

        /// <summary>创建带表盘、数字、小时指针和简化 Logo Anchor 的小时 Gauge。</summary>
        private static ChartGaugeSeriesOption CreateHourSeries(double value)
        {
            var series = CreateClockFrame(
                HourSeriesId, "hour", HourDataKey, value, maximum: 12d, splitNumber: 12);
            series.Z = 1;
            series.AxisLine.LineStyle.Width = 15f;
            series.AxisLine.LineStyle.Color.Add(new ChartGaugeColorStop(
                1d,
                ChartBrush.Solid(new SKColor(0, 0, 0, 179))));
            series.AxisLabel.FontSize = 30f;
            series.AxisLabel.Distance = 25f;
            series.AxisLabel.FormatterCallback = static scaleValue =>
                Math.Abs(scaleValue) < double.Epsilon
                    ? string.Empty
                    : scaleValue.ToString("0", CultureInfo.InvariantCulture);
            series.Anchor.Show = true;
            series.Anchor.Icon = AnchorLogoPath;
            series.Anchor.ShowAbove = false;
            series.Anchor.OffsetCenter = ChartGaugeCenter.Percent(0f, -35f);
            series.Anchor.Size = 120f;
            series.Anchor.KeepAspect = true;
            series.Anchor.ItemStyle.Color = ChartBrush.Solid(AnchorLogoColor);
            ConfigurePointer(series, width: 12f, lengthPercent: 55f);
            return series;
        }

        /// <summary>创建隐藏刻度、仅保留分钟指针和中心环的分钟 Gauge。</summary>
        private static ChartGaugeSeriesOption CreateMinuteSeries(double value)
        {
            var series = CreateClockFrame(
                MinuteSeriesId, "minute", MinuteDataKey, value, maximum: 60d, splitNumber: 60);
            series.Z = 2;
            HideScale(series);
            ConfigurePointer(series, width: 8f, lengthPercent: 70f);
            series.Anchor.Show = true;
            series.Anchor.Size = 20f;
            series.Anchor.ShowAbove = false;
            series.Anchor.ItemStyle.Color = ChartBrush.Solid(SKColors.White);
            series.Anchor.ItemStyle.BorderWidth = 15f;
            series.Anchor.ItemStyle.BorderColor = ChartBrush.Solid(HandColor);
            ConfigureShadow(series.Anchor.ItemStyle);
            return series;
        }

        /// <summary>创建最长、最细的秒钟 Gauge。</summary>
        private static ChartGaugeSeriesOption CreateSecondSeries(double value)
        {
            var series = CreateClockFrame(
                SecondSeriesId, "second", SecondDataKey, value, maximum: 60d, splitNumber: 60);
            series.Z = 3;
            series.AnimationEasingUpdate = "cubicOut";
            HideScale(series);
            ConfigurePointer(series, width: 4f, lengthPercent: 85f);
            series.Anchor.Show = true;
            series.Anchor.Size = 15f;
            series.Anchor.ShowAbove = true;
            series.Anchor.ItemStyle.Color = ChartBrush.Solid(HandColor);
            ConfigureShadow(series.Anchor.ItemStyle);
            return series;
        }

        /// <summary>创建三根表针共用的全圆 Gauge 框架和稳定数据项。</summary>
        private static ChartGaugeSeriesOption CreateClockFrame(
            string seriesId,
            string seriesName,
            string dataKey,
            double value,
            double maximum,
            int splitNumber)
        {
            return new ChartGaugeSeriesOption
            {
                Id = seriesId,
                Name = seriesName,
                Center = ChartGaugeCenter.Percent(50f, 49f),
                Radius = ChartLength.Percent(90f),
                StartAngle = 90f,
                EndAngle = -270f,
                Minimum = 0d,
                Maximum = maximum,
                SplitNumber = splitNumber,
                Clockwise = true,
                AnimationDurationUpdate = GetUpdateDuration(value),
                AnimationEasingUpdate = "cubicOut",
                Title = new()
                {
                    Show = false,
                },
                Detail = new()
                {
                    Show = false,
                },
                DataSource = CreateClockData(dataKey, value),
            };
        }

        /// <summary>隐藏分钟和秒钟层的表盘元素，避免重复绘制。</summary>
        private static void HideScale(ChartGaugeSeriesOption series)
        {
            series.AxisLine.Show = false;
            series.SplitLine.Show = false;
            series.AxisTick.Show = false;
            series.AxisLabel.Show = false;
        }

        /// <summary>给指定 Gauge 配置用户示例中的短 path 指针、尺寸、位置和阴影。</summary>
        private static void ConfigurePointer(
            ChartGaugeSeriesOption series,
            float width,
            float lengthPercent)
        {
            series.Pointer.Icon = PointerPath;
            series.Pointer.Width = width;
            series.Pointer.Length = ChartLength.Percent(lengthPercent);
            series.Pointer.OffsetCenter = ChartGaugeCenter.Percent(0f, 8f);
            series.Pointer.ItemStyle.Color = ChartBrush.Solid(HandColor);
            ConfigureShadow(series.Pointer.ItemStyle);
        }

        /// <summary>为指针或 Anchor 应用一致的轻量阴影。</summary>
        private static void ConfigureShadow(ChartItemStyleOption style)
        {
            style.ShadowColor = ChartBrush.Solid(HandShadowColor);
            style.ShadowBlur = 8f;
            style.ShadowOffsetX = 2f;
            style.ShadowOffsetY = 4f;
        }

        /// <summary>创建只更新一个稳定系列值及动画时长的 Gauge 补丁。</summary>
        private static ChartGaugeSeriesOption CreateClockPatch(
            string seriesId,
            string seriesName,
            string dataKey,
            double value,
            string easing)
        {
            return new ChartGaugeSeriesOption
            {
                Id = seriesId,
                Name = seriesName,
                AnimationDurationUpdate = GetUpdateDuration(value),
                AnimationEasingUpdate = easing,
                DataSource = CreateClockData(dataKey, value),
            };
        }

        /// <summary>把系统时间转换为连续的时、分和离散的秒三个 Gauge 数值。</summary>
        private static (double Hour, double Minute, double Second) GetClockValues(DateTime now)
        {
            var second = (double)now.Second;
            var minute = now.Minute + second / 60d;
            var hour = now.Hour % 12 + minute / 60d;
            return (hour, minute, second);
        }

        /// <summary>回绕到零时关闭该根表针的更新动画，其余更新使用 300ms。</summary>
        private static int GetUpdateDuration(double value) =>
            Math.Abs(value) < double.Epsilon ? 0 : NormalUpdateDuration;

        /// <summary>创建带稳定 Id 和 Key 的单项时钟 Gauge 数据源。</summary>
        private static ChartGaugeData CreateClockData(string key, double value)
        {
            return new ChartGaugeData().Add(new ChartGaugeDataItem(value)
            {
                Id = key,
                Key = key,
            });
        }
    }
}
