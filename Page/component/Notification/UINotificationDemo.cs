using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Notification;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Notification
{
    /// <summary>
    /// 展示 UINotification 的窗口内通知与跨平台系统通知入口。
    /// </summary>
    internal sealed class UINotificationDemo : UIScrollView, IUIRouteLifecycle
    {
        /// <summary>示例页嵌入式 CSS 资源路径。</summary>
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Notification.style.css";

        /// <summary>Key 更新示例使用的稳定通知标识。</summary>
        private const string UpdateKey = "notification-demo-update";

        /// <summary>展示当前系统通知能力和最近一次投递结果。</summary>
        private readonly UILabel _systemStatus;

        /// <summary>
        /// 创建完整的 Notification 示例页面。
        /// </summary>
        internal UINotificationDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ResetNotificationConfig();

            _systemStatus = CreateHintLabel(BuildCapabilityText());
            ClassName = ["notification-demo-view"];
            Children =
            [
                new UILabel
                {
                    Text = "Notification 通知提醒框",
                    ClassName = ["notification-demo-title"],
                },
                new UILabel
                {
                    Text = "用于较复杂、可交互或由系统主动推送的通知。",
                    ClassName = ["notification-demo-title-sub"],
                },
                new UILabel
                {
                    Text = "窗口内通知支持六个位置、Key 更新、悬停暂停、进度条和操作按钮；系统通知由 Windows、Linux 或 macOS 的通知服务决定位置与外观。",
                    ClassName = ["notification-demo-desc"],
                },
                CreateBasicSection(),
                CreatePlacementSection(),
                CreateLifecycleSection(),
                CreateActionSection(),
                CreateSystemSection(),
            ];
        }

        /// <summary>
        /// 页面进入时刷新系统通知能力文字。
        /// </summary>
        /// <param name="fromPath">来源路由。</param>
        public void OnRouteEnter(string? fromPath)
        {
            _systemStatus.Text = BuildCapabilityText();
        }

        /// <summary>
        /// 页面离开时关闭本页创建的通知并恢复默认配置。
        /// </summary>
        /// <param name="toPath">目标路由。</param>
        public void OnRouteLeave(string? toPath)
        {
            UINotification.DestroyAll();
            ResetNotificationConfig();
        }

        /// <summary>
        /// 创建基础通知类型示例。
        /// </summary>
        /// <returns>基础类型卡片。</returns>
        private static UIView CreateBasicSection()
        {
            return CreateSectionCard(
                "基础类型",
                "快捷方法适合标题和正文均为文本的常见场景；复杂交互使用 NotificationConfig。",
                CreateActionRow(
                    CreateActionButton(
                        "普通",
                        () => UINotification.Open(new NotificationConfig
                        {
                            Title = "收到一条新通知",
                            Description = "这是一条不强调状态的普通通知。",
                            Type = NotificationType.Default,
                        }),
                        "notification-action-neutral"),
                    CreateActionButton(
                        "信息",
                        () => UINotification.Info("版本提示", "新版本已下载，可在空闲时重新启动应用。"),
                        "notification-action-primary"),
                    CreateActionButton(
                        "成功",
                        () => UINotification.Success("保存成功", "表单内容已经同步到服务器。"),
                        "notification-action-success"),
                    CreateActionButton(
                        "警告",
                        () => UINotification.Warning("空间不足", "剩余空间低于 10%，建议及时清理。"),
                        "notification-action-warning"),
                    CreateActionButton(
                        "错误",
                        () => UINotification.Error("发布失败", "服务器没有通过本次内容校验。"),
                        "notification-action-danger")),
                CreateHintLabel("默认在右上角显示，4.5 秒后自动关闭。"));
        }

        /// <summary>
        /// 创建六种窗口内位置示例。
        /// </summary>
        /// <returns>位置卡片。</returns>
        private static UIView CreatePlacementSection()
        {
            return CreateSectionCard(
                "窗口内位置",
                "Placement 只控制 TCYM.UI 窗口内通知；系统通知位置由平台和用户设置控制。",
                CreateActionRow(
                    CreatePlacementButton("顶部", NotificationPlacement.Top),
                    CreatePlacementButton("左上", NotificationPlacement.TopLeft),
                    CreatePlacementButton("右上", NotificationPlacement.TopRight),
                    CreatePlacementButton("底部", NotificationPlacement.Bottom),
                    CreatePlacementButton("左下", NotificationPlacement.BottomLeft),
                    CreatePlacementButton("右下", NotificationPlacement.BottomRight)),
                CreateHintLabel("Top 与 Bottom 居中；四个角落使用全局边距和固定卡片宽度。"));
        }

        /// <summary>
        /// 创建自动关闭、进度与 Key 更新示例。
        /// </summary>
        /// <returns>生命周期卡片。</returns>
        private static UIView CreateLifecycleSection()
        {
            return CreateSectionCard(
                "生命周期、进度与 Key 更新",
                "Duration=0 会保持显示；相同 Key 会更新原卡片并重新开始倒计时。",
                CreateActionRow(
                    CreateActionButton("显示进度", () =>
                    {
                        UINotification.Open(new NotificationConfig
                        {
                            Title = "正在同步项目",
                            Description = "鼠标悬停在通知上会暂停倒计时与进度。",
                            Type = NotificationType.Info,
                            Duration = 6,
                            ShowProgress = true,
                            PauseOnHover = true,
                            Key = UpdateKey,
                        });
                    }, "notification-action-primary"),
                    CreateActionButton("更新为成功", () =>
                    {
                        UINotification.Open(new NotificationConfig
                        {
                            Title = "项目同步完成",
                            Description = "相同 Key 已将原通知更新为成功状态。",
                            Type = NotificationType.Success,
                            Duration = 3.5f,
                            ShowProgress = true,
                            PauseOnHover = true,
                            Key = UpdateKey,
                        });
                    }, "notification-action-success"),
                    CreateActionButton("永久显示", () =>
                    {
                        UINotification.Open(new NotificationConfig
                        {
                            Title = "需要人工处理",
                            Description = "Duration 为 0，只能由用户或代码主动关闭。",
                            Type = NotificationType.Warning,
                            Duration = 0,
                            Key = "notification-demo-persistent",
                        });
                    }, "notification-action-warning"),
                    CreateActionButton("关闭 Key", () => UINotification.Close(UpdateKey), "notification-action-neutral"),
                    CreateActionButton("清空全部", UINotification.DestroyAll, "notification-action-danger")),
                CreateHintLabel("更新可以同时改变标题、正文、类型、位置和关闭回调。"));
        }

        /// <summary>
        /// 创建自定义操作按钮与样式示例。
        /// </summary>
        /// <returns>交互通知卡片。</returns>
        private static UIView CreateActionSection()
        {
            return CreateSectionCard(
                "交互与自定义样式",
                "Actions 支持同步与异步回调；每个公开同步回调都有对应的 Async 路径。",
                CreateActionRow(
                    CreateActionButton("打开交互通知", () =>
                    {
                        UINotification.Open(new NotificationConfig
                        {
                            Title = "检测到可用更新",
                            Description = "可以立即安装，也可以保留通知稍后处理。",
                            Type = NotificationType.Info,
                            Duration = 0,
                            Key = "notification-demo-actions",
                            Actions =
                            [
                                new NotificationAction
                                {
                                    Text = "稍后",
                                    CloseOnClick = true,
                                    OnClick = () => UINotification.Info("已稍后处理", "本次更新提醒已经关闭。"),
                                },
                                new NotificationAction
                                {
                                    Text = "立即安装",
                                    CloseOnClick = true,
                                    OnClickAsync = async () =>
                                    {
                                        await Task.Delay(300);
                                        UINotification.Success("安装任务已创建", "后台任务将继续完成下载与安装。");
                                    },
                                    Style = new DefaultUIStyle
                                    {
                                        BackgroundColor = ColorHelper.ParseColor("#1677ff"),
                                        BorderColor = ColorHelper.ParseColor("#1677ff"),
                                        Color = SKColors.White,
                                    },
                                },
                            ],
                            Style = new DefaultUIStyle
                            {
                                BorderWidth = 1,
                                BorderColor = ColorHelper.ParseColor("#91caff"),
                                BackgroundColor = ColorHelper.ParseColor("#f0f7ff"),
                            },
                        });
                    }, "notification-action-primary"),
                    CreateActionButton("应用紧凑全局配置", ApplyCompactConfig, "notification-action-dark"),
                    CreateActionButton("恢复默认", () =>
                    {
                        ResetNotificationConfig();
                        UINotification.Success("已恢复默认", "后续通知将使用默认宽度、位置和时长。");
                    }, "notification-action-success")),
                CreateHintLabel("单条 Style 会覆盖全局样式，但不会修改其他通知。"));
        }

        /// <summary>
        /// 创建系统通知能力与降级策略示例。
        /// </summary>
        /// <returns>系统通知卡片。</returns>
        private UIView CreateSystemSection()
        {
            return CreateSectionCard(
                "系统层通知（Windows / Linux / macOS）",
                "系统通知提供程序可替换；默认实现只依赖平台能力，调用会返回明确状态。",
                CreateActionRow(
                    CreateAsyncActionButton(
                        "仅系统通知",
                        () => SendSystemNotificationAsync(fallbackToInApp: false),
                        "notification-action-dark"),
                    CreateAsyncActionButton(
                        "优先系统，失败时界面内显示",
                        () => SendSystemNotificationAsync(fallbackToInApp: true),
                        "notification-action-primary")),
                _systemStatus,
                CreateHintLabel("系统通知无法可靠调整显示位置；需要精确位置时请使用窗口内 Notification。"));
        }

        /// <summary>
        /// 创建指定窗口内位置的演示按钮。
        /// </summary>
        /// <param name="text">按钮文字。</param>
        /// <param name="placement">目标位置。</param>
        /// <returns>演示按钮。</returns>
        private static UIButton CreatePlacementButton(string text, NotificationPlacement placement)
        {
            return CreateActionButton(text, () =>
            {
                UINotification.Open(new NotificationConfig
                {
                    Title = $"{text}通知",
                    Description = $"这条通知的 Placement 是 {placement}。",
                    Type = NotificationType.Info,
                    Placement = placement,
                    Duration = 3.5f,
                });
            }, "notification-action-neutral");
        }

        /// <summary>
        /// 尝试系统通知，并把 Submitted、失败或窗口内降级结果显示到页面上。
        /// </summary>
        /// <param name="fallbackToInApp">失败时是否降级为窗口内通知。</param>
        /// <returns>完成投递尝试的任务。</returns>
        private async Task SendSystemNotificationAsync(bool fallbackToInApp)
        {
            _systemStatus.Text = "正在请求系统通知…";
            NotificationDeliveryResult result = await UINotification.ShowSystemAsync(
                new NotificationConfig
                {
                    Title = "TCYM.UI 系统通知",
                    Description = "这条消息由当前操作系统的通知能力负责显示。",
                    Type = NotificationType.Info,
                    Duration = 5,
                },
                fallbackToInApp);

            string systemDetail = result.SystemResult == null
                ? "未尝试系统通知"
                : $"{result.SystemResult.Status}: {result.SystemResult.Message}";
            _systemStatus.Text =
                $"提供程序：{result.SystemResult?.ProviderName ?? UINotification.SystemCapabilities.ProviderName}；"
                + $"实际投递：{result.DeliveredTo}；降级：{result.UsedFallback}；{systemDetail}";
        }

        /// <summary>
        /// 应用一套容易观察差异的紧凑全局配置。
        /// </summary>
        private static void ApplyCompactConfig()
        {
            UINotification.Config(new NotificationGlobalConfig
            {
                Placement = NotificationPlacement.BottomRight,
                Duration = 3,
                Top = 16,
                Bottom = 16,
                HorizontalOffset = 16,
                Width = 340,
                Gap = 10,
                MaxCount = 3,
                ShowProgress = true,
                PauseOnHover = true,
                ApplicationName = "TCYM.UI Example",
                Style = new DefaultUIStyle
                {
                    BorderRadius = 14,
                    BoxShadowOffsetY = 10,
                    BoxShadowBlur = 28,
                    BoxShadowColor = new SKColor(15, 23, 42, 50),
                },
            });
            UINotification.Info("紧凑配置已应用", "默认位置已改为右下角，最多显示 3 条。");
        }

        /// <summary>
        /// 恢复示例使用的默认通知配置。
        /// </summary>
        private static void ResetNotificationConfig()
        {
            UINotification.ResetConfig();
        }

        /// <summary>
        /// 返回当前系统通知提供程序的能力说明。
        /// </summary>
        /// <returns>可直接显示的能力文字。</returns>
        private static string BuildCapabilityText()
        {
            SystemNotificationCapabilities capabilities = UINotification.SystemCapabilities;
            return $"提供程序：{capabilities.ProviderName}；可用：{capabilities.IsSupported}；"
                + $"可控制位置：{capabilities.CanControlPlacement}。{capabilities.Description}";
        }

        /// <summary>
        /// 创建统一的示例区块卡片。
        /// </summary>
        /// <param name="title">区块标题。</param>
        /// <param name="description">区块说明。</param>
        /// <param name="children">区块交互内容。</param>
        /// <returns>区块卡片。</returns>
        private static UIView CreateSectionCard(string title, string description, params UIElement[] children)
        {
            var content = new List<UIElement>
            {
                new UILabel { Text = title, ClassName = ["notification-card-title"] },
                new UILabel { Text = description, ClassName = ["notification-card-desc"] },
            };
            content.AddRange(children);
            return new UIView
            {
                ClassName = ["notification-demo-card"],
                Children = content,
            };
        }

        /// <summary>
        /// 创建横向换行的按钮容器。
        /// </summary>
        /// <param name="buttons">示例按钮。</param>
        /// <returns>按钮容器。</returns>
        private static UIView CreateActionRow(params UIElement[] buttons)
        {
            return new UIView
            {
                ClassName = ["notification-showcase"],
                Children = buttons.ToList(),
            };
        }

        /// <summary>
        /// 创建区块底部提示文字。
        /// </summary>
        /// <param name="text">提示文字。</param>
        /// <returns>提示标签。</returns>
        private static UILabel CreateHintLabel(string text)
        {
            return new UILabel
            {
                Text = text,
                Wrap = true,
                ClassName = ["notification-demo-hint"],
            };
        }

        /// <summary>
        /// 创建同步操作按钮。
        /// </summary>
        /// <param name="text">按钮文字。</param>
        /// <param name="onClick">点击回调。</param>
        /// <param name="extraClasses">附加 CSS 类。</param>
        /// <returns>示例按钮。</returns>
        private static UIButton CreateActionButton(string text, Action onClick, params string[] extraClasses)
        {
            var classes = new List<string> { "notification-action-btn" };
            classes.AddRange(extraClasses);
            return new UIButton
            {
                Text = text,
                ClassName = classes,
                Events = new() { Click = _ => onClick() },
            };
        }

        /// <summary>
        /// 创建异步操作按钮，避免 async void 点击回调。
        /// </summary>
        /// <param name="text">按钮文字。</param>
        /// <param name="onClickAsync">异步点击回调。</param>
        /// <param name="extraClasses">附加 CSS 类。</param>
        /// <returns>示例按钮。</returns>
        private static UIButton CreateAsyncActionButton(
            string text,
            Func<Task> onClickAsync,
            params string[] extraClasses)
        {
            var classes = new List<string> { "notification-action-btn" };
            classes.AddRange(extraClasses);
            var button = new UIButton
            {
                Text = text,
                ClassName = classes,
            };
            button.OnClickAsync += _ => onClickAsync();
            return button;
        }
    }
}
