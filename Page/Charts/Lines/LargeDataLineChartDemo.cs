using System.Globalization;
using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Extensions;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Lines
{
    /// <summary>展示使用 LTTB 降采样绘制 10,000 个连续模拟数据点的折线图。</summary>
    internal sealed class LargeDataLineChartDemo : LineChartDemoPage
    {
        /// <summary>模拟数据包含的原始采样点数量。</summary>
        private const int DataPointCount = 10_000;

        /// <summary>大数据折线图使用的蓝色。</summary>
        private static readonly SKColor LineColor = new(22, 119, 255);

        /// <summary>创建 10K 模拟数据折线图示例页面。</summary>
        internal LargeDataLineChartDemo()
            : base(
                "10K 模拟数据折线图",
                "生成 10,000 个可复现的连续采样点，并通过 inside dataZoom 放大查看局部细节。",
                "原始 10,000 个数据点始终完整保留；移动鼠标会显示自由跟随的十字辅助线、轴值标签和数据提示。按住 Ctrl 滚动滚轮可缩放，按住左键拖动可平移，双击恢复完整范围。当前窗口仍通过 LTTB 按画布宽度降采样。",
                CreateOption())
        {
        }

        /// <summary>创建 10K 模拟数据折线图的完整配置。</summary>
        /// <returns>包含连续数值轴和 LTTB 折线系列的图表配置。</returns>
        private static ChartOption CreateOption()
        {
            // 1. 连续 X 轴不需要额外创建 10,000 个类目字符串。
            var data = CreateSimulationData();

            var option = new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                // 2. 大数据示例直接显示最终状态，避免对采样后的路径播放首次动画。
                Animation = new()
                {
                    Enabled = false,
                },
                // 3. 十字辅助线自由跟随鼠标；X/Y 轴值标签和数据提示均保留两位小数。
                Tooltip = new()
                {
                    Id = "large-data-axis-pointer",
                    Trigger = ChartTooltipTrigger.Axis,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove,
                    ShowContent = true,
                    Confine = true,
                    ValueFormatter = static (value, _) =>
                    {
                        // 二维 Line 的 Tooltip 值为 [X, Y]，最后一维就是要显示的模拟值。
                        var displayValue = value.Values.Count > 1
                            ? value.Values[^1]
                            : value.Primary;
                        return FormatValueWithTwoDecimals(displayValue);
                    },
                    AxisPointer = new()
                    {
                        Type = ChartTooltipAxisPointerType.Cross,
                        Axis = ChartTooltipAxisPointerAxis.X,
                        Snap = false,
                        Precision = 2,
                        Animation = ChartTooltipAxisPointerAnimation.Disabled,
                        CrossStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(100, 108, 120, 170)),
                            Width = 1f,
                            Type = "solid",
                        },
                        CrossTextStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(65, 72, 84)),
                            BackgroundColor = ChartBrush.Solid(SKColors.White),
                            BorderColor = ChartBrush.Solid(new SKColor(210, 215, 224)),
                            BorderWidth = 1f,
                            BorderRadius = 2f,
                            Padding = new ChartInsets(horizontal: 5f, vertical: 3f),
                        },
                    },
                },
                Grid = new ChartGridOption
                {
                    Show = true,
                    Left = "12px",
                    Top = "24px",
                    Right = "24px",
                    Bottom = "30px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                    BackgroundColor = ChartBrush.Solid(SKColors.White),
                },
                // 4. 页面外层滚动容器继续消费普通滚轮；Ctrl + 滚轮只缩放当前 X 轴窗口。
                DataZoom = new ChartDataZoomInsideOption
                {
                    Id = "large-data-inside",
                    XAxisIndex = 0,
                    Start = 0d,
                    End = 100d,
                    MinSpan = 1d,
                    FilterMode = ChartDataWindowFilterMode.Filter,
                    ZoomOnMouseWheel = ChartDataZoomInteractionTrigger.Ctrl,
                    MoveOnMouseMove = ChartDataZoomInteractionTrigger.Enabled,
                    MoveOnMouseWheel = ChartDataZoomInteractionTrigger.Disabled,
                    ResetOnDoubleClick = true,
                },
                XAxis = new ChartXAxisOption
                {
                    Name = "采样序号",
                    NameLocation = ChartAxisNameLocation.Middle,
                    NameGap = 38f,
                    Type = ChartAxisType.Value,
                    Scale = true,
                    AxisLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(145, 153, 166)),
                            Width = 1f,
                        },
                    },
                    AxisTick = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(145, 153, 166)),
                        },
                    },
                    AxisLabel = new()
                    {
                        Show = true,
                        Color = ChartBrush.Solid(new SKColor(45, 52, 65)),
                        FontSize = 14f,
                        // dataZoom 的窗口边界也属于 X 轴刻度，因此在这里统一保留两位小数。
                        FormatterCallback = static (in ChartAxisLabelFormatterContext context) =>
                            FormatValueWithTwoDecimals(context.Value),
                    },
                    NameTextStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(45, 52, 65)),
                        FontSize = 14f,
                    },
                    SplitLine = new()
                    {
                        Show = false,
                    },
                },
                YAxis = new ChartYAxisOption
                {
                    Name = "模拟值",
                    NameLocation = ChartAxisNameLocation.Start,
                    NameGap = 12f,
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(20d),
                    Maximum = ChartAxisBound.Fixed(90d),
                    Interval = 10d,
                    Scale = true,
                    AxisLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(145, 153, 166)),
                            Width = 1f,
                        },
                    },
                    AxisTick = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(145, 153, 166)),
                        },
                    },
                    AxisLabel = new()
                    {
                        Show = true,
                        Color = ChartBrush.Solid(new SKColor(45, 52, 65)),
                        FontSize = 14f,
                    },
                    NameTextStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(45, 52, 65)),
                        FontSize = 14f,
                    },
                    SplitLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(224, 230, 239)),
                            Type = ChartAxisLineType.Dashed,
                            Width = 1f,
                        },
                    },
                },
                // 5. LTTB 只处理当前 dataZoom 窗口；关闭 Symbol/Label/EndLabel，避免创建额外视觉节点。
                Series = new ChartLineSeriesOption
                {
                    Id = "large-data-value",
                    Name = "模拟值",
                    Sampling = ChartLineSampling.Lttb,
                    ShowSymbol = false,
                    Clip = true,
                    DataSource = data,
                    LineStyle = new()
                    {
                        Color = ChartBrush.Solid(LineColor),
                        Width = 1.8f,
                        Cap = SKStrokeCap.Round,
                        Join = SKStrokeJoin.Round,
                    },
                    ItemStyle = new()
                    {
                        Color = ChartBrush.Solid(LineColor),
                    },
                    Label = new()
                    {
                        Show = false,
                    },
                    EndLabel = new()
                    {
                        Show = false,
                    },
                    Emphasis = new()
                    {
                        Focus = "series",
                        BlurScope = "coordinateSystem",
                    },
                },
            };

            return option;
        }

        /// <summary>把数值格式化为固定两位小数，非数值保持原始文本。</summary>
        /// <param name="value">待显示的图表值。</param>
        /// <returns>固定两位小数或原始非数值文本。</returns>
        private static string FormatValueWithTwoDecimals(ChartValue value) =>
            value.Kind == ChartValueKind.Number
            && value.TryGetDouble(out var numeric)
                ? numeric.ToString("F2", CultureInfo.InvariantCulture)
                : value.AsString(CultureInfo.InvariantCulture) ?? "-";

        /// <summary>生成包含慢周期、短周期、轻微漂移和细微扰动的确定性模拟数据。</summary>
        /// <returns>包含 10,000 个 X/Y 连续数值项的 Line 数据源。</returns>
        private static ChartLineData CreateSimulationData()
        {
            var data = new ChartLineData();
            for (var index = 0; index < DataPointCount; index++)
            {
                var value =
                    50d
                    + 12d * Math.Sin(index * 2d * Math.PI / 2_400d)
                    + 4d * Math.Sin(index * 2d * Math.PI / 180d)
                    + 0.0008d * index
                    + 1.5d * Math.Sin(index * 0.61803398875d);

                data.Add(index, Math.Round(value, 2, MidpointRounding.AwayFromZero));
            }

            return data;
        }
    }
}
