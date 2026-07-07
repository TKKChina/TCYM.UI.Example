using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Enums;
using TCYM.UI.Events;
using TCYM.UI.Helpers;

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

        private PenBoard _penBoard = null!;
        private UILabel _penStatus = null!;

        private UIView BuildPenSection()
        {
            _penBoard = new PenBoard
            {
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
            _penBoard.StatusChanged = text =>
            {
                _penStatus.Text = text;
                _penStatus.RequestRedraw();
            };

            _penStatus = new UILabel
            {
                Text = "用触控笔在画板上书写（无笔时可用鼠标）。压力越大，线条越粗。",
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
                    ToolButton("清空", () => _penBoard.Clear()),
                },
            };

            return Card("压感笔画板（Pen）",
                "SDL3 新增独立的笔子系统，事件带压力 / 倾斜 / 橡皮擦端 / 笔身按钮。",
                toolbar, _penBoard, _penStatus);
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
                    Click = _ =>
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
                Events = new() { Click = _ => onClick() },
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

        // ============================ 压感画板元素 ============================

        /// <summary>
        /// 压感画板：优先使用 SDL3 笔事件（真实压力 / 倾斜 / 橡皮擦），无笔时回退到鼠标 / 触摸。
        /// </summary>
        private sealed class PenBoard : UIView
        {
            private struct Pt { public float X; public float Y; public float P; }

            private sealed class Stroke
            {
                public SKColor Color;
                public bool IsEraser;
                public readonly List<Pt> Points = new();
            }

            private readonly List<Stroke> _strokes = new();
            private Stroke? _current;
            private bool _mouseDown;
            private long _lastPenTicks;

            /// <summary>当前画笔颜色。</summary>
            public SKColor StrokeColor = ColorHelper.ParseColor("#2260ff");

            /// <summary>橡皮擦模式（供鼠标使用；触控笔用翻转的橡皮擦端自动识别）。</summary>
            public bool EraserMode;

            /// <summary>板底色（用于橡皮擦覆盖）。</summary>
            private readonly SKColor _boardColor = ColorHelper.ParseColor("#ffffff");

            /// <summary>状态文本回调（压力 / 倾斜 / 橡皮擦）。</summary>
            public Action<string>? StatusChanged;

            /// <summary>清空画板。</summary>
            public void Clear()
            {
                _strokes.Clear();
                _current = null;
                RequestRedraw();
            }

            // ---- SDL3 笔事件（带压力）----
            public override void OnPenEvent(UIPenEvent e)
            {
                base.OnPenEvent(e);
                _lastPenTicks = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                var b = GetAbsoluteBounds();
                float lx = e.Position.X - b.Left;
                float ly = e.Position.Y - b.Top;

                switch (e.Type)
                {
                    case UIPenEventType.Down:
                        BeginStroke(lx, ly, e.Pressure, e.IsEraser);
                        break;
                    case UIPenEventType.Move:
                        if (e.IsTipDown && _current != null) AppendPoint(lx, ly, e.Pressure);
                        break;
                    case UIPenEventType.Up:
                        EndStroke();
                        break;
                }

                StatusChanged?.Invoke(
                    $"笔：{e.Type}  压力={e.Pressure:0.00}  倾斜=({e.TiltX:0.#},{e.TiltY:0.#})  橡皮擦={(e.IsEraser ? "是" : "否")}  按钮={e.Button}");
            }

            // ---- 鼠标 / 触摸回退（无压力，压力取 0.5）----
            public override void OnPointerDown(float x, float y)
            {
                base.OnPointerDown(x, y);
                if (IsRecentPen()) return; // 忽略笔合成的鼠标事件，避免重复绘制
                _mouseDown = true;
                BeginStroke(x, y, 0.5f, EraserMode);
            }

            public override void OnPointerMove(float x, float y)
            {
                base.OnPointerMove(x, y);
                if (!_mouseDown || _current == null) return;
                if (IsRecentPen()) return;
                AppendPoint(x, y, 0.5f);
            }

            public override void OnPointerUp(float x, float y)
            {
                base.OnPointerUp(x, y);
                if (!_mouseDown) return;
                _mouseDown = false;
                EndStroke();
            }

            private bool IsRecentPen() => DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastPenTicks < 250;

            private void BeginStroke(float x, float y, float pressure, bool eraser)
            {
                _current = new Stroke { Color = StrokeColor, IsEraser = eraser };
                _current.Points.Add(new Pt { X = x, Y = y, P = pressure });
                _strokes.Add(_current);
                RequestRedraw();
            }

            private void AppendPoint(float x, float y, float pressure)
            {
                if (_current == null) return;
                var last = _current.Points[_current.Points.Count - 1];
                // 简单去抖：太近的点跳过
                if ((x - last.X) * (x - last.X) + (y - last.Y) * (y - last.Y) < 1.5f) return;
                _current.Points.Add(new Pt { X = x, Y = y, P = pressure });
                RequestRedraw();
            }

            private void EndStroke()
            {
                _current = null;
            }

            protected override void RenderContent(SKCanvas canvas)
            {
                base.RenderContent(canvas);

                if (_strokes.Count == 0)
                {
                    return;
                }

                using var paint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeCap = SKStrokeCap.Round,
                    StrokeJoin = SKStrokeJoin.Round,
                };

                foreach (var s in _strokes)
                {
                    paint.Color = s.IsEraser ? _boardColor : s.Color;

                    if (s.Points.Count == 1)
                    {
                        var p = s.Points[0];
                        float r = s.IsEraser ? 10f : 1.5f + p.P * 6f;
                        using var dot = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = paint.Color };
                        canvas.DrawCircle(p.X, p.Y, r, dot);
                        continue;
                    }

                    for (int i = 1; i < s.Points.Count; i++)
                    {
                        var a = s.Points[i - 1];
                        var c = s.Points[i];
                        float pr = (a.P + c.P) * 0.5f;
                        paint.StrokeWidth = s.IsEraser ? 20f : 2f + pr * 12f;
                        canvas.DrawLine(a.X, a.Y, c.X, c.Y, paint);
                    }
                }
            }
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
