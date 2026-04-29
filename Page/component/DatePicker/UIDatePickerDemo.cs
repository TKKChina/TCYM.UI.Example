using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.DatePicker
{
    internal class UIDatePickerDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.DatePicker.style.css";

        internal UIDatePickerDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "datepicker-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "DatePicker 日期选择框",
                    ClassName = new List<string> { "datepicker-demo-title" },
                },
                new UILabel
                {
                    Text = "输入或选择日期的控件。",
                    ClassName = new List<string> { "datepicker-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "当用户需要输入一个日期，可以点击标准输入框，弹出日期面板进行选择。支持日期、月份、年份、周、季度等多种选择模式。",
                    ClassName = new List<string> { "datepicker-demo-desc" },
                },
                new BasicSection(),
                new FormatSection(),
                new DisabledSection(),
                new ShowTimeSection(),
                new RangeSection(),
                new EventSection(),
            };
        }

        /// <summary>
        /// 基础日期选择
        /// </summary>
        private class BasicSection : UIView
        {
            internal BasicSection()
            {
                ClassName = new List<string> { "datepicker-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "基础日期选择",
                        ClassName = new List<string> { "datepicker-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "支持日期、月份、年份、周、季度五种选择模式，通过 Picker 属性切换。",
                        ClassName = new List<string> { "datepicker-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "datepicker-showcase" },
                        Children = new()
                        {
                            new UIDatePicker
                            {
                                Placeholder = "请选择日期",
                                AllowClear = true,
                            },
                            new UIDatePicker
                            {
                                Picker = DatePickerType.Month,
                                Placeholder = "请选择月份",
                                AllowClear = true,
                            },
                            new UIDatePicker
                            {
                                Picker = DatePickerType.Year,
                                Placeholder = "请选择年份",
                                AllowClear = true,
                            },
                            new UIDatePicker
                            {
                                Picker = DatePickerType.Week,
                                Placeholder = "请选择周",
                                AllowClear = true,
                            },
                            new UIDatePicker
                            {
                                Picker = DatePickerType.Quarter,
                                Placeholder = "请选择季度",
                                AllowClear = true,
                            },
                            new UIDatePicker
                            {
                                Picker = DatePickerType.Date,
                                Value = DateTime.Now,
                                AllowClear = true,
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 日期格式
        /// </summary>
        private class FormatSection : UIView
        {
            internal FormatSection()
            {
                ClassName = new List<string> { "datepicker-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "日期格式",
                        ClassName = new List<string> { "datepicker-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "通过 Format 属性自定义日期显示格式。",
                        ClassName = new List<string> { "datepicker-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "datepicker-showcase" },
                        Children = new()
                        {
                            new UIDatePicker
                            {
                                Placeholder = "请选择日期",
                                AllowClear = true,
                                Format = "yyyy年MM月dd日",
                            },
                            new UIDatePicker
                            {
                                Picker = DatePickerType.Month,
                                Placeholder = "请选择月份",
                                AllowClear = true,
                                Format = "yyyy年MM月",
                            },
                            new UIDatePicker
                            {
                                Picker = DatePickerType.Quarter,
                                Placeholder = "请选择季度",
                                AllowClear = true,
                                Format = "yyyy年Q季度",
                            },
                            new UIDatePicker
                            {
                                Picker = DatePickerType.Week,
                                Placeholder = "请选择周",
                                AllowClear = true,
                                Format = "yyyy年WW周",
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 禁用状态
        /// </summary>
        private class DisabledSection : UIView
        {
            internal DisabledSection()
            {
                ClassName = new List<string> { "datepicker-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "禁用状态",
                        ClassName = new List<string> { "datepicker-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "通过 Disabled 属性禁用日期选择器。",
                        ClassName = new List<string> { "datepicker-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "datepicker-showcase" },
                        Children = new()
                        {
                            new UIDatePicker
                            {
                                Picker = DatePickerType.Date,
                                Disabled = true,
                                AllowClear = true,
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 日期时间选择
        /// </summary>
        private class ShowTimeSection : UIView
        {
            internal ShowTimeSection()
            {
                ClassName = new List<string> { "datepicker-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "日期时间选择",
                        ClassName = new List<string> { "datepicker-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "通过 ShowTime 属性在日期选择的基础上增加时间选择功能。",
                        ClassName = new List<string> { "datepicker-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "datepicker-showcase" },
                        Children = new()
                        {
                            new UIDatePicker
                            {
                                Placeholder = "请选择日期时间",
                                ShowTime = true,
                                AllowClear = true,
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 区间选择
        /// </summary>
        private class RangeSection : UIView
        {
            internal RangeSection()
            {
                ClassName = new List<string> { "datepicker-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "区间选择",
                        ClassName = new List<string> { "datepicker-card-title", "label-purple" }
                    },
                    new UILabel
                    {
                        Text = "使用 UIRangePicker 组件选择日期范围，支持多种类型。",
                        ClassName = new List<string> { "datepicker-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "datepicker-showcase" },
                        Children = new()
                        {
                            new UIRangePicker
                            {
                                Placeholder = "请选择日期范围",
                                AllowClear = true,
                                Style = new UpdateUIStyle { Width = 280 },
                            },
                            new UIRangePicker
                            {
                                Placeholder = "请选择日期范围",
                                AllowClear = true,
                                Format = "yyyy年MM月dd日",
                                Style = new UpdateUIStyle { Width = 280 },
                            },
                            new UIRangePicker
                            {
                                Picker = DatePickerType.Month,
                                Placeholder = "请选择月份范围",
                                AllowClear = true,
                                Format = "yyyy年MM月",
                                Style = new UpdateUIStyle { Width = 280 },
                            },
                            new UIRangePicker
                            {
                                Picker = DatePickerType.Quarter,
                                Placeholder = "请选择季度范围",
                                AllowClear = true,
                                Format = "yyyy年Q季度",
                                Style = new UpdateUIStyle { Width = 280 },
                            },
                            new UIRangePicker
                            {
                                Picker = DatePickerType.Week,
                                Placeholder = "请选择周范围",
                                AllowClear = true,
                                Format = "yyyy年WW周",
                                Style = new UpdateUIStyle { Width = 280 },
                            },
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 事件回调
        /// </summary>
        private class EventSection : UIView
        {
            internal EventSection()
            {
                ClassName = new List<string> { "datepicker-demo-card" };

                var dateLabel = new UILabel
                {
                    Text = "日期选择：未操作",
                    Style = new DefaultUIStyle
                    {
                        FontSize = 13,
                        Color = ColorHelper.ParseColor("rgba(0,0,0,0.65)"),
                    }
                };

                var rangeLabel = new UILabel
                {
                    Text = "范围选择：未操作",
                    Style = new DefaultUIStyle
                    {
                        FontSize = 13,
                        Color = ColorHelper.ParseColor("rgba(0,0,0,0.65)"),
                    }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "事件回调",
                        ClassName = new List<string> { "datepicker-card-title", "label-cany" }
                    },
                    new UILabel
                    {
                        Text = "通过 OnChange 和 OnOk 监听日期变化与确认操作。",
                        ClassName = new List<string> { "datepicker-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "datepicker-showcase" },
                        Children = new()
                        {
                            new UIDatePicker
                            {
                                Placeholder = "请选择日期",
                                AllowClear = true,
                                OnChange = d =>
                                {
                                    dateLabel.Text = d.HasValue
                                        ? $"日期选择：{d.Value:yyyy-MM-dd}"
                                        : "日期选择：已清空";
                                },
                            },
                            dateLabel,
                            new UIRangePicker
                            {
                                Placeholder = "请选择日期范围",
                                AllowClear = true,
                                Style = new UpdateUIStyle { Width = 280 },
                                OnChange = (s, e) =>
                                {
                                    if (s.HasValue && e.HasValue)
                                        rangeLabel.Text = $"范围选择：{s.Value:yyyy-MM-dd} ~ {e.Value:yyyy-MM-dd}";
                                    else
                                        rangeLabel.Text = "范围选择：已清空";
                                },
                            },
                            rangeLabel,
                        }
                    },
                };
            }
        }
    }
}
