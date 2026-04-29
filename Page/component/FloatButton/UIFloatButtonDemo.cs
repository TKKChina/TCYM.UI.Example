using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Message;
using TCYM.UI.Elements.Tooltip;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.FloatButton
{
  internal class UIFloatButtonDemo : UIScrollView
  {
    private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.FloatButton.style.css";

    internal UIFloatButtonDemo()
    {
      UISystem.LoadStyleFile(DemoCssPath);
      ClassName = new List<string> { "float-button-demo-view" };
      Children = new()
      {
          new UILabel
          {
            Text = "FloatButton 悬浮按钮",
            ClassName = new List<string> { "float-button-demo-title" },
          },
          new UILabel
          {
            Text = "悬浮于页面右下角的快捷操作按钮。",
            ClassName = new List<string> { "float-button-demo-title-sub" },
          },
          new UILabel
          {
            Text = "这版实现参考 Ant Design 的 FloatButton，支持 fixed 定位、描述型按钮、Tooltip、Badge、按钮组，以及基于 UIScrollView 的回到顶部按钮。页面右下角有真实固定示例，可直接交互。",
            ClassName = new List<string> { "float-button-demo-desc" },
          },
          new BasicSection(),
          new DecoratedSection(),
          new GroupSection(),
          new UIFloatBackTop
          {
            Target = this,
            Icon = "&#xe649;",
            IconFont = UIFontManager.Get("IconFontExample"),
            VisibilityHeight = 200,
            OffsetBottom = 30,
          }
      };
    }

    private class BasicSection : UIView
    {
      internal BasicSection()
      {
        ClassName = new List<string> { "float-button-demo-card" };
        Children = new()
        {
          new UILabel
          {
              Text = "基础形态",
              ClassName = new List<string> { "float-button-card-title", "label-title" }
          },
          new UILabel
          {
              Text = "默认支持圆形与方形两种基础外观。Description 会自动切到方形说明模式，适合承载较轻量的快捷操作。",
              ClassName = new List<string> { "float-button-card-desc" }
          },
          new UIView
          {
              ClassName = new List<string> { "float-button-showcase" },
              Children = new()
              {
                new UIFloatButton
                {
                  Fixed = true,
                  Icon = "&#xe649;",
                  IconFont = UIFontManager.Get("IconFontExample"),
                  ButtonStyle = new DefaultUIStyle
                  {
                    BackgroundColor = ColorHelper.ParseColor("#1677ff"),
                    Color = SKColors.White,
                    BorderWidth = 0,
                    Hover = new DefaultUIStyle
                    {
                      BackgroundColor = ColorHelper.ParseColor("#4096ff"),
                    }
                  },
                  Tooltip = "回到顶部",
                  TooltipPlacement = TooltipPlacement.Bottom,
                  //Description = "帮助",
                  Click = () => UIMessage.Success("回到顶部已触发")
                }
              }
          }
        };
      }
    }

    private class DecoratedSection : UIView
    {
      internal DecoratedSection()
      {
        ClassName = new List<string> { "float-button-demo-card" };
        Children = new()
        {
          new UILabel
          {
            Text = "Tooltip 与 Badge",
            ClassName = new List<string> { "float-button-card-title", "label-green" }
          },
          new UILabel
          {
            Text = "悬浮按钮的常见搭配是 Tooltip 说明和 Badge 提醒。这里直接复用现有 Tooltip 与 Badge 组件，避免重复实现一套浮层与徽标逻辑。",
            ClassName = new List<string> { "float-button-card-desc" }
          },
          new UIView
          {
            ClassName = new List<string> { "float-button-showcase" },
            Children = new()
            {
              new UIFloatButton
              {
                Fixed = true,
                Shape = FloatButtonShape.Square,
                Description = "更新",
                Icon = "&#xe61c;",
                BadgeText = "NEW",
                BadgeColor = ColorHelper.ParseColor("#722ed1"),
                Tooltip = "查看更新日志",
                Click = () => UIMessage.Info("更新日志")
              },
            }
          }
        };
      }
    }

    private class GroupSection : UIView
    {
      internal GroupSection()
      {
        ClassName = new List<string> { "float-button-demo-card" };
        Children = new()
        {
          new UILabel
          {
              Text = "按钮组",
              ClassName = new List<string> { "float-button-card-title", "label-orange" }
          },
          new UILabel
          {
              Text = "UIFloatButtonGroup 会把内部按钮自动切到相对定位，并按垂直方向堆叠，适合放一组相关的快捷操作。",
              ClassName = new List<string> { "float-button-card-desc" }
          },
          new UIView
          {
            ClassName = new List<string> { "float-button-group-stage" },
            Children = new()
            {
              new UIFloatButtonGroup
              {
                Fixed = true,
                Gap = 10,
                Children = new()
                {
                  new UIFloatButton
                  {
                    Icon = "&#xe636;",
                    Tooltip = "新建",
                    Click = () => UIMessage.Success("新建成功")
                  },
                  new UIFloatButton
                  {
                      Icon = "&#xe60b;",
                      Tooltip = "搜索",
                      Click = () => UIMessage.Info("搜索面板")
                  },
                  new UIFloatButton
                  {
                      Icon = "&#xe6ca;",
                      Tooltip = "用户反馈",
                      Click = () => UIMessage.Info("反馈入口")
                  }
                }
              }
            }
          }
        };
      }
    }

  }
}