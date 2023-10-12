using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

public class CactusAS
{
    public static void Main(string[] args)
    {
        new CactusAS().Run();
    }

    List<int>[] edges;
    int n;
    bool[] done;
    BigInteger[] answer;
    int[] parent;
    int[][] where;

    public CactusAS()
    {
        n = 0;
    }

    public void Run()
    {
        using (StreamReader reader = new StreamReader("INPUT.TXT"))
        {
            n = int.Parse(reader.ReadLine());
            InitializeDataStructures();

            for (int i = 0; i < n - 1; i++)
            {
                string[] input = reader.ReadLine().Split();
                int a = int.Parse(input[0]) - 1;
                int b = int.Parse(input[1]) - 1;
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

        using (StreamWriter writer = new StreamWriter("OUTPUT.TXT"))
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
