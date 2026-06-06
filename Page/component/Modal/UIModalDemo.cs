using SkiaSharp;
using System.Threading.Tasks;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Input;
using TCYM.UI.Elements.Modal;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Modal
{
    internal class UIModalDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Modal.style.css";
        private const string AsyncModalKey = "modal-demo-async";

        internal UIModalDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            // 进入页面时重置全局配置，避免被其他示例改过的设置残留。
            ResetGlobalConfig();

            ClassName = new List<string> { "modal-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Modal 对话框",
                    ClassName = new List<string> { "modal-demo-title" },
                },
                new UILabel
                {
                    Text = "模态对话框，承载关键信息确认、表单提交与高强度反馈。",
                    ClassName = new List<string> { "modal-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "当前示例覆盖基础调用、语义化变体、确认对话框、异步 loading、自定义页脚与样式以及全局配置。",
                    ClassName = new List<string> { "modal-demo-desc" },
                },
                CreateBasicSection(),
                CreateSemanticSection(),
                CreateConfirmSection(),
                CreateAsyncSection(),
                CreateCustomSection(),
                CreateGlobalConfigSection(),
            };
        }

        // 基础对话框：展示标题 + 正文 + 默认 OK / Cancel 的最常见用法。
        private static UIView CreateBasicSection()
        {
            return CreateSectionCard(
                "基础对话框",
                "标题、正文和默认的 OK / Cancel 按钮，最常用的对话框形态。",
                CreateActionRow(
                    CreateActionButton("打开对话框", () =>
                    {
                        UIModal.Show(new ModalConfig
                        {
                            Title = "对话框标题",
                            Content = "这里是对话框的正文内容，用来描述需要用户确认的关键信息。",
                            OnOk = () => { UIMessage_Like("已点击确定"); return true; },
                            OnCancel = () => UIMessage_Like("已点击取消"),
                        });
                    }, "modal-action-primary"),
                    CreateActionButton("不可遮罩关闭", () =>
                    {
                        UIModal.Show(new ModalConfig
                        {
                            Title = "锁定遮罩",
                            Content = "点击遮罩不会关闭，必须通过按钮或 X 操作。",
                            MaskClosable = false,
                        });
                    }, "modal-action-neutral"),
                    CreateActionButton("禁用 Esc / 关闭按钮", () =>
                    {
                        UIModal.Show(new ModalConfig
                        {
                            Title = "强制确认",
                            Content = "Esc 键和右上角关闭按钮均被禁用，用户必须做出选择。",
                            Closable = false,
                            Keyboard = false,
                            MaskClosable = false,
                        });
                    }, "modal-action-dark"),
                    CreateActionButton("顶部对齐 (非居中)", () =>
                    {
                        UIModal.Show(new ModalConfig
                        {
                            Title = "顶部对齐对话框",
                            Content = "Centered = false 时使用 Top 指定离顶部的距离。",
                            Centered = false,
                            Top = 80,
                        });
                    }, "modal-action-neutral")
                ),
                CreateHintLabel("默认对话框为居中显示，含遮罩、Esc 关闭和右上角 X。")
            );
        }

        // 语义化变体：Info / Success / Error / Warning，对应不同图标与默认仅 OK。
        private static UIView CreateSemanticSection()
        {
            return CreateSectionCard(
                "语义化对话框",
                "通过 UIModal.Info / Success / Error / Warning 显示带图标的反馈对话框，默认仅一个 OK 按钮。",
                CreateActionRow(
                    CreateActionButton("Info", () => UIModal.Info("通知", "系统将于 2026-06-01 进行维护，请提前保存工作。"), "modal-action-primary"),
                    CreateActionButton("Success", () => UIModal.Success("操作成功", "你提交的内容已经保存到云端。"), "modal-action-success"),
                    CreateActionButton("Warning", () => UIModal.Warning("空间不足", "当前磁盘剩余空间不足 1GB，建议清理后再继续。"), "modal-action-warning"),
                    CreateActionButton("Error", () => UIModal.Error("操作失败", "请求被服务器拒绝（403），请检查权限。"), "modal-action-danger")
                ),
                CreateHintLabel("语义化对话框承担“非交互的状态反馈”，类似桌面端 MessageBox。")
            );
        }

        // Confirm：含 OK + Cancel 的确认型对话框，OkType 可选 Primary / Danger。
        private static UIView CreateConfirmSection()
        {
            return CreateSectionCard(
                "确认与危险确认",
                "Confirm 用于需要用户确认后才执行的操作；OkType = Danger 用于强调高风险，例如删除。",
                CreateActionRow(
                    CreateActionButton("确认提交", () =>
                    {
                        UIModal.Confirm(
                            "提交确认",
                            "提交后将无法再修改本次填写的内容，是否继续？",
                            onOk: () => UIMessage_Like("已提交"),
                            onCancel: () => UIMessage_Like("已取消")
                        );
                    }, "modal-action-primary"),
                    CreateActionButton("危险删除", () =>
                    {
                        UIModal.Confirm(
                            "删除确认",
                            "确定要删除该项目吗？该操作不可恢复。",
                            okType: ModalOkType.Danger,
                            okText: "删除",
                            cancelText: "再想想",
                            onOk: () => UIMessage_Like("已删除"),
                            onCancel: () => UIMessage_Like("已取消")
                        );
                    }, "modal-action-danger"),
                    CreateActionButton("自定义按钮文案", () =>
                    {
                        UIModal.Show(new ModalConfig
                        {
                            Title = "保存草稿？",
                            Content = "检测到未保存的更改，是否在退出前保存？",
                            Type = ModalType.Confirm,
                            OkText = "保存并退出",
                            CancelText = "放弃更改",
                            OnOk = () => { UIMessage_Like("已保存"); return true; },
                            OnCancel = () => UIMessage_Like("已丢弃"),
                        });
                    }, "modal-action-neutral")
                ),
                CreateHintLabel("OnOk 返回 false 可阻止默认关闭，用于在按钮回调中做校验。")
            );
        }

        // 异步 loading：点击 OK 后保留对话框 + 显示 loading，模拟提交等待结果。
        private static UIView CreateAsyncSection()
        {
            return CreateSectionCard(
                "异步确认 (ConfirmLoading)",
                "在 OnOk 中返回 false 阻止关闭，并通过 UpdateConfig 切换 ConfirmLoading，等异步任务结束后再 Destroy。",
                CreateActionRow(
                    CreateActionButton("提交并模拟 1.5s 等待", () =>
                    {
                        ModalInstance? instance = null;
                        instance = UIModal.Show(new ModalConfig
                        {
                            Key = AsyncModalKey,
                            Title = "提交工单",
                            Content = "点击确定后将向服务器发起提交请求，过程可能耗时 1~2 秒。",
                            OkText = "立即提交",
                            OnOk = () =>
                            {
                                // 进入 loading，并保留对话框
                                instance?.UpdateConfig(cfg => cfg.ConfirmLoading = true);
                                Task.Run(async () =>
                                {
                                    await Task.Delay(1500);
                                    instance?.UpdateConfig(cfg =>
                                    {
                                        cfg.ConfirmLoading = false;
                                        cfg.Content = "提交成功，工单编号 #20260522-001。";
                                    });
                                    await Task.Delay(600);
                                    instance?.Destroy();
                                });
                                return false; // 阻止默认关闭
                            },
                            OnCancel = () => UIMessage_Like("用户取消提交"),
                        });
                    }, "modal-action-primary"),
                    CreateActionButton("立刻关闭异步对话框", () => UIModal.Destroy(AsyncModalKey), "modal-action-neutral")
                ),
                CreateHintLabel("ConfirmLoading = true 时 OK 按钮会被禁用，用户依旧可以点击 Cancel / X 取消。")
            );
        }

        // 自定义页脚 / Body / 样式：演示对话框承载表单与品牌化样式的能力。
        private static UIView CreateCustomSection()
        {
            return CreateSectionCard(
                "自定义内容与样式",
                "对话框正文区支持注入任意 UIElement，页脚也可以完全替换；同时支持 BodyStyle / Style 自定义视觉。",
                CreateActionRow(
                    CreateActionButton("内嵌表单", OpenFormModal, "modal-action-primary"),
                    CreateActionButton("自定义页脚", OpenCustomFooterModal, "modal-action-neutral"),
                    CreateActionButton("品牌化样式", OpenBrandedModal, "modal-action-dark"),
                    CreateActionButton("无标题 / 无关闭按钮", () =>
                    {
                        UIModal.Show(new ModalConfig
                        {
                            Content = "这是一段最简对话框，只有正文和默认按钮。",
                            Closable = false,
                            Width = 360,
                        });
                    }, "modal-action-neutral")
                ),
                CreateHintLabel("使用 Body / FooterContent / Style 等属性可以无缝接入业务表单。")
            );
        }

        // 全局配置：影响后续新创建对话框的默认行为（如统一 ZIndex / 居中 / 遮罩样式）。
        private static UIView CreateGlobalConfigSection()
        {
            return CreateSectionCard(
                "全局配置与清空",
                "可统一控制默认宽度、遮罩样式与是否居中；DestroyAll 关闭当前所有对话框。",
                CreateActionRow(
                    CreateActionButton("更深遮罩", () =>
                    {
                        UIModal.Config(new ModalGlobalConfig
                        {
                            MaskStyle = new DefaultUIStyle
                            {
                                BackgroundColor = new SKColor(0, 0, 0, 170),
                            },
                            Width = 460,
                        });
                        UIModal.Info("已应用深色遮罩", "后续对话框的遮罩将更深，宽度默认 460。");
                    }, "modal-action-dark"),
                    CreateActionButton("无遮罩", () =>
                    {
                        UIModal.Config(new ModalGlobalConfig
                        {
                            Mask = false,
                            MaskClosable = false,
                        });
                        UIModal.Info("已关闭默认遮罩", "现在打开的对话框不会遮挡背景操作。");
                    }, "modal-action-neutral"),
                    CreateActionButton("恢复默认", () =>
                    {
                        ResetGlobalConfig();
                        UIMessage_Like("已恢复 Modal 默认配置");
                    }, "modal-action-success"),
                    CreateActionButton("关闭全部对话框", UIModal.DestroyAll, "modal-action-danger")
                ),
                CreateHintLabel("全局配置不会回写已显示的对话框，仅影响之后新创建的实例。")
            );
        }

        // ========== 自定义内容示例 ==========

        private static void OpenFormModal()
        {
            // 简易表单：注入到 Modal.Body 中
            var nameInput = new UIInput
            {
                Placeholder = "请输入姓名",
                ClassName = new List<string> { "modal-demo-form-input" },
            };
            var emailInput = new UIInput
            {
                Placeholder = "请输入邮箱",
                ClassName = new List<string> { "modal-demo-form-input" },
            };

            var form = new UIView
            {
                ClassName = new List<string> { "modal-demo-form" },
                Children = new()
                {
                    new UIView
                    {
                        ClassName = new List<string> { "modal-demo-form-row" },
                        Children = new()
                        {
                            new UILabel { Text = "姓名", ClassName = new List<string> { "modal-demo-form-label" } },
                            nameInput,
                        }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "modal-demo-form-row" },
                        Children = new()
                        {
                            new UILabel { Text = "邮箱", ClassName = new List<string> { "modal-demo-form-label" } },
                            emailInput,
                        }
                    },
                }
            };

            UIModal.Show(new ModalConfig
            {
                Title = "新建联系人",
                Body = form,
                OkText = "保存",
                Width = 460,
                OnOk = () =>
                {
                    // 简单校验：姓名不能为空
                    if (string.IsNullOrWhiteSpace(nameInput.Text))
                    {
                        UIMessage_Like("姓名不能为空");
                        return false; // 阻止关闭
                    }
                    UIMessage_Like($"已保存：{nameInput.Text}");
                    return true;
                },
            });
        }

        private static void OpenCustomFooterModal()
        {
            var footer = new UIView
            {
                Style = new DefaultUIStyle
                {
                    Display = "flex",
                    FlexDirection = "row",
                    AlignItems = "center",
                    JustifyContent = "space-between",
                    Width = "100%",
                },
                Children = new()
                {
                    new UILabel
                    {
                        Text = "需要帮助？查看文档",
                        Style = new DefaultUIStyle
                        {
                            FontSize = 12,
                            Color = ColorHelper.ParseColor("#1677ff"),
                            Cursor = TCYM.UI.Enums.UICursor.Pointer,
                        }
                    },
                    new UIView
                    {
                        Style = new DefaultUIStyle
                        {
                            Display = "flex",
                            FlexDirection = "row",
                            Gap = 8,
                        },
                        Children = new()
                        {
                            new UIButton
                            {
                                Text = "稍后再说",
                                ClassName = new List<string> { "modal-action-btn", "modal-action-neutral" },
                                Events = new() { Click = _ => UIModal.DestroyAll() },
                            },
                            new UIButton
                            {
                                Text = "立即升级",
                                ClassName = new List<string> { "modal-action-btn", "modal-action-primary" },
                                Events = new()
                                {
                                    Click = _ =>
                                    {
                                        UIMessage_Like("正在跳转升级页面...");
                                        UIModal.DestroyAll();
                                    }
                                },
                            },
                        }
                    }
                }
            };

            UIModal.Show(new ModalConfig
            {
                Title = "升级到专业版",
                Content = "解锁高级特性、扩展配额并享受 7×24 优先支持。",
                FooterContent = footer,
                HideFooter = false, // 自定义页脚替换默认按钮
                Width = 480,
            });
        }

        private static void OpenBrandedModal()
        {
            UIModal.Show(new ModalConfig
            {
                Title = "品牌化样式",
                Content = "通过 Style / BodyStyle / TitleStyle 可以为不同业务定制差异化的视觉风格。",
                OkText = "我知道了",
                HideFooter = false,
                Width = 460,
                Style = new DefaultUIStyle
                {
                    BackgroundColor = ColorHelper.ParseColor("#0f172a"),
                    BorderRadius = 16,
                    BoxShadowOffsetX = 0,
                    BoxShadowOffsetY = 14,
                    BoxShadowBlur = 32,
                    BoxShadowColor = new SKColor(15, 23, 42, 120),
                },
                TitleStyle = new DefaultUIStyle
                {
                    Color = ColorHelper.ParseColor("#f8fafc"),
                    FontSize = 18,
                },
                BodyStyle = new DefaultUIStyle
                {
                    PaddingLeft = 24,
                    PaddingRight = 24,
                    PaddingTop = 12,
                    PaddingBottom = 24,
                },
                MaskStyle = new DefaultUIStyle
                {
                    BackgroundColor = new SKColor(15, 23, 42, 180),
                },
            });
        }

        // ========== 通用工厂 ==========

        // 所有示例块共用的卡片外壳，统一标题、描述和内容布局。
        private static UIView CreateSectionCard(string title, string description, params UIElement[] children)
        {
            var sectionChildren = new List<UIElement>
            {
                new UILabel
                {
                    Text = title,
                    ClassName = new List<string> { "modal-card-title" },
                },
                new UILabel
                {
                    Text = description,
                    ClassName = new List<string> { "modal-card-desc" },
                }
            };

            sectionChildren.AddRange(children);

            return new UIView
            {
                ClassName = new List<string> { "modal-demo-card" },
                Children = sectionChildren,
            };
        }

        // 行容器用于承载一组演示按钮，保持每个 section 的交互区结构一致。
        private static UIView CreateActionRow(params UIElement[] buttons)
        {
            return new UIView
            {
                ClassName = new List<string> { "modal-showcase" },
                Children = buttons.ToList(),
            };
        }

        // 补充说明文本，放在每个卡片底部解释当前示例的使用场景。
        private static UILabel CreateHintLabel(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "modal-demo-hint" },
            };
        }

        // 按钮工厂：统一示例按钮的基础样式和点击绑定。
        private static UIButton CreateActionButton(string text, Action onClick, params string[] extraClasses)
        {
            var classNames = new List<string> { "modal-action-btn" };
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

        // 借用 Message 做轻量结果反馈，避免 Demo 弹出嵌套对话框。
        private static void UIMessage_Like(string text)
        {
            TCYM.UI.Elements.Message.UIMessage.Info(text, duration: 1.5f);
        }

        // 恢复 Modal 的默认全局配置。
        private static void ResetGlobalConfig()
        {
            UIModal.Config(new ModalGlobalConfig());
        }
    }
}
