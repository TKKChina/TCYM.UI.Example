using SkiaSharp;
using TCYM.UI.Chart.Components.Tooltip;
using TCYM.UI.Chart.Data;
using TCYM.UI.Chart.Model;
using TCYM.UI.Chart.Rendering;

namespace Page.Charts.Tree;

/// <summary>公司、五个部门和十个小组的组织架构，保留负责人和职位。</summary>
internal sealed class CompanyTreeChartDemo : TreeChartDemoPage
{
    private const string SeriesId = "company-organization";
    private const float SideMargin = 64;
    private readonly ChartTreeNode _root;

    // Value 携带不可变业务信息，提交后不会被外部修改影响。
    private sealed record OrganizationInfo(string Owner, string? Position) : IChartImmutableDataValue;

    internal CompanyTreeChartDemo() : base(
        "公司架构图",
        "星辰科技 → 五个职能部门 → 十个业务小组。",
        "卡片显示部门和负责人，根节点显示总经理职位；悬停查看完整信息。点击公司或部门折叠/展开，卡片宽度随容器调整，窄窗口中的小组名称自动分行。",
        CreateOption(out var root))
    {
        _root = root;
    }

    private static ChartOption CreateOption(out ChartTreeNode root)
    {
        // 1. 按用户提供的 JSON 原样保留 id、名称、负责人和层级关系。
        root = Node("company", "星辰科技", "王明",
        [
            Node("tech", "技术研发部", "赵宁",
            [
                Node("frontend", "前端开发组", "张晨"),
                Node("backend", "后端开发组", "林远"),
            ]),
            Node("product", "产品设计部", "李悦",
            [
                Node("planning", "产品规划组", "许欣"),
                Node("experience", "用户体验组", "苏晴"),
            ]),
            Node("sales", "市场销售部", "陈浩",
            [
                Node("marketing", "市场推广组", "杨帆"),
                Node("accounts", "客户销售组", "吴凯"),
            ]),
            Node("operations", "运营服务部", "刘敏",
            [
                Node("business", "业务运营组", "郑宇"),
                Node("support", "客户服务组", "何佳"),
            ]),
            Node("admin", "财务行政部", "周静",
            [
                Node("finance", "财务管理组", "孙琳"),
                Node("hr", "人事行政组", "徐婷"),
            ]),
        ], position: "总经理");

        // 2. 完整 Options 保留在本类，不把 Tree 配置放进公共页面。
        return new ChartOption
        {
            Background = ChartBrush.Solid(SKColors.White),
            Animation = new() { Enabled = true },
            Tooltip = new()
            {
                Trigger = ChartTooltipTrigger.Item,
                Confine = true,
                FormatterCallback = static (in ChartTooltipFormatterContext context) =>
                {
                    if (context.Parameters.Count == 0) return ChartTooltipContent.FromText("");
                    var item = context.Parameters[0];
                    if (item.Value.Primary.ToObject() is not OrganizationInfo info)
                        return ChartTooltipContent.FromText(item.Name);
                    return ChartTooltipContent.FromText(
                        $"{item.Name}\r\n负责人：{info.Owner}" +
                        (string.IsNullOrEmpty(info.Position) ? "" : $"\r\n职位：{info.Position}"));
                },
            },
            Series = new ChartTreeSeriesOption
            {
                Id = SeriesId, Name = "星辰科技组织架构",
                DataSource = new ChartTreeData().Add(root),
                Layout = ChartTreeLayout.Orthogonal,
                Orient = ChartTreeOrient.TB,
                EdgeShape = ChartTreeEdgeShape.Polyline,
                EdgeForkPosition = ChartLength.Percent(50),
                InitialTreeDepth = -1,
                ExpandAndCollapse = true,
                Roam = true,
                Left = ChartLength.Pixels(SideMargin), Right = ChartLength.Pixels(SideMargin),
                Top = ChartLength.Pixels(80), Bottom = ChartLength.Pixels(90),
                Symbol = "roundRect",
                SymbolSize = new ChartSymbolSize(140, 68),
                LineStyle = new() { Color = ChartBrush.Solid(new SKColor(138, 166, 200)), Width = 2 },
                Label = new() { Show = true, Position = "inside", Align = ChartTextAlign.Center },
                Emphasis = new()
                {
                    ItemStyle = new() { BorderColor = ChartBrush.Solid(new SKColor(255, 183, 77)), BorderWidth = 3 },
                },
            },
        };
    }

    private static ChartTreeNode Node(string id, string name, string owner,
        ChartTreeNode[]? children = null, string? position = null)
    {
        var isRoot = id == "company";
        var isTeam = children is null || children.Length == 0;
        var textColor = isTeam ? new SKColor(35, 63, 98) : SKColors.White;
        var node = new ChartTreeNode
        {
            Id = id, Name = name,
            Value = ChartValue.FromObject(new OrganizationInfo(owner, position)),
            SymbolSize = isRoot ? new ChartSymbolSize(188, 84)
                : isTeam ? new ChartSymbolSize(116, 64) : new ChartSymbolSize(144, 68),
            ItemStyle = new()
            {
                Color = ChartBrush.Solid(isRoot ? new SKColor(38, 70, 116)
                    : isTeam ? new SKColor(240, 246, 253) : new SKColor(40, 111, 199)),
                BorderColor = ChartBrush.Solid(isTeam ? new SKColor(166, 192, 224) : SKColors.White),
                BorderWidth = 1.5f,
                ShadowBlur = 5,
                ShadowColor = ChartBrush.Solid(new SKColor(67, 106, 155, 35)),
            },
            Label = new()
            {
                Show = true, Position = "inside", Align = ChartTextAlign.Center,
                Formatter = $"{{title|{name}}}\n{{owner|{owner}{(position is null ? "" : " · " + position)}}}",
                Rich = new()
                {
                    ["title"] = new ChartTextStyle
                    {
                        Color = ChartBrush.Solid(textColor), FontSize = isRoot ? 20 : 14,
                        FontWeight = 600, LineHeight = isRoot ? 28 : 22, Align = ChartTextAlign.Center,
                    },
                    ["owner"] = new ChartTextStyle
                    {
                        Color = ChartBrush.Solid(textColor), FontSize = isRoot ? 14 : 12,
                        LineHeight = 20, Align = ChartTextAlign.Center,
                    },
                },
            },
        };
        foreach (var child in children ?? []) node.Children.Add(child);
        return node;
    }

    protected override void OnChartSizeChanged(SKSize size)
    {
        // 十个小组占十个叶槽。限制卡片宽度，避免窄窗口内相邻卡片重叠。
        var slot = MathF.Max(1, (size.Width - SideMargin * 2) / 9);
        var teamWidth = Math.Clamp(slot - 8, 44, 116);
        var departmentWidth = Math.Clamp(slot * 2 - 12, 72, 144);
        var compact = teamWidth < 96;
        foreach (var department in _root.Children)
        {
            department.SymbolSize = new ChartSymbolSize(departmentWidth, 68);
            department.Label.Rich["title"].FontSize = Math.Clamp((departmentWidth - 12) / 5, 11, 14);
            foreach (var team in department.Children)
            {
                team.SymbolSize = new ChartSymbolSize(teamWidth, compact ? 80 : 64);
                var info = (OrganizationInfo)team.Value.ToObject()!;
                var name = team.Name!;
                // 仅改变显示换行；Tooltip 和业务 Name 始终保留完整名称。
                var title = compact ? $"{{title|{name[..2]}}}\n{{title|{name[2..]}}}" : $"{{title|{name}}}";
                team.Label.Formatter = title + $"\n{{owner|{info.Owner}}}";
                team.Label.Rich["title"].FontSize = compact ? Math.Clamp((teamWidth - 12) / 3, 10, 14) : 14;
            }
        }
        Chart.SetOption(new ChartOption
        {
            Series = new ChartTreeSeriesOption { Id = SeriesId, DataSource = new ChartTreeData().Add(_root) },
        }, new ChartSetOptionOptions { LazyUpdate = true });
    }
}
