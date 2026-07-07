using System;
using System.Collections.Generic;
using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Events;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Gamepad
{
    /// <summary>
    /// 手柄可视化 Demo：以 Xbox One 手柄轮廓图（xbox.png）为底，叠加实时输入可视化——
    /// 按下按钮高亮、推动摇杆偏移、LT/RT 扳机随力度橙色填充、方向键点亮，底部显示连接状态。
    /// 布局对齐参考工程 TCYM.XboxOneQuanba 的 XboxOneController 版式（517×357 坐标系）。
    /// </summary>
    internal class UIGamepadDemo : UIScrollView
    {
        internal UIGamepadDemo()
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
                new UILabel
                {
                    Text = "手柄可视化",
                    Style = new DefaultUIStyle { FontSize = 22, FontWeight = 700, Color = ColorHelper.ParseColor("#111827") },
                },
                new UILabel
                {
                    Text = "连接手柄后按键 / 推杆即可实时高亮；支持 SDL3 Gamepad、Joystick 与 winmm 后备设备。",
                    Style = new DefaultUIStyle { FontSize = 13, Color = ColorHelper.ParseColor("#6b7280"), MarginTop = 6, MarginBottom = 16 },
                },
                new GamepadView
                {
                    Style = new DefaultUIStyle
                    {
                        Width = "100%",
                        Height = 470,
                        BackgroundColor = ColorHelper.ParseColor("#2b2b2b"),
                        BorderRadius = 14,
                        Overflow = "hidden",
                    },
                },
                new UILabel
                {
                    Text = "提示：A绿 / B红 / X蓝 / Y黄；LT/RT 扳机随力度橙色填充；左右摇杆随轴偏移；方向键与 View/Menu/Xbox 键按下高亮。",
                    Style = new DefaultUIStyle { FontSize = 12, Color = ColorHelper.ParseColor("#9ca3af"), MarginTop = 14 },
                },
            };
        }

        /// <summary>
        /// 手柄绘制元素：订阅全局摇杆事件、维护实时状态，用 Skia 在 xbox.png 轮廓上叠加交互可视化。
        /// 采用 517×357 虚拟坐标（等于底图尺寸），等比缩放居中到元素内容区。
        /// </summary>
        private sealed class GamepadView : UIView, IDisposable
        {
            // 虚拟坐标系 = xbox.png 尺寸。
            private const float VW = 517f;
            private const float VH = 357f;

            // 配色（对齐参考工程）。
            private static readonly SKColor White = new(0xF0, 0xF0, 0xF0);
            private static readonly SKColor Gray = new(0x88, 0x88, 0x88);
            private static readonly SKColor XBlue = new(0x04, 0xB0, 0xFF);
            private static readonly SKColor YYellow = new(0xEC, 0xDC, 0x0E);
            private static readonly SKColor AGreen = new(0x7A, 0xBF, 0x61);
            private static readonly SKColor BRed = new(0xEF, 0x4A, 0x4D);
            private static readonly SKColor Orange = new(0xF3, 0x8F, 0x2A);

            // 底图（进程内共享，只解码一次）。
            private static SKImage? s_body;
            private static bool s_bodyLoaded;

            // 运行时状态（由摇杆事件更新）。
            private readonly HashSet<int> _connected = new();
            private readonly HashSet<int> _buttons = new();
            private float _lx, _ly, _rx, _ry, _lt, _rt;
            private UIJoystickHatState _hat = UIJoystickHatState.Centered;
            private UIHIDDeviceType _deviceType = UIHIDDeviceType.Gamepad;
            private int _activeId;
            private bool _subscribed;

            public GamepadView()
            {
                var mgr = UISystem.Manager;
                if (mgr != null)
                {
                    mgr.OnAnyJoystickEvent += HandleJoystick;
                    _subscribed = true;
                }
            }

            public void Dispose()
            {
                if (!_subscribed) return;
                var mgr = UISystem.Manager;
                if (mgr != null) mgr.OnAnyJoystickEvent -= HandleJoystick;
                _subscribed = false;
            }

            private static SKImage? Body()
            {
                if (s_bodyLoaded) return s_body;
                s_bodyLoaded = true;
                try
                {
                    var asm = typeof(UIGamepadDemo).Assembly;
                    string? name = Array.Find(asm.GetManifestResourceNames(),
                        n => n.EndsWith("xbox.png", StringComparison.OrdinalIgnoreCase));
                    if (name != null)
                    {
                        using var stream = asm.GetManifestResourceStream(name);
                        if (stream != null)
                        {
                            using var data = SKData.Create(stream);
                            s_body = SKImage.FromEncodedData(data);
                        }
                    }
                }
                catch { }
                return s_body;
            }

            private void HandleJoystick(UIJoystickEvent e)
            {
                if (e == null) return;
                switch (e.Type)
                {
                    case UIJoystickEventType.DeviceAdded:
                        _connected.Add(e.DeviceId);
                        _activeId = e.DeviceId;
                        _deviceType = e.DeviceType;
                        break;
                    case UIJoystickEventType.DeviceRemoved:
                        _connected.Remove(e.DeviceId);
                        if (_connected.Count == 0) ResetInputs();
                        break;
                    case UIJoystickEventType.ButtonDown:
                        _activeId = e.DeviceId;
                        _buttons.Add(e.Button);
                        break;
                    case UIJoystickEventType.ButtonUp:
                        _buttons.Remove(e.Button);
                        break;
                    case UIJoystickEventType.AxisMotion:
                        _activeId = e.DeviceId;
                        ApplyAxis(e.Axis, e.AxisNormalized);
                        break;
                    case UIJoystickEventType.HatMotion:
                        _hat = e.HatState;
                        break;
                }
                RequestRedraw();
            }

            private void ResetInputs()
            {
                _buttons.Clear();
                _lx = _ly = _rx = _ry = _lt = _rt = 0;
                _hat = UIJoystickHatState.Centered;
            }

            private void ApplyAxis(int axis, float v)
            {
                switch (axis)
                {
                    case 0: _lx = v; break;
                    case 1: _ly = v; break;
                    case 2: _rx = v; break;
                    case 3: _ry = v; break;
                    case 4: _lt = Math.Clamp(v, 0f, 1f); break;
                    case 5: _rt = Math.Clamp(v, 0f, 1f); break;
                }
            }

            protected override void RenderContent(SKCanvas canvas)
            {
                base.RenderContent(canvas);
                if (canvas == null) return;

                float w = Width, h = Height;
                if (w <= 1 || h <= 1) return;

                float scale = Math.Min(w / VW, h / VH) * 0.98f;
                float ox = (w - VW * scale) / 2f;
                float oy = (h - VH * scale) / 2f;

                int save = canvas.Save();
                canvas.Translate(ox, oy);
                canvas.Scale(scale);

                DrawBody(canvas);
                DrawTriggers(canvas);
                DrawGuide(canvas);
                DrawCenterButtons(canvas);
                DrawStick(canvas, 133.5f, 98.5f, _lx, _ly, _buttons.Contains(7));   // 左摇杆 (L3=7)
                DrawStick(canvas, 318.5f, 188.5f, _rx, _ry, _buttons.Contains(8));  // 右摇杆 (R3=8)
                DrawDpad(canvas, 168.5f, 188.5f);
                DrawAbxy(canvas);
                DrawStatus(canvas);

                canvas.RestoreToCount(save);
            }

            // ======================= 部件绘制 =======================

            private static void DrawBody(SKCanvas canvas)
            {
                var body = Body();
                if (body == null) return;
                canvas.DrawImage(body, SKRect.Create(0, 0, VW, VH),
                    new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None));
            }

            // LT / RT 扳机徽标（顶部两角），按力度橙色填充。
            private void DrawTriggers(SKCanvas canvas)
            {
                DrawTrigger(canvas, 30f, 30f, "LT", Math.Max(_lt, _buttons.Contains(9) ? 1f : 0f));
                DrawTrigger(canvas, 487f, 30f, "RT", Math.Max(_rt, _buttons.Contains(10) ? 1f : 0f));
            }

            private void DrawTrigger(SKCanvas canvas, float cx, float cy, string label, float amount)
            {
                const float r = 20f;
                if (amount > 0.01f)
                {
                    using var glow = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = Orange.WithAlpha((byte)(70 + 170 * Math.Clamp(amount, 0f, 1f))) };
                    canvas.DrawCircle(cx, cy, r, glow);
                }
                using var ring = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 2.5f, Color = White };
                canvas.DrawCircle(cx, cy, r, ring);
                using var tp = new SKPaint { IsAntialias = true, Color = White };
                using var font = new SKFont(UIElement.DefaultTypeface, 16f) { Embolden = true };
                DrawCenteredText(canvas, label, cx, cy, font, tp);
            }

            // Xbox 指南键：底图已画标志，仅在按下时叠加高亮圈。
            private void DrawGuide(SKCanvas canvas)
            {
                if (!_buttons.Contains(5)) return;
                const float cx = 258.5f, cy = 48f, r = 20f;
                using var fill = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = AGreen.WithAlpha(120) };
                canvas.DrawCircle(cx, cy, r, fill);
                using var ring = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 2.5f, Color = AGreen };
                canvas.DrawCircle(cx, cy, r, ring);
            }

            // View（双方块）与 Menu（三横线）中央键。
            private void DrawCenterButtons(SKCanvas canvas)
            {
                DrawViewButton(canvas, 222.5f, 98.5f, _buttons.Contains(4));   // BACK / View
                DrawMenuButton(canvas, 294.5f, 98.5f, _buttons.Contains(6));   // START / Menu
            }

            private static void DrawViewButton(SKCanvas canvas, float cx, float cy, bool pressed)
            {
                SKColor c = pressed ? AGreen : White;
                if (pressed)
                {
                    using var f = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = AGreen.WithAlpha(90) };
                    canvas.DrawCircle(cx, cy, 14f, f);
                }
                using var ring = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 2f, Color = c };
                canvas.DrawCircle(cx, cy, 14f, ring);
                using var icon = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1.6f, Color = c };
                canvas.DrawRect(SKRect.Create(cx - 5f, cy - 4f, 7f, 7f), icon);
                canvas.DrawRect(SKRect.Create(cx - 1f, cy - 1f, 7f, 7f), icon);
            }

            private static void DrawMenuButton(SKCanvas canvas, float cx, float cy, bool pressed)
            {
                SKColor c = pressed ? AGreen : White;
                if (pressed)
                {
                    using var f = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = AGreen.WithAlpha(90) };
                    canvas.DrawCircle(cx, cy, 14f, f);
                }
                using var ring = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 2f, Color = c };
                canvas.DrawCircle(cx, cy, 14f, ring);
                using var icon = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1.6f, StrokeCap = SKStrokeCap.Round, Color = c };
                for (int i = -1; i <= 1; i++)
                {
                    float y = cy + i * 4f;
                    canvas.DrawLine(cx - 6f, y, cx + 6f, y, icon);
                }
            }

            // 摇杆：外环 + 随轴偏移的内圈；按下（L3/R3）高亮。
            private void DrawStick(SKCanvas canvas, float cx, float cy, float ax, float ay, bool pressed)
            {
                const float outerR = 40f, knobR = 24f, travel = 13f;
                using var ring = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 2.5f, Color = White };
                canvas.DrawCircle(cx, cy, outerR, ring);

                float kx = cx + Math.Clamp(ax, -1f, 1f) * travel;
                float ky = cy + Math.Clamp(ay, -1f, 1f) * travel;
                using var knob = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = pressed ? AGreen : White };
                canvas.DrawCircle(kx, ky, knobR, knob);
                using var knobRing = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1.5f, Color = Gray };
                canvas.DrawCircle(kx, ky, knobR, knobRing);
                // 4 个凹点细节（对齐参考 IceXboxRocker）
                using var dot = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = Gray };
                float dd = knobR - 6f;
                canvas.DrawCircle(kx, ky - dd, 1.6f, dot);
                canvas.DrawCircle(kx, ky + dd, 1.6f, dot);
                canvas.DrawCircle(kx - dd, ky, 1.6f, dot);
                canvas.DrawCircle(kx + dd, ky, 1.6f, dot);
            }

            // 十字方向键（单一轮廓），按方向键或帽子（POV）点亮对应臂。
            private void DrawDpad(SKCanvas canvas, float cx, float cy)
            {
                const float arm = 24f, half = 9f;
                bool up = _buttons.Contains(11) || _hat.HasFlag(UIJoystickHatState.Up);
                bool down = _buttons.Contains(12) || _hat.HasFlag(UIJoystickHatState.Down);
                bool left = _buttons.Contains(13) || _hat.HasFlag(UIJoystickHatState.Left);
                bool right = _buttons.Contains(14) || _hat.HasFlag(UIJoystickHatState.Right);

                using (var fill = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = AGreen })
                {
                    if (up) canvas.DrawRect(SKRect.Create(cx - half, cy - arm, half * 2, arm), fill);
                    if (down) canvas.DrawRect(SKRect.Create(cx - half, cy, half * 2, arm), fill);
                    if (left) canvas.DrawRect(SKRect.Create(cx - arm, cy - half, arm, half * 2), fill);
                    if (right) canvas.DrawRect(SKRect.Create(cx, cy - half, arm, half * 2), fill);
                }

                using var b = new SKPathBuilder();
                b.MoveTo(cx - half, cy - arm);
                b.LineTo(cx + half, cy - arm);
                b.LineTo(cx + half, cy - half);
                b.LineTo(cx + arm, cy - half);
                b.LineTo(cx + arm, cy + half);
                b.LineTo(cx + half, cy + half);
                b.LineTo(cx + half, cy + arm);
                b.LineTo(cx - half, cy + arm);
                b.LineTo(cx - half, cy + half);
                b.LineTo(cx - arm, cy + half);
                b.LineTo(cx - arm, cy - half);
                b.LineTo(cx - half, cy - half);
                b.Close();
                using var path = b.Detach();
                using var stroke = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 2.5f, Color = White, StrokeJoin = SKStrokeJoin.Round };
                canvas.DrawPath(path, stroke);
            }

            // ABXY 按钮簇（Y上 / X左 / B右 / A下）。
            private void DrawAbxy(SKCanvas canvas)
            {
                const float cx = 383.5f, cy = 103.5f, off = 35f, r = 20f;
                DrawRoundButton(canvas, cx, cy - off, r, "Y", YYellow, _buttons.Contains(3));
                DrawRoundButton(canvas, cx - off, cy, r, "X", XBlue, _buttons.Contains(2));
                DrawRoundButton(canvas, cx + off, cy, r, "B", BRed, _buttons.Contains(1));
                DrawRoundButton(canvas, cx, cy + off, r, "A", AGreen, _buttons.Contains(0));
            }

            private static void DrawRoundButton(SKCanvas canvas, float cx, float cy, float r, string label, SKColor color, bool pressed)
            {
                if (pressed)
                {
                    using var fill = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = color };
                    canvas.DrawCircle(cx, cy, r, fill);
                }
                using var ring = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 2.5f, Color = pressed ? color : White };
                canvas.DrawCircle(cx, cy, r, ring);

                using var tp = new SKPaint { IsAntialias = true, Color = pressed ? SKColors.White : color };
                using var font = new SKFont(UIElement.DefaultTypeface, 22f) { Embolden = true };
                DrawCenteredText(canvas, label, cx, cy, font, tp);
            }

            // 底部连接状态。
            private void DrawStatus(SKCanvas canvas)
            {
                bool connected = _connected.Count > 0;
                string text = connected
                    ? $"已连接 · {(_deviceType == UIHIDDeviceType.Gamepad ? "手柄" : "摇杆")} #{_activeId}"
                    : "未连接";
                using var paint = new SKPaint { IsAntialias = true, Color = connected ? AGreen : Orange };
                using var font = new SKFont(UIElement.DefaultTypeface, 18f);
                DrawCenteredText(canvas, text, 258.5f, 340f, font, paint);
            }

            private static void DrawCenteredText(SKCanvas canvas, string text, float cx, float cy, SKFont font, SKPaint paint)
            {
                var m = font.Metrics;
                float baseline = cy - (m.Ascent + m.Descent) / 2f;
                canvas.DrawText(text, cx, baseline, SKTextAlign.Center, font, paint);
            }
        }
    }
}
