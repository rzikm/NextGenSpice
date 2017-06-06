using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NextGenSpice.Numerics;

namespace NextGenSpice
{
    public unsafe class TestQD
    {
        delegate void DllCallback();

        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void Run_qd(qd_real* matrix, int size, DllCallback callback);

        private qd_real[,] matrix;
        private qd_real[,] matrix2;

        public TestQD(int size)
        {
            matrix2 = new qd_real[size, size + 1];
            Random rnd = new Random(42);
            for (var i = 0; i < size; i++)
            for (var j = 0; j < size; j++)
            {
                matrix2[i, j] = 1 / (qd_real.Zero + i + j + 1);
                matrix2[i, size] += matrix2[i, j];
            }
        }

        public qd_real[] RunTest_Native()
        {
            matrix = (qd_real[,]) matrix2.Clone();
            Stopwatch sw = Stopwatch.StartNew();
            fixed (qd_real* m = matrix)
            {
                Run_qd(m, matrix.GetLength(0), Callback);
            }
            sw.Stop();
            Console.WriteLine($"Native:  {sw.Elapsed}");
            return GetResults();
        }

        private qd_real[] GetResults()
        {
            qd_real[] ret = new qd_real[matrix.GetLength(0)];
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


        public qd_real[] RunTest_Managed()
        {
            matrix = (qd_real[,]) matrix2.Clone();
            Stopwatch sw = Stopwatch.StartNew();
            {
                RunManaged(matrix, matrix.GetLength(0), Callback);
            }
            sw.Stop();
            Console.WriteLine($"Managed: {sw.Elapsed}");
            return GetResults();
        }


        static void RunManaged(qd_real[,] m, int size, DllCallback callback)
        {
            for (int i = 0; i < size; i++)
            {
                // Search for maximum in this column
                qd_real maxEl = (m[i, i].Abs());
                int maxRow = i;
                for (int k = i + 1; k < size; k++)
                {
                    if (m[k, i].Abs() > maxEl)
                    {
                        maxEl = m[k, i].Abs();
                        maxRow = k;
                    }
                }

                // Swap maximum row with current row (column by column)
                for (int k = i; k < size + 1; k++)
                {
                    qd_real tmp = m[maxRow, k];
                    m[maxRow, k] = m[i, k];
                    m[i, k] = tmp;
                    callback();
                }

                // Make all rows below this one 0 in current column
                for (int k = i + 1; k < size; k++)
                {
                    qd_real c = -m[k, i] / m[i, i];
                    for (int j = i; j < size + 1; j++)
                    {
                        if (i == j)
                        {
                            m[k, j] = qd_real.Zero;
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