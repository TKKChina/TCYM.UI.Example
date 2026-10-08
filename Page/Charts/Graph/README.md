# Graph 案例

沿用 Lines 的标题、说明、预览卡片和离页释放写法。每个具体类自行创建完整 `ChartOption`、原始节点和边，不把配置放进公共基类。

| 案例 | 文件 | 行为 |
| --- | --- | --- |
| 关系图 | RelationGraphDemo.cs | 8 个资源节点、7 条数量连线，100px 圆点、双行名称、后台 force 与固定节点尺寸漫游 |
| 动态增加图节点 | DynamicNodesGraphDemo.cs | 每 200ms 加一点，固定中心，随机连线，后台 force，暂停 / 重置 |
| 复杂关系图 | ComplexRelationGraphDemo.cs | 11 人、10 条关系，图片节点，三类图例，富文本人数和分别处理 node / edge 的 Tooltip |
| 多节点分布拓扑图 | DistributedTopologyGraphDemo.cs | 24 个区域 / 机房、53 个部门，77 条连线，另用一个不可见汇聚节点承载中心端点 |

已按要求移除环形人物关系、设备力导向拓扑两个旧案例及专属素材；当前保留上表四个案例。

## 公共文件只负责什么

- `GraphChartDemoPage`：页面外壳、按钮、Loading 状态、后台图片预加载、UI 线程定时更新和路由退出取消。页面与预览区均不显示滚动条，但保留滚轮和触屏滚动。
- `GraphDemoChart`：仅在逻辑内容区尺寸变化时通知各案例；不改变 Options 归属，也不逐帧重新创建数据。
- `GraphImageAssets`：从嵌入资源提前解码图片，`Resolve` 只创建独占 SKImage，不执行磁盘/网络 I/O 或解码；先释放 Chart，再释放缓存。
- `GraphEdgeEffects`：通过 `ChartGraphSeriesViewFactory` 的场景装饰回调，在原生 Graph 已呈现的边路径上画箭头和流动 pin。原生节点、拖动、缩放、Tooltip 和 force 生命周期仍由框架处理；装饰不拦截点击、不逐帧 SetOption。
- 图片放在 `Assets` 并作为 EmbeddedResource 编译，运行时不联网。

## 与附件的对应和明确差异

1. 画布与 Lines 一致：宽度 `100%`、高度 `590px`，没有固定宽度或最小宽度限制。Graph 的 None 坐标是逻辑像素，复杂关系按相对位置换算，多节点拓扑按当前画布重新计算椭圆；窗口变化时自动适配。
2. 新关系图保留对象属性、关联对象、所属类别、对象四个分类；原片段没有“所属类别”的节点，不额外虚构数据。Name 用于身份与 Tooltip，Value 保存显示名称，节点内通过 FormatterCallback 将“资源数量”“主题订阅”显示为双行文字。
3. 目前 Graph 无普通几何补间或 force 逐步布局动画，不提交无效的动画配置。force 是本库有界后台求解，结果不保证与 ECharts 逐点一致。关系图按放大展示需求使用 100px 节点、180px 圆心间距，节点文字 22px、数量文字 14px，让初始图形更舒展。
4. 原生 Graph 没有 edgeSymbol / lines.effect；箭头与流动 pin 为示例层装饰，不声称原生 lines 系列已实现。标记取实际边路径，拖动和缩放后仍能跟随，隐藏和离页取消帧任务。
5. 动态加点以固定随机种子运行，300 个节点后暂停增长，可重置；没有越过本库 force 容量边界无限增长。已有节点按稳定 Id 保留已呈现坐标，显式改 X/Y 时仍使用新值。
6. 复杂关系原片段的 category 与图片颜色不一致，本例把红色图标归到“确诊病例”、黄色归到“无症状感染”、青色归到“未感染”，确保三类图例都有对应节点。Num、Type、日期与关系不改；这些都是附件的演示数据。
7. `FormatterCallback` 返回纯文本，富文本人数徽标使用逐节点 `Formatter` 模板与系列 `Rich`；业务对象使用只读 record + `IChartImmutableDataValue`，兼容快照管线。
8. 新关系图配置 `NodeScaleRatio = 0`：漫游时节点和节点标签保持大小，位置与连线仍正常缩放。未设置该字段的旧案例仍保留完全缩放行为。

## 来源

- [关系图](https://www.makeapie.cn/echarts_content/xFo04oy52F.html)
- [复杂关系图](https://www.makeapie.cn/echarts_content/x1Ntt0X7HZ.html)
- [多节点分布拓扑图](https://www.makeapie.cn/echarts_content/xQ7enajOSz.html)

第三方 PNG 沿用附件指定来源，云图由附件内嵌 Base64 解码，没有重绘或改变原图文件；资源文件名对应表见 Assets/SOURCES.md。

## 验证

`TCYM.UI.Chart.Tests --graph-live-demo` 验证 force 追加 / 重排位置延续、显式坐标覆盖和场景装饰隐藏 / Clear / Dispose。
`--m8-graph` 保留完整 Graph 数据、布局、事件、force、DPI/Zoom 与 Exact Golden 回归。
`--graph-node-scale` 验证 NodeScaleRatio 的 clone/merge/null、范围检查、缩放、节点标签与 Tooltip 边界。
