using TCYM.UI.Chart;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Core;
using TCYM.UI.Core.Routing;
using TCYM.UI.Elements;

namespace Page.Charts.Candlestick
{
    /// <summary>沿用 Lines 的标题、说明、预览卡片和图表释放方式。</summary>
    internal abstract class CandlestickChartDemoPage : UIScrollView, IUIRouteLifecycle
    {
        private bool _disposed;
        private readonly UIElement _canvas;

        /// <summary>普通案例的 UIChart；后台案例通过自己的画布发送命令。</summary>
        protected UIChart Chart => (UIChart)_canvas;

        /// <summary>放置缩放、刷选等示例操作按钮；具体 Options 仍由各案例自行创建。</summary>
        protected UIView Toolbar { get; }

        protected CandlestickChartDemoPage(
            string title,
            string subtitle,
            string description,
            ChartOption option,
            ChartRegistry? registry = null)
            : this(title, subtitle, description,
                new UIChart(registry ?? ChartModules.CreateDefaultRegistry()))
        {
            Chart.SetOption(option, new ChartSetOptionOptions
            {
                NotMerge = true,
                LazyUpdate = true,
            });
        }

        /// <summary>允许大数据案例提供后台画布，构造页面时不执行耗时 SetOption。</summary>
        protected CandlestickChartDemoPage(
            string title,
            string subtitle,
            string description,
            UIElement canvas)
        {
            UISystem.LoadStyleFile("res://TCYM.UI.Example/Page.Charts.Candlestick.style.css");

            _canvas = canvas;
            _canvas.ClassName = "candlestick-chart-canvas";
            Toolbar = new UIView { ClassName = "candlestick-chart-toolbar" };
            ClassName = "candlestick-chart-demo";
            Children =
            [
                new UILabel { Text = title, ClassName = "candlestick-chart-demo-title" },
                new UILabel { Text = subtitle, ClassName = "candlestick-chart-demo-subtitle" },
                new UILabel { Text = description, ClassName = "candlestick-chart-demo-description" },
                new UIView
                {
                    ClassName = "candlestick-chart-preview-card",
                    Children =
                    [
                        new UILabel { Text = "效果预览", ClassName = "candlestick-chart-card-title" },
                        new UILabel
                        {
                            Text = "预览画布与 Lines 一样为 100% × 590；Ctrl + 滚轮缩放，拖动底部滑块调整查看范围。",
                            ClassName = "candlestick-chart-preview-help",
                        },
                        Toolbar,
                        new UIScrollView
                        {
                            ClassName = "candlestick-chart-preview-scroll",
                            Children = [_canvas],
                        },
                    ],
                },
            ];
        }

        /// <summary>用普通按钮接入图表动作；事件采用标准 += 订阅。</summary>
        protected UIButton AddToolbarButton(string text, Action action)
        {
            var button = new UIButton { Text = text, ClassName = "candlestick-chart-toolbar-button" };
            button.OnClick += _ => action();
            Toolbar.AddChild(button);
            return button;
        }

        internal static CandlestickChartDemoPage CreateShanghaiIndex() => new ShanghaiIndexCandlestickChartDemo();
        internal static CandlestickChartDemoPage CreateLargeData() => new LargeDataCandlestickChartDemo();
        internal static CandlestickChartDemoPage CreateIntradayMultipleDays() => new IntradayMultipleDaysChartDemo();
        internal static CandlestickChartDemoPage CreateIntradaySingleDay() => new IntradaySingleDayChartDemo();
        internal static CandlestickChartDemoPage CreateBrush() => new BrushCandlestickChartDemo();

        public virtual void OnRouteEnter(string? fromPath) { }

        public void OnRouteLeave(string? toPath)
        {
            if (_disposed) return;
            if (_canvas is UIChart chart)
            {
                chart.Clear();
                chart.Dispose();
            }
            else if (_canvas is IDisposable disposable)
            {
                disposable.Dispose();
            }
            _disposed = true;
        }
    }
}
