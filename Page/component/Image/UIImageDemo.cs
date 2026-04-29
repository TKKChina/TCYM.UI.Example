using TCYM.UI.Binding;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Image;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Image
{
    internal class UIImageDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Image.style.css";
        private static readonly string DemoPngSource = UIFileHelper.ResolveAssetPath("Assets", "Images", "tu.png");
        private static readonly string DemoGifSource = UIFileHelper.ResolveAssetPath("Assets", "Images", "gif.gif");

        internal UIImageDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "image-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Image 图片",
                    ClassName = new List<string> { "image-demo-title", "label-title" }
                },
                new UILabel
                {
                    Text = "可预览图片，支持多帧格式、缩放、旋转和复制。",
                    ClassName = new List<string> { "image-demo-title-sub" }
                },
                new UILabel
                {
                    Text = "UIImage 可通过 Source 加载 res://、绝对路径或相对路径，也可直接传入 ImageBytes。GIF、APNG、动画 WebP 等多帧格式会自动播放；工具条支持常显或悬停显示，并提供放大、缩小、旋转、重置和复制操作。",
                    ClassName = new List<string> { "image-demo-desc" }
                },
                new BasicImageSection(),
                new TransformImageSection(),
                new AnimatedImageSection(),
            };
        }

        private class BasicImageSection : UIView
        {
            internal BasicImageSection()
            {
                ClassName = new List<string> { "image-demo-card" };
                Children = new()
                {
                    CreateSectionTitle("基础用法"),
                    CreateSectionDescription("设置 ImageBytes 或 Source 即可渲染图片。ShowToolbar=true 启用内置预览工具条，ToolbarAlwaysVisible=true 时一直显示。"),
                    new UIView
                    {
                        ClassName = new List<string> { "image-showcase" },
                        Children = new()
                        {
                            new UIImage
                            {
                                Source = DemoPngSource,
                                ObjectFit = UIImageFit.Cover,
                                ShowToolbar = true,
                                ToolbarAlwaysVisible = true,
                                Style = new DefaultUIStyle
                                {
                                    Width = 720,
                                    Height = 460,
                                    BorderRadius = 8,
                                    BackgroundColor = ColorHelper.ParseColor("#f6f8fb"),
                                    BorderWidth = 1,
                                    BorderColor = ColorHelper.ParseColor("#d7dde8")
                                }
                            }
                        }
                    }
                };
            }
        }

        private class TransformImageSection : UIView
        {
            internal TransformImageSection()
            {
                ClassName = new List<string> { "image-demo-card" };

                var statusLabel = new UILabel
                {
                    Text = "缩放 100%，旋转 0°",
                    ClassName = new List<string> { "image-status-label" }
                };

                var image = new UIImage
                {
                    Source = DemoPngSource,
                    ObjectFit = UIImageFit.Contain,
                    ShowToolbar = true,
                    Style = new DefaultUIStyle
                    {
                        Width = 540,
                        Height = 345,
                        BorderRadius = 8,
                        BackgroundColor = ColorHelper.ParseColor("#ffffff"),
                        BorderWidth = 1,
                        BorderColor = ColorHelper.ParseColor("#d9d9d9")
                    },
                    OnTransformChanged = target => statusLabel.Text = BuildTransformText(target),
                    OnCopy = (target, copied) => statusLabel.Text = copied ? "已复制当前帧到剪贴板" : "复制失败：当前平台或剪贴板不可用",
                    OnToolbarActionCompleted = (target, action, succeeded) => statusLabel.Text = BuildToolbarActionText(target, action, succeeded)
                };

                Children = new()
                {
                    CreateSectionTitle("缩放、旋转与复制"),
                    CreateSectionDescription("可以通过组件方法控制图片，也可以直接使用悬停工具条。工具栏操作完成后会触发 OnToolbarActionCompleted 回调。"),
                    new UIView
                    {
                        ClassName = new List<string> { "image-transform-row" },
                        Children = new()
                        {
                            image,
                            new UIView
                            {
                                ClassName = new List<string> { "image-control-panel" },
                                Children = new()
                                {
                                    statusLabel,
                                    CreateButton("放大", image.ZoomIn),
                                    CreateButton("缩小", image.ZoomOut),
                                    CreateButton("左旋", image.RotateLeft),
                                    CreateButton("右旋", image.RotateRight),
                                    CreateButton("重置", image.ResetTransform),
                                    CreateButton("复制", () => image.CopyToClipboard()),
                                }
                            }
                        }
                    }
                };
            }
        }

        private class AnimatedImageSection : UIView
        {
            internal AnimatedImageSection()
            {
                ClassName = new List<string> { "image-demo-card" };

                var apngImage = new UIImage
                {
                    Source = DemoGifSource,
                    ObjectFit = UIImageFit.Contain,
                    ShowToolbar = true,
                    Style = new DefaultUIStyle
                    {
                        Width = 240,
                        Height = 180,
                        BorderRadius = 8,
                        BackgroundColor = ColorHelper.ParseColor("#000"),
                        BorderWidth = 1,
                        BorderColor = ColorHelper.ParseColor("#d7dde8")
                    }
                };

                Children = new()
                {
                    CreateSectionTitle("GIF / APNG 多帧图片"),
                    CreateSectionDescription("同一个解码通道会读取 SkiaSharp 支持的多帧格式。这里直接使用 Assets/Images/gif.gif 作为示例，业务中也可设置 Source 指向 .apng 文件。"),
                    new UIView
                    {
                        ClassName = new List<string> { "image-showcase" },
                        Children = new()
                        {
                            apngImage,
                            new UIView
                            {
                                ClassName = new List<string> { "image-format-list" },
                                Children = new()
                                {
                                    CreateFormatLabel("APNG：自动读取帧时长并循环播放"),
                                    CreateFormatLabel("GIF：支持常见动图和局部帧更新"),
                                    CreateFormatLabel("复制：复制当前渲染帧到系统剪贴板"),
                                }
                            }
                        }
                    }
                };
            }
        }

        private static UILabel CreateSectionTitle(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "image-card-title", "label-title" }
            };
        }

        private static UILabel CreateSectionDescription(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "image-card-desc" }
            };
        }

        private static UILabel CreateFormatLabel(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "image-format-label" }
            };
        }

        private static UIButton CreateButton(string text, Action action)
        {
            return new UIButton
            {
                Text = text,
                ClassName = new List<string> { "image-action-button" },
                Events = new UIEventBindings
                {
                    Click = _ => action()
                }
            };
        }

        private static string BuildTransformText(UIImage image)
        {
            return $"缩放 {(int)Math.Round(image.Scale * 100)}%，旋转 {(int)Math.Round(image.RotationDegrees)}°";
        }

        private static string BuildToolbarActionText(UIImage image, UIImageToolbarAction action, bool succeeded)
        {
            var actionText = action switch
            {
                UIImageToolbarAction.ZoomOut => "工具栏缩小",
                UIImageToolbarAction.ZoomIn => "工具栏放大",
                UIImageToolbarAction.RotateLeft => "工具栏左旋",
                UIImageToolbarAction.RotateRight => "工具栏右旋",
                UIImageToolbarAction.Reset => "工具栏重置",
                UIImageToolbarAction.Copy => "工具栏复制",
                _ => "工具栏操作"
            };

            var resultText = succeeded ? "完成" : "失败";
            return $"{actionText}{resultText}，{BuildTransformText(image)}";
        }
    }
}