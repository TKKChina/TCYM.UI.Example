using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.TimePicker;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.TimePicker
{
    internal class UITimePickerDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.TimePicker.style.css";

        internal UITimePickerDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = "timepicker-demo-view";
            Children = new()
            {
                new UILabel
                {
                    Text = "TimePicker 时间选择框",
                    ClassName = "timepicker-demo-title"
                },
                new UILabel
                {
                    Text = "输入或选择一天中的时间。",
                    ClassName = "timepicker-demo-subtitle"
                },
                new UILabel
                {
                    Text = "参考 Ant Design TimePicker，支持 24/12 小时制、格式、步长、禁用时间、确认模式、清空、尺寸、变体和状态。Value 使用 TimeSpan? 表示一天内时间。",
                    ClassName = "timepicker-demo-desc"
                },
                BuildBasicSection(),
                BuildFormatAndStepSection(),
                BuildSizeSection(),
                BuildVariantSection(),
                BuildDisabledSection(),
                BuildEventSection()
            };
        }

        private static UIElement BuildBasicSection()
        {
            return BuildCard(
                "基本使用",
                "点击输入框打开时、分、秒三列面板；可设置默认值、隐藏秒列或添加前缀。",
                new UITimePicker(),
                new UITimePicker
                {
                    Value = new TimeSpan(13, 30, 56)
                },
                new UITimePicker
                {
                    Format = "HH:mm",
                    Placeholder = "请选择时分"
                },
                new UITimePicker
                {
                    PrefixText = "UTC",
                    Value = new TimeSpan(8, 0, 0),
                    Format = "HH:mm"
                });
        }

        private static UIElement BuildFormatAndStepSection()
        {
            return BuildCard(
                "12 小时制与步长",
                "Use12Hours 增加 AM/PM 列；HourStep、MinuteStep、SecondStep 控制可选项间隔。",
                new UITimePicker
                {
                    Use12Hours = true,
                    Value = new TimeSpan(13, 5, 30)
                },
                new UITimePicker
                {
                    Value = new TimeSpan(9, 30, 20),
                    MinuteStep = 15,
                    SecondStep = 10
                },
                new UITimePicker
                {
                    Value = new TimeSpan(18, 45, 0),
                    Format = "HH '时' mm '分'"
                });
        }

        private static UIElement BuildSizeSection()
        {
            return BuildCard(
                "三种尺寸",
                "Small、Medium 和 Large 分别对应 24、32 和 40 像素高度。",
                new UITimePicker
                {
                    Size = TimePickerSize.Small,
                    Value = new TimeSpan(8, 0, 0),
                    Format = "HH:mm"
                },
                new UITimePicker
                {
                    Size = TimePickerSize.Medium,
                    Value = new TimeSpan(12, 30, 0),
                    Format = "HH:mm"
                },
                new UITimePicker
                {
                    Size = TimePickerSize.Large,
                    Value = new TimeSpan(18, 45, 0),
                    Format = "HH:mm"
                });
        }

        private static UIElement BuildVariantSection()
        {
            return BuildCard(
                "形态变体与校验状态",
                "支持 Outlined、Filled、Borderless、Underlined，以及 Error、Warning 状态。",
                new UITimePicker
                {
                    Variant = TimePickerVariant.Outlined,
                    Placeholder = "Outlined"
                },
                new UITimePicker
                {
                    Variant = TimePickerVariant.Filled,
                    Placeholder = "Filled"
                },
                new UITimePicker
                {
                    Variant = TimePickerVariant.Borderless,
                    Placeholder = "Borderless"
                },
                new UITimePicker
                {
                    Variant = TimePickerVariant.Underlined,
                    Placeholder = "Underlined"
                },
                new UITimePicker
                {
                    Status = TimePickerStatus.Error,
                    Placeholder = "Error"
                },
                new UITimePicker
                {
                    Status = TimePickerStatus.Warning,
                    Placeholder = "Warning"
                });
        }

        private static UIElement BuildDisabledSection()
        {
            return BuildCard(
                "禁用与可选时间",
                "第一个选择框整体禁用；第二个仅允许 09:00 到 18:00，并隐藏其余小时和 30 分钟之后的分钟选项。",
                new UITimePicker
                {
                    Disabled = true,
                    Value = new TimeSpan(12, 0, 0)
                },
                new UITimePicker
                {
                    Value = new TimeSpan(9, 15, 0),
                    Format = "HH:mm",
                    MinuteStep = 15,
                    HideDisabledOptions = true,
                    DisabledHour = hour => hour < 9 || hour > 18,
                    DisabledMinute = (_, minute) => minute > 30
                });
        }

        private static UIElement BuildEventSection()
        {
            var status = new UILabel
            {
                Text = "尚未选择",
                ClassName = "timepicker-event-status"
            };
            var picker = new UITimePicker
            {
                Value = new TimeSpan(10, 30, 0),
                MinuteStep = 5,
                NeedConfirm = true,
                RenderExtraFooter = () => new UILabel
                {
                    Text = "工作日排班时间",
                    Style = new DefaultUIStyle
                    {
                        FontSize = 12,
                        Color = ColorHelper.ParseColor("rgba(0,0,0,0.55)")
                    }
                },
                OnSelect = value => status.Text = value.HasValue
                    ? $"待确认：{value.Value:hh\\:mm\\:ss}"
                    : "待确认：空",
                OnChange = value => status.Text = value.HasValue
                    ? $"已提交：{value.Value:hh\\:mm\\:ss}"
                    : "已清空",
                OnClear = () => status.Text = "已清空"
            };

            var immediateStatus = new UILabel
            {
                Text = "即时提交：尚未操作",
                ClassName = "timepicker-event-status"
            };
            var immediate = new UITimePicker
            {
                Format = "HH:mm",
                MinuteStep = 15,
                NeedConfirm = false,
                ShowNow = false,
                OnChange = value => immediateStatus.Text = value.HasValue
                    ? $"即时提交：{value.Value:hh\\:mm}"
                    : "即时提交：已清空"
            };

            return BuildCard(
                "确认模式、附加内容与事件",
                "NeedConfirm=true 时 OnSelect 更新待选值，点击确定后触发 OnChange；关闭确认模式后选择即提交。",
                new UIView
                {
                    ClassName = "timepicker-event-group",
                    Children = new() { picker, status }
                },
                new UIView
                {
                    ClassName = "timepicker-event-group",
                    Children = new() { immediate, immediateStatus }
                });
        }

        private static UIElement BuildCard(string title, string description, params UIElement[] children)
        {
            return new UIView
            {
                ClassName = "timepicker-demo-card",
                Children = new()
                {
                    new UILabel
                    {
                        Text = title,
                        ClassName = "timepicker-card-title"
                    },
                    new UILabel
                    {
                        Text = description,
                        ClassName = "timepicker-card-desc"
                    },
                    new UIView
                    {
                        ClassName = "timepicker-showcase",
                        Children = children.ToList()
                    }
                }
            };
        }
    }
}
