using SkiaSharp;
using TCYM.UI.Binding;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Carousel;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Carousel
{
    internal class UICarouselDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Carousel.style.css";

        internal UICarouselDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "carousel-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Carousel 走马灯",
                    ClassName = new List<string> { "carousel-demo-title" },
                },
                new UILabel
                {
                    Text = "一组轮播内容区域，支持自动播放、箭头、拖拽、淡入淡出和自定义指示点。",
                    ClassName = new List<string> { "carousel-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "Items 可以放任意 UIElement；Dots 支持默认样式，也可以通过 DotTemplate 回调返回自定义 UIElement。",
                    ClassName = new List<string> { "carousel-demo-desc" },
                },
                new BasicCarouselSection(),
                new AutoplayCarouselSection(),
                new DraggableCarouselSection(),
                new CustomDotsCarouselSection(),
                new FadeCarouselSection(),
            };
        }

        private class BasicCarouselSection : UIView
        {
            internal BasicCarouselSection()
            {
                ClassName = new List<string> { "carousel-demo-card" };

                var carousel = new UICarousel
                {
                    Arrows = true,
                    Draggable = true,
                    DotPlacement = CarouselDotPlacement.Bottom,
                    Style = new UpdateUIStyle
                    {
                        Height = 250,
                        BorderRadius = 12,
                    },
                    Items = CreateSlides("基础", "#0f62fe", "#16a34a", "#f59e0b"),
                };

                Children = new()
                {
                    CreateSectionTitle("基础用法", "label-title"),
                    CreateSectionDescription("使用 Items 添加轮播页，开启 Arrows 和 Dots 即可完成常见的走马灯交互。"),
                    carousel,
                };
            }
        }

        private class AutoplayCarouselSection : UIView
        {
            internal AutoplayCarouselSection()
            {
                ClassName = new List<string> { "carousel-demo-card" };

                var status = CreateStatusLabel("当前页：1 / 3");
                var carousel = new UICarousel
                {
                    Autoplay = true,
                    AutoplaySpeed = 2200,
                    DotDuration = true,
                    Arrows = true,
                    DotPlacement = CarouselDotPlacement.End,
                    ActiveDotColor = ColorHelper.ParseColor("#7c3aed"),
                    DotColor = ColorHelper.ParseColor("rgba(124,58,237,0.28)"),
                    Style = new UpdateUIStyle
                    {
                        Height = 230,
                        BorderRadius = 12,
                    },
                    Items = CreateSlides("自动播放", "#7c3aed", "#0891b2", "#dc2626"),
                    AfterChange = index => status.Text = $"当前页：{index + 1} / 3",
                };

                Children = new()
                {
                    CreateSectionTitle("自动播放与侧边指示点", "label-green"),
                    CreateSectionDescription("Autoplay 与 AutoplaySpeed 控制自动切换。DotDuration=true 时默认指示点会显示播放进度。"),
                    carousel,
                    status,
                };
            }
        }

        private class CustomDotsCarouselSection : UIView
        {
            internal CustomDotsCarouselSection()
            {
                ClassName = new List<string> { "carousel-demo-card" };

                var carousel = new UICarousel
                {
                    Arrows = true,
                    DotPlacement = CarouselDotPlacement.Top,
                    ActiveDotColor = ColorHelper.ParseColor("#1677ff"),
                    DotColor = ColorHelper.ParseColor("rgba(22,119,255,0.16)"),
                    DotGap = 6,
                    DotOffset = 12,
                    DotTemplate = BuildNumberDot,
                    Style = new UpdateUIStyle
                    {
                        Height = 240,
                        BorderRadius = 12,
                    },
                    Items = CreateSlides("自定义 dots", "#1677ff", "#13a36e", "#fa8c16"),
                };

                Children = new()
                {
                    CreateSectionTitle("自定义指示点", "label-orange"),
                    CreateSectionDescription("DotTemplate 会收到 UICarouselDotContext，可根据 IsActive、ActiveDotColor 等上下文返回任意 UIElement。"),
                    carousel,
                };
            }
        }

        private class DraggableCarouselSection : UIView
        {
            internal DraggableCarouselSection()
            {
                ClassName = new List<string> { "carousel-demo-card" };

                var status = CreateStatusLabel("拖拽卡片或悬停显示箭头");
                var carousel = new UICarousel
                {
                    Arrows = true,
                    ArrowAlwaysVisible = false,
                    Draggable = true,
                    Infinite = false,
                    DotPlacement = CarouselDotPlacement.Bottom,
                    ActiveDotColor = ColorHelper.ParseColor("#0891b2"),
                    DotColor = ColorHelper.ParseColor("rgba(8,145,178,0.22)"),
                    Style = new UpdateUIStyle
                    {
                        Height = 230,
                        BorderRadius = 12,
                    },
                    Items = CreateSlides("拖拽切换", "#0891b2", "#4338ca", "#be123c"),
                    AfterChange = index => status.Text = $"已切换到第 {index + 1} 页",
                };

                Children = new()
                {
                    CreateSectionTitle("拖拽切换与悬停箭头", "label-cany"),
                    CreateSectionDescription("Draggable=true 后可直接拖拽切换；ArrowAlwaysVisible=false 时，箭头只在鼠标悬停 Carousel 时显示。"),
                    carousel,
                    status,
                };
            }
        }

        private class FadeCarouselSection : UIView
        {
            internal FadeCarouselSection()
            {
                ClassName = new List<string> { "carousel-demo-card" };

                var carousel = new UICarousel
                {
                    Effect = CarouselEffect.Fade,
                    Arrows = true,
                    Autoplay = true,
                    AutoplaySpeed = 2600,
                    Speed = 650,
                    Easing = "ease-in-out",
                    DotPlacement = CarouselDotPlacement.Bottom,
                    ActiveDotColor = ColorHelper.ParseColor("#ffffff"),
                    DotColor = ColorHelper.ParseColor("rgba(255,255,255,0.32)"),
                    Style = new UpdateUIStyle
                    {
                        Height = 240,
                        BorderRadius = 12,
                    },
                    Items = CreateSlides("淡入淡出", "#111827", "#334155", "#0f766e"),
                };

                Children = new()
                {
                    CreateSectionTitle("Fade 效果", "label-red"),
                    CreateSectionDescription("Effect=Fade 时轮播页使用淡入淡出切换；WaitForAnimate 可用于阻止动画未完成时的重复切换。"),
                    carousel,
                };
            }
        }

        private static List<UIElement> CreateSlides(string prefix, params string[] colors)
        {
            var result = new List<UIElement>();
            for (int i = 0; i < colors.Length; i++)
            {
                result.Add(CreateSlide(
                    $"{prefix} 0{i + 1}",
                    i switch
                    {
                        0 => "清晰聚焦当前内容，适合公告、看板或产品焦点图。",
                        1 => "支持任意 UIElement，页面里可以放图文、按钮或数据摘要。",
                        _ => "指示点、箭头、自动播放和拖拽都可以按场景组合使用。",
                    },
                    i switch
                    {
                        0 => "交互状态",
                        1 => "可组合内容",
                        _ => "自定义 dots",
                    },
                    colors[i],
                    i));
            }

            return result;
        }

        private static UIView CreateSlide(string title, string desc, string badge, string color, int index)
        {
            var baseColor = ColorHelper.ParseColor(color);
            var softColor = new SKColor(
                (byte)Math.Min(255, baseColor.Red + 44),
                (byte)Math.Min(255, baseColor.Green + 44),
                (byte)Math.Min(255, baseColor.Blue + 44),
                255);

            return new UIView
            {
                ClassName = new List<string> { "carousel-slide-card" },
                Style = new DefaultUIStyle
                {
                    BackgroundGradient = $"linear-gradient(135deg, {ColorHelper.ColorToRgbaString(baseColor)}, {ColorHelper.ColorToRgbaString(softColor)})",
                },
                Children = new()
                {
                    new UILabel
                    {
                        Text = badge,
                        ClassName = new List<string> { "carousel-slide-badge" },
                    },
                    new UILabel
                    {
                        Text = title,
                        ClassName = new List<string> { "carousel-slide-title" },
                    },
                    new UILabel
                    {
                        Text = desc,
                        ClassName = new List<string> { "carousel-slide-desc" },
                    },
                    new UILabel
                    {
                        Text = $"0{index + 1}",
                        ClassName = new List<string> { "carousel-slide-index" },
                    },
                }
            };
        }

        private static UIView BuildNumberDot(UICarouselDotContext context)
        {
            return new UIView
            {
                Style = new DefaultUIStyle
                {
                    Width = context.IsActive ? 38 : 28,
                    Height = 24,
                    BorderRadius = 999,
                    Display = "flex",
                    AlignItems = "center",
                    JustifyContent = "center",
                    BackgroundColor = context.IsActive ? context.ActiveDotColor : context.DotColor,
                    BorderWidth = 1,
                    BorderColor = context.IsActive ? context.ActiveDotColor : ColorHelper.ParseColor("rgba(22,119,255,0.24)"),
                    PointerEvents = "none",
                },
                Children = new()
                {
                    new UILabel
                    {
                        Text = (context.Index + 1).ToString(),
                        ClassName = new List<string> { context.IsActive ? "carousel-number-dot-active" : "carousel-number-dot" },
                    }
                }
            };
        }

        private static UILabel CreateSectionTitle(string text, string accentClass)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "carousel-card-title", accentClass }
            };
        }

        private static UILabel CreateSectionDescription(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "carousel-card-desc" }
            };
        }

        private static UILabel CreateStatusLabel(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "carousel-status-label" }
            };
        }
    }
}
