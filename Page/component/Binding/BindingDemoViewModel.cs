using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SkiaSharp;
using TCYM.UI.Binding;
using TCYM.UI.Elements.Select;
using TCYM.UI.Elements.Tree;

namespace TCYM.UI.Example.Page.component.Binding;

/// <summary>
/// Binding 示例页的独立 ViewModel。
/// 属性按案例分组，所有 UI 动作只修改这里的状态，再由 Binding 驱动组件更新。
/// </summary>
internal sealed class BindingDemoViewModel : ObservableObject
{
    // LeftView 组合绑定与嵌套路径案例。
    private string _name = "TCYM";
    private string _inputText = string.Empty;
    private List<string> _classNames = new() { "binding-leftview-label" };
    private BindingDemoProfile _profile = new();
    private bool _alternateStyle;

    // 自动属性与公共 UIElement 属性案例。
    private float _progress = 35f;
    private float _controlWidth = 220f;
    private DateTime? _selectedDate = DateTime.Today;
    private int _appearanceIndex;
    private float _appearanceWidth = 360f;
    private float _appearanceHeight = 72f;
    private float _appearanceRadius = 12f;
    private float _appearanceOpacity = 1f;
    private SKColor _appearanceBackground = new(230, 244, 255);
    private SKColor _appearanceForeground = new(9, 88, 217);
    private SKColor _appearanceBorder = new(145, 202, 255);
    private string _appearanceText = "公共属性由 ViewModel 驱动";

    // 交互组件 TwoWay 案例。
    private bool _enabled = true;
    private bool _accepted;
    private float _volume = 45f;
    private object? _status = "处理中";
    private List<object?> _statusOptions = new();

    // Select 与 Pagination 案例。
    private object? _selectedRole = "developer";
    private List<SelectOption> _roleOptions = new();
    private int _currentPage = 2;
    private int _pageSize = 10;
    private int _totalItems = 86;

    // 集合与 Explicit 案例。
    private ObservableCollection<TreeNode> _treeNodes = new();
    private bool _treeShowLine = true;
    private int _treeVersion = 1;
    private string _explicitText = "显式提交值";

    /// <summary>创建 ViewModel，并开始监听根节点集合变化。</summary>
    internal BindingDemoViewModel()
    {
        _treeNodes.CollectionChanged += HandleTreeCollectionChanged;
    }

    /// <summary>按钮显示名称。</summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>输入框文本；变化时同步通知依赖的颜色属性。</summary>
    public string InputText
    {
        get => _inputText;
        set
        {
            if (SetProperty(ref _inputText, value))
            {
                RaisePropertyChanged(nameof(Color));
                RaisePropertyChanged(nameof(InputDisplayText));
                RefreshBoundLabelClasses();
            }
        }
    }

    /// <summary>
    /// 输入为空时提供明确的占位说明，避免绑定结果 Label 变成没有内容的彩色空白块。
    /// </summary>
    public string InputDisplayText =>
        string.IsNullOrEmpty(InputText)
            ? "ViewModel.Text：<空字符串>"
            : InputText;

    /// <summary>根据输入长度计算文字、边框和进度颜色。</summary>
    public SKColor Color
    {
        get
        {
            int length = string.IsNullOrEmpty(InputText) ? 0 : InputText.Length;
            return length switch
            {
                0 => SKColors.Black,
                1 => SKColors.Red,
                2 => SKColors.Green,
                3 => SKColors.Blue,
                4 => SKColors.Orange,
                _ => SKColors.Purple
            };
        }
    }

    /// <summary>动态 CSS 类名集合。</summary>
    public List<string> ClassNames
    {
        get => _classNames;
        set => SetProperty(ref _classNames, value);
    }

    /// <summary>嵌套属性路径案例的数据对象。</summary>
    public BindingDemoProfile Profile
    {
        get => _profile;
        set => SetProperty(ref _profile, value);
    }

    /// <summary>Progress 当前进度。</summary>
    public float Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    /// <summary>绑定到 UIButton.WidthProperty 的公共布局属性。</summary>
    public float ControlWidth
    {
        get => _controlWidth;
        set => SetProperty(ref _controlWidth, value);
    }

    /// <summary>DatePicker 当前日期。</summary>
    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set => SetProperty(ref _selectedDate, value);
    }

    /// <summary>公共属性预览区域宽度。</summary>
    public float AppearanceWidth
    {
        get => _appearanceWidth;
        set => SetProperty(ref _appearanceWidth, value);
    }

    /// <summary>公共属性预览区域高度。</summary>
    public float AppearanceHeight
    {
        get => _appearanceHeight;
        set => SetProperty(ref _appearanceHeight, value);
    }

    /// <summary>公共属性预览区域圆角。</summary>
    public float AppearanceRadius
    {
        get => _appearanceRadius;
        set => SetProperty(ref _appearanceRadius, value);
    }

    /// <summary>公共属性预览区域透明度。</summary>
    public float AppearanceOpacity
    {
        get => _appearanceOpacity;
        set => SetProperty(ref _appearanceOpacity, value);
    }

    /// <summary>公共属性预览区域背景色。</summary>
    public SKColor AppearanceBackground
    {
        get => _appearanceBackground;
        set => SetProperty(ref _appearanceBackground, value);
    }

    /// <summary>公共属性预览区域前景色。</summary>
    public SKColor AppearanceForeground
    {
        get => _appearanceForeground;
        set => SetProperty(ref _appearanceForeground, value);
    }

    /// <summary>公共属性预览区域边框色。</summary>
    public SKColor AppearanceBorder
    {
        get => _appearanceBorder;
        set => SetProperty(ref _appearanceBorder, value);
    }

    /// <summary>公共属性预览区域文字。</summary>
    public string AppearanceText
    {
        get => _appearanceText;
        set => SetProperty(ref _appearanceText, value);
    }

    /// <summary>Switch 的 TwoWay 状态。</summary>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (SetProperty(ref _enabled, value))
                RaisePropertyChanged(nameof(InteractiveSummary));
        }
    }

    /// <summary>Checkbox 的 TwoWay 状态。</summary>
    public bool Accepted
    {
        get => _accepted;
        set
        {
            if (SetProperty(ref _accepted, value))
                RaisePropertyChanged(nameof(InteractiveSummary));
        }
    }

    /// <summary>Slider 的 TwoWay 数值。</summary>
    public float Volume
    {
        get => _volume;
        set
        {
            if (SetProperty(ref _volume, value))
                RaisePropertyChanged(nameof(InteractiveSummary));
        }
    }

    /// <summary>Segmented 的 TwoWay 选中值。</summary>
    public object? Status
    {
        get => _status;
        set
        {
            if (SetProperty(ref _status, value))
                RaisePropertyChanged(nameof(InteractiveSummary));
        }
    }

    /// <summary>Segmented 的 OneWay 选项数据。</summary>
    public List<object?> StatusOptions
    {
        get => _statusOptions;
        set => SetProperty(ref _statusOptions, value);
    }

    /// <summary>汇总多个 TwoWay 控件当前写回的值。</summary>
    public string InteractiveSummary =>
        $"启用={Enabled}，已确认={Accepted}，音量={Volume:0}，状态={Status}";

    /// <summary>Select 的 OneWay 选项数据。</summary>
    public List<SelectOption> RoleOptions
    {
        get => _roleOptions;
        set
        {
            if (SetProperty(ref _roleOptions, value))
                RaisePropertyChanged(nameof(SelectionSummary));
        }
    }

    /// <summary>Select 的 TwoWay 选中值。</summary>
    public object? SelectedRole
    {
        get => _selectedRole;
        set
        {
            if (SetProperty(ref _selectedRole, value))
                RaisePropertyChanged(nameof(SelectionSummary));
        }
    }

    /// <summary>把 Select 当前值转换为用户可读说明。</summary>
    public string SelectionSummary
    {
        get
        {
            string label = RoleOptions
                .FirstOrDefault(option => Equals(option.Value, SelectedRole))
                ?.Label ?? "(未选择)";
            return $"当前职位：{label} / {SelectedRole}";
        }
    }

    /// <summary>Pagination 当前页。</summary>
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (SetProperty(ref _currentPage, Math.Max(1, value)))
                RaisePropertyChanged(nameof(PagingSummary));
        }
    }

    /// <summary>Pagination 每页条数。</summary>
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (SetProperty(ref _pageSize, Math.Max(1, value)))
                RaisePropertyChanged(nameof(PagingSummary));
        }
    }

    /// <summary>Pagination 数据总数。</summary>
    public int TotalItems
    {
        get => _totalItems;
        set
        {
            if (SetProperty(ref _totalItems, Math.Max(0, value)))
                RaisePropertyChanged(nameof(PagingSummary));
        }
    }

    /// <summary>分页状态说明。</summary>
    public string PagingSummary =>
        $"第 {CurrentPage} 页，每页 {PageSize} 条，共 {TotalItems} 条";

    /// <summary>
    /// Tree 根节点集合。替换集合时会重新连接 CollectionChanged，
    /// 用于同时演示集合原地变化和整个 Source 对象替换。
    /// </summary>
    public ObservableCollection<TreeNode> TreeNodes
    {
        get => _treeNodes;
        set
        {
            var next = value ?? new ObservableCollection<TreeNode>();
            if (ReferenceEquals(_treeNodes, next)) return;

            _treeNodes.CollectionChanged -= HandleTreeCollectionChanged;
            if (SetProperty(ref _treeNodes, next))
            {
                _treeNodes.CollectionChanged += HandleTreeCollectionChanged;
                RaisePropertyChanged(nameof(TreeSummary));
            }
        }
    }

    /// <summary>Tree 是否显示连接线。</summary>
    public bool TreeShowLine
    {
        get => _treeShowLine;
        set => SetProperty(ref _treeShowLine, value);
    }

    /// <summary>Tree 集合状态说明。</summary>
    public string TreeSummary =>
        $"当前根节点：{TreeNodes.Count}，数据版本：{_treeVersion}";

    /// <summary>Explicit 触发器案例的源值。</summary>
    public string ExplicitText
    {
        get => _explicitText;
        set => SetProperty(ref _explicitText, value);
    }

    /// <summary>同时更新多个自动生成的目标属性。</summary>
    internal void AdvanceAutomaticProperties()
    {
        Progress = Progress >= 90f ? 15f : Progress + 15f;
        ControlWidth = ControlWidth >= 320f ? 220f : ControlWidth + 25f;
        SelectedDate = (SelectedDate ?? DateTime.Today).AddDays(1);
    }

    /// <summary>轮换公共 UIElement 属性，验证布局和重绘失效策略。</summary>
    internal void CycleAppearance()
    {
        _appearanceIndex = (_appearanceIndex + 1) % 3;
        switch (_appearanceIndex)
        {
            case 1:
                AppearanceWidth = 430f;
                AppearanceHeight = 82f;
                AppearanceRadius = 24f;
                AppearanceOpacity = 0.88f;
                AppearanceBackground = new SKColor(246, 255, 237);
                AppearanceForeground = new SKColor(35, 120, 4);
                AppearanceBorder = new SKColor(183, 235, 143);
                AppearanceText = "尺寸、颜色、圆角和透明度已更新";
                break;
            case 2:
                AppearanceWidth = 320f;
                AppearanceHeight = 66f;
                AppearanceRadius = 6f;
                AppearanceOpacity = 0.72f;
                AppearanceBackground = new SKColor(255, 247, 230);
                AppearanceForeground = new SKColor(173, 78, 0);
                AppearanceBorder = new SKColor(255, 213, 145);
                AppearanceText = "再次更新：公共属性仍保持动态";
                break;
            default:
                AppearanceWidth = 360f;
                AppearanceHeight = 72f;
                AppearanceRadius = 12f;
                AppearanceOpacity = 1f;
                AppearanceBackground = new SKColor(230, 244, 255);
                AppearanceForeground = new SKColor(9, 88, 217);
                AppearanceBorder = new SKColor(145, 202, 255);
                AppearanceText = "公共属性由 ViewModel 驱动";
                break;
        }
    }

    /// <summary>重置所有交互组件，由源端反向更新目标控件。</summary>
    internal void ResetInteractiveControls()
    {
        Enabled = true;
        Accepted = false;
        Volume = 45f;
        Status = "处理中";
    }

    /// <summary>增加分页总数，演示 OneWay 源更新与 TwoWay 页码共存。</summary>
    internal void AddRecords()
    {
        TotalItems += 17;
    }

    /// <summary>恢复 Select 与 Pagination 案例的初始 Source 状态。</summary>
    internal void ResetSelectionAndPaging()
    {
        SelectedRole = "developer";
        CurrentPage = 2;
        PageSize = 10;
        TotalItems = 86;
    }

    /// <summary>向当前 ObservableCollection 原地追加一个 TreeNode。</summary>
    internal void AddTreeNode()
    {
        int index = TreeNodes.Count + 1;
        TreeNodes.Add(new TreeNode($"dynamic-{_treeVersion}-{index}", $"动态节点 {index}"));
    }

    /// <summary>替换整个树集合，触发 SourceProperty 重订阅。</summary>
    internal void ReplaceTreeNodes()
    {
        _treeVersion++;
        TreeNodes = BindingDemoData.CreateTreeNodes(_treeVersion);
    }

    /// <summary>切换一个普通自动生成的 Tree 属性。</summary>
    internal void ToggleTreeLine()
    {
        TreeShowLine = !TreeShowLine;
    }

    /// <summary>从源端重置 Explicit 案例。</summary>
    internal void ResetExplicitText()
    {
        ExplicitText = $"源端重置 {DateTime.Now:HH:mm:ss}";
    }

    /// <summary>切换 Label 的动态 ClassName 与按钮文字。</summary>
    internal void ToggleBoundStyle()
    {
        _alternateStyle = !_alternateStyle;
        RefreshBoundLabelClasses();
        Name = _alternateStyle
            ? "恢复默认绑定样式"
            : "切换绑定的 Name / ClassName";
    }

    /// <summary>
    /// 根据输入是否为空和当前主题状态重新生成绑定类名。
    /// 空值样式最后加入，使其优先于蓝色/橙色主题并保持可读性。
    /// </summary>
    private void RefreshBoundLabelClasses()
    {
        var classes = new List<string> { "binding-leftview-label" };
        if (_alternateStyle)
            classes.Add("binding-leftview-label-alt");
        if (string.IsNullOrEmpty(InputText))
            classes.Add("binding-leftview-label-empty");
        ClassNames = classes;
    }

    private void HandleTreeCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs args)
    {
        RaisePropertyChanged(nameof(TreeSummary));
    }
}

/// <summary>嵌套路径重订阅案例使用的 Profile 数据。</summary>
internal sealed class BindingDemoProfile : ObservableObject
{
    private string? _displayName;

    /// <summary>显示名称。</summary>
    public string? DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }
}
