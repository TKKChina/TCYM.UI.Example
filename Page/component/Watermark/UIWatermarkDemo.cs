using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Watermark
{
    internal class UIWatermarkDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Watermark.style.css";

        internal UIWatermarkDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = "watermark-demo-view";
            Children = new()
            {
                new UILabel
                {
                    Text = "Watermark 水印",
                    ClassName = "watermark-demo-title",
                },
                new UILabel
                {
                    Text = "在页面上添加文本或图片等水印信息。",
                    ClassName = "watermark-demo-title-sub",
                },
                new UILabel
                {
                    Text = "使用 Watermark 组件可以在内容区域上添加半透明水印，适用于版权保护、信息标识等场景。支持自定义内容、旋转角度、间距、偏移和字体样式。",
                    ClassName = "watermark-demo-desc",
                },
                new BasicSection(),
                new RotateSection(),
                new GapSection(),
                new CustomFontSection(),
                new MultiLineSection(),
            };
        }

        /// <summary>
        /// 创建占位内容块。
        /// </summary>
        private static UIView CreatePlaceholderContent()
        {
            return new UIView
            {
                ClassName = "watermark-content-block",
                Children = new()
                {
                    new UIView { ClassName = "watermark-content-title" },
                    new UIView { ClassName = "watermark-content-line" },
                    new UIView { ClassName = "watermark-content-line" },
                    new UIView { ClassName = "watermark-content-line" },
                    new UIView { ClassName = "watermark-content-line-short" },
                }
            };
        }

        /// <summary>
        /// 创建水印卡片容器。
        /// </summary>
        private static UIView CreateWatermarkCard(UIWatermark watermark, string label)
        {
            return new UIView
            {
                ClassName = "watermark-item-wrapper",
                Children = new()
                {
                    watermark,
                    new UILabel
                    {
                        Text = label,
                        ClassName = "watermark-item-label",
                    }
                }
            };
        }

        /// <summary>
        /// 基础用法
        /// </summary>
        private class BasicSection : UIView
        {
            internal BasicSection()
            {
                ClassName = "watermark-demo-card";
                Children = new()
                {
                    new UILabel
                    {
                        Text = "基础用法",
                        ClassName = "watermark-card-title"
                    },
                    new UILabel
                    {
                        Text = "默认显示水印内容为「TCYM.UI」，旋转角度 -22 度。可以通过 Content 属性自定义水印文本。",
                        ClassName = "watermark-card-desc"
                    },
                    new UIView
                    {
                        ClassName = "watermark-showcase",
                        Children = new()
                        {
                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card",
                                    Children = { CreatePlaceholderContent() }
                                },
                                "默认水印"),

                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card",
                                    Content = "TCYM.UI 测试",
                                    Children = { CreatePlaceholderContent() }
                                },
                                "自定义内容"),
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 旋转角度
        /// </summary>
        private class RotateSection : UIView
        {
            internal RotateSection()
            {
                ClassName = "watermark-demo-card";
                Children = new()
                {
                    new UILabel
                    {
                        Text = "旋转角度",
                        ClassName = "watermark-card-title"
                    },
                    new UILabel
                    {
                        Text = "通过 Rotate 属性设置水印旋转角度，默认 -22 度。可以设置为 0 度（水平）、-45 度等。",
                        ClassName = "watermark-card-desc"
                    },
                    new UIView
                    {
                        ClassName = "watermark-showcase",
                        Children = new()
                        {
                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card",
                                    Rotate = 0,
                                    Children = { CreatePlaceholderContent() }
                                },
                                "0 度"),

                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card",
                                    Rotate = -22,
                                    Children = { CreatePlaceholderContent() }
                                },
                                "-22 度（默认）"),

                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card",
                                    Rotate = -45,
                                    Children = { CreatePlaceholderContent() }
                                },
                                "-45 度"),
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 间距与偏移
        /// </summary>
        private class GapSection : UIView
        {
            internal GapSection()
            {
                ClassName = "watermark-demo-card";
                Children = new()
                {
                    new UILabel
                    {
                        Text = "间距与偏移",
                        ClassName = "watermark-card-title"
                    },
                    new UILabel
                    {
                        Text = "通过 GapX / GapY 设置水印之间的间距，通过 OffsetX / OffsetY 设置水印相对于左上角的偏移量。",
                        ClassName = "watermark-card-desc"
                    },
                    new UIView
                    {
                        ClassName = "watermark-showcase",
                        Children = new()
                        {
                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card",
                                    GapX = 12,
                                    GapY = 8,
                                    Children = { CreatePlaceholderContent() }
                                },
                                "默认间距"),

                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card",
                                    GapX = 40,
                                    GapY = 30,
                                    Children = { CreatePlaceholderContent() }
                                },
                                "大间距"),

                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card",
                                    OffsetX = 30,
                                    OffsetY = 20,
                                    Children = { CreatePlaceholderContent() }
                                },
                                "偏移位置"),
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 自定义字体
        /// </summary>
        private class CustomFontSection : UIView
        {
            internal CustomFontSection()
            {
                ClassName = "watermark-demo-card";
                Children = new()
                {
                    new UILabel
                    {
                        Text = "自定义字体",
                        ClassName = "watermark-card-title"
                    },
                    new UILabel
                    {
                        Text = "通过 FontConfig 属性配置水印字体的颜色、大小、粗细和字体族。",
                        ClassName = "watermark-card-desc"
                    },
                    new UIView
                    {
                        ClassName = "watermark-showcase",
                        Children = new()
                        {
                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card",
                                    Content = "TCYM.UI",
                                    FontConfig = new WatermarkFont
                                    {
                                        Color = new SKColor(255, 0, 0, 60),
                                        FontSize = 16,
                                    },
                                    Children = { CreatePlaceholderContent() }
                                },
                                "红色字体"),

                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card",
                                    Content = "TCYM.UI",
                                    FontConfig = new WatermarkFont
                                    {
                                        Color = new SKColor(22, 119, 255, 60),
                                        FontSize = 20,
                                        FontWeight = "bold",
                                    },
                                    Children = { CreatePlaceholderContent() }
                                },
                                "蓝色粗体"),

                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card"   ,
                                    Content = "TCYM.UI",
                                    FontConfig = new WatermarkFont
                                    {
                                        Color = new SKColor(0, 0, 0, 80),
                                        FontSize = 12,
                                    },
                                    WatermarkWidth = 80,
                                    WatermarkHeight = 40,
                                    Children = { CreatePlaceholderContent() }
                                },
                                "小字号密水印"),
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 多行水印
        /// </summary>
        private class MultiLineSection : UIView
        {
            internal MultiLineSection()
            {
                ClassName = "watermark-demo-card";
                Children = new()
                {
                    new UILabel
                    {
                        Text = "多行水印",
                        ClassName = "watermark-card-title"
                    },
                    new UILabel
                    {
                        Text = "水印内容支持较长文本，通过调整 WatermarkWidth 和 WatermarkHeight 控制水印单元大小。",
                        ClassName = "watermark-card-desc"
                    },
                    new UIView
                    {
                        ClassName = "watermark-showcase",
                        Children = new()
                        {
                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card",
                                    Content = "TCYM.UI Framework",
                                    WatermarkWidth = 180,
                                    WatermarkHeight = 80,
                                    Children = { CreatePlaceholderContent() }
                                },
                                "长文本"),

                            CreateWatermarkCard(
                                new UIWatermark
                                {
                                    ClassName = "watermark-item-card",
                                    Content = "版权所有 TCYM© 2026",
                                    WatermarkWidth = 160,
                                    WatermarkHeight = 70,
                                    Rotate = -15,
                                    FontConfig = new WatermarkFont
                                    {
                                        Color = new SKColor(0, 0, 0, 50),
                                        FontSize = 13,
                                    },
                                    Children = { CreatePlaceholderContent() }
                                },
                                "版权信息"),
                        }
                    },
                };
            }
        }
    }
}