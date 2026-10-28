using System;
using System.Collections.Generic;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Menu;
using TCYM.UI.Helpers;

namespace Page.Charts
{
    /// <summary>
    /// 图表示例页的左侧导航菜单，继承自 UIScrollView 以支持滚动。
    /// </summary>
    internal class ChartMenu : UIScrollView
    {

        /// <summary>创建图表示例页的分组导航菜单。</summary>
        internal ChartMenu()
        {
            Style = new DefaultUIStyle
            {
                Width = 260,
                Height = "100%",
                OverflowX = "hidden",
            };
            Children = new()
            {
                new UIMenu
                {
                    Mode = MenuMode.Inline,
                    Theme = MenuTheme.Light,
                    ClassName = "charts-navigation-menu",
                    SelectedKeys = ["chart_line_basic"],
                    Style = new UpdateUIStyle
                    {
                        Width = "100%",
                    },
                    IconFontFamily = UIFontManager.Get("IconFontExample"),
                    Items = new List<MenuItem>
                    {
                        MenuItem.Group("group-line", "Line 折线图", new List<MenuItem>
                        {
                            MenuItem.Divider("group-line-divider-top"),
                            new("chart_line_basic", "基础折线图"),
                            new("chart_line_area", "面积折线图"),
                            new("chart_line_multiple", "多系列折线图"),
                            new("chart_line_step", "阶梯折线图"),
                            new("chart_line_smooth", "平滑折线图"),
                            new("chart_line_realtime", "实时追加折线图"),
                            new("chart_line_large_data", "10K 模拟数据折线图"),
                        }, new UIBadge { CountText = "NEW", ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#34f50d") }),
                        MenuItem.Group("group-bar", "Bar 柱状图", new List<MenuItem>
                        {
                            MenuItem.Divider("group-bar-divider-top"),
                            new("chart_bar_mixed", "折柱混合"),
                            new("chart_bar_rounded_stack", "圆角堆积柱状图"),
                            new("chart_bar_gradient", "柱状渐变图"),
                            new("chart_bar_multiple_y", "多 Y 轴图"),
                            new("chart_bar_horizontal", "横向柱状图"),
                            new("chart_bar_horizontal_stack", "横向堆叠条形图"),
                            new("chart_bar_polar_stack", "极坐标堆积柱状图"),
                            new("chart_bar_polar_rose", "极坐标玫瑰图"),
                        }, new UIBadge { CountText = "NEW", ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#34f50d") }),
                        MenuItem.Group("group-pie", "Pie 饼图", new List<MenuItem>
                        {
                            MenuItem.Divider("group-pie-divider-top"),
                            new("chart_pie_nested", "嵌套饼图"),
                            new("chart_pie_rounded_donut", "圆角环形图"),
                            new("chart_pie_dynamic_data", "饼图动态数据"),
                            new("chart_pie_polar_progress", "柱饼图的完成进度"),
                            new("chart_pie_nightingale_rose", "基础南丁格尔玫瑰图"),
                            new("chart_pie_half_donut", "半环形图"),
                            new("chart_pie_enhanced_half_donut", "半圆增强展示"),
                        }, new UIBadge { CountText = "NEW", ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#34f50d") }),
                        MenuItem.Group("group-gauge", "Gauge 仪表盘", new List<MenuItem>
                        {
                            MenuItem.Divider("group-gauge-divider-top"),
                            new("chart_gauge_speed", "速度仪表盘"),
                            new("chart_gauge_stage_speed", "阶段速仪表盘"),
                            new("chart_gauge_temperature", "气温仪表盘"),
                            new("chart_gauge_clock", "时钟仪表盘"),
                            new("chart_gauge_custom", "自定义仪表"),
                            new("chart_gauge_pie", "饼仪图"),
                        }, new UIBadge { CountText = "NEW", ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#34f50d") }),
                        MenuItem.Group("group-scatter", "Scatter 散点图", new List<MenuItem>
                        {
                            MenuItem.Divider("group-scatter-divider-top"),
                            new("chart_scatter_basic", "基础散点图"),
                            new("chart_scatter_anscombe", "安斯库姆四重奏"),
                            new("chart_scatter_clustering", "数据聚合"),
                            new("chart_scatter_effect", "涟漪特效散点图"),
                            new("chart_scatter_special_relation", "特殊关系图"),
                        }, new UIBadge { CountText = "NEW", ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#34f50d") }),
                        MenuItem.Group("group-candlestick", "Candlestick K 线图", new List<MenuItem>
                        {
                            MenuItem.Divider("group-candlestick-divider-top"),
                            new("chart_candlestick_shanghai_index", "上证指数"),
                            new("chart_candlestick_large_data", "大数据量 K 线图"),
                            new("chart_candlestick_intraday_multiple_days", "断轴上的日内走势图"),
                            new("chart_candlestick_intraday_single_day", "断轴上的日内走势图 (II)"),
                            new("chart_candlestick_brush", "K 线图刷选"),
                        }, new UIBadge { CountText = "NEW", ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#34f50d") }),
                        MenuItem.Group("group-radar", "Radar 雷达图", new List<MenuItem>
                        {
                            MenuItem.Divider("group-radar-divider-top"),
                            new("chart_radar_basic", "基础雷达图"),
                            new("chart_radar_customized", "自定义雷达图"),
                            new("chart_radar_gradient", "渐变色雷达图"),
                            new("chart_radar_multidimensional", "多维雷达图"),
                        }, new UIBadge { CountText = "NEW", ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#34f50d") }),
                        MenuItem.Group("group-graph", "Graph 关系图", new List<MenuItem>
                        {
                            MenuItem.Divider("group-graph-divider-top"),
                            new("chart_graph_relation", "关系图"),
                            new("chart_graph_dynamic", "动态增加图节点"),
                            new("chart_graph_complex", "复杂关系图"),
                            new("chart_graph_distributed", "多节点分布拓扑图"),
                        }, new UIBadge { CountText = "NEW", ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#34f50d") }),
                        MenuItem.Group("group-tree", "Tree 树状图", new List<MenuItem>
                        {
                            MenuItem.Divider("group-tree-divider-top"),
                            new("chart_tree_company", "公司架构图"),
                            new("chart_tree_left_to_right", "从左到右树状图"),
                            new("chart_tree_top_to_bottom", "从上到下树状图"),
                            new("chart_tree_right_to_left", "从右到左树状图"),
                        }, new UIBadge { CountText = "NEW", ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#34f50d") }),
                        MenuItem.Group("group-treemap", "TreeMap 矩形树图", new List<MenuItem>
                        {
                            MenuItem.Divider("group-treemap-divider-top"),
                            new("chart_treemap_disk", "磁盘占用"),
                            new("chart_treemap_parent", "显示父层级"),
                            new("chart_treemap_gradient", "映射为渐变色"),
                        }, new UIBadge { CountText = "NEW", ShowOutline = false, BadgeColor = ColorHelper.ParseColor("#34f50d") }),
                        MenuItem.Divider("charts-navigation-divider-bottom"),
                        new("app", "返回上一级") { Icon = "&#xe7ed;", Danger = true }
                    },
                    OnSelect = (keys, item) =>
                    {
                        UIRouterNavigator.NavigateById(keys[0]);
                    },
                },
            };
        }
    }
}
