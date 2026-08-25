using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Bars
{
    /// <summary>展示两个柱系列和一个折线系列分别绑定三条 Y 轴的组合图。</summary>
    internal sealed class MultipleYAxisBarChartDemo : BarChartDemoPage
    {
        /// <summary>蒸发量系列及第一条右轴使用的蓝色。</summary>
        private static readonly SKColor EvaporationColor = new(80, 112, 221);

        /// <summary>降水量系列及第二条右轴使用的黄绿色。</summary>
        private static readonly SKColor PrecipitationColor = new(182, 214, 52);

        /// <summary>温度折线及左轴使用的深灰蓝色。</summary>
        private static readonly SKColor TemperatureColor = new(80, 83, 114);

        /// <summary>创建多 Y 轴图页面。</summary>
        internal MultipleYAxisBarChartDemo()
            : base(
                "多 Y 轴图",
                "蒸发量、降水量和温度分别使用三条独立数值轴。",
                "两条 ml 轴放在右侧，其中第二条通过 Offset 向外移动 80px；温度轴保留在左侧。三个系列通过 YAxisIndex 明确绑定，Cross 辅助线可同时读取当前月份的三组值。",
                CreateOption())
        {
        }

        /// <summary>创建包含三条 Y 轴、两个 Bar 和一个 Line 的完整配置。</summary>
        /// <returns>包含三轴绑定与 Cross Tooltip 的完整配置。</returns>
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
                    Id = "multiple-y-tooltip",
                    Trigger = ChartTooltipTrigger.Axis,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                    AxisPointer = new()
                    {
                        Type = ChartTooltipAxisPointerType.Cross,
                        Axis = ChartTooltipAxisPointerAxis.X,
                        Snap = false,
                        Precision = 2,
                    },
                },
                Legend = new ChartLegendOption
                {
                    Id = "multiple-y-legend",
                    Top = "8px",
                    Left = ChartLength.FromKeyword("center"),
                    ItemGap = 24f,
                    TextStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(62, 69, 82)),
                        FontSize = 14f,
                    },
                },
                Grid = new ChartGridOption
                {
                    Left = "30px",
                    Top = "58px",
                    Right = "50px",
                    Bottom = "36px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                },
                XAxis = new ChartXAxisOption
                {
                    Id = "multiple-y-x",
                    Type = ChartAxisType.Category,
                    BoundaryGap = true,
                    Data = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                    AxisTick = new()
                    {
                        Show = true,
                        AlignWithLabel = true,
                    },
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
                },
                YAxis =
                [
                    CreateValueAxis(
                        "multiple-y-evaporation-axis",
                        "Evaporation",
                        ChartYAxisPosition.Right,
                        offset: 0f,
                        EvaporationColor,
                        "{value} ml",
                        showSplitLine: true),
                    CreateValueAxis(
                        "multiple-y-precipitation-axis",
                        "Precipitation",
                        ChartYAxisPosition.Right,
                        offset: 80f,
                        PrecipitationColor,
                        "{value} ml",
                        showSplitLine: false),
                    CreateValueAxis(
                        "multiple-y-temperature-axis",
                        "温度",
                        ChartYAxisPosition.Left,
                        offset: 0f,
                        TemperatureColor,
                        "{value} °C",
                        showSplitLine: false),
                ],
                Series =
                [
                    new ChartBarSeriesOption
                    {
                        Id = "multiple-y-evaporation",
                        Name = "Evaporation",
                        XAxisIndex = 0,
                        YAxisIndex = 0,
                        BarMaxWidth = ChartLength.Pixels(28f),
                        BarGap = ChartBarGap.Percent(30f),
                        DataSource = new ChartBarData(2.0d, 4.9d, 7.0d, 23.2d, 25.6d, 76.7d, 135.6d, 162.2d, 32.6d, 20.0d, 6.4d, 3.3d),
                        ItemStyle = new()
                        {
                            Color = ChartBrush.Solid(EvaporationColor),
                            BorderRadius = ChartBorderRadius.FromValues(4, 4, 0, 0),
                        },
                        Emphasis = new()
                        {
                            Focus = "series",
                            BlurScope = "coordinateSystem",
                        },
                    },
                    new ChartBarSeriesOption
                    {
                        Id = "multiple-y-precipitation",
                        Name = "Precipitation",
                        XAxisIndex = 0,
                        YAxisIndex = 1,
                        BarMaxWidth = ChartLength.Pixels(28f),
                        BarGap = ChartBarGap.Percent(30f),
                        DataSource = new ChartBarData(2.6d,5.9d,9.0d,26.4d,28.7d,70.7d,175.6d,182.2d,48.7d,18.8d,6.0d,2.3d),
                        ItemStyle = new()
                        {
                            Color = ChartBrush.Solid(PrecipitationColor),
                            BorderRadius = ChartBorderRadius.FromValues(4, 4, 0, 0),
                        },
                        Emphasis = new()
                        {
                            Focus = "series",
                            BlurScope = "coordinateSystem",
                        },
                    },
                    new ChartLineSeriesOption
                    {
                        Id = "multiple-y-temperature",
                        Name = "Temperature",
                        XAxisIndex = 0,
                        YAxisIndex = 2,
                        ShowSymbol = true,
                        ShowAllSymbol = ChartLineShowAllSymbol.All,
                        Symbol = "circle",
                        SymbolSize = 9f,
                        Clip = true,
                        DataSource = new ChartLineData(2.0d, 2.2d, 3.3d, 4.5d, 6.3d, 10.2d, 20.3d, 23.4d, 23.0d, 16.5d, 12.0d, 6.2d),
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(TemperatureColor),
                            Width = 2.5f,
                            Cap = SKStrokeCap.Round,
                            Join = SKStrokeJoin.Round,
                        },
                        ItemStyle = new()
                        {
                            Color = ChartBrush.Solid(TemperatureColor),
                            BorderColor = ChartBrush.Solid(SKColors.White),
                            BorderWidth = 1.5f,
                        },
                        Emphasis = new()
                        {
                            Focus = "series",
                            BlurScope = "coordinateSystem",
                        },
                    },
                ],
            };
        }

        /// <summary>创建多 Y 轴示例中的单条数值轴。</summary>
        /// <param name="id">轴稳定 ID。</param>
        /// <param name="name">轴名称。</param>
        /// <param name="position">轴位于 Grid 左侧或右侧。</param>
        /// <param name="offset">相对同侧默认位置的像素偏移。</param>
        /// <param name="color">轴线、刻度和文字颜色。</param>
        /// <param name="formatter">刻度标签模板。</param>
        /// <param name="showSplitLine">是否由当前轴绘制横向分隔线。</param>
        /// <returns>可直接放入 YAxis 集合的数值轴。</returns>
        private static ChartYAxisOption CreateValueAxis(
            string id,
            string name,
            ChartYAxisPosition position,
            float offset,
            SKColor color,
            string formatter,
            bool showSplitLine)
        {
            return new ChartYAxisOption
            {
                Id = id,
                Type = ChartAxisType.Value,
                Name = name,
                NameLocation = ChartAxisNameLocation.Start,
                NameGap = 10f,
                Position = position,
                Offset = offset,
                AlignTicks = true,
                Scale = true,
                AxisLine = new()
                {
                    Show = true,
                    LineStyle = new()
                    {
                        Color = ChartBrush.Solid(color),
                        Width = 1.5f,
                    },
                },
                AxisTick = new()
                {
                    Show = true,
                    LineStyle = new()
                    {
                        Color = ChartBrush.Solid(color),
                    },
                },
                AxisLabel = new()
                {
                    Formatter = formatter,
                    Color = ChartBrush.Solid(color),
                },
                NameTextStyle = new()
                {
                    Color = ChartBrush.Solid(color),
                    FontSize = 13f,
                },
                SplitLine = new()
                {
                    Show = showSplitLine,
                    LineStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(228, 233, 241)),
                        Type = ChartAxisLineType.Dashed,
                    },
                },
            };
        }
    }
}
