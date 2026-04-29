using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Select;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Select
{
  internal class UISelectDemo : UIScrollView
  {
    private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Select.style.css";

    internal UISelectDemo()
    {
        UISystem.LoadStyleFile(DemoCssPath);
        ClassName = new List<string> { "select-demo-view" };
        Children = new()
        {
            new UILabel
            {
                Text = "Select 选择器组件",
                ClassName = new List<string> { "select-demo-title" },
            },
            new UILabel
            {
                Text = "下拉选择器，当选项过多时使用。",
                ClassName = new List<string> { "select-demo-title-sub" },
            },
            new UILabel
            {
                Text = "弹出一个下拉菜单给用户选择操作，用于代替原生的选择器。支持单选、多选、搜索、虚拟滚动等多种模式。",
                ClassName = new List<string> { "select-demo-desc" },
            },
            new BasicSection(),
            new SearchSection(),
            new MultipleSection(),
            new CustomRenderSection(),
            new VirtualScrollSection(),
        };
    }

    private static List<SelectOption> CreateBasicOptions()
    {
        return new()
        {
            new SelectOption { Value = "option1", Label = "选项一" },
            new SelectOption { Value = "option2", Label = "选项二" },
            new SelectOption { Value = "option3", Label = "选项三" },
            new SelectOption { Value = "option4", Label = "选项四" },
        };
    }

    private class BasicSection : UIView
    {
      internal BasicSection()
      {
        ClassName = new List<string> { "select-demo-card" };

        Children = new()
        {
            new UILabel
            {
                Text = "基本使用",
                ClassName = new List<string> { "select-card-title", "label-title" }
            },
            new UILabel
            {
                Text = "基础的下拉选择器，包含默认、禁用和可清除三种状态。",
                ClassName = new List<string> { "select-card-desc" }
            },
            new UIView
            {
                ClassName = new List<string> { "select-showcase" },
                Children = new()
                {
                    new UISelect
                    {
                        Options = CreateBasicOptions(),
                        Placeholder = "请选择一个选项",
                        Style = new UpdateUIStyle { Width = 180, Height = 30 }
                    },
                    new UISelect
                    {
                        Disabled = true,
                        Placeholder = "禁用状态",
                        Style = new UpdateUIStyle { Width = 120, Height = 30 }
                    },
                    new UISelect
                    {
                        Options = CreateBasicOptions(),
                        SelectedValue = "option2",
                        AllowClear = true,
                        Style = new UpdateUIStyle { Width = 180, Height = 30 }
                    },
                }
            },
        };
      }
    }

    private class SearchSection : UIView
    {
      internal SearchSection()
      {
        ClassName = new List<string> { "select-demo-card" };

        Children = new()
        {
            new UILabel
            {
                Text = "带搜索",
                ClassName = new List<string> { "select-card-title", "label-green" }
            },
            new UILabel
            {
                Text = "开启 ShowSearch 后，可在下拉菜单中输入关键词快速筛选选项。",
                ClassName = new List<string> { "select-card-desc" }
            },
            new UIView
            {
                ClassName = new List<string> { "select-showcase" },
                Children = new()
                {
                    new UISelect
                    {
                        Options = CreateBasicOptions(),
                        Placeholder = "搜索并选择",
                        ShowSearch = true,
                        Style = new UpdateUIStyle { Width = 180, Height = 30 }
                    },
                }
            },
        };
      }
    }

    private class MultipleSection : UIView
    {
      internal MultipleSection()
      {
        ClassName = new List<string> { "select-demo-card" };

        Children = new()
        {
            new UILabel
            {
                Text = "多选模式",
                ClassName = new List<string> { "select-card-title", "label-orange" }
            },
            new UILabel
            {
                Text = "Multiple 允许选择多个选项；Tags 模式支持搜索和可清除，MaxTagCount 控制标签最大显示数量。",
                ClassName = new List<string> { "select-card-desc" }
            },
            new UIView
            {
                ClassName = new List<string> { "select-showcase" },
                Children = new()
                {
                    new UISelect
                    {
                        Options = CreateBasicOptions(),
                        Placeholder = "多选模式",
                        IsShowClose = false,
                        MaxTagCount = 2,
                        Mode = SelectMode.Multiple,
                    },
                    new UISelect
                    {
                        Options = CreateBasicOptions(),
                        Values = new List<object?> { "option2", "option3" },
                        ShowSearch = true,
                        Placeholder = "Tags 模式",
                        MaxTagCount = 2,
                        MaxCount = 2,  // 最多选择 2 项
                        AllowClear = true,
                        Mode = SelectMode.Tags,
                        Style = new UpdateUIStyle { Width = 220, Height = 30 }
                    },
                }
            },
        };
      }
    }

    private class CustomRenderSection : UIView
    {
      internal CustomRenderSection()
      {
        ClassName = new List<string> { "select-demo-card" };

        var options = new List<SelectOption>
        {
            new SelectOption("home", "首页") { CustomFields = { ["icon"] = "&#xe8c7;" } },
            new SelectOption("search", "搜索") { CustomFields = { ["icon"] = "&#xe63f;" } },
            new SelectOption("settings", "设置") { CustomFields = { ["icon"] = "&#xe60f;" } },
            new SelectOption("user", "用户") { CustomFields = { ["icon"] = "&#xe68a;" } },
        };

        Children = new()
        {
            new UILabel
            {
                Text = "自定义渲染",
                ClassName = new List<string> { "select-card-title", "label-red" }
            },
            new UILabel
            {
                Text = "通过 OptionRender 自定义下拉选项的渲染内容，可在选项前添加图标等元素。",
                ClassName = new List<string> { "select-card-desc" }
            },
            new UIView
            {
                ClassName = new List<string> { "select-showcase" },
                Children = new()
                {
                    new UISelect
                    {
                        Options = options,
                        Placeholder = "带图标的选项",
                        OptionItemHeight = 30,
                        IsShowClose = false,
                        MaxTagCount = 2,
                        Mode = SelectMode.Multiple,
                        OptionRender = (option, uiSelect) =>
                        {
                            return new UIView
                            {
                                Style = new DefaultUIStyle
                                {
                                    Width = "100%",
                                    Height = 30,
                                    Display = "flex",
                                    FlexDirection = "row",
                                    AlignItems = "center",
                                    JustifyContent = "flex-start",
                                    Gap = 8,
                                    PointerEvents = "box-only",
                                },
                                Children = new()
                                {
                                    new UIIcon
                                    {
                                        Content = option.CustomFields["icon"]?.ToString() ?? string.Empty,
                                        Style = new DefaultUIStyle
                                        {
                                            FontFamily = UIFontManager.Get("TCYMIconFont"),
                                            Color = ColorHelper.ParseColor("#555"),
                                            FontSize = 20,
                                            Width = 20,
                                            Height = 20,
                                            Display = "flex",
                                            AlignItems = "center",
                                            JustifyContent = "center",
                                        },
                                    },
                                    new UILabel { Text = option.Label },
                                }
                            };
                        },
                    },
                    new UISelect
                    {
                        Options = options,
                        Style = new UpdateUIStyle{ 
                            Width = 200
                        },
                        TagRender  = (select,uiSelect) =>{
                            var Color = select.Label.Contains("首页")? TagClassColor.Magenta :
                                select.Label.Contains("搜索")? TagClassColor.Orange :
                                select.Label.Contains("设置")? TagClassColor.Green :
                                select.Label.Contains("用户")? TagClassColor.Blue :
                                TagClassColor.Volcano;
                            return new UITag{
                                Text = select.Label,
                                ClassColor = Color,
                            };
                        },
                        MaxTagCount = 2,
                        Mode = SelectMode.Tags
                    }
                }
            },
        };
      }
    }

    private class VirtualScrollSection : UIView
    {
      internal VirtualScrollSection()
      {
        ClassName = new List<string> { "select-demo-card" };

        Children = new()
        {
            new UILabel
            {
                Text = "虚拟滚动",
                ClassName = new List<string> { "select-card-title", "label-cany" }
            },
            new UILabel
            {
                Text = "当数据量很大时开启 VirtualScroll，只渲染可见区域的选项，保持流畅交互体验。",
                ClassName = new List<string> { "select-card-desc" }
            },
            new UIView
            {
                ClassName = new List<string> { "select-showcase" },
                Children = new()
                {
                    new UISelect
                    {
                        Options = Enumerable.Range(1, 1000).Select(i => new SelectOption
                        {
                            Value = $"option{i}",
                            Label = $"选项 {i}"
                        }).ToList(),
                        Placeholder = "1000 条虚拟滚动",
                        ShowSearch = true,
                        OptionItemHeight = 22,
                        VirtualScroll = true,
                        Style = new UpdateUIStyle { Width = 200, Height = 30 }
                    },
                }
            },
        };
      }
    }
  }
}
