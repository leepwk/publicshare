using System.Collections.Generic;
using System.Linq;
using TestConsoleApp.Interfaces;
using TestConsoleApp.Models;

namespace TestConsoleApp
{
    public class Scrabble : IScrabble
    {
        private Dictionary<char, int> points = new Dictionary<char, int> {
            {'a', 1},
            {'b', 3},
            {'c', 3},
            {'d', 2},
            {'e', 1},
            {'f', 4},
            {'g', 2},
            {'h', 4},
            {'i', 1},
            {'j', 8},
            {'k', 5},
            {'l', 1},
            {'m', 3},
            {'n', 1},
            {'o', 1},
            {'p', 3},
            {'q', 10},
            {'r', 1},
            {'s', 1},
            {'t', 1},
            {'u', 1},
            {'v', 4},
            {'w', 4},
            {'x', 8},
            {'y', 4},
            {'z', 10}
        };

        public List<WordItem> Process(List<string> words, int numOf)
        {
            var result = new List<WordItem>();

            if (words.Any())
            {
                result = WordPoints(words.ToList());
                result = result.OrderByDescending(item => item.Points).ThenBy(item => item.Word).Take(numOf).ToList();
            }

            return result;
        }

        private List<WordItem> WordPoints(List<string> words)
        {
            var result = new List<WordItem>();

            foreach (var word in words)
            {
                var score = 0;

                foreach (var c in word.ToLower())
                {
                    score += points[c];
                }

                result.Add(new WordItem { Word = word, Points = score });
            }

            return result;
        }
    }
}
