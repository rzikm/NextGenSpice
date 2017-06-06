using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NextGenSpice
{
    public unsafe class Test
    {
        delegate void DllCallback();

        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void Run_d(double* matrix, int size, DllCallback callback);

        private double[,] matrix;
        private double[,] matrix2;

        public Test(int size)
        {
            matrix2 = new double[size, size + 1];
            Random rnd = new Random(42);
            for (var i = 0; i < size; i++)
            for (var j = 0; j < size; j++)
            {
                matrix2[i, j] = 1f / (i + j + 1);
                matrix2[i, size] += matrix2[i, j];
            }
        }

        public double[] RunTest_Native()
        {
            matrix = (double[,]) matrix2.Clone();
            Stopwatch sw = Stopwatch.StartNew();
            fixed (double* m = matrix)
            {
                Run_d(m, matrix.GetLength(0), Callback);
            }
            sw.Stop();
            Console.WriteLine($"Native:  {sw.Elapsed}");
            return GetResults();
        }

        private double[] GetResults()
        {
            double[] ret = new double[matrix.GetLength(0)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = matrix[i, ret.Length];
            }
            return ret;
        }

        private long i = 0;

        public void Callback()
        {
            i++;
        }

        public double[] RunTest_Managed()
        {
            matrix = (double[,]) matrix2.Clone();
            Stopwatch sw = Stopwatch.StartNew();
            {
                RunManaged(matrix, matrix.GetLength(0), Callback);
            }
            sw.Stop();
            Console.WriteLine($"Managed: {sw.Elapsed}");
            return GetResults();
        }

        static void RunManaged(double[,] m, int size, DllCallback callback)
        {
            for (int i = 0; i < size; i++)
            {
                // Search for maximum in this column
                double maxEl = Math.Abs(m[i, i]);
                int maxRow = i;
                for (int k = i + 1; k < size; k++)
                {
                    if (Math.Abs(m[k, i]) > maxEl)
                    {
                        maxEl = Math.Abs(m[k, i]);
                        maxRow = k;
                    }
                }

                // Swap maximum row with current row (column by column)
                for (int k = i; k < size + 1; k++)
                {
                    double tmp = m[maxRow, k];
                    m[maxRow, k] = m[i, k];
                    m[i, k] = tmp;
                    callback();
                }

                // Make all rows below this one 0 in current column
                for (int k = i + 1; k < size; k++)
                {
                    double c = -m[k, i] / m[i, i];
                    for (int j = i; j < size + 1; j++)
                    {
                        if (i == j)
                        {
                            m[k, j] = 0;
                        }
                        else
                        {
                            m[k, j] += c * m[i, j];
                        }
                    }
                }
            }

            // Solve equation Ax=b for an upper triangular matrix A
            for (int i = size - 1; i >= 0; i--)
            {
                m[i, size] = m[i, size] / m[i, i];
                for (int k = i - 1; k >= 0; k--)
                {
                    m[k, size] -= m[k, i] * m[i, size];
                }
                callback();
            }
        }
    }
}