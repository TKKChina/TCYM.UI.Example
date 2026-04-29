using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Storage;

namespace TCYM.UI.Example.Page.component.FilePicker
{
    internal class UIFilePickerDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.FilePicker.style.css";

        private readonly UILabel _fileResultLabel;
        private readonly UILabel _folderResultLabel;

        internal UIFilePickerDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);

            _fileResultLabel = CreateResultLabel("尚未调用 OpenFilePickerAsync。", "file-picker-result");
            _folderResultLabel = CreateResultLabel("尚未调用 OpenFolderPickerAsync。", "file-picker-result file-picker-result-folder");

            ClassName = new List<string> { "file-picker-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "文件选择器",
                    ClassName = new List<string> { "file-picker-demo-title" },
                },
                new UILabel
                {
                    Text = "演示 UISystem.OpenFilePickerAsync 和 UISystem.OpenFolderPickerAsync 的原生调用方式。",
                    ClassName = new List<string> { "file-picker-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "这个页面覆盖单选、多选、文件类型过滤和起始目录建议。结果会直接回显到页面，方便确认返回值结构。",
                    ClassName = new List<string> { "file-picker-demo-desc" },
                },
                CreateFilePickerSection(),
                CreateFolderPickerSection(),
            };
        }

        private UIView CreateFilePickerSection()
        {
            return CreateSectionCard(
                "OpenFilePickerAsync",
                "适合上传文件、导入资源、选择日志或配置文件。示例同时演示单选、多选和文件过滤器。",
                new UIView
                {
                    ClassName = new List<string> { "file-picker-showcase" },
                    Children = new()
                    {
                        CreateButtonRow(
                            CreateActionButton("单选文件", OpenSingleFileAsync, "file-picker-btn-primary"),
                            CreateActionButton("多选文件", OpenMultipleFilesAsync, "file-picker-btn-secondary"),
                            CreateActionButton("Patterns 过滤", OpenFilteredFilesAsync, "file-picker-btn-soft")
                        ),
                        _fileResultLabel
                    }
                },
                CreateHintLabel("单选和多选都使用 SuggestedStartLocation；过滤示例显式设置 FilePickerFileType.Patterns。")
            );
        }

        private UIView CreateFolderPickerSection()
        {
            return CreateSectionCard(
                "OpenFolderPickerAsync",
                "适合导出目录、扫描素材目录、选择项目根目录或批量处理目标路径。",
                new UIView
                {
                    ClassName = new List<string> { "file-picker-showcase" },
                    Children = new()
                    {
                        CreateButtonRow(
                            CreateActionButton("选择单个文件夹", OpenSingleFolderAsync, "file-picker-btn-primary"),
                            CreateActionButton("选择多个文件夹", OpenMultipleFoldersAsync, "file-picker-btn-secondary")
                        ),
                        _folderResultLabel
                    }
                },
                CreateHintLabel("文件夹选择器只返回目录路径，不包含文件类型过滤；常用参数是 Title、AllowMultiple 和 SuggestedStartLocation。")
            );
        }

        private async void OpenSingleFileAsync()
        {
            SetLabelText(_fileResultLabel, "正在打开原生文件选择器（单选）...");

            IReadOnlyList<string> files = await UISystem.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择一个文件",
                AllowMultiple = false,
                SuggestedStartLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            });

            SetLabelText(_fileResultLabel, BuildFileResultText("单选文件", files));
        }

        private async void OpenMultipleFilesAsync()
        {
            SetLabelText(_fileResultLabel, "正在打开原生文件选择器（多选）...");

            IReadOnlyList<string> files = await UISystem.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择多个文件",
                AllowMultiple = true,
                SuggestedStartLocation = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            });

            SetLabelText(_fileResultLabel, BuildFileResultText("多选文件", files));
        }

        private async void OpenFilteredFilesAsync()
        {
            SetLabelText(_fileResultLabel, "正在打开带 Patterns 过滤器的文件选择器...");

            IReadOnlyList<string> files = await UISystem.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择图片或文本文件",
                AllowMultiple = true,
                SuggestedStartLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new FilePickerFileType
                    {
                        Name = "图片文件",
                        Patterns = new List<string> { "*.png", "*.jpg", "*.jpeg", "*.webp" }
                    },
                    new FilePickerFileType
                    {
                        Name = "文本文件",
                        Patterns = new List<string> { "*.txt", "*.md", "*.json" }
                    }
                }
            });

            SetLabelText(_fileResultLabel, BuildFileResultText("Patterns 过滤", files));
        }

        private async void OpenSingleFolderAsync()
        {
            SetLabelText(_folderResultLabel, "正在打开原生文件夹选择器（单      选）...");

            IReadOnlyList<string> folders = await UISystem.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "选择一个文件夹",
                AllowMultiple = false,
                SuggestedStartLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            });

            SetLabelText(_folderResultLabel, BuildFolderResultText("单个文件夹", folders));
        }

        private async void OpenMultipleFoldersAsync()
        {
            SetLabelText(_folderResultLabel, "正在打开原生文件夹选择器（多选）...");

            IReadOnlyList<string> folders = await UISystem.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "选择多个文件夹",
                AllowMultiple = true,
                SuggestedStartLocation = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            });

            SetLabelText(_folderResultLabel, BuildFolderResultText("多个文件夹", folders));
        }

        private static UIView CreateSectionCard(string title, string description, params UIElement[] children)
        {
            var sectionChildren = new List<UIElement>
            {
                new UILabel
                {
                    Text = title,
                    ClassName = new List<string> { "file-picker-card-title" },
                },
                new UILabel
                {
                    Text = description,
                    ClassName = new List<string> { "file-picker-card-desc" },
                }
            };

            sectionChildren.AddRange(children);

            return new UIView
            {
                ClassName = new List<string> { "file-picker-demo-card" },
                Children = sectionChildren,
            };
        }

        private static UIView CreateButtonRow(params UIElement[] buttons)
        {
            return new UIView
            {
                ClassName = new List<string> { "file-picker-button-row" },
                Children = buttons.ToList(),
            };
        }

        private static UIButton CreateActionButton(string text, Action onClick, params string[] extraClasses)
        {
            var classNames = new List<string> { "file-picker-action-btn" };
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

        private static UILabel CreateHintLabel(string text)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string> { "file-picker-demo-hint" },
            };
        }

        private static UILabel CreateResultLabel(string text, string classNames)
        {
            return new UILabel
            {
                Text = text,
                ClassName = new List<string>(classNames.Split(' ', StringSplitOptions.RemoveEmptyEntries)),
            };
        }

        private static string BuildFileResultText(string operation, IReadOnlyList<string> files)
        {
            if (files == null || files.Count == 0)
            {
                return $"{operation}：用户取消选择，返回空列表。";
            }

            List<string> lines = new()
            {
                $"{operation}：共返回 {files.Count} 个文件"
            };

            foreach (string file in files.Take(6))
            {
                lines.Add($"- {Path.GetFileName(file)}");
                lines.Add($"  {file}");
            }

            if (files.Count > 6)
            {
                lines.Add($"- 其余 {files.Count - 6} 个文件已省略");
            }

            return string.Join("\n", lines);
        }

        private static string BuildFolderResultText(string operation, IReadOnlyList<string> folders)
        {
            if (folders == null || folders.Count == 0)
            {
                return $"{operation}：用户取消选择，返回空列表。";
            }

            List<string> lines = new()
            {
                $"{operation}：共返回 {folders.Count} 个目录"
            };

            foreach (string folder in folders.Take(6))
            {
                lines.Add($"- {folder}");
            }

            if (folders.Count > 6)
            {
                lines.Add($"- 其余 {folders.Count - 6} 个目录已省略");
            }

            return string.Join("\n", lines);
        }

        private static void SetLabelText(UILabel label, string text)
        {
            label.Text = text;
            label.RequestLayout();
            label.RequestRedraw();
        }
    }
}