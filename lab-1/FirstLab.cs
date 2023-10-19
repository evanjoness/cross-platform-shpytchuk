using System;
using System.IO;
using System.Collections.Generic;

namespace Labs
{
    public class FirstLab
    {
        public static void Run(string inputPath, string outputPath)
        {
            try
            {
                // Check if the input file exists
                if (!File.Exists(inputPath))
                {
                    Console.Error.WriteLine($"Error: Input file not found at {inputPath}");
                    return;
                }

                // Read input data from the file
                string[] input = File.ReadAllLines(inputPath);

                if (input.Length < 2)
                {
                    Console.Error.WriteLine("Error: Input file is missing data.");
                    return;
                }

                int n, k, p;
                string w;

                if (!int.TryParse(input[0].Split()[0], out n) ||
                    !int.TryParse(input[0].Split()[1], out k) ||
                    !int.TryParse(input[0].Split()[2], out p))
                {
                    Console.Error.WriteLine("Error: Invalid input data format.");
                    return;
                }

                w = input[1];

                Dictionary<char, string> morphism = new Dictionary<char, string>();

                for (int i = 2; i < 2 + n; i++)
                {
                    if (i - 2 >= 0 && i - 2 < 26)
                    {
                        morphism[(char)('A' + i - 2)] = input[i];
                    }
                    else
                    {
                        Console.Error.WriteLine("Error: Too many morphism rules provided.");
                        return;
                    }
                }

                // Compute fk(w) for k times
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

                // Output the p-th character or "-" if it's absent
                if (p >= 1 && p <= w.Length)
                {
                    char resultChar = w[p - 1];
                    Console.WriteLine(resultChar);
                    File.WriteAllText(outputPath, resultChar.ToString());
                }
                else
                {
                    Console.WriteLine("-");
                    File.WriteAllText(outputPath, "-");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
