using Page.Charts.Bars;
using Page.Charts.Candlestick;
using Page.Charts.Gauge;
using Page.Charts.Graph;
using Page.Charts.Lines;
using Page.Charts.Pie;
using Page.Charts.Radar;
using Page.Charts.Scatter;
using Page.Charts.Tree;
using TCYM.UI.Core.Routing;

namespace Page.Charts
{
    /// <summary>创建图表示例页内部使用的独立路由。</summary>
    internal static class ChartRouter
    {

        /// <summary>创建 Line、Bar、Pie、Gauge、Scatter、Candlestick、Radar、Graph 与 Tree 分类示例路由。</summary>
        /// <returns>按图表类型分组、按需加载各独立案例的路由器。</returns>
        internal static UIRouter Create()
        {
            UIRouteRecord[] routes =
            [

                new()
                {
                  Id = "line",
                  Path = "lines",
                  Name = "折线图",
                  ShowLoading = true,
                  KeepAlive = false,
                  Element = UIRoute.LazyLoad(LineChartDemoPage.CreateBasic),
                  Children =
                  [
                      new()
                      {
                          Id = "chart_line_basic",
                          Path = "basic",
                          Name = "基础折线图",
                          ShowLoading = true,
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateBasic),
                      },
                      new()
                      {
                          Id = "chart_line_area",
                          Path = "area",
                          Name = "面积折线图",
                          ShowLoading = true,
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateArea),
                      },
                      new()
                      {
                          Id = "chart_line_multiple",
                          Path = "multiple",
                          Name = "多系列折线图",
                          ShowLoading = true,
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateMultiple),
                      },
                      new()
                      {
                          Id = "chart_line_step",
                          Path = "step",
                          Name = "阶梯折线图",
                          ShowLoading = true,
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateStep),
                      },
                      new()
                      {
                          Id = "chart_line_smooth",
                          Path = "smooth",
                          Name = "平滑折线图",
                          ShowLoading = true,
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateSmooth),
                      },
                      new()
                      {
                          Id = "chart_line_realtime",
                          Path = "realtime",
                          Name = "实时追加折线图",
                          ShowLoading = true,
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateRealtime),
                      },
                      new()
                      {
                          Id = "chart_line_large_data",
                          Path = "large-data",
                          Name = "10K 模拟数据折线图",
                          ShowLoading = true,
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateLargeData),
                      },
                  ]
                },
                new()
                {
                    Id = "bar",
                    Path = "bars",
                    Name = "Bar 柱状图",
                    ShowLoading = true,
                    KeepAlive = false,
                    Element = UIRoute.LazyLoad(BarChartDemoPage.CreateMixed),
                    Children =
                    [
                        new()
                        {
                            Id = "chart_bar_mixed",
                            Path = "mixed",
                            Name = "折柱混合",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreateMixed),
                        },
                        new()
                        {
                            Id = "chart_bar_rounded_stack",
                            Path = "rounded-stack",
                            Name = "圆角堆积柱状图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreateRoundedStack),
                        },
                        new()
                        {
                            Id = "chart_bar_gradient",
                            Path = "gradient",
                            Name = "柱状渐变图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreateGradient),
                        },
                        new()
                        {
                            Id = "chart_bar_multiple_y",
                            Path = "multiple-y",
                            Name = "多 Y 轴图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreateMultipleYAxis),
                        },
                        new()
                        {
                            Id = "chart_bar_horizontal",
                            Path = "horizontal",
                            Name = "横向柱状图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreateHorizontal),
                        },
                        new()
                        {
                            Id = "chart_bar_horizontal_stack",
                            Path = "horizontal-stack",
                            Name = "横向堆叠条形图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreateHorizontalStack),
                        },
                        new()
                        {
                            Id = "chart_bar_polar_stack",
                            Path = "polar-stack",
                            Name = "极坐标堆积柱状图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreatePolarStack),
                        },
                        new()
                        {
                            Id = "chart_bar_polar_rose",
                            Path = "polar-rose",
                            Name = "极坐标玫瑰图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreatePolarRose),
                        },
                    ],
                },
                new()
                {
                    Id = "pie",
                    Path = "pies",
                    Name = "Pie 饼图",
                    ShowLoading = true,
                    KeepAlive = false,
                    Element = UIRoute.LazyLoad(PieChartDemoPage.CreateNested),
                    Children =
                    [
                        new()
                        {
                            Id = "chart_pie_nested",
                            Path = "nested",
                            Name = "嵌套饼图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreateNested),
                        },
                        new()
                        {
                            Id = "chart_pie_rounded_donut",
                            Path = "rounded-donut",
                            Name = "圆角环形图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreateRoundedDonut),
                        },
                        new()
                        {
                            Id = "chart_pie_dynamic_data",
                            Path = "dynamic-data",
                            Name = "饼图动态数据",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreateDynamicData),
                        },
                        new()
                        {
                            Id = "chart_pie_polar_progress",
                            Path = "polar-progress",
                            Name = "柱饼图的完成进度",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreatePolarProgress),
                        },
                        new()
                        {
                            Id = "chart_pie_nightingale_rose",
                            Path = "nightingale-rose",
                            Name = "基础南丁格尔玫瑰图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreateNightingaleRose),
                        },
                        new()
                        {
                            Id = "chart_pie_half_donut",
                            Path = "half-donut",
                            Name = "半环形图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreateHalfDonut),
                        },
                        new()
                        {
                            Id = "chart_pie_enhanced_half_donut",
                            Path = "enhanced-half-donut",
                            Name = "半圆增强展示",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreateEnhancedHalfDonut),
                        },
                    ],
                },
                new()
                {
                    Id = "gauge",
                    Path = "gauges",
                    Name = "Gauge 仪表盘",
                    ShowLoading = true,
                    KeepAlive = false,
                    Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreateSpeed),
                    Children =
                    [
                        new()
                        {
                            Id = "chart_gauge_speed",
                            Path = "speed",
                            Name = "速度仪表盘",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreateSpeed),
                        },
                        new()
                        {
                            Id = "chart_gauge_stage_speed",
                            Path = "stage-speed",
                            Name = "阶段速仪表盘",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreateStageSpeed),
                        },
                        new()
                        {
                            Id = "chart_gauge_temperature",
                            Path = "temperature",
                            Name = "气温仪表盘",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreateTemperature),
                        },
                        new()
                        {
                            Id = "chart_gauge_clock",
                            Path = "clock",
                            Name = "时钟仪表盘",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreateClock),
                        },
                        new()
                        {
                            Id = "chart_gauge_custom",
                            Path = "custom",
                            Name = "自定义仪表",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreateCustom),
                        },
                        new()
                        {
                            Id = "chart_gauge_pie",
                            Path = "pie-gauge",
                            Name = "饼仪图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreatePieGauge),
                        },
                    ],
                },
                new()
                {
                    Id = "scatter",
                    Path = "scatters",
                    Name = "Scatter 散点图",
                    ShowLoading = true,
                    KeepAlive = false,
                    Element = UIRoute.LazyLoad(ScatterChartDemoPage.CreateBasic),
                    Children =
                    [
                        new()
                        {
                            Id = "chart_scatter_basic",
                            Path = "basic",
                            Name = "基础散点图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(ScatterChartDemoPage.CreateBasic),
                        },
                        new()
                        {
                            Id = "chart_scatter_anscombe",
                            Path = "anscombe",
                            Name = "安斯库姆四重奏",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(ScatterChartDemoPage.CreateAnscombeQuartet),
                        },
                        new()
                        {
                            Id = "chart_scatter_clustering",
                            Path = "clustering",
                            Name = "数据聚合",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(ScatterChartDemoPage.CreateClustering),
                        },
                        new()
                        {
                            Id = "chart_scatter_effect",
                            Path = "effect",
                            Name = "涟漪特效散点图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(ScatterChartDemoPage.CreateEffect),
                        },
                        new()
                        {
                            Id = "chart_scatter_special_relation",
                            Path = "special-relation",
                            Name = "特殊关系图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(ScatterChartDemoPage.CreateSpecialRelation),
                        },
                    ],
                },
                new()
                {
                    Id = "candlestick",
                    Path = "candlesticks",
                    Name = "Candlestick K 线图",
                    ShowLoading = true,
                    KeepAlive = false,
                    Element = UIRoute.LazyLoad(CandlestickChartDemoPage.CreateShanghaiIndex),
                    Children =
                    [
                        new()
                        {
                            Id = "chart_candlestick_shanghai_index",
                            Path = "shanghai-index",
                            Name = "上证指数",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(CandlestickChartDemoPage.CreateShanghaiIndex),
                        },
                        new()
                        {
                            Id = "chart_candlestick_large_data",
                            Path = "large-data",
                            Name = "大数据量 K 线图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(CandlestickChartDemoPage.CreateLargeData),
                        },
                        new()
                        {
                            Id = "chart_candlestick_intraday_multiple_days",
                            Path = "intraday-multiple-days",
                            Name = "断轴上的日内走势图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(CandlestickChartDemoPage.CreateIntradayMultipleDays),
                        },
                        new()
                        {
                            Id = "chart_candlestick_intraday_single_day",
                            Path = "intraday-single-day",
                            Name = "断轴上的日内走势图 (II)",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(CandlestickChartDemoPage.CreateIntradaySingleDay),
                        },
                        new()
                        {
                            Id = "chart_candlestick_brush",
                            Path = "brush",
                            Name = "K 线图刷选",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(CandlestickChartDemoPage.CreateBrush),
                        },
                    ],
                },
                new()
                {
                    Id = "radar",
                    Path = "radars",
                    Name = "Radar 雷达图",
                    ShowLoading = true,
                    KeepAlive = false,
                    Element = UIRoute.LazyLoad(RadarChartDemoPage.CreateBasic),
                    Children =
                    [
                        new()
                        {
                            Id = "chart_radar_basic",
                            Path = "basic",
                            Name = "基础雷达图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(RadarChartDemoPage.CreateBasic),
                        },
                        new()
                        {
                            Id = "chart_radar_customized",
                            Path = "customized",
                            Name = "自定义雷达图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(RadarChartDemoPage.CreateCustomized),
                        },
                        new()
                        {
                            Id = "chart_radar_gradient",
                            Path = "gradient",
                            Name = "渐变色雷达图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(RadarChartDemoPage.CreateGradient),
                        },
                        new()
                        {
                            Id = "chart_radar_multidimensional",
                            Path = "multidimensional",
                            Name = "多维雷达图",
                            ShowLoading = true,
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(RadarChartDemoPage.CreateMultidimensional),
                        },
                    ],
                },
            ];

            routes = [.. routes, new UIRouteRecord
            {
                Id = "graph", Path = "graphs", Name = "Graph 关系图", KeepAlive = false, ShowLoading = true,
                Element = UIRoute.LazyLoad(GraphChartDemoPage.CreateRelation),
                Children =
                [
                    new()
                    {
                        Id = "chart_graph_relation", Path = "relation", Name = "关系图",
                        KeepAlive = false, ShowLoading = true, Element = UIRoute.LazyLoad(GraphChartDemoPage.CreateRelation),
                    },
                    new()
                    {
                        Id = "chart_graph_dynamic", Path = "dynamic", Name = "动态增加图节点",
                        KeepAlive = false, ShowLoading = true, Element = UIRoute.LazyLoad(GraphChartDemoPage.CreateDynamic),
                    },
                    new()
                    {
                        Id = "chart_graph_complex", Path = "complex", Name = "复杂关系图",
                        KeepAlive = false, ShowLoading = true, Element = UIRoute.LazyLoad(GraphChartDemoPage.CreateComplex),
                    },
                    new()
                    {
                        Id = "chart_graph_distributed", Path = "distributed", Name = "多节点分布拓扑图",
                        KeepAlive = false, ShowLoading = true, Element = UIRoute.LazyLoad(GraphChartDemoPage.CreateDistributed),
                    },
                ],
            }];
            routes = [.. routes, new UIRouteRecord
            {
                Id = "tree", Path = "trees", Name = "Tree 树状图", KeepAlive = false, ShowLoading = true,
                Element = UIRoute.LazyLoad(TreeChartDemoPage.CreateCompany),
                Children =
                [
                    new()
                    {
                        Id = "chart_tree_company", Path = "company", Name = "公司架构图",
                        KeepAlive = false, ShowLoading = true, Element = UIRoute.LazyLoad(TreeChartDemoPage.CreateCompany),
                    },
                    new()
                    {
                        Id = "chart_tree_left_to_right", Path = "left-to-right", Name = "从左到右树状图",
                        KeepAlive = false, ShowLoading = true, Element = UIRoute.LazyLoad(TreeChartDemoPage.CreateLeftToRight),
                    },
                    new()
                    {
                        Id = "chart_tree_top_to_bottom", Path = "top-to-bottom", Name = "从上到下树状图",
                        KeepAlive = false, ShowLoading = true, Element = UIRoute.LazyLoad(TreeChartDemoPage.CreateTopToBottom),
                    },
                    new()
                    {
                        Id = "chart_tree_right_to_left", Path = "right-to-left", Name = "从右到左树状图",
                        KeepAlive = false, ShowLoading = true, Element = UIRoute.LazyLoad(TreeChartDemoPage.CreateRightToLeft),
                    },
                ],
            }];
            return new UIRouter(routes);
        }
    }
}
