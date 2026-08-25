using SkiaSharp;
using System.Drawing;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Bars
{
    /// <summary>展示纵向渐变、顶部圆角、阴影与 rich 百分比标签的柱状图。</summary>
    internal sealed class GradientBarChartDemo : BarChartDemoPage
    {
        /// <summary>创建柱状渐变图页面。</summary>
        internal GradientBarChartDemo()
            : base(
                "柱状渐变图",
                "使用相对柱体坐标的线性渐变，并在柱顶显示圆角百分比标签。",
                "渐变从青绿色过渡到蓝色，柱体顶部使用 30px 圆角并附加发光阴影；标签通过 rich 文本样式绘制为深色圆角胶囊。",
                CreateOption())
        {
        }

        /// <summary>创建渐变柱状图的完整配置。</summary>
        /// <returns>包含纵向渐变、阴影与 rich 标签的完整配置。</returns>
        private static ChartOption CreateOption()
        {
            var series = new ChartBarSeriesOption
            {
                Id = "gradient-rate",
                Name = "完成率",
                XAxisId = "gradient-x",
                YAxisId = "gradient-y",
                BarWidth = ChartLength.Pixels(24f),
                DataSource = new ChartBarData(100d, 100d, 90d, 10d, 90d, 90d, 20d, 56d, 89d),
                ItemStyle = new()
                {
                    Color = new ChartLinearGradientBrush(
                        new SKPoint(0f, 0f),
                        new SKPoint(0f, 1f),
                        [new SKColor(65, 225, 212), new SKColor(16, 167, 219)],
                        [0f, 1f]),
                    BorderRadius = ChartBorderRadius.FromValues(30, 30, 0, 0),
                    ShadowColor = ChartBrush.Solid(new SKColor(0, 255, 225, 210)),
                    ShadowBlur = 4f,
                },
                Label = new()
                {
                    Show = true,
                    Position = "top",
                    Distance = 12f,
                    Formatter = "{value|{c}%}",
                    Rich = new()
                    {
                        ["value"] = new ChartTextStyle
                        {
                            Color = ChartBrush.Solid(SKColors.White),
                            FontSize = 13f,
                            FontWeight = 600,
                            Align = ChartTextAlign.Center,
                            VerticalAlign = ChartTextVerticalAlign.Middle,
                            Padding = new ChartInsets(horizontal: 12f, vertical: 7f),
                            BackgroundColor = ChartBrush.Solid(new SKColor(37, 36, 83)),
                            BorderRadius = 16f,
                        }
                    }
                },
                Emphasis = new()
                {
                    Focus = "series",
                    BlurScope = "coordinateSystem",
                },
            };

            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 700,
                    UpdateDuration = 450,
                },
                Tooltip = new ChartTooltipOption
                {
                    Id = "gradient-tooltip",
                    Trigger = ChartTooltipTrigger.Axis,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                    AxisPointer = new()
                    {
                        Type = ChartTooltipAxisPointerType.Shadow,
                        Axis = ChartTooltipAxisPointerAxis.X,
                    },
                },
                Grid = new ChartGridOption
                {
                    Top = "72px",
                    Right = "3%",
                    Left = "5%",
                    Bottom = "52px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                },
                XAxis = new ChartXAxisOption
                {
                    Id = "gradient-x",
                    Type = ChartAxisType.Category,
                    BoundaryGap = true,
                    Data =
                    [
                        "学员续费率", "试听课转换率", "课程消费率", "课后评分率",
                        "作业完成率", "班级满班率", "排课上课率", "体验课转化率",
                    ],
                    AxisLine = new()
                    {
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(39, 36, 86)),
                        },
                    },
                    AxisTick = new() { Show = false },
                    AxisLabel = new()
                    {
                        Interval = 0,
                        Margin = 18f,
                        Color = ChartBrush.Solid(new SKColor(89, 88, 141)),
                        FontSize = 12f,
                    },
                    SplitLine = new() { Show = false },
                },
                YAxis = new ChartYAxisOption
                {
                    Id = "gradient-y",
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(0),
                    Maximum = ChartAxisBound.Fixed(120),
                    Interval = 20d,
                    Scale = true,
                    AxisLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(144, 151, 156)),
                        },
                    },
                    AxisTick = new() { Show = false },
                    AxisLabel = new()
                    {
                        Formatter = "{value}%",
                        Color = ChartBrush.Solid(new SKColor(89, 88, 141)),
                    },
                    SplitLine = new() { Show = false },
                },
                Series = series,
            };
        }
    }
}
