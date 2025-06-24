using System;
using System.Collections.Generic;
using TestConsoleApp.Interfaces;

namespace TestConsoleApp
{
    public class Palindrome : IPalindrome
    {
        public bool IsPalindrome(string word)
        {
            List<char> reversedChar = new List<char>();

            foreach (char c in word)
            {
                reversedChar.Insert(0, c);
            }

            var reversedWord = String.Join("", reversedChar).ToLower();
            return reversedWord == word.ToLower();
        }
    }

    public class PalindromeEfficient : IPalindrome
    {
        public bool IsPalindrome(string word)
        {
            word = word.ToLower();

            int left = 0;
            int right = word.Length - 1;

            while (left < right)
            {
                if (word[left] != word[right])
                    return false;

                left++;
                right--;
            }

            return true;
        }
    }
}
