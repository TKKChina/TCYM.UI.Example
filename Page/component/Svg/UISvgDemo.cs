using SkiaSharp;
using TCYM.UI.Core;
using TCYM.UI.Elements;
using TCYM.UI.Elements.Svg;
using TCYM.UI.Helpers;

namespace TCYM.UI.Example.Page.component.Svg
{
    internal class UISvgDemo : UIScrollView
    {
        private const string DemoCssPath = "res://TCYM.UI.Example/Page.component.Svg.style.css";
        private const string SvgAnimCssPath = "res://TCYM.UI.Example/Page.component.Svg.svg-animation.css";

        internal UISvgDemo()
        {
            UISystem.LoadStyleFile(DemoCssPath);
            ClassName = new List<string> { "svg-demo-view" };
            Children = new()
            {
                new UILabel
                {
                    Text = "SVG 矢量图形",
                    ClassName = new List<string> { "svg-demo-title" },
                },
                new UILabel
                {
                    Text = "支持内联 SVG 内容和外部 SVG 文件加载，内置动画引擎。",
                    ClassName = new List<string> { "svg-demo-title-sub" },
                },
                new UILabel
                {
                    Text = "UISvg 组件支持 SVG 基本元素（rect、circle、ellipse、line、polyline、polygon、path、text、g）、CSS 样式、@keyframes 动画及 <animate> 内联动画。可通过 SvgContent 传入内联 SVG，或通过 Source 加载外部文件。",
                    ClassName = new List<string> { "svg-demo-desc" },
                },
                new BasicSection(),
                new AnimationSection(),
                new ExternalSection(),
            };
        }

        /// <summary>
        /// 基础用法
        /// </summary>
        private class BasicSection : UIView
        {
            internal BasicSection()
            {
                ClassName = new List<string> { "svg-demo-card" };

                var svgContent = @"<svg width=""220"" height=""160"" viewBox=""0 0 220 160"" xmlns=""http://www.w3.org/2000/svg"">
                <rect x=""10"" y=""10"" width=""60"" height=""40"" rx=""6"" ry=""6"" fill=""#4caf50"" stroke=""#1b5e20"" stroke-width=""2"" />
                <circle cx=""40"" cy=""90"" r=""12"" fill=""#ff5722"" stroke=""#bf360c"" stroke-width=""2"">
                <animate attributeName=""r"" from=""8"" to=""18"" dur=""1.2s"" repeatCount=""indefinite"" />
                </circle>
                <ellipse cx=""110"" cy=""40"" rx=""25"" ry=""15"" fill=""rgba(33,150,243,0.6)"" stroke=""#1565c0"" stroke-width=""2"" />
                <line x1=""10"" y1=""120"" x2=""200"" y2=""120"" stroke=""#9c27b0"" stroke-width=""2"" stroke-dasharray=""6 4"">
                <animate attributeName=""stroke-dashoffset"" from=""0"" to=""20"" dur=""2s"" repeatCount=""indefinite"" />
                </line>
                <polyline points=""10,140 40,110 70,140 100,110 130,140"" fill=""none"" stroke=""#ff9800"" stroke-width=""2"" />
                <polygon points=""150,90 175,130 125,130"" fill=""#ffc107"" stroke=""#ff6f00"" stroke-width=""2"" />
                <path d=""M140 15 C160 0, 200 0, 210 20 C220 40, 180 60, 160 40 Z"" fill=""#e91e63"" fill-opacity=""0.7"" stroke=""#880e4f"" stroke-width=""2"" />
                <g>
                <text x=""110"" y=""150"" font-size=""14"" text-anchor=""middle"" fill=""#424242"">UISvg Demo</text>
                </g>
            </svg>";

                Children = new()
                {
                    new UILabel
                    {
                        Text = "基础用法",
                        ClassName = new List<string> { "svg-card-title", "label-title" }
                    },
                    new UILabel
                    {
                        Text = "通过 SvgContent 传入内联 SVG 字符串。支持 rect、circle、ellipse、line、polyline、polygon、path、text 等基本元素，以及内联 <animate> 动画。",
                        ClassName = new List<string> { "svg-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "svg-showcase" },
                        Children = new()
                        {
                            new UISvg
                            {
                                Width = 220,
                                Height = 160,
                                SvgContent = svgContent,
                                AutoPlay = true,
                                Style = new DefaultUIStyle
                                {
                                    Width = 220,
                                    Height = 160,
                                    Display = "flex",
                                    AlignItems = "center",
                                    JustifyContent = "center",
                                }
                            }
                        }
                    },
                };
            }
        }

        /// <summary>
        /// CSS 动画与关键帧
        /// </summary>
        private class AnimationSection : UIView
        {
            internal AnimationSection()
            {
                ClassName = new List<string> { "svg-demo-card" };

                var svgServer = @"<svg
      width=""74""
      height=""90""
      viewBox=""0 0 74 90""
      fill=""none""
      xmlns=""http://www.w3.org/2000/svg""
    >
      <path
        d=""M40 76.5L72 57V69.8615C72 70.5673 71.628 71.2209 71.0211 71.5812L40 90V76.5Z""
        fill=""#396CAA""
      ></path>
      <path
        d=""M34 75.7077L2 57V69.8615C2 70.5673 2.37203 71.2209 2.97892 71.5812L34 90V75.7077Z""
        fill=""#396DAC""
      ></path>
      <path d=""M34 76.5H40V90H34V76.5Z"" fill=""#396CAA""></path>
      <path
        d=""M3.27905 55.593L35.2806 37.5438C36.3478 36.9419 37.6522 36.9419 38.7194 37.5438L70.721 55.593C71.7294 56.1618 71.7406 57.6102 70.7411 58.1945L39.2712 76.593C37.8682 77.4133 36.1318 77.4133 34.7288 76.593L3.25887 58.1945C2.25937 57.6102 2.27061 56.1618 3.27905 55.593Z""
        fill=""#163C79""
        stroke=""#396CAA""
      ></path>
      <path
        d=""M40 79L72 60V70.4001C72 71.1151 71.6183 71.7758 70.9987 72.1329L40 90V79Z""
        fill=""#173D7A""
      ></path>
      <path d=""M34 79L3 61V71.5751L34 90V79Z"" fill=""#0665B2""></path>
      <path
        class=""strobe_color1""
        d=""M58 72.5L60.5 71V74L58 75.5V72.5Z""
        fill=""#FF715E""
      ></path>
      <path
        class=""strobe_color2""
        d=""M63 69.5L65.5 68V71L63 72.5V69.5Z""
        fill=""rgba(23, 227, 0, 0.706)""
      ></path>
      <path d=""M68 66.5L70.5 65V68L68 69.5V66.5Z"" fill=""#FF715E""></path>
      <path
        d=""M40 58.5L72 39V51.8615C72 52.5673 71.628 53.2209 71.0211 53.5812L40 72V58.5Z""
        fill=""#396CAA""
      ></path>
      <path
        d=""M34 57.7077L2 39V51.8615C2 52.5673 2.37203 53.2209 2.97892 53.5812L34 72V57.7077Z""
        fill=""#396DAC""
      ></path>
      <path d=""M34 58.5H40V72H34V58.5Z"" fill=""#396CAA""></path>
      <path
        d=""M3.27905 37.593L35.2806 19.5438C36.3478 18.9419 37.6522 18.9419 38.7194 19.5438L70.721 37.593C71.7294 38.1618 71.7406 39.6102 70.7411 40.1945L39.2712 58.593C37.8682 59.4133 36.1318 59.4133 34.7288 58.593L3.25887 40.1945C2.25937 39.6102 2.27061 38.1618 3.27905 37.593Z""
        fill=""#163C79""
        stroke=""#396CAA""
      ></path>
      <path
        d=""M40 61L72 42V52.4001C72 53.1151 71.6183 53.7758 70.9987 54.1329L40 72V61Z""
        fill=""#173D7A""
      ></path>
      <path d=""M34 61L3 43V53.5751L34 72V61Z"" fill=""#0665B2""></path>
      <path d=""M58 54.5L60.5 53V56L58 57.5V54.5Z"" fill=""#FF715E""></path>
      <path d=""M63 51.5L65.5 50V53L63 54.5V51.5Z"" fill=""black""></path>
      <path
        class=""strobe_color1""
        d=""M63 51.5L65.5 50V53L63 54.5V51.5Z""
        fill=""#FF715E""
      ></path>
      <path d=""M68 48.5L70.5 47V50L68 51.5V48.5Z"" fill=""#FF715E""></path>
      <path
        d=""M40 40.5L72 21V33.8615C72 34.5673 71.628 35.2209 71.0211 35.5812L40 54V40.5Z""
        fill=""#396CAA""
      ></path>
      <path
        d=""M34 39.7077L2 21V33.8615C2 34.5673 2.37203 35.2209 2.97892 35.5812L34 54V39.7077Z""
        fill=""#396DAC""
      ></path>
      <path d=""M34 40.5H40V54H34V40.5Z"" fill=""#396CAA""></path>
      <path
        d=""M3.27905 19.593L35.2806 1.54381C36.3478 0.941872 37.6522 0.941872 38.7194 1.54381L70.721 19.593C71.7294 20.1618 71.7406 21.6102 70.7411 22.1945L39.2712 40.593C37.8682 41.4133 36.1318 41.4133 34.7288 40.593L3.25887 22.1945C2.25937 21.6102 2.27061 20.1618 3.27905 19.593Z""
        fill=""#124E89""
        stroke=""#396CAA""
      ></path>
      <path
        d=""M40 43L72 24V34.4001C72 35.1151 71.6183 35.7758 70.9987 36.1329L40 54V43Z""
        fill=""#173D7A""
      ></path>
      <path d=""M34 43L3 25V35.5751L34 54V43Z"" fill=""#0665B2""></path>
      <path d=""M68 30.5L70.5 29V32L68 33.5V30.5Z"" fill=""#FF715E""></path>
      <path
        class=""strobe_color3""
        d=""M58 36.5L60.5 35V38L58 39.5V36.5Z""
        fill=""#FF715E""
      ></path>
      <path d=""M63 33.5L65.5 32V35L63 36.5V33.5Z"" fill=""#FF715E""></path>
      <path
        d=""M20.1902 22.0719C18.8101 21.3026 18.8252 19.3119 20.2168 18.5636L36.1054 10.0189C37.2884 9.3827 38.7116 9.3827 39.8946 10.0189L55.7832 18.5636C57.1748 19.3119 57.1899 21.3026 55.8098 22.0719L40.4345 30.6429C38.9211 31.4865 37.0789 31.4865 35.5655 30.6429L20.1902 22.0719Z""
        fill=""#396CAA""
      ></path>
      <path
        d=""M11 52.755C11 51.9801 11.8432 51.4997 12.5098 51.8947L23.5196 58.419C24.1273 58.7792 24.5 59.4332 24.5 60.1396V60.245C24.5 61.0199 23.6568 61.5003 22.9902 61.1053L11.9804 54.581C11.3727 54.2208 11 53.5668 11 52.8604V52.755Z""
        fill=""#396CAA""
      ></path>
      <mask
        id=""mask0_2_176""
        style=""mask-type:alpha""
        maskUnits=""userSpaceOnUse""
        x=""11""
        y=""51""
        width=""14""
        height=""11""
      >
        <path
          d=""M11 52.755C11 51.9801 11.8432 51.4997 12.5098 51.8947L23.5196 58.419C24.1273 58.7792 24.5 59.4332 24.5 60.1396V60.245C24.5 61.0199 23.6568 61.5003 22.9902 61.1053L11.9804 54.581C11.3727 54.2208 11 53.5668 11 52.8604V52.755Z""
          fill=""#396CAA""
        ></path>
      </mask>
      <g mask=""url(#mask0_2_176)"">
        <path
          d=""M11.5 52.7417C11.5 51.9803 12.3349 51.5138 12.9833 51.9128L23.5482 58.4143C24.1397 58.7783 24.5 59.4231 24.5 60.1176V61.5L12.4598 54.4195C11.8651 54.0698 11.5 53.4315 11.5 52.7417V52.7417Z""
          fill=""#163874""
        ></path>
      </g>
      <mask
        id=""mask1_2_176""
        style=""mask-type:alpha""
        maskUnits=""userSpaceOnUse""
        x=""19""
        y=""9""
        width=""38""
        height=""23""
      >
        <path
          d=""M20.1902 22.0719C18.8101 21.3026 18.8252 19.3119 20.2168 18.5636L36.1054 10.0189C37.2884 9.3827 38.7116 9.3827 39.8946 10.0189L55.7832 18.5636C57.1748 19.3119 57.1899 21.3026 55.8098 22.0719L40.4345 30.6429C38.9211 31.4865 37.0789 31.4865 35.5655 30.6429L20.1902 22.0719Z""
          fill=""#396CAA""
        ></path>
      </mask>
      <g mask=""url(#mask1_2_176)"">
        <path
          d=""M18 21.3115L36.167 11.9451C37.3171 11.3521 38.6829 11.3521 39.833 11.9451L58 21.3115L40.3567 30.7405C38.8841 31.5275 37.1159 31.5275 35.6433 30.7405L18 21.3115Z""
          fill=""#173D7A""
        ></path>
      </g>
      <path
        d=""M37.447 21.565L35 19.9799L37.6941 18.66L40.141 20.245L37.447 21.565Z""
        fill=""#FF715E""
      ></path>
      <path
        d=""M48.9738 30.8646L47.0741 29.7745L49.1792 28.684L51.0789 29.7741L48.9738 30.8646Z""
        fill=""#173E7B""
      ></path>
      <path
        d=""M52.0661 29.0093L50.1635 27.9242L52.2657 26.8282L54.1682 27.9133L52.0661 29.0093Z""
        fill=""#173E7B""
      ></path>
      <path
        class=""strobe_led1""
        d=""M55.1521 27.1464L53.2538 26.054L55.3602 24.9661L57.2585 26.0586L55.1521 27.1464Z""
        fill=""#3A6DAB""
      ></path>
    </svg>";

                var svgText = @"<svg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'>
    <defs>
        <filter id='glow'>
            <feGaussianBlur stdDeviation='3' result='blur'/>
            <feMerge>
                <feMergeNode in='blur'/>
                <feMergeNode in='SourceGraphic'/>
            </feMerge>
        </filter>
    </defs>
    <text x='100%' Y='100%' dominant-baseline=""middle"" alignment-baseline=""middle"" text-anchor='middle' font-size='60' font-family='Arial Black' fill='none' stroke='#00c8ff' stroke-width='2' filter='url(#glow)' stroke-dasharray='400'>
            TCYM
        <animate attributeName='stroke-dashoffset' values='400;0;0;400' keyTimes='0;0.4;0.7;1' dur='3s' repeatCount='indefinite'/>
        <animate attributeName='opacity' values='1;.7;1' dur='2s' repeatCount='indefinite'/>
    </text>
</svg>";

                Children = new()
                {
                    new UILabel
                    {
                        Text = "CSS 动画与关键帧",
                        ClassName = new List<string> { "svg-card-title", "label-green" }
                    },
                    new UILabel
                    {
                        Text = "通过 SvgCssPath 加载外部 CSS，支持 @keyframes 关键帧动画和 class 样式绑定。也支持 SVG 内联 <animate> 描边动画、滤镜等效果。",
                        ClassName = new List<string> { "svg-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "svg-showcase" },
                        Children = new()
                        {
                            new UISvg
                            {
                                SvgContent = svgServer,
                                SvgCssPath = SvgAnimCssPath,
                                AutoPlay = true,
                                Style = new DefaultUIStyle
                                {
                                    Width = 200,
                                    Height = 200,
                                    BorderColor = ColorHelper.ParseColor("#e8e8e8"),
                                    BorderWidth = 1,
                                    BorderRadius = 8,
                                }
                            },
                            new UISvg
                            {
                                SvgContent = svgText,
                                AutoPlay = true,
                                Style = new DefaultUIStyle
                                {
                                    Width = 200,
                                    Height = 200,
                                    BorderColor = ColorHelper.ParseColor("#e8e8e8"),
                                    BorderWidth = 1,
                                    BorderRadius = 8,
                                    Display = "flex",
                                    JustifyContent = "center",
                                    AlignItems = "center",
                                }
                            }
                        }
                    },
                };
            }
        }

        /// <summary>
        /// 外部 SVG 资源
        /// </summary>
        private class ExternalSection : UIView
        {
            internal ExternalSection()
            {
                ClassName = new List<string> { "svg-demo-card" };
                Children = new()
                {
                    new UILabel
                    {
                        Text = "外部 SVG 资源加载",
                        ClassName = new List<string> { "svg-card-title", "label-orange" }
                    },
                    new UILabel
                    {
                        Text = "通过 Source 属性加载外部 .svg 文件。支持 res:// 资源路径协议，自动解析并渲染 SVG 内容与动画。",
                        ClassName = new List<string> { "svg-card-desc" }
                    },
                    new UIView
                    {
                        ClassName = new List<string> { "svg-showcase" },
                        Children = new()
                        {
                            new UISvg
                            {
                                Source = "res://TCYM.UI.Example/Assets.Images.vsg.svg",
                                AutoPlay = true,
                                Style = new DefaultUIStyle
                                {
                                    Width = 200,
                                    Height = 200,
                                    BorderColor = ColorHelper.ParseColor("#e8e8e8"),
                                    BorderWidth = 1,
                                    BorderRadius = 8,
                                    Display = "flex",
                                    JustifyContent = "center",
                                    AlignItems = "center",
                                }
                            },
                            new UISvg
                            {
                                Source = "res://TCYM.UI.Example/Assets.Images.work.svg",
                                AutoPlay = true,
                                Style = new DefaultUIStyle
                                {
                                    Width = 200,
                                    Height = 200,
                                    BorderColor = ColorHelper.ParseColor("#e8e8e8"),
                                    BorderWidth = 1,
                                    BorderRadius = 8,
                                    Display = "flex",
                                    JustifyContent = "center",
                                    AlignItems = "center",
                                }
                            }
                        }
                    },
                };
            }
        }
    }
}
