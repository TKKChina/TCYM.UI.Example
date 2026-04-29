using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Table;

namespace TCYM.UI.Example.Page.component.Table.Sections
{
  internal sealed class TableCanvasFilterRemoteSection
  {
    internal UIView Build()
    {
      var filterInfo = TableSectionHelper.CreateHintLabel("远程筛选参数：无");
      var allData = TableDemoData.GenerateUserData(50);
      UITable? tableRef = null;

      var columns = new List<TableColumn>
      {
          new() { Title = "姓名", DataIndex = "name", Width = 120 },
          new()
          {
            Title = "城市", DataIndex = "city", Width = 120,
            IsFilterRemote = true,
            Filters = new List<FilterItem>
            {
                new() { Text = "北京", Value = "北京" },
                new() { Text = "上海", Value = "上海" },
                new() { Text = "广州", Value = "广州" },
                new() { Text = "深圳", Value = "深圳" },
            },
            FilterSearch = true,
          },
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
            var vals = kv.Value.Select(v => v?.ToString()).ToHashSet();
            filtered = filtered.Where(row => row.TryGetValue(key, out var v) && vals.Contains(v?.ToString())).ToList();
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

      return TableSectionHelper.CreateSectionCard("远程筛选", "设置 IsFilterRemote=true 后筛选不在本地执行，通过 OnTableChange 拿到筛选参数请求后端。", filterInfo, table);
    }
  }
}
