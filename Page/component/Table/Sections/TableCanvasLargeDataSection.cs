using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Table;

namespace TCYM.UI.Example.Page.component.Table.Sections
{
    internal sealed class TableCanvasLargeDataSection
    {
        internal UIView Build()
        {
            var info = TableSectionHelper.CreateHintLabel("数据量：10000 条（不分页）");
            var columns = new List<TableColumn>
            {
                new() { Title = "编号", DataIndex = "id", Width = 80 },
                new() { Title = "姓名", DataIndex = "name", Width = 120 },
                new() { Title = "年龄", DataIndex = "age", Width = 100, Align = ColumnAlign.Center },
                new() { Title = "城市", DataIndex = "city", Width = 120 },
                new() { Title = "标签", DataIndex = "tag", Width = 120 },
                new() { Title = "分数", DataIndex = "score", Width = 100, Align = ColumnAlign.Right },
            };

            var table = new UITable
            {
                Columns = columns,
                DataSource = TableDemoData.GenerateUserData(10000).Cast<object>().ToList(),
                Bordered = true,
                ShowHeader = true,
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = 400,
                }
            };

            return TableSectionHelper.CreateSectionCard("性能测试：10000 条（不分页）", "超过 100 条会自动启用虚拟列表优化渲染性能。", info, table);
        }
    }
}
