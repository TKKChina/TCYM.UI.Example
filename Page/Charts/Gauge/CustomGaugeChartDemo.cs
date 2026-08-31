using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Core;
using TCYM.UI.Elements.Message;

namespace Page.Charts.Gauge
{
    /// <summary>展示跨越负值和正值区间的双层自定义仪表盘。</summary>
    internal sealed class CustomGaugeChartDemo : GaugeChartDemoPage
    {
        /// <summary>仪表盘初次显示的数值。</summary>
        private const double InitialValue = 100d;

        /// <summary>仪表盘最小值。</summary>
        private const double MinimumValue = -180d;

        /// <summary>仪表盘最大值。</summary>
        private const double MaximumValue = 180d;

        /// <summary>灰色未激活区间的颜色。</summary>
        private static readonly ChartBrush InactiveBrush =
            ChartBrush.Solid(new SKColor(225, 225, 225, 102));

        /// <summary>相邻两次随机数值更新之间的时间间隔。</summary>
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(2d);

        /// <summary>控制动态更新循环的取消源。</summary>
        private CancellationTokenSource? _updateCancellation;

        /// <summary>创建双层自定义仪表盘页面。</summary>
        internal CustomGaugeChartDemo()
            : base(
                "自定义仪表",
                "每 2 秒在 -180～180 区间生成一个随机值，并同步更新渐变区间、指针和数值。",
                "外层 Gauge 只绘制 16px 区间轨道：正值高亮 0 到当前值，负值高亮当前值到 0；内层 Gauge 绘制刻度、标签、渐变指针和中心 Anchor，离开页面后动态任务立即停止。",
                CreateOption(InitialValue))
        {
        }

        /// <summary>页面显示后启动每两秒更新一次的随机数值循环。</summary>
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

        /// <summary>页面离开时先停止动态任务，再由基类清理图表资源。</summary>
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

        /// <summary>按固定间隔生成随机值，并把图表更新投递到 UI 线程。</summary>
        /// <param name="cancellationToken">页面离开或释放时触发的取消令牌。</param>
        private async Task RunRealtimeUpdatesAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(UpdateInterval);
            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    var value = Math.Round(
                        MinimumValue
                            + Random.Shared.NextDouble() * (MaximumValue - MinimumValue),
                        2,
                        MidpointRounding.AwayFromZero);
                    await UIDispatcher.InvokeAsync(
                        () => UpdateValue(value, cancellationToken));
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
                        $"自定义仪表更新已停止：{exception.Message}",
                        duration: 3f,
                        key: "custom-gauge-update-error");
                });
            }
        }

        /// <summary>在同一个 SetOption 补丁中同步更新指针、详情和正负区间颜色。</summary>
        /// <param name="value">本次生成的 -180～180 随机值。</param>
        /// <param name="cancellationToken">用于阻止离页后已排队的 UI 更新。</param>
        private void UpdateValue(double value, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var gradient = CreateBlueGradient();
            var indicatorPatch = new ChartGaugeSeriesOption
            {
                Id = "custom-gauge-indicator",
                AnimationDurationUpdate = 0,
                DataSource = CreateData("custom-gauge-indicator-value", value),
            };
            var rangePatch = new ChartGaugeSeriesOption
            {
                Id = "custom-gauge-range",
                AnimationDurationUpdate = 0,
                DataSource = CreateData("custom-gauge-range-value", value),
            };
            foreach (var stop in CreateRangeStops(value, gradient))
            {
                rangePatch.AxisLine.LineStyle.Color.Add(stop);
            }

            Chart.SetOption(new ChartOption
            {
                Series =
                [
                    indicatorPatch,
                    rangePatch,
                ],
            }, new ChartSetOptionOptions
            {
                Silent = true,
            });
        }

        /// <summary>创建包含刻度层和进度轨道层的完整配置。</summary>
        /// <param name="value">仪表盘初次显示的值。</param>
        /// <returns>自定义仪表盘使用的完整配置。</returns>
        private static ChartOption CreateOption(double value)
        {
            var gradient = CreateBlueGradient();

            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Series =
                [
                    CreateIndicatorSeries(value, gradient),
                    CreateRangeSeries(value, gradient),
                ],
            };
        }

        /// <summary>创建负责刻度、指针、中心锚点和数值文本的内层 Gauge。</summary>
        /// <param name="value">当前仪表盘数值。</param>
        /// <param name="gradient">指针与锚点边框共用的蓝色渐变。</param>
        /// <returns>半径为 83% 的主仪表盘系列。</returns>
        private static ChartGaugeSeriesOption CreateIndicatorSeries(
            double value,
            ChartBrush gradient)
        {
            return new ChartGaugeSeriesOption
            {
                Id = "custom-gauge-indicator",
                Name = "内gauge",
                StartAngle = 215f,
                EndAngle = -35f,
                Minimum = MinimumValue,
                Maximum = MaximumValue,
                Radius = ChartLength.Percent(83f),
                AxisLine = new()
                {
                    Show = false,
                },
                AxisTick = new()
                {
                    Length = ChartLength.Pixels(10f),
                    LineStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(153, 153, 153)),
                    },
                },
                SplitLine = new()
                {
                    Length = ChartLength.Pixels(15f),
                    LineStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(153, 153, 153)),
                    },
                },
                AxisLabel = new()
                {
                    FontSize = 16,
                    Color = ChartBrush.Solid(new SKColor(153, 153, 153)),
                },
                Pointer = new()
                {
                    Show = true,
                    Width = 17f,
                    Length = ChartLength.Percent(50f),
                    ItemStyle = new()
                    {
                        Color = gradient,
                        ShadowColor = ChartBrush.Solid(new SKColor(0, 138, 255, 115)),
                        ShadowBlur = 10f,
                        ShadowOffsetX = 2f,
                        ShadowOffsetY = 2f,
                    },
                },
                Anchor = new()
                {
                    Show = true,
                    ShowAbove = true,
                    Size = 16f,
                    ItemStyle = new()
                    {
                        Color = ChartBrush.Solid(SKColors.White),
                        BorderColor = gradient,
                        BorderWidth = 20f,
                    },
                },
                Title = new()
                {
                    Show = false,
                },
                Detail = new()
                {
                    Show = true,
                    FormatterCallback = static current => $"{current:F2}",
                    Color = ChartBrush.Solid(new SKColor(21, 152, 255)),
                    FontSize = 40f,
                    OffsetCenter = ChartGaugeCenter.Pixels(0f, 100f),
                },
                DataSource = CreateData("custom-gauge-indicator-value", value),
            };
        }

        /// <summary>创建只负责突出零点到当前值区间的外层 Gauge。</summary>
        /// <param name="value">当前仪表盘数值。</param>
        /// <param name="gradient">激活区间使用的蓝色渐变。</param>
        /// <returns>半径为 90% 的细轨道系列。</returns>
        private static ChartGaugeSeriesOption CreateRangeSeries(
            double value,
            ChartBrush gradient)
        {
            var series = new ChartGaugeSeriesOption
            {
                Id = "custom-gauge-range",
                Name = "外gauge",
                StartAngle = 215.5f,
                EndAngle = -35.5f,
                Minimum = MinimumValue,
                Maximum = MaximumValue,
                Radius = ChartLength.Percent(90f),
                AxisLine = new()
                {
                    Show = true,
                    LineStyle = new()
                    {
                        Width = 16f,
                    },
                },
                AxisTick = new()
                {
                    Show = false,
                },
                SplitLine = new()
                {
                    Show = false,
                },
                AxisLabel = new()
                {
                    Show = false,
                },
                Pointer = new()
                {
                    Show = false,
                },
                Anchor = new()
                {
                    Show = false,
                },
                Title = new()
                {
                    Show = false,
                },
                Detail = new()
                {
                    Show = false,
                },
                DataSource = CreateData("custom-gauge-range-value", value),
            };

            foreach (var stop in CreateRangeStops(value, gradient))
            {
                series.AxisLine.LineStyle.Color.Add(stop);
            }

            return series;
        }

        /// <summary>根据当前值创建灰色与渐变高亮区间的颜色断点。</summary>
        /// <param name="value">当前仪表盘数值。</param>
        /// <param name="gradient">零点到当前值区间使用的渐变。</param>
        /// <returns>按 Gauge 比例升序排列的颜色断点。</returns>
        private static IReadOnlyList<ChartGaugeColorStop> CreateRangeStops(
            double value,
            ChartBrush gradient)
        {
            var zeroPosition = NormalizePosition(0d);
            var valuePosition = NormalizePosition(value);
            if (value > 0d)
            {
                return
                [
                    new ChartGaugeColorStop(zeroPosition, InactiveBrush),
                    new ChartGaugeColorStop(valuePosition, gradient),
                    new ChartGaugeColorStop(1d, InactiveBrush),
                ];
            }

            if (value < 0d)
            {
                return
                [
                    new ChartGaugeColorStop(valuePosition, InactiveBrush),
                    new ChartGaugeColorStop(zeroPosition, gradient),
                    new ChartGaugeColorStop(1d, InactiveBrush),
                ];
            }

            return
            [
                new ChartGaugeColorStop(1d, InactiveBrush),
            ];
        }

        /// <summary>创建从青色过渡到蓝色的水平方向渐变。</summary>
        /// <returns>可安全由多个 Gauge 图元共享的不可变渐变画刷。</returns>
        private static ChartBrush CreateBlueGradient()
        {
            return new ChartLinearGradientBrush(
                new SKPoint(0f, 0f),
                new SKPoint(1f, 0f),
                [
                    new SKColor(65, 215, 243),
                    new SKColor(61, 159, 255),
                ],
                [0f, 1f]);
        }

        /// <summary>把值域中的数值转换为 Gauge 颜色断点使用的 0～1 比例。</summary>
        /// <param name="value">要转换的有限数值。</param>
        /// <returns>位于 0～1 的归一化位置。</returns>
        private static double NormalizePosition(double value)
        {
            return Math.Clamp(
                (value - MinimumValue) / (MaximumValue - MinimumValue),
                0d,
                1d);
        }

        /// <summary>创建带稳定键的单值 Gauge 数据。</summary>
        /// <param name="key">当前系列的稳定数据项键。</param>
        /// <param name="value">当前仪表盘数值。</param>
        /// <returns>显示当前值和业务名称的数据源。</returns>
        private static ChartGaugeData CreateData(string key, double value)
        {
            return new ChartGaugeData().Add(new ChartGaugeDataItem(
                value,
                "年售电量情况")
            {
                Key = key,
            });
        }
    }
}
