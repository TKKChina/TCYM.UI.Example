using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Chart.Series.Stack;

namespace Page.Charts.Bars
{
    /// <summary>展示两个堆积柱系列与一个总量折线系列共用坐标轴的混合图。</summary>
    internal sealed class MixedBarLineChartDemo : BarChartDemoPage
    {
        /// <summary>老用户柱使用的珊瑚色。</summary>
        private static readonly SKColor ExistingUserColor = new(255, 144, 128);

        /// <summary>新用户柱使用的青绿色。</summary>
        private static readonly SKColor NewUserColor = new(0, 191, 183);

        /// <summary>总量折线使用的黄色。</summary>
        private static readonly SKColor TotalColor = new(252, 230, 48);

        /// <summary>创建折柱混合示例页面。</summary>
        internal MixedBarLineChartDemo()
            : base(
                "折柱混合",
                "堆积柱展示老用户与新用户，折线展示每月用户总量。",
                "两个 Bar 使用同一个 stack 形成月度总柱高；总量已经是两组数据之和，因此 Line 不再重复加入 stack。移动鼠标可通过 shadow 辅助带同时查看三个系列，并使用 focus=series 突出当前系列。",
                CreateOption())
        {
        }

        /// <summary>创建折柱混合图的完整配置。</summary>
        /// <returns>包含 Tooltip、Legend、坐标轴和三个系列的完整配置。</returns>
        private static ChartOption CreateOption()
        {
            var option = new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 700,
                    UpdateDuration = 500,
                },
                Tooltip = new ChartTooltipOption
                {
                    Id = "mixed-tooltip",
                    Trigger = ChartTooltipTrigger.Axis,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                    AxisPointer = new()
                    {
                        Type = ChartTooltipAxisPointerType.Shadow,
                        Axis = ChartTooltipAxisPointerAxis.X,
                    },
                    FormatterCallback = (in ChartTooltipFormatterContext context) =>
                    {
                        if (context.Parameters.Count == 0)
                        {
                            return null;
                        }

                        var parameters = context.Parameters;
                        var culture = context.Culture;
                        var category = parameters[0].Name ?? "-";
                        var header = category.EndsWith('月')
                            ? category
                            : $"{category}月";
                        var lines = parameters
                            .Select(parameter => new ChartTooltipContentLine(
                                name: parameter.SeriesName ?? "数值",
                                values:
                                [
                                    parameter.Value.Primary.AsString(culture) ?? "-",
                                ],
                                markerColor: parameter.Color
                                    ?? ChartBrush.Solid(SKColors.Gray),
                                markerType: "circle"))
                            .ToArray();

                        return ChartTooltipContent.FromSections(
                        [
                            new ChartTooltipContentSection(
                                header: header,
                                lines: lines),
                        ]);
                    },
                },
                Legend = new ChartLegendOption
                {
                    Id = "mixed-legend",
                    Top = "10px",
                    Left = ChartLength.FromKeyword("center"),
                    Orient = ChartLegendOrient.Horizontal,
                    ItemGap = 24f,
                    TextStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(144, 151, 156)),
                        FontSize = 14f,
                    },
                },
                Grid = new ChartGridOption
                {
                    Left = "24px",
                    Top = "58px",
                    Right = "24px",
                    Bottom = "34px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                    BackgroundColor = ChartBrush.Solid(SKColors.White),
                },
                XAxis = new ChartXAxisOption
                {
                    Id = "mixed-x",
                    Type = ChartAxisType.Category,
                    BoundaryGap = true,
                    Data = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"],
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
                        Interval = 0,
                        Color = ChartBrush.Solid(new SKColor(82, 89, 96)),
                    },
                    SplitLine = new() { Show = false },
                },
                YAxis = new ChartYAxisOption
                {
                    Id = "mixed-y",
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(0d),
                    Maximum = ChartAxisBound.Fixed(600d),
                    Interval = 100d,
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
                        Color = ChartBrush.Solid(new SKColor(82, 89, 96)),
                    },
                    SplitLine = new() { Show = false },
                },
                Series =
                [
                    CreateBarSeries(
                        "mixed-existing",
                        "老用户",
                        ExistingUserColor,
                        [198.66d, 330.81d, 151.95d, 160.12d, 222.56d, 229.05d, 128.53d, 250.91d, 224.47d, 473.99d, 126.85d, 260.50d],
                        labelPosition: "insideTop",
                        labelColor: SKColors.White),
                    CreateBarSeries(
                        "mixed-new",
                        "新用户",
                        NewUserColor,
                        [82.89d, 67.54d, 62.07d, 59.43d, 67.02d, 67.09d, 35.66d, 71.78d, 81.61d, 78.85d, 79.12d, 72.30d],
                        labelPosition: "top",
                        labelColor: NewUserColor),
                    new ChartLineSeriesOption
                    {
                        Id = "mixed-total",
                        Name = "总",
                        XAxisId = "mixed-x",
                        YAxisId = "mixed-y",
                        ShowSymbol = true,
                        ShowAllSymbol = ChartLineShowAllSymbol.All,
                        Symbol = "circle",
                        SymbolSize = 16f,
                        Clip = true,
                        DataSource = new ChartLineData(
                            281.55d, 398.35d, 214.02d, 219.55d, 289.57d, 296.14d,
                            164.18d, 322.69d, 306.08d, 552.84d, 205.97d, 332.79d),
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(TotalColor),
                            Width = 2.5f,
                            Cap = SKStrokeCap.Round,
                            Join = SKStrokeJoin.Round,
                        },
                        ItemStyle = new()
                        {
                            Color = ChartBrush.Solid(TotalColor),
                            BorderColor = ChartBrush.Solid(SKColors.White),
                            BorderWidth = 2f,
                        },
                        Label = new()
                        {
                            Show = true,
                            Position = "top",
                            Distance = 8f,
                            Color = ChartBrush.Solid(new SKColor(173, 145, 0)),
                            FormatterCallback = FormatPositiveValue,
                        },
                        Emphasis = new()
                        {
                            Focus = "series",
                            BlurScope = "coordinateSystem",
                        },
                        Blur = new()
                        {
                            // Line path 与 symbol 使用框架默认 normal opacity × 0.1；Label 当前
                            // 没有独立 opacity 字段，因此示例用同色 10% Alpha 保持一致弱化。
                            Label = new()
                            {
                                Color = ChartBrush.Solid(TotalColor.WithAlpha(26)),
                            },
                        },
                    },
                ],
            };

            return option;
        }

        /// <summary>创建参与“用户总量”堆积的单个 Bar 系列。</summary>
        /// <param name="id">系列稳定 ID。</param>
        /// <param name="name">Legend 与 Tooltip 显示名称。</param>
        /// <param name="color">柱体填充色。</param>
        /// <param name="values">按月份排列的数据。</param>
        /// <param name="labelPosition">标签位置关键字。</param>
        /// <param name="labelColor">标签文字颜色。</param>
        /// <returns>绑定混合图坐标轴和用户堆积组的 Bar 系列。</returns>
        private static ChartBarSeriesOption CreateBarSeries(
            string id,
            string name,
            SKColor color,
            IReadOnlyList<double> values,
            string labelPosition,
            SKColor labelColor)
        {
            var data = new ChartBarData();
            for (var index = 0; index < values.Count; index++)
            {
                data.Add(new ChartBarDataItem(values[index])
                {
                    Key = $"{id}:{index + 1}",
                    Name = (index + 1).ToString(),
                });
            }

            return new ChartBarSeriesOption
            {
                Id = id,
                Name = name,
                XAxisId = "mixed-x",
                YAxisId = "mixed-y",
                Stack = "user-total",
                StackStrategy = ChartStackStrategy.SameSign,
                StackOrder = ChartStackOrder.SeriesAscending,
                BarMaxWidth = ChartLength.Pixels(35f),
                BarGap = ChartBarGap.Percent(10f),
                DataSource = data,
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(color),
                },
                Label = new()
                {
                    Show = true,
                    Position = labelPosition,
                    Distance = 5f,
                    Color = ChartBrush.Solid(labelColor),
                    FormatterCallback = FormatPositiveValue,
                },
                Emphasis = new()
                {
                    Focus = "series",
                    BlurScope = "coordinateSystem",
                },
            };
        }

        /// <summary>只显示正数，并用最多两位小数输出标签。</summary>
        /// <param name="context">当前数据项的格式化上下文。</param>
        /// <returns>正数文本或空字符串。</returns>
        private static string FormatPositiveValue(ChartFormatterContext context) =>
            context.PrimaryValue.TryGetDouble(out var value) && value > 0d
                ? value.ToString("0.##", context.Culture)
                : string.Empty;
    }
}
