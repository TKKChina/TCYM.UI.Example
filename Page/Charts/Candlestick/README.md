# Candlestick 示例

每个案例的完整 ChartOption、数据准备与 FormatterCallback 都在对应类中。CandlestickChartDemoPage 仅负责公共排版、操作栏、工厂入口与资源释放。

- ShanghaiIndexCandlestickChartDemo：附件中的 88 条原始上证 OHLC，四条均线和 Mark。
- LargeDataCandlestickChartDemo：固定种子在后台生成 100,000 条分钟数据，K 线 Large/Progressive 和两个 Large 成交量批次。
- IntradayMultipleDaysChartDemo：四个交易日，休市处插入约 1% 宽度的空类目以断开面积折线。
- IntradaySingleDayChartDemo：压缩 90 分钟午休，合并 11:30/13:00 标签，Tooltip 保留真实 UTC 时间。
- BrushCandlestickChartDemo：DJI K 线、均线、逐项涨跌成交量，BrushLink 和双 Grid DataZoom。

## 与附件 ECharts 配置的对应边界

大数据页进入时先显示 loading。BackgroundCandlestickChart 的专用后台线程负责生成 Options、构建 ChartEngine、缩放和 CPU 绘制；UI 线程只接收不可变 SKImage 并绘制。两个线程不共享可变图表内核，也不会把耗时 SetOption 重新投递回 UI。完整首帧就绪后隐藏 loading、启用按钮；离开页面取消任务且不阻塞等待后台清理。生成循环响应取消，已开始的内核构建在返回后检查取消并释放，结果不会再提交到离开的页面。普通案例仍使用原来的 UIChart。

后台生命周期回归可在仓库根目录运行（net10.0 同样支持）：

```powershell
dotnet run --project TCYM.UI.Chart.Tests/TCYM.UI.Chart.Tests.csproj -c Release -f net8.0 -- --background-candlestick
```

当前框架没有原生 axis.breaks/breakArea/breakLabelLayout，两个日内案例使用示例层坐标映射。多日间隔的约 1% 是完整数据范围下的比例，并非随每次缩放重新分配的原生断轴间隔。单日主轴刻度显示真实时间，Slider 隐藏不能自定义回调的分钟序号端点。

当前 toolbox 只有命令协议，示例用 UIButton 调用现有 DataZoom/Brush actions。当前没有 axisPointer.link 顶层配置，双图通过 DataZoom 和日期 Key 联动，Tooltip 的十字线属于当前命中的坐标系。

当前 Bar 视觉管线未消费 VisualMap。大数据案例用两个同位涨/跌 Large 批次实现红绿配色；普通刷选案例逐项设置 ItemStyle.Color，效果与附件 sign 分段映射相同。Brush 案例的 Line 系列保留同日期 Key，通过 BrushLink 接收 K 线选择，不添加 Line 列点击行为。

## 数据来源

上证数据来自用户附件。stock-DJI.json 原样取自 [Apache ECharts 官方示例数据](https://echarts.apache.org/examples/data/asset/data/stock-DJI.json)，共 3,141 条，2004-01-02 至 2016-06-22；随程序集嵌入，运行时不请求网络。所有历史行情仅用于展示组件。
