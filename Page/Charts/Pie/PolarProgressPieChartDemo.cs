using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.CoordinateSystems.Polar;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Chart.Text;

namespace Page.Charts.Pie
{
    /// <summary>展示 Polar Bar 进度环、Pie 刻度环和多标题组合的复合图表。</summary>
    internal sealed class PolarProgressPieChartDemo : PieChartDemoPage
    {
        private const string PolarId = "pie-progress-polar";
        private const string RadiusAxisId = "pie-progress-radius";
        private const string AngleAxisId = "pie-progress-angle";

        /// <summary>创建柱饼进度图页面。</summary>
        internal PolarProgressPieChartDemo()
            : base(
                "柱饼图的完成进度",
                "使用 Polar Bar 绘制 80% 进度，并用两层 150 等份 Pie 构造细刻度环。",
                "Polar Bar 负责主进度和背景轨道，两个 Pie 系列分别绘制深色底圈与前 50 格渐变高亮；三个 Title 组件替代 graphic 文本，全部配置都保留在当前示例类中。",
                CreateOption(),
                CreateRegistry())
        {
        }

        /// <summary>创建同时包含默认 Pie 模块和 Polar 模块的注册表。</summary>
        /// <returns>可绘制 Pie 与 Polar Bar 的独立注册表。</returns>
        private static ChartRegistry CreateRegistry()
        {
            var registry = ChartModules.CreateDefaultRegistry();
            ChartPolarModule.Register(registry);
            return registry;
        }

        /// <summary>创建柱饼进度图的完整配置。</summary>
        /// <returns>包含标题、Polar、轴、Bar 和两个 Pie 系列的配置。</returns>
        private static ChartOption CreateOption()
        {
            var background = new SKColor(24, 15, 42);
            return new ChartOption
            {
                Background = ChartBrush.Solid(background),
                Animation = new()
                {
                    Enabled = false,
                },
                Components =
                {
                    CreateTitle(
                        "pie-progress-value-title",
                        "118",
                        "37%",
                        50f,
                        new SKColor(0, 255, 255),
                        100),
                    CreateTitle(
                        "pie-progress-main-title",
                        "DESIGN ELEMENTS",
                        "57%",
                        13f,
                        new SKColor(141, 135, 147),
                        400),
                    CreateTitle(
                        "pie-progress-sub-title",
                        "DONUT CHART",
                        "63%",
                        10f,
                        new SKColor(65, 63, 112),
                        400),
                },
                Polar = new ChartPolarOption
                {
                    Id = PolarId,
                    Center = ChartPolarCenter.From(
                        50f,
                        55f,
                        ChartLengthUnit.Percent),
                    Radius = new ChartPolarRadius(
                        ChartLength.Percent(52f),
                        ChartLength.Percent(73f)),
                },
                RadiusAxis = new ChartRadiusAxisOption
                {
                    Id = RadiusAxisId,
                    PolarId = PolarId,
                    Type = ChartAxisType.Category,
                    BoundaryGap = ChartAxisBoundaryGap.Category(true),
                    Data = ["完成度"],
                    AxisLine = new() { Show = false },
                    AxisTick = new() { Show = false },
                    AxisLabel = new() { Show = false },
                    SplitLine = new() { Show = false },
                },
                AngleAxis = new ChartAngleAxisOption
                {
                    Id = AngleAxisId,
                    PolarId = PolarId,
                    Type = ChartAxisType.Value,
                    Minimum = ChartAxisBound.Fixed(0d),
                    Maximum = ChartAxisBound.Fixed(100d),
                    StartValue = 0d,
                    StartAngle = 90d,
                    Clockwise = true,
                    Scale = true,
                    Show = false,
                    AxisLine = new() { Show = false },
                    AxisTick = new() { Show = false },
                    AxisLabel = new() { Show = false },
                    SplitLine = new() { Show = false },
                },
                Series =
                [
                    CreateProgressBar(),
                    CreateTickSeries(
                        "pie-progress-ticks-background",
                        "刻度底圈",
                        z: 1,
                        CreateBackgroundTicks()),
                    CreateTickSeries(
                        "pie-progress-ticks-foreground",
                        "高亮刻度",
                        z: 2,
                        CreateForegroundTicks()),
                ],
            };
        }

        /// <summary>创建居中显示的单行 Title 组件。</summary>
        private static ChartTitleOption CreateTitle(
            string id,
            string text,
            string top,
            float fontSize,
            SKColor color,
            int fontWeight)
        {
            return new ChartTitleOption
            {
                Id = id,
                Text = text,
                Left = ChartLength.Percent(50f),
                Top = top,
                TextAlign = ChartTextAlign.Center,
                Padding = new ChartInsets(0f),
                Silent = true,
                Z = 10,
                TextStyle = new ChartTextStyle
                {
                    FontSize = fontSize,
                    FontWeight = fontWeight,
                    Color = ChartBrush.Solid(color),
                    Align = ChartTextAlign.Center,
                },
            };
        }

        /// <summary>创建值为 80 的圆头 Polar Bar 进度环。</summary>
        private static ChartBarSeriesOption CreateProgressBar()
        {
            var data = new ChartBarData();
            data.Add(new ChartBarDataItem(
                ChartValue.From("完成度"),
                ChartValue.From(80d))
            {
                Key = "pie-progress-value",
                Name = "完成度",
            });

            return new ChartBarSeriesOption
            {
                Id = "pie-progress-bar",
                Name = "完成度",
                CoordinateSystem = ChartPolar2D.CoordinateSystemType,
                PolarId = PolarId,
                RoundCap = true,
                BarWidth = ChartLength.Pixels(12f),
                ShowBackground = true,
                BackgroundStyle = new()
                {
                    Color = ChartBrush.Solid(new SKColor(46, 40, 86)),
                    BorderRadius = 30f,
                },
                ItemStyle = new()
                {
                    Color = new ChartLinearGradientBrush(
                        new SKPoint(0f, 1f),
                        new SKPoint(0f, 0f),
                        [new SKColor(88, 95, 225), new SKColor(0, 255, 255)],
                        [0f, 1f]),
                    BorderRadius = 30f,
                },
                DataSource = data,
            };
        }

        /// <summary>创建位于相同半径上的 150 等份 Pie 刻度环。</summary>
        private static ChartPieSeriesOption CreateTickSeries(
            string id,
            string name,
            int z,
            ChartPieData data)
        {
            return new ChartPieSeriesOption
            {
                Id = id,
                Name = name,
                Z = z,
                Silent = true,
                Center = ChartPieCenter.Percent(50f, 55f),
                Radius = new ChartPieRadius(
                    ChartLength.Percent(50f),
                    ChartLength.Percent(58f)),
                Label = new()
                {
                    Show = false,
                    Position = ChartPieLabelPosition.Inside,
                },
                LabelLine = new()
                {
                    Show = false,
                },
                ItemStyle = new()
                {
                    BorderColor = ChartBrush.Solid(new SKColor(24, 15, 42)),
                    BorderWidth = 2f,
                },
                Emphasis = new()
                {
                    Disabled = true,
                    Scale = false,
                },
                DataSource = data,
            };
        }

        /// <summary>创建 150 格深色底圈。</summary>
        private static ChartPieData CreateBackgroundTicks()
        {
            var data = new ChartPieData();
            for (var index = 0; index < 150; index++)
            {
                data.Add(CreateTickItem(
                    $"pie-progress-background-{index}",
                    ChartBrush.Solid(new SKColor(37, 31, 69))));
            }

            return data;
        }

        /// <summary>创建前 50 格渐变、其余 100 格透明的高亮刻度圈。</summary>
        private static ChartPieData CreateForegroundTicks()
        {
            var gradient = new ChartLinearGradientBrush(
                new SKPoint(0f, 0f),
                new SKPoint(0f, 1f),
                [new SKColor(0, 255, 255), new SKColor(84, 103, 223)],
                [0f, 1f]);
            var transparent = ChartBrush.Solid(SKColors.Transparent);
            var data = new ChartPieData();
            for (var index = 0; index < 150; index++)
            {
                data.Add(CreateTickItem(
                    $"pie-progress-foreground-{index}",
                    index < 50 ? gradient : transparent));
            }

            return data;
        }

        /// <summary>创建单个等权重刻度扇区。</summary>
        private static ChartPieDataItem CreateTickItem(string key, ChartBrush color)
        {
            return new ChartPieDataItem(1d, key)
            {
                Key = key,
                ItemStyle = new()
                {
                    Color = color,
                },
            };
        }
    }
}
