using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Table.Sections
{
    internal static class TableSectionHelper
    {
        internal static UIView CreateSectionCard(string title, string description, params UIElement[] content)
        {
            var card = new UIView
            {
                ClassName = new List<string> { "table-demo-card" },
                Children = new List<UIElement>
                {
                    new UILabel
                    {
                        Text = title,
                        ClassName = new List<string> { "table-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = description,
                        ClassName = new List<string> { "table-card-desc" }
                    }
                }
            };

            foreach (var item in content)
            {
                card.AddChild(item);
            }

            return card;
        }

        internal static UILabel CreateHintLabel(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "table-hint-label" }
            };
        }

        internal static void SetLabelText(UILabel label, string text)
        {
            label.Text = text;
            label.RequestLayout();
            label.RequestRedraw();
        }

        internal static SKColor ParseColor(string color)
        {
            return ColorHelper.ParseColor(color);
        }
    }
}
