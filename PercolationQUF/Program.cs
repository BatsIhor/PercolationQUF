using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PercolationQUF
{
    class Program
    {
        static void Main(string[] args)
        {
            Percolation perc = new Percolation(4);

            while (!perc.percolates())
            {
                Console.WriteLine("Union or x to exit");
                string str1 = Console.ReadLine();
                if (str1 == "x")
                    break;
                string str2 = Console.ReadLine();


                int x = int.Parse(str1);
                int y = int.Parse(str2);

                //Random rnd = new Random();
                //int x = rnd.Next(N);
                //int y = rnd.Next(N);

                perc.open(x, y);

                perc.Print();
            }
        }
    }

    public class Percolation
    {
        UnionFind uf;
        int[,] openSites;
        int[,] fullSites;

        int N;

        // create N-by-N grid, with all sites blocked
        public Percolation(int N)
        {
            uf = new UnionFind(N * N + 2);

            this.N = N;

            int x = 0;

            openSites = new int[N, N];
            fullSites = new int[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {

                    openSites[i, j] = x;
                    fullSites[i, j] = 0;
                    x++;
                }
            }

            for (int i = 0; i < N; i++)
            {
                //fullSites[0, i] = 1;
                uf.union(N * N, i);
                uf.union(N * N + 1, N * N - N + i);
            }
        }

        // open site (row i, column j) if it is not already
        public void open(int i, int j)
        {
            //Check boundaries
            if (i < 0 || i >= N) return;
            if (j < 0 || j >= N) return;

            //if is not open Open it
            if (!isFull(i, j))
            {
                fullSites[i, j] = 1;
            }

            // if we have neighbors lets connect with them using union
            if (isFull(i + 1, j))
            {
                uf.union(uf.root(openSites[i, j]), uf.root(openSites[i + 1, j]));
            }

            if (isFull(i, j + 1))
            {
                uf.union(uf.root(openSites[i, j]), uf.root(openSites[i, j + 1]));
            }

            if (isFull(i - 1, j))
            {
                uf.union(uf.root(openSites[i, j]), uf.root(openSites[i - 1, j]));
            }

            if (isFull(i, j - 1))
            {
                uf.union(uf.root(openSites[i, j]), uf.root(openSites[i, j - 1]));
            }
        }

        private void markAllNieghborsAsFull(int i, int j)
        {
            if (i < 0 || i >= N) return;
            if (j < 0 || j >= N) return;

            if (isOpen(i, j) && !isFull(i, j))
            {
                fullSites[i, j] = 1;

                markAllNieghborsAsFull(i + 1, j);
                markAllNieghborsAsFull(i, j + 1);
                markAllNieghborsAsFull(i - 1, j);
                markAllNieghborsAsFull(i, j - 1);
            }
        }

        // does the system percolate?
        public bool percolates()
        {
            return uf.root(N * N) == uf.root(N * N + 1);
        }

        // is site (row i, column j) open?
        public bool isOpen(int i, int j)
        {
            if (i < 0 || i >= N) return false;
            if (j < 0 || j >= N) return false;

            return openSites[i, j] == 1;
        }

        // is site (row i, column j) full?
        public bool isFull(int i, int j)
        {
            if (i < 0 || i >= N) return false;
            if (j < 0 || j >= N) return false;
            return fullSites[i, j] == 1;
        }

        public void Print()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Console.Write("\t" + openSites[i, j]);
                }
                PrintFull(i);

                Console.WriteLine();
            }

            PrintFull();
        }


        void PrintFull(int i)
        {
            Console.Write("\t\t\t");

            for (int j = 0; j < N; j++)
            {
                Console.Write(fullSites[i, j]);
            }
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

    public class UnionFind
    {
        public int[] id;
        private int[] size;

        public UnionFind(int count)
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
            Console.WriteLine(p + " " + q);

            //connect roots 
            int pid_root = root(p);
            int qid_root = root(q);

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
            return root(p) == root(q);
        }

        public void Print()
        {
            for (int i = 0; i < id.Length; i++)
            {
                Console.Write(id[i] + " ");
            }
            Console.WriteLine();
        }

        public int root(int x)
        {
            if (id[x] != x)
            {
                return root(id[x]);
            }
            return x;
        }
    }
}
