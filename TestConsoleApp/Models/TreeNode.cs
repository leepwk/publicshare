using System.Collections.Generic;
using TestConsoleApp.Interfaces;

namespace TestConsoleApp.Models
{
    public class TreeNode : ITreeNode
    {
        public int Value { get; set; }
        public IEnumerable<ITreeNode> Children { get; set; }
    }
}
