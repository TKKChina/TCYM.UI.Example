using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Radar
{
    /// <summary>并排展示自动指标范围的圆形网格与固定范围的六边形网格。</summary>
    internal sealed class CustomizedRadarChartDemo : RadarChartDemoPage
    {
        private static readonly SKColor ColorA = SKColor.Parse("#67f9d8");
        private static readonly SKColor ColorB = SKColor.Parse("#ffe434");
        private static readonly SKColor ColorC = SKColor.Parse("#56a3f1");
        private static readonly SKColor ColorD = SKColor.Parse("#ff917c");

        internal CustomizedRadarChartDemo()
            : base(
                "自定义雷达图",
                "两个 Radar 坐标分别展示圆形分区、矩形节点、数值标签和径向渐变。",
                "左侧五维数据包含负值，各指标自动确定范围；右侧六维数据使用固定最大值。Data C 显示虚线、矩形节点与当前指标值，Data D 使用径向渐变填充。",
                CreateOption())
        {
        }

        private static ChartOption CreateOption()
        {
            var firstData = new ChartRadarData()
                .Add(new ChartRadarDataItem([100, 8, 0.4, -80, 2000])
                {
                    Key = "data-a", Name = "Data A",
                    ItemStyle = new() { Color = ChartBrush.Solid(ColorA) },
                })
                .Add(new ChartRadarDataItem([60, 5, 0.3, -100, 1500])
                {
                    Key = "data-b", Name = "Data B",
                    ItemStyle = new() { Color = ChartBrush.Solid(ColorB) },
                    AreaStyle = new() { Color = ChartBrush.Solid(ColorB.WithAlpha(153)), Opacity = 0.7f },
                });
            var secondData = new ChartRadarData()
                .Add(new ChartRadarDataItem([120, 118, 130, 100, 99, 70])
                {
                    Key = "data-c", Name = "Data C",
                    Symbol = "rect", SymbolSize = 12f,
                    ItemStyle = new() { Color = ChartBrush.Solid(ColorC) },
                    LineStyle = new() { Type = ChartLineStyleType.Dashed },
                    Label = new() { Show = true },
                })
                .Add(new ChartRadarDataItem([100, 93, 50, 90, 70, 60])
                {
                    Key = "data-d", Name = "Data D",
                    ItemStyle = new() { Color = ChartBrush.Solid(ColorD) },
                    Label = new() { Show = false },
                    AreaStyle = new()
                    {
                        Color = new ChartRadialGradientBrush(
                            new SKPoint(0.1f, 0.6f), 1f,
                            [ColorD.WithAlpha(26), ColorD.WithAlpha(230)], [0f, 1f]),
                        Opacity = 0.7f,
                    },
                });

            var option = new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Color = { ChartBrush.Solid(ColorA), ChartBrush.Solid(ColorB), ChartBrush.Solid(ColorC), ChartBrush.Solid(ColorD) },
                Animation = new()
                {
                    Enabled = true, Duration = 1000, UpdateDuration = 600,
                    Easing = "cubicOut", UpdateEasing = "cubicInOut",
                },
                Title = new ChartTitleOption
                {
                    Text = "Customized Radar Chart", Left = "12px", Top = "12px",
                    TextStyle = new() { FontSize = 18f },
                },
                Legend = new ChartLegendOption
                {
                    Left = "center", Top = "48px",
                    Data = { "Data A", "Data B", "Data C", "Data D" },
                },
                Tooltip = new ChartTooltipOption { Trigger = ChartTooltipTrigger.Item, Confine = true },
                Radar = new ChartRadarOption
                {
                    Id = "custom-circle",
                    Center = new ChartPolarCenter(ChartLength.Percent(25f), ChartLength.Percent(50f)),
                    Radius = ChartPolarRadius.FromOuter(ChartLength.Pixels(120f)),
                    StartAngle = 90, SplitNumber = 4, Shape = ChartRadarShape.Circle,
                    Indicator =
                    [
                        new() { Name = "Indicator1" },
                        new() { Name = "Indicator2" },
                        new() { Name = "Indicator3" },
                        new() { Name = "Indicator4" },
                        new() { Name = "Indicator5" },
                    ],
                    AxisName = new()
                    {
                        Formatter = "【{value}】", Color = ChartBrush.Solid(SKColor.Parse("#428bd4")),
                    },
                    SplitArea = new()
                    {
                        AreaStyle = new()
                        {
                            Color =
                            {
                                ChartBrush.Solid(SKColor.Parse("#77eadf")),
                                ChartBrush.Solid(SKColor.Parse("#26c3be")),
                                ChartBrush.Solid(SKColor.Parse("#64afe9")),
                                ChartBrush.Solid(SKColor.Parse("#428bd4")),
                            },
                        },
                    },
                    AxisLine = new() { LineStyle = new() { Color = ChartBrush.Solid(new SKColor(211, 253, 250, 204)) } },
                    SplitLine = new() { LineStyle = new() { Color = ChartBrush.Solid(new SKColor(211, 253, 250, 204)) } },
                },
                Series =
                [
                    new ChartRadarSeriesOption
                    {
                        Id = "custom-ab", Name = "Data A / B", RadarIndex = 0,
                        Symbol = "circle", SymbolSize = 5f,
                        DataSource = firstData,
                        Emphasis = new() { LineStyle = new() { Width = 4f } },
                    },
                    new ChartRadarSeriesOption
                    {
                        Id = "custom-cd", Name = "Data C / D", RadarIndex = 1,
                        DataSource = secondData,
                        // 数据项不能携带回调：在系列层格式化，仅 Data C 开启 Label.Show。
                        Label = new()
                        {
                            FontSize = 12f,
                            FormatterCallback = context => context.PrimaryValue.AsString(context.Culture) ?? "",
                        },
                    },
                    CreateCircleShadow(),
                ],
            };

            option.Components.Add(new ChartRadarOption
            {
                Id = "custom-polygon",
                    Center = new ChartPolarCenter(ChartLength.Percent(75f), ChartLength.Percent(50f)),
                Radius = ChartPolarRadius.FromOuter(ChartLength.Pixels(120f)),
                Shape = ChartRadarShape.Polygon,
                Indicator =
                [
                    new() { Name = "Indicator1", Max = 150 },
                    new() { Name = "Indicator2", Max = 150 },
                    new() { Name = "Indicator3", Max = 150 },
                    new() { Name = "Indicator4", Max = 120 },
                    new() { Name = "Indicator5", Max = 108 },
                    new() { Name = "Indicator6", Max = 72 },
                ],
                AxisName = new()
                {
                    // 使用 Rich 为轴名补充背景、圆角和内边距。
                    Formatter = "{badge|{value}}",
                    Rich = new()
                    {
                        ["badge"] = new ChartTextStyle
                        {
                            Color = ChartBrush.Solid(SKColors.White),
                            BackgroundColor = ChartBrush.Solid(SKColor.Parse("#666666")),
                            BorderRadius = 3f,
                            Padding = new ChartInsets(horizontal: 5f, vertical: 3f),
                        },
                    },
                },
            });
            return option;
        }

        /// <summary>SplitArea 尚无阴影字段，用静默圆盘在圆形网格下面绘制同样的底盘阴影。</summary>
        private static ChartPieSeriesOption CreateCircleShadow() => new()
        {
            Id = "custom-circle-shadow", Z = -2, Silent = true,
            AnimationDuration = 0, AnimationDurationUpdate = 0,
            Center = ChartPieCenter.Percent(25f, 50f),
            Radius = new ChartPieRadius(ChartLength.Pixels(0f), ChartLength.Pixels(120f)),
            Label = new() { Show = false },
            LabelLine = new() { Show = false },
            ItemStyle = new()
            {
                Color = ChartBrush.Solid(SKColors.White),
                ShadowColor = ChartBrush.Solid(new SKColor(0, 0, 0, 51)),
                ShadowBlur = 10f,
            },
            DataSource = new ChartPieData().Add(new ChartPieDataItem(1d) { Key = "background-disc" }),
        };
    }
}
