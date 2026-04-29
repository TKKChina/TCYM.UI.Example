using System;
using System.Collections.Generic;
using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Slider;

namespace TCYM.UI.Example.Page.component.Slider
{
    /// <summary>
    /// Slider 滑动输入条组件示例页面。
    /// 展示基础用法、带刻度、范围模式、禁用态和温度计效果。
    /// </summary>
    public class UISliderDemo : UIScrollView
    {
        const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Slider.style.css";

        internal UISliderDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "slider-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Slider 滑动输入条",
                    ClassName = new List<string> { "slider-demo-title", "label-title" }
                },
                new UILabel
                {
                    Text = "滑动型输入器，展示当前值和可选范围。",
                    ClassName = new List<string> { "slider-demo-title-sub" }
                },
                new UILabel
                {
                    Text = "当用户需要在数值区间/自定义区间内进行选择时，可为连续或离散值。",
                    ClassName = new List<string> { "slider-demo-desc" }
                },
                new BasicSliderDemo(),
                new MarksSliderDemo(),
                new RangeSliderDemo(),
                new DisabledSliderDemo(),
                new RulerSliderDemo(),
                new ThermoSliderDemo(),
            };
        }

        // ═══════════ 基础滑块 ═══════════
        class BasicSliderDemo : UIView
        {
            internal BasicSliderDemo()
            {
                ClassName = new List<string> { "slider-demo-card" };
                var valueLabel = new UILabel
                {
                    Text = "当前值: 30",
                    ClassName = new List<string> { "slider-value-label" }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "基础滑块",
                        ClassName = new List<string> { "slider-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "基本滑动条。拖动滑块可以选择值，拖拽时显示当前值 Tooltip。",
                        ClassName = new List<string> { "slider-card-desc" }
                    },
                    new UISlider
                    {
                        DefaultValue = 30,
                        Min = 0,
                        Max = 100,
                        Step = 1,
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            Height = 40,
                        },
                        OnChange = v => valueLabel.Text = $"当前值: {(int)v}",
                    },
                    valueLabel,
                };
            }
        }

        // ═══════════ 带刻度滑块 ═══════════
        class MarksSliderDemo : UIView
        {
            internal MarksSliderDemo()
            {
                ClassName = new List<string> { "slider-demo-card" };

                var valueLabel1 = new UILabel
                {
                    Text = "当前值: 37",
                    ClassName = new List<string> { "slider-value-label" }
                };
                var valueLabel2 = new UILabel
                {
                    Text = "当前值: 37 (step=null 只能选刻度)",
                    ClassName = new List<string> { "slider-value-label" }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "带标签的滑块",
                        ClassName = new List<string> { "slider-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "使用 Marks 属性标注分段式滑块。当 Step=null 时，可选值仅有 Marks 标记位置。",
                        ClassName = new List<string> { "slider-card-desc" }
                    },
                    // included=true (默认)
                    new UILabel
                    {
                        Text = "included = true (默认高亮区间):",
                        Style = new DefaultUIStyle { FontSize = 12, Color = new SKColor(0, 0, 0, 150), MarginBottom = 4 }
                    },
                    new UISlider
                    {
                        DefaultValue = 37,
                        Min = 0,
                        Max = 100,
                        Step = 1,
                        Included = true,
                        Marks = new Dictionary<float, string>
                        {
                            { 0, "0°C" },
                            { 26, "26°C" },
                            { 37, "37°C" },
                            { 100, "100°C" }
                        },
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            Height = 50,
                        },
                        OnChange = v => valueLabel1.Text = $"当前值: {(int)v}",
                    },
                    valueLabel1,
                    // included=false
                    new UILabel
                    {
                        Text = "included = false (并列关系，不高亮区间):",
                        Style = new DefaultUIStyle { FontSize = 12, Color = new SKColor(0, 0, 0, 150), MarginTop = 16, MarginBottom = 4 }
                    },
                    new UISlider
                    {
                        DefaultValue = 37,
                        Min = 0,
                        Max = 100,
                        Included = false,
                        Marks = new Dictionary<float, string>
                        {
                            { 0, "0°C" },
                            { 26, "26°C" },
                            { 37, "37°C" },
                            { 100, "100°C" }
                        },
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            Height = 50,
                        },
                    },
                    // step=null
                    new UILabel
                    {
                        Text = "step = null (只能选择刻度位置):",
                        Style = new DefaultUIStyle { FontSize = 12, Color = new SKColor(0, 0, 0, 150), MarginTop = 16, MarginBottom = 4 }
                    },
                    new UISlider
                    {
                        DefaultValue = 37,
                        Min = 0,
                        Max = 100,
                        Step = null,
                        Marks = new Dictionary<float, string>
                        {
                            { 0, "0°C" },
                            { 26, "26°C" },
                            { 37, "37°C" },
                            { 100, "100°C" }
                        },
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            Height = 50,
                        },
                        OnChange = v => valueLabel2.Text = $"当前值: {(int)v} (step=null 只能选刻度)",
                    },
                    valueLabel2,
                };
            }
        }

        // ═══════════ 范围滑块 ═══════════
        class RangeSliderDemo : UIView
        {
            internal RangeSliderDemo()
            {
                ClassName = new List<string> { "slider-demo-card" };

                var valueLabel = new UILabel
                {
                    Text = "范围: 20 - 50",
                    ClassName = new List<string> { "slider-value-label" }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "范围滑块",
                        ClassName = new List<string> { "slider-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "当 Range 为 true 时，渲染为双滑块。可以选择一个区间范围。",
                        ClassName = new List<string> { "slider-card-desc" }
                    },
                    new UISlider
                    {
                        Range = true,
                        DefaultRange = new float[] { 20, 50 },
                        Min = 0,
                        Max = 100,
                        Step = 1,
                        Marks = new Dictionary<float, string>
                        {
                            { 0, "0" },
                            { 25, "25" },
                            { 50, "50" },
                            { 75, "75" },
                            { 100, "100" }
                        },
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            Height = 50,
                        },
                        OnRangeChange = (s, e) => valueLabel.Text = $"范围: {(int)s} - {(int)e}",
                    },
                    valueLabel,
                };
            }
        }

        // ═══════════ 禁用滑块 ═══════════
        class DisabledSliderDemo : UIView
        {
            internal DisabledSliderDemo()
            {
                ClassName = new List<string> { "slider-demo-card" };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "禁用状态",
                        ClassName = new List<string> { "slider-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "当 Disabled 为 true 时，滑块处于不可用状态。",
                        ClassName = new List<string> { "slider-card-desc" }
                    },
                    new UISlider
                    {
                        DefaultValue = 30,
                        Min = 0,
                        Max = 100,
                        Disabled = true,
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            Height = 40,
                        },
                    },
                };
            }
        }

        // ═══════════ 刻度尺Slider（ShowRuler）═══════════
        class RulerSliderDemo : UIView
        {
            internal RulerSliderDemo()
            {
                ClassName = new List<string> { "slider-demo-card" };

                var valueLabel = new UILabel
                {
                    Text = "当前值: 60",
                    ClassName = new List<string> { "slider-value-label" }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "刻度尺滑块（ShowRuler）",
                        ClassName = new List<string> { "slider-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "UISlider 启用 ShowRuler 后，显示与 UIThermometer 相同的主/次刻度线 + 主刻度数字 + 当前值高亮效果。",
                        ClassName = new List<string> { "slider-card-desc" }
                    },
                    new UISlider
                    {
                        Vertical = true,
                        DefaultValue = 60,
                        Min = 0,
                        Max = 100,
                        Step = 1,
                        // 刻度尺
                        ShowRuler = true,
                        RulerMajorStep = 10,
                        RulerMinorStep = 2,
                        RulerGap = 8,
                        RulerPosition = "right",
                        RulerMajorTickLength = 16,
                        RulerMinorTickLength = 8,
                        RulerFontSize = 13,
                        RulerActiveScale = 1.5f,
                        RulerTickColor = new SKColor(136, 136, 136),
                        RulerTextColor = new SKColor(136, 136, 136),
                        RulerTextActiveColor = new SKColor(51, 51, 51),
                        // 轨道加宽
                        RailSize = 20,
                        RailRadius = 0,
                        // 矩形手柄 + 握纹
                        HandleShape = "rect",
                        HandleRectWidth = 30,
                        HandleRectHeight = 30,
                        HandleRectRadius = 4,
                        HandleGripLines = 5,
                        HandleBorderWidth = 1,
                        // 配色 (青色渐变)
                        TrackGradientColors = new SKColor[]
                        {
                            new SKColor(0, 230, 127),
                            new SKColor(0, 223, 224),
                        },
                        TrackHoverGradientColors = new SKColor[]
                        {
                            new SKColor(0, 230, 127),
                            new SKColor(0, 223, 224),
                        },
                        HandleColor = new SKColor(0, 229, 204),
                        HandleBorderColor = new SKColor(0, 204, 179),
                        HandleActiveColor = new SKColor(0, 204, 179),
                        HandleGripColor = new SKColor(0, 0, 0, 100),
                        RailColor = new SKColor(221, 221, 221),
                        RailHoverColor = new SKColor(204, 204, 204),
                        Style = new DefaultUIStyle
                        {
                            Height = 300,
                        },
                        OnChange = v => valueLabel.Text = $"当前值: {(int)v}",
                    },
                    valueLabel,
                };
            }
        }

        // ═══════════ 温度计滑块（类 CodePen 效果）═══════════
        class ThermoSliderDemo : UIView
        {
            internal ThermoSliderDemo()
            {
                ClassName = new List<string> { "thermo-card" };

                var tempLabel = new UILabel
                {
                    Text = "CURRENT TEMP  COMFORTABLE",
                    ClassName = new List<string> { "thermo-label" }
                };

                Children = new()
                {
                    new UIView
                    {
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            Display = "flex",
                            FlexDirection = "column",
                            AlignItems = "center",
                        },
                        Children = new()
                        {
                            new UIThermometer
                            {
                                DefaultValue = 82,
                                Min = 20,
                                Max = 110,
                                Step = 1,
                                MajorStep = 10,
                                MinorStep = 2,
                                Unit = "°",
                                GlowColor = new SKColor(232, 232, 0),    // 亮黄
                                CapsuleColor = new SKColor(24, 24, 28),
                                CapsuleBorderColor = new SKColor(68, 68, 76),
                                TrackEmptyColor = new SKColor(34, 34, 40),
                                TickColor = new SKColor(136, 136, 136),
                                TickTextColor = new SKColor(136, 136, 136),
                                TickTextActiveColor = new SKColor(224, 224, 224),
                                CapsuleWidth = 80,
                                HandleDiameter = 32,
                                
                                TrackWidth = 16,
                                ValueFontSize = 52,
                                TickFontSize = 13,
                                GlowRadius = 18,
                                Style = new DefaultUIStyle
                                {
                                    Height = 560,
                                    PaddingTop = 10,
                                },
                                OnChange = v =>
                                {
                                    int temp = (int)v;
                                    string comfort;
                                    if (temp < 40) comfort = "COLD";
                                    else if (temp < 60) comfort = "COOL";
                                    else if (temp < 80) comfort = "COMFORTABLE";
                                    else if (temp < 95) comfort = "WARM";
                                    else comfort = "HOT";
                                    tempLabel.Text = $"CURRENT TEMP  {comfort}";
                                },
                            },
                        }
                    },
                    tempLabel,
                };
            }
        }
    }
}
