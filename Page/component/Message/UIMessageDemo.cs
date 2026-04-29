using SkiaSharp;
using System.Threading.Tasks;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Message;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Message
{
    internal class UIMessageDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Message.style.css";
        private const string LoadingMessageKey = "message-demo-loading";

        internal UIMessageDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            // Demo 页面进入时重置一次全局配置，避免其他示例改过的配置残留到这里。
            ResetGlobalConfig();

            ClassName = new List<string> { "message-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Message 全局提示",
                    ClassName = new List<string> { "message-demo-title" },
                },
                new UILabel
                {
                    Text = "轻量的全局反馈组件，用于展示操作结果、状态变更和后台进度。",
                    ClassName = new List<string> { "message-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "适合非阻塞反馈场景。当前示例覆盖基础类型、带 key 的状态更新、全局配置以及自定义样式。",
                    ClassName = new List<string> { "message-demo-desc" },
                },
                CreateBasicSection(),
                CreateKeyUpdateSection(),
                CreateGlobalConfigSection(),
                CreateCustomStyleSection(),
            };
        }

        // 基础类型示例：展示 Message 最常用的几种快捷调用方式。
        private static UIView CreateBasicSection()
        {
            return CreateSectionCard(
                "基础类型",
                "最常见的四种反馈类型加一个 loading 状态。默认 3 秒自动关闭，loading 可单独指定时长。",
                CreateActionRow(
                    CreateActionButton("信息", () => UIMessage.Info("这是一条普通信息提示。"), "message-action-primary"),
                    CreateActionButton("成功", () => UIMessage.Success("保存成功，数据已经同步。"), "message-action-success"),
                    CreateActionButton("警告", () => UIMessage.Warning("还有 2 项必填信息未完成。"), "message-action-warning"),
                    CreateActionButton("错误", () => UIMessage.Error("请求失败，请稍后重试。"), "message-action-danger"),
                    CreateActionButton("Loading", () => UIMessage.Loading("正在同步消息中心...", duration: 2f), "message-action-neutral")
                ),
                CreateHintLabel("Message 是全局浮层，不会打断当前视图交互。")
            );
        }

        // key 更新示例：同一条消息可以从 loading 原地切换到 success / error。
        private static UIView CreateKeyUpdateSection()
        {
            return CreateSectionCard(
                "使用 key 更新状态",
                "当同一任务需要经历“加载中 -> 成功 / 失败”时，使用 key 可以原地更新同一条消息，避免多条堆叠。",
                CreateActionRow(
                    CreateActionButton("开始保存并更新", StartSuccessFlow, "message-action-primary"),
                    CreateActionButton("开始发布并失败", StartFailureFlow, "message-action-warning"),
                    CreateActionButton("关闭当前加载", () => UIMessage.Close(LoadingMessageKey), "message-action-neutral"),
                    CreateActionButton("连续触发 4 条", TriggerBurstMessages, "message-action-dark")
                ),
                CreateHintLabel("这里的“开始保存并更新”和“开始发布并失败”都复用同一个 key。")
            );
        }

        // 全局配置示例：统一影响后续新创建的 Message 展示位置、时长和默认外观。
        private static UIView CreateGlobalConfigSection()
        {
            return CreateSectionCard(
                "全局配置",
                "可以统一控制顶部偏移、默认时长、最大显示条数，以及所有消息的基础外观。",
                CreateActionRow(
                    CreateActionButton("深色主题", ApplyDarkConfig, "message-action-dark"),
                    CreateActionButton("紧凑模式", ApplyCompactConfig, "message-action-neutral"),
                    CreateActionButton("恢复默认", () =>
                    {
                        ResetGlobalConfig();
                        UIMessage.Success("已恢复 Message 默认配置。", duration: 2f);
                    }, "message-action-success")
                ),
                CreateHintLabel("全局配置只影响后续新创建的消息，不会回写已显示的旧消息。")
            );
        }

        // 单条样式示例：通过 Open 传入 MessageConfig，为某一条消息单独定制外观。
        private static UIView CreateCustomStyleSection()
        {
            return CreateSectionCard(
                "自定义样式与清空",
                "通过 Open 传入 MessageConfig，可以为单条消息定义独立的容器样式与文本样式。",
                CreateActionRow(
                    CreateActionButton("成功卡片样式", OpenSuccessCardMessage, "message-action-success"),
                    CreateActionButton("重要提醒样式", OpenWarningCardMessage, "message-action-warning"),
                    CreateActionButton("清空全部消息", UIMessage.DestroyAll, "message-action-danger")
                ),
                CreateHintLabel("自定义样式适合强调高优先级反馈，但仍建议保持 Message 的短平快特性。")
            );
        }

        // 所有示例块共用的卡片外壳，统一标题、描述和内容布局。
        private static UIView CreateSectionCard(string title, string description, params UIElement[] children)
        {
            var sectionChildren = new List<UIElement>
            {
                new UILabel
                {
                    Text = title,
                    ClassName = new List<string> { "message-card-title" },
                },
                new UILabel
                {
                    Text = description,
                    ClassName = new List<string> { "message-card-desc" },
                }
            };

            sectionChildren.AddRange(children);

            return new UIView
            {
                ClassName = new List<string> { "message-demo-card" },
                Children = sectionChildren,
            };
        }

        // 行容器用于承载一组演示按钮，保持每个 section 的交互区结构一致。
        private static UIView CreateActionRow(params UIElement[] buttons)
        {
            return new UIView
            {
                ClassName = new List<string> { "message-showcase" },
                Children = buttons.ToList(),
            };
        }

        // 补充说明文本，放在每个卡片底部解释当前示例的使用场景。
        private static UILabel CreateHintLabel(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "message-demo-hint" },
            };
        }

        // 按钮工厂：统一示例按钮的基础样式和点击绑定。
        private static UIButton CreateActionButton(string text, Action onClick, params string[] extraClasses)
        {
            var classNames = new List<string> { "message-action-btn" };
            classNames.AddRange(extraClasses);

            return new UIButton
            {
                Text = text,
                ClassName = classNames,
                Events = new()
                {
                    Click = _ => onClick()
                }
            };
        }

        // 先显示 loading，再通过相同 key 更新成成功态，模拟“保存中 -> 保存成功”。
        private static void StartSuccessFlow()
        {
            UIMessage.Loading("正在保存草稿，请稍候...", key: LoadingMessageKey);
            Task.Run(async () =>
            {
                await Task.Delay(1200);
                UIMessage.Success("草稿保存成功，已完成自动更新。", duration: 2.4f, key: LoadingMessageKey);
            });
        }

        // 与成功流相同，但最终更新为错误态，用来演示失败分支。
        private static void StartFailureFlow()
        {
            UIMessage.Loading("正在发布内容，请稍候...", key: LoadingMessageKey);
            Task.Run(async () =>
            {
                await Task.Delay(1400);
                UIMessage.Error("发布失败，服务器返回了校验错误。", duration: 2.8f, key: LoadingMessageKey);
            });
        }

        // 批量触发多条消息，用来观察 maxCount 等全局配置的限制效果。
        private static void TriggerBurstMessages()
        {
            for (int index = 1; index <= 4; index++)
            {
                UIMessage.Info($"第 {index} 条批量消息", duration: 2f + index * 0.3f);
            }
        }

        // 深色主题配置，适合演示全局换肤后的 Message 外观。
        private static void ApplyDarkConfig()
        {
            UIMessage.Config(new MessageGlobalConfig
            {
                Top = 68,
                Duration = 2.8f,
                MaxCount = 3,
                Style = new DefaultUIStyle
                {
                    BackgroundColor = ColorHelper.ParseColor("#0f172a"),
                    BorderRadius = 12,
                    PaddingLeft = 16,
                    PaddingRight = 16,
                    PaddingTop = 10,
                    PaddingBottom = 10,
                    BoxShadowOffsetX = 0,
                    BoxShadowOffsetY = 10,
                    BoxShadowBlur = 24,
                    BoxShadowColor = new SKColor(15, 23, 42, 80),
                },
                ContentStyle = new DefaultUIStyle
                {
                    Color = ColorHelper.ParseColor("#e2e8f0"),
                    FontSize = 14,
                }
            });

            UIMessage.Info("已应用深色全局配置，最多同时显示 3 条。", duration: 2.2f);
        }

        // 紧凑模式配置，重点展示顶部偏移、默认时长和最大显示条数的变化。
        private static void ApplyCompactConfig()
        {
            UIMessage.Config(new MessageGlobalConfig
            {
                Top = 24,
                Duration = 1.8f,
                MaxCount = 2,
                Style = new DefaultUIStyle
                {
                    BackgroundColor = ColorHelper.ParseColor("#ffffff"),
                    BorderRadius = 999,
                    PaddingLeft = 14,
                    PaddingRight = 14,
                    PaddingTop = 8,
                    PaddingBottom = 8,
                    BorderWidth = 1,
                    BorderColor = ColorHelper.ParseColor("#d9d9d9"),
                    BoxShadowOffsetX = 0,
                    BoxShadowOffsetY = 4,
                    BoxShadowBlur = 14,
                    BoxShadowColor = new SKColor(0, 0, 0, 24),
                },
                ContentStyle = new DefaultUIStyle
                {
                    Color = ColorHelper.ParseColor("#262626"),
                    FontSize = 13,
                }
            });

            UIMessage.Success("已切换为紧凑模式，消息更贴近顶部。", duration: 2f);
        }

        // 单条成功消息的自定义卡片风格，不影响全局默认样式。
        private static void OpenSuccessCardMessage()
        {
            UIMessage.Open(new MessageConfig
            {
                Content = "自动化部署完成，测试环境已经发布到最新版本。",
                Type = MessageType.Success,
                Duration = 3.2f,
                Style = new DefaultUIStyle
                {
                    BackgroundColor = ColorHelper.ParseColor("#f6ffed"),
                    BorderRadius = 14,
                    BorderWidth = 1,
                    BorderColor = ColorHelper.ParseColor("#b7eb8f"),
                    PaddingLeft = 18,
                    PaddingRight = 18,
                    PaddingTop = 11,
                    PaddingBottom = 11,
                    BoxShadowOffsetX = 0,
                    BoxShadowOffsetY = 10,
                    BoxShadowBlur = 22,
                    BoxShadowColor = new SKColor(82, 196, 26, 42),
                },
                ContentStyle = new DefaultUIStyle
                {
                    Color = ColorHelper.ParseColor("#135200"),
                    FontSize = 14,
                }
            });
        }

        // 单条警告消息的自定义卡片风格，用于强调高优先级提醒。
        private static void OpenWarningCardMessage()
        {
            UIMessage.Open(new MessageConfig
            {
                Content = "检测到风控规则变更，请在 10 分钟内完成人工复核。",
                Type = MessageType.Warning,
                Duration = 4f,
                Style = new DefaultUIStyle
                {
                    BackgroundColor = ColorHelper.ParseColor("#fff7e6"),
                    BorderRadius = 14,
                    BorderWidth = 1,
                    BorderColor = ColorHelper.ParseColor("#ffd591"),
                    PaddingLeft = 18,
                    PaddingRight = 18,
                    PaddingTop = 11,
                    PaddingBottom = 11,
                    BoxShadowOffsetX = 0,
                    BoxShadowOffsetY = 10,
                    BoxShadowBlur = 22,
                    BoxShadowColor = new SKColor(250, 173, 20, 42),
                },
                ContentStyle = new DefaultUIStyle
                {
                    Color = ColorHelper.ParseColor("#874d00"),
                    FontSize = 14,
                }
            });
        }

        // 恢复 Message 的默认全局配置，便于不同示例之间相互隔离。
        private static void ResetGlobalConfig()
        {
            UIMessage.Config(new MessageGlobalConfig());
        }
    }
}