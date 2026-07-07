# TCYM.UI.Example

TCYM.UI.Example 是一个可独立构建和运行的 TCYM.UI 示例工程，用于对外展示基础组件、布局、样式、SDL3 输入事件与跨平台窗口能力。

这个仓库不依赖上级源码目录中的 TCYM.UI 和 TCYM.UI.Generator 项目，而是直接引用 `Libs` 目录中的已编译二进制文件，因此可以单独复制、单独构建、单独发布。公开展示仓库不包含 TCYM.UI.Pro 工程源码和 Pro 运行依赖，Player、MvGigECamera、YOLO 等 Pro 示例默认从编译项中排除。

## 文档地址

[https://tcym.top:8035/tcym/UI/Doc/index.html](https://tcym.top:8035/tcym/UI/Doc/index.html)

## 演示视频

[![TCYM.UI.Example 演示视频](Assets/Images/demo-video-preview.gif)](demovideo.mp4)

点击上方预览图打开演示视频，或下载 [demovideo.mp4](demovideo.mp4) 查看。

## 环境要求

- .NET 8 SDK
- Windows 或 Linux 桌面环境
- 可用的 SDL3 运行环境；仓库已随 `Libs/Win/SDL3.dll` 和 `Libs/Linux/libSDL3.so.0` 提供常用运行库

## 演示内容

- 基础组件：按钮、图标、文本、图片、菜单、分页、标签页、表格、树、表单控件等
- 布局与反馈：分隔面板、虚拟滚动、消息、对话框、水印等
- SDL3 能力：触控笔事件、文件拖放位置更新、手柄/摇杆输入可视化
- 新增组件示例：Carousel 走马灯

## 目录说明

- `Assets`：示例所需图片、字体等资源
- `Page`：示例页面与组件演示代码
- `Libs`：独立运行所需的 TCYM.UI、TCYM.UI.Generator、SDL3 等二进制依赖
- `Program.cs`：示例程序入口

## 构建与运行

在仓库根目录执行：

```bash
dotnet restore
dotnet build TCYM.UI.Example.csproj
dotnet run --project TCYM.UI.Example.csproj
```

## 许可证说明

- 示例源码使用 MIT License，见根目录 LICENSE。
- `Libs` 目录中的 TCYM.UI 相关二进制文件请按随附许可证使用，见 `Libs/LICENSE-TCYM.UI.txt`。
