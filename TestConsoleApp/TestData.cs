using System;
using NUnit.Framework;
using System.Collections.Generic;
using TestConsoleApp.Interfaces;
using TestConsoleApp.Models;

namespace TestConsoleApp
{
    public static class TestData
    {
        public static TestCaseData TreeData1(bool isRecursive = false)
        {
            return isRecursive
                ? new TestCaseData(
                    new TreeRecursive
                    {
                        Children = new List<ITreeNode>()
                    },
                    0,
                    0
                ).SetName($"RecursiveTreeEmpty")
                : new TestCaseData(
                    new Tree
                    {
                        Children = new List<ITreeNode>()
                    },
                    0,
                    0
                ).SetName($"TreeEmpty");
        }

        public static TestCaseData TreeData2(bool isRecursive = false)
        {
            return isRecursive
                ? new TestCaseData(
                    new TreeRecursive
                    {
                        Children = new List<ITreeNode>
                        {
                            new TreeNode { Value = 1 }
                        }
                    },
                    1,
                    1
                ).SetName($"RecursiveSingleTreeNode")
                : new TestCaseData(
                    new Tree
                    {
                        Children = new List<ITreeNode>
                        {
                            new TreeNode { Value = 1 }
                        }
                    },
                    1,
                    1
                ).SetName($"TreeSingleTreeNode");
        }

        public static TestCaseData TreeData3(bool isRecursive = false)
        {
            return isRecursive
                ? new TestCaseData(
                    new Tree
                    {
                        Children = new List<ITreeNode>
                        {
                            new TreeNode
                            {
                                Value = 1,
                                Children = new List<ITreeNode>
                                {
                                    new TreeNode { Value = 2 },
                                    new TreeNode
                                    {
                                        Value = 3,
                                        Children = new List<ITreeNode>
                                        {
                                            new TreeNode { Value = 4 }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    4,
                    10
                ).SetName($"RecursiveTreeNestedTreeNodes")
                : new TestCaseData(
                    new Tree
                    {
                        Children = new List<ITreeNode>
                        {
                            new TreeNode
                            {
                                Value = 1,
                                Children = new List<ITreeNode>
                                {
                                    new TreeNode { Value = 2 },
                                    new TreeNode
                                    {
                                        Value = 3,
                                        Children = new List<ITreeNode>
                                        {
                                            new TreeNode { Value = 4 }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    4,
                    10
                ).SetName($"TreeNestedTreeNodes");
        }

        public static TestCaseData TreeData4(bool isRecursive = false)
        {
            return isRecursive
                ? new TestCaseData(
                    new Tree
                    {
                        Children = new List<ITreeNode>
                        {
                            new TreeNode
                            {
                                Value = 1,
                                Children = new List<ITreeNode>
                                {
                                    new TreeNode { Value = 2 },
                                    new TreeNode { Value = 3 }
                                }
                            },
                            new TreeNode
                            {
                                Value = 4
                            }
                        }
                    },
                    4,
                    10
                ).SetName($"RecursiveTreeMultipleRootNodes")
                : new TestCaseData(
                    new Tree
                    {
                        Children = new List<ITreeNode>
                        {
                            new TreeNode
                            {
                                Value = 1,
                                Children = new List<ITreeNode>
                                {
                                    new TreeNode { Value = 2 },
                                    new TreeNode { Value = 3 }
                                }
                            },
                            new TreeNode
                            {
                                Value = 4
                            }
                        }
                    },
                    4,
                    10
                ).SetName($"TreeMultipleRootNodes");
        }

        public static TestCaseData ScrabbleData1()
        {
            return new TestCaseData(
                new List<WordItem>
                {
                    new WordItem { Word = "Fumble", Points = 13 },
                    new WordItem { Word = "Oxide", Points = 13 },
                    new WordItem { Word = "Knack", Points = 15 },
                    new WordItem { Word = "Scowl", Points = 10 },
                    new WordItem { Word = "Zipper", Points = 19 },
                    new WordItem { Word = "Delightful", Points = 18 },
                    new WordItem { Word = "Hilarity", Points = 14 },
                    new WordItem { Word = "Existence", Points = 18 },
                    new WordItem { Word = "Surrendered", Points = 13 },
                    new WordItem { Word = "Quiet", Points = 14 }
                },
                10
            ).SetName($"ScrabbleTestData1");
        }

        public static TestCaseData TemperatureData1()
        {
            return new TestCaseData(
                new string[]
                {
                    "london,2025-04-01,19.5",
                    "london,2025-05-01,20.8",
                    "london,2025-04-04,17.7",
                    "paris,2025-04-01,27.2",
                    "milan,2025-04-02,21.3",
                    "london,2025-04-10,16.9",
                    "paris,2025-04-06,22.5",
                    "milan,2025-05-01,21.5"
                },
                4,
                "PARIS, 24.85"
            ).SetName($"TemperatureTestData1");
        }

        public static TestCaseData TemperatureData2()
        {
            return new TestCaseData(
                new string[]
                {
                    "london,2025-04-01,29.5",
                    "london,2025-05-01,23.8",
                    "london,2025-04-04,27.7",
                    "paris,2025-04-01,27.2",
                    "milan,2025-04-02,20.3",
                    "london,2025-04-10,19.9",
                    "paris,2025-04-06,22.5",
                    "milan,2025-05-01,21.5"
                },
                4,
                "LONDON, 25.7"
            ).SetName($"TemperatureTestData2");
        }

        public static TestCaseData OrdersData1()
        {
            return new TestCaseData(
                new List<OrderEvent>
                {
                    new OrderEvent{ OrderId ="o1", TraderId = "traderA", TimeStamp = DateTime.Parse("2023-06-01T10:00:00"), Action = "NEW", Quantity = 100 },
                    new OrderEvent{ OrderId ="o1", TraderId = "traderA", TimeStamp = DateTime.Parse("2023-06-01T10:00:30"), Action = "CANCEL", Quantity = 100 },
                    new OrderEvent{ OrderId ="o2", TraderId = "traderA", TimeStamp = DateTime.Parse("2023-06-01T10:01:00"), Action = "NEW", Quantity = 200 },
                    new OrderEvent{ OrderId ="o2", TraderId = "traderA", TimeStamp = DateTime.Parse("2023-06-01T10:01:20"), Action = "CANCEL", Quantity = 200 },
                    new OrderEvent{ OrderId ="o3", TraderId = "traderA", TimeStamp = DateTime.Parse("2023-06-01T10:02:00"), Action = "NEW", Quantity = 50 },
                    new OrderEvent{ OrderId ="o4", TraderId = "traderA", TimeStamp = DateTime.Parse("2023-06-01T10:02:10"), Action = "NEW", Quantity = 70 },
                    new OrderEvent{ OrderId ="o5", TraderId = "traderA", TimeStamp = DateTime.Parse("2023-06-01T10:02:30"), Action = "NEW", Quantity = 90 },
                    new OrderEvent{ OrderId ="o3", TraderId = "traderA", TimeStamp = DateTime.Parse("2023-06-01T10:02:50"), Action = "CANCEL", Quantity = 50 },
                    new OrderEvent{ OrderId ="o4", TraderId = "traderA", TimeStamp = DateTime.Parse("2023-06-01T10:02:59"), Action = "CANCEL", Quantity = 70 },
                    new OrderEvent{ OrderId ="o5", TraderId = "traderA", TimeStamp = DateTime.Parse("2023-06-01T10:03:00"), Action = "EXECUTE", Quantity = 90 },
                    new OrderEvent{ OrderId ="o1", TraderId = "traderB", TimeStamp = DateTime.Parse("2023-06-01T10:00:00"), Action = "NEW", Quantity = 100 },
                    new OrderEvent{ OrderId ="o1", TraderId = "traderB", TimeStamp = DateTime.Parse("2023-06-01T10:00:30"), Action = "CANCEL", Quantity = 100 },
                    new OrderEvent{ OrderId ="o2", TraderId = "traderB", TimeStamp = DateTime.Parse("2023-06-01T10:01:00"), Action = "NEW", Quantity = 200 },
                    new OrderEvent{ OrderId ="o2", TraderId = "traderB", TimeStamp = DateTime.Parse("2023-06-01T10:01:20"), Action = "CANCEL", Quantity = 200 },
                    new OrderEvent{ OrderId ="o3", TraderId = "traderB", TimeStamp = DateTime.Parse("2023-06-01T10:02:00"), Action = "NEW", Quantity = 50 },
                    new OrderEvent{ OrderId ="o4", TraderId = "traderB", TimeStamp = DateTime.Parse("2023-06-01T10:02:10"), Action = "NEW", Quantity = 70 },
                    new OrderEvent{ OrderId ="o3", TraderId = "traderB", TimeStamp = DateTime.Parse("2023-06-01T10:02:50"), Action = "CANCEL", Quantity = 50 },
                    new OrderEvent{ OrderId ="o4", TraderId = "traderB", TimeStamp = DateTime.Parse("2023-06-01T10:02:59"), Action = "CANCEL", Quantity = 70 },
                    new OrderEvent{ OrderId ="o1", TraderId = "traderC", TimeStamp = DateTime.Parse("2023-06-01T10:00:00"), Action = "NEW", Quantity = 100 },
                    new OrderEvent{ OrderId ="o1", TraderId = "traderC", TimeStamp = DateTime.Parse("2023-06-01T10:02:30"), Action = "CANCEL", Quantity = 100 },
                    new OrderEvent{ OrderId ="o2", TraderId = "traderC", TimeStamp = DateTime.Parse("2023-06-01T10:01:00"), Action = "NEW", Quantity = 200 },
                    new OrderEvent{ OrderId ="o2", TraderId = "traderC", TimeStamp = DateTime.Parse("2023-06-01T10:01:20"), Action = "CANCEL", Quantity = 200 },
                    new OrderEvent{ OrderId ="o3", TraderId = "traderC", TimeStamp = DateTime.Parse("2023-06-01T10:02:00"), Action = "NEW", Quantity = 50 },
                    new OrderEvent{ OrderId ="o4", TraderId = "traderC", TimeStamp = DateTime.Parse("2023-06-01T10:02:10"), Action = "NEW", Quantity = 70 },
                    new OrderEvent{ OrderId ="o5", TraderId = "traderC", TimeStamp = DateTime.Parse("2023-06-01T10:02:30"), Action = "NEW", Quantity = 90 },
                    new OrderEvent{ OrderId ="o3", TraderId = "traderC", TimeStamp = DateTime.Parse("2023-06-01T10:02:50"), Action = "CANCEL", Quantity = 50 },
                    new OrderEvent{ OrderId ="o4", TraderId = "traderC", TimeStamp = DateTime.Parse("2023-06-01T10:02:59"), Action = "CANCEL", Quantity = 70 },
                    new OrderEvent{ OrderId ="o5", TraderId = "traderC", TimeStamp = DateTime.Parse("2023-06-01T10:03:00"), Action = "EXECUTE", Quantity = 90 },
                    new OrderEvent{ OrderId ="o1", TraderId = "traderD", TimeStamp = DateTime.Parse("2023-06-01T10:00:00"), Action = "NEW", Quantity = 100 },
                    new OrderEvent{ OrderId ="o1", TraderId = "traderD", TimeStamp = DateTime.Parse("2023-06-01T10:00:59"), Action = "CANCEL", Quantity = 100 },
                    new OrderEvent{ OrderId ="o2", TraderId = "traderD", TimeStamp = DateTime.Parse("2023-06-01T10:01:00"), Action = "NEW", Quantity = 200 },
                    new OrderEvent{ OrderId ="o2", TraderId = "traderD", TimeStamp = DateTime.Parse("2023-06-01T10:01:20"), Action = "CANCEL", Quantity = 200 },
                    new OrderEvent{ OrderId ="o3", TraderId = "traderD", TimeStamp = DateTime.Parse("2023-06-01T10:02:00"), Action = "NEW", Quantity = 50 },
                    new OrderEvent{ OrderId ="o4", TraderId = "traderD", TimeStamp = DateTime.Parse("2023-06-01T10:02:10"), Action = "NEW", Quantity = 70 },
                    new OrderEvent{ OrderId ="o5", TraderId = "traderD", TimeStamp = DateTime.Parse("2023-06-01T10:02:30"), Action = "NEW", Quantity = 90 },
                    new OrderEvent{ OrderId ="o3", TraderId = "traderD", TimeStamp = DateTime.Parse("2023-06-01T10:02:50"), Action = "CANCEL", Quantity = 50 },
                    new OrderEvent{ OrderId ="o4", TraderId = "traderD", TimeStamp = DateTime.Parse("2023-06-01T10:02:59"), Action = "CANCEL", Quantity = 70 },
                    new OrderEvent{ OrderId ="o5", TraderId = "traderD", TimeStamp = DateTime.Parse("2023-06-01T10:03:00"), Action = "CANCEL", Quantity = 90 }
                },
                new List<string> { "traderA", "traderD" }
            ).SetName($"OrdersData1");
        }

        public static TestCaseData GalaxyData1()
        {
            return new TestCaseData(
                Tuple.Create(
                    new List<List<int>> {
                        new List<int> { 15 },
                        new List<int> { 2, 3 },
                        new List<int> { 40, 5, 12 },
                        new List<int> { 1, 5, 6, 10 }
                    },
                    50
                ),
                true,
                27
                ).SetName($"GalaxyData1");
        }
        public static TestCaseData GalaxyData2()
        {
            return new TestCaseData(
                Tuple.Create(
                    new List<List<int>> {
                        new List<int> { 15 },
                        new List<int> { 2, 3 },
                        new List<int> { 40, 5, 12 },
                        new List<int> { 1, 5, 6, 10 }
                    },
                    25
                ), 
                false,
                27
            ).SetName($"GalaxyData2");
        }
    }
}
