using TCYM.UI.Binding;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Input;
using TCYM.UI.Elements.Segmented;
using TCYM.UI.Elements.Select;
using TCYM.UI.Elements.Slider;
using TCYM.UI.Elements.Tree;
using TCYM.UI.Enums;

namespace TCYM.UI.Example.Page.component.Binding;

/// <summary>
/// 展示 Binding V2 的页面。
/// 本类只负责组件组合和 Binding 声明；状态、样例数据与 Converter 均位于独立文件中。
/// </summary>
internal sealed class UIBindingDemo : UIScrollView
{
    /// <summary>页面专用样式的嵌入资源路径。</summary>
    private const string DemoCssPath =
        "res://TCYM.UI.Example/Page.component.Binding.style.css";

    /// <summary>每次替换 BindingContext 时递增，帮助确认新旧数据实例已经切换。</summary>
    private int _contextVersion = 1;

    /// <summary>创建页面、装载样式并建立所有示例卡片。</summary>
    internal UIBindingDemo()
    {
        UISystem.LoadStyleFile(DemoCssPath);
        BindingContext = BindingDemoData.CreateModel(_contextVersion);
        ClassName = new List<string> { "binding-demo-view" };

        Children = new()
        {
            new UILabel
            {
                Text = "Binding 数据绑定",
                ClassName = new List<string> { "binding-demo-title" }
            },
            new UILabel
            {
                Text = "以强类型属性描述符和 lambda 路径连接控件与 ViewModel。",
                ClassName = new List<string> { "binding-demo-title-sub" }
            },
            new UILabel
            {
                Text = "支持所有组件公开属性、BindingContext 继承、TwoWay 回写、嵌套路径重订阅、OneTime，以及运行时替换数据上下文。",
                ClassName = new List<string> { "binding-demo-desc" }
            },
            CreateViewBindingCard(),
            CreateAutomaticPropertyCard(),
            CreateBasePropertyBindingCard(),
            CreateTwoWayControlsCard(),
            CreateSelectionAndPagingCard(),
            CreateCollectionBindingCard(),
            CreateExplicitUpdateCard(),
            CreateContextCard(),
            CreateOneTimeCard()
        };
    }

    /// <summary>
    /// 演示无需手写 BindingProperty 的组件属性，以及派生组件可直接使用的 UIElement 公共描述符。
    /// </summary>
    private UIView CreateAutomaticPropertyCard()
    {
        return Card(
            "所有组件属性自动 Binding",
            "未手写 [BindingProperty] 的普通组件属性也会生成 XxxProperty；公共 UIElement 属性可通过派生组件类型直接使用。",
            new UILabel
            {
                Text = "UIProgress.PercentProperty / UIDatePicker.ValueProperty / UIButton.WidthProperty",
                ClassName = new List<string> { "binding-code-hint" }
            },
            new UIProgress
            {
                Width = 420,
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UIProgress.PercentProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.Progress
                    },
                    new()
                    {
                        TargetProperty = UIProgress.StrokeColorProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.Color
                    }
                }
            },
            new UIDatePicker
            {
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UIDatePicker.ValueProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.SelectedDate
                    }
                }
            },
            new UIButton
            {
                Text = "更新进度、日期和按钮宽度",
                ClassName = new List<string>
                {
                    "binding-action-btn",
                    "binding-action-primary"
                },
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UIButton.WidthProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.ControlWidth
                    }
                },
                Events = new()
                {
                    Click = _ =>
                    {
                        if (BindingContext is BindingDemoViewModel model)
                            model.AdvanceAutomaticProperties();
                    }
                }
            });
    }

    /// <summary>
    /// 演示 Width、Height、颜色、边框、圆角和透明度等 UIElement 公共属性。
    /// 点击按钮后一次更新多个属性，可直接观察 Layout 与 Redraw 失效是否生效。
    /// </summary>
    private UIView CreateBasePropertyBindingCard()
    {
        var preview = new UILabel
        {
            BorderWidth = 1,
            ClassName = new List<string> { "binding-base-preview" },
            Binding = new()
            {
                new()
                {
                    TargetProperty = UILabel.TextProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.AppearanceText
                },
                new()
                {
                    TargetProperty = WidthProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.AppearanceWidth
                },
                new()
                {
                    TargetProperty = HeightProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.AppearanceHeight
                },
                new()
                {
                    TargetProperty = BackgroundColorProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.AppearanceBackground
                },
                new()
                {
                    TargetProperty = TextColorProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.AppearanceForeground
                },
                new()
                {
                    TargetProperty = BorderColorProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.AppearanceBorder
                },
                new()
                {
                    TargetProperty = BorderRadiusProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.AppearanceRadius
                },
                new()
                {
                    TargetProperty = OpacityProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.AppearanceOpacity
                }
            }
        };

        return Card(
            "UIElement 公共属性绑定",
            "同一个 Label 同时绑定 8 个公共属性；这些描述符声明在 UIElement，但可以通过 UILabel.WidthProperty 等形式直接使用。",
            new UILabel
            {
                Text = "Text / Width / Height / BackgroundColor / TextColor / BorderColor / BorderRadius / Opacity",
                ClassName = new List<string> { "binding-code-hint" }
            },
            preview,
            ActionButton(
                "轮换公共属性",
                "binding-action-primary",
                () =>
                {
                    if (BindingContext is BindingDemoViewModel model)
                        model.CycleAppearance();
                }));
    }

    /// <summary>
    /// 演示多个交互控件共享同一 ViewModel，并通过默认 TwoWay 描述符自动回写。
    /// Options 使用 OneWay，Value/Checked 使用 TwoWay，Label 再从源端汇总状态。
    /// </summary>
    private UIView CreateTwoWayControlsCard()
    {
        var enabledSwitch = new UISwitch
        {
            Binding = new()
            {
                new()
                {
                    TargetProperty = UISwitch.CheckedProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.Enabled
                }
            }
        };

        var acceptedCheckbox = new UICheckbox
        {
            Binding = new()
            {
                new()
                {
                    TargetProperty = UICheckbox.IsCheckedProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.Accepted
                }
            }
        };

        var volumeSlider = new UISlider
        {
            Min = 0,
            Max = 100,
            Step = 1,
            ClassName = new List<string> { "binding-slider" },
            Binding = new()
            {
                new()
                {
                    TargetProperty = UISlider.ValueProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.Volume
                }
            }
        };

        var statusSegmented = new UISegmented
        {
            Binding = new()
            {
                new()
                {
                    TargetProperty = UISegmented.OptionsProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.StatusOptions
                },
                new()
                {
                    TargetProperty = UISegmented.ValueProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.Status
                }
            }
        };

        // 使用统一的两列容器承载全部交互控件，避免不同控件自身宽度导致起始位置漂移。
        var controlsGrid = new UIView
        {
            ClassName = new List<string> { "binding-control-grid" },
            Children = new()
            {
                ControlRow("启用功能", enabledSwitch),
                ControlRow("确认协议", acceptedCheckbox),
                ControlRow("音量", volumeSlider),
                ControlRow("处理状态", statusSegmented)
            }
        };

        return Card(
            "多控件 TwoWay 回写",
            "操作 Switch、Checkbox、Slider 和 Segmented 后，控件通过目标变化通知写回同一个 ViewModel；重置按钮则演示 Source 反向更新所有控件。",
            new UILabel
            {
                Text = "CheckedProperty / IsCheckedProperty / ValueProperty + Converter",
                ClassName = new List<string> { "binding-code-hint" }
            },
            controlsGrid,
            new UILabel
            {
                ClassName = new List<string> { "binding-result", "binding-result-primary" },
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UILabel.TextProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.Volume,
                        Converter = BindingDemoPercentConverter.Instance
                    }
                }
            },
            new UILabel
            {
                ClassName = new List<string> { "binding-result", "binding-result-success" },
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UILabel.TextProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.InteractiveSummary
                    }
                }
            },
            ActionButton(
                "从 Source 重置控件",
                "binding-action-default",
                () =>
                {
                    if (BindingContext is BindingDemoViewModel model)
                        model.ResetInteractiveControls();
                }));
    }

    /// <summary>
    /// 演示复杂选项对象、Select 双向值，以及 Pagination 的 Total/Current/PageSize 混合方向绑定。
    /// </summary>
    private UIView CreateSelectionAndPagingCard()
    {
        var roleSelect = new UISelect
        {
            AllowClear = true,
            ShowSearch = true,
            ClassName = new List<string> { "binding-select" },
            Binding = new()
            {
                new()
                {
                    TargetProperty = UISelect.OptionsProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.RoleOptions
                },
                new()
                {
                    TargetProperty = UISelect.ValueProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.SelectedRole
                }
            }
        };

        var pagination = new UIPagination
        {
            ShowSizeChanger = true,
            ShowQuickJumper = true,
            PageSizeOptions = new List<int> { 10, 20, 50 },
            Binding = new()
            {
                new()
                {
                    TargetProperty = UIPagination.TotalProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.TotalItems
                },
                new()
                {
                    TargetProperty = UIPagination.CurrentProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.CurrentPage
                },
                new()
                {
                    TargetProperty = UIPagination.PageSizeProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.PageSize
                }
            }
        };

        return Card(
            "Select 与 Pagination",
            "Options/Total 由 Source 单向提供，SelectedRole、Current 和 PageSize 在用户操作后 TwoWay 回写。",
            new UILabel
            {
                Text = "UISelect.OptionsProperty + ValueProperty / UIPagination.TotalProperty + CurrentProperty + PageSizeProperty",
                ClassName = new List<string> { "binding-code-hint" }
            },
            roleSelect,
            new UILabel
            {
                ClassName = new List<string> { "binding-result", "binding-result-primary" },
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UILabel.TextProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.SelectionSummary
                    }
                }
            },
            pagination,
            new UILabel
            {
                ClassName = new List<string> { "binding-result", "binding-result-neutral" },
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UILabel.TextProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.PagingSummary
                    }
                }
            },
            new UIView
            {
                ClassName = new List<string> { "binding-action-row" },
                Children = new()
                {
                    ActionButton(
                        "增加 17 条数据",
                        "binding-action-primary",
                        () =>
                        {
                            if (BindingContext is BindingDemoViewModel model)
                                model.AddRecords();
                        }),
                    ActionButton(
                        "重置选择和分页",
                        "binding-action-default",
                        () =>
                        {
                            if (BindingContext is BindingDemoViewModel model)
                                model.ResetSelectionAndPaging();
                        })
                }
            });
    }

    /// <summary>
    /// 演示 ObservableCollection 原地变化、整个集合对象替换，以及自动生成的普通 Tree 属性。
    /// </summary>
    private UIView CreateCollectionBindingCard()
    {
        var tree = new UITree
        {
            DefaultExpandAll = true,
            ClassName = new List<string> { "binding-tree" },
            Binding = new()
            {
                new()
                {
                    TargetProperty = UITree.TreeDataProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.TreeNodes
                },
                new()
                {
                    TargetProperty = UITree.ShowLineProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.TreeShowLine
                }
            }
        };

        return Card(
            "集合变化与 Source 替换",
            "第一个按钮修改当前 ObservableCollection；第二个按钮替换整个集合并触发路径重订阅；ShowLine 是自动生成的普通属性描述符。",
            new UILabel
            {
                Text = "UITree.TreeDataProperty + UITree.ShowLineProperty",
                ClassName = new List<string> { "binding-code-hint" }
            },
            tree,
            new UILabel
            {
                ClassName = new List<string> { "binding-result", "binding-result-success" },
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UILabel.TextProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.TreeSummary
                    }
                }
            },
            new UIView
            {
                ClassName = new List<string> { "binding-action-row" },
                Children = new()
                {
                    ActionButton(
                        "追加集合节点",
                        "binding-action-primary",
                        () =>
                        {
                            if (BindingContext is BindingDemoViewModel model)
                                model.AddTreeNode();
                        }),
                    ActionButton(
                        "替换整个集合",
                        "binding-action-default",
                        () =>
                        {
                            if (BindingContext is BindingDemoViewModel model)
                                model.ReplaceTreeNodes();
                        }),
                    ActionButton(
                        "切换连接线",
                        "binding-action-default",
                        () =>
                        {
                            if (BindingContext is BindingDemoViewModel model)
                                model.ToggleTreeLine();
                        })
                }
            });
    }

    /// <summary>
    /// 演示 Explicit 更新触发器：输入过程只改变 Target，点击提交后才写回 Source。
    /// </summary>
    private UIView CreateExplicitUpdateCard()
    {
        var explicitInput = new UIInput
        {
            Placeholder = "输入内容后点击“提交到 Source”",
            ClassName = new List<string> { "binding-explicit-input" },
            Binding = new()
            {
                new()
                {
                    TargetProperty = UIInput.TextProperty,
                    SourceProperty = (BindingDemoViewModel vm) => vm.ExplicitText,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = BindingUpdateSourceTrigger.Explicit
                }
            }
        };

        return Card(
            "Explicit 手动回写",
            "UpdateSourceTrigger.Explicit 不会在每次输入时更新 ViewModel；调用 UpdateBindingSource 后，下面的 Source 标签才变化。",
            new UILabel
            {
                Text = "UpdateSourceTrigger = Explicit / UpdateBindingSource(UIInput.TextProperty)",
                ClassName = new List<string> { "binding-code-hint" }
            },
            explicitInput,
            new UILabel
            {
                ClassName = new List<string> { "binding-result", "binding-result-neutral" },
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UILabel.TextProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.ExplicitText,
                        StringFormat = "Source 当前值：{0}"
                    }
                }
            },
            new UIView
            {
                ClassName = new List<string> { "binding-action-row" },
                Children = new()
                {
                    ActionButton(
                        "提交到 Source",
                        "binding-action-primary",
                        () => explicitInput.UpdateBindingSource(UIInput.TextProperty)),
                    ActionButton(
                        "从 Source 重置",
                        "binding-action-default",
                        () =>
                        {
                            if (BindingContext is BindingDemoViewModel model)
                                model.ResetExplicitText();
                        })
                }
            });
    }

    /// <summary>保留最初从 View 复制过来的组合绑定案例。</summary>
    private UIView CreateViewBindingCard()
    {
        return Card(
            "组合绑定",
            "UILabel 绑定 ClassName 与 Text，UIInput 绑定 Text 与颜色，UIButton 绑定 Name。",
            new UILabel
            {
                Text = "ClassNameProperty + UILabel.TextProperty",
                ClassName = new List<string> { "binding-code-hint" }
            },
            new UILabel
            {
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = ClassNameProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.ClassNames
                    },
                    new()
                    {
                        TargetProperty = UILabel.TextProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.InputDisplayText
                    }
                }
            },
            new UILabel
            {
                Text = "UIInput.TextProperty + InputTextColorProperty + InputFocusBorderColorProperty",
                ClassName = new List<string> { "binding-code-hint" }
            },
            new UIInput
            {
                Type = UIInputType.Text,
                Placeholder = "输入不同长度的内容，文字与 Focus 边框颜色会变化",
                Suffix = new UILabel
                {
                    Text = "RMB",
                    Style = new DefaultUIStyle
                    {
                        Width = 30,
                        Height = "100%",
                        Display = "flex",
                        AlignItems = "center",
                        JustifyContent = "center",
                        FontSize = 14
                    }
                },
                AllowClear = true,
                Count = new CountConfig
                {
                    Max = 30,
                    Show = false,
                    ExceedFormatter = (text, max) => text.Substring(0, max)
                },
                ClassName = new List<string> { "binding-leftview-input" },
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UIInput.TextProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.InputText,
                        Mode = BindingMode.TwoWay
                    },
                    new()
                    {
                        TargetProperty = UIInput.InputTextColorProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.Color
                    },
                    new()
                    {
                        TargetProperty = UIInput.InputFocusBorderColorProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.Color
                    }
                }
            },
            new UILabel
            {
                Text = "UIButton.TextProperty",
                ClassName = new List<string> { "binding-code-hint" }
            },
            new UIButton
            {
                ClassName = new List<string>
                {
                    "binding-action-btn",
                    "binding-action-default"
                },
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UIButton.TextProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.Name
                    }
                },
                Events = new()
                {
                    Click = _ =>
                    {
                        if (BindingContext is BindingDemoViewModel model)
                            model.ToggleBoundStyle();
                    }
                }
            });
    }

    /// <summary>演示父级 BindingContext 继承，以及嵌套对象替换后的路径重订阅。</summary>
    private UIView CreateContextCard()
    {
        return Card(
            "BindingContext 与嵌套路径",
            "子控件自动继承页面 BindingContext；替换 Context 或 Profile 后，活动绑定会重新解析并解除旧订阅。",
            new UILabel
            {
                Text = "vm.Profile.DisplayName",
                ClassName = new List<string> { "binding-code-hint" }
            },
            new UILabel
            {
                ClassName = new List<string> { "binding-result", "binding-result-success" },
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UILabel.TextProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.Profile.DisplayName,
                        TargetNullValue = "(profile is null)"
                    }
                }
            },
            new UIView
            {
                ClassName = new List<string> { "binding-action-row" },
                Children = new()
                {
                    ActionButton(
                        "替换 BindingContext",
                        "binding-action-primary",
                        () =>
                        {
                            _contextVersion++;
                            BindingContext = BindingDemoData.CreateModel(_contextVersion);
                        }),
                    ActionButton(
                        "替换嵌套 Profile",
                        "binding-action-default",
                        () =>
                        {
                            if (BindingContext is not BindingDemoViewModel model)
                                return;

                            model.Profile = new BindingDemoProfile
                            {
                                DisplayName =$"Profile replaced at {DateTime.Now:HH:mm:ss}"
                            };
                        })
                }
            });
    }

    /// <summary>演示 OneTime 在当前 Source 上只读取一次的行为。</summary>
    private static UIView CreateOneTimeCard()
    {
        return Card(
            "OneTime 初始快照",
            "OneTime 在当前 Source 上只读取一次；BindingContext 实例替换时会为新 Source 重新求值。",
            new UILabel
            {
                Text = "Mode = BindingMode.OneTime",
                ClassName = new List<string> { "binding-code-hint" }
            },
            new UILabel
            {
                ClassName = new List<string> { "binding-result", "binding-result-neutral" },
                Binding = new()
                {
                    new()
                    {
                        TargetProperty = UILabel.TextProperty,
                        SourceProperty = (BindingDemoViewModel vm) => vm.InputText,
                        Mode = BindingMode.OneTime,
                        StringFormat = "OneTime 初始快照：{0}"
                    }
                }
            });
    }

    /// <summary>创建带标题的行布局，避免控件案例重复声明相同容器结构。</summary>
    private static UIView ControlRow(string title, UIElement control)
    {
        return new UIView
        {
            ClassName = new List<string> { "binding-control-row" },
            Children = new()
            {
                new UILabel
                {
                    Text = title,
                    ClassName = new List<string> { "binding-control-label" }
                },
                new UIView
                {
                    ClassName = new List<string> { "binding-control-slot" },
                    Children = new() { control }
                }
            }
        };
    }

    /// <summary>创建统一的 Demo 卡片。</summary>
    private static UIView Card(
        string title,
        string description,
        params UIElement[] children)
    {
        var showcase = new UIView
        {
            ClassName = new List<string> { "binding-showcase" }
        };
        foreach (var child in children)
            showcase.AddChild(child);

        return new UIView
        {
            ClassName = new List<string> { "binding-demo-card" },
            Children = new()
            {
                new UILabel
                {
                    Text = title,
                    ClassName = new List<string> { "binding-card-title" }
                },
                new UILabel
                {
                    Text = description,
                    ClassName = new List<string> { "binding-card-desc" }
                },
                showcase,
            }
        };
    }

    /// <summary>创建统一样式的操作按钮。</summary>
    private static UIButton ActionButton(
        string text,
        string variantClass,
        Action action)
    {
        return new UIButton
        {
            Text = text,
            ClassName = new List<string>
            {
                "binding-action-btn",
                variantClass
            },
            Events = new()
            {
                Click = _ => action()
            }
        };
    }
}
