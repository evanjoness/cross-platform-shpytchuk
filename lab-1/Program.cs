using System;
using System.IO;

class Program
{
    static void Main()
    {
        //зчитуєм вхідні дані з файлу INPUT.TXT
        string[] input = File.ReadAllLines("INPUT.TXT");
        int W = int.Parse(input[0].Split()[0]);
        int H = int.Parse(input[0].Split()[1]);

        // Вираховування кількості невироджених прямокутників
        long result = CountRectangles(W, H);

        // Записуєм результат у вихідний файл OUTPUT.TXT
        File.WriteAllText("OUTPUT.TXT", result.ToString());
    }

    static long CountRectangles(int W, int H)
    {

        long s = 0;
        for (long i = 1; i <= W; i++)
        {
            for (long j = 1; j <= H; j++)
            {
                s += i * j;
            }
        }

        return s;
    }
}
