using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Scatter
{
    /// <summary>展示先在 C# 中完成 KMeans，再用 VisualMap 为聚类结果着色的散点图。</summary>
    internal sealed class ClusteringScatterChartDemo : ScatterChartDemoPage
    {
        private const int ClusterCount = 6;
        private const string SeriesId = "clustering-points";
        private const double Epsilon = 1e-12;

        private static readonly SKColor[] ClusterColors =
        [
            new(80, 112, 221), new(182, 214, 52), new(80, 83, 114),
            new(255, 153, 77), new(12, 168, 223), new(255, 209, 10),
        ];

        /// <summary>原示例的六十个二维点，Index 用于稳定处理距离相同的情况。</summary>
        private static readonly Sample[] Samples =
        [
            new(3.275154, 2.957587, 0), new(-3.344465, 2.603513, 1),
            new(0.355083, -3.376585, 2), new(1.852435, 3.547351, 3),
            new(-2.078973, 2.552013, 4), new(-0.993756, -0.884433, 5),
            new(2.682252, 4.007573, 6), new(-3.087776, 2.878713, 7),
            new(-1.565978, -1.256985, 8), new(2.441611, 0.444826, 9),
            new(-0.659487, 3.111284, 10), new(-0.459601, -2.618005, 11),
            new(2.17768, 2.387793, 12), new(-2.920969, 2.917485, 13),
            new(-0.028814, -4.168078, 14), new(3.625746, 2.119041, 15),
            new(-3.912363, 1.325108, 16), new(-0.551694, -2.814223, 17),
            new(2.855808, 3.483301, 18), new(-3.594448, 2.856651, 19),
            new(0.421993, -2.372646, 20), new(1.650821, 3.407572, 21),
            new(-2.082902, 3.384412, 22), new(-0.718809, -2.492514, 23),
            new(4.513623, 3.841029, 24), new(-4.822011, 4.607049, 25),
            new(-0.656297, -1.449872, 26), new(1.919901, 4.439368, 27),
            new(-3.287749, 3.918836, 28), new(-1.576936, -2.977622, 29),
            new(3.598143, 1.97597, 30), new(-3.977329, 4.900932, 31),
            new(-1.79108, -2.184517, 32), new(3.914654, 3.559303, 33),
            new(-1.910108, 4.166946, 34), new(-1.226597, -3.317889, 35),
            new(1.148946, 3.345138, 36), new(-2.113864, 3.548172, 37),
            new(0.845762, -3.589788, 38), new(2.629062, 3.535831, 39),
            new(-1.640717, 2.990517, 40), new(-1.881012, -2.485405, 41),
            new(4.606999, 3.510312, 42), new(-4.366462, 4.023316, 43),
            new(0.765015, -3.00127, 44), new(3.121904, 2.173988, 45),
            new(-4.025139, 4.65231, 46), new(-0.559558, -3.840539, 47),
            new(4.376754, 4.863579, 48), new(-1.874308, 4.032237, 49),
            new(-0.089337, -3.026809, 50), new(3.997787, 2.518662, 51),
            new(-3.082978, 2.884822, 52), new(0.845235, -3.454465, 53),
            new(1.327224, 3.358778, 54), new(-2.889949, 3.596178, 55),
            new(-0.966018, -2.839827, 56), new(2.960769, 3.079555, 57),
            new(-3.275518, 1.577068, 58), new(0.639276, -3.41284, 59),
        ];

        internal ClusteringScatterChartDemo()
            : base(
                "数据聚合",
                "在 Example 中对原始二维点执行可重复的六组 KMeans 聚类。",
                "每个点依次保存 X、Y 和聚类编号；单条 Scatter 绘制全部点，左侧 Piecewise VisualMap 根据第三维着色，并可点击集群进行筛选。",
                CreateOption())
        {
        }

        /// <summary>创建聚类数据、离散 VisualMap 和坐标轴。</summary>
        private static ChartOption CreateOption()
        {
            // 不使用 Random，保证每次打开页面得到相同的集群编号和颜色。
            var clusterIndexes = RunKMeans(Samples, ClusterCount);
            var data = new ChartScatterData();
            for (var index = 0; index < Samples.Length; index++)
            {
                var point = Samples[index];
                data.Add(new ChartScatterDataItem(
                    new ChartValue[] { point.X, point.Y, clusterIndexes[index] })
                {
                    Key = "cluster-point-" + point.Index,
                    Name = "点 " + (point.Index + 1),
                });
            }

            var visualMap = new ChartVisualMapPiecewiseOption
            {
                Id = "clustering-visual-map",
                SeriesId = SeriesId,
                Dimension = 2,
                Min = 0d,
                Max = ClusterCount - 1d,
                SelectedMode = ChartVisualMapSelectedMode.Multiple,
                Orient = ChartVisualMapOrient.Vertical,
                Left = "14px",
                Top = "112px",
                ItemWidth = 16f,
                ItemHeight = 14f,
                ItemGap = 8f,
                HoverLink = true,
            };
            for (var index = 0; index < ClusterCount; index++)
            {
                var piece = new ChartVisualMapPiece
                {
                    Value = index,
                    Label = "集群 " + index,
                };
                piece.Visual.Color.Add(ChartBrush.Solid(ClusterColors[index]));
                visualMap.Pieces.Add(piece);
            }

            return new ChartOption
            {
                Background = ChartBrush.Solid(SKColors.White),
                Animation = new() { Enabled = true, Duration = 700 },
                Tooltip = new ChartTooltipOption
                {
                    Trigger = ChartTooltipTrigger.Item,
                    TriggerOn = ChartTooltipTriggerOn.MouseMove | ChartTooltipTriggerOn.Click,
                    Confine = true,
                    FormatterCallback = FormatTooltip,
                },
                VisualMap = visualMap,
                Grid = new ChartGridOption
                {
                    Left = "132px",
                    Top = "28px",
                    Right = "28px",
                    Bottom = "34px",
                    ContainLabel = true,
                    OuterBoundsMode = ChartGridOuterBoundsMode.None,
                    BorderWidth = 0f,
                },
                XAxis = new ChartXAxisOption
                {
                    Name = "X",
                    Type = ChartAxisType.Value,
                    Scale = true,
                    SplitLine = CreateSplitLine(),
                },
                YAxis = new ChartYAxisOption
                {
                    Name = "Y",
                    Type = ChartAxisType.Value,
                    Scale = true,
                    SplitLine = CreateSplitLine(),
                },
                Series = new ChartScatterSeriesOption
                {
                    Id = SeriesId,
                    Name = "聚类点",
                    Symbol = "circle",
                    SymbolSize = 15f,
                    Clip = true,
                    DataSource = data,
                    ItemStyle = new()
                    {
                        BorderColor = ChartBrush.Solid(new SKColor(85, 85, 85)),
                        BorderWidth = 1f,
                    },
                    Emphasis = new() { Focus = "self", Scale = 1.25f },
                },
            };
        }

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

        private static ChartTooltipContent? FormatTooltip(
            in ChartTooltipFormatterContext context)
        {
            if (context.Parameters.Count == 0
                || context.Parameters[0].Value.Values.Count < 3)
            {
                return null;
            }

            var values = context.Parameters[0].Value.Values;
            var cluster = values[2].TryGetDouble(out var value)
                ? "集群 " + (int)value
                : "未分组";
            return ChartTooltipContent.FromText(
                cluster
                + "\r\nX：" + Format(values[0], context)
                + "\r\nY：" + Format(values[1], context));
        }

        private static string Format(
            ChartValue value,
            in ChartTooltipFormatterContext context) =>
            value.TryGetDouble(out var number)
                ? number.ToString("0.######", context.Culture)
                : "-";

        /// <summary>使用最远点初始化，并在收敛后按中心坐标重新编号。</summary>
        private static int[] RunKMeans(IReadOnlyList<Sample> points, int clusterCount)
        {
            var centers = InitializeCenters(points, clusterCount);
            var assignments = Enumerable.Repeat(-1, points.Count).ToArray();
            for (var iteration = 0; iteration < 100; iteration++)
            {
                var changed = Assign(points, centers, assignments);
                var next = CalculateCenters(points, centers, assignments, clusterCount);
                var movement = Enumerable.Range(0, clusterCount)
                    .Max(index => DistanceSquared(centers[index], next[index]));
                centers = next;
                if (!changed && movement <= Epsilon)
                {
                    break;
                }
            }

            // 最后一次中心更新后重新分配，确保结果对应最终中心。
            Assign(points, centers, assignments);
            var order = Enumerable.Range(0, clusterCount)
                .OrderBy(index => centers[index].X)
                .ThenBy(index => centers[index].Y)
                .ThenBy(index => index)
                .ToArray();
            var displayIndexes = new int[clusterCount];
            for (var index = 0; index < order.Length; index++)
            {
                displayIndexes[order[index]] = index;
            }

            return assignments.Select(index => displayIndexes[index]).ToArray();
        }

        /// <summary>第一个中心取最左点，后续中心取离已有中心最远的点。</summary>
        private static Center[] InitializeCenters(IReadOnlyList<Sample> points, int count)
        {
            var result = new Center[count];
            var selected = new bool[points.Count];
            var first = Enumerable.Range(0, points.Count)
                .OrderBy(index => points[index].X)
                .ThenBy(index => points[index].Y)
                .ThenBy(index => points[index].Index)
                .First();
            result[0] = new(points[first].X, points[first].Y);
            selected[first] = true;

            for (var centerIndex = 1; centerIndex < count; centerIndex++)
            {
                var best = -1;
                var bestDistance = double.NegativeInfinity;
                for (var pointIndex = 0; pointIndex < points.Count; pointIndex++)
                {
                    if (selected[pointIndex])
                    {
                        continue;
                    }

                    var nearest = Enumerable.Range(0, centerIndex)
                        .Min(index => DistanceSquared(points[pointIndex], result[index]));
                    if (nearest > bestDistance + Epsilon
                        || (Math.Abs(nearest - bestDistance) <= Epsilon
                            && (best < 0 || points[pointIndex].Index < points[best].Index)))
                    {
                        best = pointIndex;
                        bestDistance = nearest;
                    }
                }

                selected[best] = true;
                result[centerIndex] = new(points[best].X, points[best].Y);
            }

            return result;
        }

        /// <summary>把每个点分配给最近中心；同距时保留编号较小的中心。</summary>
        private static bool Assign(
            IReadOnlyList<Sample> points,
            IReadOnlyList<Center> centers,
            int[] assignments)
        {
            var changed = false;
            for (var pointIndex = 0; pointIndex < points.Count; pointIndex++)
            {
                var nearest = 0;
                var nearestDistance = DistanceSquared(points[pointIndex], centers[0]);
                for (var centerIndex = 1; centerIndex < centers.Count; centerIndex++)
                {
                    var distance = DistanceSquared(points[pointIndex], centers[centerIndex]);
                    if (distance < nearestDistance - Epsilon)
                    {
                        nearest = centerIndex;
                        nearestDistance = distance;
                    }
                }

                changed |= assignments[pointIndex] != nearest;
                assignments[pointIndex] = nearest;
            }

            return changed;
        }

        /// <summary>用每组点的平均坐标更新中心。</summary>
        private static Center[] CalculateCenters(
            IReadOnlyList<Sample> points,
            IReadOnlyList<Center> oldCenters,
            IReadOnlyList<int> assignments,
            int count)
        {
            var sumX = new double[count];
            var sumY = new double[count];
            var totals = new int[count];
            for (var index = 0; index < points.Count; index++)
            {
                var cluster = assignments[index];
                sumX[cluster] += points[index].X;
                sumY[cluster] += points[index].Y;
                totals[cluster]++;
            }

            // 本例的最远点初始化不会产生空组；防御情况下保留旧中心。
            var result = oldCenters.ToArray();
            for (var cluster = 0; cluster < count; cluster++)
            {
                if (totals[cluster] > 0)
                {
                    result[cluster] = new(
                        sumX[cluster] / totals[cluster],
                        sumY[cluster] / totals[cluster]);
                }
            }

            return result;
        }

        private static double DistanceSquared(Sample point, Center center)
        {
            var deltaX = point.X - center.X;
            var deltaY = point.Y - center.Y;
            return deltaX * deltaX + deltaY * deltaY;
        }

        private static double DistanceSquared(Center first, Center second)
        {
            var deltaX = first.X - second.X;
            var deltaY = first.Y - second.Y;
            return deltaX * deltaX + deltaY * deltaY;
        }

        private readonly record struct Sample(double X, double Y, int Index);
        private readonly record struct Center(double X, double Y);
    }
}
