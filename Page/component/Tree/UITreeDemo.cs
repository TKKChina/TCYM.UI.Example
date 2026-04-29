using System.Collections.ObjectModel;
using TCYM.UI.Binding;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Tree;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Tree
{
    internal class UITreeDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Tree.style.css";

        internal UITreeDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "tree-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Tree 树形控件",
                    ClassName = new List<string> { "tree-demo-title" },
                },
                new UILabel
                {
                    Text = "多层次的结构列表。",
                    ClassName = new List<string> { "tree-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "文件夹、组织架构、生物分类、国家地区等等，世间万物的大多数结构都是树形结构。使用树控件可以完整展现其中的层级关系，并具有展开收起选择等交互功能。",
                    ClassName = new List<string> { "tree-demo-desc" },
                },
                new BasicSection(),
                new CheckableSection(),
                new DefaultExpandSection(),
                new CustomIconSection(),
                new CustomExpandIconSection(),
                new CustomTitleRenderSection(),
                new EventSection(),
            };
        }

        // ─── 公共数据模型 ───
        internal class TreeDemoModel : ObservableObject
        {
            private ObservableCollection<TreeNode> _basicNodes = new()
            {
                new("0", "根节点", new ObservableCollection<TreeNode>
                {
                    new("0-0", "子节点1", new ObservableCollection<TreeNode>
                    {
                        new() { Value = "0-0-0", Label = "孙子节点1", Disabled = true },
                        new("0-0-1", "孙子节点2", new ObservableCollection<TreeNode>
                        {
                            new() { Value = "0-0-1-0", Label = "曾孙子节点1" },
                            new() { Value = "0-0-1-1", Label = "曾孙子节点2" },
                        }),
                        new("0-0-2", "孙子节点3"),
                    }),
                    new("0-1", "子节点2"),
                }),
                new("2", "第二个根节点", new ObservableCollection<TreeNode>
                {
                    new("2-0", "子节点A"),
                    new("2-1", "子节点B"),
                }),
            };

            public ObservableCollection<TreeNode> BasicNodes
            {
                get => _basicNodes;
                set => SetProperty(ref _basicNodes, value);
            }

            private ObservableCollection<TreeNode> _checkableNodes = new()
            {
                new("0", "根节点", new ObservableCollection<TreeNode>
                {
                    new()
                    {
                        Label = "子节点1", Value = "0-1", Disabled = true,
                        Children = new()
                        {
                            new() { Value = "0-1-0", Label = "孙子节点1" },
                            new()
                            {
                                Value = "0-1-1", Label = "孙子节点2",
                                Icon = new UIIcon
                                {
                                    Content = "&#xe798;",
                                    Style = new DefaultUIStyle
                                    {
                                        FontFamily = UIFontManager.Get("TCYMIconFont"),
                                        Color = ColorHelper.ParseColor("#409eff"),
                                        MarginRight = 2,
                                    }
                                }
                            },
                        }
                    },
                    new("0-2", "子节点2"),
                }),
                new("2", "第二个根节点", new ObservableCollection<TreeNode>
                {
                    new("2-0", "子节点A"),
                    new("2-1", "子节点B"),
                }),
            };

            public ObservableCollection<TreeNode> CheckableNodes
            {
                get => _checkableNodes;
                set => SetProperty(ref _checkableNodes, value);
            }

            private ObservableCollection<TreeNode> _fileNodes = new()
            {
                new()
                {
                    Label = "根文件夹", Value = "folder-1",
                    Icon = new UIIcon
                    {
                        Content = "&#xeac5;",
                        Style = new DefaultUIStyle
                        {
                            FontFamily = UIFontManager.Get("TCYMIconFont"),
                            Color = ColorHelper.ParseColor("#f90"),
                            MarginRight = 2,
                        }
                    },
                    Children = new()
                    {
                        new()
                        {
                            Label = "子文件夹1", Value = "folder-1-1",
                            Icon = new UIIcon
                            {
                                Content = "&#xe795;",
                                Style = new DefaultUIStyle
                                {
                                    FontFamily = UIFontManager.Get("TCYMIconFont"),
                                    Color = ColorHelper.ParseColor("#3b82f6"),
                                    MarginRight = 2,
                                }
                            },
                            Children = new()
                            {
                                new()
                                {
                                    Label = "文件1.txt", Value = "file-1-1-1",
                                    Icon = new UIIcon
                                    {
                                        Content = "&#xe83d;",
                                        Style = new DefaultUIStyle
                                        {
                                            FontFamily = UIFontManager.Get("TCYMIconFont"),
                                            Color = ColorHelper.ParseColor("#fb6340"),
                                            MarginRight = 2,
                                        }
                                    },
                                },
                                new()
                                {
                                    Label = "文件2.docx", Value = "file-1-1-2",
                                    Icon = new UIIcon
                                    {
                                        Content = "&#xe83e;",
                                        Style = new DefaultUIStyle
                                        {
                                            FontFamily = UIFontManager.Get("TCYMIconFont"),
                                            Color = ColorHelper.ParseColor("#11cdef"),
                                            MarginRight = 2,
                                        }
                                    },
                                },
                                new()
                                {
                                    Label = "文件3.pdf", Value = "file-1-1-3",
                                    Icon = new UIIcon
                                    {
                                        Content = "&#xe83c;",
                                        Style = new DefaultUIStyle
                                        {
                                            FontFamily = UIFontManager.Get("TCYMIconFont"),
                                            Color = ColorHelper.ParseColor("#8965e0"),
                                            MarginRight = 2,
                                        }
                                    },
                                },
                                new()
                                {
                                    Label = "文件4.apk", Value = "file-1-1-4",
                                    Icon = new UIIcon
                                    {
                                        Content = "&#xe6ab;",
                                        Style = new DefaultUIStyle
                                        {
                                            FontFamily = UIFontManager.Get("TCYMIconFont"),
                                            Color = ColorHelper.ParseColor("#10b981"),
                                            MarginRight = 2,
                                        }
                                    },
                                },
                            }
                        },
                        new()
                        {
                            Label = "子文件夹2", Value = "folder-1-2",
                            Icon = new UIIcon
                            {
                                Content = "&#xe798;",
                                Style = new DefaultUIStyle
                                {
                                    FontFamily = UIFontManager.Get("TCYMIconFont"),
                                    Color = ColorHelper.ParseColor("#3b82f6"),
                                    MarginRight = 2,
                                }
                            },
                        },
                    }
                },
            };

            public ObservableCollection<TreeNode> FileNodes
            {
                get => _fileNodes;
                set => SetProperty(ref _fileNodes, value);
            }
        }

        /// <summary>
        /// 基本用法
        /// </summary>
        private class BasicSection : UIView
        {
            internal BasicSection()
            {
                var model = new TreeDemoModel();
                ClassName = new List<string> { "tree-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "基本用法",
                        ClassName = new List<string> { "tree-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "最简单的用法，展示可展开/收起的树形结构。支持 Disabled 禁用节点。",
                        ClassName = new List<string> { "tree-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tree-showcase" },
                        Children = new()
                        {
                            new UITree
                            {
                                Style = new UpdateUIStyle { Width = 260 },
                                DataContext = new UIDataContext(model),
                                Binding = new()
                                {
                                    new BindingSpec
                                    {
                                        TargetProperty = nameof(UITree.TreeData),
                                        SourceProperty = nameof(TreeDemoModel.BasicNodes),
                                    }
                                }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 可勾选
        /// </summary>
        private class CheckableSection : UIView
        {
            internal CheckableSection()
            {
                var model = new TreeDemoModel();
                ClassName = new List<string> { "tree-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "可勾选",
                        ClassName = new List<string> { "tree-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "设置 Checkable=true 在节点前添加复选框。支持 CheckboxLinked 父子联动、DisableCheckbox 禁用复选框，以及节点级别的自定义图标。",
                        ClassName = new List<string> { "tree-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tree-showcase" },
                        Children = new()
                        {
                            new UITree
                            {
                                Style = new UpdateUIStyle { Width = 260 },
                                Checkable = true,
                                DataContext = new UIDataContext(model),
                                Binding = new()
                                {
                                    new BindingSpec
                                    {
                                        TargetProperty = nameof(UITree.TreeData),
                                        SourceProperty = nameof(TreeDemoModel.CheckableNodes),
                                    }
                                }
                            },
                            new UITree
                            {
                                Style = new UpdateUIStyle { Width = 260 },
                                Checkable = true,
                                CheckboxLinked = false,
                                DataContext = new UIDataContext(model),
                                Binding = new()
                                {
                                    new BindingSpec
                                    {
                                        TargetProperty = nameof(UITree.TreeData),
                                        SourceProperty = nameof(TreeDemoModel.BasicNodes),
                                    }
                                }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 默认展开与选中
        /// </summary>
        private class DefaultExpandSection : UIView
        {
            internal DefaultExpandSection()
            {
                var model = new TreeDemoModel();
                ClassName = new List<string> { "tree-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "默认展开与选中",
                        ClassName = new List<string> { "tree-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "通过 DefaultExpanded 设置默认展开的节点、DefaultSelected 设置默认选中节点、DefaultExpandAll 展开全部节点。",
                        ClassName = new List<string> { "tree-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tree-showcase" },
                        Children = new()
                        {
                            new UITree
                            {
                                Style = new UpdateUIStyle { Width = 260 },
                                DefaultExpanded = new[] { "2" },
                                DefaultSelected = new[] { "2-1" },
                                DataContext = new UIDataContext(model),
                                Binding = new()
                                {
                                    new BindingSpec
                                    {
                                        TargetProperty = nameof(UITree.TreeData),
                                        SourceProperty = nameof(TreeDemoModel.BasicNodes),
                                    }
                                }
                            },
                            new UITree
                            {
                                Style = new UpdateUIStyle { Width = 260 },
                                DefaultExpandAll = true,
                                DataContext = new UIDataContext(model),
                                Binding = new()
                                {
                                    new BindingSpec
                                    {
                                        TargetProperty = nameof(UITree.TreeData),
                                        SourceProperty = nameof(TreeDemoModel.BasicNodes),
                                    }
                                }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 自定义节点图标
        /// </summary>
        private class CustomIconSection : UIView
        {
            internal CustomIconSection()
            {
                var model = new TreeDemoModel();
                ClassName = new List<string> { "tree-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义节点图标",
                        ClassName = new List<string> { "tree-card-title", "label-purple" }
                    },
                    new UILabel
                    {
                        Text = "通过 TreeNode.Icon 为每个节点设置不同的图标，适合文件管理器等场景。支持虚线连接线 LineStyle=\"dashed\"。",
                        ClassName = new List<string> { "tree-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tree-showcase" },
                        Children = new()
                        {
                            new UITree
                            {
                                Style = new UpdateUIStyle { Width = 260 },
                                DefaultExpanded = new[] { "folder-1-1" },
                                LineStyle = "dashed",
                                DataContext = new UIDataContext(model),
                                Binding = new()
                                {
                                    new BindingSpec
                                    {
                                        TargetProperty = nameof(UITree.TreeData),
                                        SourceProperty = nameof(TreeDemoModel.FileNodes),
                                    }
                                }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 自定义展开/折叠图标
        /// </summary>
        private class CustomExpandIconSection : UIView
        {
            internal CustomExpandIconSection()
            {
                var model = new TreeDemoModel();
                ClassName = new List<string> { "tree-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义展开/折叠图标",
                        ClassName = new List<string> { "tree-card-title", "label-cany" }
                    },
                    new UILabel
                    {
                        Text = "通过 SwitcherIcon 自定义展开/折叠时的图标。参数为 (TreeNode, bool isExpanded)，返回自定义 UIIcon。",
                        ClassName = new List<string> { "tree-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tree-showcase" },
                        Children = new()
                        {
                            new UITree
                            {
                                Style = new UpdateUIStyle { Width = 260 },
                                DefaultExpandAll = true,
                                SwitcherIcon = (TreeNode node, bool isExpanded) =>
                                {
                                    return new UIIcon
                                    {
                                        Content = isExpanded ? "&#xe640;" : "&#xe62f;",
                                        Style = new DefaultUIStyle
                                        {
                                            FontFamily = UIFontManager.Get("TCYMIconFont"),
                                            Color = ColorHelper.ParseColor("#409eff"),
                                            MarginRight = 2,
                                        }
                                    };
                                },
                                DataContext = new UIDataContext(model),
                                Binding = new()
                                {
                                    new BindingSpec
                                    {
                                        TargetProperty = nameof(UITree.TreeData),
                                        SourceProperty = nameof(TreeDemoModel.BasicNodes),
                                    }
                                }
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 自定义标题渲染
        /// </summary>
        private class CustomTitleRenderSection : UIView
        {
            internal CustomTitleRenderSection()
            {
                var model = new TreeDemoModel();
                ClassName = new List<string> { "tree-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义标题渲染",
                        ClassName = new List<string> { "tree-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "通过 TitleRender 完全自定义节点标题区域。hover 时显示操作按钮（添加子节点、删除节点）。支持虚线连接线。",
                        ClassName = new List<string> { "tree-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tree-showcase" },
                        Children = new()
                        {
                            CreateCustomTitleTree(model),
                        }
                    },
                };
            }

            private static UITree CreateCustomTitleTree(TreeDemoModel model)
            {
                var tree = new UITree
                {
                    Style = new UpdateUIStyle { Width = 300 },
                    DefaultExpandAll = true,
                    LineStyle = "dashed",
                    TitleRender = (TreeNode node, UITree treeInstance) =>
                    {
                        return new UIView
                        {
                            Style = new DefaultUIStyle
                            {
                                Width = "100%",
                                Height = "100%",
                                Display = "flex",
                                AlignItems = "center",
                                Cursor = node.Disabled ? Enums.UICursor.NotAllowed : Enums.UICursor.Pointer,
                            },
                            Children = new()
                            {
                                new UILabel
                                {
                                    Text = node.Label,
                                    Style = new DefaultUIStyle
                                    {
                                        FontSize = 14,
                                        PointerEvents = "none",
                                        Color = node.Disabled
                                            ? ColorHelper.ParseColor("#c0c0c0")
                                            : ColorHelper.ParseColor("#000000"),
                                    }
                                }
                            },
                            Events = new()
                            {
                                MouseEnter = (e) =>
                                {
                                    e.Element?.AddChild(new UIIcon
                                    {
                                        Content = "&#xe62f;",
                                        Style = new DefaultUIStyle
                                        {
                                            FontFamily = UIFontManager.Get("TCYMIconFont"),
                                            Color = ColorHelper.ParseColor("#409eff"),
                                            MarginRight = 2,
                                            MarginLeft = 5,
                                            FontSize = 16,
                                        },
                                        Events = new()
                                        {
                                            MouseDown = (ev) =>
                                            {
                                                void AddChildNode(IList<TreeNode> nodes)
                                                {
                                                    foreach (var n in nodes)
                                                    {
                                                        if (n.Value == node.Value)
                                                        {
                                                            if (n.Children == null) n.Children = new();
                                                            n.Children.Add(new TreeNode
                                                            {
                                                                Value = $"{n.Value}-new-{n.Children.Count}",
                                                                Label = "新节点"
                                                            });
                                                            return;
                                                        }
                                                        if (n.Children != null && n.Children.Count > 0)
                                                            AddChildNode(n.Children);
                                                    }
                                                }
                                                AddChildNode(model.BasicNodes);
                                            }
                                        }
                                    });
                                    e.Element?.AddChild(new UIIcon
                                    {
                                        Content = "&#xe612;",
                                        Style = new DefaultUIStyle
                                        {
                                            FontFamily = UIFontManager.Get("TCYMIconFont"),
                                            Color = ColorHelper.ParseColor("#FF4D4F"),
                                            MarginRight = 2,
                                            MarginLeft = 2,
                                            FontSize = 14,
                                        },
                                        Events = new()
                                        {
                                            MouseDown = (ev) =>
                                            {
                                                void RemoveNode(IList<TreeNode> nodes)
                                                {
                                                    for (int i = 0; i < nodes.Count; i++)
                                                    {
                                                        if (nodes[i].Value == node.Value)
                                                        {
                                                            nodes.RemoveAt(i);
                                                            return;
                                                        }
                                                        if (nodes[i].Children != null && nodes[i].Children?.Count > 0)
                                                            RemoveNode(nodes[i].Children);
                                                    }
                                                }
                                                RemoveNode(model.BasicNodes);
                                            }
                                        }
                                    });
                                },
                                MouseLeave = (e) =>
                                {
                                    e.Element?.ClearChildren();
                                    e.Element?.AddChild(new UILabel
                                    {
                                        Text = node.Label,
                                        Style = new DefaultUIStyle
                                        {
                                            FontSize = 14,
                                            PointerEvents = "none",
                                            Color = node.Disabled
                                                ? ColorHelper.ParseColor("#c0c0c0")
                                                : ColorHelper.ParseColor("#000000"),
                                        }
                                    });
                                },
                            },
                        };
                    },
                    DataContext = new UIDataContext(model),
                    Binding = new()
                    {
                        new BindingSpec
                        {
                            TargetProperty = nameof(UITree.TreeData),
                            SourceProperty = nameof(TreeDemoModel.BasicNodes),
                        }
                    }
                };
                return tree;
            }
        }

        /// <summary>
        /// 事件回调
        /// </summary>
        private class EventSection : UIView
        {
            internal EventSection()
            {
                var model = new TreeDemoModel();
                ClassName = new List<string> { "tree-demo-card" };

                var selectLabel = new UILabel
                {
                    Text = "选中节点：(无)",
                    ClassName = new List<string> { "tree-event-label" }
                };
                var expandLabel = new UILabel
                {
                    Text = "展开节点：(无)",
                    ClassName = new List<string> { "tree-event-label" }
                };
                var checkLabel = new UILabel
                {
                    Text = "勾选节点：(无)",
                    ClassName = new List<string> { "tree-event-label" }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "事件回调",
                        ClassName = new List<string> { "tree-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "通过 OnSelect 监听选中变化、OnExpand 监听展开/收起、OnCheck 监听复选框勾选变化。",
                        ClassName = new List<string> { "tree-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "tree-showcase" },
                        Children = new()
                        {
                            CreateEventTree(model, selectLabel, expandLabel, checkLabel),
                        }
                    },
                    selectLabel,
                    expandLabel,
                    checkLabel,
                };
            }

            private static UITree CreateEventTree(TreeDemoModel model, UILabel selectLabel, UILabel expandLabel, UILabel checkLabel)
            {
                var tree = new UITree
                {
                    Style = new UpdateUIStyle { Width = 260 },
                    Checkable = true,
                    OnSelect = (keys, node) =>
                    {
                        selectLabel.Text = $"选中节点：{node.Label} (Value={node.Value})";
                    },
                    OnExpand = (keys, node, expanded) =>
                    {
                        expandLabel.Text = $"展开节点：{node.Label} (展开={expanded}，当前展开数={keys.Length})";
                    },
                    OnCheck = (keys, node, isChecked) =>
                    {
                        checkLabel.Text = $"勾选节点：{node.Label} (选中={isChecked}，已选数={keys.Length})";
                    },
                    DataContext = new UIDataContext(model),
                    Binding = new()
                    {
                        new BindingSpec
                        {
                            TargetProperty = nameof(UITree.TreeData),
                            SourceProperty = nameof(TreeDemoModel.BasicNodes),
                        }
                    }
                };
                return tree;
            }
        }
    }
}
