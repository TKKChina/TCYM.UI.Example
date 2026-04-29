using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Message;
using TCYM.UI.Elements.Table;

namespace TCYM.UI.Example.Page.component.Table.Sections
{
    internal sealed class TableCanvasSection
    {
        internal UIView Build()
        {
            var info = TableSectionHelper.CreateHintLabel("当前已支持固定表头、固定列、分页、汇总行、虚拟绘制、本地排序、本地筛选、默认搜索筛选、行选择、Toolbar、列设置、Loading、基础树形展开与连接线，以及可见区内的自定义 Render / TitleRender 宿主。 ");
            var filterInfo = TableSectionHelper.CreateHintLabel("筛选：无");
            var selectionInfo = TableSectionHelper.CreateHintLabel("选中：1, 3");
            var data = TableDemoData.GenerateEllipsisData(100);

            ApplySalaryData(data);

            var columns = new List<TableColumn>
            {
                new()
                {
                    Title = "编号",
                    DataIndex = "id",
                    Width = 80,
                    Fixed = ColumnFixed.Left,
                    Sorter = (left, right) =>
                    {
                        int li = int.TryParse((left as IDictionary<string, object>)?["id"]?.ToString(), out var lv) ? lv : 0;
                        int ri = int.TryParse((right as IDictionary<string, object>)?["id"]?.ToString(), out var rv) ? rv : 0;
                        return li.CompareTo(ri);
                    },
                    Summary = rows => new UILabel { Text = $"总计 {rows.Count} 条" }
                },
                new()
                {
                    Title = "姓名",
                    DataIndex = "name",
                    Align = ColumnAlign.Center,
                    Width = 120,
                    Fixed = ColumnFixed.Left,
                    IsDefaultFilterDropdown = true,
                    Summary = _ => new UILabel { Text = "固定左列" }
                },
                new() { Title = "个人简介", DataIndex = "bio", Width = 260, Ellipsis = true },
                new() { Title = "工作经历", DataIndex = "experience", Width = 320, Ellipsis = true },
                new() { Title = "联系地址", DataIndex = "address", Width = 420, Ellipsis = true },
                new()
                {
                    Title = "所属部门全称",
                    DataIndex = "department",
                    Width = 220,
                    Ellipsis = true,
                    TitleRender = _ => new UILabel
                    {
                        Text = "自定义部门",
                        Style = new DefaultUIStyle
                        {
                            FontSize = 14,
                            Color = TableSectionHelper.ParseColor("#1677ff"),
                            FontWeight = 700,
                        }
                    },
                    Sorter = (left, right) => string.Compare(
                        (left as IDictionary<string, object>)?["department"]?.ToString(),
                        (right as IDictionary<string, object>)?["department"]?.ToString(),
                        StringComparison.OrdinalIgnoreCase),
                    Summary = _ => new UILabel { Text = "部门汇总" }
                },
                new()
                {
                    Title = "职级",
                    DataIndex = "level",
                    Width = 160,
                    Filters = new List<FilterItem>
                    {
                        new() { Text = "P6 工程师", Value = "P6 工程师" },
                        new() { Text = "P7 高级工程师", Value = "P7 高级工程师" },
                        new() { Text = "P8 技术专家", Value = "P8 技术专家" },
                    },
                    FilterSearch = true,
                    OnFilter = (filterValue, record) =>
                    {
                        if (record is IDictionary<string, object> dict && dict.TryGetValue("level", out var level))
                        {
                            return level?.ToString() == filterValue?.ToString();
                        }

                        return false;
                    },
                    Render = (value, _, _) =>
                    {
                        var tag = value?.ToString() ?? string.Empty;
                        var color = tag switch
                        {
                            "P6 工程师" => "#1677ff",
                            "P7 高级工程师" => "#fa8c16",
                            "P8 技术专家" => "#722ed1",
                            _ => "#606266",
                        };

                        return new UITag
                        {
                            Text = tag,
                            Variant = TagVariant.Filled,
                            Style = new DefaultUIStyle
                            {
                                Color = TableSectionHelper.ParseColor(color),
                                PaddingLeft = 4,
                                PaddingRight = 4,
                                PaddingTop = 2,
                                PaddingBottom = 2,
                                FontSize = 12,
                                BorderRadius = 5,
                            }
                        };
                    },
                    Sorter = (left, right) => string.Compare(
                        (left as IDictionary<string, object>)?["level"]?.ToString(),
                        (right as IDictionary<string, object>)?["level"]?.ToString(),
                        StringComparison.OrdinalIgnoreCase),
                    Summary = rows => new UILabel
                    {
                        Text = $"当前数据 {rows.Select(row => (row as IDictionary<string, object>)?["level"]?.ToString()).Where(level => !string.IsNullOrWhiteSpace(level)).Distinct().Count()} 个职级"
                    }
                },
                new()
                {
                    Title = "月薪",
                    DataIndex = "salary",
                    Width = 140,
                    Render = (value, _, _) => new UILabel
                    {
                        Text = $"￥{ParseSalary(value):N0}",
                        Style = new DefaultUIStyle
                        {
                            FontSize = 13,
                            FontWeight = 700,
                            Color = GetSalaryLevelColor(ParseSalary(value)),
                        }
                    },
                    Sorter = (left, right) => GetSalary(left).CompareTo(GetSalary(right)),
                    Summary = rows => new UILabel
                    {
                        Text = $"￥{rows.Sum(GetSalary):N0}",
                        Style = new DefaultUIStyle
                        {
                            FontSize = 14,
                            Width= "100%",
                            Height  = "100%",
                            FontWeight = 700,
                            Display = "flex",
                            AlignItems = "center",
                            //JustifyContent = "start",
                            Color = GetSalaryLevelColor(rows.Sum(GetSalary)),
                        }
                    }
                },
                new()
                {
                    Title = "邮箱",
                    DataIndex = "email",
                    Width = 240,
                    Ellipsis = true,
                    Fixed = ColumnFixed.Right,
                    Summary = _ => new UILabel { Text = "固定右列" }
                },
                new()
                {
                   Title = "操作",
                   Width = 220,
                   Fixed = ColumnFixed.Right,
                   Render = (_, record, _) => BuildActionBar(record)
                },
            };

            var table = new UITable
            {
                Columns = columns,
                DataSource = data.Cast<object>().ToList(),
                Total = data.Count,
                RowSelection = new RowSelectionConfig
                {
                    Type = SelectionType.Checkbox,
                    ColumnWidth = 52,
                    DefaultSelectedRowKeys = new List<string> { "1", "3" },
                    OnChange = (keys, _) =>
                    {
                        TableSectionHelper.SetLabelText(selectionInfo, $"选中：{(keys.Count > 0 ? string.Join(", ", keys) : "无")}");
                    }
                },
                // Scroll = new ScrollConfig
                // {
                //     X = 1300,
                //     Y = 300,
                // },
                SearchHighlightColor = TableSectionHelper.ParseColor("#d46b08"),
                OnTableChange = (_, filters, _) =>
                {
                    var parts = new List<string>();
                    foreach (var entry in filters)
                    {
                        if (entry.Value.Count > 0)
                        {
                            parts.Add($"{entry.Key}=[{string.Join(",", entry.Value)}]");
                        }
                    }

                    TableSectionHelper.SetLabelText(filterInfo, $"筛选：{(parts.Count > 0 ? string.Join(", ", parts) : "无")}");
                },
                Toolbar = new TableToolbar
                {
                    Show = true,
                    Add = new ToolbarActionConfig
                    {
                        OnClick = () => UIMessage.Info("CanvasV2 工具栏新增")
                    },
                    Edit = new ToolbarActionConfig
                    {
                        OnClick = () => UIMessage.Info("CanvasV2 工具栏编辑")
                    },
                    Delete = new ToolbarActionConfig
                    {
                        OnClick = () => UIMessage.Error("CanvasV2 工具栏删除")
                    },
                    Refresh = new ToolbarActionConfig
                    {
                        OnClick = () => UIMessage.Success("CanvasV2 工具栏刷新")
                    },
                    Import = new ToolbarActionConfig
                    {
                        OnClick = () => UIMessage.Info("CanvasV2 工具栏导入")
                    },
                    Export = new ToolbarActionConfig
                    {
                        OnClick = () => UIMessage.Info("CanvasV2 工具栏导出")
                    },
                    DownloadTemplate = new ToolbarActionConfig
                    {
                        OnClick = () => UIMessage.Info("CanvasV2 工具栏下载模板")
                    },
                    IsShowCustomDefault = true,
                    CustomToolbarRight = new UIButton
                    {
                        Text = "自定义按钮",
                        Style = new DefaultUIStyle
                        {
                            Height = 30,
                            PaddingLeft = 12,
                            PaddingRight = 12,
                            FontSize = 13,
                            BorderRadius = 4,
                            BackgroundColor = TableSectionHelper.ParseColor("#1677FF"),
                            Color = TableSectionHelper.ParseColor("#ffffff"),
                        },
                        Events = new()
                        {
                            Click = (e) => UIMessage.Info("CanvasV2 自定义按钮"),
                        }
                    },
                    SetColumn = new SetColumnConfig
                    {
                        FixedEnabled = true,
                        HideColumn = true,
                        Draggable = true,
                    }
                },
                Pagination = new TablePaginationConfig
                {
                    PageSize = 20,
                    ShowSizeChanger = true,
                    ShowQuickJumper = true,
                    PageSizeOptions = new() { 20, 100, 200,500 },
                },
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = 450,
                    BorderWidth = 1,
                    BorderColor = TableSectionHelper.ParseColor("#f0f0f0"),
                }
            };

            return TableSectionHelper.CreateSectionCard(
                "Canvas Table 基础能力",
                "Canvas 绘制版表格的主能力演示：固定表头、固定列、分页、本地排序、本地筛选、默认搜索筛选、行选择、Toolbar、列设置，以及可见区内的自定义 Render / TitleRender 宿主。",
                info,
                filterInfo,
                selectionInfo,
                table);
        }

        private static void ApplySalaryData(List<Dictionary<string, object>> rows)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var level = GetStringValue(row, "level");
                var salary = level switch
                {
                    "P6 工程师" => 22000 + i * 1200,
                    "P7 高级工程师" => 32000 + i * 1500,
                    "P8 技术专家" => 46000 + i * 1800,
                    _ => 18000 + i * 1000,
                };

                row["salary"] = salary;
            }
        }

        private static UIView BuildActionBar(object? record)
        {
            var name = GetStringValue(record, "name");
            var levelRank = GetLevelRank(record);

            var actions = new UIView
            {
                Style = new DefaultUIStyle
                {
                    Display = "flex",
                    AlignItems = "center",
                    Gap = 8,
                }
            };

            actions.AddChild(CreateActionButton("查看", "#1677ff", () => UIMessage.Info($"查看：{name}")));

            if (levelRank < 7)
            {
                actions.AddChild(CreateActionButton("编辑", "#fa8c16", () => UIMessage.Info($"编辑：{name}")));
                actions.AddChild(CreateActionButton("删除", "#ff4d4f", () => UIMessage.Error($"删除：{name}")));
            }

            return actions;
        }

        private static UIButton CreateActionButton(string text, string backgroundColor, Action onClick)
        {
            return new UIButton
            {
                Text = text,
                Style = new DefaultUIStyle
                {
                    Height = 28,
                    PaddingLeft = 10,
                    PaddingRight = 10,
                    FontSize = 12,
                    BorderRadius = 14,
                    BackgroundColor = TableSectionHelper.ParseColor(backgroundColor),
                    Color = TableSectionHelper.ParseColor("#ffffff"),
                },
                Events = new()
                {
                    Click = _ => onClick()
                }
            };
        }

        private static int GetSalary(object? record)
        {
            return ParseSalary(record is IDictionary<string, object> row && row.TryGetValue("salary", out var salary) ? salary : null);
        }

        private static int ParseSalary(object? value)
        {
            return int.TryParse(value?.ToString(), out var salary) ? salary : 0;
        }

        private static SkiaSharp.SKColor GetSalaryLevelColor(int salary)
        {
            return salary switch
            {
                >= 45000 => TableSectionHelper.ParseColor("#cf1322"),
                >= 30000 => TableSectionHelper.ParseColor("#d46b08"),
                _ => TableSectionHelper.ParseColor("#389e0d"),
            };
        }

        private static int GetLevelRank(object? record)
        {
            var level = GetStringValue(record, "level");
            if (string.IsNullOrWhiteSpace(level) || level.Length < 2 || level[0] != 'P')
            {
                return 0;
            }

            return int.TryParse(level[1..].Split(' ')[0], out var rank) ? rank : 0;
        }

        private static string GetStringValue(object? record, string key)
        {
            return record is IDictionary<string, object> row ? GetStringValue(row, key) : string.Empty;
        }

        private static string GetStringValue(IDictionary<string, object> row, string key)
        {
            return row.TryGetValue(key, out var value) ? value?.ToString() ?? string.Empty : string.Empty;
        }
    }
}