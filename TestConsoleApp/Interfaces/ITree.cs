using System.Collections.Generic;
using TestConsoleApp.Models;

namespace TestConsoleApp.Interfaces
{
    public interface ITree
    {
        IEnumerable<ITreeNode> Children { get; set; }
        TreeResult Process();

        int CountNodes();
        int TotalValues();
    }
}
