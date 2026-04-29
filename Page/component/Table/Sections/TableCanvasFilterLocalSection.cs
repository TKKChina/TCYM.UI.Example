using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Table;

namespace TCYM.UI.Example.Page.component.Table.Sections
{
  internal sealed class TableCanvasFilterLocalSection
  {
    internal UIView Build()
    {
      var filterInfo = TableSectionHelper.CreateHintLabel("筛选：无");
      var columns = new List<TableColumn>
      {
        new() { Title = "姓名", DataIndex = "name", Width = 120 },
        new()
        {
            Title = "城市", DataIndex = "city", Width = 120,
            Filters = new List<FilterItem>
            {
              new() { Text = "北京", Value = "北京" },
              new() { Text = "上海", Value = "上海" },
              new() { Text = "广州", Value = "广州" },
              new() { Text = "深圳", Value = "深圳" },
            },
            FilterSearch = true,
            OnFilter = (filterValue, record) =>
            {
              if (record is IDictionary<string, object> dict && dict.TryGetValue("city", out var city))
                  return city?.ToString() == filterValue?.ToString();
              return false;
            }
        },
        new()
        {
            Title = "标签", DataIndex = "tag", Width = 120,
            Filters = new List<FilterItem>
            {
              new() { Text = "开发", Value = "开发" },
              new() { Text = "设计", Value = "设计" },
              new() { Text = "测试", Value = "测试" },
            },
            OnFilter = (filterValue, record) =>
            {
              if (record is IDictionary<string, object> dict && dict.TryGetValue("tag", out var tag))
                  return tag?.ToString() == filterValue?.ToString();
              return false;
            }
        },
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

      return TableSectionHelper.CreateSectionCard("本地筛选", "设置列的 Filters 和 OnFilter 属性启用本地筛选。", filterInfo, table);
    }
  }
}
