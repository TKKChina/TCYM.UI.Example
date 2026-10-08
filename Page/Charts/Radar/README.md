# Radar 示例

每个具体类各自创建完整 Options 和原始数据，RadarChartDemoPage 只负责与 Lines 一致的标题、说明、590px 预览卡片、工厂入口和离页释放。

- BasicRadarChartDemo：六维预算/实际支出，多边形网格与两个数据项图例。
- CustomizedRadarChartDemo：两个 Radar；自动范围的圆形网格、负值、富文本轴名、矩形节点、虚线、指标数值回调与径向渐变。
- GradientRadarChartDemo：七维因素、橙色环带和分层描边、黄色面积、指标名称点击事件。
- MultidimensionalRadarChartDemo：圆形虚线网格、2016/2017 年度对比、透明粗描边节点、右侧竖向图例。

## 透明度与动画

四个案例均在自己的 Options 中配置 Animation：初次展开 1000ms（cubicOut），数据更新 600ms（cubicInOut）。轮廓、面积、圆点和标签同步运动；鼠标悬停不会重新播放，隐藏后重新显示的数据会从中心展开。

AreaStyle.Opacity 控制面积不透明度，不会把线条和圆点一起变淡。多维案例的蓝色为 0.7；绿色颜色自身 alpha=0.4，再乘 Opacity=0.7，有效不透明度为 0.28，重叠时仍可看到另一年度和网格。

自定义案例的静默 Pie 阴影底盘禁用动画，装饰网格保持固定，仅数据图形展开。标签显示最终原始值并随顶点运动，不做数值滚动动画。

## 附件配置的对应方式

1. Radar 数据项标签不能携带委托，因此自定义案例的 FormatterCallback 放在系列层，仅 Data C 的 Label.Show 为 true。回调读取当前指标的 PrimaryValue。
2. AxisName 的背景、圆角和 Padding 通过 Rich 样式实现。
3. SplitArea 当前不包含阴影字段，自定义案例用一层同位置、同半径的静默 Pie 圆盘绘制底盘阴影；它不出现在显式图例中。
4. SplitLine.LineStyle.Color 当前只支持一个画刷，渐变案例用五个无数据、无轴名的单圈 Radar 装饰组件分别描边；主 Radar 的指标数据仍只有一组。
5. 渐变案例原图例 a/b 与唯一数据项“因素”不匹配，示例使用“因素”作为有效图例名称。原 reverse 后的六色数组保留，五层网格与原默认 splitNumber 一致。
6. 原始指标名称、最大值和全部向量数值均按附件保留；布局仅为 590px 示例画布留出标题和图例空间。
