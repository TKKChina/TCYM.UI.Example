using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Message;
using TCYM.UI.Elements.Table;

namespace TCYM.UI.Example.Page.component.Table.Sections
{
    internal sealed class TableCanvasExpandableSection
    {
        internal UIView Build()
        {
            UIStyleManager.RegisterClassStyle("canvas-v2-row-warning", new DefaultUIStyle
            {
                BackgroundColor = TableSectionHelper.ParseColor("#fff7e6"),
                BorderBottomColor = TableSectionHelper.ParseColor("#ffd591"),
                Hover = new DefaultUIStyle
                {
                    BackgroundColor = TableSectionHelper.ParseColor("#ffe7ba"),
                }
            });
            UIStyleManager.RegisterClassStyle("canvas-v2-row-muted", new DefaultUIStyle
            {
                BackgroundColor = TableSectionHelper.ParseColor("#f6ffed"),
                BorderBottomColor = TableSectionHelper.ParseColor("#b7eb8f"),
                Hover = new DefaultUIStyle
                {
                    BackgroundColor = TableSectionHelper.ParseColor("#d9f7be"),
                }
            });

            var expandableInfo = TableSectionHelper.CreateHintLabel("展开行：非树形独立表格，左侧单独展开列；第 2、5 行禁用展开，并应用 RowClassName 样式");
            var expandableEventInfo = TableSectionHelper.CreateHintLabel("双击：无");

            var expandableColumns = new List<TableColumn>
            {
                new() { Title = "编号", DataIndex = "id", Width = 80, Fixed = ColumnFixed.Left },
                new() { Title = "姓名", DataIndex = "name", Width = 140, Fixed = ColumnFixed.Left },
                new() { Title = "所属部门", DataIndex = "department", Width = 220, Ellipsis = true },
                new() { Title = "职级", DataIndex = "level", Width = 120, Align = ColumnAlign.Center },
                new() { Title = "邮箱", DataIndex = "email", Width = 240, Ellipsis = true },
            };

            var expandableTable = new UITable
            {
                Columns = expandableColumns,
                DataSource = TableDemoData.GenerateEllipsisData(6).Cast<object>().ToList(),
                RowClassName = (record, index) =>
                {
                    if (record is not IDictionary<string, object> dict)
                    {
                        return string.Empty;
                    }

                    var id = dict.TryGetValue("id", out var idValue) ? idValue?.ToString() ?? string.Empty : string.Empty;
                    return id switch
                    {
                        "2" => "canvas-v2-row-warning",
                        "4" => "canvas-v2-row-muted",
                        _ => string.Empty,
                    };
                },
                OnRowDoubleClick = (_, index) =>
                {
                    TableSectionHelper.SetLabelText(expandableEventInfo, $"双击：第 {index + 1} 行");
                },
                Expandable = new ExpandableConfig
                {
                    RowExpandable = record =>
                    {
                        if (record is not IDictionary<string, object> dict)
                        {
                            return false;
                        }

                        var id = dict.TryGetValue("id", out var idValue) ? idValue?.ToString() ?? string.Empty : string.Empty;
                        return id != "2" && id != "5";
                    },
                    ExpandedRowRender = (record, index, indent, expanded) =>
                    {
                        if (record is not IDictionary<string, object> dict)
                        {
                            return null;
                        }

                        var name = dict.TryGetValue("name", out var nameValue) ? nameValue?.ToString() ?? string.Empty : string.Empty;
                        var department = dict.TryGetValue("department", out var departmentValue) ? departmentValue?.ToString() ?? string.Empty : string.Empty;
                        var email = dict.TryGetValue("email", out var emailValue) ? emailValue?.ToString() ?? string.Empty : string.Empty;
                        var experience = dict.TryGetValue("experience", out var experienceValue) ? experienceValue?.ToString() ?? string.Empty : string.Empty;
                        var address = dict.TryGetValue("address", out var addressValue) ? addressValue?.ToString() ?? string.Empty : string.Empty;

                        var container = new UIView
                        {
                            Style = new DefaultUIStyle
                            {
                                Width = "100%",
                                Height = 112,
                                Display = "flex",
                                FlexDirection = "row",
                                AlignItems = "center",
                                JustifyContent = "space-between",
                                PaddingLeft = 16,
                                PaddingRight = 16,
                                BackgroundColor = TableSectionHelper.ParseColor("#fafcff"),
                                BorderBottom = true,
                                BorderBottomWidth = 1,
                                BorderBottomColor = TableSectionHelper.ParseColor("#e6f4ff"),
                            }
                        };

                        var textBlock = new UIView
                        {
                            Style = new DefaultUIStyle
                            {
                                Display = "flex",
                                FlexDirection = "column",
                                Gap = 4,
                                Height = "100%",
                                JustifyContent = "center",
                            }
                        };
                        textBlock.AddChild(new UILabel
                        {
                            Text = $"{name} / {department}",
                            Style = new DefaultUIStyle
                            {
                                FontSize = 14,
                                FontWeight = 700,
                                Color = TableSectionHelper.ParseColor("#1f1f1f"),
                            }
                        });
                        textBlock.AddChild(new UILabel
                        {
                            Text = $"第 {index + 1} 行的独立展开内容。邮箱 {email}。",
                            Style = new DefaultUIStyle
                            {
                                FontSize = 12,
                                Color = TableSectionHelper.ParseColor("#666666"),
                            }
                        });
                        textBlock.AddChild(new UILabel
                        {
                            Text = $"工作经历：{experience}",
                            Style = new DefaultUIStyle
                            {
                                FontSize = 12,
                                Color = TableSectionHelper.ParseColor("#666666"),
                            }
                        });

                        var actionColumn = new UIView
                        {
                            Style = new DefaultUIStyle
                            {
                                Display = "flex",
                                FlexDirection = "column",
                                AlignItems = "flex-end",
                                JustifyContent = "center",
                                Gap = 8,
                            }
                        };
                        actionColumn.AddChild(new UILabel
                        {
                            Text = address,
                            Style = new DefaultUIStyle
                            {
                                FontSize = 12,
                                Color = TableSectionHelper.ParseColor("#8c8c8c"),
                            }
                        });
                        actionColumn.AddChild(new UIButton
                        {
                            Text = expanded ? "查看详情" : "展开详情",
                            Style = new DefaultUIStyle
                            {
                                Height = 30,
                                PaddingLeft = 12,
                                PaddingRight = 12,
                                FontSize = 12,
                                BorderRadius = 15,
                                BackgroundColor = TableSectionHelper.ParseColor("#1677ff"),
                                Color = TableSectionHelper.ParseColor("#ffffff"),
                            },
                            Events = new()
                            {
                                Click = _ => UIMessage.Info($"ExpandedRowRender: {name}")
                            }
                        });

                        container.AddChild(textBlock);
                        container.AddChild(actionColumn);
                        return container;
                    },
                    DefaultExpandedRowKeys = new List<string> { "1" },
                },
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = 360,
                    BorderWidth = 1,
                    BorderColor = TableSectionHelper.ParseColor("#f0f0f0"),
                }
            };

            return TableSectionHelper.CreateSectionCard(
                "Canvas Table Expandable",
                "独立展开行演示：支持按行控制是否可展开、展开区域自定义渲染、行样式区分和双击事件。",
                expandableInfo,
                expandableEventInfo,
                expandableTable);
        }
    }
}