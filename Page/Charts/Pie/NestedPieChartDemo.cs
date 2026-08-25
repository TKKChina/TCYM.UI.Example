using SkiaSharp;
using TCYM.UI.Chart.Components.Legend;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Pie
{
    /// <summary>展示由内层来源分类和外层明细来源组成的嵌套饼图。</summary>
    internal sealed class NestedPieChartDemo : PieChartDemoPage
    {
        /// <summary>创建嵌套饼图页面。</summary>
        internal NestedPieChartDemo()
            : base(
                "嵌套饼图",
                "使用两个 Pie 系列同时展示访问来源的大类和明细组成。",
                "内圈半径为 0%～30%，并开启单选模式；外圈半径为 40%～55%。点击内圈扇区可以切换选中项，移动鼠标可查看名称、数值和占比。",
                CreateOption())
        {
        }

        /// <summary>创建包含 Tooltip、纵向图例以及内外两个 Pie 系列的完整配置。</summary>
        /// <returns>嵌套饼图使用的完整配置。</returns>
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
                    Id = "nested-pie-tooltip",
                    Trigger = ChartTooltipTrigger.Item,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                    // Canvas Tooltip 使用换行符代替 ECharts HTML 示例中的 <br/>。
                    Formatter = "{a}\r\n{b}: {c} ({d}%)",
                },
                Legend = new ChartLegendOption
                {
                    Id = "nested-pie-legend",
                    Show = true,
                    Left = ChartLength.FromKeyword("left"),
                    Top = "18px",
                    Orient = ChartLegendOrient.Vertical,
                    ItemGap = 9f,
                    Data =
                    {
                        "直达",
                        "营销广告",
                        "搜索引擎",
                        "邮件营销",
                        "联盟广告",
                        "视频广告",
                        "百度",
                        "谷歌",
                        "必应",
                        "其他",
                    },
                },
                Series =
                [
                    new ChartPieSeriesOption
                    {
                        Id = "nested-pie-inner",
                        Name = "访问来源",
                        SelectedMode = ChartSeriesSelectedMode.Single,
                        Radius = new ChartPieRadius(
                            ChartLength.Pixels(0f),
                            ChartLength.Percent(30f)),
                        Label = new()
                        {
                            Show = true,
                            Position = ChartPieLabelPosition.Inner,
                            Formatter = "{b}",
                            FontSize = 12f,
                            Color = ChartBrush.Solid(SKColors.White),
                        },
                        LabelLine = new()
                        {
                            Show = false,
                        },
                        DataSource = CreateInnerData(),
                    },
                    new ChartPieSeriesOption
                    {
                        Id = "nested-pie-outer",
                        Name = "访问来源",
                        Radius = new ChartPieRadius(
                            ChartLength.Percent(40f),
                            ChartLength.Percent(55f)),
                        AvoidLabelOverlap = true,
                        Label = new()
                        {
                            Show = true,
                            Position = ChartPieLabelPosition.Outer,
                            Formatter = "{b}",
                            FontSize = 12f,
                            Color = ChartBrush.Solid(new SKColor(67, 74, 89)),
                        },
                        LabelLine = new()
                        {
                            Show = true,
                            Length = 14f,
                            Length2 = 10f,
                        },
                        DataSource = CreateOuterData(),
                    },
                ],
            };
        }

        /// <summary>创建内圈的大类数据，并将“直达”设为初始选中项。</summary>
        /// <returns>包含三个访问来源大类的 Pie 数据。</returns>
        private static ChartPieData CreateInnerData()
        {
            return new ChartPieData()
                .Add(new ChartPieDataItem(335d, "直达")
                {
                    Key = "nested-inner-direct",
                    Selected = true,
                })
                .Add(new ChartPieDataItem(679d, "营销广告")
                {
                    Key = "nested-inner-marketing",
                })
                .Add(new ChartPieDataItem(1548d, "搜索引擎")
                {
                    Key = "nested-inner-search",
                });
        }

        /// <summary>创建外圈的八个明细来源数据项。</summary>
        /// <returns>带稳定 Key 的访问来源明细数据。</returns>
        private static ChartPieData CreateOuterData()
        {
            var values = new (string Key, string Name, double Value)[]
            {
                ("direct", "直达", 335d),
                ("email", "邮件营销", 310d),
                ("union", "联盟广告", 234d),
                ("video", "视频广告", 135d),
                ("baidu", "百度", 1048d),
                ("google", "谷歌", 251d),
                ("bing", "必应", 147d),
                ("other", "其他", 102d),
            };

            var data = new ChartPieData();
            foreach (var (key, name, value) in values)
            {
                data.Add(new ChartPieDataItem(value, name)
                {
                    Key = $"nested-outer-{key}",
                });
            }

            return data;
        }
    }
}
