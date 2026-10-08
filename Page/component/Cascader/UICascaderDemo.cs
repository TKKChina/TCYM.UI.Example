using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Cascader;
using TCYM.UI.Elements.Tree;

namespace TCYM.UI.Example.Page.component.Cascader
{
    internal class UICascaderDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Cascader.style.css";

        internal UICascaderDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = "cascader-demo-view";
            Children = new()
            {
                new UILabel
                {
                    Text = "Cascader 级联选择器",
                    ClassName = "cascader-demo-title"
                },
                new UILabel
                {
                    Text = "在弹出的多列面板中沿层级路径选择一个或多个选项。",
                    ClassName = "cascader-demo-subtitle"
                },
                new UILabel
                {
                    Text = "参考 Semi Design Cascader。弹层按当前路径逐级渲染选项列，支持单选、多选（Checkbox 父子联动）、ChangeOnSelect、悬停展开、路径搜索、清除、禁用、尺寸、变体和事件回调。",
                    ClassName = "cascader-demo-desc"
                },
                BuildBasicSection(),
                BuildExpandSection(),
                BuildMultipleSection(),
                BuildSearchSection(),
                BuildAppearanceSection(),
                BuildEventSection()
            };
        }

        private static UIElement BuildBasicSection()
        {
            return BuildCard(
                "基本使用",
                "点击选择框打开级联面板，沿列逐级点击叶子节点完成选择。可以设置默认值、允许清除，或禁用整个选择器。",
                new UICascader
                {
                    Data = CreateRegionData(),
                    Placeholder = "请选择省市区",
                    AllowClear = true,
                    Style = new UpdateUIStyle { Width = 280 }
                },
                new UICascader
                {
                    Data = CreateRegionData(),
                    DefaultValue = "zj-hz-xh",
                    AllowClear = true,
                    Style = new UpdateUIStyle { Width = 280 }
                },
                new UICascader
                {
                    Data = CreateRegionData(),
                    DefaultValue = "gd-sz-ns",
                    Disabled = true,
                    Style = new UpdateUIStyle { Width = 280 }
                });
        }

        private static UIElement BuildExpandSection()
        {
            return BuildCard(
                "悬停展开与 ChangeOnSelect",
                "左侧 ExpandTrigger.Hover 悬停即展开下一列；右侧 ChangeOnSelect 允许直接选中任意非叶子节点，触发器显示所选层级。",
                new UIView
                {
                    ClassName = "cascader-example-group",
                    Children = new()
                    {
                        new UILabel
                        {
                            Text = "悬停展开",
                            ClassName = "cascader-example-caption"
                        },
                        new UICascader
                        {
                            Data = CreateRegionData(),
                            ExpandTrigger = CascaderExpandTrigger.Hover,
                            Placeholder = "悬停展开下一级",
                            AllowClear = true,
                            Style = new UpdateUIStyle { Width = 300 }
                        }
                    }
                },
                new UIView
                {
                    ClassName = "cascader-example-group",
                    Children = new()
                    {
                        new UILabel
                        {
                            Text = "可选任意层级",
                            ClassName = "cascader-example-caption"
                        },
                        new UICascader
                        {
                            Data = CreateRegionData(),
                            ChangeOnSelect = true,
                            DefaultValue = "zj-hz",
                            Placeholder = "可选省或市",
                            AllowClear = true,
                            Style = new UpdateUIStyle { Width = 300 }
                        }
                    }
                });
        }

        private static UIElement BuildMultipleSection()
        {
            return BuildCard(
                "多选",
                "Multiple 模式下选项带 Checkbox，勾选父节点自动选中全部子节点；触发器以标签回填完整路径，MaxTagCount 控制展示数量。",
                new UICascader
                {
                    Data = CreateRegionData(),
                    Multiple = true,
                    DefaultValues = new[] { "zj-hz-xh", "zj-hz-bj", "gd-gz-th" },
                    AllowClear = true,
                    MaxTagCount = 2,
                    MaxTagTextLength = 10,
                    Placeholder = "请选择多个地区",
                    Style = new UpdateUIStyle { Width = 360 }
                },
                new UICascader
                {
                    Data = CreateRegionData(),
                    Multiple = true,
                    DefaultValue = "js-nj",
                    ShowPath = false,
                    AllowClear = true,
                    Placeholder = "标签只显示末级",
                    Style = new UpdateUIStyle { Width = 260 }
                });
        }

        private static UIElement BuildSearchSection()
        {
            return BuildCard(
                "搜索",
                "ShowSearch 在弹层顶部显示搜索框，命中节点以完整路径平铺展示；点击结果直接提交该路径。",
                new UICascader
                {
                    Data = CreateRegionData(),
                    ShowSearch = true,
                    SearchPlaceholder = "搜索省市区",
                    Placeholder = "支持搜索",
                    AllowClear = true,
                    Style = new UpdateUIStyle { Width = 280 }
                });
        }

        private static UIElement BuildAppearanceSection()
        {
            return BuildCard(
                "尺寸、变体与状态",
                "支持 Small、Medium、Large，以及 Outlined、Filled、Borderless、Underlined、Error 和 Warning。",
                new UICascader
                {
                    Data = CreateRegionData(),
                    Size = CascaderSize.Small,
                    Placeholder = "Small",
                    Style = new UpdateUIStyle { Width = 150 }
                },
                new UICascader
                {
                    Data = CreateRegionData(),
                    Variant = CascaderVariant.Filled,
                    Placeholder = "Filled",
                    Style = new UpdateUIStyle { Width = 160 }
                },
                new UICascader
                {
                    Data = CreateRegionData(),
                    Variant = CascaderVariant.Underlined,
                    Placeholder = "Underlined",
                    Style = new UpdateUIStyle { Width = 170 }
                },
                new UICascader
                {
                    Data = CreateRegionData(),
                    Size = CascaderSize.Large,
                    Status = CascaderStatus.Error,
                    Placeholder = "Error",
                    Style = new UpdateUIStyle { Width = 170 }
                },
                new UICascader
                {
                    Data = CreateRegionData(),
                    Status = CascaderStatus.Warning,
                    Placeholder = "Warning",
                    Style = new UpdateUIStyle { Width = 170 }
                });
        }

        private static UIElement BuildEventSection()
        {
            var status = new UILabel
            {
                Text = "尚未选择",
                ClassName = "cascader-event-status"
            };
            var picker = new UICascader
            {
                Data = CreateRegionData(),
                Placeholder = "选择后查看事件回显",
                AllowClear = true,
                Style = new UpdateUIStyle { Width = 340 },
                OnChange = (values, paths) =>
                {
                    string pathText = paths.Length == 0
                        ? "空"
                        : string.Join("；", paths.Select(path => string.Join(" / ", path.Select(node => node.Label))));
                    status.Text = $"OnChange：{string.Join(", ", values)} / {pathText}";
                },
                OnSelect = (path, nodes) =>
                    status.Text = $"OnSelect：{string.Join(" / ", nodes.Select(node => node.Label))}",
                OnClear = () => status.Text = "OnClear：已清空"
            };

            return BuildCard(
                "事件回显",
                "OnChange 返回全部叶子值与完整路径节点；OnSelect 在每次提交路径时触发；OnClear 响应清除操作。",
                new UIView
                {
                    ClassName = "cascader-event-group",
                    Children = new() { picker, status }
                });
        }

        private static UIElement BuildCard(string title, string description, params UIElement[] children)
        {
            return new UIView
            {
                ClassName = "cascader-demo-card",
                Children = new()
                {
                    new UILabel
                    {
                        Text = title,
                        ClassName = "cascader-card-title"
                    },
                    new UILabel
                    {
                        Text = description,
                        ClassName = "cascader-card-desc"
                    },
                    new UIView
                    {
                        ClassName = "cascader-showcase",
                        Children = children.ToList()
                    }
                }
            };
        }

        private static List<TreeNode> CreateRegionData()
        {
            return
            [
                new TreeNode("zj", "浙江省", new List<TreeNode>
                {
                    new TreeNode("zj-hz", "杭州市", new List<TreeNode>
                    {
                        new("zj-hz-xh", "西湖区"),
                        new("zj-hz-bj", "滨江区"),
                        new("zj-hz-yh", "余杭区")
                    }),
                    new TreeNode("zj-nb", "宁波市", new List<TreeNode>
                    {
                        new("zj-nb-hs", "海曙区"),
                        new("zj-nb-jb", "江北区")
                    })
                }),
                new TreeNode("js", "江苏省", new List<TreeNode>
                {
                    new TreeNode("js-nj", "南京市", new List<TreeNode>
                    {
                        new("js-nj-xw", "玄武区"),
                        new("js-nj-gl", "鼓楼区")
                    }),
                    new TreeNode("js-sz", "苏州市", new List<TreeNode>
                    {
                        new("js-sz-gs", "姑苏区"),
                        new("js-sz-gy", "工业园区")
                    })
                }),
                new TreeNode("gd", "广东省", new List<TreeNode>
                {
                    new TreeNode("gd-gz", "广州市", new List<TreeNode>
                    {
                        new("gd-gz-th", "天河区"),
                        new("gd-gz-yx", "越秀区")
                    }),
                    new TreeNode("gd-sz", "深圳市", new List<TreeNode>
                    {
                        new("gd-sz-ns", "南山区"),
                        new("gd-sz-ft", "福田区")
                    })
                }),
                new("hk", "港澳台（暂不可选）") { Disabled = true }
            ];
        }
    }
}
