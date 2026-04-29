using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Example.Page.component.Table.Sections;

namespace TCYM.UI.Example.Page.component.Table
{
  internal class UITableDemo : UIScrollView
  {
    private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Table.style.css";

    internal UITableDemo()
    {
      UISystem.LoadStyleFile(DemoCssPath);

      // 首屏先渲染前两个 Table 示例，其余 section 由 UIScrollView 在后续帧按批次真实创建。
      InitialChildLoadCount = 2;
      DeferredChildBatchSize = 1;
      DeferredChildLoadIntervalSeconds = 0.03f;

      ClassName = new List<string> { "table-demo-view" };

      Children = new()
      {
        new UILabel
        {
          Text = "Table 高性能表格演示",
            ClassName = new List<string> { "table-demo-title" }
        },
        new UILabel
        {
          Text = "高性能 Canvas Table 示例，基于画布渲染、可见区虚拟绘制和宿主化交互内容，兼顾大数据量场景下的滚动性能与交互完整性。",
            ClassName = new List<string> { "table-demo-title-sub" },
        },
        new UILabel
        {
          Text = "示例覆盖固定表头、固定列、分页、汇总、独立展开行、树形、Loading，以及本地/远程筛选与默认搜索等高性能表格核心场景。",
            ClassName = new List<string> { "table-demo-desc" },
        }
      };

      AddDeferredChildren(
        static () => new TableCanvasSection().Build(),
        static () => new TableCanvasExpandableSection().Build(),
        static () => new TableCanvasTreeSection().Build(),
        static () => new TableCanvasFilterLocalSection().Build(),
        static () => new TableCanvasFilterRemoteSection().Build(),
        static () => new TableCanvasDefaultFilterLocalSection().Build(),
        static () => new TableCanvasDefaultFilterRemoteSection().Build(),
        static () => new TableCanvasLargeDataSection().Build()
      );
    }
  }
}
