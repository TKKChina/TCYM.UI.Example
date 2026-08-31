using Page.Charts.Bars;
using Page.Charts.Gauge;
using Page.Charts.Lines;
using Page.Charts.Pie;
using TCYM.UI.Core.Routing;

namespace Page.Charts
{
    /// <summary>创建图表示例页内部使用的独立路由。</summary>
    internal static class ChartRouter
    {

        /// <summary>创建 Line、Bar、Pie 与 Gauge 分类示例使用的路由表。</summary>
        /// <returns>绑定七个 Line、八个 Bar、七个 Pie 和六个 Gauge 页面的独立路由器。</returns>
        internal static UIRouter Create()
        {
            UIRouteRecord[] routes =
            [

                new()
                {
                  Id = "line",
                  Path = "lines",
                  Name = "折线图",
                  KeepAlive = false,
                  Element = UIRoute.LazyLoad(LineChartDemoPage.CreateBasic),
                  Children =
                  [
                      new()
                      {
                          Id = "chart_line_basic",
                          Path = "basic",
                          Name = "基础折线图",
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateBasic),
                      },
                      new()
                      {
                          Id = "chart_line_area",
                          Path = "area",
                          Name = "面积折线图",
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateArea),
                      },
                      new()
                      {
                          Id = "chart_line_multiple",
                          Path = "multiple",
                          Name = "多系列折线图",
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateMultiple),
                      },
                      new()
                      {
                          Id = "chart_line_step",
                          Path = "step",
                          Name = "阶梯折线图",
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateStep),
                      },
                      new()
                      {
                          Id = "chart_line_smooth",
                          Path = "smooth",
                          Name = "平滑折线图",
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateSmooth),
                      },
                      new()
                      {
                          Id = "chart_line_realtime",
                          Path = "realtime",
                          Name = "实时追加折线图",
                          KeepAlive = false,
                          Element = UIRoute.LazyLoad(LineChartDemoPage.CreateRealtime),
                      },
                      new()
                      {
                          Id = "chart_line_large_data",
                          Path = "large-data",
                          Name = "10K 模拟数据折线图",
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
                    KeepAlive = false,
                    Element = UIRoute.LazyLoad(BarChartDemoPage.CreateMixed),
                    Children =
                    [
                        new()
                        {
                            Id = "chart_bar_mixed",
                            Path = "mixed",
                            Name = "折柱混合",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreateMixed),
                        },
                        new()
                        {
                            Id = "chart_bar_rounded_stack",
                            Path = "rounded-stack",
                            Name = "圆角堆积柱状图",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreateRoundedStack),
                        },
                        new()
                        {
                            Id = "chart_bar_gradient",
                            Path = "gradient",
                            Name = "柱状渐变图",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreateGradient),
                        },
                        new()
                        {
                            Id = "chart_bar_multiple_y",
                            Path = "multiple-y",
                            Name = "多 Y 轴图",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreateMultipleYAxis),
                        },
                        new()
                        {
                            Id = "chart_bar_horizontal",
                            Path = "horizontal",
                            Name = "横向柱状图",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreateHorizontal),
                        },
                        new()
                        {
                            Id = "chart_bar_horizontal_stack",
                            Path = "horizontal-stack",
                            Name = "横向堆叠条形图",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreateHorizontalStack),
                        },
                        new()
                        {
                            Id = "chart_bar_polar_stack",
                            Path = "polar-stack",
                            Name = "极坐标堆积柱状图",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(BarChartDemoPage.CreatePolarStack),
                        },
                        new()
                        {
                            Id = "chart_bar_polar_rose",
                            Path = "polar-rose",
                            Name = "极坐标玫瑰图",
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
                    KeepAlive = false,
                    Element = UIRoute.LazyLoad(PieChartDemoPage.CreateNested),
                    Children =
                    [
                        new()
                        {
                            Id = "chart_pie_nested",
                            Path = "nested",
                            Name = "嵌套饼图",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreateNested),
                        },
                        new()
                        {
                            Id = "chart_pie_rounded_donut",
                            Path = "rounded-donut",
                            Name = "圆角环形图",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreateRoundedDonut),
                        },
                        new()
                        {
                            Id = "chart_pie_dynamic_data",
                            Path = "dynamic-data",
                            Name = "饼图动态数据",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreateDynamicData),
                        },
                        new()
                        {
                            Id = "chart_pie_polar_progress",
                            Path = "polar-progress",
                            Name = "柱饼图的完成进度",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreatePolarProgress),
                        },
                        new()
                        {
                            Id = "chart_pie_nightingale_rose",
                            Path = "nightingale-rose",
                            Name = "基础南丁格尔玫瑰图",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreateNightingaleRose),
                        },
                        new()
                        {
                            Id = "chart_pie_half_donut",
                            Path = "half-donut",
                            Name = "半环形图",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(PieChartDemoPage.CreateHalfDonut),
                        },
                        new()
                        {
                            Id = "chart_pie_enhanced_half_donut",
                            Path = "enhanced-half-donut",
                            Name = "半圆增强展示",
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
                    KeepAlive = false,
                    Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreateSpeed),
                    Children =
                    [
                        new()
                        {
                            Id = "chart_gauge_speed",
                            Path = "speed",
                            Name = "速度仪表盘",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreateSpeed),
                        },
                        new()
                        {
                            Id = "chart_gauge_stage_speed",
                            Path = "stage-speed",
                            Name = "阶段速仪表盘",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreateStageSpeed),
                        },
                        new()
                        {
                            Id = "chart_gauge_temperature",
                            Path = "temperature",
                            Name = "气温仪表盘",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreateTemperature),
                        },
                        new()
                        {
                            Id = "chart_gauge_clock",
                            Path = "clock",
                            Name = "时钟仪表盘",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreateClock),
                        },
                        new()
                        {
                            Id = "chart_gauge_custom",
                            Path = "custom",
                            Name = "自定义仪表",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreateCustom),
                        },
                        new()
                        {
                            Id = "chart_gauge_pie",
                            Path = "pie-gauge",
                            Name = "饼仪图",
                            KeepAlive = false,
                            Element = UIRoute.LazyLoad(GaugeChartDemoPage.CreatePieGauge),
                        },
                    ],
                },
            ];

            return new UIRouter(routes);
        }
    }
}
