using SkiaSharp;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Scatter
{
    /// <summary>展示二维数值坐标系中的基础散点图。</summary>
    internal sealed class BasicScatterChartDemo : ScatterChartDemoPage
    {
        /// <summary>基础散点使用的 ECharts 风格蓝色。</summary>
        private static readonly SKColor PointColor = new(84, 112, 198);

        /// <summary>创建基础散点图示例页面。</summary>
        internal BasicScatterChartDemo()
            : base(
                "基础散点图",
                "在两个数值轴上展示一组二维离散点。",
                "每个数据项依次保存 X、Y 两个值；SymbolSize 统一设置为 20px，适合直接观察点在数值坐标系中的分布。",
                CreateOption())
        {
        }

        /// <summary>创建基础散点图的完整配置。</summary>
        /// <returns>包含 22 个原始数据点、数值轴和 Tooltip 的图表配置。</returns>
        private static ChartOption CreateOption()
        {
            // Scatter 的二维数据按 [x, y] 顺序添加，不需要额外的 category 轴。
            var data = new ChartScatterData()
                .Add(10.00d, 8.04d)
                .Add(8.07d, 6.95d)
                .Add(13.00d, 7.58d)
                .Add(9.05d, 8.81d)
                .Add(11.00d, 8.33d)
                .Add(14.00d, 7.66d)
                .Add(13.40d, 6.81d)
                .Add(10.00d, 6.33d)
                .Add(14.00d, 8.96d)
                .Add(12.50d, 6.82d)
                .Add(9.15d, 7.20d)
                .Add(11.50d, 7.20d)
                .Add(3.03d, 4.23d)
                .Add(12.20d, 7.83d)
                .Add(2.02d, 4.47d)
                .Add(1.05d, 3.33d)
                .Add(4.05d, 4.96d)
                .Add(6.03d, 7.24d)
                .Add(12.00d, 6.26d)
                .Add(12.00d, 8.84d)
                .Add(7.08d, 5.82d)
                .Add(5.02d, 5.68d);

            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 700,
                },
                Tooltip = new ChartTooltipOption
                {
                    Id = "basic-scatter-tooltip",
                    Trigger = ChartTooltipTrigger.Item,
                    Confine = true,
                    Formatter = "点 {a}: ({c})",
                },
                Grid = new ChartGridOption
                {
                    Show = true,
                    Left = "24px",
                    Top = "22px",
                    Right = "24px",
                    Bottom = "24px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                    BackgroundColor = ChartBrush.Solid(SKColors.White),
                },
                XAxis = CreateValueAxisX(),
                YAxis = CreateValueAxisY(),
                Series = new ChartScatterSeriesOption
                {
                    Id = "basic-scatter-series",
                    Name = "数据点",
                    Symbol = "circle",
                    SymbolSize = 20f,
                    Clip = true,
                    DataSource = data,
                    ItemStyle = new()
                    {
                        Color = ChartBrush.Solid(PointColor),
                    },
                    Emphasis = new()
                    {
                        Focus = "series",
                        BlurScope = "coordinateSystem",
                    },
                },
            };
        }

        /// <summary>创建基础散点图的水平数值轴。</summary>
        /// <returns>带浅色分隔线的 X 轴。</returns>
        private static ChartXAxisOption CreateValueAxisX() => new()
        {
            Type = ChartAxisType.Value,
            Minimum = ChartAxisBound.Fixed(0d),
            Maximum = ChartAxisBound.Fixed(15d),
            Interval = 3d,
            Scale = false,
            AxisLabel = new()
            {
                Color = ChartBrush.Solid(new SKColor(72, 79, 91)),
                FontSize = 13f,
            },
            SplitLine = CreateSplitLine(),
        };

        /// <summary>创建基础散点图的垂直数值轴。</summary>
        /// <returns>带浅色分隔线的 Y 轴。</returns>
        private static ChartYAxisOption CreateValueAxisY() => new()
        {
            Type = ChartAxisType.Value,
            Minimum = ChartAxisBound.Fixed(0d),
            Maximum = ChartAxisBound.Fixed(10d),
            Interval = 2d,
            Scale = false,
            AxisLabel = new()
            {
                Color = ChartBrush.Solid(new SKColor(72, 79, 91)),
                FontSize = 13f,
            },
            SplitLine = CreateSplitLine(),
        };

        /// <summary>创建两个数值轴共用的浅色虚线分隔线。</summary>
        /// <returns>不会抢夺数据点视觉焦点的轴分隔线。</returns>
        private static ChartAxisSplitLineOption CreateSplitLine() => new()
        {
            Show = true,
            LineStyle = new()
            {
                Color = ChartBrush.Solid(new SKColor(225, 230, 238)),
                Type = ChartAxisLineType.Dashed,
                Width = 1f,
            },
        };
    }
}
