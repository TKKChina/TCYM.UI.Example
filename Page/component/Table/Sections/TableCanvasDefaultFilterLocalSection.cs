using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Table;

namespace TCYM.UI.Example.Page.component.Table.Sections
{
  internal sealed class TableCanvasDefaultFilterLocalSection
  {
    internal UIView Build()
    {
      var filterInfo = TableSectionHelper.CreateHintLabel("筛选：无");
      var columns = new List<TableColumn>
      {
        new() { Title = "姓名", DataIndex = "name", Width = 120, IsDefaultFilterDropdown = true },
        new() { Title = "城市", DataIndex = "city", Width = 120, IsDefaultFilterDropdown = true },
        new() { Title = "标签", DataIndex = "tag", Width = 120, IsDefaultFilterDropdown = true },
        new() { Title = "分数", DataIndex = "score", Width = 80, Align = ColumnAlign.Right },
      };

      var table = new UITable
      {
        Columns = columns,
        DataSource = TableDemoData.GenerateUserData(30).Cast<object>().ToList(),
        OnTableChange = (_, filters, sorter) =>
        {
          var parts = new List<string>();
          foreach (var kv in filters)
          {
            if (kv.Value.Count > 0)
              parts.Add($"{kv.Key}=[{string.Join(",", kv.Value)}]");
          }
          var filterText = parts.Count > 0 ? string.Join(", ", parts) : "无";
          TableSectionHelper.SetLabelText(filterInfo, $"筛选：{filterText}");
        },
        Style = new DefaultUIStyle
        {
          Width = "100%",
          Height = 300,
        }
      };

      return TableSectionHelper.CreateSectionCard("默认搜索筛选（本地）", "设置 IsDefaultFilterDropdown=true 启用默认搜索框筛选。", filterInfo, table);
    }
  }
}
