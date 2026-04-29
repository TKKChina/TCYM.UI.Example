using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Table;

namespace TCYM.UI.Example.Page.component.Table.Sections
{
    internal sealed class TableCustomStyleSection
    {
        internal UIView Build()
        {
            var hint = TableSectionHelper.CreateHintLabel("通过 UITable 默认类名 + 自定义类名，实现主题化外观。");

            var columns = new List<TableColumn>
            {
                new() { Title = "姓名", DataIndex = "name", Width = 120 },
                new() { Title = "年龄", DataIndex = "age", Width = 80, Align = ColumnAlign.Center },
                new() { Title = "城市", DataIndex = "city", Width = 120 },
                new()
                {
                    Title = "标签", DataIndex = "tag", Width = 110,
                    Render = (val, _, _) =>
                    {
                        return new UILabel
                        {
                            Text = val?.ToString() ?? "-",
                            ClassName = new List<string> { "table-custom-tag" }
                        };
                    }
                },
                new() { Title = "分数", DataIndex = "score", Width = 90, Align = ColumnAlign.Right },
            };

            var table = new UITable
            {
                Columns = columns,
                DataSource = TableDemoData.GenerateUserData(18).Cast<object>().ToList(),
                Bordered = true,
                Size = TableSize.Middle,
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = 320,
                }
            };
            table.AddClass("table-custom-style-table");


            var card = TableSectionHelper.CreateSectionCard(
                "自定义样式表格",
                "示例通过 class 定制表头、行、单元格和固定区域视觉。",
                hint,
                table);

            card.AddClass("table-custom-style-card");
            return card;
        }
    }
}
