using SkiaSharp;
using System.Globalization;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Pie
{
    /// <summary>展示带间隔扇区、双层标签和中心治理率文字的增强环形图。</summary>
    internal sealed class EnhancedHalfDonutPieChartDemo : PieChartDemoPage
    {
        private static readonly string[] MetricNames =
        [
            "总户数",
            "总人口数",
            "已覆盖户数",
            "已覆盖人口数",
        ];

        private static readonly double[] MetricValues = [1234d, 4700d, 1200d, 3687d];

        /// <summary>创建半圆增强展示页面。</summary>
        internal EnhancedHalfDonutPieChartDemo()
            : base(
                "半圆增强展示",
                "四个业务指标使用彩色扇区和透明间隔组成开放式环形布局。",
                "同一份扇区权重分别交给内侧名称层和外侧数值层；透明占位项留出间隔与底部开口，Title 组件在圆心显示治理率。FormatterCallback 从稳定 DataIndex 读取业务名称和真实数值。",
                CreateOption())
        {
        }

        /// <summary>创建增强环形图的完整配置。</summary>
        /// <returns>包含双层 Pie、隐藏 Tooltip/Legend 和中心标题的配置。</returns>
        private static ChartOption CreateOption()
        {
            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 800,
                    UpdateDuration = 500,
                },
                Tooltip = new ChartTooltipOption
                {
                    Id = "enhanced-half-tooltip",
                    Show = false,
                },
                Legend = new ChartLegendOption
                {
                    Id = "enhanced-half-legend",
                    Show = false,
                },
                Components =
                {
                    new ChartTitleOption
                    {
                        Id = "enhanced-half-center-title",
                        Text = "治理率(以户数计算)",
                        Subtext = "80%",
                        Left = ChartLength.Percent(50f),
                        Top = "46%",
                        TextAlign = ChartTextAlign.Center,
                        ItemGap = 8f,
                        Padding = new ChartInsets(0f),
                        Silent = true,
                        Z = 20,
                        TextStyle = new ChartTextStyle
                        {
                            Color = ChartBrush.Solid(new SKColor(51, 51, 51)),
                            FontSize = 16f,
                            Align = ChartTextAlign.Center,
                        },
                        SubtextStyle = new ChartTextStyle
                        {
                            Color = ChartBrush.Solid(SKColors.Red),
                            FontSize = 26f,
                            Align = ChartTextAlign.Center,
                        },
                    },
                },
                Series =
                [
                    CreateNameSeries(),
                    CreateValueSeries(),
                ],
            };
        }

        /// <summary>创建内侧显示业务名称的 Pie 系列。</summary>
        private static ChartPieSeriesOption CreateNameSeries()
        {
            return CreateSeries(
                "enhanced-half-names",
                z: 2,
                new ChartPieLabelOption
                {
                    Show = true,
                    Position = ChartPieLabelPosition.Inside,
                    FontSize = 14f,
                    Color = ChartBrush.Solid(SKColors.White),
                    FormatterCallback = static context => FormatMetricName(context),
                });
        }

        /// <summary>创建外侧显示原始业务值的 Pie 系列。</summary>
        private static ChartPieSeriesOption CreateValueSeries()
        {
            return CreateSeries(
                "enhanced-half-values",
                z: 2,
                new ChartPieLabelOption
                {
                    Show = true,
                    Position = ChartPieLabelPosition.Outer,
                    FontSize = 18f,
                    FormatterCallback = static context => FormatMetricValue(context),
                });
        }

        /// <summary>创建两个标签层共用的 Pie 几何和样式。</summary>
        private static ChartPieSeriesOption CreateSeries(
            string id,
            int z,
            ChartPieLabelOption label)
        {
            return new ChartPieSeriesOption
            {
                Id = id,
                Name = "治理指标",
                Z = z,
                Clockwise = false,
                StartAngle = -30f,
                Center = ChartPieCenter.Percent(50f, 52f),
                Radius = new ChartPieRadius(
                    ChartLength.Pixels(100f),
                    ChartLength.Pixels(200f)),
                AvoidLabelOverlap = false,
                Label = label,
                LabelLine = new ChartPieLabelLineOption
                {
                    Show = true,
                    Length = ChartLength.Pixels(30f),
                    Length2 = ChartLength.Pixels(60f),
                },
                Emphasis = new()
                {
                    Scale = false,
                },
                DataSource = CreateData(id),
            };
        }

        /// <summary>按业务项生成彩色扇区、透明小间隔和末尾开放区。</summary>
        private static ChartPieData CreateData(string seriesId)
        {
            var colors = new[]
            {
                new SKColor(255, 153, 153),
                new SKColor(255, 176, 63),
                new SKColor(61, 186, 45),
                new SKColor(43, 166, 254),
            };
            var data = new ChartPieData();
            for (var index = 0; index < MetricNames.Length; index++)
            {
                var color = colors[index];
                data.Add(new ChartPieDataItem(20d, MetricNames[index])
                {
                    Key = $"{seriesId}:metric:{index}",
                    ItemStyle = new()
                    {
                        Color = ChartBrush.Solid(color),
                        BorderWidth = 20f,
                        BorderColor = ChartBrush.Solid(color.WithAlpha(102)),
                    },
                });
                data.Add(CreatePlaceholder($"{seriesId}:gap:{index}", 8d));
            }

            data.Add(CreatePlaceholder($"{seriesId}:opening", 40d));
            return data;
        }

        /// <summary>创建不会显示、命中或产生标签的透明占位扇区。</summary>
        private static ChartPieDataItem CreatePlaceholder(string key, double value)
        {
            return new ChartPieDataItem(value, string.Empty)
            {
                Key = key,
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(SKColors.Transparent),
                    BorderColor = ChartBrush.Solid(SKColors.Transparent),
                    BorderWidth = 0f,
                },
                Label = new()
                {
                    Show = false,
                },
                LabelLine = new()
                {
                    Show = false,
                },
                Emphasis = new()
                {
                    Disabled = true,
                },
            };
        }

        /// <summary>把四字以上名称拆成两行，透明占位项返回空文本。</summary>
        private static string FormatMetricName(ChartFormatterContext context)
        {
            var name = context.Name;
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            return name.Length > 4
                ? $"{name[..3]}\n{name[3..]}"
                : name;
        }

        /// <summary>根据偶数数据下标返回真实业务值，透明占位项返回空文本。</summary>
        private static string FormatMetricValue(ChartFormatterContext context)
        {
            if (string.IsNullOrEmpty(context.Name) || (context.DataIndex & 1) != 0)
            {
                return string.Empty;
            }

            var metricIndex = context.DataIndex / 2;
            return metricIndex >= 0 && metricIndex < MetricValues.Length
                ? MetricValues[metricIndex].ToString("0", CultureInfo.InvariantCulture)
                : string.Empty;
        }
    }
}
