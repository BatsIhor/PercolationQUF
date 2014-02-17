using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PercolationQUF
{
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        int N = 5;
    //        Percolation perc = new Percolation(N);

    //        while (!perc.percolates())
    //        {
    //            Console.WriteLine("Union or x to exit");
    //            //string str1 = Console.ReadLine();
    //            //if (str1 == "x")
    //            //    break;
    //            //string str2 = Console.ReadLine();
    //            //int x = int.Parse(str1);
    //            //int y = int.Parse(str2);

    //            Random rnd = new Random();
    //            int x = rnd.Next(0, N);
    //            int y = rnd.Next(0, N);

    //            perc.open(x, y);

    //            perc.Print();
    //            Console.ReadLine();
    //        }

    //        perc.Print();
    //        Console.WriteLine("We did it!");
    //        Console.ReadLine();
    //    }
    //}

    public class PercolationStats
    {
        private double[] results;

        private int T;

        // perform T independent computational experiments on an N-by-N grid
        public PercolationStats(int N, int T)
        {
            this.T = T;
            results = new double[T];

            Stopwatch sw = new Stopwatch();

            sw.Start();
            for (int i = 0; i < T; i++)
            {
                Percolation percolation = new Percolation(N);
                double inserts = 0;
                Random rnd = new Random(13);
                Random rnd1 = new Random(21);
                while (!percolation.percolates())
                {
                   
                    int x = rnd.Next(0, N);
                    int y = rnd1.Next(0, N);

                    if (x >= 0 && x < N && y >= 0 && y < N)
                    {
                        percolation.open(x, y);
                        inserts++;
                    }

                    percolation.Print();
                    //Console.ReadLine();
                }
                sw.Stop();

                TimeSpan timeSpan = sw.Elapsed;

                Console.WriteLine(timeSpan);
                percolation.Print();

                results[i] = inserts / (N * N);

            }

        }

        // sample mean of percolation threshold
        public double mean()
        {
            return mean(results);
        }

        public static double mean(double[] a)
        {
            if (a.Length == 0) return Double.NaN;
            double sum1 = sum(a);
            return sum1 / a.Length;
        }

        public static double sum(double[] a)
        {
            double sum = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += a[i];
            }
            return sum;
        }

        // sample standard deviation of percolation threshold
        public double stddev()
        {
            return stddev(results);
        }

        public static double stddev(double[] a)
        {
            return Math.Sqrt(var(a));
        }

        public static double var(double[] a)
        {
            if (a.Length == 0) return Double.NaN;
            double avg = mean(a);
            double sum = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += (a[i] - avg) * (a[i] - avg);
            }
            return sum / (a.Length - 1);
        }

        // returns lower bound of the 95% confidence interval
        public double confidenceLo()
        {

            double m = mean();
            double d = stddev(results);
            //A 95% confidence interval would be [mean - 1.96*stdev, mean + 1.96*stdev]
            return m - (1.96 * d) / Math.Sqrt(T);
        }

        // returns upper bound of the 95% confidence interval
        public double confidenceHi()
        {
            double m = mean();
            double d = stddev(results);
            //A 95% confidence interval would be [mean - 1.96*stdev, mean + 1.96*stdev]
            return m + (1.96 * d) / Math.Sqrt(T);
        }

        // test client, described below
        public static void Main(String[] args)
        {
            if (args.Length == 2)
            {
                int N = 0;
                int T = 0;

                //int.TryParse(args[0], out N);
                //int.TryParse(args[1], out T);

                string x = Console.ReadLine();
                string y = Console.ReadLine();
                int.TryParse(x, out N);
                int.TryParse(y, out T);

                PercolationStats percolationStats = new PercolationStats(N, T);

                Console.WriteLine("mean\t\t\t= " + percolationStats.mean());
                Console.WriteLine("stddev\t\t\t= " + percolationStats.stddev());

                Console.WriteLine("95% confidence interval = {0}, {1}", percolationStats.confidenceHi(), percolationStats.confidenceLo());

                Console.ReadLine();
            }
        }
    }

    public class Percolation
    {
        private WeightedQuickUnionUF uf;
        private int[,] indexes;
        private bool[,] openItems;

        private int N;

        // create N-by-N grid, with all sites blocked
        public Percolation(int N)
        {
            uf = new WeightedQuickUnionUF(N * N + 2);

            this.N = N;
            int x = 0;

            indexes = new int[N, N];
            openItems = new bool[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    indexes[i, j] = x;
                    openItems[i, j] = false;
                    x++;
                }
            }

            for (int i = 0; i < N; i++)
            {
                uf.union(N * N, i);
                uf.union(N * N + 1, N * N - N + i);
            }
        }

        // open site (row i, column j) if it is not already
        public void open(int i, int j)
        {
            if (i < 0 || i >= N) return;
            if (j < 0 || j >= N) return;

            if (!isOpen(i, j))
            {
                openItems[i, j] = true;
            }

            if (i + 1 < N && isOpen(i + 1, j) && uf.connected(uf.find(indexes[i, j]), uf.find(indexes[i + 1, j])))
            {
                uf.union(uf.find(indexes[i, j]), uf.find(indexes[i + 1, j]));
            }

            if (j + 1 < N && isOpen(i, j + 1) && uf.connected(uf.find(indexes[i, j]), uf.find(indexes[i, j + 1])))
            {
                uf.union(uf.find(indexes[i, j]), uf.find(indexes[i, j + 1]));
            }

            if (i - 1 >= 0 && isOpen(i - 1, j) && uf.connected(uf.find(indexes[i, j]), uf.find(indexes[i - 1, j])))
            {
                uf.union(uf.find(indexes[i, j]), uf.find(indexes[i - 1, j]));
            }

            if (j - 1 >= 0 && isOpen(i, j - 1) && uf.connected(uf.find(indexes[i, j]), uf.find(indexes[i, j - 1])))
            {
                uf.union(uf.find(indexes[i, j]), uf.find(indexes[i, j - 1]));
            }
        }

        // does the system percolate?
        public bool percolates()
        {
            return uf.find(N * N) == uf.find(N * N + 1);
        }

        // is site (row i, column j) open?
        public bool isOpen(int i, int j)
        {
            if (i < 0 || i > N) throw
           new Exception("isOpen index i out of bounds " + i);
            if (j < 0 || j > N) throw
           new Exception("isOpen index j out of bounds " + j);

            return openItems[i, j];
        }

        // is site (row i, column j) full?
        public bool isFull(int i, int j)
        {
            if (i < 0 || i > N) throw
           new Exception("isFull index i out of bounds " + i);
            if (j < 0 || j > N) throw
           new Exception("isFull index j out of bounds " + j);

            return uf.find(indexes[i, j]) == uf.find(N * N);
        }

        public void Print()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Console.Write(openItems[i, j] ? 1 : 0);
                }
                Console.WriteLine();
            }

            PrintFull();
        }

        void PrintFull()
        {
            Console.WriteLine();
            for (int j = 0; j < N * N + 2; j++)
            {
                Console.Write(" " + uf.id[j]);
            }
            Console.WriteLine();

        }
    }

    public class WeightedQuickUnionUF
    {
        public int[] id;
        private int[] size;

        public WeightedQuickUnionUF(int count)
        {
            id = new int[count];
            size = new int[count];

            for (int i = 0; i < count; i++)
            {
                id[i] = i;
                size[i] = 1;
            }
        }

        public void union(int p, int q)
        {
            //Console.WriteLine(p + " " + q);

            //connect roots 
            int pid_root = find(p);
            int qid_root = find(q);

            if (pid_root == qid_root)
                return;

            if (size[pid_root] < size[qid_root])
            {
                id[pid_root] = qid_root;
                size[qid_root] += size[pid_root];
            }
            else
            {
                id[qid_root] = pid_root;
                size[pid_root] += size[qid_root];
            }
        }

        public bool connected(int p, int q)
        {
            return find(p) == find(q);
        }

        public void Print()
        {
            for (int i = 0; i < id.Length; i++)
            {
                Console.Write(id[i] + " ");
            }
            Console.WriteLine();
        }

        public int find(int x)
        {
            if (id[x] != x)
            {
                return find(id[x]);
            }
            return x;
        }
    }
}
