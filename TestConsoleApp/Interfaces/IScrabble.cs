using System.Collections.Generic;
using TestConsoleApp.Models;

namespace TestConsoleApp.Interfaces
{
    public interface IScrabble
    {
        List<WordItem> Process(List<string> words, int numOf);
    }
}