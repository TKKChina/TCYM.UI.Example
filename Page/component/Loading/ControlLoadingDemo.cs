using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Loading;
using TCYM.UI.Enums;

namespace TCYM.UI.Example.Page.component.Loading;

/// <summary>Loading 组件示例：遮罩、行内、霓虹追逐和四点花瓣动画。</summary>
internal sealed class ControlLoadingDemo : UIScrollView
{
    private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Loading.style.css";

    internal ControlLoadingDemo()
    {
        UISystem.LoadStyleFile(DemoCssPath);
        ClassName = "loading-demo-view";

        var standard = new ControlLoading();
        var custom = new ControlLoading
        {
            SizeLoader = 56, DurationSec = 1.6f,
            PrimaryColor = new SKColor(0x5E, 0xEA, 0xD4),
            SecondaryColor = new SKColor(0xC4, 0xB5, 0xFD),
        };
        var inline = new ControlLoading
        {
            SizeLoader = 40,
            Style = new UpdateUIStyle { Position = "relative", Width = 80, Height = 80, BackdropBlur = 0 },
        };
        var neonDefault = new NeonLoading
        {
            SizeLoader = 72,
            GlowSigma = 0,
            Style = new DefaultUIStyle { Width = 72, Height = 72 },
        };
        var neonPurple = new NeonLoading
        {
            SizeLoader = 72,
            NeonColor = new SKColor(124, 58, 237),
            RotateDurationSec = 0.9f,
            PauseDurationSec = 0.12f,
            GlowSigma = 0,
            TiltDegrees = 20,
            Style = new DefaultUIStyle { Width = 72, Height = 72 },
        };
        var tomatoDefault = new TomatoNovelLoading
        {
            SizeLoader = 72,
            GlowSigma = 0,
            Style = new DefaultUIStyle { Width = 72, Height = 72 },
        };
        var tomatoOcean = new TomatoNovelLoading
        {
            SizeLoader = 72,
            RotateDurationSec = 1.4f,
            MorphDurationSec = 0.72f,
            GlowSigma = 0,
            StartAngleDeg = 15,
            DotColors =
            [
                new SKColor(14, 165, 233),
                new SKColor(6, 182, 212),
                new SKColor(20, 184, 166),
                new SKColor(34, 197, 94),
            ],
            Style = new DefaultUIStyle { Width = 72, Height = 72 },
        };

        ControlLoading[] loaders = [standard, custom, inline];
        bool visible = true;
        var pauseButton = MakeButton("暂停动画", primary: true);
        pauseButton.OnClick += _ =>
        {
            bool running = !standard.IsAnimating;
            foreach (var loader in loaders) loader.IsAnimating = running;
            pauseButton.Text = running ? "暂停动画" : "继续动画";
        };
        var visibleButton = MakeButton("隐藏加载");
        visibleButton.OnClick += _ =>
        {
            visible = !visible;
            foreach (var loader in loaders)
                loader.UpdateStyle(new UpdateUIStyle { Display = visible ? "block" : "none" });
            visibleButton.Text = visible ? "隐藏加载" : "显示加载";
        };
        var restartButton = MakeButton("重新播放");
        restartButton.OnClick += _ =>
        {
            foreach (var loader in loaders)
            {
                loader.Restart();
                loader.IsAnimating = true;
            }
            pauseButton.Text = "暂停动画";
        };

        Children =
        [
            new UILabel { Text = "Loading 加载动画", ClassName = "loading-demo-title" },
            new UILabel { Text = "在等待数据、页面切换和后台处理中提供明确的加载反馈。", ClassName = "loading-demo-title-sub" },
            new UILabel
            {
                Text = "包含 ControlLoading 遮罩、NeonLoading 霓虹追逐和 TomatoNovelLoading 四点花瓣三种组件；支持尺寸、颜色与运动节奏定制。",
                ClassName = "loading-demo-desc",
            },
            CreateSectionCard(
                "ControlLoading 遮罩与行内模式",
                "默认覆盖相对定位的父容器并模糊背景，也可以切换为普通布局组件。",
                new UIView
                {
                    ClassName = "loading-action-row",
                    Children = [pauseButton, visibleButton, restartButton],
                },
                new UIView
                {
                    ClassName = "loading-overlay-grid",
                    Children =
                    [
                        CreateOverlaySample("默认遮罩", "35dp · 1 秒 · 蓝橙双点", standard, dark: false),
                        CreateOverlaySample("定制主题", "56dp · 1.6 秒 · 青紫配色", custom, dark: true),
                    ],
                },
                CreateInlineSample(inline)),
            CreateSectionCard(
                "NeonLoading 霓虹追逐",
                "多个光点沿倾斜轨道追逐，并在每轮结束时短暂停顿；适合媒体、图表和设备连接场景。",
                CreateMotionGrid(
                    CreateMotionSample("天蓝轻量模式", "无模糊 · 45° 倾角", neonDefault),
                    CreateMotionSample("紫色快速模式", "无模糊 · 0.9 秒 · 20° 倾角", neonPurple))),
            CreateSectionCard(
                "TomatoNovelLoading 四点花瓣",
                "四个彩色点在圆点与花瓣之间连续形变，可调整轨道、光晕、旋转和形变节奏。",
                CreateMotionGrid(
                    CreateMotionSample("经典轻量模式", "暖色花瓣 · 默认节奏", tomatoDefault),
                    CreateMotionSample("海洋配色", "蓝绿渐进 · 舒缓旋转", tomatoOcean))),
        ];
    }

    private static UIView CreateSectionCard(string title, string description, params UIElement[] children)
    {
        var content = new List<UIElement>
        {
            new UILabel { Text = title, ClassName = "loading-card-title" },
            new UILabel { Text = description, ClassName = "loading-card-desc" },
        };
        content.AddRange(children);

        return new UIView
        {
            ClassName = "loading-demo-card",
            Children = content,
        };
    }

    private static UIView CreateOverlaySample(string title, string detail, ControlLoading loading, bool dark)
    {
        var stageClasses = new List<string> { "loading-overlay-stage" };
        if (dark) stageClasses.Add("loading-overlay-stage-dark");

        return new UIView
        {
            ClassName = "loading-overlay-sample",
            Children =
            [
                new UILabel { Text = title, ClassName = "loading-sample-title" },
                new UILabel { Text = detail, ClassName = "loading-sample-meta" },
                new UIView
                {
                    ClassName = stageClasses,
                    Children =
                    [
                        new UILabel { Text = "正在同步工作区", ClassName = "loading-stage-title" },
                        new UILabel { Text = "背景内容保持在原位，加载层负责状态反馈。", ClassName = "loading-stage-copy" },
                        new UIView { ClassName = "loading-stage-line" },
                        new UIView { ClassName = "loading-stage-line loading-stage-line-short" },
                        loading,
                    ],
                },
            ],
        };
    }

    private static UIView CreateInlineSample(ControlLoading loading)
    {
        return new UIView
        {
            ClassName = "loading-inline-demo",
            Children =
            [
                loading,
                new UIView
                {
                    ClassName = "loading-inline-copy",
                    Children =
                    [
                        new UILabel { Text = "普通布局", ClassName = "loading-sample-title" },
                        new UILabel
                        {
                            Text = "Position=relative，固定 80 × 80，并关闭 BackdropBlur。",
                            ClassName = "loading-sample-meta",
                        },
                    ],
                },
            ],
        };
    }

    private static UIView CreateMotionGrid(params UIElement[] children)
    {
        return new UIView
        {
            ClassName = "loading-motion-grid",
            Children = children.ToList(),
        };
    }

    private static UIView CreateMotionSample(string title, string detail, UIElement loading)
    {
        return new UIView
        {
            ClassName = "loading-motion-item",
            Children =
            [
                new UIView
                {
                    ClassName = "loading-motion-canvas",
                    Children = [loading],
                },
                new UILabel
                {
                    Text = title,
                    ClassName = "loading-sample-title loading-motion-title",
                },
                new UILabel { Text = detail, ClassName = "loading-sample-meta loading-motion-meta" },
            ],
        };
    }

    private static UIButton MakeButton(string text, bool primary = false)
    {
        var classes = new List<string> { "loading-action-button" };
        if (primary) classes.Add("loading-action-button-primary");
        return new UIButton { Text = text, ClassName = classes };
    }
}
