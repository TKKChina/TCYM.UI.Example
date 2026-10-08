using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Events;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Glass;

internal sealed partial class UIGlassDemo
{
    /// <summary>演示真实滚动时间轴、动画合成与滤镜；时间动画只在点击后有限播放。</summary>
    private static UIView CreateAdvancedMotionSection()
    {
        var eventLog = new UILabel
        {
            Id = "css-motion2-events", ClassName = "css-motion2-log",
            Text = "事件记录：滚动或点击播放后显示最近 6 条真实生命周期通知。",
        };
        var messages = new Queue<string>();

        // 只在生命周期边界修改日志，不使用逐帧文本刷新或额外计时器。
        void Record(UIAnimationEvent e)
        {
            messages.Enqueue($"{e.Type,-9}  {e.AnimationName}  {e.ElapsedTime:0.00}s");
            while (messages.Count > 6) messages.Dequeue();
            eventLog.Text = string.Join("\n", messages);
        }

        var replace = new UILabel { Id = "css-motion2-replace", Text = "replace", ClassName = "css-motion2-scale css-motion2-replace" };
        var add = new UILabel { Id = "css-motion2-add", Text = "add", ClassName = "css-motion2-scale css-motion2-add" };
        var accumulate = new UILabel { Id = "css-motion2-accumulate", Text = "累积", ClassName = "css-motion2-scale css-motion2-accumulate" };
        var filter = new UIView
        {
            Id = "css-motion2-filter", ClassName = "css-motion2-filter",
            Children = new()
            {
                new UILabel { Text = "FILTER · 整体滤镜", ClassName = "css-motion2-filter-title" },
                new UIView { ClassName = "css-motion2-color css-motion2-color-blue" },
                new UIView { ClassName = "css-motion2-color css-motion2-color-pink" },
            },
        };
        (UIElement Element, string Declaration)[] samples =
        [
            (replace, "animation:glass-advanced-scale 1.6s linear both; animation-composition:replace;"),
            (add, "animation:glass-advanced-scale 1.6s linear both; animation-composition:add;"),
            (accumulate, "animation:glass-advanced-scale 1.6s linear both; animation-composition:accumulate;"),
            (filter, "animation:glass-advanced-filter 1.4s ease-in-out 2 alternate forwards;"),
        ];

        // 先让旧实例退出，再提交完整动画声明，保证连续点击也能产生正确 cancel/start。
        void StopSamples()
        {
            foreach (var sample in samples)
            {
                sample.Element.UpdateStyle(new UpdateUIStyle { AnimationName = "none" });
                sample.Element.Update(0);
            }
        }

        void ReplaySamples()
        {
            StopSamples();
            foreach (var sample in samples)
            {
                var style = new DefaultUIStyle();
                style.ParseFromCss(sample.Declaration);
                sample.Element.UpdateStyle(style);
            }
        }

        var viewSubject = new UILabel
        {
            Id = "css-motion2-view", Text = "view(block)\n进入窗口后渐显、位移归零", ClassName = "css-motion2-view",
        };
        var source = new UIScrollView
        {
            Id = "css-motion2-source", ClassName = "css-motion2-source", EnableDeferredChildLoading = false,
            Children = new()
            {
                new UIView
                {
                    ClassName = "css-motion2-scroll-content",
                    Children = new()
                    {
                        new UILabel { Text = "↓ 在此区域滚动\n底色由 scroll(self block) 驱动", ClassName = "css-motion2-scroll-hint" },
                        viewSubject,
                        new UILabel { Text = "滚动终点 · 向上滚动可反向播放", ClassName = "css-motion2-scroll-end" },
                    },
                },
            },
        };
        var offset = new UILabel
        {
            Id = "css-motion2-offset", Text = "ScrollY: 0 CSS px · 下方进度条只在 20%～80% 范围增长。",
            ClassName = "css-motion2-offset",
        };
        source.OnScrollChanged += (y, _) => offset.Text = $"ScrollY: {DPIHelper.ToDp(y):0} CSS px · 进度条范围 20%～80%。";

        var section = new UIView
        {
            Id = "css-motion2-section", ClassName = "glass-demo-card",
            Events = new() { AnimationStart = Record, AnimationIteration = Record, AnimationEnd = Record, AnimationCancel = Record },
            Children = new()
            {
                CreateSectionTitle("滚动动画、合成与生命周期事件"),
                CreateSectionDescription("左侧由真实 UIScrollView 驱动，不随时间自动前进；右侧与滤镜样本默认静止，点击播放只运行有限次数。事件从样本冒泡到本区域。"),
                new UIView
                {
                    ClassName = "css-vars-controls",
                    Children = new()
                    {
                        new UIButton { Id = "css-motion2-play", Text = "播放 / 重播有限动画", ClassName = "css-vars-button", Events = new() { Click = _ => ReplaySamples() } },
                        new UIButton { Id = "css-motion2-stop", Text = "取消并恢复基础样式", ClassName = "css-vars-button", Events = new() { Click = _ => StopSamples() } },
                    },
                },
                new UIView
                {
                    ClassName = "glass-selector-grid",
                    Children = new()
                    {
                        new UIView
                        {
                            ClassName = "glass-selector-example css-motion2-scroll-card",
                            Children = new()
                            {
                                new UILabel { Text = "scroll() / view() / animation-range", ClassName = "glass-selector-example-title" },
                                new UILabel { Text = "滚轮、拖动滚动条或点击定位；进度条通过命名时间轴绑定其兄弟滚动容器。", ClassName = "glass-selector-example-desc" },
                                source,
                                offset,
                                new UIView { ClassName = "css-motion2-meter-track", Children = new() { new UIView { Id = "css-motion2-meter", ClassName = "css-motion2-meter" } } },
                                new UIView
                                {
                                    ClassName = "css-vars-controls",
                                    Children = new()
                                    {
                                        new UIButton { Id = "css-motion2-top", Text = "顶部", ClassName = "css-motion2-scroll-button", Events = new() { Click = _ => source.ScrollByY(-100000) } },
                                        new UIButton { Id = "css-motion2-middle", Text = "中间", ClassName = "css-motion2-scroll-button", Events = new() { Click = _ => source.ScrollByY(DPIHelper.ToPixels(210) - source.ScrollY) } },
                                        new UIButton { Id = "css-motion2-bottom", Text = "底部", ClassName = "css-motion2-scroll-button", Events = new() { Click = _ => source.ScrollByY(100000) } },
                                    },
                                },
                                new UILabel { Text = "animation-timeline: scroll(self block);\nanimation-timeline: view(block);\nanimation-range: entry 0% cover 70%;\n/* 外部进度条：--glass-progress + 20% 80% */", ClassName = "css-motion-code" },
                            },
                        },
                        new UIView
                        {
                            ClassName = "glass-selector-example css-motion2-compose-card",
                            Children = new()
                            {
                                new UILabel { Text = "animation-composition 对照", ClassName = "glass-selector-example-title" },
                                new UILabel { Text = "基础 scale(1.25)，动画终点 scale(1.5)。replace 替换，add 拼接，accumulate 累积缩放增量。", ClassName = "glass-selector-example-desc" },
                                new UIView
                                {
                                    ClassName = "css-motion2-scale-row",
                                    Children = new()
                                    {
                                        CreateCompositionSample(replace, "1.50×", "替换"),
                                        CreateCompositionSample(add, "1.875×", "拼接相乘"),
                                        CreateCompositionSample(accumulate, "1.75×", "增量相加"),
                                    },
                                },
                                new UILabel { Text = "transform: scale(1.25);\nanimation: glass-advanced-scale 1.6s linear both;\nanimation-composition: replace; /* 或 add / accumulate */", ClassName = "css-motion-code" },
                                new UILabel { Text = "以上倍数为播放终点；未点击时三个样本都保持基础样式。", ClassName = "css-motion2-offset" },
                            },
                        },
                        new UIView
                        {
                            ClassName = "glass-selector-example css-motion2-filter-card",
                            Children = new()
                            {
                                new UILabel { Text = "filter-list：作用于整个子树", ClassName = "glass-selector-example-title" },
                                new UILabel { Text = "文字、底色和彩色块一起应用 hue-rotate、saturate、blur 和 drop-shadow；两轮往返后停止。", ClassName = "glass-selector-example-desc" },
                                new UIView { ClassName = "css-motion2-filter-stage", Children = new() { filter } },
                                new UILabel { Text = "from { filter: none; }\nto { filter: hue-rotate(160deg) saturate(1.5)\n             blur(2px) drop-shadow(6px 8px 4px #0f172a66); }", ClassName = "css-motion-code" },
                            },
                        },
                        new UIView
                        {
                            ClassName = "glass-selector-example css-motion2-event-card",
                            Children = new()
                            {
                                new UILabel { Text = "start / iteration / end / cancel", ClassName = "glass-selector-example-title" },
                                new UILabel { Text = "这里只在真实边界记录事件，不按帧轮询。播放中点击取消可看到 Cancel；反向滚动会再次进入活动区间。", ClassName = "glass-selector-example-desc" },
                                eventLog,
                                new UILabel { Text = "Events = new() {\n    AnimationStart = e => Log(e.AnimationName, e.ElapsedTime),\n    AnimationCancel = e => Log(e.Type)\n};", ClassName = "css-motion-code" },
                            },
                        },
                    },
                },
            },
        };
        return section;
    }

    /// <summary>为三种合成方式提供同尺寸样本与终点说明，避免动画改变布局影响对比。</summary>
    private static UIView CreateCompositionSample(UILabel actor, string factor, string description) => new()
    {
        ClassName = "css-motion2-scale-cell",
        Children = new()
        {
            new UIView { ClassName = "css-motion2-scale-stage", Children = new() { actor } },
            new UILabel { Text = factor + "\n" + description, ClassName = "css-motion2-scale-caption" },
        },
    };
}
