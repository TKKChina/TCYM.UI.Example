using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Tree;
using TCYM.UI.Elements.TreeSelect;

namespace TCYM.UI.Example.Page.component.TreeSelect
{
    internal class UITreeSelectDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.TreeSelect.style.css";

        internal UITreeSelectDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = "tree-select-demo-view";
            Children = new()
            {
                new UILabel
                {
                    Text = "TreeSelect 树选择器",
                    ClassName = "tree-select-demo-title"
                },
                new UILabel
                {
                    Text = "在下拉面板中从层级数据里选择一个或多个节点。",
                    ClassName = "tree-select-demo-subtitle"
                },
                new UILabel
                {
                    Text = "参考 Ant Design TreeSelect。弹层主体直接使用 UITree，支持单选、多选、搜索、Checkbox 父子联动、严格勾选、回填策略、清除、禁用、尺寸、变体和事件回调。",
                    ClassName = "tree-select-demo-desc"
                },
                BuildBasicSection(),
                BuildSearchAndMultipleSection(),
                BuildCheckableSection(),
                BuildAppearanceSection(),
                BuildZoomSection(),
                BuildEventSection()
            };
        }

        private static UIElement BuildBasicSection()
        {
            return BuildCard(
                "基本使用",
                "点击选择框打开 UITree 面板。可以设置默认值、允许清除，或禁用整个选择器。",
                new UITreeSelect
                {
                    TreeData = CreateDepartmentData(),
                    Placeholder = "请选择部门",
                    TreeDefaultExpandAll = true,
                    AllowClear = true,
                    Style = new UpdateUIStyle { Width = 250 }
                },
                new UITreeSelect
                {
                    TreeData = CreateDepartmentData(),
                    DefaultValue = "platform-api",
                    AllowClear = true,
                    Style = new UpdateUIStyle { Width = 250 }
                },
                new UITreeSelect
                {
                    TreeData = CreateDepartmentData(),
                    DefaultValue = "sales-east",
                    Disabled = true,
                    Style = new UpdateUIStyle { Width = 250 }
                });
        }

        private static UIElement BuildSearchAndMultipleSection()
        {
            return BuildCard(
                "搜索与多选",
                "Multiple 自动启用搜索。搜索会保留匹配节点的祖先路径，MaxTagCount 控制触发器中显示的标签数量。",
                new UITreeSelect
                {
                    TreeData = CreateDepartmentData(),
                    ShowSearch = true,
                    SearchPlaceholder = "搜索部门或岗位",
                    Placeholder = "搜索并选择",
                    TreeDefaultExpandAll = true,
                    Style = new UpdateUIStyle { Width = 280 }
                },
                new UITreeSelect
                {
                    Multiple = true,
                    TreeData = CreateDepartmentData(),
                    DefaultValues = new[] { "platform-web", "sales-east", "finance" },
                    AllowClear = true,
                    MaxTagCount = 2,
                    MaxTagTextLength = 8,
                    Placeholder = "请选择多个节点",
                    Style = new UpdateUIStyle { Width = 320 }
                });
        }

        private static UIElement BuildCheckableSection()
        {
            return BuildCard(
                "Checkbox 与回填策略",
                "TreeCheckable 使用 UITree 的 Checkbox。左侧父子联动并用 ShowParent 回填；右侧 TreeCheckStrictly 让父子节点完全独立。",
                new UIView
                {
                    ClassName = "tree-select-example-group",
                    Children = new()
                    {
                        new UILabel
                        {
                            Text = "父子联动 / ShowParent",
                            ClassName = "tree-select-example-caption"
                        },
                        new UITreeSelect
                        {
                            TreeData = CreateDepartmentData(),
                            TreeCheckable = true,
                            ShowCheckedStrategy = TreeSelectShowCheckedStrategy.ShowParent,
                            DefaultValues = new[] { "platform-web", "platform-api" },
                            AllowClear = true,
                            TreeDefaultExpandAll = true,
                            Style = new UpdateUIStyle { Width = 300 }
                        }
                    }
                },
                new UIView
                {
                    ClassName = "tree-select-example-group",
                    Children = new()
                    {
                        new UILabel
                        {
                            Text = "严格勾选 / ShowAll",
                            ClassName = "tree-select-example-caption"
                        },
                        new UITreeSelect
                        {
                            TreeData = CreateDepartmentData(),
                            TreeCheckable = true,
                            TreeCheckStrictly = true,
                            ShowCheckedStrategy = TreeSelectShowCheckedStrategy.ShowAll,
                            DefaultValues = new[] { "platform", "sales-east" },
                            AllowClear = true,
                            TreeDefaultExpandAll = true,
                            Style = new UpdateUIStyle { Width = 300 }
                        }
                    }
                });
        }

        private static UIElement BuildAppearanceSection()
        {
            return BuildCard(
                "尺寸、变体与状态",
                "支持 Small、Medium、Large，以及 Outlined、Filled、Borderless、Underlined、Error 和 Warning。",
                new UITreeSelect
                {
                    TreeData = CreateDepartmentData(),
                    Size = TreeSelectSize.Small,
                    Placeholder = "Small",
                    Style = new UpdateUIStyle { Width = 150 }
                },
                new UITreeSelect
                {
                    TreeData = CreateDepartmentData(),
                    Variant = TreeSelectVariant.Filled,
                    Placeholder = "Filled",
                    Style = new UpdateUIStyle { Width = 160 }
                },
                new UITreeSelect
                {
                    TreeData = CreateDepartmentData(),
                    Variant = TreeSelectVariant.Underlined,
                    Placeholder = "Underlined",
                    Style = new UpdateUIStyle { Width = 170 }
                },
                new UITreeSelect
                {
                    TreeData = CreateDepartmentData(),
                    Size = TreeSelectSize.Large,
                    Status = TreeSelectStatus.Error,
                    Placeholder = "Error",
                    Style = new UpdateUIStyle { Width = 170 }
                },
                new UITreeSelect
                {
                    TreeData = CreateDepartmentData(),
                    Status = TreeSelectStatus.Warning,
                    Placeholder = "Warning",
                    Style = new UpdateUIStyle { Width = 170 }
                });
        }

        private static UIElement BuildEventSection()
        {
            var status = new UILabel
            {
                Text = "尚未选择",
                ClassName = "tree-select-event-status"
            };
            var picker = new UITreeSelect
            {
                Multiple = true,
                TreeData = CreateDepartmentData(),
                AllowClear = true,
                TreeDefaultExpandAll = true,
                Placeholder = "选择后查看事件回显",
                Style = new UpdateUIStyle { Width = 340 },
                OnChange = (values, nodes) =>
                {
                    string labels = nodes.Length == 0
                        ? "空"
                        : string.Join("、", nodes.Select(node => node.Label));
                    status.Text = $"OnChange：{string.Join(", ", values)} / {labels}";
                },
                OnSearch = value => status.Text = string.IsNullOrEmpty(value)
                    ? "OnSearch：已清空"
                    : $"OnSearch：{value}",
                OnClear = () => status.Text = "OnClear：已清空"
            };

            return BuildCard(
                "事件回显",
                "OnChange 返回全部 Value 与原始 TreeNode；OnSearch 仅响应用户输入，OnClear 响应清除操作。",
                new UIView
                {
                    ClassName = "tree-select-event-group",
                    Children = new() { picker, status }
                });
        }

        private static UIElement BuildZoomSection()
        {
            float[] zoomLevels = [0.75f, 1f, 1.25f, 1.5f];
            int zoomIndex = 1;
            var picker = new UITreeSelect
            {
                TreeData = CreateDepartmentData(),
                DefaultValue = "platform-api",
                AllowClear = true,
                TreeDefaultExpandAll = true,
                Style = new UpdateUIStyle
                {
                    Width = 300,
                    Zoom = 1f
                }
            };
            var state = new UILabel
            {
                Text = "当前 Zoom：100%",
                ClassName = "tree-select-zoom-state"
            };
            var zoomButton = new UIButton
            {
                Text = "切换 Zoom",
                ClassName = "tree-select-zoom-button"
            };
            zoomButton.OnClick += _ =>
            {
                zoomIndex = (zoomIndex + 1) % zoomLevels.Length;
                float zoom = zoomLevels[zoomIndex];
                picker.Style = new UpdateUIStyle
                {
                    Width = 300,
                    Zoom = zoom
                };
                state.Text = $"当前 Zoom：{zoom * 100:0}%";
            };

            return BuildCard(
                "高 DPI、DPR 与视觉 Zoom",
                "组件会在窗口跨屏后重同步内部样式；DPR-only 不改变逻辑布局。切换 Zoom 后打开下拉面板，可直接检查触发器、清除按钮、Popup、内边距和命中是否同步。",
                new UIView
                {
                    ClassName = "tree-select-zoom-stage",
                    Children = new() { picker }
                },
                new UIView
                {
                    ClassName = "tree-select-zoom-actions",
                    Children = new() { zoomButton, state }
                });
        }

        private static UIElement BuildCard(string title, string description, params UIElement[] children)
        {
            return new UIView
            {
                ClassName = "tree-select-demo-card",
                Children = new()
                {
                    new UILabel
                    {
                        Text = title,
                        ClassName = "tree-select-card-title"
                    },
                    new UILabel
                    {
                        Text = description,
                        ClassName = "tree-select-card-desc"
                    },
                    new UIView
                    {
                        ClassName = "tree-select-showcase",
                        Children = children.ToList()
                    }
                }
            };
        }

        private static List<TreeNode> CreateDepartmentData()
        {
            return
            [
                new TreeNode("engineering", "研发中心", new List<TreeNode>
                {
                    new TreeNode("platform", "平台组", new List<TreeNode>
                    {
                        new("platform-web", "Web 前端"),
                        new("platform-api", "平台 API"),
                        new("platform-mobile", "移动端")
                    }),
                    new TreeNode("quality", "质量组", new List<TreeNode>
                    {
                        new("quality-auto", "自动化测试"),
                        new("quality-manual", "手工测试")
                    })
                }),
                new TreeNode("sales", "销售中心", new List<TreeNode>
                {
                    new("sales-east", "华东销售"),
                    new("sales-south", "华南销售"),
                    new("sales-west", "西区销售")
                }),
                new("finance", "财务部"),
                new TreeNode("archived", "已归档部门") { Disabled = true }
            ];
        }
    }
}
