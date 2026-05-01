using System;
using System.Collections.Generic;
using System.Linq;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using SplitterElements = TCYM.UI.Elements.Splitter;

namespace TCYM.UI.Example.Page.component.Splitter
{
    /// <summary>
    /// Splitter 分隔面板示例页面。
    /// 展示水平/垂直布局、可折叠、多面板以及懒更新模式。
    /// </summary>
    public class UISplitterDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Splitter.style.css";

        internal UISplitterDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "splitter-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "Splitter 分隔面板",
                    ClassName = new List<string> { "splitter-demo-title", "label-title" }
                },
                new UILabel
                {
                    Text = "自由切分工作区，支持拖拽、折叠和双击重置。",
                    ClassName = new List<string> { "splitter-demo-title-sub" }
                },
                new UILabel
                {
                    Text = "适合构建 IDE、管理后台、三栏工作台这类需要调整区域尺寸的界面。当前示例覆盖水平、垂直、可折叠和 lazy 模式。",
                    ClassName = new List<string> { "splitter-demo-desc" }
                },
                new BasicSplitterSection(),
                new VerticalSplitterSection(),
                new CollapsibleSplitterSection(),
                new LazySplitterSection(),
            };
        }

        private static UILabel CreateStatusLabel(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "splitter-status" }
            };
        }

        private static SplitterElements.UISplitterPanel CreatePanel(
            string eyebrow,
            string title,
            string body,
            string note,
            params string[] classNames)
        {
            var names = new List<string> { "splitter-demo-panel" };
            if (classNames != null && classNames.Length > 0)
            {
                names.AddRange(classNames);
            }

            return new SplitterElements.UISplitterPanel
            {
                ClassName = names,
                Children = new()
                {
                    new UILabel
                    {
                        Text = eyebrow,
                        ClassName = new List<string> { "splitter-panel-eyebrow" }
                    },
                    new UILabel
                    {
                        Text = title,
                        ClassName = new List<string> { "splitter-panel-title" }
                    },
                    new UILabel
                    {
                        Text = body,
                        ClassName = new List<string> { "splitter-panel-body" }
                    },
                    new UILabel
                    {
                        Text = note,
                        ClassName = new List<string> { "splitter-panel-note" }
                    }
                }
            };
        }

        private static SplitterElements.UISplitterPanel ConfigurePanel(
            SplitterElements.UISplitterPanel panel,
            Action<SplitterElements.UISplitterPanel> configure)
        {
            configure(panel);
            return panel;
        }

        private static string FormatSizes(float[] sizes)
        {
            return string.Join(" / ", sizes.Select(size => $"{Math.Round(size)}px"));
        }

        private static string FormatCollapsedStates(bool[] collapsed)
        {
            return string.Join("、", collapsed.Select((state, index) => $"P{index + 1}:{(state ? "收起" : "展开")}"));
        }

        private sealed class BasicSplitterSection : UIView
        {
            internal BasicSplitterSection()
            {
                ClassName = new List<string> { "splitter-demo-card" };
                var status = CreateStatusLabel("实时尺寸：拖拽中间分隔条查看两个面板的宽度变化。");

                Children = new()
                {
                    new UILabel
                    {
                        Text = "基础水平分隔",
                        ClassName = new List<string> { "splitter-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "默认从左到右排列，给左侧面板设置 defaultSize、min、max，右侧自动填充剩余空间。",
                        ClassName = new List<string> { "splitter-card-desc" }
                    },
                    new SplitterElements.UISplitter
                    {
                        ClassName = new List<string> { "splitter-demo-stage" },
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            Height = 230,
                        },
                        SplitTriggerSize = 12,
                        SplitBarDraggableSize = 78,
                        Children = new()
                        {
                            ConfigurePanel(CreatePanel(
                                "Primary Pane",
                                "资源区",
                                "适合放导航树、文件列表、筛选器等可频繁调整宽度的内容。",
                                "defaultSize: 38%   min: 200px   max: 68%",
                                "splitter-panel-sky"), panel =>
                            {
                                panel.DefaultSize = "38%";
                                panel.Min = 200;
                                panel.Max = "68%";
                            }),
                            ConfigurePanel(CreatePanel(
                                "Workspace",
                                "主工作区",
                                "中间区域没有显式 size，自动吃掉剩余空间，适合表格、编辑器或预览面板。",
                                "当左侧拖拽变窄/变宽时，这里会实时补偿。",
                                "splitter-panel-indigo"), panel =>
                            {
                                panel.Min = 280;
                            })
                        },
                        OnResize = sizes => status.Text = $"实时尺寸：{FormatSizes(sizes)}",
                    },
                    status,
                };
            }
        }

        private sealed class VerticalSplitterSection : UIView
        {
            internal VerticalSplitterSection()
            {
                ClassName = new List<string> { "splitter-demo-card" };
                var status = CreateStatusLabel("实时高度：上下拖拽改变预览区和日志区的高度占比。");

                Children = new()
                {
                    new UILabel
                    {
                        Text = "垂直方向",
                        ClassName = new List<string> { "splitter-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "orientation = Vertical 时，上下排列面板，常用于预览区 + 输出区、主列表 + 详情区这种结构。",
                        ClassName = new List<string> { "splitter-card-desc" }
                    },
                    new SplitterElements.UISplitter
                    {
                        Orientation = SplitterElements.SplitterOrientation.Vertical,
                        ClassName = new List<string> { "splitter-demo-stage" },
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            Height = 500,
                        },
                        SplitTriggerSize = 12,
                        SplitBarDraggableSize = 90,
                        Children = new()
                        {
                            ConfigurePanel(CreatePanel(
                                "Preview",
                                "上方预览区",
                                "这里可以放画布、视频、图像或运行结果，适合获得更大可视区域。",
                                "defaultSize: 62%   min: 120px",
                                "splitter-panel-mint"), panel =>
                            {
                                panel.DefaultSize = "62%";
                                panel.Min = 120;
                            }),
                            ConfigurePanel(CreatePanel(
                                "Logs",
                                "下方输出区",
                                "较小但需要固定可用空间的区域，比如日志、终端、属性面板。",
                                "min: 96px",
                                "splitter-panel-rose"), panel =>
                            {
                                panel.Min = 96;
                            })
                        },
                        OnResize = sizes => status.Text = $"实时高度：{FormatSizes(sizes)}",
                    },
                    status,
                };
            }
        }

        private sealed class CollapsibleSplitterSection : UIView
        {
            internal CollapsibleSplitterSection()
            {
                ClassName = new List<string> { "splitter-demo-card" };
                var status = CreateStatusLabel("点击拖拽条两侧的箭头可收起/展开侧边栏，适合三栏工作台。 ");

                Children = new()
                {
                    new UILabel
                    {
                        Text = "多面板与可折叠",
                        ClassName = new List<string> { "splitter-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "左右侧边栏开启 collapsible，并把 showCollapsibleIcon 设为 Visible，形成典型的导航区 / 主内容 / 检查器布局。",
                        ClassName = new List<string> { "splitter-card-desc" }
                    },
                    new SplitterElements.UISplitter
                    {
                        ClassName = new List<string> { "splitter-demo-stage" },
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            Height = 250,
                        },
                        SplitTriggerSize = 12,
                        SplitBarDraggableSize = 86,
                        Children = new()
                        {
                            ConfigurePanel(CreatePanel(
                                "Left Dock",
                                "导航侧栏",
                                "收起后只保留主工作区，让布局更像桌面 IDE。",
                                "defaultSize: 22%   min: 140px   collapsible",
                                "splitter-panel-sand"), panel =>
                            {
                                panel.DefaultSize = "22%";
                                panel.Min = 140;
                                panel.Collapsible = true;
                                panel.CollapsibleOptions = new SplitterElements.UISplitterPanelCollapsibleOptions
                                {
                                    ShowCollapsibleIcon = SplitterElements.SplitterCollapsibleIconVisibility.Visible,
                                    Start = true,
                                    End = false,
                                };
                            }),
                            ConfigurePanel(CreatePanel(
                                "Canvas",
                                "主内容区",
                                "中间面板不设置固定尺寸，让左右两侧的收起和拖拽都集中影响它。",
                                "min: 280px",
                                "splitter-panel-ink"), panel =>
                            {
                                panel.Min = 280;
                            }),
                            ConfigurePanel(CreatePanel(
                                "Inspector",
                                "属性检查器",
                                "右侧适合表单、属性列表、运行信息等辅助内容。",
                                "defaultSize: 24%   min: 160px   collapsible",
                                "splitter-panel-lilac"), panel =>
                            {
                                panel.DefaultSize = "24%";
                                panel.Min = 160;
                                panel.Collapsible = true;
                                panel.CollapsibleOptions = new SplitterElements.UISplitterPanelCollapsibleOptions
                                {
                                    ShowCollapsibleIcon = SplitterElements.SplitterCollapsibleIconVisibility.Visible,
                                    Start = false,
                                    End = true,
                                };
                            })
                        },
                        OnResize = sizes => status.Text = $"当前尺寸：{FormatSizes(sizes)}",
                        OnCollapse = (collapsed, sizes) => status.Text = $"折叠状态：{FormatCollapsedStates(collapsed)} | 尺寸：{FormatSizes(sizes)}",
                    },
                    status,
                };
            }
        }

        private sealed class LazySplitterSection : UIView
        {
            internal LazySplitterSection()
            {
                ClassName = new List<string> { "splitter-demo-card" };
                var status = CreateStatusLabel("lazy 模式：拖拽时仅显示预览线，松手后一次性提交尺寸。双击拖拽条可重置默认尺寸。");

                Children = new()
                {
                    new UILabel
                    {
                        Text = "Lazy 模式与双击重置",
                        ClassName = new List<string> { "splitter-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "在复杂内容区或昂贵布局场景中，打开 lazy 可以避免拖拽过程持续重排；双击拖拽条则回到 defaultSize。",
                        ClassName = new List<string> { "splitter-card-desc" }
                    },
                    new SplitterElements.UISplitter
                    {
                        Lazy = true,
                        ClassName = new List<string> { "splitter-demo-stage" },
                        Style = new DefaultUIStyle
                        {
                            Width = "100%",
                            Height = 220,
                        },
                        SplitTriggerSize = 12,
                        SplitBarDraggableSize = 92,
                        Children = new()
                        {
                            ConfigurePanel(CreatePanel(
                                "Layers",
                                "图层区",
                                "拖拽过程中不会实时改写面板大小，只画一条预览分隔线。",
                                "defaultSize: 260px   max: 420px",
                                "splitter-panel-cyan"), panel =>
                            {
                                panel.DefaultSize = 260;
                                panel.Min = 180;
                                panel.Max = 420;
                            }),
                            ConfigurePanel(CreatePanel(
                                "Timeline",
                                "时间轴",
                                "松手时统一提交最终尺寸；双击拖拽条会恢复到默认布局。",
                                "min: 260px",
                                "splitter-panel-amber"), panel =>
                            {
                                panel.Min = 260;
                            })
                        },
                        OnResizeStart = sizes => status.Text = $"开始拖拽：{FormatSizes(sizes)}",
                        OnResizeEnd = sizes => status.Text = $"提交尺寸：{FormatSizes(sizes)}",
                        OnDraggerDoubleClick = index => status.Text = $"拖拽条 {index + 1} 已重置到默认尺寸",
                    },
                    status,
                };
            }
        }
    }
}