using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Table;

namespace TCYM.UI.Example.Page.component.Table.Sections
{
  internal sealed class TableCanvasDefaultFilterRemoteSection
  {
    internal UIView Build()
    {
      var filterInfo = TableSectionHelper.CreateHintLabel("远程筛选参数：无");
      var allData = TableDemoData.GenerateUserData(50);
      UITable? tableRef = null;

      var columns = new List<TableColumn>
      {
        new() { Title = "姓名", DataIndex = "name", Width = 120, IsDefaultFilterDropdown = true, IsFilterRemote = true },
        new() { Title = "城市", DataIndex = "city", Width = 120, IsDefaultFilterDropdown = true, IsFilterRemote = true },
        new() { Title = "标签", DataIndex = "tag", Width = 120 },
        new() { Title = "分数", DataIndex = "score", Width = 80, Align = ColumnAlign.Right },
      };

      var table = new UITable
      {
        Columns = columns,
        DataSource = allData.Cast<object>().ToList(),
        OnTableChange = (_, filters, _) =>
        {
          var filtered = new List<Dictionary<string, object>>(allData);
          var parts = new List<string>();
          foreach (var kv in filters)
          {
            if (kv.Value.Count <= 0) continue;
            parts.Add($"{kv.Key}=[{string.Join(",", kv.Value)}]");
            var key = kv.Key;
            var keyword = kv.Value[0]?.ToString() ?? string.Empty;
            if (keyword.Length <= 0) continue;
            filtered = filtered.Where(row => row.TryGetValue(key, out var v) && (v?.ToString()?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true)).ToList();
          }

          var paramText = parts.Count > 0 ? string.Join(", ", parts) : "无";
          TableSectionHelper.SetLabelText(filterInfo, $"远程筛选参数：{paramText}（请求中...）");

          var result = filtered;
          Task.Run(async () =>
          {
            if (tableRef == null) return;
            tableRef.Loading = true;
            await Task.Delay(500);
            tableRef.Loading = false;
            tableRef.DataSource = result.Cast<object>().ToList();
            TableSectionHelper.SetLabelText(filterInfo, $"远程筛选参数：{paramText}（结果 {result.Count} 条）");
          });
        },
        Style = new DefaultUIStyle
        {
          Width = "100%",
          Height = 300,
        }
      };
      tableRef = table;

      return TableSectionHelper.CreateSectionCard("默认搜索筛选（远程）", "IsDefaultFilterDropdown + IsFilterRemote=true，搜索关键字通过 OnTableChange 传递到后端。", filterInfo, table);
    }
  }
}
