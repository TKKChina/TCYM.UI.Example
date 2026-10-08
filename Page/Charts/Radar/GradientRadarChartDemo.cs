using SkiaSharp;
using TCYM.UI.Chart.Components.Common;
using TCYM.UI.Chart.Interaction;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Elements.Message;

namespace Page.Charts.Radar
{
    /// <summary>深色底盘、橙色渐变环带与黄色七维因素面积。</summary>
    internal sealed class GradientRadarChartDemo : RadarChartDemoPage
    {
        private const int SplitCount = 5;
        private const float RadiusPercent = 80f;
        private static readonly byte[] RingAlphas = [255, 204, 153, 102, 51, 26];

        internal GradientRadarChartDemo()
            : base(
                "渐变色雷达图",
                "深蓝背景上的七维因素分析与由内向外渐淡的橙色环带。",
                "黄色轮廓和半透明面积展示各因素的数值；点击图例可隐藏/显示数据，点击外圈指标名称可查看名称事件。",
                CreateOption())
        {
            Chart.ChartEvent += (_, args) =>
            {
                if (args.Type == ChartEventType.Click
                    && args.ComponentType == "radar"
                    && args.ComponentId == "gradient-radar")
                    UIMessage.Info($"指标：{args.Name}", duration: 2f, key: "gradient-radar-indicator");
            };
        }

        private static ChartOption CreateOption()
        {
            var indicator = CreateIndicators();
            var option = new ChartOption
            {
                Background = ChartBrush.Solid(SKColor.Parse("#000928")),
                Animation = new()
                {
                    Enabled = true, Duration = 1000, UpdateDuration = 600,
                    Easing = "cubicOut", UpdateEasing = "cubicInOut",
                },
                Tooltip = new ChartTooltipOption { Trigger = ChartTooltipTrigger.Item, Confine = true },
                Legend = new ChartLegendOption
                {
                    Show = true, Icon = "circle",
                    Left = "center", Bottom = "30px",
                    ItemWidth = 14f, ItemHeight = 14f, ItemGap = 21f,
                    // 原配置 a/b 没有对应数据；这里使用真实数据项名称，确保图例可操作。
                    Data = { "因素" },
                    TextStyle = new() { FontSize = 12f, Color = ChartBrush.Solid(SKColor.Parse("#8c8c8c")) },
                },
                Radar = new ChartRadarOption
                {
                    Id = "gradient-radar",
                    Center = new ChartPolarCenter(ChartLength.Percent(50f), ChartLength.Percent(50f)),
                    Radius = ChartPolarRadius.FromOuter(ChartLength.Percent(RadiusPercent)),
                    Shape = ChartRadarShape.Polygon,
                    SplitNumber = SplitCount,
                    Indicator = indicator,
                    TriggerEvent = true,
                    AxisNameGap = 2f,
                    AxisName = new()
                    {
                        Formatter = "{name|{value}}",
                        Rich = new()
                        {
                            ["name"] = new ChartTextStyle
                            {
                                Color = ChartBrush.Solid(SKColors.White),
                                FontSize = 20f,
                                Padding = new ChartInsets(horizontal: 5f, vertical: 3f),
                            },
                        },
                    },
                    AxisLine = new() { Show = false },
                    SplitLine = new() { Show = false },
                    SplitArea = new() { Show = true },
                },
                Series = new ChartRadarSeriesOption
                {
                    Id = "gradient-factors", Name = "一级权重分析", RadarId = "gradient-radar",
                    Symbol = "none", SymbolSize = 0f,
                    LineStyle = new() { Color = ChartBrush.Solid(new SKColor(252, 211, 3)), Width = 1f },
                    ItemStyle = new() { Color = ChartBrush.Solid(new SKColor(252, 211, 3)) },
                    AreaStyle = new() { Color = ChartBrush.Solid(new SKColor(252, 211, 3, 77)), Opacity = 0.7f },
                    DataSource = new ChartRadarData().Add(new ChartRadarDataItem(
                        [4300, 10000, 28000, 35000, 50000, 19000, 21000])
                    {
                        Key = "factors", Name = "因素",
                    }),
                },
            };

            // SplitArea 支持颜色数组，按附件 reverse() 后的顺序从内向外填充。
            foreach (var alpha in RingAlphas)
                option.Radar!.SplitArea.AreaStyle.Color.Add(ChartBrush.Solid(new SKColor(222, 134, 85, alpha)));

            // SplitLine 目前只接收单个画刷：每一层使用一个静默、单圈 Radar 装饰组件。
            // 这些组件没有数据系列、图例和轴名，不会改变主 Radar 的指标或 Tooltip。
            for (var ring = 1; ring <= SplitCount; ring++)
            {
                option.Components.Add(new ChartRadarOption
                {
                    Id = $"gradient-ring-{ring}",
                    Silent = true,
                    Center = new ChartPolarCenter(ChartLength.Percent(50f), ChartLength.Percent(50f)),
                    Radius = ChartPolarRadius.FromOuter(ChartLength.Percent(RadiusPercent * ring / SplitCount)),
                    Shape = ChartRadarShape.Polygon,
                    SplitNumber = 1,
                    Indicator = CreateIndicators(),
                    AxisName = new() { Show = false },
                    AxisLabel = new() { Show = false },
                    AxisLine = new() { Show = false },
                    SplitArea = new() { Show = false },
                    SplitLine = new()
                    {
                        Show = true,
                        LineStyle = new()
                        {
                            Width = 2f,
                            Color = ChartBrush.Solid(new SKColor(224, 134, 82, RingAlphas[ring - 1])),
                        },
                    },
                });
            }
            return option;
        }

        private static ChartRadarIndicatorOption[] CreateIndicators() =>
        [
            new() { Name = "资金周转", Max = 6500 },
            new() { Name = "其他", Max = 16000 },
            new() { Name = "金融理财", Max = 30000 },
            new() { Name = "投资", Max = 38000 },
            new() { Name = "个人消费", Max = 52000 },
            new() { Name = "固定资产", Max = 25000 },
            new() { Name = "生产经营", Max = 25000 },
        ];
    }
}
