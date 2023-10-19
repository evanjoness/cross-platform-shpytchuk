using System;
using System.Collections.Generic;
using System.IO;

namespace Labs
{
    public class ThirdLab
    {
        public static void Run(string inputPath, string outputPath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(inputPath))
                {
                    string? line = reader.ReadLine();
                    if (line == null)
                    {
                        throw new ArgumentException("Input file is empty.");
                    }

                    string[] input = line.Split();
                    if (input.Length != 2 || !int.TryParse(input[0], out int n) || !int.TryParse(input[1], out int m))
                    {
                        throw new ArgumentException("Invalid values for n and m in the input file.");
                    }

                    line = reader.ReadLine();
                    if (line == null || !int.TryParse(line, out int r))
                    {
                        throw new ArgumentException("Invalid value for r in the input file.");
                    }

                    List<Edge>[] edges = new List<Edge>[n];
                    for (int i = 0; i < n; i++)
                    {
                        edges[i] = new List<Edge>();
                    }

                    int[] dga = new int[n];
                    int[] dgb = new int[m];
                    int totalCost = 0;

                    for (int i = 0; i < r; i++)
                    {
                        line = reader.ReadLine();
                        if (line == null)
                        {
                            throw new ArgumentException("Incomplete edge data in the input file.");
                        }

                        string[] data = line.Split();
                        if (data.Length != 3 || !int.TryParse(data[0], out int a) || !int.TryParse(data[1], out int b) || !int.TryParse(data[2], out int c))
                        {
                            throw new ArgumentException("Invalid edge data in the input file.");
                        }

                        a--; // Decrement to match 0-based indexing
                        b--; // Decrement to match 0-based indexing
                        dga[a]++;
                        dgb[b]++;
                        edges[a].Add(new Edge(i + 1, a, b, 1, -c, 0));
                        totalCost += c;
                    }

                    Graph g = new Graph(n + m + 2);

                    for (int i = 0; i < n; i++)
                    {
                        g.AddEdge(0, 0, i + 1, dga[i] - 1, 0);
                    }

                    for (int i = 0; i < m; i++)
                    {
                        g.AddEdge(0, i + n + 1, n + m + 1, dgb[i] - 1, 0);
                    }

                    for (int i = 0; i < n; i++)
                    {
                        foreach (Edge e in edges[i])
                        {
                            g.AddEdge(e.i, e.s + 1, n + e.d + 1, 1, e.p);
                        }
                    }

                    int cost = totalCost + g.MinCostFlow(0, m + n + 1);

                    bool[] buy = new bool[r];
                    int nbuy = 0;

                    for (int i = 1; i < n + 1; i++)
                    {
                        foreach (Edge e in g.edges[i])
                        {
                            if (e.i != 0 && e.f == 0)
                            {
                                buy[e.i - 1] = true;
                                nbuy++;
                            }
                        }
                    }

                    using (StreamWriter writer = new StreamWriter(outputPath))
                    {
                        Console.WriteLine(cost);
                        Console.WriteLine(nbuy);
                        writer.WriteLine(cost);
                        writer.WriteLine(nbuy);

                        for (int i = 0; i < r; i++)
                        {
                            if (buy[i])
                            {
                                Console.Write(i + 1);
                                Console.Write(" ");
                                writer.Write(i + 1);
                                writer.Write(" ");
                            }
                        }

                        writer.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }

        public class Graph
        {
            int n;
            public List<Edge>[] edges;

            public Graph(int n)
            {
                this.n = n;
                edges = new List<Edge>[n];
                for (int i = 0; i < n; i++)
                {
                    edges[i] = new List<Edge>();
                }
            }

            public void AddEdge(int i, int s, int d, int c, int p)
            {
                Edge ef = new Edge(i, s, d, c, p, 0);
                Edge er = new Edge(0, d, s, 0, -p, 0);
                ef.r = er;
                er.r = ef;
                edges[s].Add(ef);
                edges[d].Add(er);
            }

            public int MinCostFlow(int s, int t)
            {
                int[] d = new int[n];
                bool[] u = new bool[n];

                d[s] = 0;
                u[s] = true;
                bool changed = true;

                while (changed)
                {
                    changed = false;
                    for (int i = 0; i < n; i++)
                    {
                        if (u[i])
                        {
                            foreach (Edge e in edges[i])
                            {
                                if (e.c > 0 && (!u[e.d] || d[e.d] > d[i] + e.p))
                                {
                                    u[e.d] = true;
                                    d[e.d] = d[i] + e.p;
                                    changed = true;
                                }
                            }
                        }
                    }
                }

                int[] q = new int[n];

                for (int i = 0; i < n; i++)
                {
                    q[i] = d[i];
                }

                int curFlow = 0;
                int cost = 0;
                bool[] v = new bool[n];
                int[] c = new int[n];
                Edge[] from = new Edge[n];

                while (u[t])
                {
                    Array.Fill(u, false);
                    Array.Fill(v, false);
                    Array.Fill(c, 0);

                    // Run Dijkstra
                    d[s] = 0;
                    v[s] = true;
                    c[s] = int.MaxValue;

                    while (!u[t])
                    {
                        int i = -1;
                        for (int j = 0; j < n; j++)
                        {
                            if (!u[j] && v[j] && (i == -1 || d[j] < d[i]))
                            {
                                i = j;
                            }
                        }

                        if (i == -1)
                        {
                            break;
                        }

                        u[i] = true;

                        foreach (Edge e in edges[i])
                        {
                            if (e.c > e.f)
                            {
                                if (!v[e.d] || d[e.d] > d[i] + e.p + q[e.s] - q[e.d])
                                {
                                    v[e.d] = true;
                                    d[e.d] = d[i] + e.p + q[e.s] - q[e.d];
                                    c[e.d] = Math.Min(c[i], e.c - e.f);
                                    from[e.d] = e;
                                }
                            }
                        }
                    }

                    if (!u[t])
                    {
                        break;
                    }

                    for (int i = 0; i < n; i++)
                    {
                        if (!u[i])
                        {
                            d[i] = d[t];
                        }
                    }

                    if (q[t] + d[t] > 0)
                    {
                        break;
                    }

                    curFlow += c[t];
                    cost += (q[t] + d[t]) * c[t];
                    int k = t;

                    while (k != s)
                    {
                        Edge e = from[k];
                        e.f += c[t];
                        e.r.f -= c[t];
                        k = e.s;
                    }

                    for (int i = 0; i < n; i++)
                    {
                        q[i] += d[i];
                    }
                }

                return cost;
            }
        }

        public class Edge
        {
            public int s;
            public int d;
            public int c;
            public int p;
            public int f;
            public int i;
            public Edge r;

            public Edge(int i, int s, int d, int c, int p, int f)
            {
                this.i = i;
                this.s = s;
                this.d = d;
                this.c = c;
                this.p = p;
                this.f = f;
            }
        }
    }
}
