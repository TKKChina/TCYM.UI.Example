using TCYM.UI.Core;
using TCYM.UI.Elements;

namespace TCYM.UI.Example.Page.component.Label;

internal partial class UILabelDemo
{
    /// <summary>展示显式授权的文字选择、跨标签区域与空白起拖，并保留默认禁选和动态策略示例。</summary>
    private static UIView CreateUserSelectSection()
    {
        var text = CreateSelectionSample("label-user-select-text", "text：从这里拖动，只选择需要的文字。", "label-select-text");
        var none = CreateSelectionSample("label-user-select-none", "none：这段文字用于说明，拖动不会开始选区。", "label-select-none");
        var all = CreateSelectionSample("label-user-select-all", "all：选中其中一部分时，当前标签的完整文字一起选中。", "label-select-all");
        var unlocked = CreateSelectionSample("label-user-select-child-text", "子级 text：即使父容器是 none，这一段仍可拖动选择。", "label-select-text");
        var dynamic = CreateSelectionSample("label-user-select-dynamic", "动态示例：点击下方按钮，切换这段文字是否可选。");
        dynamic.Style = new DefaultUIStyle { UserSelect = "none" };
        var regionFirst = new UILabel
        {
            Id = "label-user-select-region-first",
            Text = "第一段：这是同一选择区域里的第一个 UILabel。可以从右侧留白开始，向下面继续拖动。",
            ClassName = "label-selection-region-line",
        };
        var regionSecond = new UILabel
        {
            Id = "label-user-select-region-second",
            Text = "第二段：这是另一个 UILabel，文字会随区域宽度换行。拖动时可以跨过段落之间的间隙，把多个标签中的文字连成一个选区。",
            ClassName = "label-selection-region-line",
        };
        var regionLast = new UILabel
        {
            Id = "label-user-select-region-last",
            Text = "第三段：中间灰色 none 提示不参与选择。松开鼠标后留在区域内可保留选区；移出整个区域、点击区域外或窗口失焦后清除。",
            ClassName = "label-selection-region-line",
        };
        var region = new UIView
        {
            Id = "label-user-select-region",
            ClassName = "label-selection-region",
            Children =
            [
                new UILabel { Text = "容器 user-select: text · 从文字、边距或段间空白开始拖动", ClassName = "label-preview-title label-select-none" },
                regionFirst,
                new UIView { Id = "label-user-select-region-gap", ClassName = "label-selection-region-gap" },
                regionSecond,
                new UILabel
                {
                    Id = "label-user-select-region-excluded",
                    Text = "这是一段 user-select: none 提示：跨过它时不纳入选中文字。",
                    ClassName = "label-selection-region-excluded",
                },
                regionLast,
            ],
        };
        var status = new UILabel
        {
            Id = "label-user-select-status",
            Text = "动态策略：none（不可选择）。先启用选择，再在预览文字上拖动。",
            ClassName = "label-selection-status",
        };
        var selected = new UILabel
        {
            Id = "label-user-select-result",
            Text = "当前无选区。拖动时在这里实时查看各 UILabel.SelectedText。",
            ClassName = "label-selection-result",
        };
        bool enabled = false;
        var toggle = new UIButton
        {
            Id = "label-user-select-toggle",
            Text = "启用动态示例选择",
            ClassName = "label-selection-button",
        };
        toggle.OnClick += _ =>
        {
            enabled = !enabled;
            dynamic.UpdateStyle(new UpdateUIStyle { UserSelect = enabled ? "text" : "none" });
            toggle.Text = enabled ? "禁用动态示例选择" : "启用动态示例选择";
            status.Text = enabled ? "动态策略：text（允许拖动选择）。" : "动态策略：none（不再开始新选区）。";
        };
        var selectable = new[] { text, all, unlocked, dynamic, regionFirst, regionSecond, regionLast };

        var section = CreateSectionCard(
            "CSS user-select：文本选择",
            "普通控件默认不选择文字。text / all / contain 显式开启选择；下方共享 text 容器演示跨多个 UILabel 与从区域空白起拖，灰色 none 子项不参与选区。",
            new UIView
            {
                ClassName = "label-selection-grid",
                Children =
                [
                    CreateSelectionCard("user-select: text", "显式允许选择；父级 none 不会封锁这里的 text。", text),
                    CreateSelectionCard("user-select: none", "禁用文本选择不等于禁用点击，也不是防复制机制。", none),
                    CreateSelectionCard("user-select: all", "选区原子化为当前标签的完整 Text。", all),
                    CreateSelectionCard("父 none / 子 text", "上方普通子级继承 auto 的限制，下方显式 text 解除限制。",
                        new UIView
                        {
                            ClassName = "label-selection-parent-none",
                            Children =
                            [
                                new UILabel { Text = "父容器 none：本行 auto，不可开始选择。", ClassName = "label-selection-muted" },
                                unlocked,
                            ],
                        }),
                    CreateSelectionCard("UpdateUIStyle 动态切换", "按钮只更新 UserSelect，不改动文字内容与布局。", dynamic),
                ],
            },
            region,
            new SelectionFeedbackPanel(selectable, selected)
            {
                Children =
                [
                    new UIView { ClassName = "label-selection-actions", Children = [toggle] },
                    status,
                    selected,
                ],
            },
            new UILabel
            {
                Text = ".selection-region { user-select: text; }\n.selection-region .decoration { user-select: none; }\n.copy-all { user-select: all; }\n\nlabel.UpdateStyle(new UpdateUIStyle { UserSelect = \"text\" });",
                ClassName = "label-flow-code label-select-text",
            });
        section.Id = "label-user-select-section";
        return section;
    }

    /// <summary>创建不附带点击处理器的真实选择目标，策略由 CSS 或 UIStyle 独立提供。</summary>
    private static UILabel CreateSelectionSample(string id, string text, string? policyClass = null) => new()
    {
        Id = id,
        Text = text,
        ClassName = policyClass == null ? "label-selection-sample" : "label-selection-sample " + policyClass,
    };

    /// <summary>复用文本示例卡片外观，将操作说明与选择目标分开，避免误选标题。</summary>
    private static UIView CreateSelectionCard(string title, string note, UIElement sample) => new()
    {
        ClassName = "label-selection-card",
        Children =
        [
            new UILabel { Text = title, ClassName = "label-preview-title label-select-none" },
            new UILabel { Text = note, ClassName = "label-selection-note" },
            sample,
        ],
    };

    /// <summary>无需移出选区点击按钮，逐帧读取公开选中文字；内容未变化时不重复更新布局。</summary>
    private sealed class SelectionFeedbackPanel : UIView
    {
        private readonly IReadOnlyList<UILabel> _labels;
        private readonly UILabel _output;

        /// <summary>关联真实选择目标，不给标签附加会改变默认选择策略的点击处理器。</summary>
        internal SelectionFeedbackPanel(IReadOnlyList<UILabel> labels, UILabel output)
        {
            _labels = labels;
            _output = output;
            ClassName = "label-selection-feedback";
        }

        /// <summary>按示例阅读顺序组合各标签选中片段，松开后移出区域时同步显示清除结果。</summary>
        public override void Update(float deltaSeconds)
        {
            base.Update(deltaSeconds);
            string selection = string.Join(" | ", _labels.Where(label => label.SelectionLength > 0).Select(label => label.SelectedText));
            string text = selection.Length == 0 ? "当前无选区（默认禁选；离开选择区域后会清除）。" : "SelectedText：" + selection;
            if (_output.Text != text) _output.Text = text;
        }
    }
}
