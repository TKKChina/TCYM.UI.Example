using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Core;
using TCYM.UI.Elements.Message;

namespace Page.Charts.Scatter
{
    /// <summary>使用普通 Scatter 的尺寸更新动画近似展示 effectScatter 涟漪效果。</summary>
    internal sealed class EffectScatterChartDemo : ScatterChartDemoPage
    {
        private const string OrdinarySeriesId = "effect-scatter-ordinary";
        private const string HighlightSeriesId = "effect-scatter-highlight";
        private const string FirstRingSeriesId = "effect-scatter-ring-first";
        private const string SecondRingSeriesId = "effect-scatter-ring-second";
        private const string HeightAxisId = "effect-scatter-height";
        private const string WeightAxisId = "effect-scatter-weight";

        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(650d);
        private static readonly SKColor OrdinaryColor = new(80, 112, 221);
        private static readonly SKColor HighlightColor = new(238, 102, 102);

        /// <summary>需求中两个需要突出显示的数据点。</summary>
        private static readonly (double Height, double Weight)[] HighlightPoints =
        [
            (172.7d, 105.2d),
            (153.4d, 42d),
        ];

        /// <summary>需求中原样保留的 260 个普通身高/体重样本。</summary>
        private static readonly (double Height, double Weight)[] OrdinaryPoints =
        [
            (161.2d, 51.6d),
            (167.5d, 59.0d),
            (159.5d, 49.2d),
            (157.0d, 63.0d),
            (155.8d, 53.6d),
            (170.0d, 59.0d),
            (159.1d, 47.6d),
            (166.0d, 69.8d),
            (176.2d, 66.8d),
            (160.2d, 75.2d),
            (172.5d, 55.2d),
            (170.9d, 54.2d),
            (172.9d, 62.5d),
            (153.4d, 42.0d),
            (160.0d, 50.0d),
            (147.2d, 49.8d),
            (168.2d, 49.2d),
            (175.0d, 73.2d),
            (157.0d, 47.8d),
            (167.6d, 68.8d),
            (159.5d, 50.6d),
            (175.0d, 82.5d),
            (166.8d, 57.2d),
            (176.5d, 87.8d),
            (170.2d, 72.8d),
            (174.0d, 54.5d),
            (173.0d, 59.8d),
            (179.9d, 67.3d),
            (170.5d, 67.8d),
            (160.0d, 47.0d),
            (154.4d, 46.2d),
            (162.0d, 55.0d),
            (176.5d, 83.0d),
            (160.0d, 54.4d),
            (152.0d, 45.8d),
            (162.1d, 53.6d),
            (170.0d, 73.2d),
            (160.2d, 52.1d),
            (161.3d, 67.9d),
            (166.4d, 56.6d),
            (168.9d, 62.3d),
            (163.8d, 58.5d),
            (167.6d, 54.5d),
            (160.0d, 50.2d),
            (161.3d, 60.3d),
            (167.6d, 58.3d),
            (165.1d, 56.2d),
            (160.0d, 50.2d),
            (170.0d, 72.9d),
            (157.5d, 59.8d),
            (167.6d, 61.0d),
            (160.7d, 69.1d),
            (163.2d, 55.9d),
            (152.4d, 46.5d),
            (157.5d, 54.3d),
            (168.3d, 54.8d),
            (180.3d, 60.7d),
            (165.5d, 60.0d),
            (165.0d, 62.0d),
            (164.5d, 60.3d),
            (156.0d, 52.7d),
            (160.0d, 74.3d),
            (163.0d, 62.0d),
            (165.7d, 73.1d),
            (161.0d, 80.0d),
            (162.0d, 54.7d),
            (166.0d, 53.2d),
            (174.0d, 75.7d),
            (172.7d, 61.1d),
            (167.6d, 55.7d),
            (151.1d, 48.7d),
            (164.5d, 52.3d),
            (163.5d, 50.0d),
            (152.0d, 59.3d),
            (169.0d, 62.5d),
            (164.0d, 55.7d),
            (161.2d, 54.8d),
            (155.0d, 45.9d),
            (170.0d, 70.6d),
            (176.2d, 67.2d),
            (170.0d, 69.4d),
            (162.5d, 58.2d),
            (170.3d, 64.8d),
            (164.1d, 71.6d),
            (169.5d, 52.8d),
            (163.2d, 59.8d),
            (154.5d, 49.0d),
            (159.8d, 50.0d),
            (173.2d, 69.2d),
            (170.0d, 55.9d),
            (161.4d, 63.4d),
            (169.0d, 58.2d),
            (166.2d, 58.6d),
            (159.4d, 45.7d),
            (162.5d, 52.2d),
            (159.0d, 48.6d),
            (162.8d, 57.8d),
            (159.0d, 55.6d),
            (179.8d, 66.8d),
            (162.9d, 59.4d),
            (161.0d, 53.6d),
            (151.1d, 73.2d),
            (168.2d, 53.4d),
            (168.9d, 69.0d),
            (173.2d, 58.4d),
            (171.8d, 56.2d),
            (178.0d, 70.6d),
            (164.3d, 59.8d),
            (163.0d, 72.0d),
            (168.5d, 65.2d),
            (166.8d, 56.6d),
            (172.7d, 105.2d),
            (163.5d, 51.8d),
            (169.4d, 63.4d),
            (167.8d, 59.0d),
            (159.5d, 47.6d),
            (167.6d, 63.0d),
            (161.2d, 55.2d),
            (160.0d, 45.0d),
            (163.2d, 54.0d),
            (162.2d, 50.2d),
            (161.3d, 60.2d),
            (149.5d, 44.8d),
            (157.5d, 58.8d),
            (163.2d, 56.4d),
            (172.7d, 62.0d),
            (155.0d, 49.2d),
            (156.5d, 67.2d),
            (164.0d, 53.8d),
            (160.9d, 54.4d),
            (162.8d, 58.0d),
            (167.0d, 59.8d),
            (160.0d, 54.8d),
            (160.0d, 43.2d),
            (168.9d, 60.5d),
            (158.2d, 46.4d),
            (156.0d, 64.4d),
            (160.0d, 48.8d),
            (167.1d, 62.2d),
            (158.0d, 55.5d),
            (167.6d, 57.8d),
            (156.0d, 54.6d),
            (162.1d, 59.2d),
            (173.4d, 52.7d),
            (159.8d, 53.2d),
            (170.5d, 64.5d),
            (159.2d, 51.8d),
            (157.5d, 56.0d),
            (161.3d, 63.6d),
            (162.6d, 63.2d),
            (160.0d, 59.5d),
            (168.9d, 56.8d),
            (165.1d, 64.1d),
            (162.6d, 50.0d),
            (165.1d, 72.3d),
            (166.4d, 55.0d),
            (160.0d, 55.9d),
            (152.4d, 60.4d),
            (170.2d, 69.1d),
            (162.6d, 84.5d),
            (170.2d, 55.9d),
            (158.8d, 55.5d),
            (172.7d, 69.5d),
            (167.6d, 76.4d),
            (162.6d, 61.4d),
            (167.6d, 65.9d),
            (156.2d, 58.6d),
            (175.2d, 66.8d),
            (172.1d, 56.6d),
            (162.6d, 58.6d),
            (160.0d, 55.9d),
            (165.1d, 59.1d),
            (182.9d, 81.8d),
            (166.4d, 70.7d),
            (165.1d, 56.8d),
            (177.8d, 60.0d),
            (165.1d, 58.2d),
            (175.3d, 72.7d),
            (154.9d, 54.1d),
            (158.8d, 49.1d),
            (172.7d, 75.9d),
            (168.9d, 55.0d),
            (161.3d, 57.3d),
            (167.6d, 55.0d),
            (165.1d, 65.5d),
            (175.3d, 65.5d),
            (157.5d, 48.6d),
            (163.8d, 58.6d),
            (167.6d, 63.6d),
            (165.1d, 55.2d),
            (165.1d, 62.7d),
            (168.9d, 56.6d),
            (162.6d, 53.9d),
            (164.5d, 63.2d),
            (176.5d, 73.6d),
            (168.9d, 62.0d),
            (175.3d, 63.6d),
            (159.4d, 53.2d),
            (160.0d, 53.4d),
            (170.2d, 55.0d),
            (162.6d, 70.5d),
            (167.6d, 54.5d),
            (162.6d, 54.5d),
            (160.7d, 55.9d),
            (160.0d, 59.0d),
            (157.5d, 63.6d),
            (162.6d, 54.5d),
            (152.4d, 47.3d),
            (170.2d, 67.7d),
            (165.1d, 80.9d),
            (172.7d, 70.5d),
            (165.1d, 60.9d),
            (170.2d, 63.6d),
            (170.2d, 54.5d),
            (170.2d, 59.1d),
            (161.3d, 70.5d),
            (167.6d, 52.7d),
            (167.6d, 62.7d),
            (165.1d, 86.3d),
            (162.6d, 66.4d),
            (152.4d, 67.3d),
            (168.9d, 63.0d),
            (170.2d, 73.6d),
            (175.2d, 62.3d),
            (175.2d, 57.7d),
            (160.0d, 55.4d),
            (165.1d, 104.1d),
            (174.0d, 55.5d),
            (170.2d, 77.3d),
            (160.0d, 80.5d),
            (167.6d, 64.5d),
            (167.6d, 72.3d),
            (167.6d, 61.4d),
            (154.9d, 58.2d),
            (162.6d, 81.8d),
            (175.3d, 63.6d),
            (171.4d, 53.4d),
            (157.5d, 54.5d),
            (165.1d, 53.6d),
            (160.0d, 60.0d),
            (174.0d, 73.6d),
            (162.6d, 61.4d),
            (174.0d, 55.5d),
            (162.6d, 63.6d),
            (161.3d, 60.9d),
            (156.2d, 60.0d),
            (149.9d, 46.8d),
            (169.5d, 57.3d),
            (160.0d, 64.1d),
            (175.3d, 63.6d),
            (169.5d, 67.3d),
            (160.0d, 75.5d),
            (172.7d, 68.2d),
            (162.6d, 61.4d),
            (157.5d, 76.8d),
            (176.5d, 71.8d),
            (164.4d, 55.5d),
            (160.7d, 48.6d),
            (174.0d, 66.4d),
            (163.8d, 67.3d),
        ];

        private CancellationTokenSource? _updateCancellation;
        private bool _ringsReversed;

        /// <summary>创建涟漪特效散点图示例页面。</summary>
        internal EffectScatterChartDemo()
            : base(
                "涟漪特效散点图",
                "在身高与体重样本中持续突出两个重点数据点。",
                "当前框架没有原生 effectScatter 系列；本示例使用普通 Scatter 叠加重点点与两个 Silent 空心环，并每 650ms 反相更新两层环的尺寸，以更新动画近似涟漪效果。",
                CreateOption())
        {
        }

        /// <summary>页面显示后启动两层空心环的反相更新循环。</summary>
        protected override void OnDemoRouteEnter(string? fromPath)
        {
            _ = fromPath;
            if (_updateCancellation is not null)
            {
                return;
            }

            _updateCancellation = CancellationTokenSource.CreateLinkedTokenSource(LifetimeToken);
            _ = RunRingUpdatesAsync(_updateCancellation.Token);
        }

        /// <summary>页面离开时先停止呼吸环循环，再由基类清理图表资源。</summary>
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

        /// <summary>每 650ms 把下一帧环尺寸更新投递到 UI 线程。</summary>
        private async Task RunRingUpdatesAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(UpdateInterval);
            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    await UIDispatcher.InvokeAsync(() => ToggleRingSizes(cancellationToken));
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // 路由离开属于预期结束，不显示错误。
            }
            catch (Exception exception)
            {
                await UIDispatcher.InvokeAsync(() =>
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        UIMessage.Error(
                            $"涟漪散点动画已停止：{exception.Message}",
                            duration: 3f,
                            key: "effect-scatter-update-error");
                    }
                });
            }
        }

        /// <summary>交换两层空心环的 28/52 像素尺寸并提交增量配置。</summary>
        private void ToggleRingSizes(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            _ringsReversed = !_ringsReversed;
            Chart.SetOption(new ChartOption
            {
                Series =
                [
                    CreateRingSizePatch(FirstRingSeriesId, _ringsReversed ? 52f : 28f),
                    CreateRingSizePatch(SecondRingSeriesId, _ringsReversed ? 28f : 52f),
                ],
            }, new ChartSetOptionOptions
            {
                Silent = true,
            });
        }

        /// <summary>创建只修改指定呼吸环尺寸和更新动画的系列补丁。</summary>
        private static ChartScatterSeriesOption CreateRingSizePatch(
            string seriesId,
            float symbolSize)
        {
            return new ChartScatterSeriesOption
            {
                Id = seriesId,
                SymbolSize = symbolSize,
                AnimationDurationUpdate = 650,
                AnimationEasingUpdate = "cubicOut",
            };
        }

        /// <summary>创建坐标轴、Tooltip、普通样本、重点点和两层空心环的完整配置。</summary>
        private static ChartOption CreateOption()
        {
            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 500,
                    UpdateDuration = 650,
                    UpdateEasing = "cubicOut",
                },
                Tooltip = new ChartTooltipOption
                {
                    Id = "effect-scatter-tooltip",
                    Trigger = ChartTooltipTrigger.Item,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                    FormatterCallback = FormatTooltip,
                },
                Grid = new ChartGridOption
                {
                    Left = "28px",
                    Top = "24px",
                    Right = "28px",
                    Bottom = "30px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                    BackgroundColor = ChartBrush.Solid(SKColors.White),
                },
                XAxis = new ChartXAxisOption
                {
                    Id = HeightAxisId,
                    Name = "身高（cm）",
                    NameLocation = ChartAxisNameLocation.Middle,
                    NameGap = 36f,
                    Type = ChartAxisType.Value,
                    Scale = true,
                    AxisLabel = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(74, 85, 104)),
                    },
                    SplitLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(225, 230, 239)),
                        },
                    },
                },
                YAxis = new ChartYAxisOption
                {
                    Id = WeightAxisId,
                    Name = "体重（kg）",
                    NameLocation = ChartAxisNameLocation.Start,
                    NameGap = 12f,
                    Type = ChartAxisType.Value,
                    Scale = true,
                    AxisLabel = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(74, 85, 104)),
                    },
                    SplitLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(225, 230, 239)),
                        },
                    },
                },
                Series =
                [
                    CreateOrdinarySeries(),
                    CreateHighlightSeries(),
                    CreateRingSeries(FirstRingSeriesId, 28f, "first"),
                    CreateRingSeries(SecondRingSeriesId, 52f, "second"),
                ],
            };
        }

        /// <summary>创建保留全部 260 个样本的普通 Scatter 系列。</summary>
        private static ChartScatterSeriesOption CreateOrdinarySeries()
        {
            return new ChartScatterSeriesOption
            {
                Id = OrdinarySeriesId,
                Name = "普通样本",
                XAxisId = HeightAxisId,
                YAxisId = WeightAxisId,
                Z = 1,
                Symbol = "circle",
                SymbolSize = 8f,
                Clip = true,
                DataSource = CreateOrdinaryData(),
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(OrdinaryColor),
                    Opacity = 0.72f,
                },
                Emphasis = new()
                {
                    Focus = "series",
                    BlurScope = "coordinateSystem",
                },
            };
        }

        /// <summary>创建覆盖在普通样本上的两个重点点。</summary>
        private static ChartScatterSeriesOption CreateHighlightSeries()
        {
            return new ChartScatterSeriesOption
            {
                Id = HighlightSeriesId,
                Name = "重点样本",
                XAxisId = HeightAxisId,
                YAxisId = WeightAxisId,
                Z = 4,
                Symbol = "circle",
                SymbolSize = 20f,
                Clip = true,
                DataSource = CreateHighlightData("highlight"),
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(HighlightColor),
                    BorderColor = ChartBrush.Solid(SKColors.White),
                    BorderWidth = 2f,
                    ShadowColor = ChartBrush.Solid(HighlightColor.WithAlpha(100)),
                    ShadowBlur = 8f,
                },
                Emphasis = new()
                {
                    Focus = "series",
                    BlurScope = "coordinateSystem",
                },
            };
        }

        /// <summary>创建不参与命中和 Tooltip 的单层空心呼吸环。</summary>
        private static ChartScatterSeriesOption CreateRingSeries(
            string seriesId,
            float symbolSize,
            string keyPrefix)
        {
            return new ChartScatterSeriesOption
            {
                Id = seriesId,
                Name = "呼吸环",
                XAxisId = HeightAxisId,
                YAxisId = WeightAxisId,
                Z = 3,
                Silent = true,
                Symbol = "circle",
                SymbolSize = symbolSize,
                Clip = true,
                AnimationDuration = 0,
                AnimationDurationUpdate = 650,
                AnimationEasingUpdate = "cubicOut",
                DataSource = CreateHighlightData($"ring-{keyPrefix}"),
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(SKColors.Transparent),
                    BorderColor = ChartBrush.Solid(HighlightColor.WithAlpha(185)),
                    BorderWidth = 2f,
                    ShadowColor = ChartBrush.Solid(HighlightColor.WithAlpha(75)),
                    ShadowBlur = 6f,
                },
                Label = new()
                {
                    Show = false,
                },
            };
        }

        /// <summary>将普通点转换为带索引稳定身份的数据源。</summary>
        private static ChartScatterData CreateOrdinaryData()
        {
            var data = new ChartScatterData();
            for (var index = 0; index < OrdinaryPoints.Length; index++)
            {
                var point = OrdinaryPoints[index];
                var key = $"ordinary-{index:D3}";
                data.Add(new ChartScatterDataItem(point.Height, point.Weight)
                {
                    Id = key,
                    Key = key,
                    Name = $"样本 {index + 1}",
                });
            }

            return data;
        }

        /// <summary>创建两个重点点的数据源，并为每个叠加系列提供稳定逐项身份。</summary>
        private static ChartScatterData CreateHighlightData(string keyPrefix)
        {
            var data = new ChartScatterData();
            for (var index = 0; index < HighlightPoints.Length; index++)
            {
                var point = HighlightPoints[index];
                var key = $"{keyPrefix}-{index}";
                data.Add(new ChartScatterDataItem(point.Height, point.Weight)
                {
                    Id = key,
                    Key = key,
                    Name = $"重点样本 {index + 1}",
                });
            }

            return data;
        }

        /// <summary>按身高和体重格式化普通点及重点点 Tooltip。</summary>
        private static ChartTooltipContent? FormatTooltip(
            in ChartTooltipFormatterContext context)
        {
            if (context.Parameters.Count == 0)
            {
                return null;
            }

            var parameter = context.Parameters[0];
            var values = parameter.Value.Values;
            if (values.Count < 2
                || !values[0].TryGetDouble(out var height)
                || !values[1].TryGetDouble(out var weight))
            {
                return null;
            }

            var culture = context.Culture;
            return ChartTooltipContent.FromText(
                $"{parameter.SeriesName ?? "样本"}\r\n" +
                $"身高：{height.ToString("0.0", culture)} cm\r\n" +
                $"体重：{weight.ToString("0.0", culture)} kg");
        }
    }
}
