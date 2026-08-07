using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Anchor;

namespace TCYM.UI.Example.Page.component.Anchor
{
    internal class UIAnchorDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Anchor.style.css";

        internal UIAnchorDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            EnableDeferredChildLoading = false;
            ClassName = new List<string> { "anchor-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Anchor 锚点",
                    ClassName = new List<string> { "anchor-demo-title" },
                },
                new UILabel
                {
                    Text = "用于跳转到滚动区域内的指定位置。",
                    ClassName = new List<string> { "anchor-demo-subtitle" },
                },
                new UILabel
                {
                    Text = "UIAnchor 通过组件 Id 查找目标，只在自身存在 UIScrollView 父级或祖先级时生效。嵌套滚动时会先滚动目标所在的内层 UIScrollView，再滚动与 Anchor 最近的共同外层 UIScrollView。",
                    ClassName = new List<string> { "anchor-demo-desc" },
                },
                new BasicSection(),
                new CustomStyleSection(),
                new NestedScrollSection(),
            };
        }

        private sealed class BasicSection : UIView
        {
            internal BasicSection()
            {
                var status = new UILabel
                {
                    Text = "当前锚点：anchor-basic-overview",
                    ClassName = new List<string> { "anchor-demo-status" },
                };

                var anchor = new UIAnchor
                {
                    Affix = true,
                    TargetOffset = 12,
                    Bounds = 8,
                    Items = new List<AnchorItem>
                    {
                        new("anchor-basic-overview", "基本介绍"),
                        new("anchor-basic-when", "何时使用"),
                        new("anchor-basic-api", "API")
                        {
                            Children = new List<AnchorItem>
                            {
                                new("anchor-basic-props", "Anchor Props"),
                                new("anchor-basic-token", "Design Token"),
                            },
                        },
                    },
                    OnChange = id =>
                    {
                        status.Text = $"当前锚点：{id ?? "无"}";
                        status.RequestRedraw();
                    },
                };

                var content = new UIView
                {
                    ClassName = new List<string> { "anchor-basic-content" },
                    Children = new()
                    {
                        CreateTarget("anchor-basic-overview", "基本介绍", "点击右侧锚点后，组件会通过 UIManager 的 Id 注册表找到这一节，并将它对齐到滚动视口顶部。", "#e6f4ff"),
                        CreateTarget("anchor-basic-when", "何时使用", "当内容较长、需要展示当前位置并在多个章节间快速跳转时使用。滚动时活动指示器会随章节自动更新。", "#f6ffed"),
                        CreateTarget("anchor-basic-api", "API", "Items 支持树形层级；ComponentId 指向目标，TargetOffset 可以控制滚动完成后的顶部留白。", "#fffbe6"),
                        CreateTarget("anchor-basic-props", "Anchor Props", "Bounds 控制高亮切换边界，Affix 用于在最近的滚动视口内保持锚点位置。", "#fff2e8"),
                        CreateTarget("anchor-basic-token", "Design Token", "可通过 LinkColor、ActiveColor、RailColor、缩进和指示器尺寸调整视觉样式。", "#f9f0ff"),
                    },
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "基本用法",
                        ClassName = new List<string> { "anchor-card-title" },
                    },
                    new UILabel
                    {
                        Text = "Anchor 与目标位于同一个 UIScrollView 中；右侧导航使用 Affix 保持在该滚动视口内。",
                        ClassName = new List<string> { "anchor-card-desc" },
                    },
                    status,
                    new UIScrollView
                    {
                        EnableDeferredChildLoading = false,
                        ClassName = new List<string> { "anchor-basic-stage" },
                        Children = new() { content, anchor },
                    },
                };

                ClassName = new List<string> { "anchor-demo-card" };
            }
        }

        private sealed class CustomStyleSection : UIView
        {
            internal CustomStyleSection()
            {
                var anchor = new UIAnchor
                {
                    ClassName = "anchor-custom-nav",
                    Affix = true,
                    TargetOffset = 12,
                    Bounds = 8,
                    ItemHeight = 36,
                    ItemPaddingStart = 18,
                    IndicatorWidth = 8,
                    IndicatorHeight = 8,
                    RailColor = Helpers.ColorHelper.ParseColor("#d9e2f2"),
                    ActiveColor = Helpers.ColorHelper.ParseColor("#5b8ff9"),
                    Items = new List<AnchorItem>
                    {
                        new("anchor-custom-progress", "项目进展"),
                        new("anchor-custom-info", "基本信息"),
                        new("anchor-custom-properties", "项目属性"),
                        new("anchor-custom-investment", "投资概况"),
                        new("anchor-custom-completed", "完成情况"),
                        new("anchor-custom-stage-target", "项目阶段信息")
                        {
                            ClassName = new List<string> { "anchor-custom-stage-link" },
                        },
                    },
                };

                var content = new UIView
                {
                    ClassName = "anchor-custom-content",
                    Children = new()
                    {
                        CreateTarget("anchor-custom-progress", "项目进展", "活动项通过 .UIAnchor-link-active 设置胶囊背景和文字样式。", "#f7faff"),
                        CreateTarget("anchor-custom-info", "基本信息", "在 UIAnchor 根节点添加 ClassName，便于用后代选择器限制样式作用域。", "#ffffff"),
                        CreateTarget("anchor-custom-properties", "项目属性", "所有链接都包含 .UIAnchor-link 基础类。", "#f9fbff"),
                        CreateTarget("anchor-custom-investment", "投资概况", "禁用项会自动追加 .UIAnchor-link-disabled。", "#ffffff"),
                        CreateTarget("anchor-custom-completed", "完成情况", "圆角、背景、字号、间距和悬停态均可通过 CSS 覆盖。", "#f9fbff"),
                        CreateTarget("anchor-custom-stage-target", "项目阶段信息", "AnchorItem.ClassName 可为单个锚点追加自定义类。", "#ffffff"),
                    },
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义 Class 样式",
                        ClassName = "anchor-card-title",
                    },
                    new UILabel
                    {
                        Text = "根组件 ClassName 可限定样式作用域；AnchorItem.ClassName 可定制单个链接。活动与禁用状态拥有固定状态类。",
                        ClassName = "anchor-card-desc",
                    },
                    new UIScrollView
                    {
                        EnableDeferredChildLoading = false,
                        ClassName = "anchor-custom-scroll",
                        Children = new() { content, anchor },
                    },
                };

                ClassName = "anchor-demo-card";
            }
        }

        private sealed class NestedScrollSection : UIView
        {
            internal NestedScrollSection()
            {
                var status = new UILabel
                {
                    Text = "嵌套链路：等待选择",
                    ClassName = new List<string> { "anchor-demo-status" },
                };

                var anchor = new UIAnchor
                {
                    Affix = true,
                    TargetOffset = 10,
                    Items = new List<AnchorItem>
                    {
                        new("anchor-nested-one", "内层章节一"),
                        new("anchor-nested-two", "内层章节二"),
                        new("anchor-nested-three", "内层章节三"),
                        new("anchor-nested-four", "内层章节四"),
                    },
                    OnChange = id =>
                    {
                        status.Text = $"嵌套链路：{id ?? "无"}";
                        status.RequestRedraw();
                    },
                };

                var innerContent = new UIView
                {
                    ClassName = new List<string> { "anchor-inner-content" },
                    Children = new()
                    {
                        CreateTarget("anchor-nested-one", "内层章节一", "目标先在最内层 UIScrollView 中对齐。", "#e6f4ff"),
                        CreateTarget("anchor-nested-two", "内层章节二", "完成内层滚动后，再根据更新后的全局坐标处理外层滚动。", "#f6ffed"),
                        CreateTarget("anchor-nested-three", "内层章节三", "每一层都独立执行边界限制，不会把滚动量错误累加到无关容器。", "#fffbe6"),
                        CreateTarget("anchor-nested-four", "内层章节四", "到达内容底部时，最后一个锚点仍会正确保持高亮。", "#f9f0ff"),
                    },
                };

                var nestedRow = new UIView
                {
                    ClassName = new List<string> { "anchor-nested-row" },
                    Children = new()
                    {
                        new UIScrollView
                        {
                            EnableDeferredChildLoading = false,
                            ClassName = new List<string> { "anchor-inner-scroll" },
                            Children = new() { innerContent },
                        },
                        anchor,
                    },
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "多层 UIScrollView",
                        ClassName = new List<string> { "anchor-card-title" },
                    },
                    new UILabel
                    {
                        Text = "Anchor 位于外层 UIScrollView，目标位于右侧内层 UIScrollView。点击会按“内层 → 外层”的顺序完成定位。",
                        ClassName = new List<string> { "anchor-card-desc" },
                    },
                    status,
                    new UIScrollView
                    {
                        EnableDeferredChildLoading = false,
                        ClassName = new List<string> { "anchor-nested-scroll" },
                        Children = new()
                        {
                            new UIView { ClassName = new List<string> { "anchor-nested-spacer" } },
                            nestedRow,
                            new UILabel
                            {
                                Text = "外层滚动区域尾部：用于验证共同外层 UIScrollView 的边界处理。",
                                ClassName = new List<string> { "anchor-nested-footer" },
                            },
                        },
                    },
                };

                ClassName = new List<string> { "anchor-demo-card" };
            }
        }

        private static UIView CreateTarget(string id, string title, string description, string background)
        {
            return new UIView
            {
                Id = id,
                Style = new DefaultUIStyle
                {
                    BackgroundColor = Helpers.ColorHelper.ParseColor(background),
                },
                ClassName = new List<string> { "anchor-target" },
                Children = new()
                {
                    new UILabel
                    {
                        Text = title,
                        ClassName = new List<string> { "anchor-target-title" },
                    },
                    new UILabel
                    {
                        Text = description,
                        Wrap = true,
                        ClassName = new List<string> { "anchor-target-desc" },
                    },
                },
            };
        }
    }
}
