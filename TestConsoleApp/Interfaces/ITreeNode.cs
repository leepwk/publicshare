using System.Collections.Generic;
using TestConsoleApp.Models;

namespace TestConsoleApp.Interfaces
{
    public interface ITreeNode
    {
        int Value { get; set; }

        IEnumerable<ITreeNode> Children { get; set; }
    }
}
