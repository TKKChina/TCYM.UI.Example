using SkiaSharp;
using TCYM.UI.Chart.Components.Mark;
using TCYM.UI.Chart.Core;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Scatter
{
    /// <summary>使用四个独立 Grid 展示安斯库姆四重奏。</summary>
    internal sealed class AnscombeQuartetScatterChartDemo : ScatterChartDemoPage
    {
        /// <summary>四组数据依次使用的系列颜色。</summary>
        private static readonly SKColor[] SeriesColors =
        [
            new(84, 112, 198),
            new(145, 204, 117),
            new(250, 200, 88),
            new(238, 102, 102),
        ];

        /// <summary>创建安斯库姆四重奏示例页面。</summary>
        internal AnscombeQuartetScatterChartDemo()
            : base(
                "安斯库姆四重奏",
                "四组统计特征相近的数据，在散点图中呈现完全不同的分布。",
                "四个 Scatter 分别绑定四套 Grid、X 轴和 Y 轴；每个系列还拥有独立的 y = 0.5 * x + 3 参考线。",
                CreateOption(),
                CreateRegistry())
        {
        }

        /// <summary>创建包含 Mark 模块的图表注册表。</summary>
        /// <returns>可绘制四条独立 MarkLine 的图表注册表。</returns>
        private static ChartRegistry CreateRegistry()
        {
            var registry = ChartModules.CreateDefaultRegistry();
            ChartMarkModule.Register(registry);
            return registry;
        }

        /// <summary>创建安斯库姆四重奏的完整配置。</summary>
        /// <returns>包含四个 Grid、八条轴、四个 Scatter 和四条参考线的配置。</returns>
        private static ChartOption CreateOption()
        {
            var option = new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new()
                {
                    Enabled = true,
                    Duration = 650,
                },
                Title = new ChartTitleOption
                {
                    Id = "anscombe-title",
                    Text = "散点图",
                    Left = ChartLength.FromKeyword("center"),
                    Top = "0px",
                },
                Tooltip = new ChartTooltipOption
                {
                    Id = "anscombe-tooltip",
                    Trigger = ChartTooltipTrigger.Item,
                    Confine = true,
                    Formatter = "点 {a}: ({c})",
                },
            };

            // GridIndex、XAxisIndex 和 YAxisIndex 依靠下面完全相同的 0..3 顺序关联。
            option.AddComponent(CreateGrid(
                "anscombe-grid-1",
                left: ChartLength.Percent(7f),
                top: ChartLength.Percent(7f)));
            option.AddComponent(CreateGrid(
                "anscombe-grid-2",
                right: ChartLength.Percent(7f),
                top: ChartLength.Percent(7f)));
            option.AddComponent(CreateGrid(
                "anscombe-grid-3",
                left: ChartLength.Percent(7f),
                bottom: ChartLength.Percent(7f)));
            option.AddComponent(CreateGrid(
                "anscombe-grid-4",
                right: ChartLength.Percent(7f),
                bottom: ChartLength.Percent(7f)));

            for (var gridIndex = 0; gridIndex < 4; gridIndex++)
            {
                option.XAxis.Add(CreateXAxis(gridIndex));
                option.YAxis.Add(CreateYAxis(gridIndex));
            }

            option.Series.Add(CreateSeries(
                index: 0,
                name: "I",
                points:
                [
                    (10.0d, 8.04d), (8.0d, 6.95d), (13.0d, 7.58d),
                    (9.0d, 8.81d), (11.0d, 8.33d), (14.0d, 9.96d),
                    (6.0d, 7.24d), (4.0d, 4.26d), (12.0d, 10.84d),
                    (7.0d, 4.82d), (5.0d, 5.68d),
                ]));
            option.Series.Add(CreateSeries(
                index: 1,
                name: "II",
                points:
                [
                    (10.0d, 9.14d), (8.0d, 8.14d), (13.0d, 8.74d),
                    (9.0d, 8.77d), (11.0d, 9.26d), (14.0d, 8.10d),
                    (6.0d, 6.13d), (4.0d, 3.10d), (12.0d, 9.13d),
                    (7.0d, 7.26d), (5.0d, 4.74d),
                ]));
            option.Series.Add(CreateSeries(
                index: 2,
                name: "III",
                points:
                [
                    (10.0d, 7.46d), (8.0d, 6.77d), (13.0d, 12.74d),
                    (9.0d, 7.11d), (11.0d, 7.81d), (14.0d, 8.84d),
                    (6.0d, 6.08d), (4.0d, 5.39d), (12.0d, 8.15d),
                    (7.0d, 6.42d), (5.0d, 5.73d),
                ]));
            option.Series.Add(CreateSeries(
                index: 3,
                name: "IV",
                points:
                [
                    (8.0d, 6.58d), (8.0d, 5.76d), (8.0d, 7.71d),
                    (8.0d, 8.84d), (8.0d, 8.47d), (8.0d, 7.04d),
                    (8.0d, 5.25d), (19.0d, 12.50d), (8.0d, 5.56d),
                    (8.0d, 7.91d), (8.0d, 6.89d),
                ]));

            return option;
        }

        /// <summary>创建四宫格中的一个绘图区。</summary>
        /// <param name="id">Grid 稳定 ID。</param>
        /// <param name="left">可选左侧位置。</param>
        /// <param name="top">可选顶部位置。</param>
        /// <param name="right">可选右侧位置。</param>
        /// <param name="bottom">可选底部位置。</param>
        /// <returns>宽高固定为 38% 的 Grid。</returns>
        private static ChartGridOption CreateGrid(
            string id,
            ChartLength? left = null,
            ChartLength? top = null,
            ChartLength? right = null,
            ChartLength? bottom = null) => new()
        {
            Id = id,
            Show = true,
            Left = left,
            Top = top,
            Right = right,
            Bottom = bottom,
            Width = "38%",
            Height = "38%",
            ContainLabel = true,
            OuterBoundsMode = ChartGridOuterBoundsMode.None,
            BorderWidth = 0f,
            BackgroundColor = ChartBrush.Solid(SKColors.White),
        };

        /// <summary>创建固定范围为 0..20 的水平数值轴。</summary>
        /// <param name="gridIndex">轴所属 Grid 的索引。</param>
        /// <returns>与指定 Grid 绑定的 X 轴。</returns>
        private static ChartXAxisOption CreateXAxis(int gridIndex) => new()
        {
            Id = $"anscombe-x-{gridIndex + 1}",
            GridIndex = gridIndex,
            Type = ChartAxisType.Value,
            Minimum = ChartAxisBound.Fixed(0d),
            Maximum = ChartAxisBound.Fixed(20d),
            Interval = 5d,
            Scale = true,
            AxisLabel = CreateAxisLabel(),
            SplitLine = CreateSplitLine(),
        };

        /// <summary>创建固定范围为 0..15 的垂直数值轴。</summary>
        /// <param name="gridIndex">轴所属 Grid 的索引。</param>
        /// <returns>与指定 Grid 绑定的 Y 轴。</returns>
        private static ChartYAxisOption CreateYAxis(int gridIndex) => new()
        {
            Id = $"anscombe-y-{gridIndex + 1}",
            GridIndex = gridIndex,
            Type = ChartAxisType.Value,
            Minimum = ChartAxisBound.Fixed(0d),
            Maximum = ChartAxisBound.Fixed(15d),
            Interval = 5d,
            Scale = true,
            AxisLabel = CreateAxisLabel(),
            SplitLine = CreateSplitLine(),
        };

        /// <summary>创建四组数据中的一个 Scatter 系列。</summary>
        /// <param name="index">系列及坐标轴索引。</param>
        /// <param name="name">系列罗马数字名称。</param>
        /// <param name="points">需要原样保留的二维点集合。</param>
        /// <returns>绑定独立坐标轴和独立 MarkLine 的 Scatter 系列。</returns>
        private static ChartScatterSeriesOption CreateSeries(
            int index,
            string name,
            IReadOnlyList<(double X, double Y)> points)
        {
            var data = new ChartScatterData();
            foreach (var (x, y) in points)
            {
                data.Add(x, y);
            }

            var color = SeriesColors[index];
            var seriesId = $"anscombe-series-{index + 1}";
            return new ChartScatterSeriesOption
            {
                Id = seriesId,
                Name = name,
                XAxisIndex = index,
                YAxisIndex = index,
                Symbol = "circle",
                SymbolSize = 10f,
                Clip = true,
                DataSource = data,
                ItemStyle = new()
                {
                    Color = ChartBrush.Solid(color),
                },
                Emphasis = new()
                {
                    Focus = "series",
                    BlurScope = "coordinateSystem",
                },
                // 每个系列都创建自己的 MarkLine，避免四个面板共享可变 Option 实例。
                MarkLine = CreateReferenceLine(seriesId, color),
            };
        }

        /// <summary>创建 y = 0.5 * x + 3 的独立参考线。</summary>
        /// <param name="seriesId">宿主系列稳定 ID。</param>
        /// <param name="color">参考线及标签颜色。</param>
        /// <returns>从 (0,3) 延伸到 (20,13) 的 MarkLine。</returns>
        private static ChartMarkLineOption CreateReferenceLine(string seriesId, SKColor color) => new()
        {
            Animation = true,
            Symbol = ChartMarkSymbolPair.Uniform("none"),
            LineStyle = new()
            {
                Color = ChartBrush.Solid(color),
                Type = ChartLineStyleType.Solid,
                Width = 1.5f,
            },
            Label = new()
            {
                Show = true,
                Position = "end",
                Align = ChartTextAlign.Right,
                Distance = 4f,
                Formatter = "y = 0.5 * x + 3",
                Color = ChartBrush.Solid(color),
                FontSize = 10f,
            },
            Tooltip = new()
            {
                Show = true,
                ShowContent = true,
                Trigger = ChartTooltipTrigger.Item,
                Confine = true,
                Formatter = "y = 0.5 * x + 3",
            },
            Data =
            {
                new ChartMarkLineEndpointsDataItem(
                    new ChartMarkLineEndpointOption(new ChartMarkPositionOption
                    {
                        Coordinate = new ChartMarkCoordinate(0d, 3d),
                    }),
                    new ChartMarkLineEndpointOption(new ChartMarkPositionOption
                    {
                        Coordinate = new ChartMarkCoordinate(20d, 13d),
                    }))
                {
                    Key = $"{seriesId}-reference-line",
                    Name = "y = 0.5 * x + 3",
                },
            },
        };

        /// <summary>创建四宫格坐标轴共用的标签样式。</summary>
        /// <returns>适合紧凑面板的轴标签。</returns>
        private static ChartAxisLabelOption CreateAxisLabel() => new()
        {
            Show = true,
            Color = ChartBrush.Solid(new SKColor(72, 79, 91)),
            FontSize = 11f,
            HideOverlap = true,
        };

        /// <summary>创建四宫格坐标轴共用的浅色虚线分隔线。</summary>
        /// <returns>不会遮挡散点的轴分隔线。</returns>
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
