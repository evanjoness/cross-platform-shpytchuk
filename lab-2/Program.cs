using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Labs
{
    public class SecondLab
    {
        List<int>[] edges;
        int n;
        bool[] done;
        BigInteger[] answer;
        int[] parent;
        int[][] where;

        public SecondLab()
        {
            n = 0;
        }

        public static void Run(string inputPath, string outputPath)
        {
            SecondLab cactus = new SecondLab();
            try
            {
                cactus.Execute(inputPath, outputPath);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }

        private void Execute(string inputPath, string outputPath)
        {
            using (StreamReader reader = new StreamReader(inputPath))
            {
                string? line = reader.ReadLine(); // Use nullable reference type
                if (line == null)
                {
                    throw new ArgumentException("Input file is empty.");
                }

                if (!int.TryParse(line, out n))
                {
                    throw new ArgumentException("Invalid value for n in the input file.");
                }

                InitializeDataStructures();

                for (int i = 0; i < n - 1; i++)
                {
                    line = reader.ReadLine(); // Read the next line
                    if (line == null)
                    {
                        throw new ArgumentException("Input file is missing edge data.");
                    }

                    string[] input = line.Split();
                    if (input.Length != 2 || !int.TryParse(input[0], out int a) || !int.TryParse(input[1], out int b))
                    {
                        throw new ArgumentException("Invalid edge data in the input file.");
                    }

                    a--; // Decrement to match 0-based indexing
                    b--; // Decrement to match 0-based indexing

                    edges[a].Add(b);
                    edges[b].Add(a);
                }
            }

            parent = new int[n];
            where = new int[n][];
            for (int i = 0; i < n; i++)
            {
                where[i] = new int[n];
            }

            Dfs(0);

            answer = new BigInteger[n];
            done = new bool[n];
            BigInteger result = Calc(0);

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                writer.WriteLine(result);
                Console.WriteLine(result);
            }
        }


        private void InitializeDataStructures()
        {
            edges = new List<int>[n];
            for (int i = 0; i < n; i++)
            {
                edges[i] = new List<int>();
            }
        }

        private void Dfs(int x)
        {
            foreach (int y in edges[x])
            {
                if (y != parent[x])
                {
                    parent[y] = x;
                    Dfs(y);
                    for (int i = 0; i < n; i++)
                    {
                        if (where[y][i] != 0)
                        {
                            where[x][i] = y;
                        }
                    }
                    where[x][y] = y;
                }
            }
        }

        private BigInteger Calc(int x)
        {
            if (done[x])
            {
                return answer[x];
            }
            done[x] = true;

            BigInteger noEdge = BigInteger.One;
            foreach (int y in edges[x])
            {
                if (y != parent[x])
                {
                    noEdge *= Calc(y);
                }
            }

            answer[x] = noEdge;
            BigInteger[] backEdge = new BigInteger[n];
            for (int i = 0; i < n; i++)
            {
                backEdge[i] = BigInteger.Zero;
            }

            for (int i = 0; i < n; i++)
            {
                if (where[x][i] != 0)
                {
                    int j = x;
                    BigInteger r = BigInteger.One;
                    while (true)
                    {
                        foreach (int y in edges[j])
                        {
                            if (y != parent[j] && y != where[j][i])
                            {
                                r *= Calc(y);
                            }
                        }
                        if (j == i)
                        {
                            break;
                        }
                        j = where[j][i];
                    }
                    backEdge[i] = r;
                    if (where[x][i] != i)
                    {
                        answer[x] += backEdge[i];
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                if (where[x][i] != 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (where[x][j] != 0 && where[x][i] != where[x][j])
                        {
                            BigInteger r = backEdge[i] * backEdge[j] / noEdge;
                            answer[x] += r;
                        }
                    }
                }
            }

            return answer[x];
        }
    }
}
