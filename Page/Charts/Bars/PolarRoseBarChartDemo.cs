using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.CoordinateSystems.Polar;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Bars
{
    /// <summary>展示 Angle category 与 Radius value 组合形成的极坐标玫瑰柱。</summary>
    internal sealed class PolarRoseBarChartDemo : BarChartDemoPage
    {
        private const string PolarId = "polar-rose-system";

        private static readonly string[] Months =
        [
            "1月", "2月", "3月", "4月", "5月", "6月",
            "7月", "8月", "9月", "10月", "11月", "12月",
        ];

        private static readonly double[] Values =
        [
            5.2411d, 2.4115d, 6.7911d, 8.8065d, 5.7217d, 5.3058d,
            3.9398d, 0d, 0d, 0d, 0.8595d, 2.3129d,
        ];

        private static readonly SKColor[] Colors =
        [
            new(245, 77, 77),
            new(248, 117, 68),
            new(255, 174, 0),
            new(220, 255, 0),
            new(37, 208, 83),
            new(1, 255, 245),
            new(0, 124, 255),
            new(66, 69, 255),
            new(195, 46, 255),
            new(255, 98, 232),
            new(255, 98, 232),
            new(255, 98, 232),
        ];

        /// <summary>创建极坐标玫瑰图页面。</summary>
        internal PolarRoseBarChartDemo()
            : base(
                "极坐标系下玫瑰图",
                "以月份作为角度类目，以业务值作为径向长度绘制十二个彩色扇形柱。",
                "AngleAxis 是 category base axis，每个月占据一个角度带；RadiusAxis 固定为 0～10，并用“万”作为单位。每个数据项通过 ItemStyle 设置独立颜色。",
                CreateOption(),
                CreateRegistry())
        {
        }

        /// <summary>创建包含 Polar 模块的独立注册表。</summary>
        /// <returns>可绘制 Polar Bar 的注册表。</returns>
        private static ChartRegistry CreateRegistry()
        {
            var registry = ChartModules.CreateDefaultRegistry();
            ChartPolarModule.Register(registry);
            return registry;
        }

        /// <summary>创建深色极坐标玫瑰图的完整配置。</summary>
        /// <returns>包含十二个月份、径向刻度和逐项颜色的配置。</returns>
        private static ChartOption CreateOption()
        {
            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 750,
                    UpdateDuration = 450,
                },
                Tooltip = new ChartTooltipOption
                {
                    Id = "polar-rose-tooltip",
                    Trigger = ChartTooltipTrigger.Item,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                    BackgroundColor = ChartBrush.Solid(new SKColor(0, 25, 64, 200)),
                    BorderColor = ChartBrush.Solid(new SKColor(0, 199, 255)),
                    BorderWidth = 1f,
                    TextStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(255, 255, 255)),
                        FontSize = 14f,
                    },
                },
                Polar = new ChartPolarOption
                {
                    Id = PolarId,
                    Center = ChartPolarCenter.Default,
                    Radius = ChartPolarRadius.FromOuter(ChartLength.Percent(72f)),
                },
                AngleAxis = new ChartAngleAxisOption
                {
                    Id = "polar-rose-angle",
                    PolarId = PolarId,
                    Type = ChartAxisType.Category,
                    BoundaryGap = ChartAxisBoundaryGap.Category(true),
                    StartAngle = 90d,
                    Clockwise = true,
                    Data = Months
                        .Select(static month => ChartValue.From(month))
                        .ToArray(),
                    AxisLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(0, 199, 255)),
                            Width = 1f,
                            Type = ChartAxisLineType.Solid,
                        },
                    },
                    AxisTick = new() { Show = false },
                    AxisLabel = new()
                    {
                        Interval = 0,
                        Show = true,
                        Color = ChartBrush.Solid(new SKColor(0, 199, 255)),
                        Margin = 8f,
                        FontSize = 16f,
                        HideOverlap = false,
                    },
                    SplitLine = new() { Show = false },
                },
                RadiusAxis = new ChartRadiusAxisOption
                {
                    Id = "polar-rose-radius",
                    PolarId = PolarId,
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(0d),
                    Maximum = ChartAxisBound.Fixed(10d),
                    Interval = 1d,
                    StartValue = 0d,
                    Scale = true,
                    AxisLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(0, 199, 255)),
                            Width = 1f,
                            Type = ChartAxisLineType.Solid,
                        },
                    },
                    AxisTick = new() { Show = false },
                    AxisLabel = new()
                    {
                        Show = true,
                        Formatter = "{value} 万",
                        Color = ChartBrush.Solid(new SKColor(0, 199, 255)),
                        FontSize = 14f,
                    },
                    SplitLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(0, 199, 255, 150)),
                            Width = 1f,
                            Type = ChartAxisLineType.Solid,
                        },
                    },
                },
                Series = new ChartBarSeriesOption
                {
                    Id = "polar-rose-values",
                    Name = "月度数值",
                    CoordinateSystem = ChartPolar2D.CoordinateSystemType,
                    PolarId = PolarId,
                    BarWidth = ChartLength.Percent(68f),
                    DataSource = CreateData(),
                    ItemStyle = new()
                    {
                        BorderRadius = ChartBorderRadius.FromValues(4, 4, 0, 0),
                    },
                    Emphasis = new()
                    {
                        Focus = "series",
                        BlurScope = "coordinateSystem",
                    },
                },
            };
        }

        /// <summary>创建 Radius value、Angle category 二维数据并设置逐项颜色。</summary>
        /// <returns>十二个月份对应的富 Bar 数据。</returns>
        private static ChartBarData CreateData()
        {
            var data = new ChartBarData();
            for (var index = 0; index < Months.Length; index++)
            {
                var item = new ChartBarDataItem(
                    ChartValue.From(Values[index]),
                    ChartValue.From(Months[index]))
                {
                    Key = $"polar-rose:{Months[index]}",
                    Name = Months[index],
                };
                item.ItemStyle.Color = ChartBrush.Solid(Colors[index]);
                data.Add(item);
            }

            return data;
        }
    }
}
