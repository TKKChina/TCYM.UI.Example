using TCYM.UI.Core;
using TCYM.UI.Elements;

namespace TCYM.UI.Example.Page.component.Glass;

internal sealed partial class UIGlassDemo
{
    /// <summary>演示有限动画与分属性过渡；默认暂停，只由按钮和实际悬停驱动，不建立定时器。</summary>
    private static UIView CreateAnimationSection()
    {
        var clocks = new UILabel { Id = "css-motion-clocks", Text = "双时钟", ClassName = "css-motion-actor css-motion-clocks" };
        var steps = new UILabel { Id = "css-motion-steps", Text = "5 阶", ClassName = "css-motion-actor css-motion-steps" };
        var spring = new UILabel { Id = "css-motion-bezier", Text = "回弹", ClassName = "css-motion-actor css-motion-bezier" };
        (UIElement Element, string Names)[] samples =
        [
            (clocks, "glass-motion-slide, glass-motion-fade"),
            (steps, "glass-motion-step"),
            (spring, "glass-motion-spring"),
        ];
        var status = new UILabel
        {
            Id = "css-motion-status",
            ClassName = "css-motion-status",
            Text = "默认暂停 · 点击开始；每条动画仅播放 2 次，结束后自动停止。",
        };

        void SetPlayback(string state)
        {
            foreach (var sample in samples)
                sample.Element.UpdateStyle(new UpdateUIStyle { AnimationPlayState = state });
            status.Text = state == "paused" ? "已暂停 · 再次开始会延续原进度；重播从起点重新演示。"
                : "播放中 · 不同时钟独立推进；最多 4 秒结束，结束后可点击重播。";
        }

        void Replay()
        {
            foreach (var sample in samples)
            {
                // 先让旧实例真正退出，再恢复名称列表；不覆盖 CSS 的时长、缓动和方向。
                sample.Element.UpdateStyle(new UpdateUIStyle { AnimationName = "none" });
                sample.Element.Update(0f);
                var restore = new DefaultUIStyle();
                restore.ParseFromCss($"animation-name:{sample.Names}; animation-play-state:running;");
                sample.Element.UpdateStyle(restore);
            }
            status.Text = "已重播 · CSS 中的名称列表、独立时长与有限次数保持不变。";
        }

        return new UIView
        {
            Id = "css-motion-demo-section",
            ClassName = "glass-demo-card",
            Children = new()
            {
                CreateSectionTitle("动画与过渡"),
                CreateSectionDescription("关键帧支持多轨道、独立时钟、alternate、steps 与贝塞尔回弹；右下角使用 transition 长属性，为不同属性设置独立时长和延迟。"),
                new UIView
                {
                    ClassName = "css-vars-controls",
                    Children = new()
                    {
                        new UIButton { Id = "css-motion-start", Text = "开始 / 继续", ClassName = "css-vars-button", Events = new() { Click = _ => SetPlayback("running") } },
                        new UIButton { Id = "css-motion-pause", Text = "暂停", ClassName = "css-vars-button", Events = new() { Click = _ => SetPlayback("paused") } },
                        new UIButton { Id = "css-motion-replay", Text = "从头重播", ClassName = "css-vars-button", Events = new() { Click = _ => Replay() } },
                    },
                },
                status,
                new UIView
                {
                    ClassName = "glass-selector-grid",
                    Children = new()
                    {
                        CreateMotionCard("两个动画，两个时钟", "位移 2 秒 / 透明度 1 秒；均 alternate × 2，短轨道先结束。", clocks,
                            "animation: slide 2s linear 2 alternate both paused,\n           fade 1s linear 2 alternate both paused;"),
                        CreateMotionCard("steps：逐格推进", "每轮 2 秒、分成 5 阶；第二轮反向返回，不会无限循环。", steps,
                            "animation: step 2s steps(5,end) 2 alternate both paused;"),
                        CreateMotionCard("cubic-bezier：保留回弹", "Y 控制点可超出 0..1，橙色方块先回拉再越过终点。", spring,
                            "animation: spring 1.6s cubic-bezier(.3,-.5,.7,1.5)\n           2 alternate both paused;"),
                        new UIView
                        {
                            ClassName = "glass-selector-example css-motion-card",
                            Children = new()
                            {
                                new UILabel { Text = "分属性 transition + delay", ClassName = "glass-selector-example-title" },
                                new UILabel { Text = "悬停观察：底色立即变化，位移延迟 120ms，透明度延迟 200ms。", ClassName = "glass-selector-example-desc" },
                                new UILabel { Id = "css-motion-hover", Text = "把鼠标移到这里", ClassName = "css-motion-hover" },
                                new UILabel { Text = "transition-property: background-color, transform, opacity, color;\ntransition-duration: .35s, .7s, .4s, .35s;\ntransition-delay: 0s, .12s, .2s, 0s;", ClassName = "css-motion-code" },
                            },
                        },
                    },
                },
            },
        };
    }

    /// <summary>复用页面已有双列卡片，以固定正尺寸轨道容纳移动样本和终点标记。</summary>
    private static UIView CreateMotionCard(string title, string description, UIElement sample, string code)
        => new()
        {
            ClassName = "glass-selector-example css-motion-card",
            Children = new()
            {
                new UILabel { Text = title, ClassName = "glass-selector-example-title" },
                new UILabel { Text = description, ClassName = "glass-selector-example-desc" },
                new UIView
                {
                    ClassName = "css-motion-track",
                    Children = new()
                    {
                        new UIView { ClassName = "css-motion-endmark" },
                        sample,
                    },
                },
                new UILabel { Text = code, ClassName = "css-motion-code" },
            },
        };
}
