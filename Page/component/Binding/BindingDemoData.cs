using System.Collections.ObjectModel;
using TCYM.UI.Binding;
using TCYM.UI.Elements.Select;
using TCYM.UI.Elements.Tree;

namespace TCYM.UI.Example.Page.component.Binding;

/// <summary>
/// 集中创建 Binding Demo 使用的初始数据。
/// 页面类只描述 UI，避免把选项、树节点和测试数据散落在组件初始化器中。
/// </summary>
internal static class BindingDemoData
{
    /// <summary>创建一套新的页面数据，用于演示运行时替换 BindingContext。</summary>
    /// <param name="version">上下文版本号，便于从页面上观察实例已经替换。</param>
    internal static BindingDemoViewModel CreateModel(int version)
    {
        return new BindingDemoViewModel
        {
            Name = "切换绑定的 Name / ClassName",
            InputText = $"Context #{version}",
            ClassNames = new List<string> { "binding-leftview-label" },
            Profile = new BindingDemoProfile
            {
                DisplayName = $"Nested profile #{version}"
            },
            RoleOptions = CreateRoleOptions(),
            StatusOptions = CreateStatusOptions(),
            TreeNodes = CreateTreeNodes(version),
            ExplicitText = $"显式提交值 #{version}"
        };
    }

    /// <summary>创建 Select 案例的职位选项。</summary>
    internal static List<SelectOption> CreateRoleOptions()
    {
        return
        [
            new SelectOption { Label = "开发工程师", Value = "developer" },
            new SelectOption { Label = "产品经理", Value = "product" },
            new SelectOption { Label = "设计师", Value = "designer" },
            new SelectOption { Label = "测试工程师", Value = "tester" }
        ];
    }

    /// <summary>创建 Segmented 案例的状态选项。</summary>
    internal static List<object?> CreateStatusOptions() =>
        ["待处理", "处理中", "已完成"];

    /// <summary>
    /// 创建 Tree 案例的数据。
    /// 每次替换都会带上版本号，用于确认源对象替换后目标组件已重建。
    /// </summary>
    internal static ObservableCollection<TreeNode> CreateTreeNodes(int version)
    {
        return
        [
            new TreeNode(
                $"project-{version}",
                $"Binding 项目 #{version}",
                [
                    new TreeNode($"runtime-{version}", "Runtime"),
                    new TreeNode($"generator-{version}", "Generator"),
                    new TreeNode($"example-{version}", "Example")
                ]),
            new TreeNode(
                $"validation-{version}",
                "验证",
                [
                    new TreeNode($"net8-{version}", ".NET 8"),
                    new TreeNode($"net10-{version}", ".NET 10")
                ])
        ];
    }
}

/// <summary>
/// 把 Slider 的数值转换为带百分号的展示文本。
/// ConvertBack 同样保留，便于该 Converter 在 TwoWay 场景中复用。
/// </summary>
internal sealed class BindingDemoPercentConverter : IBindingValueConverter
{
    internal static BindingDemoPercentConverter Instance { get; } = new();

    private BindingDemoPercentConverter()
    {
    }

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter)
    {
        float number = value == null
            ? 0f
            : global::System.Convert.ToSingle(value);
        return $"当前音量：{number:0}%";
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type sourceType, object? parameter)
    {
        string text = value?.ToString()?.Trim() ?? string.Empty;
        int separator = text.LastIndexOf('：');
        if (separator >= 0)
            text = text[(separator + 1)..];
        text = text.Trim().TrimEnd('%');
        return float.TryParse(text, out float number)
            ? number
            : BindingValue.Unset;
    }
}
