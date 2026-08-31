using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Gauge
{
    /// <summary>展示由八个主体层和三组双层 Gauge 泡泡组成的复合饼仪图。</summary>
    internal sealed class PieGaugeChartDemo : GaugeChartDemoPage
    {
        private static readonly SKColor ChartBackgroundColor = new(238, 241, 250);
        private static readonly SKColor BubbleTextColor = new(85, 85, 136);

        internal PieGaugeChartDemo()
            : base(
                "饼仪图",
                "使用多层 Pie 与 Gauge 组合出带刻度、中心总览和信息泡泡的复合仪表。",
                "八个主体系列负责数据环、背景、刻度和装饰圈；每个泡泡由 Z=10 的 Anchor 背景 Gauge 与 Z=11 的 Title/Detail 文字 Gauge 构成，不使用 Pie MarkPoint。",
                CreateOption())
        {
        }

        private static ChartOption CreateOption()
        {
            var option = new ChartOption
            {
                Background = ChartBrush.Solid(ChartBackgroundColor),
                Animation = new() { Enabled = true, Duration = 800, UpdateDuration = 500 },
                Series =
                [
                    CreateSolidPie("pie-gauge-backdrop", "白色底盘", 0f, 65f, 0,SKColors.White, new SKColor(231, 229, 255), 12f, 7f,ChartPieAnimationType.Scale),
                    CreateSolidPie("pie-gauge-mask", "中层遮罩", 0f, 47f, 1,ChartBackgroundColor),
                    CreateSolidPie("pie-gauge-outer-ring", "最外装饰环", 86f, 90f, 1,new SKColor(244, 244, 253)),
                    CreateSolidPie("pie-gauge-white-ring", "白色衬圈", 36f, 42f, 2,SKColors.White),
                    CreateOuterSeparatorGauge(),
                    CreateSegmentRing(),
                    CreateOverviewPie(),
                    CreateOuterTickGauge(),
                ],
            };

            AddBubble(option, "performance", "性能", 56d,offsetX: 135f, offsetY: -128f, size: 65f,new SKColor(107, 3, 240), 12f, 7f);
            AddBubble(option, "time", "时间", 20d, offsetX: 165f, offsetY: -52f, size: 58f, new SKColor(244, 125, 7));
            AddBubble(option, "speed", "速度", 24d,offsetX: 233f, offsetY: -112f, size: 53f,new SKColor(85, 85, 136));
            return option;
        }

        private static ChartPieSeriesOption CreateSegmentRing()
        {
            var data = new ChartPieData();
            AddSegment(data, "performance", "性能", 395d, new SKColor(135, 118, 255));
            AddSegment(data, "time", "时间", 160d, new SKColor(251, 165, 68));
            AddSegment(data, "speed", "速度", 141d, new SKColor(243, 109, 142));
            return new ChartPieSeriesOption
            {
                Id = "pie-gauge-segments",
                Name = "饼图",
                Z = 4,
                Center = ChartPieCenter.Percent(50f, 50f),
                Radius = PieRadius(37f, 42f),
                StartAngle = 0f,
                EndAngle = 360f,
                ItemStyle = new()
                {
                    BorderRadius = 10f,
                    BorderWidth = 2f,
                    BorderColor = ChartBrush.Solid(SKColors.White),
                },
                Label = new() { Show = false },
                LabelLine = new() { Show = false },
                Emphasis = new() { Scale = false },
                DataSource = data,
            };
        }

        private static void AddSegment(
            ChartPieData data,
            string key,
            string name,
            double value,
            SKColor color)
        {
            data.Add(new ChartPieDataItem(value, name)
            {
                Key = $"pie-gauge-segment-{key}",
                ItemStyle = new() { Color = ChartBrush.Solid(color) },
            });
        }

        private static ChartPieSeriesOption CreateOverviewPie()
        {
            var data = new ChartPieData();
            data.Add(new ChartPieDataItem(75d, "总览")
            {
                Key = "pie-gauge-overview-value",
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(new SKColor(112, 109, 252)),
                    ShadowColor = ChartBrush.Solid(new SKColor(204, 204, 204)),
                    ShadowBlur = 12f,
                    ShadowOffsetY = 7f,
                },
                Label = new()
                {
                    Show = true,
                    Position = ChartPieLabelPosition.Center,
                    Formatter = "{title|总   览}\n{value|75}{unit|%}",
                    Rich = new()
                    {
                        ["title"] = WhiteText(13f, 600, 23f),
                        ["value"] = WhiteText(42f, 600),
                        ["unit"] = WhiteText(15f),
                    },
                },
                LabelLine = new() { Show = false },
            });
            return new ChartPieSeriesOption
            {
                Id = "pie-gauge-overview",
                Name = "总览",
                Z = 3,
                Silent = true,
                Center = ChartPieCenter.Percent(50f, 50f),
                Radius = PieRadius(0f, 30f),
                Label = new() { Show = false },
                LabelLine = new() { Show = false },
                Emphasis = new() { Scale = false },
                AnimationType = ChartPieAnimationType.Scale,
                DataSource = data,
            };
        }

        private static ChartTextStyle WhiteText(
            float fontSize,
            int? fontWeight = null,
            float? lineHeight = null)
        {
            var style = new ChartTextStyle
            {
                Color = ChartBrush.Solid(SKColors.White),
                FontSize = fontSize,
                Align = ChartTextAlign.Center,
            };
            if (fontWeight is { } weight)
            {
                style.FontWeight = weight;
            }

            if (lineHeight is { } height)
            {
                style.LineHeight = height;
            }

            return style;
        }

        private static ChartPieSeriesOption CreateSolidPie(
            string id,
            string name,
            float innerRadius,
            float outerRadius,
            int z,
            SKColor color,
            SKColor? shadowColor = null,
            float? shadowBlur = null,
            float? shadowOffsetY = null,
            ChartPieAnimationType? animationType = null)
        {
            var style = new ChartItemStyleOption
            {
                Color = ChartBrush.Solid(color),
                ShadowBlur = shadowBlur,
                ShadowOffsetY = shadowOffsetY,
            };
            if (shadowColor is { } shadow)
            {
                style.ShadowColor = ChartBrush.Solid(shadow);
            }

            var data = new ChartPieData();
            data.Add(new ChartPieDataItem(1d, name) { Key = $"{id}-value" });
            return new ChartPieSeriesOption
            {
                Id = id,
                Name = name,
                Z = z,
                Silent = true,
                Center = ChartPieCenter.Percent(50f, 50f),
                Radius = PieRadius(innerRadius, outerRadius),
                AvoidLabelOverlap = false,
                LegendHoverLink = false,
                ItemStyle = style,
                Label = new() { Show = false },
                LabelLine = new() { Show = false },
                Emphasis = new() { Scale = false },
                AnimationType = animationType,
                DataSource = data,
            };
        }

        private static ChartGaugeSeriesOption CreateOuterTickGauge()
        {
            var tickColor = ChartBrush.Solid(new SKColor(226, 222, 253));
            return new ChartGaugeSeriesOption
            {
                Id = "pie-gauge-outer-ticks",
                Name = "外部刻度",
                Z = 5,
                Silent = true,
                Center = ChartGaugeCenter.Percent(50f, 50f),
                Radius = ChartLength.Percent(65f),
                Minimum = 0d,
                Maximum = 200d,
                SplitNumber = 30,
                StartAngle = 0f,
                EndAngle = 360f,
                AxisLine = new() { Show = false },
                AxisLabel = new() { Show = false },
                AxisTick = new()
                {
                    Show = true,
                    SplitNumber = 6,
                    Length = ChartLength.Pixels(20f),
                    LineStyle = new() { Color = tickColor, Width = 1f },
                },
                SplitLine = new()
                {
                    Show = true,
                    Length = ChartLength.Pixels(20f),
                    LineStyle = new() { Color = tickColor, Width = 1f },
                },
                Progress = new() { Show = false },
                Pointer = new() { Show = false },
                Anchor = new() { Show = false },
                Title = new() { Show = false },
                Detail = new() { Show = false },
                AnimationDuration = 0,
                AnimationDurationUpdate = 0,
            };
        }

        private static ChartGaugeSeriesOption CreateOuterSeparatorGauge()
        {
            var background = ChartBrush.Solid(ChartBackgroundColor);
            return new ChartGaugeSeriesOption
            {
                Id = "pie-gauge-outer-separator",
                Name = "最外分割线",
                Z = 2,
                Silent = true,
                Center = ChartGaugeCenter.Percent(50f, 50f),
                Radius = ChartLength.Percent(80f),
                StartAngle = 360f,
                EndAngle = 0f,
                Minimum = 0d,
                Maximum = 100d,
                SplitNumber = 4,
                AxisLine = new()
                {
                    Show = true,
                    LineStyle = new()
                    {
                        Width = 1f,
                        ColorStops =
                        {
                            new ChartGaugeColorStop(0.5d,
                                ChartBrush.Solid(new SKColor(210, 209, 245))),
                            new ChartGaugeColorStop(1d, background),
                        },
                    },
                },
                Progress = new() { Show = false },
                AxisTick = new() { Show = false },
                AxisLabel = new() { Show = false },
                SplitLine = new()
                {
                    Show = true,
                    Length = ChartLength.Pixels(23f),
                    Distance = -11f,
                    LineStyle = new() { Width = 57f, Color = background },
                },
                Pointer = new() { Show = false },
                Anchor = new() { Show = false },
                Title = new() { Show = false },
                Detail = new() { Show = false },
                AnimationDuration = 0,
                AnimationDurationUpdate = 0,
            };
        }

        private static void AddBubble(
            ChartOption option,
            string id,
            string name,
            double value,
            float offsetX,
            float offsetY,
            float size,
            SKColor borderColor,
            float? shadowBlur = null,
            float? shadowOffsetY = null)
        {
            option.Series.Add(CreateBubbleBackground(id, name, value, offsetX, offsetY, size, borderColor, shadowBlur, shadowOffsetY));
            option.Series.Add(CreateBubbleText(id, name, value, offsetX, offsetY));
        }

        private static ChartGaugeSeriesOption CreateBubbleBackground(
            string id,
            string name,
            double value,
            float offsetX,
            float offsetY,
            float size,
            SKColor borderColor,
            float? shadowBlur,
            float? shadowOffsetY)
        {
            var style = new ChartItemStyleOption
            {
                Color = ChartBrush.Solid(SKColors.White),
                BorderColor = ChartBrush.Solid(borderColor),
                BorderWidth = 4f,
                ShadowBlur = shadowBlur,
                ShadowOffsetY = shadowOffsetY,
            };
            if (shadowBlur is not null)
            {
                style.ShadowColor = ChartBrush.Solid(new SKColor(204, 204, 204));
            }

            return CreateBubbleGauge(
                $"pie-gauge-bubble-{id}-background", name, value, 10,
                new ChartGaugeAnchorOption
                {
                    Show = true,
                    ShowAbove = true,
                    Size = size,
                    Icon = "circle",
                    OffsetCenter = ChartGaugeCenter.Pixels(offsetX, offsetY),
                    ItemStyle = style,
                },
                new ChartGaugeTitleOption { Show = false },
                new ChartGaugeDetailOption { Show = false });
        }

        private static ChartGaugeSeriesOption CreateBubbleText(
            string id,
            string name,
            double value,
            float offsetX,
            float offsetY)
        {
            return CreateBubbleGauge(
                $"pie-gauge-bubble-{id}-text", name, value, 11,
                new ChartGaugeAnchorOption { Show = false },
                new ChartGaugeTitleOption
                {
                    Show = true,
                    OffsetCenter = ChartGaugeCenter.Pixels(offsetX, offsetY - 6f),
                    Color = ChartBrush.Solid(BubbleTextColor),
                    FontSize = 13f,
                    FontWeight = 500,
                    LineHeight = 11f,
                    Align = ChartTextAlign.Center,
                },
                new ChartGaugeDetailOption
                {
                    Show = true,
                    OffsetCenter = ChartGaugeCenter.Pixels(offsetX, offsetY + 18f),
                    FormatterCallback = static current => $"{{value|{current:F0}}}{{unit|%}}",
                    Color = ChartBrush.Solid(BubbleTextColor),
                    FontSize = 12f,
                    FontWeight = 500,
                    Align = ChartTextAlign.Center,
                    Rich = new()
                    {
                        ["value"] = new ChartTextStyle
                        {
                            Color = ChartBrush.Solid(BubbleTextColor),
                            FontSize = 12f,
                            FontWeight = 500,
                        },
                        ["unit"] = new ChartTextStyle
                        {
                            Color = ChartBrush.Solid(BubbleTextColor),
                            Padding = new ChartInsets(left: 2f, top: 5f, right: 0f, bottom: 0f),
                            FontSize = 7f,
                            FontWeight = 700,
                        },
                    },
                });
        }

        private static ChartGaugeSeriesOption CreateBubbleGauge(
            string id,
            string name,
            double value,
            int z,
            ChartGaugeAnchorOption anchor,
            ChartGaugeTitleOption title,
            ChartGaugeDetailOption detail)
        {
            var dataKey = $"{id}-value";
            return new ChartGaugeSeriesOption
            {
                Id = id,
                Name = name,
                Z = z,
                Silent = true,
                Center = ChartGaugeCenter.Middle,
                Radius = ChartLength.Percent(1f),
                Minimum = 0d,
                Maximum = 100d,
                AxisLine = new() { Show = false },
                Progress = new() { Show = false },
                AxisTick = new() { Show = false },
                SplitLine = new() { Show = false },
                AxisLabel = new() { Show = false },
                Pointer = new() { Show = false },
                Anchor = anchor,
                Title = title,
                Detail = detail,
                AnimationDuration = 0,
                AnimationDurationUpdate = 0,
                DataSource = new ChartGaugeData().Add(
                    new ChartGaugeDataItem(value, name)
                    {
                        Id = dataKey,
                        Key = dataKey,
                    }),
            };
        }

        private static ChartPieRadius PieRadius(float inner, float outer) => new(
            ChartLength.Percent(inner),
            ChartLength.Percent(outer));
    }
}
