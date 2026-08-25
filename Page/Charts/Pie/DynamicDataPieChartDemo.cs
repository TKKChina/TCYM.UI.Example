using System.Globalization;
using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Chart.Text;
using TCYM.UI.Core;
using TCYM.UI.Elements.Message;

namespace Page.Charts.Pie
{
    /// <summary>展示每秒随机更新百分比的动态饼图进度环。</summary>
    internal sealed class DynamicDataPieChartDemo : PieChartDemoPage
    {
        /// <summary>中心百分比标题使用的稳定组件标识。</summary>
        private const string TitleId = "dynamic-pie-title";

        /// <summary>第一层背景环使用的稳定系列标识。</summary>
        private const string BackgroundRingId = "dynamic-pie-background";

        /// <summary>第二层扩散阴影背景环使用的稳定系列标识。</summary>
        private const string BackgroundShadowRingId = "dynamic-pie-background-shadow";

        /// <summary>动态进度环使用的稳定系列标识。</summary>
        private const string MainSeriesId = "dynamic-pie-main";

        /// <summary>示例初次显示的进度比例。</summary>
        private const double InitialPercent = 0.7d;

        /// <summary>相邻两次随机进度更新之间的时间间隔。</summary>
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(1d);

        /// <summary>深色预览画布背景色。</summary>
        private static readonly SKColor CanvasBackgroundColor = new(51, 54, 69);

        /// <summary>背景环填充色。</summary>
        private static readonly SKColor RingBackgroundColor = new(49, 52, 67);

        /// <summary>背景环阴影色。</summary>
        private static readonly SKColor RingShadowColor = new(27, 30, 37);

        /// <summary>动态进度环使用的玫红色。</summary>
        private static readonly SKColor ProgressColor = new(251, 53, 138);

        /// <summary>控制每秒随机更新循环的取消源。</summary>
        private CancellationTokenSource? _updateCancellation;

        /// <summary>创建动态数据饼图页面。</summary>
        internal DynamicDataPieChartDemo()
            : base(
                "饼图动态数据",
                "每秒生成一个随机百分比，并通过普通 SetOption 增量更新进度环。",
                "两层静态 Pie 叠加背景与阴影，最外层细 Pie 显示动态进度；中心 Title 和主系列都依靠稳定 Id 合并，因此更新时不会重建背景环。",
                CreateOption(InitialPercent))
        {
        }

        /// <summary>页面显示后启动每秒更新循环。</summary>
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

        /// <summary>页面离开时先停止定时更新，再由基类清理图表资源。</summary>
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

        /// <summary>等待固定间隔并把随机进度更新投递到 UI 线程。</summary>
        /// <param name="cancellationToken">页面离开或释放时触发的取消令牌。</param>
        private async Task RunRealtimeUpdatesAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(UpdateInterval);
            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    var percent = Random.Shared.NextDouble();
                    await UIDispatcher.InvokeAsync(() => UpdatePercent(percent, cancellationToken));
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // 路由离开属于预期结束，不需要向用户显示错误。
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
                        $"动态饼图更新已停止：{exception.Message}",
                        duration: 3f,
                        key: "dynamic-pie-update-error");
                });
            }
        }

        /// <summary>仅更新中心标题和主进度系列，保留两层静态背景环。</summary>
        /// <param name="percent">范围为 0 到 1 的新进度比例。</param>
        /// <param name="cancellationToken">用于阻止离页后已经排队的 UI 更新。</param>
        private void UpdatePercent(double percent, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Chart.SetOption(new ChartOption
            {
                Components =
                {
                    new ChartTitleOption
                    {
                        Id = TitleId,
                        Text = FormatPercent(percent),
                    },
                },
                Series =
                [
                    new ChartPieSeriesOption
                    {
                        Id = MainSeriesId,
                        DataSource = CreateProgressData(percent),
                    },
                ],
            }, new ChartSetOptionOptions
            {
                Silent = true,
            });
        }

        /// <summary>创建标题、两层背景环和动态进度环的完整初始配置。</summary>
        /// <param name="percent">初次显示的进度比例。</param>
        /// <returns>可以直接提交给 UIChart 的完整饼图配置。</returns>
        private static ChartOption CreateOption(double percent)
        {
            return new ChartOption
            {
                Background = ChartBrush.Solid(CanvasBackgroundColor),
                Animation = new()
                {
                    Enabled = true,
                    UpdateDuration = 500,
                    UpdateEasing = "cubicInOut",
                },
                Components =
                {
                    CreateCenterTitle(percent),
                },
                Series =
                [
                    CreateBackgroundSeries(
                        BackgroundRingId,
                        "背景环",
                        shadowBlur: 10f,
                        z: 0),
                    CreateBackgroundSeries(
                        BackgroundShadowRingId,
                        "背景扩散阴影",
                        shadowBlur: 50f,
                        z: 1),
                    CreateMainSeries(percent),
                ],
            };
        }

        /// <summary>创建位于画布正中心的百分比标题。</summary>
        /// <param name="percent">要格式化为整数百分比的进度比例。</param>
        /// <returns>带稳定 Id 的中心 Title 组件。</returns>
        private static ChartTitleOption CreateCenterTitle(double percent)
        {
            return new ChartTitleOption
            {
                Id = TitleId,
                Text = FormatPercent(percent),
                Left = ChartLength.Percent(50f),
                Top = ChartLength.Percent(50f),
                TextAlign = ChartTextAlign.Center,
                TextVerticalAlign = ChartTextVerticalAlign.Middle,
                Padding = new ChartInsets(0f),
                Silent = true,
                Z = 10,
                TextStyle = new ChartTextStyle
                {
                    Color = ChartBrush.Solid(new SKColor(152, 160, 196)),
                    FontSize = 64f,
                    FontWeight = 700,
                    Align = ChartTextAlign.Center,
                    VerticalAlign = ChartTextVerticalAlign.Middle,
                },
            };
        }

        /// <summary>创建一层关闭动画、标签和交互的静态背景环。</summary>
        /// <param name="id">用于普通 SetOption 合并的稳定系列标识。</param>
        /// <param name="name">便于阅读配置的系列名称。</param>
        /// <param name="shadowBlur">当前背景层使用的阴影模糊半径。</param>
        /// <param name="z">当前背景层的绘制顺序。</param>
        /// <returns>包含单个完整圆数据项的背景 Pie 系列。</returns>
        private static ChartPieSeriesOption CreateBackgroundSeries(
            string id,
            string name,
            float shadowBlur,
            int z)
        {
            var data = new ChartPieData();
            data.Add(new ChartPieDataItem(1d, name)
            {
                Key = $"{id}-value",
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(RingBackgroundColor),
                    ShadowBlur = shadowBlur,
                    ShadowColor = ChartBrush.Solid(RingShadowColor),
                },
            });

            return new ChartPieSeriesOption
            {
                Id = id,
                Name = name,
                Z = z,
                Silent = true,
                Radius = new ChartPieRadius(
                    ChartLength.Percent(39f),
                    ChartLength.Percent(49f)),
                Label = new()
                {
                    Show = false,
                },
                LabelLine = new()
                {
                    Show = false,
                },
                AnimationDuration = 0,
                AnimationDurationUpdate = 0,
                DataSource = data,
            };
        }

        /// <summary>创建位于背景环外侧的动态细进度环。</summary>
        /// <param name="percent">范围为 0 到 1 的进度比例。</param>
        /// <returns>带更新动画和稳定 Id 的主 Pie 系列。</returns>
        private static ChartPieSeriesOption CreateMainSeries(double percent)
        {
            return new ChartPieSeriesOption
            {
                Id = MainSeriesId,
                Name = "main",
                Z = 2,
                Radius = new ChartPieRadius(
                    ChartLength.Percent(50f),
                    ChartLength.Percent(51f)),
                Label = new()
                {
                    Show = false,
                },
                LabelLine = new()
                {
                    Show = false,
                },
                AnimationDurationUpdate = 500,
                AnimationEasingUpdate = "cubicInOut",
                DataSource = CreateProgressData(percent),
            };
        }

        /// <summary>创建进度和透明剩余量两项数据，使 Pie 始终按完整圆计算角度。</summary>
        /// <param name="percent">范围为 0 到 1 的进度比例。</param>
        /// <returns>包含稳定 Key 的两项 Pie 数据。</returns>
        private static ChartPieData CreateProgressData(double percent)
        {
            percent = Math.Clamp(percent, 0d, 1d);
            var data = new ChartPieData();
            data.Add(new ChartPieDataItem(percent, "进度")
            {
                Key = "dynamic-pie-progress",
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(ProgressColor),
                    ShadowBlur = 10f,
                    ShadowColor = ChartBrush.Solid(ProgressColor),
                },
            });
            data.Add(new ChartPieDataItem(1d - percent, "剩余")
            {
                Key = "dynamic-pie-remainder",
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(SKColors.Transparent),
                },
            });
            return data;
        }

        /// <summary>按照 ECharts 示例的 toFixed(0) 规则格式化中心百分比。</summary>
        /// <param name="percent">范围为 0 到 1 的进度比例。</param>
        /// <returns>不带小数位的百分比文本。</returns>
        private static string FormatPercent(double percent) =>
            Math.Round(percent * 100d, 0, MidpointRounding.AwayFromZero)
                .ToString("F0", CultureInfo.InvariantCulture) + "%";
    }
}
