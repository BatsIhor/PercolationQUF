    public class PercolationStats
    {
        private double[] results;
        private int T;

        // perform T independent computational experiments on an N-by-N grid
        public PercolationStats(int N, int T)
        {
            this.T = T;
            results = new double[T];

            for (int i = 0; i < T; i++)
            {
                Percolation percolation = new Percolation(N);
                double inserts = 0;
                
                while (!percolation.percolates())
                {
                    int x = StdRandom.uniform(0, N);
                    int y = StdRandom.uniform(0, N);

                    if (x >= 0 && x < N && y >= 0 && y < N)
                    {
                        percolation.open(x, y);
                        inserts++;
                    }
                }
                results[i] = inserts / (N * N);
            }
        }

        // sample mean of percolation threshold
        public double mean()
        {
            return StdStats.mean(results);
        }
        
        // sample standard deviation of percolation threshold
        public double stddev()
        {
            return StdStats.stddev(results);
        }

        // returns lower bound of the 95% confidence interval
        public double confidenceLo()
        {
            double m = mean();
            double d = stddev();
            return m - (1.96 * d) / Math.sqrt(T);
        }

        // returns upper bound of the 95% confidence interval
        public double confidenceHi()
        {
            double m = mean();
            double d = stddev();
            return m + (1.96 * d) / Math.sqrt(T);
        }

        // test client, described below
        public static void main(String[] args)
        {
            if (args.length == 2)
            {
                int n = 0;
                int t = 0;
                
                int val;
                try
                {
                    n = Integer.parseInt(args[0]);
                    t = Integer.parseInt(args[1]);
                }
                catch (NumberFormatException nfe)
                {
                    return;
                }
                
                if (t <= 0)
                {
                    throw new IllegalArgumentException();
                }                
                if (n <= 0)
                {
                    throw new IllegalArgumentException();
                }
                
                PercolationStats percStats = new PercolationStats(n, t);
                System.out.println("mean\t\t\t= " + percStats.mean());
                System.out.println("stddev\t\t\t= " + percStats.stddev());

                double hi = percStats.confidenceHi();
                double lo = percStats.confidenceLo();

                System.out.println("95/% confidence interval = "
                                       + hi + " " + lo);
            }           
        }
    }