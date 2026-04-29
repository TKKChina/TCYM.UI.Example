using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Pagination
{
    internal class UIPaginationDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Pagination.style.css";
        internal UIPaginationDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "pagination-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Pagination 分页组件",
                    ClassName = new List<string> { "pagination-demo-title" },
                },
                new UILabel
                {
                    Text = "当数据量过多时，使用分页分解数据。",
                    ClassName = new List<string> { "pagination-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "采用分页的形式分隔长列表，每次只加载一个页面。支持基础分页、显示总数、页大小切换、快速跳转、小尺寸及禁用等多种模式。",
                    ClassName = new List<string> { "pagination-demo-desc" },
                },
                new BasicSection(),
                new AdvanceSection(),
                new CompactSection(),
                new DisabledSection(),
            };
        }

        private class BasicSection : UIView
        {
            internal BasicSection()
            {
                ClassName = new List<string> { "pagination-demo-card" };

                var status = new UILabel
                {
                    Text = "当前第 1 页，每页 10 条",
                    ClassName = new List<string> { "pagination-hint-label" }
                };

                var pagination = new UIPagination
                {
                    Total = 126,
                    Current = 1,
                    PageSize = 10,
                    OnChange = (page, pageSize) =>
                    {
                        status.Text = $"当前第 {page} 页，每页 {pageSize} 条";
                        status.RequestLayout();
                        status.RequestRedraw();
                    }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "基础分页",
                        ClassName = new List<string> { "pagination-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "最基本的页码切换场景，包含上一页/下一页和页码按钮。",
                        ClassName = new List<string> { "pagination-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "pagination-showcase" },
                        Children = new() { pagination, status }
                    },
                };
            }
        }

        private class AdvanceSection : UIView
        {
            internal AdvanceSection()
            {
                ClassName = new List<string> { "pagination-demo-card" };

                var status = new UILabel
                {
                    Text = "展示 1-20 条，共 356 条",
                    ClassName = new List<string> { "pagination-hint-label" }
                };

                var pagination = new UIPagination
                {
                    Total = 356,
                    Current = 1,
                    PageSize = 20,
                    ShowSizeChanger = true,
                    ShowQuickJumper = true,
                    ShowTotal = (total, range) => $"第 {range.Start}-{range.End} 条 / 共 {total} 条",
                    PageSizeOptions = new() { 10, 20, 30, 50, 100 },
                    OnChange = (page, pageSize) =>
                    {
                        var start = (page - 1) * pageSize + 1;
                        var end = Math.Min(356, page * pageSize);
                        status.Text = $"展示 {start}-{end} 条，共 356 条";
                        status.RequestLayout();
                        status.RequestRedraw();
                    },
                    OnShowSizeChange = (page, pageSize) =>
                    {
                        var start = (page - 1) * pageSize + 1;
                        var end = Math.Min(356, page * pageSize);
                        status.Text = $"页大小切换为 {pageSize}，当前展示 {start}-{end} 条";
                        status.RequestLayout();
                        status.RequestRedraw();
                    }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "总数 + 页大小 + 快速跳转",
                        ClassName = new List<string> { "pagination-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "集成显示总条数、每页条数切换器和快速跳转输入框，适合数据量较大的表格场景。",
                        ClassName = new List<string> { "pagination-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "pagination-showcase" },
                        Children = new() { pagination, status }
                    },
                };
            }
        }

        private class CompactSection : UIView
        {
            internal CompactSection()
            {
                ClassName = new List<string> { "pagination-demo-card" };

                var status = new UILabel
                {
                    Text = "Small 模式，开启更紧凑页码",
                    ClassName = new List<string> { "pagination-hint-label" }
                };

                var pagination = new UIPagination
                {
                    Total = 180,
                    Current = 8,
                    PageSize = 10,
                    Size = PaginationSize.Small,
                    ShowLessItems = true,
                    ShowQuickJumper = true,
                    OnChange = (page, pageSize) =>
                    {
                        status.Text = $"Small 模式：第 {page} 页，每页 {pageSize} 条";
                        status.RequestLayout();
                        status.RequestRedraw();
                    }
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "小尺寸 / 紧凑页码",
                        ClassName = new List<string> { "pagination-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "使用 Small 尺寸和 ShowLessItems 让分页更加紧凑，适合空间有限的场景。",
                        ClassName = new List<string> { "pagination-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "pagination-showcase" },
                        Children = new() { pagination, status }
                    },
                };
            }
        }

        private class DisabledSection : UIView
        {
            internal DisabledSection()
            {
                ClassName = new List<string> { "pagination-demo-card" };

                var pagination = new UIPagination
                {
                    Total = 85,
                    Current = 3,
                    PageSize = 10,
                    ShowSizeChanger = true,
                    Disabled = true
                };

                var singlePage = new UIPagination
                {
                    Total = 8,
                    Current = 1,
                    PageSize = 10,
                    HideOnSinglePage = true,
                    ShowTotal = (total, range) => $"这里不会显示分页：总数 {total}"
                };

                Children = new()
                {
                    new UILabel
                    {
                        Text = "禁用 / 单页隐藏",
                        ClassName = new List<string> { "pagination-card-title", "label-red" }
                    },
                    new UILabel
                    {
                        Text = "Disabled 禁用所有交互；HideOnSinglePage 在总数不足一页时自动隐藏分页控件。",
                        ClassName = new List<string> { "pagination-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "pagination-showcase" },
                        Children = new()
                        {
                            pagination,
                            new UILabel
                            {
                                Text = "下方启用了 HideOnSinglePage，因为总数不足一页所以不会渲染页码：",
                                ClassName = new List<string> { "pagination-hint-label" }
                            },
                            singlePage
                        }
                    },
                };
            }
        }
    }
}
