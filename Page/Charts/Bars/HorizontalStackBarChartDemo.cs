using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Chart.Series.Stack;

namespace Page.Charts.Bars
{
    /// <summary>展示五个来源系列按星期横向累计的堆叠条形图。</summary>
    internal sealed class HorizontalStackBarChartDemo : BarChartDemoPage
    {
        /// <summary>创建横向堆叠条形图页面。</summary>
        internal HorizontalStackBarChartDemo()
            : base(
                "横向堆叠条形图",
                "五个来源系列按星期横向堆叠，展示每日总量及其组成。",
                "Value XAxis 与 Category YAxis 决定横向布局；五个 Bar 共用 total stack，标签显示各段数值，shadow 辅助带可一次查看当前星期的全部来源。",
                CreateOption())
        {
        }

        /// <summary>创建横向堆叠条形图的完整配置。</summary>
        /// <returns>包含 Legend、axis Tooltip 和五个 stack 系列的配置。</returns>
        private static ChartOption CreateOption()
        {
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
                    Id = "horizontal-stack-tooltip",
                    Trigger = ChartTooltipTrigger.Axis,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                    AxisPointer = new()
                    {
                        Type = ChartTooltipAxisPointerType.Shadow,
                        Axis = ChartTooltipAxisPointerAxis.Y,
                    },
                },
                Legend = new ChartLegendOption
                {
                    Id = "horizontal-stack-legend",
                    Top = "8px",
                    Left = ChartLength.FromKeyword("center"),
                    ItemGap = 20f,
                    TextStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(55, 63, 78)),
                        FontSize = 13f,
                    },
                },
                Grid = new ChartGridOption
                {
                    Left = "20px",
                    Top = "58px",
                    Right = "24px",
                    Bottom = "28px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                },
                XAxis = new ChartXAxisOption
                {
                    Id = "horizontal-stack-x",
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(0d),
                    Maximum = ChartAxisBound.Fixed(3000d),
                    Interval = 500d,
                    Scale = true,
                    AxisLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(145, 153, 166)),
                        },
                    },
                    AxisTick = new() { Show = false },
                    AxisLabel = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(70, 78, 92)),
                    },
                    SplitLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(228, 233, 241)),
                            Type = ChartAxisLineType.Dashed,
                        },
                    },
                },
                YAxis = new ChartYAxisOption
                {
                    Id = "horizontal-stack-y",
                    Type = ChartAxisType.Category,
                    BoundaryGap = true,
                    Data = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
                    AxisLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(145, 153, 166)),
                        },
                    },
                    AxisTick = new() { Show = false },
                    AxisLabel = new()
                    {
                        Interval = 0,
                        Color = ChartBrush.Solid(new SKColor(55, 63, 78)),
                    },
                    SplitLine = new() { Show = false },
                },
                Series =
                [
                    CreateSeries(
                        "horizontal-stack-direct",
                        "Direct",
                        new SKColor(80, 112, 221),
                        [320d, 302d, 301d, 334d, 390d, 330d, 320d]),
                    CreateSeries(
                        "horizontal-stack-mail",
                        "Mail Ad",
                        new SKColor(115, 192, 222),
                        [120d, 132d, 101d, 134d, 90d, 230d, 210d]),
                    CreateSeries(
                        "horizontal-stack-affiliate",
                        "Affiliate Ad",
                        new SKColor(145, 204, 117),
                        [220d, 182d, 191d, 234d, 290d, 330d, 310d]),
                    CreateSeries(
                        "horizontal-stack-video",
                        "Video Ad",
                        new SKColor(250, 200, 88),
                        [150d, 212d, 201d, 154d, 190d, 330d, 410d]),
                    CreateSeries(
                        "horizontal-stack-search",
                        "Search Engine",
                        new SKColor(238, 102, 102),
                        [820d, 832d, 901d, 934d, 1290d, 1330d, 1320d]),
                ],
            };
        }

        /// <summary>创建横向 total stack 中的单个来源系列。</summary>
        /// <param name="id">系列稳定 ID。</param>
        /// <param name="name">Legend 与 Tooltip 显示名称。</param>
        /// <param name="color">当前堆叠段的填充色。</param>
        /// <param name="values">Mon 到 Sun 的来源数值。</param>
        /// <returns>绑定横向坐标轴和 total stack 的 Bar 系列。</returns>
        private static ChartBarSeriesOption CreateSeries(
            string id,
            string name,
            SKColor color,
            IReadOnlyList<double> values)
        {
            var categories = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            var data = new ChartBarData();
            for (var index = 0; index < values.Count; index++)
            {
                data.Add(new ChartBarDataItem(
                    values[index],
                    categories[index])
                {
                    Key = $"{id}:{categories[index]}",
                    Name = categories[index],
                });
            }

            return new ChartBarSeriesOption
            {
                Id = id,
                Name = name,
                XAxisId = "horizontal-stack-x",
                YAxisId = "horizontal-stack-y",
                Stack = "total",
                StackStrategy = ChartStackStrategy.SameSign,
                StackOrder = ChartStackOrder.SeriesAscending,
                BarWidth = ChartLength.Pixels(26f),
                DataSource = data,
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(color),
                },
                Label = new()
                {
                    Show = true,
                    Position = "inside",
                    Color = ChartBrush.Solid(SKColors.White),
                    FontSize = 12f,
                    FormatterCallback = static parameters =>
                        parameters.PrimaryValue.AsString(parameters.Culture) ?? "-",
                },
                Emphasis = new()
                {
                    Focus = "series",
                    BlurScope = "coordinateSystem",
                },
            };
        }
    }
}
