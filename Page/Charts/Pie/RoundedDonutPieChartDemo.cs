using SkiaSharp;
using TCYM.UI.Chart.Components.Legend;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;
using TCYM.UI.Chart.Text;

namespace Page.Charts.Pie
{
    /// <summary>展示带扇区间隔、圆角和中心强调标签的环形图。</summary>
    internal sealed class RoundedDonutPieChartDemo : PieChartDemoPage
    {
        /// <summary>创建圆角环形图页面。</summary>
        internal RoundedDonutPieChartDemo()
            : base(
                "圆角环形图",
                "通过 PadAngle 和 BorderRadius 为每个环形扇区留出圆角间隔。",
                "普通状态隐藏标签；鼠标移入某个扇区时，Emphasis.Label 会在圆心显示该项名称，并使用 40px 粗体突出当前数据。",
                CreateOption())
        {
        }

        /// <summary>创建包含 Tooltip、顶部图例和圆角环形系列的完整配置。</summary>
        /// <returns>圆角环形图使用的完整配置。</returns>
        private static ChartOption CreateOption()
        {
            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 800,
                    UpdateDuration = 500,
                },
                Tooltip = new ChartTooltipOption
                {
                    Id = "rounded-donut-tooltip",
                    Trigger = ChartTooltipTrigger.Item,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                },
                Legend = new ChartLegendOption
                {
                    Id = "rounded-donut-legend",
                    Show = true,
                    Top = "5%",
                    Left = ChartLength.FromKeyword("center"),
                    Orient = ChartLegendOrient.Horizontal,
                    ItemGap = 18f,
                },
                Series =
                [
                    new ChartPieSeriesOption
                    {
                        Id = "rounded-donut",
                        Name = "收入",
                        Radius = new ChartPieRadius(
                            ChartLength.Percent(40f),
                            ChartLength.Percent(70f)),
                        AvoidLabelOverlap = false,
                        LabelLayout = new()
                        {
                            HideOverlap = false,
                        },
                        PadAngle = 5f,
                        ItemStyle = new()
                        {
                            BorderRadius = 10f,
                        },
                        Label = new()
                        {
                            Show = false,
                            Position = ChartPieLabelPosition.Center,
                        },
                        Emphasis = new()
                        {
                            Label = new()
                            {
                                Show = true,
                                Position = ChartPieLabelPosition.Center,
                                Formatter = "{center|{b}}",
                                Rich = new()
                                {
                                    ["center"] = new ChartTextStyle
                                    {
                                        FontSize = 40f,
                                        FontWeight = 700,
                                    },
                                },
                            },
                        },
                        LabelLine = new()
                        {
                            Show = false,
                        },
                        DataSource = CreateData(),
                    },
                ],
            };
        }

        /// <summary>创建五项收入来源数据。</summary>
        /// <returns>带稳定 Key 的圆角环形图数据。</returns>
        private static ChartPieData CreateData()
        {
            var values = new (string Key, string Name, double Value)[]
            {
                ("search", "搜索引擎", 1048d),
                ("service", "服务", 735d),
                ("email", "邮件", 580d),
                ("union", "联盟广告", 484d),
                ("video", "视频广告", 300d),
            };

            var data = new ChartPieData();
            foreach (var (key, name, value) in values)
            {
                data.Add(new ChartPieDataItem(value, name)
                {
                    Key = $"rounded-donut-{key}",
                });
            }

            return data;
        }
    }
}
