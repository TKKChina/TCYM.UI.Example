using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Chart.Text;

namespace Page.Charts.Bars
{
    /// <summary>展示排名 rich 轴标签、前景值与 100% 背景轨道重叠的横向柱状图。</summary>
    internal sealed class HorizontalBarChartDemo : BarChartDemoPage
    {
        /// <summary>创建横向柱状图页面。</summary>
        internal HorizontalBarChartDemo()
            : base(
                "横向柱状图",
                "左侧显示带前三名颜色的业务名称，右侧显示百分比值。",
                "X 轴作为数值轴并隐藏；两个 Y 类目轴共享相同槽位。蓝色前景柱通过 barGap=-100% 覆盖灰色 100% 背景轨道，标签 TextFormatter 生成排名 rich 文本。",
                CreateOption())
        {
        }

        /// <summary>创建横向柱状图的完整配置。</summary>
        /// <returns>包含双 Y 类目轴、前景值和背景轨道的配置。</returns>
        private static ChartOption CreateOption()
        {
            var names = new[]
            {
                "申请立案",
                "无法核查",
                "已进入法律程库",
                "重复舆情",
                "咨询类",
                "快速调处",
                "不属监察管辖",
                "未发现违法行为",
            };
            var values = new[] { 36d, 54d, 29d, 25d, 45d, 29d, 25d, 45d };
            var categoryAxis = CreateCategoryAxis(names);

            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 650,
                    UpdateDuration = 400,
                },
                Tooltip = new ChartTooltipOption
                {
                    Id = "horizontal-tooltip",
                    Trigger = ChartTooltipTrigger.Item,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                    AxisPointer = new()
                    {
                        Type = ChartTooltipAxisPointerType.None,
                    },
                    FormatterCallback = (in ChartTooltipFormatterContext context) =>
                    {
                        if (context.Parameters.Count == 0)
                        {
                            return null;
                        }

                        var parameter = context.Parameters[0];
                        var value = parameter.Value.Primary.AsString(context.Culture) ?? "-";
                        return ChartTooltipContent.FromText(
                            $"{parameter.Name ?? "-"} : {value}%");
                    },
                },
                Grid = new ChartGridOption
                {
                    Left = "12px",
                    Top = "14px",
                    Right = "12px",
                    Bottom = "14px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                },
                XAxis = new ChartXAxisOption
                {
                    Id = "horizontal-x",
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(0d),
                    Maximum = ChartAxisBound.Fixed(100d),
                    Scale = true,
                    Show = false,
                },
                YAxis =
                [
                    categoryAxis,
                    new ChartYAxisOption
                    {
                        Id = "horizontal-values-y",
                        Type = ChartAxisType.Category,
                        Position = ChartYAxisPosition.Right,
                        Inverse = true,
                        BoundaryGap = true,
                        Data = values
                            .Select(static value => ChartValue.From(value))
                            .ToArray(),
                        AxisLine = new() { Show = false },
                        AxisTick = new() { Show = false },
                        SplitLine = new() { Show = false },
                        AxisLabel = new()
                        {
                            Show = true,
                            Formatter = "{value}%",
                            Color = ChartBrush.Solid(new SKColor(49, 150, 250)),
                            FontSize = 12f,
                            Margin = 10f,
                        },
                    },
                ],
                Series =
                [
                    new ChartBarSeriesOption
                    {
                        Id = "horizontal-values",
                        Name = "值",
                        XAxisId = "horizontal-x",
                        YAxisId = "horizontal-category-y",
                        BarWidth = ChartLength.Pixels(20f),
                        BarCategoryGap = ChartLength.Percent(42f),
                        Z = 2,
                        DataSource = CreateData("horizontal-values", names, values),
                        ItemStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(49, 150, 250)),
                            BorderRadius = 30,
                        },
                    },
                    new ChartBarSeriesOption
                    {
                        Id = "horizontal-background",
                        Name = "背景",
                        XAxisId = "horizontal-x",
                        YAxisId = "horizontal-category-y",
                        BarWidth = ChartLength.Pixels(20f),
                        BarGap = ChartBarGap.Percent(-100f),
                        BarCategoryGap = ChartLength.Percent(42f),
                        Z = 1,
                        Silent = true,
                        DataSource = CreateData(
                            "horizontal-background",
                            names,
                            Enumerable.Repeat(100d, names.Length).ToArray()),
                        ItemStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(237, 237, 237)),
                            BorderRadius = 30,
                        },
                        Tooltip = new()
                        {
                            Show = false,
                        },
                    },
                ],
            };
        }

        /// <summary>创建左侧名称与排名 rich 文本使用的主类目轴。</summary>
        /// <param name="names">按展示顺序排列的业务名称。</param>
        /// <returns>逆序显示、隐藏轴线与刻度的 Y 类目轴。</returns>
        private static ChartYAxisOption CreateCategoryAxis(IReadOnlyList<string> names)
        {
            var axis = new ChartYAxisOption
            {
                Id = "horizontal-category-y",
                Type = ChartAxisType.Category,
                Position = ChartYAxisPosition.Left,
                Inverse = true,
                BoundaryGap = true,
                Data = names
                    .Select(static name => ChartValue.From(name))
                    .ToArray(),
                AxisLine = new() { Show = false },
                AxisTick = new() { Show = false },
                SplitLine = new() { Show = false },
                AxisLabel = new()
                {
                    Show = true,
                    Interval = 0,
                    Align = ChartTextAlign.Right,
                    Margin = 12f,
                    Color = ChartBrush.Solid(new SKColor(51, 51, 51)),
                    FontSize = 14f,
                    TextFormatter = static (in ChartFormatterParameters parameters) =>
                    {
                        var rank = parameters.DataIndex + 1;
                        var style = rank <= 3 ? $"index{rank}" : "index";
                        return ChartTextContent.FromRuns(
                        [
                            new ChartTextRun(rank.ToString(), style),
                            new ChartTextRun(" "),
                            new ChartTextRun(parameters.FormatValue(), "name"),
                        ]);
                    },
                    Rich = new()
                    {
                        ["index"] = CreateRankStyle(new SKColor(157, 157, 157)),
                        ["index1"] = CreateRankStyle(new SKColor(248, 119, 123)),
                        ["index2"] = CreateRankStyle(new SKColor(255, 161, 69)),
                        ["index3"] = CreateRankStyle(new SKColor(106, 222, 141)),
                        ["name"] = new ChartTextStyle
                        {
                            Width = 112f,
                            Align = ChartTextAlign.Left,
                            Color = ChartBrush.Solid(new SKColor(51, 51, 51)),
                            FontSize = 14f,
                        },
                    },
                },
            };
            return axis;
        }

        /// <summary>创建排名数字使用的加粗 rich 文本样式。</summary>
        /// <param name="color">当前排名使用的文字颜色。</param>
        /// <returns>右对齐、加粗的 rich 文本样式。</returns>
        private static ChartTextStyle CreateRankStyle(SKColor color) => new()
        {
            Color = ChartBrush.Solid(color),
            FontSize = 14f,
            FontWeight = 700,
            Align = ChartTextAlign.Right,
        };

        /// <summary>创建带稳定 Key 与业务名称的 value/category 二维 Bar 数据。</summary>
        /// <param name="seriesId">构造稳定 Key 使用的系列 ID。</param>
        /// <param name="names">按类目顺序排列的业务名称。</param>
        /// <param name="values">与名称一一对应的 X 数值。</param>
        /// <returns>使用 value/category 二维语法的横向 Bar 数据。</returns>
        private static ChartBarData CreateData(
            string seriesId,
            IReadOnlyList<string> names,
            IReadOnlyList<double> values)
        {
            if (names.Count != values.Count)
            {
                throw new ArgumentException("横向 Bar 名称与数值数量必须一致。", nameof(values));
            }

            var data = new ChartBarData();
            for (var index = 0; index < names.Count; index++)
            {
                // 横向 Bar 必须显式提供 [xValue, yCategory]；标量简写固定采用
                // rawIndex/value 语义，只适合 X category 的纵向 Bar。
                data.Add(new ChartBarDataItem(
                    ChartValue.From(values[index]),
                    ChartValue.From(names[index]))
                {
                    Key = $"{seriesId}:{index}",
                    Name = names[index],
                });
            }

            return data;
        }
    }
}
