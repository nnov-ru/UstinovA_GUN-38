using System;
using System.Numerics;
using System.Text;
namespace HomeWork
{
    internal class Program
    {
        public static string ConcatenateStrings(string str1, string str2)
        {
            return str1 + str2;
        }

        public static string GreetUser(string name, int age) 
        {
            return $"Hello, {name}!\nYou are {age} years old.";
        }

        public static string AnalyzeString(string input)
        {
            return $"Number of characters in your string: {input.Length}\nUppercase: {input.ToUpper()}\nLowercase: {input.ToLower()}";
        }

        public static string GetFirst5Chars(string input)
        {
            if (input.Length <= 5)
            { return input; }
            else
            { return input.Substring(0, 5); }
        }

        public static StringBuilder StringConcatenator(string[] strings)
        {
            var builder = new StringBuilder();
            foreach (var str in strings)
            {
                builder.Append(str).Append(" ");
            }
            return builder;
        }

        public static string ReplaceWords(string inputString, string wordToReplace, string replacementWord)
        {
            return inputString.Replace(wordToReplace, replacementWord);
        }

    static void Main(string[] args)
        {
            Console.WriteLine("\nTASK 1.\n" + ConcatenateStrings("O Brother, ", "Where Art Thou ?"));
            Console.WriteLine("\nTASK 2.\n" + GreetUser("Lemahn Brother", 65));
            Console.WriteLine("\nTASK 3.\n" + AnalyzeString("BrotherLinesSES"));
            Console.WriteLine("\nTASK 4.\n" + GetFirst5Chars("6pa+"));
            var builder = StringConcatenator(new[] {"Bs", "R", "Ow"});
            Console.WriteLine("\nTASK 5.\n" + builder.ToString());
            Console.WriteLine("\nTASK 6.\n" + ReplaceWords("O Brother, Where Art Thou ?", "Art Thou", "Are You"));
        }
    }
}
