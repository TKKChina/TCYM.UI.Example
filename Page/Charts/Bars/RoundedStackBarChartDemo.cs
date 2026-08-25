using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Chart.Series.Stack;

namespace Page.Charts.Bars
{
    /// <summary>展示两个独立 stack 中仅最上方可见柱段具有圆角的柱状图。</summary>
    internal sealed class RoundedStackBarChartDemo : BarChartDemoPage
    {
        /// <summary>创建带圆角的堆积柱状图页面。</summary>
        internal RoundedStackBarChartDemo()
            : base(
                "带圆角的堆积柱状图",
                "逐类目判断 stack 顶部系列，只给最上方的可见柱段设置圆角。",
                "示例保留空值与零值的区别：空值不会生成柱段，零值保留数据槽但不参与顶部判断。每个数据项都可通过 ItemStyle 单独设置 BorderRadius。",
                CreateOption())
        {
        }

        /// <summary>创建两个 stack、五个系列和逐项顶部圆角的完整配置。</summary>
        /// <returns>包含逐项圆角样式的完整柱状图配置。</returns>
        private static ChartOption CreateOption()
        {
            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 650,
                    UpdateDuration = 450,
                },
                Tooltip = new ChartTooltipOption
                {
                    Id = "rounded-stack-tooltip",
                    Trigger = ChartTooltipTrigger.Axis,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                    AxisPointer = new()
                    {
                        Type = ChartTooltipAxisPointerType.Shadow,
                        Axis = ChartTooltipAxisPointerAxis.X,
                    },
                },
                Legend = new ChartLegendOption
                {
                    Id = "rounded-stack-legend",
                    Top = "8px",
                    Left = ChartLength.FromKeyword("center"),
                    ItemGap = 22f,
                    TextStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(70, 78, 92)),
                        FontSize = 14f,
                    },
                },
                Grid = new ChartGridOption
                {
                    Left = "28px",
                    Top = "58px",
                    Right = "24px",
                    Bottom = "34px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                },
                XAxis = new ChartXAxisOption
                {
                    Id = "rounded-stack-x",
                    Type = ChartAxisType.Category,
                    BoundaryGap = true,
                    Data = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
                    AxisTick = new() { Show = false },
                    AxisLine = new()
                    {
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(145, 153, 166)),
                        },
                    },
                    AxisLabel = new()
                    {
                        Interval = 0,
                        Color = ChartBrush.Solid(new SKColor(55, 63, 78)),
                    },
                    SplitLine = new() { Show = false },
                },
                YAxis = new ChartYAxisOption
                {
                    Id = "rounded-stack-y",
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(0d),
                    Scale = true,
                    AxisLine = new() { Show = false },
                    AxisTick = new() { Show = false },
                    AxisLabel = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(55, 63, 78)),
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
                Series =
                [
                    CreateSeries(
                        "rounded-a", "a", "stack-a",
                        [120d, 200d, 150d, 80d, 70d, 110d, 130d],
                        new SKColor(80, 112, 221),
                        new HashSet<int> { 5, 6 }),
                    CreateSeries(
                        "rounded-b", "b", "stack-a",
                        [10d, 46d, 64d, null, 0d, null, 0d],
                        new SKColor(145, 204, 117),
                        new HashSet<int> { 1, 2 }),
                    CreateSeries(
                        "rounded-c", "c", "stack-a",
                        [30d, null, 0d, 20d, 10d, null, 0d],
                        new SKColor(250, 200, 88),
                        new HashSet<int> { 0, 3, 4 }),
                    CreateSeries(
                        "rounded-d", "d", "stack-b",
                        [30d, null, 0d, 20d, 10d, null, 0d],
                        new SKColor(238, 102, 102),
                        new HashSet<int> { 3, 4 }),
                    CreateSeries(
                        "rounded-e", "e", "stack-b",
                        [10d, 20d, 150d, 0d, null, 50d, 10d],
                        new SKColor(115, 192, 222),
                        new HashSet<int> { 0, 1, 2, 5, 6 }),
                ],
            };
        }

        /// <summary>创建一个 stack 系列，并只给指定类目的正数柱段添加顶部圆角。</summary>
        /// <param name="id">系列稳定 ID。</param>
        /// <param name="name">Legend 与 Tooltip 显示名称。</param>
        /// <param name="stack">堆积分组键。</param>
        /// <param name="values">按星期排列的数值；空值表示缺失柱段。</param>
        /// <param name="color">柱段颜色。</param>
        /// <param name="roundedIndices">当前系列位于 stack 顶部的类目索引。</param>
        /// <returns>带逐项 BorderRadius 的 Bar 系列。</returns>
        private static ChartBarSeriesOption CreateSeries(
            string id,
            string name,
            string stack,
            IReadOnlyList<double?> values,
            SKColor color,
            IReadOnlySet<int> roundedIndices)
        {
            var categories = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            var data = new ChartBarData();
            for (var index = 0; index < values.Count; index++)
            {
                var value = values[index];
                var item = new ChartBarDataItem(
                    value is { } number ? ChartValue.From(number) : ChartValue.Null)
                {
                    Key = $"{id}:{categories[index]}",
                    Name = categories[index],
                };

                item.ItemStyle.BorderRadius = roundedIndices.Contains(index) && value is > 0d
                    ? ChartBorderRadius.FromValues(20, 20, 0, 0)
                    : 0;
                data.Add(item);
            }

            return new ChartBarSeriesOption
            {
                Id = id,
                Name = name,
                XAxisId = "rounded-stack-x",
                YAxisId = "rounded-stack-y",
                Stack = stack,
                StackStrategy = ChartStackStrategy.SameSign,
                StackOrder = ChartStackOrder.SeriesAscending,
                BarMaxWidth = ChartLength.Pixels(34f),
                BarGap = ChartBarGap.Percent(22f),
                BarCategoryGap = ChartLength.Percent(28f),
                DataSource = data,
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(color),
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
