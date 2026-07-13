using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.ColorPicker;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.ColorPicker
{
    internal class UIColorPickerDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.ColorPicker.style.css";

        internal UIColorPickerDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "color-picker-demo-view" };
            Children = new()
            {
                new UILabel { Text = "ColorPicker 颜色选择器", ClassName = new List<string> { "color-picker-demo-title" } },
                new UILabel { Text = "用于选择、预览和输出颜色。", ClassName = new List<string> { "color-picker-demo-title-sub" } },
                new UILabel
                {
                    Text = "支持单色与线性渐变、HEX / RGB / HSB 格式、透明度、预设颜色、清除、尺寸、禁用和受控状态。",
                    ClassName = new List<string> { "color-picker-demo-desc" }
                },
                new BasicSection(),
                new SizeSection(),
                new FormatAndAlphaSection(),
                new PresetSection(),
                new GradientSection(),
                new StateSection(),
                new EventSection()
            };
        }

        private static UIView Card(string title, string description, params UIElement[] children)
        {
            var showcase = new UIView { ClassName = new List<string> { "color-picker-showcase" } };
            foreach (var child in children) showcase.AddChild(child);
            return new UIView
            {
                ClassName = new List<string> { "color-picker-demo-card" },
                Children = new()
                {
                    new UILabel { Text = title, ClassName = new List<string> { "color-picker-card-title" } },
                    new UILabel { Text = description, ClassName = new List<string> { "color-picker-card-desc" } },
                    showcase
                }
            };
        }

        private sealed class BasicSection : UIFragment
        {
            internal BasicSection()
            {
                Children = new()
                {
                    Card(
                        "基本使用",
                        "点击色块打开颜色面板；ShowText 可以在触发器中显示当前颜色值。",
                        new UIColorPicker { Value = ColorHelper.ParseColor("#1677ff") },
                        new UIColorPicker { Value = ColorHelper.ParseColor("#722ed1"), ShowText = true },
                        new UIColorPicker
                        {
                            Value = ColorHelper.ParseColor("rgba(19,194,194,0.65)"),
                            ShowText = true,
                            TextFormatter = color => color.HasValue ? "品牌青" : "未选择"
                        })
                };
            }
        }

        private sealed class SizeSection : UIFragment
        {
            internal SizeSection()
            {
                Children = new()
                {
                    Card(
                        "触发器尺寸",
                        "Size 提供 Small、Medium、Large 三种尺寸。",
                        new UIColorPicker { Size = ColorPickerSize.Small, Value = ColorHelper.ParseColor("#52c41a") },
                        new UIColorPicker { Size = ColorPickerSize.Medium, Value = ColorHelper.ParseColor("#faad14") },
                        new UIColorPicker { Size = ColorPickerSize.Large, Value = ColorHelper.ParseColor("#ff4d4f") },
                        new UIColorPicker { Size = ColorPickerSize.Large, ShowText = true, Value = ColorHelper.ParseColor("#13c2c2") })
                };
            }
        }

        private sealed class FormatAndAlphaSection : UIFragment
        {
            internal FormatAndAlphaSection()
            {
                Children = new()
                {
                    Card(
                        "颜色编码与透明度",
                        "Format 控制输出格式；DisabledAlpha 隐藏透明度滑轨，DisabledFormat 锁定格式。",
                        new UIColorPicker { ShowText = true, Format = ColorPickerFormat.Hex, Value = ColorHelper.ParseColor("#1677ff") },
                        new UIColorPicker { ShowText = true, Format = ColorPickerFormat.Rgb, Value = ColorHelper.ParseColor("rgba(22,119,255,0.7)") },
                        new UIColorPicker { ShowText = true, Format = ColorPickerFormat.Hsb, Value = ColorHelper.ParseColor("#eb2f96") },
                        new UIColorPicker
                        {
                            ShowText = true,
                            DisabledAlpha = true,
                            DisabledFormat = true,
                            Value = ColorHelper.ParseColor("#2f54eb")
                        })
                };
            }
        }

        private sealed class PresetSection : UIFragment
        {
            internal PresetSection()
            {
                Children = new()
                {
                    Card(
                        "预设颜色",
                        "Presets 将常用色按组展示在面板底部。",
                        new UIColorPicker
                        {
                            ShowText = true,
                            Value = ColorHelper.ParseColor("#1677ff"),
                            Presets = new List<ColorPickerPreset>
                            {
                                new()
                                {
                                    Label = "推荐色",
                                    Key = "recommended",
                                    Colors = new List<SKColor>
                                    {
                                        ColorHelper.ParseColor("#1677ff"),
                                        ColorHelper.ParseColor("#722ed1"),
                                        ColorHelper.ParseColor("#eb2f96"),
                                        ColorHelper.ParseColor("#ff4d4f"),
                                        ColorHelper.ParseColor("#fa8c16"),
                                        ColorHelper.ParseColor("#fadb14"),
                                        ColorHelper.ParseColor("#52c41a"),
                                        ColorHelper.ParseColor("#13c2c2")
                                    }
                                }
                            }
                        })
                };
            }
        }

        private sealed class GradientSection : UIFragment
        {
            internal GradientSection()
            {
                Children = new()
                {
                    Card(
                        "渐变色",
                        "Mode 设为 Gradient 后可选择、拖动或新增停靠点；AllowedModes 可在单色和渐变之间切换。",
                        new UIColorPicker
                        {
                            Mode = ColorPickerMode.Gradient,
                            ShowText = true,
                            GradientStops = new List<ColorGradientStop>
                            {
                                new(ColorHelper.ParseColor("#108ee9"), 0f),
                                new(ColorHelper.ParseColor("#87d068"), 1f)
                            }
                        },
                        new UIColorPicker
                        {
                            Mode = ColorPickerMode.Gradient,
                            AllowedModes = new[] { ColorPickerMode.Single, ColorPickerMode.Gradient },
                            GradientStops = new List<ColorGradientStop>
                            {
                                new(ColorHelper.ParseColor("#ff4d4f"), 0f),
                                new(ColorHelper.ParseColor("#faad14"), 0.48f),
                                new(ColorHelper.ParseColor("#52c41a"), 1f)
                            }
                        })
                };
            }
        }

        private sealed class StateSection : UIFragment
        {
            internal StateSection()
            {
                Children = new()
                {
                    Card(
                        "清除、禁用与触发方式",
                        "AllowClear 允许悬停后清除；Trigger 支持 Click 和 Hover。",
                        new UIColorPicker { AllowClear = true, ShowText = true, Value = ColorHelper.ParseColor("#1677ff") },
                        new UIColorPicker { Disabled = true, ShowText = true, Value = ColorHelper.ParseColor("#bfbfbf") },
                        new UIColorPicker
                        {
                            Trigger = ColorPickerTrigger.Hover,
                            ShowText = true,
                            Value = ColorHelper.ParseColor("#13c2c2"),
                            TextFormatter = _ => "悬停打开"
                        })
                };
            }
        }

        private sealed class EventSection : UIView
        {
            internal EventSection()
            {
                ClassName = new List<string> { "color-picker-demo-card" };
                var result = new UILabel
                {
                    Text = "尚未选择颜色",
                    ClassName = new List<string> { "color-picker-event-result" }
                };
                var picker = new UIColorPicker
                {
                    AllowClear = true,
                    ShowText = true,
                    Value = ColorHelper.ParseColor("#1677ff"),
                    OnChange = (color, css) => result.Text = $"onChange: {css}",
                    OnChangeComplete = color => result.Text += color.HasValue ? "  · 已完成" : "  · 已清除",
                    OnOpenChange = open =>
                    {
                        if (open) result.Text = "颜色面板已打开";
                    }
                };
                Children = new()
                {
                    new UILabel { Text = "事件回调", ClassName = new List<string> { "color-picker-card-title" } },
                    new UILabel { Text = "通过 OnChange、OnChangeComplete 和 OnOpenChange 监听交互。", ClassName = new List<string> { "color-picker-card-desc" } },
                    new UIView
                    {
                        ClassName = new List<string> { "color-picker-showcase" },
                        Children = new() { picker, result }
                    }
                };
            }
        }
    }
}
