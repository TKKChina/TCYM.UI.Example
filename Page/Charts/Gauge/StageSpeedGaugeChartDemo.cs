using SkiaSharp;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Core;
using TCYM.UI.Elements.Message;

namespace Page.Charts.Gauge
{
    /// <summary>展示按三个速度阶段着色并每两秒更新数据的动态仪表盘。</summary>
    internal sealed class StageSpeedGaugeChartDemo : GaugeChartDemoPage
    {
        /// <summary>阶段速度仪表盘用于普通 SetOption 合并的稳定系列标识。</summary>
        private const string SeriesId = "stage-speed-gauge";

        /// <summary>阶段速度数据项用于更新动画匹配的稳定标识。</summary>
        private const string DataKey = "stage-speed-gauge-value";

        /// <summary>示例首次进入时显示的速度。</summary>
        private const double InitialValue = 70d;

        /// <summary>相邻两次随机速度更新之间的时间间隔。</summary>
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(2d);

        /// <summary>控制动态速度更新循环的取消源。</summary>
        private CancellationTokenSource? _updateCancellation;

        /// <summary>创建阶段速度仪表盘示例页面。</summary>
        internal StageSpeedGaugeChartDemo()
            : base(
                "阶段速度仪表盘",
                "使用三段轴线颜色区分低速、中速和高速区间。",
                "指针、刻度文字和详情通过 Auto/Inherited 颜色跟随当前速度所属区间；页面显示后每两秒生成一个保留两位小数的新速度。",
                CreateOption())
        {
        }

        /// <summary>页面显示后启动每两秒更新一次的速度循环。</summary>
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

        /// <summary>等待固定间隔，并将随机速度更新投递到 UI 线程。</summary>
        /// <param name="cancellationToken">页面离开或释放时触发的取消令牌。</param>
        private async Task RunRealtimeUpdatesAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(UpdateInterval);
            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    var value = Math.Round(
                        Random.Shared.NextDouble() * 100d,
                        2,
                        MidpointRounding.AwayFromZero);
                    await UIDispatcher.InvokeAsync(() => UpdateValue(value, cancellationToken));
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
                        $"阶段速度仪表盘更新已停止：{exception.Message}",
                        duration: 3f,
                        key: "stage-speed-gauge-update-error");
                });
            }
        }

        /// <summary>只提交稳定系列的数据补丁，保留完整的静态样式配置。</summary>
        /// <param name="value">范围为 0～100 的新速度。</param>
        /// <param name="cancellationToken">用于阻止离页后已排队的 UI 更新。</param>
        private void UpdateValue(double value, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Chart.SetOption(new ChartOption
            {
                Series =
                [
                    new ChartGaugeSeriesOption
                    {
                        Id = SeriesId,
                        DataSource = CreateData(value),
                    },
                ],
            }, new ChartSetOptionOptions
            {
                Silent = true,
            });
        }

        /// <summary>创建阶段速度仪表盘的完整初始配置。</summary>
        /// <returns>包含分段轴弧、继承颜色、详情和稳定数据标识的配置。</returns>
        private static ChartOption CreateOption()
        {
            var lowSpeed = ChartBrush.Solid(new SKColor(103, 224, 227));
            var middleSpeed = ChartBrush.Solid(new SKColor(55, 162, 218));
            var highSpeed = ChartBrush.Solid(new SKColor(253, 102, 109));
            var white = ChartBrush.Solid(SKColors.White);

            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 800,
                    UpdateDuration = 500,
                    UpdateEasing = "cubicOut",
                },
                Series =
                [
                    new ChartGaugeSeriesOption
                    {
                        Id = SeriesId,
                        Name = "阶段速度",
                        StartAngle = 215f,
                        EndAngle = -35f,
                        Minimum = 0d,
                        Maximum = 100d,
                        AxisLine = new()
                        {
                            LineStyle = new()
                            {
                                Width = 30f,
                                Color =
                                {
                                    new ChartGaugeColorStop(0.3d, lowSpeed),
                                    new ChartGaugeColorStop(0.7d, middleSpeed),
                                    new ChartGaugeColorStop(1d, highSpeed),
                                },
                            },
                        },
                        Pointer = new()
                        {
                            ItemStyle = new()
                            {
                                ColorSource = ChartColorSource.Auto,
                            },
                        },
                        AxisTick = new()
                        {
                            Distance = -30f,
                            Length = ChartLength.Pixels(8f),
                            LineStyle = new()
                            {
                                Color = white,
                                Width = 2f,
                            },
                        },
                        SplitLine = new()
                        {
                            Distance = -30f,
                            Length = ChartLength.Pixels(30f),
                            LineStyle = new()
                            {
                                Color = white,
                                Width = 4f,
                            },
                        },
                        AxisLabel = new()
                        {
                            ColorSource = ChartColorSource.Inherit,
                            Distance = 40f,
                            FontSize = 20f,
                        },
                        Detail = new()
                        {
                            ValueAnimation = true,
                            Formatter = "{value} km/h",
                            ColorSource = ChartColorSource.Inherit,
                        },
                        DataSource = CreateData(InitialValue),
                    },
                ],
            };
        }

        /// <summary>创建带稳定 Id 和 Key 的单项阶段速度数据。</summary>
        /// <param name="value">当前速度。</param>
        /// <returns>可以直接赋给 Gauge 系列的数据源。</returns>
        private static ChartGaugeData CreateData(double value)
        {
            return new ChartGaugeData().Add(new ChartGaugeDataItem(value, "速度")
            {
                Id = DataKey,
                Key = DataKey,
            });
        }
    }
}
