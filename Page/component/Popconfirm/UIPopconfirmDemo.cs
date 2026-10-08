using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Popconfirm;
using TCYM.UI.Elements.Tooltip;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Popconfirm
{
    /// <summary>Popconfirm 气泡确认框示例页。</summary>
    internal sealed class UIPopconfirmDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Popconfirm.style.css";

        internal UIPopconfirmDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = "popconfirm-demo-view";
            Children =
            [
                new UILabel
                {
                    Text = "Popconfirm 气泡确认框",
                    ClassName = "popconfirm-demo-title",
                },
                new UILabel
                {
                    Text = "点击目标元素，在附近弹出轻量确认操作。",
                    ClassName = "popconfirm-demo-title-sub",
                },
                new UILabel
                {
                    Text = "支持标题与描述、十二向定位、确认/取消、自定义图标、禁用、受控显隐，以及自动等待异步确认。",
                    Wrap = true,
                    ClassName = "popconfirm-demo-desc",
                },
                new BasicSection(),
                new PlacementSection(),
                new AsyncSection(),
                new CustomStyleSection(),
                new ControlledAndCustomSection(),
            ];
        }

        private sealed class BasicSection : UIView
        {
            internal BasicSection()
            {
                var state = CreateStateLabel("等待操作");
                var deleteConfirm = new UIPopconfirm
                {
                    ClassName = "popconfirm-demo-instance",
                    Title = "确定删除这条任务吗？",
                    Description = "删除后无法恢复。",
                    OkText = "删除",
                    CancelText = "保留",
                    PopupWidth = 180,
                    OkType = PopconfirmOkType.Danger,
                    Placement = TooltipPlacement.Top,
                    OnConfirm = () => state.Text = "已确认删除",
                    OnCancel = () => state.Text = "已取消删除",
                    Children = new()
                    {
                        CreateTriggerButton("删除任务", danger: true)
                    }
                };

                var quickConfirm = new UIPopconfirm
                {
                    ClassName = "popconfirm-demo-instance",
                    Title = "立即发布当前版本？",
                    ShowCancel = false,
                    OkText = "知道了",
                    PopupWidth = 180,
                    Placement = TooltipPlacement.Right,
                    OnConfirm = () => state.Text = "已执行单按钮确认",
                    Children = new()
                    {
                        CreateTriggerButton("单按钮确认")
                    }
                };

                Children =
                [
                    CreateSectionTitle("基础用法"),
                    CreateSectionDescription("Popconfirm 默认由点击触发；确认与取消会自动关闭气泡，并触发各自回调。ShowCancel=false 可隐藏取消按钮。"),
                    new UIView
                    {
                        ClassName = "popconfirm-showcase",
                        Children = [deleteConfirm, quickConfirm],
                    },
                    state,
                ];
                ClassName = "popconfirm-demo-card";
            }
        }

        private sealed class PlacementSection : UIView
        {
            internal PlacementSection()
            {
                var placements = new (TooltipPlacement Placement, string Text)[]
                {
                    (TooltipPlacement.TopLeft, "TL"),
                    (TooltipPlacement.Top, "Top"),
                    (TooltipPlacement.TopRight, "TR"),
                    (TooltipPlacement.LeftTop, "LT"),
                    (TooltipPlacement.Left, "Left"),
                    (TooltipPlacement.LeftBottom, "LB"),
                    (TooltipPlacement.RightTop, "RT"),
                    (TooltipPlacement.Right, "Right"),
                    (TooltipPlacement.RightBottom, "RB"),
                    (TooltipPlacement.BottomLeft, "BL"),
                    (TooltipPlacement.Bottom, "Bottom"),
                    (TooltipPlacement.BottomRight, "BR"),
                };
                var grid = new UIView { ClassName = "popconfirm-placement-grid" };
                foreach (var item in placements)
                {
                    var popconfirm = new UIPopconfirm
                    {
                        Title = $"{item.Text} 方位确认",
                        Description = "空间不足时会自动翻转或平移。",
                        Placement = item.Placement,
                        PopupWidth = 210,
                        OkText = "确认",
                    };
                    popconfirm.AddClass("popconfirm-demo-instance");
                    popconfirm.AddChild(CreateTriggerButton(item.Text, compact: true));
                    grid.AddChild(popconfirm);
                }

                Children =
                [
                    CreateSectionTitle("十二个方向", "label-purple"),
                    CreateSectionDescription("Placement 与 UITooltip 共用十二个方向；AutoAdjustOverflow 默认开启，靠近视口边缘时自动调整。"),
                    grid,
                ];
                ClassName = "popconfirm-demo-card";
            }
        }

        private sealed class AsyncSection : UIView
        {
            internal AsyncSection()
            {
                var state = CreateStateLabel("异步任务尚未开始");
                var asyncConfirm = new UIPopconfirm
                {
                    Title = "提交当前配置？",
                    Description = "确认后会等待回调完成，并阻止重复提交。",
                    OkText = "提交",
                    OkLoadingText = "提交中",
                    Placement = TooltipPlacement.Top,
                    OnConfirmAsync = async () =>
                    {
                        state.Text = "正在提交，请稍候…";
                        await Task.Delay(1200);
                        state.Text = "提交完成，气泡已自动关闭";
                    },
                    OnCancel = () => state.Text = "已取消提交",
                };
                asyncConfirm.AddClass("popconfirm-demo-instance");
                asyncConfirm.AddChild(CreateTriggerButton("异步确认"));

                Children =
                [
                    CreateSectionTitle("异步关闭", "label-green"),
                    CreateSectionDescription("OnConfirmAsync 会被组件等待；执行期间确认按钮自动禁用并显示旋转 loading，成功完成后再关闭。"),
                    new UIView
                    {
                        ClassName = "popconfirm-showcase",
                        Children = [asyncConfirm],
                    },
                    state,
                ];
                ClassName = "popconfirm-demo-card";
            }
        }

        private sealed class CustomStyleSection : UIView
        {
            internal CustomStyleSection()
            {
                var state = CreateStateLabel("等待应用自定义主题");
                var themed = new UIPopconfirm
                {
                    Title = "应用深色品牌主题？",
                    Description = "所有语义区域均可按实例设置 UIStyle。",
                    PopupWidth = 280,
                    Color = ColorHelper.ParseColor("#111827"),
                    IconColor = ColorHelper.ParseColor("#fbbf24"),
                    ArrowSize = 7,
                    PopupStyle = new DefaultUIStyle
                    {
                        PaddingLeft = 16,
                        PaddingRight = 16,
                        PaddingTop = 14,
                        PaddingBottom = 12,
                        BorderWidth = 1,
                        BorderColor = ColorHelper.ParseColor("#7c3aed"),
                        BorderRadius = 12,
                        BoxShadowOffsetY = 8,
                        BoxShadowBlur = 24,
                        BoxShadowColor = ColorHelper.ParseColor("rgba(76,29,149,0.35)"),
                    },
                    ArrowStyle = new UpdateUIStyle
                    {
                        Width = 16,
                        Height = 8,
                    },
                    ContentStyle = new UpdateUIStyle
                    {
                        Gap = 2,
                    },
                    MessageStyle = new UpdateUIStyle
                    {
                        Gap = 12,
                    },
                    IconStyle = new UpdateUIStyle
                    {
                        Width = 18,
                        Height = 20,
                    },
                    TitleStyle = new UpdateUIStyle
                    {
                        FontSize = 15,
                        FontWeight = 700,
                        Color = SKColors.White,
                    },
                    DescriptionStyle = new UpdateUIStyle
                    {
                        Color = ColorHelper.ParseColor("#cbd5e1"),
                    },
                    ActionsStyle = new UpdateUIStyle
                    {
                        Gap = 10,
                        MarginTop = 12,
                    },
                    CancelButtonStyle = new UpdateUIStyle
                    {
                        Color = ColorHelper.ParseColor("#e2e8f0"),
                        BackgroundColor = ColorHelper.ParseColor("#1f2937"),
                        BorderColor = ColorHelper.ParseColor("#475569"),
                    },
                    OkButtonStyle = new UpdateUIStyle
                    {
                        BackgroundColor = ColorHelper.ParseColor("#7c3aed"),
                        BorderColor = ColorHelper.ParseColor("#7c3aed"),
                        Color = SKColors.White,
                    },
                    OnConfirm = () => state.Text = "已应用深色品牌主题",
                    OnCancel = () => state.Text = "已保留默认主题",
                };
                themed.AddClass("popconfirm-demo-instance");
                themed.AddChild(CreateTriggerButton("自定义样式"));

                Children =
                [
                    CreateSectionTitle("自定义样式", "label-cany"),
                    CreateSectionDescription("通过 PopupStyle、ContentStyle、MessageStyle、IconStyle、TitleStyle、DescriptionStyle、ActionsStyle、OkButtonStyle、CancelButtonStyle 和 ArrowStyle 分别覆盖语义区域。"),
                    new UIView
                    {
                        ClassName = "popconfirm-showcase popconfirm-theme-showcase",
                        Children = [themed],
                    },
                    state,
                ];
                ClassName = "popconfirm-demo-card";
            }
        }

        private sealed class ControlledAndCustomSection : UIView
        {
            internal ControlledAndCustomSection()
            {
                var state = CreateStateLabel("受控状态：关闭");
                UIPopconfirm? controlled = null;
                controlled = new UIPopconfirm
                {
                    Open = false,
                    Title = "这是受控 Popconfirm",
                    Description = "OnOpenChange 将下一状态同步回 Open。",
                    Placement = TooltipPlacement.Bottom,
                    OnOpenChange = next =>
                    {
                        controlled!.Open = next;
                        state.Text = next ? "受控状态：打开" : "受控状态：关闭";
                    },
                    OnConfirm = () => state.Text = "受控确认已执行",
                };
                controlled.AddClass("popconfirm-demo-instance");
                controlled.AddChild(CreateTriggerButton("受控模式"));

                var custom = new UIPopconfirm
                {
                    TitleContent = new UILabel
                    {
                        Text = "覆盖现有草稿？",
                        ClassName = "popconfirm-custom-title",
                    },
                    DescriptionContent = new UILabel
                    {
                        Text = "自定义标题、描述、Icon 和危险按钮可以组合使用。",
                        Wrap = true,
                        ClassName = "popconfirm-custom-description",
                    },
                    Icon = new UILabel
                    {
                        Text = "!",
                        TextAlign = "center",
                        ClassName = "popconfirm-custom-icon",
                    },
                    OkType = PopconfirmOkType.Danger,
                    OkText = "覆盖",
                    Placement = TooltipPlacement.TopRight,
                    OnConfirm = () => state.Text = "已覆盖草稿",
                };
                custom.AddClass("popconfirm-demo-instance");
                custom.AddChild(CreateTriggerButton("自定义内容"));

                var disabled = new UIPopconfirm
                {
                    Title = "不会显示",
                    Disabled = true,
                };
                disabled.AddClass("popconfirm-demo-instance");
                disabled.AddChild(CreateTriggerButton("禁用状态", muted: true));

                Children =
                [
                    CreateSectionTitle("受控、自定义与禁用", "label-orange"),
                    CreateSectionDescription("Open/OnOpenChange 支持完全受控；TitleContent、DescriptionContent、Icon 和按钮样式可替换默认语义区域；Disabled 会阻止打开。"),
                    new UIView
                    {
                        ClassName = "popconfirm-showcase",
                        Children = [controlled, custom, disabled],
                    },
                    state,
                ];
                ClassName = "popconfirm-demo-card";
            }
        }

        private static UILabel CreateSectionTitle(string text, string accentClass = "label-title")
        {
            return new UILabel
            {
                Text = text,
                ClassName = new() { "popconfirm-card-title", accentClass },
            };
        }

        private static UILabel CreateSectionDescription(string text)
        {
            return new UILabel
            {
                Text = text,
                Wrap = true,
                ClassName = "popconfirm-card-desc",
            };
        }

        private static UILabel CreateStateLabel(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = "popconfirm-state-label",
            };
        }

        private static UIButton CreateTriggerButton( string text,bool danger = false,bool compact = false,bool muted = false)
        {
            var classes = new List<string> { "popconfirm-trigger-button" };
            if (danger) classes.Add("popconfirm-trigger-danger");
            if (compact) classes.Add("popconfirm-trigger-compact");
            if (muted) classes.Add("popconfirm-trigger-muted");
            return new UIButton
            {
                Text = text,
                ClassName = classes,
            };
        }
    }
}
