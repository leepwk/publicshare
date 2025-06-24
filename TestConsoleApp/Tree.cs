using System.Collections.Generic;
using System.Linq;
using TestConsoleApp.Interfaces;
using TestConsoleApp.Models;

namespace TestConsoleApp
{
    public class Tree : ITree
    {
        public IEnumerable<ITreeNode> Children { get; set; }

        public TreeResult Process()
        {
            var result = new TreeResult();

            if (Children.Any())
            {
                var stack = new Stack<ITreeNode>(Children);

                while (stack.Any())
                {
                    var node = stack.Pop();
                    result.NodesCount++;
                    result.TotalValues += node.Value;

                    if (node.Children != null)
                    {
                        foreach (var nodeChild in node.Children)
                        {
                            stack.Push(nodeChild);
                        }
                    }
                }
            }

            return result;
        }

        public int CountNodes()
        {
            throw new System.NotImplementedException();
        }

        public int TotalValues()
        {
            throw new System.NotImplementedException();
        }

        public TreeResult Process(ITreeNode treeNode)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TreeRecursive : ITree
    {
        public IEnumerable<ITreeNode> Children { get; set; }
        public TreeResult Process()
        {
            var result = new TreeResult
            {
                NodesCount = CountNodes(),
                TotalValues = TotalValues()
            };

            return result;
        }

        public int CountNodes()
        {
            return CountNodesRecursive(Children);
        }

        private int CountNodesRecursive(IEnumerable<ITreeNode> treeNode)
        {
            int result = 0;

            if (treeNode == null)
            {
                return 0;
            }

            foreach (var childNode in treeNode)
            {
                result++;
                result += CountNodesRecursive(childNode.Children);
            }

            return result;
        }

        public int TotalValues()
        {
            return TotalValuesRecursive(Children);
        }

        private int TotalValuesRecursive(IEnumerable<ITreeNode> treeNode)
        {
            int result = 0;

            if (treeNode == null)
            {
                return 0;
            }

            foreach (var childNode in treeNode)
            {
                result += childNode.Value;
                result += TotalValuesRecursive(childNode.Children);
            }

            return result;
        }

    }
}
