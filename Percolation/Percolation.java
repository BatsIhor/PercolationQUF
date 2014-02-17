    public class Percolation
    {
        //replace with array
        private int[][] seeds;
        private boolean[][] openSeeds;
        private int dimens;
        private int N;

        private WeightedQuickUnionUF wQU;

        public Percolation(int N)
        {
            this.N = N;
            dimens = N * N;
            seeds = new int[N][N];
            openSeeds = new boolean[N][N];

            int x = 0;

            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    seeds[i][j] = x;
                    x++;
                    openSeeds[i][j] = false;
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
                 new IndexOutOfBoundsException("index i out of bounds " + i);
            if (j < 0 || j > N) throw
                 new IndexOutOfBoundsException("index j out of bounds " + j);

        }

        public void open(int i, int j)
        {
            validationIndeces(i, j);
            openSeeds[i][ j] = true;

            if ((j - 1 > 0) && isOpen(i, j - 1))
            {
                wQU.union(seeds[i][ j], seeds[i][ j - 1]);
            }

            if ((j + 1 < N) && isOpen(i, j + 1))
            {
                wQU.union(seeds[i][ j], seeds[i][ j + 1]);
            }

            if ((i - 1 > 0) && isOpen(i - 1, j))
            {
                wQU.union(seeds[i][ j], seeds[i - 1][ j]);
            }

            if ((i + 1 < N) && isOpen(i + 1, j))
            {
                wQU.union(seeds[i][ j], seeds[i + 1][ j]);
            }

            if (i == 1)
            {
                wQU.union(dimens, seeds[i][j]);
            }

            if (i == N)
            {
                wQU.union(dimens + 1, seeds[i][j]);
            }
        }

        public boolean isOpen(int i, int j)
        {
            validationIndeces(i, j);
            return openSeeds[i][ j];
        }

        public boolean isFull(int i, int j)
        {
            validationIndeces(i, j);

            if (wQU.connected(dimens, seeds[i][ j]))
                return true;

            return false;
        }

        public boolean percolates()
        {     // does the system percolate?
            if (wQU.connected(dimens, dimens + 1))
                return true;
            return false;
        }
     }