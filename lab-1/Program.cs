using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        // Читаємо вхідні дані з файлу INPUT.TXT
        string inputPath = "../../../INPUT.TXT";
        string outputPath = "../../../OUTPUT.TXT";

        string[] input = File.ReadAllLines(inputPath);
        int n, k, p;
        string w;
        n = int.Parse(input[0].Split()[0]);
        k = int.Parse(input[0].Split()[1]);
        p = int.Parse(input[0].Split()[2]);
        w = input[1];
        Dictionary<char, string> morphism = new Dictionary<char, string>();

        for (int i = 2; i < 2 + n; i++)
        {
            morphism[(char)('A' + i - 2)] = input[i];
        }

        // Обчислюємо fk(w) для k разів
        for (int i = 0; i < k; i++)
        {
            string newW = "";
            foreach (char c in w)
            {
                if (morphism.ContainsKey(c))
                {
                    newW += morphism[c];
                }
                else
                {
                    newW += c;
                }
            }
            w = newW;
        }

        // Виводимо p-й символ або "-" якщо він відсутній
        if (p <= w.Length)
        {
            Console.WriteLine(w[p - 1]);
            File.WriteAllText(outputPath, w[p - 1].ToString());
        }
        else
        {
            Console.WriteLine("-");
            File.WriteAllText(outputPath, "-");
        }
    }
}
