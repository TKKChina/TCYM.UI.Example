using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.CoordinateSystems.Polar;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Bars
{
    /// <summary>展示 Radius category 与 Angle value 组合形成的极坐标环向堆积柱。</summary>
    internal sealed class PolarStackBarChartDemo : BarChartDemoPage
    {
        private const string PolarId = "polar-stack-system";
        private const string RadiusAxisId = "polar-stack-radius";
        private const string AngleAxisId = "polar-stack-angle";

        /// <summary>创建极坐标堆积柱状图页面。</summary>
        internal PolarStackBarChartDemo()
            : base(
                "极坐标系下的堆积柱状图",
                "以星期作为半径类目，以数值作为角度长度绘制三个堆积系列。",
                "RadiusAxis 是 category base axis，因此每个星期占据一条同心圆带；A、B、C 使用同一个 stack，后续系列从前一系列的角度终点继续绘制。",
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

        /// <summary>创建极坐标堆积柱状图的完整配置。</summary>
        /// <returns>包含 Polar、两条轴、Legend、Tooltip 和三个 stack 系列的配置。</returns>
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
                    Id = "polar-stack-tooltip",
                    Trigger = ChartTooltipTrigger.Item,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                },
                Legend = new ChartLegendOption
                {
                    Id = "polar-stack-legend",
                    Show = true,
                    Top = "10px",
                    Left = ChartLength.FromKeyword("center"),
                    ItemGap = 28f,
                    TextStyle = new()
                    {
                        Color = ChartBrush.Solid(new SKColor(55, 63, 78)),
                        FontSize = 14f,
                    },
                },
                Polar = new ChartPolarOption
                {
                    Id = PolarId,
                    Center = ChartPolarCenter.Default,
                    Radius = new ChartPolarRadius(
                        ChartLength.Percent(12f),
                        ChartLength.Percent(74f)),
                },
                RadiusAxis = new ChartRadiusAxisOption
                {
                    Id = RadiusAxisId,
                    PolarId = PolarId,
                    Type = ChartAxisType.Category,
                    BoundaryGap = ChartAxisBoundaryGap.Category(true),
                    Data = ["Mon", "Tue", "Wed", "Thu"],
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
                        FontSize = 14f,
                    },
                    SplitLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(224, 230, 239)),
                            Type = ChartAxisLineType.Dashed,
                        },
                    },
                },
                AngleAxis = new ChartAngleAxisOption
                {
                    Id = AngleAxisId,
                    PolarId = PolarId,
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(0d),
                    Maximum = ChartAxisBound.Fixed(18d),
                    Interval = 2d,
                    StartValue = 0d,
                    StartAngle = 90d,
                    Clockwise = true,
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
                        Color = ChartBrush.Solid(new SKColor(90, 98, 112)),
                        FontSize = 12f,
                    },
                    SplitLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Color = ChartBrush.Solid(new SKColor(224, 230, 239)),
                            Type = ChartAxisLineType.Dashed,
                        },
                    },
                },
                Series =
                [
                    CreateSeries(
                        "polar-stack-a",
                        "A",
                        new SKColor(80, 112, 221),
                        [1d, 2d, 3d, 4d]),
                    CreateSeries(
                        "polar-stack-b",
                        "B",
                        new SKColor(145, 204, 117),
                        [2d, 4d, 6d, 8d]),
                    CreateSeries(
                        "polar-stack-c",
                        "C",
                        new SKColor(250, 200, 88),
                        [1d, 2d, 3d, 4d]),
                ],
            };
        }

        /// <summary>创建绑定 Radius category / Angle value 的单个堆积系列。</summary>
        /// <param name="id">系列稳定 ID。</param>
        /// <param name="name">Legend 与 Tooltip 显示名称。</param>
        /// <param name="color">扇环填充色。</param>
        /// <param name="values">Mon 到 Thu 的角度数值。</param>
        /// <returns>属于公共 polar-stack 分组的 Bar 系列。</returns>
        private static ChartBarSeriesOption CreateSeries(
            string id,
            string name,
            SKColor color,
            IReadOnlyList<double> values)
        {
            var categories = new[] { "Mon", "Tue", "Wed", "Thu" };
            var data = new ChartBarData();
            for (var index = 0; index < categories.Length; index++)
            {
                data.Add(new ChartBarDataItem(
                    ChartValue.From(categories[index]),
                    ChartValue.From(values[index]))
                {
                    Key = $"{id}:{categories[index]}",
                    Name = categories[index],
                });
            }

            return new ChartBarSeriesOption
            {
                Id = id,
                Name = name,
                CoordinateSystem = ChartPolar2D.CoordinateSystemType,
                PolarId = PolarId,
                Stack = "polar-stack",
                BarMaxWidth = ChartLength.Pixels(34f),
                BarGap = ChartBarGap.Percent(8f),
                DataSource = data,
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(color),
                    BorderRadius = 3,
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
