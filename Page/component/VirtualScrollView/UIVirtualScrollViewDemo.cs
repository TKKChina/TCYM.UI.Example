using SkiaSharp;
using System.Collections.Generic;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.VirtualScrollView
{
    internal class UIVirtualScrollViewDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.VirtualScrollView.style.css";

        internal UIVirtualScrollViewDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);

            ClassName = new List<string> { "virtual-scroll-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "VirtualScrollView 虚拟滚动",
                    ClassName = new List<string> { "virtual-scroll-demo-title" },
                },
                new UILabel
                {
                    Text = "基于 UIScrollView 的数据虚拟化模式，只渲染视口附近的元素，适合超长列表和混合高度内容。",
                    ClassName = new List<string> { "virtual-scroll-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "当前 Demo 展示固定高度 20000 条列表和混合高度 5000 条卡片列表，两种场景都使用 Items 数据模式驱动。",
                    ClassName = new List<string> { "virtual-scroll-demo-desc" },
                },
                CreateFixedHeightSection(),
                CreateMixedHeightSection(),
            };
        }

        private static UIView CreateFixedHeightSection()
        {
            var statusLabel = CreateStatusLabel("点击任意行，查看虚拟列表中的交互回调。", "virtual-scroll-status");

            return CreateSectionCard(
                "固定高度列表",
                "每一项高度固定为 32px，总数 20000 条。适合日志、搜索结果、简单菜单、消息流等密集型列表。",
                new UIView
                {
                    ClassName = new List<string> { "virtual-scroll-showcase" },
                    Children = new()
                    {
                        statusLabel,
                        CreateFixedHeightHost(statusLabel)
                    }
                },
                CreateHintLabel("滚动容器本身保持固定高度，内容通过 Items 虚拟化渲染，避免一次性创建全部子元素。")
            );
        }

        private static UIView CreateMixedHeightSection()
        {
            var statusLabel = CreateStatusLabel("混合高度项会根据 heightProvider 参与虚拟区间计算。", "virtual-scroll-status virtual-scroll-status-accent");

            return CreateSectionCard(
                "混合高度内容",
                "列表内同时存在普通行和信息卡片，使用 heightProvider 指定不同高度，适合动态 feed、事件流和运维告警面板。",
                new UIView
                {
                    ClassName = new List<string> { "virtual-scroll-showcase" },
                    Children = new()
                    {
                        statusLabel,
                        CreateMixedHeightHost(statusLabel)
                    }
                },
                CreateHintLabel("如果单项真实高度会变化，可以再配合 realizedHeightProvider 做回写，保持滚动区间稳定。")
            );
        }

        private static UIView CreateSectionCard(string title, string description, params UIElement[] children)
        {
            var sectionChildren = new List<UIElement>
            {
                new UILabel
                {
                    Text = title,
                    ClassName = new List<string> { "virtual-scroll-card-title" },
                },
                new UILabel
                {
                    Text = description,
                    ClassName = new List<string> { "virtual-scroll-card-desc" },
                }
            };

            sectionChildren.AddRange(children);

            return new UIView
            {
                ClassName = new List<string> { "virtual-scroll-demo-card" },
                Children = sectionChildren,
            };
        }

        private static UILabel CreateStatusLabel(string text, string className)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string>(className.Split(' ', StringSplitOptions.RemoveEmptyEntries)),
            };
        }

        private static UILabel CreateHintLabel(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "virtual-scroll-demo-hint" },
            };
        }

        private static UIScrollView CreateFixedHeightHost(UILabel statusLabel)
        {
            return new UIScrollView
            {
                ClassName = new List<string> { "virtual-scroll-host" },
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = 340,
                    OverflowX = "hidden",
                },
                Items = () =>
                {
                    IList<object> items = BuildFixedItems(20000);
                    Func<object, UIElement> factory = data => CreateFixedItem((VirtualScrollRecord)data, statusLabel);
                    Func<object, float> heightProvider = _ => 32f;
                    Func<object, UIElement, float> realizedHeightProvider = (_, element) => element.Height;
                    Func<object, UIElement, float> realizedWidthProvider = (_, element) => element.Width;
                    return (items, factory, heightProvider, null, null, realizedHeightProvider, realizedWidthProvider);
                }
            };
        }

        private static UIScrollView CreateMixedHeightHost(UILabel statusLabel)
        {
            return new UIScrollView
            {
                ClassName = new List<string> { "virtual-scroll-host", "virtual-scroll-host-soft" },
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = 380,
                    OverflowX = "hidden",
                },
                Items = () =>
                {
                    IList<object> items = BuildMixedItems(5000);
                    Func<object, UIElement> factory = data => CreateMixedItem((VirtualScrollRecord)data, statusLabel);
                    Func<object, float> heightProvider = data => ((VirtualScrollRecord)data).Height;
                    Func<object, UIElement, float> realizedHeightProvider = (_, element) => element.Height;
                    Func<object, UIElement, float> realizedWidthProvider = (_, element) => element.Width;
                    return (items, factory, heightProvider, null, null, realizedHeightProvider, realizedWidthProvider);
                }
            };
        }

        private static IList<object> BuildFixedItems(int count)
        {
            var items = new List<object>(count);
            for (int index = 1; index <= count; index++)
            {
                items.Add(new VirtualScrollRecord
                {
                    Index = index,
                    Title = $"日志条目 {index:00000}",
                    Description = index % 500 == 0 ? "热点分片命中" : "普通消息",
                    Height = 32f,
                    Highlight = index % 500 == 0,
                    IsCard = false,
                });
            }
            return items;
        }

        private static IList<object> BuildMixedItems(int count)
        {
            var items = new List<object>(count);
            for (int index = 1; index <= count; index++)
            {
                bool isCard = index % 9 == 0;
                items.Add(new VirtualScrollRecord
                {
                    Index = index,
                    Title = isCard ? $"告警事件 #{index:0000}" : $"普通事件 #{index:0000}",
                    Description = isCard ? "节点负载升高，建议在 10 分钟内完成排查。" : "滚动区域仅渲染当前可见窗口附近的内容。",
                    Height = isCard ? 72f : 30f,
                    Highlight = index % 27 == 0,
                    IsCard = isCard,
                });
            }
            return items;
        }

        private static UIElement CreateFixedItem(VirtualScrollRecord record, UILabel statusLabel)
        {
            var classNames = new List<string> { "virtual-scroll-row" };
            if (record.Highlight)
            {
                classNames.Add("virtual-scroll-row-highlight");
            }

            return new UILabel
            {
                Text = $"{record.Index:00000} · {record.Title} · {record.Description}",
                Wrap = false,
                ClassName = classNames,
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = record.Height,
                },
                Events = new()
                {
                    Click = _ => UpdateStatus(statusLabel, $"固定高度列表：点击了第 {record.Index:00000} 条，类型 = {record.Description}")
                }
            };
        }

        private static UIElement CreateMixedItem(VirtualScrollRecord record, UILabel statusLabel)
        {
            if (!record.IsCard)
            {
                return new UILabel
                {
                    Text = $"{record.Index:0000} · {record.Title}",
                    Wrap = false,
                    ClassName = new List<string> { "virtual-scroll-row", "virtual-scroll-row-compact" },
                    Style = new DefaultUIStyle
                    {
                        Width = "100%",
                        Height = record.Height,
                    },
                    Events = new()
                    {
                        Click = _ => UpdateStatus(statusLabel, $"混合高度列表：点击了普通行 {record.Index:0000}")
                    }
                };
            }

            var cardClassNames = new List<string> { "virtual-scroll-mixed-card" };
            if (record.Highlight)
            {
                cardClassNames.Add("virtual-scroll-mixed-card-warning");
            }

            return new UIView
            {
                ClassName = cardClassNames,
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = record.Height,
                },
                Events = new()
                {
                    Click = _ => UpdateStatus(statusLabel, $"混合高度列表：点击了卡片项 {record.Index:0000}，说明 = {record.Description}")
                },
                Children = new()
                {
                    new UILabel
                    {
                        Text = record.Title,
                        ClassName = new List<string> { "virtual-scroll-mixed-card-title" },
                    },
                    new UILabel
                    {
                        Text = record.Description,
                        ClassName = new List<string> { "virtual-scroll-mixed-card-desc" },
                    }
                }
            };
        }

        private static void UpdateStatus(UILabel label, string text)
        {
            label.Text = text;
            label.RequestLayout();
            label.RequestRedraw();
        }

        private sealed class VirtualScrollRecord
        {
            public int Index { get; init; }
            public string Title { get; init; } = string.Empty;
            public string Description { get; init; } = string.Empty;
            public float Height { get; init; }
            public bool Highlight { get; init; }
            public bool IsCard { get; init; }
        }
    }
}