using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCountServer
{
    class WordCounter
    {
        private static readonly HashSet<char> Vowels = new HashSet<char>
        {
            'a', 'e', 'i', 'o', 'u'
        };

        private static bool HasMoreConsonantsThanVowels(string word)
        {
            int vowels = 0;
            int consonants = 0;

            foreach (char c in word.ToLower())
            {
                if (!char.IsLetter(c))
                    continue;

                if (Vowels.Contains(c))
                    vowels++;
                else
                    consonants++;
            }

            return (vowels + consonants) > 0 && consonants > vowels;
        }

        public static int CountWords(string text)
        {
            string[] words = text.Split(
                new char[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?', ';', ':' },
                StringSplitOptions.RemoveEmptyEntries
            );

            int count = 0;
            foreach (string word in words)
            {
                if (HasMoreConsonantsThanVowels(word))
                    count++;
            }

            return count;
        }
    }
}
