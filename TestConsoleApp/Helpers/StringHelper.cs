using System.Collections.Generic;

namespace TestConsoleApp.Helpers
{
    public static class StringHelper
    {
        public static Dictionary<char, int> RomanDictionary = new Dictionary<char, int>
        {
            { 'I', 1 },
            { 'V', 5 },
            { 'X', 10 },
            { 'L', 50 },
            { 'C', 100 },
            { 'D', 500 },
            { 'M', 1000 },
            { ' ', 0 }
        };

        private static HashSet<string> SubtractivePairs = new HashSet<string>
        {
            "IV", "IX", "XL", "XC", "CD", "CM"
        };
    
        public static bool IsValidRoman(this string word)
        {
            if (string.IsNullOrEmpty(word)) return false;

            word = word.ToUpper();
            int repeatCount = 1;
            char prevChar = '\0';

            for (int i = 0; i < word.Length; i++)
            {
                char curr = word[i];

                if (!RomanDictionary.ContainsKey(curr)) return false;

                // Check repeat limits
                if (i > 0 && curr == prevChar)
                {
                    repeatCount++;

                    // V, L, D can't repeat at all
                    if (curr == 'V' || curr == 'L' || curr == 'D')
                        return false;

                    // I, X, C, M can't repeat more than 3 times
                    if (repeatCount > 3)
                        return false;
                }
                else
                {
                    repeatCount = 1;
                }

                // Check subtractive notation
                if (i > 0 && RomanDictionary[curr] > RomanDictionary[prevChar])
                {
                    string pair = $"{prevChar}{curr}";

                    // Only allow specific subtractive pairs
                    if (!SubtractivePairs.Contains(pair))
                        return false;

                    // Can't subtract twice in a row like IIV
                    if (i >= 2 && word[i - 2] == prevChar)
                        return false;

                    // V, L, D should never appear in subtraction
                    if (prevChar == 'V' || prevChar == 'L' || prevChar == 'D')
                        return false;
                }

                prevChar = curr;
            }

            return true;
        }
    }
}
