using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Core;
using TCYM.UI.Elements.Message;

namespace Page.Charts.Gauge
{
    /// <summary>展示两个同心 Gauge 系列同步更新的动态气温仪表盘。</summary>
    internal sealed class TemperatureGaugeChartDemo : GaugeChartDemoPage
    {
        private const string MainSeriesId = "temperature-gauge-main";
        private const string AccentSeriesId = "temperature-gauge-accent";
        private const string MainDataKey = "temperature-main-value";
        private const string AccentDataKey = "temperature-accent-value";
        private const double InitialTemperature = 20d;
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(2d);
        private static readonly SKColor MainColor = new(255, 171, 145);
        private static readonly SKColor AccentColor = new(253, 115, 71);
        private static readonly SKColor ScaleColor = new(153, 153, 153);

        /// <summary>控制两秒一次同步更新循环的取消源。</summary>
        private CancellationTokenSource? _updateCancellation;

        /// <summary>创建气温仪表盘示例页面。</summary>
        internal TemperatureGaugeChartDemo()
            : base(
                "气温仪表盘",
                "两个同心 Gauge 共用 0～60 °C 值域，并以不同宽度绘制同一个温度。",
                "主 Gauge 提供刻度、数值和 30px 进度环，第二个 Gauge 叠加 8px 强调环；两者每 2 秒通过稳定 Id 与 Key 同步更新，离开页面后立即停止任务。",
                CreateOption(InitialTemperature))
        {
        }

        /// <summary>页面显示后启动气温随机更新循环。</summary>
        protected override void OnDemoRouteEnter(string? fromPath)
        {
            _ = fromPath;
            if (_updateCancellation is not null)
            {
                return;
            }

            _updateCancellation = CancellationTokenSource.CreateLinkedTokenSource(LifetimeToken);
            _ = RunTemperatureUpdatesAsync(_updateCancellation.Token);
        }

        /// <summary>页面离开时先停止动态任务，再由基类释放图表。</summary>
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

        /// <summary>每两秒生成一个两位小数的随机温度，并把更新投递到 UI 线程。</summary>
        private async Task RunTemperatureUpdatesAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(UpdateInterval);
            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    var temperature = Math.Round(
                        Random.Shared.NextDouble() * 60d,
                        2,
                        MidpointRounding.AwayFromZero);
                    await UIDispatcher.InvokeAsync(() =>
                        UpdateTemperature(temperature, cancellationToken));
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
                        $"气温仪表盘更新已停止：{exception.Message}",
                        duration: 3f,
                        key: "temperature-gauge-update-error");
                });
            }
        }

        /// <summary>用同一个温度值增量更新主环与强调环。</summary>
        private void UpdateTemperature(double temperature, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Chart.SetOption(new ChartOption
            {
                Series =
                [
                    CreateTemperaturePatch(MainSeriesId, MainDataKey, "当前气温", temperature),
                    CreateTemperaturePatch(AccentSeriesId, AccentDataKey, "强调进度", temperature),
                ],
            }, new ChartSetOptionOptions
            {
                Silent = true,
            });
        }

        /// <summary>创建两个完整 Gauge 系列及初始温度。</summary>
        private static ChartOption CreateOption(double temperature)
        {
            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 600,
                    Easing = "cubicOut",
                    UpdateDuration = 500,
                    UpdateEasing = "cubicInOut",
                },
                Series =
                [
                    CreateMainSeries(temperature),
                    CreateAccentSeries(temperature),
                ],
            };
        }

        /// <summary>创建包含刻度、详情文本和宽进度环的主 Gauge。</summary>
        private static ChartGaugeSeriesOption CreateMainSeries(double temperature)
        {
            var series = CreateSeriesFrame(
                MainSeriesId, "气温", MainDataKey, "当前气温", temperature);
            series.Z = 2;
            series.ItemStyle.Color = ChartBrush.Solid(MainColor);
            series.Progress.Show = true;
            series.Progress.Width = 30f;
            series.Pointer.Show = false;
            series.AxisLine.LineStyle.Width = 30f;
            series.AxisTick.Distance = -45f;
            series.AxisTick.SplitNumber = 5;
            series.AxisTick.LineStyle.Width = 2f;
            series.AxisTick.LineStyle.Color = ChartBrush.Solid(ScaleColor);
            series.SplitLine.Distance = -52f;
            series.SplitLine.Length = ChartLength.Pixels(14f);
            series.SplitLine.LineStyle.Width = 3f;
            series.SplitLine.LineStyle.Color = ChartBrush.Solid(ScaleColor);
            series.AxisLabel.Distance = -20f;
            series.AxisLabel.Color = ChartBrush.Solid(ScaleColor);
            series.AxisLabel.FontSize = 20f;
            series.Anchor.Show = false;
            series.Title.Show = false;
            series.Detail.ValueAnimation = true;
            series.Detail.Width = ChartLength.Percent(60f);
            series.Detail.LineHeight = 40f;
            series.Detail.BorderRadius = 8f;
            series.Detail.OffsetCenter = ChartGaugeCenter.Percent(0f, -15f);
            series.Detail.FontSize = 60f;
            series.Detail.FontWeight = 700;
            series.Detail.Formatter = "{value} °C";
            series.Detail.ColorSource = ChartColorSource.Inherit;
            return series;
        }

        /// <summary>创建只绘制内侧细进度环的第二个 Gauge。</summary>
        private static ChartGaugeSeriesOption CreateAccentSeries(double temperature)
        {
            var series = CreateSeriesFrame(
                AccentSeriesId, "气温强调环", AccentDataKey, "强调进度", temperature);
            series.Z = 3;
            series.ItemStyle.Color = ChartBrush.Solid(AccentColor);
            series.Progress.Show = true;
            series.Progress.Width = 8f;
            series.Pointer.Show = false;
            series.AxisLine.Show = false;
            series.AxisTick.Show = false;
            series.SplitLine.Show = false;
            series.AxisLabel.Show = false;
            series.Anchor.Show = false;
            series.Title.Show = false;
            series.Detail.Show = false;
            return series;
        }

        /// <summary>创建两个温度系列共用的圆心、角度、值域和稳定数据。</summary>
        private static ChartGaugeSeriesOption CreateSeriesFrame(
            string seriesId,
            string seriesName,
            string dataKey,
            string dataName,
            double temperature)
        {
            return new ChartGaugeSeriesOption
            {
                Id = seriesId,
                Name = seriesName,
                Center = ChartGaugeCenter.Percent(50f, 60f),
                StartAngle = 200f,
                EndAngle = -20f,
                Minimum = 0d,
                Maximum = 60d,
                SplitNumber = 12,
                AnimationDurationUpdate = 500,
                AnimationEasingUpdate = "cubicInOut",
                DataSource = CreateTemperatureData(dataKey, dataName, temperature),
            };
        }

        /// <summary>创建只携带新值且不会覆盖静态样式的最小 Gauge 补丁。</summary>
        private static ChartGaugeSeriesOption CreateTemperaturePatch(
            string seriesId,
            string dataKey,
            string dataName,
            double temperature)
        {
            return new ChartGaugeSeriesOption
            {
                Id = seriesId,
                AnimationDurationUpdate = 500,
                AnimationEasingUpdate = "cubicInOut",
                DataSource = CreateTemperatureData(dataKey, dataName, temperature),
            };
        }

        /// <summary>创建包含稳定 Id、Key 和温度值的单项 Gauge 数据源。</summary>
        private static ChartGaugeData CreateTemperatureData(
            string key,
            string name,
            double temperature)
        {
            return new ChartGaugeData().Add(new ChartGaugeDataItem(temperature, name)
            {
                Id = key,
                Key = key,
            });
        }
    }
}
