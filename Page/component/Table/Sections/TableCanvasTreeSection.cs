using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Table;

namespace TCYM.UI.Example.Page.component.Table.Sections
{
    internal sealed class TableCanvasTreeSection
    {
        internal UIView Build()
        {
            var treeInfo = TableSectionHelper.CreateHintLabel("树形：默认全部展开");
            var loadingInfo = TableSectionHelper.CreateHintLabel("Loading：关闭");

            var treeColumns = new List<TableColumn>
            {
                new() { Title = "部门名称", DataIndex = "name", Width = 220, IsDefaultFilterDropdown = true },
                new() { Title = "负责人", DataIndex = "leader", Width = 140 },
                new() { Title = "人数", DataIndex = "count", Width = 90, Align = ColumnAlign.Right },
            };

            var treeTable = new UITable
            {
                Columns = treeColumns,
                DataSource = TableDemoData.GenerateTreeData().Cast<object>().ToList(),
                Expandable = new ExpandableConfig
                {
                    DefaultExpandAllRows = true,
                    ChildrenColumnName = "children",
                },
                TreeTableLine = true,
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = 320,
                    BorderWidth = 1,
                    BorderColor = TableSectionHelper.ParseColor("#f0f0f0"),
                }
            };

            var toggleLoadingButton = new UIButton
            {
                Text = "切换 Tree Loading",
                Style = new DefaultUIStyle
                {
                    Width = 150,
                    Height = 32,
                    BorderRadius = 6,
                    BackgroundColor = TableSectionHelper.ParseColor("#1677ff"),
                    Color = TableSectionHelper.ParseColor("#ffffff"),
                    FontSize = 13,
                },
                Events = new()
                {
                    Click = _ =>
                    {
                        treeTable.Loading = !treeTable.Loading;
                        TableSectionHelper.SetLabelText(loadingInfo, $"Loading：{(treeTable.Loading ? "开启" : "关闭")}");
                    }
                }
            };

            return TableSectionHelper.CreateSectionCard(
                "Canvas Table Tree",
                "树形表格演示：默认展开全部节点，支持树形连接线和 Loading 切换。",
                treeInfo,
                loadingInfo,
                toggleLoadingButton,
                treeTable);
        }
    }
}