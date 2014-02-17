using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PercolationQUF
{
    /// <summary>
    /// Percolation statistics and timing.
    /// </summary>
    public class PercolationStats
    {
        private double[] results;

        private int T;

        /// <summary>
        /// perform T independent computational experiments on an N-by-N grid
        /// </summary>
        /// <param name="N"></param>
        /// <param name="T"></param>
        public PercolationStats(int N, int T)
        {
            this.T = T;
            results = new double[T];

            Stopwatch sw = new Stopwatch();

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

                    //percolation.Print();
                    //Console.ReadLine();
                }

                //percolation.Print();

                results[i] = inserts / (N * N);
            }
        }

        #region math for statistics

        /// <summary>
        ///  sample mean of percolation threshold
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        ///sample standard deviation of percolation threshold
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// returns lower bound of the 95% confidence interval
        /// </summary>
        /// <returns></returns>
        public double confidenceLo()
        {

            double m = mean();
            double d = stddev(results);
            //A 95% confidence interval would be [mean - 1.96*stdev, mean + 1.96*stdev]
            return m - (1.96 * d) / Math.Sqrt(T);
        }

        /// <summary>
        /// returns upper bound of the 95% confidence interval
        /// </summary>
        /// <returns></returns>
        public double confidenceHi()
        {
            double m = mean();
            double d = stddev(results);
            //A 95% confidence interval would be [mean - 1.96*stdev, mean + 1.96*stdev]
            return m + (1.96 * d) / Math.Sqrt(T);
        }

        #endregion

        /// <summary>
        /// test client, described below
        /// </summary>
        /// <param name="args"></param> 
        public static void Main(String[] args)
        {
            int N = 0; //size of array
            int T = 0; //number times to check

            string n = Console.ReadLine();
            string t = Console.ReadLine();

            int.TryParse(n, out N);
            int.TryParse(t, out T);

            Stopwatch sw = new Stopwatch();

            sw.Start();
            PercolationStats percolationStats = new PercolationStats(N, T);
            sw.Stop();

            Console.WriteLine("mean\t\t\t= " + percolationStats.mean());
            Console.WriteLine("stddev\t\t\t= " + percolationStats.stddev());
            Console.WriteLine("95% confidence interval = {0}, {1}", percolationStats.confidenceHi(), percolationStats.confidenceLo());
            Console.WriteLine("Time = {0}", sw.Elapsed);

            Console.ReadLine();
        }
    }

    /// <summary>
    /// Percolation logic itself
    /// </summary>
    public class Percolation
    {
        //replace with array
        private int[, ] seeds;
        private bool[, ] openSeeds;
        private int dimens;
        private int N;

        private WeightedQuickUnionUF wQU;

        public Percolation(int N)
        {
            this.N = N;
            dimens = N * N;
            seeds = new int[N, N];
            openSeeds = new bool[N, N];

            int x = 0;

            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    seeds[i, j] = x;
                    x++;
                    openSeeds[i, j] = false;
                }

            wQU = new WeightedQuickUnionUF(dimens + 2);
            for (int i = 0; i < N; i++)
            {
                wQU.union(dimens, i);
                wQU.union(N * N + 1, N * N - N + i);
            }

        }

        private void validationIndeces(int i, int j)
        {
            if (i < 0 || i > N) throw
                 new Exception("index i out of bounds " + i);
            if (j < 0 || j > N) throw
                 new Exception("index j out of bounds " + j);

        }

        public void open(int i, int j)
        {
            validationIndeces(i, j);
            openSeeds[i, j] = true;

            if ((j - 1 > 0) && isOpen(i, j - 1))
            {
                wQU.union(seeds[i, j], seeds[i, j - 1]);
            }

            if ((j + 1 < N) && isOpen(i, j + 1))
            {
                wQU.union(seeds[i, j], seeds[i, j + 1]);
            }

            if ((i - 1 > 0) && isOpen(i - 1, j))
            {
                wQU.union(seeds[i, j], seeds[i - 1, j]);
            }

            if ((i + 1 < N) && isOpen(i + 1, j))
            {
                wQU.union(seeds[i, j], seeds[i + 1, j]);
            }

            if (i == 1)
            {
                wQU.union(dimens, seeds[i, j]);
            }

            if (i == N)
            {
                wQU.union(dimens + 1, seeds[i, j]);
            }
        }

        public bool isOpen(int i, int j)
        {
            validationIndeces(i, j);
            return openSeeds[i, j];
        }

        public bool isFull(int i, int j)
        {
            validationIndeces(i, j);

            return wQU.connected(dimens, seeds[i, j]);
             
        }

        public bool percolates()
        {
            return wQU.connected(dimens, dimens + 1);
        }
    }

    /// <summary>
    /// Wighted quick union alg.
    /// </summary>
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
