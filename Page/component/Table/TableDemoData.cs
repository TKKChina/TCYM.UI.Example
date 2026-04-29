namespace TCYM.UI.Example.Page.component.Table
{
    internal static class TableDemoData
    {
        internal static List<Dictionary<string, object>> GenerateUserData(int count)
        {
            var names = new[] { "张三", "李四", "王五", "赵六", "钱七", "孙八", "周九", "吴十", "郑一", "冯二" };
            var cities = new[] { "北京", "上海", "广州", "深圳", "杭州", "成都", "武汉", "南京", "西安", "重庆" };
            var tags = new[] { "开发", "设计", "测试", "运维", "产品", "运营" };

            var data = new List<Dictionary<string, object>>(count);
            for (int i = 0; i < count; i++)
            {
                data.Add(new Dictionary<string, object>
                {
                    ["id"] = (i + 1).ToString(),
                    ["name"] = names[i % names.Length],
                    ["age"] = 22 + (i % 20),
                    ["city"] = cities[i % cities.Length],
                    ["tag"] = tags[i % tags.Length],
                    ["score"] = 60 + (i * 7 % 40),
                });
            }

            return data;
        }

        internal static List<Dictionary<string, object>> GenerateTreeData()
        {
            return new List<Dictionary<string, object>>
            {
                new()
                {
                    ["id"] = "1",
                    ["name"] = "技术中心",
                    ["leader"] = "张三",
                    ["count"] = 68,
                    ["children"] = new List<object>
                    {
                        new Dictionary<string, object>
                        {
                            ["id"] = "1-1",
                            ["name"] = "前端平台组",
                            ["leader"] = "李四",
                            ["count"] = 18,
                            ["children"] = new List<object>
                            {
                                new Dictionary<string, object>
                                {
                                    ["id"] = "1-1-1",
                                    ["name"] = "组件库小队",
                                    ["leader"] = "郑一",
                                    ["count"] = 7,
                                },
                                new Dictionary<string, object>
                                {
                                    ["id"] = "1-1-2",
                                    ["name"] = "低代码小队",
                                    ["leader"] = "吴十",
                                    ["count"] = 6,
                                },
                                new Dictionary<string, object>
                                {
                                    ["id"] = "1-1-3",
                                    ["name"] = "可视化小队",
                                    ["leader"] = "周九",
                                    ["count"] = 5,
                                },
                            }
                        },
                        new Dictionary<string, object>
                        {
                            ["id"] = "1-2",
                            ["name"] = "服务端架构组",
                            ["leader"] = "王五",
                            ["count"] = 24,
                            ["children"] = new List<object>
                            {
                                new Dictionary<string, object>
                                {
                                    ["id"] = "1-2-1",
                                    ["name"] = "订单域",
                                    ["leader"] = "冯二",
                                    ["count"] = 8,
                                },
                                new Dictionary<string, object>
                                {
                                    ["id"] = "1-2-2",
                                    ["name"] = "会员域",
                                    ["leader"] = "孙八",
                                    ["count"] = 7,
                                },
                                new Dictionary<string, object>
                                {
                                    ["id"] = "1-2-3",
                                    ["name"] = "基础设施域",
                                    ["leader"] = "赵六",
                                    ["count"] = 9,
                                },
                            }
                        },
                        new Dictionary<string, object>
                        {
                            ["id"] = "1-3",
                            ["name"] = "质量保障组",
                            ["leader"] = "赵六",
                            ["count"] = 14,
                            ["children"] = new List<object>
                            {
                                new Dictionary<string, object>
                                {
                                    ["id"] = "1-3-1",
                                    ["name"] = "自动化测试",
                                    ["leader"] = "钱七",
                                    ["count"] = 8,
                                },
                                new Dictionary<string, object>
                                {
                                    ["id"] = "1-3-2",
                                    ["name"] = "性能测试",
                                    ["leader"] = "孙八",
                                    ["count"] = 6,
                                },
                            }
                        },
                        new Dictionary<string, object>
                        {
                            ["id"] = "1-4",
                            ["name"] = "移动端组",
                            ["leader"] = "周九",
                            ["count"] = 12,
                        },
                    }
                },
                new()
                {
                    ["id"] = "2",
                    ["name"] = "产品设计部",
                    ["leader"] = "钱七",
                    ["count"] = 31,
                    ["children"] = new List<object>
                    {
                        new Dictionary<string, object>
                        {
                            ["id"] = "2-1",
                            ["name"] = "产品规划组",
                            ["leader"] = "郑一",
                            ["count"] = 11,
                        },
                        new Dictionary<string, object>
                        {
                            ["id"] = "2-2",
                            ["name"] = "交互设计组",
                            ["leader"] = "李四",
                            ["count"] = 9,
                        },
                        new Dictionary<string, object>
                        {
                            ["id"] = "2-3",
                            ["name"] = "视觉设计组",
                            ["leader"] = "吴十",
                            ["count"] = 11,
                        },
                    }
                },
                new()
                {
                    ["id"] = "3",
                    ["name"] = "运营增长部",
                    ["leader"] = "孙八",
                    ["count"] = 27,
                    ["children"] = new List<object>
                    {
                        new Dictionary<string, object>
                        {
                            ["id"] = "3-1",
                            ["name"] = "用户运营组",
                            ["leader"] = "王五",
                            ["count"] = 10,
                        },
                        new Dictionary<string, object>
                        {
                            ["id"] = "3-2",
                            ["name"] = "内容运营组",
                            ["leader"] = "冯二",
                            ["count"] = 8,
                        },
                        new Dictionary<string, object>
                        {
                            ["id"] = "3-3",
                            ["name"] = "渠道增长组",
                            ["leader"] = "赵六",
                            ["count"] = 9,
                        },
                    }
                },
                new()
                {
                    ["id"] = "4",
                    ["name"] = "数据智能部",
                    ["leader"] = "冯二",
                    ["count"] = 22,
                    ["children"] = new List<object>
                    {
                        new Dictionary<string, object>
                        {
                            ["id"] = "4-1",
                            ["name"] = "BI 分析组",
                            ["leader"] = "张三",
                            ["count"] = 7,
                        },
                        new Dictionary<string, object>
                        {
                            ["id"] = "4-2",
                            ["name"] = "算法平台组",
                            ["leader"] = "王五",
                            ["count"] = 9,
                            ["children"] = new List<object>
                            {
                                new Dictionary<string, object>
                                {
                                    ["id"] = "4-2-1",
                                    ["name"] = "推荐算法小队",
                                    ["leader"] = "李四",
                                    ["count"] = 4,
                                },
                                new Dictionary<string, object>
                                {
                                    ["id"] = "4-2-2",
                                    ["name"] = "风控算法小队",
                                    ["leader"] = "钱七",
                                    ["count"] = 5,
                                },
                            }
                        },
                        new Dictionary<string, object>
                        {
                            ["id"] = "4-3",
                            ["name"] = "数据治理组",
                            ["leader"] = "周九",
                            ["count"] = 6,
                        },
                    }
                },
            };
        }

        internal static List<Dictionary<string, object>> GenerateEllipsisData(int count)
        {
            var bios = new[]
            {
                "资深全栈工程师，拥有十年以上互联网研发经验，擅长分布式架构设计与性能优化",
                "毕业于清华大学计算机系，热衷开源社区贡献，曾主导多个千万级用户产品的技术架构",
                "前端技术专家，精通 React、Vue 等主流框架，对可视化和图形学有深入研究",
                "后端资深开发，熟练掌握 .NET、Java、Go 等多种语言，专注高并发与微服务",
                "全栈开发者，擅长快速原型搭建与 MVP 验证，对产品设计也有独到见解",
            };
            var experiences = new[]
            {
                "2015-2018 腾讯 高级工程师 / 2018-2022 阿里巴巴 技术专家 / 2022-至今 字节跳动 架构师",
                "2016-2020 百度 前端工程师 / 2020-2024 美团 前端负责人 / 2024-至今 自主创业",
                "2014-2017 华为 软件开发 / 2017-2021 京东 高级开发 / 2021-至今 滴滴 技术总监",
                "2017-2020 网易 后端开发 / 2020-2023 拼多多 架构师 / 2023-至今 蚂蚁集团 P8",
                "2018-2022 小米 全栈开发 / 2022-至今 快手 技术经理",
            };
            var addresses = new[]
            {
                "北京市海淀区中关村大街1号创新科技大厦A座2301室",
                "上海市浦东新区张江高科技园区碧波路888号腾讯大厦15楼",
                "广州市天河区珠江新城花城大道18号南塔38层",
                "深圳市南山区粤海街道科技南十路全志科技大厦6楼",
                "杭州市余杭区文一西路969号阿里巴巴西溪园区3号楼",
            };
            var remarks = new[]
            {
                "该员工综合能力突出，多次获得年度优秀员工，团队协作和项目管理能力很强",
                "技术能力扎实，善于解决复杂问题，在性能优化和系统稳定性方面有丰富实战经验",
                "注重代码质量与工程规范，在团队内推动了多项研发效能提升改进措施",
                "积极参与技术分享和社区活动，发表过多篇高质量技术博客文章",
                "具有良好的跨团队沟通能力，推动了多个跨部门协作项目顺利落地",
            };
            var hobbies = new[]
            {
                "阅读、马拉松、摄影、旅行、咖啡品鉴、围棋",
                "编程、骑行、登山、美食探店、电子音乐制作",
                "游泳、书法、油画、家庭园艺、纪录片观赏",
                "篮球、吉他、天文观测、模型制作、读书会",
                "跑步、烹饪、桌游、志愿者服务、户外露营",
            };
            var departments = new[]
            {
                "技术研发中心-基础架构部-平台工程组",
                "产品设计部-用户体验中心-交互设计组",
                "数据智能事业部-算法平台-推荐引擎组",
                "商业化事业部-广告技术中心-投放引擎组",
                "基础设施部-云原生平台-容器服务组",
            };
            var emails = new[]
            {
                "zhangsan.tech@longdomaincompany.com",
                "lisi.frontend@anotherlongdomain.org",
                "wangwu.backend@enterprise-solutions.cn",
                "zhaoliu.fullstack@globaltech-corp.com",
                "qianqi.devops@cloudplatform-inc.net",
            };

            var baseData = GenerateUserData(count);
            for (int i = 0; i < baseData.Count; i++)
            {
                var row = baseData[i];
                row["bio"] = bios[i % bios.Length];
                row["experience"] = experiences[i % experiences.Length];
                row["address"] = addresses[i % addresses.Length];
                row["remark"] = remarks[i % remarks.Length];
                row["hobby"] = hobbies[i % hobbies.Length];
                row["department"] = departments[i % departments.Length];
                row["email"] = emails[i % emails.Length];
                row["level"] = (i % 3) switch
                {
                    0 => "P6 工程师",
                    1 => "P7 高级工程师",
                    _ => "P8 技术专家"
                };
            }

            return baseData;
        }
    }
}
