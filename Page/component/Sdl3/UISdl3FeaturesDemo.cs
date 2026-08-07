using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Enums;
using TCYM.UI.Events;
using TCYM.UI.Helpers;
using TCYM.UI.SDL3;

namespace TCYM.UI.Example.Page.component.Sdl3
{
    /// <summary>
    /// SDL3 新特性演示页：压感笔画板（Pen）+ 文件拖放导入（Drop）。
    /// 这些能力 SDL2 都不支持，迁移到 SDL3 后才可用。
    /// </summary>
    internal class UISdl3FeaturesDemo : UIScrollView
    {
        internal UISdl3FeaturesDemo()
        {
            Style = new DefaultUIStyle
            {
                Width = "100%",
                Height = "100%",
                BackgroundColor = ColorHelper.ParseColor("#f5f7fa"),
                PaddingLeft = 24,
                PaddingRight = 24,
                PaddingTop = 20,
                PaddingBottom = 40,
                Display = "flex",
                FlexDirection = "column",
            };

            Children = new()
            {
                Title("SDL3 新特性", "以下能力 SDL2 不支持，升级到 SDL3 后才可用。"),
                BuildPenSection(),
                BuildDropSection(),
                BuildInfoSection(),
            };
        }

        // ============================ 压感笔画板 ============================

        private UIPenBoard _penBoard = null!;
        private UILabel _penStatus = null!;

        private UIView BuildPenSection()
        {
            string sdlVersion = GetSdlVersionText();
            _penBoard = new UIPenBoard
            {
                UseFixedStrokeWidth = false,
                MinStrokeWidth = 1.5f,
                MaxStrokeWidth = 20f,
                PressureSmoothing = 0.65f,
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = 360,
                    BackgroundColor = ColorHelper.ParseColor("#ffffff"),
                    BorderRadius = 10,
                    BorderWidth = 1,
                    BorderColor = ColorHelper.ParseColor("#e5e7eb"),
                    Overflow = "hidden",
                },
            };
            _penBoard.OnPen += e =>
            {
                float width = _penBoard.UseFixedStrokeWidth
                    ? _penBoard.FixedStrokeWidth
                    : _penBoard.MinStrokeWidth + e.Pressure *
                        (_penBoard.MaxStrokeWidth - _penBoard.MinStrokeWidth);
                _penStatus.Text =
                    $"SDL={sdlVersion}  输入=Pen  事件={e.Type}  压力={e.Pressure:0.00}  线宽≈{width:0.0}px  " +
                    $"倾斜=({e.TiltX:0.#},{e.TiltY:0.#})  " +
                    $"橡皮擦={(e.IsEraser ? "是" : "否")}  按钮={e.Button}";
                _penStatus.RequestRedraw();
            };
            _penBoard.InputSampled += (source, pressure) =>
            {
                // 原生 Pen 的详细状态由 OnPen 显示；此处专门显示 Windows 驱动的兼容回退链路。
                if (source == UIPenBoardInputSource.Pen)
                {
                    return;
                }

                float width = _penBoard.UseFixedStrokeWidth
                    ? _penBoard.FixedStrokeWidth
                    : _penBoard.MinStrokeWidth + pressure *
                        (_penBoard.MaxStrokeWidth - _penBoard.MinStrokeWidth);
                string sourceText = source switch
                {
                    UIPenBoardInputSource.PenTouch => "PenTouch（笔兼容触摸）",
                    UIPenBoardInputSource.Touch => "Touch",
                    _ => "Mouse",
                };
                _penStatus.Text =
                    $"SDL={sdlVersion}  输入={sourceText}  压力={pressure:0.00}  线宽≈{width:0.0}px";
                _penStatus.RequestRedraw();
            };

            _penStatus = new UILabel
            {
                Text = $"SDL={sdlVersion}。用触控笔在画板上书写，压力越大，线条越粗。",
                Style = new DefaultUIStyle
                {
                    FontSize = 13,
                    Color = ColorHelper.ParseColor("#6b7280"),
                    MarginTop = 8,
                },
            };

            var toolbar = new UIView
            {
                Style = new DefaultUIStyle
                {
                    Display = "flex",
                    FlexDirection = "row",
                    AlignItems = "center",
                    Gap = 10,
                    MarginBottom = 12,
                },
                Children = new()
                {
                    ColorSwatch("#2260ff"),
                    ColorSwatch("#ff4d4f"),
                    ColorSwatch("#16a34a"),
                    ColorSwatch("#f59e0b"),
                    ColorSwatch("#111827"),
                    ToolButton("橡皮擦", () =>
                    {
                        _penBoard.EraserMode = !_penBoard.EraserMode;
                        _penStatus.Text = _penBoard.EraserMode ? "橡皮擦：开（鼠标也会擦除；触控笔翻转即橡皮擦端）" : "橡皮擦：关";
                        _penStatus.RequestRedraw();
                    }),
                    ToolButton("固定粗细", () =>
                    {
                        _penBoard.UseFixedStrokeWidth = !_penBoard.UseFixedStrokeWidth;
                        _penStatus.Text = _penBoard.UseFixedStrokeWidth
                            ? $"固定粗细：开（{_penBoard.FixedStrokeWidth:0.#} px，不使用压力）"
                            : "固定粗细：关（根据触控笔压力改变线宽）";
                        _penStatus.RequestRedraw();
                    }),
                    ToolButton("撤销", () => _penBoard.Undo()),
                    ToolButton("重做", () => _penBoard.Redo()),
                    ToolButton("清空", () => _penBoard.Clear()),
                },
            };

            return Card("压感笔画板（Pen）",
                "SDL3 新增独立的笔子系统，事件带压力 / 倾斜 / 橡皮擦端 / 笔身按钮。",
                toolbar, _penBoard, _penStatus);
        }

        private static string GetSdlVersionText()
        {
            try
            {
                int version = SDL.SDL_GetVersion();
                int major = version / 1_000_000;
                int minor = version / 1_000 % 1_000;
                int patch = version % 1_000;
                return $"{major}.{minor}.{patch}";
            }
            catch
            {
                return "unknown";
            }
        }

        private UIView ColorSwatch(string hex)
        {
            var color = ColorHelper.ParseColor(hex);
            return new UIView
            {
                Style = new DefaultUIStyle
                {
                    Width = 26,
                    Height = 26,
                    BackgroundColor = color,
                    BorderRadius = 13,
                    BorderWidth = 2,
                    BorderColor = ColorHelper.ParseColor("#ffffff"),
                    Cursor = UICursor.Pointer,
                },
                Events = new()
                {
                    PointerPressed = _ =>
                    {
                        _penBoard.StrokeColor = color;
                        _penBoard.EraserMode = false;
                    }
                },
            };
        }

        private static UIButton ToolButton(string text, Action onClick)
        {
            return new UIButton
            {
                Text = text,
                Style = new DefaultUIStyle
                {
                    FontSize = 13,
                    PaddingLeft = 12,
                    PaddingRight = 12,
                    PaddingTop = 6,
                    PaddingBottom = 6,
                    BackgroundColor = ColorHelper.ParseColor("#eef2ff"),
                    Color = ColorHelper.ParseColor("#3949ab"),
                    BorderRadius = 6,
                    Cursor = UICursor.Pointer,
                },
                Events = new() { PointerPressed = _ => onClick() },
            };
        }

        // ============================ 文件拖放导入 ============================

        private UIView BuildDropSection()
        {
            var dropZone = new DropZone
            {
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Height = 180,
                    BackgroundColor = ColorHelper.ParseColor("#fafafa"),
                    BorderRadius = 10,
                    Display = "flex",
                    FlexDirection = "column",
                    AlignItems = "center",
                    JustifyContent = "center",
                    Overflow = "hidden",
                },
            };

            return Card("文件拖放导入（Drop）",
                "把文件从资源管理器拖进窗口，SDL3 还新增了拖动过程中的位置更新（DROP_POSITION）。",
                dropZone);
        }

        // ============================ 说明 ============================

        private UIView BuildInfoSection()
        {
            var text = string.Join("\n",
                "· 触摸取消（FINGER_CANCELED → TouchCancel）：手势被系统打断时会正确取消，元素的 OnTouchCancel 现在可用。",
                "· 窗口状态事件（最小化 / 最大化 / 恢复 / 显示器切换）：会自动重新同步窗口度量并重绘。",
                "· 以上均为 SDL3 新增，SDL2 时代不可用。");

            return Card("其他已接入的 SDL3 事件",
                "无需交互，后台已生效。",
                new UILabel
                {
                    Text = text,
                    Style = new DefaultUIStyle
                    {
                        FontSize = 13,
                        Color = ColorHelper.ParseColor("#4b5563"),
                    },
                });
        }

        // ============================ 通用小部件 ============================

        private static UIView Title(string title, string subtitle)
        {
            return new UIView
            {
                Style = new DefaultUIStyle { Display = "flex", FlexDirection = "column", MarginBottom = 16 },
                Children = new()
                {
                    new UILabel
                    {
                        Text = title,
                        Style = new DefaultUIStyle { FontSize = 24, FontWeight = 700, Color = ColorHelper.ParseColor("#111827") },
                    },
                    new UILabel
                    {
                        Text = subtitle,
                        Style = new DefaultUIStyle { FontSize = 14, Color = ColorHelper.ParseColor("#6b7280"), MarginTop = 6 },
                    },
                },
            };
        }

        private static UIView Card(string title, string description, params UIElement[] children)
        {
            var list = new List<UIElement>
            {
                new UILabel { Text = title, Style = new DefaultUIStyle { FontSize = 17, FontWeight = 700, Color = ColorHelper.ParseColor("#1f2937"), MarginBottom = 4 } },
                new UILabel { Text = description, Style = new DefaultUIStyle { FontSize = 13, Color = ColorHelper.ParseColor("#6b7280"), MarginBottom = 14 } },
            };
            list.AddRange(children);

            return new UIView
            {
                Style = new DefaultUIStyle
                {
                    Width = "100%",
                    Display = "flex",
                    FlexDirection = "column",
                    BackgroundColor = ColorHelper.ParseColor("#ffffff"),
                    BorderRadius = 12,
                    PaddingLeft = 20,
                    PaddingRight = 20,
                    PaddingTop = 18,
                    PaddingBottom = 20,
                    MarginBottom = 20,
                    BorderWidth = 1,
                    BorderColor = ColorHelper.ParseColor("#eef0f3"),
                },
                Children = list,
            };
        }

        // ============================ 拖放落点元素 ============================

        /// <summary>
        /// 文件拖放落点：拖动经过时高亮（DROP_POSITION），松手后展示拖入的文件路径。
        /// </summary>
        private sealed class DropZone : UIView
        {
            private readonly UILabel _hint;
            private readonly UILabel _result;
            private bool _hover;

            public DropZone()
            {
                _hint = new UILabel
                {
                    Text = "把文件拖到这里导入",
                    Style = new DefaultUIStyle { FontSize = 16, FontWeight = 700, Color = ColorHelper.ParseColor("#374151") },
                };
                _result = new UILabel
                {
                    Text = "（尚未拖入文件）",
                    Style = new DefaultUIStyle { FontSize = 13, Color = ColorHelper.ParseColor("#9ca3af"), MarginTop = 10 },
                };
                Children = new() { _hint, _result };
                ApplyHoverStyle();
            }

            public override void OnFileDropEvent(UIFileDropEvent e)
            {
                base.OnFileDropEvent(e);

                if (e.Phase == UIFileDropPhase.Over)
                {
                    if (!_hover)
                    {
                        _hover = true;
                        ApplyHoverStyle();
                    }
                    return;
                }

                // Drop
                _hover = false;
                ApplyHoverStyle();

                var lines = new List<string>();
                if (e.Files != null && e.Files.Count > 0)
                {
                    lines.Add($"共拖入 {e.Files.Count} 个文件：");
                    foreach (var f in e.Files.Take(6))
                    {
                        lines.Add($"· {Path.GetFileName(f)}   （{f}）");
                    }
                    if (e.Files.Count > 6) lines.Add($"· 其余 {e.Files.Count - 6} 个已省略");
                }
                else if (!string.IsNullOrEmpty(e.Text))
                {
                    lines.Add("拖入文本：");
                    lines.Add(e.Text!);
                }
                else
                {
                    lines.Add("（未获取到文件或文本）");
                }

                _result.Text = string.Join("\n", lines);
                _result.Style = new DefaultUIStyle { FontSize = 13, Color = ColorHelper.ParseColor("#374151"), MarginTop = 10 };
                e.Handled = true;
                RequestRedraw();
            }

            private void ApplyHoverStyle()
            {
                BackgroundColor = _hover ? ColorHelper.ParseColor("#eef5ff") : ColorHelper.ParseColor("#fafafa");
                BorderColor = _hover ? ColorHelper.ParseColor("#2260ff") : ColorHelper.ParseColor("#d1d5db");
                BorderWidth = 2;
                _hint.Text = _hover ? "松手即可导入" : "把文件拖到这里导入";
                RequestRedraw();
            }
        }
    }
}
