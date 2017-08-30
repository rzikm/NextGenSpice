using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NextGenSpice
{
    public unsafe class TestGeDouble : TestBaseDouble
    {

        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void Run_d(double* matrix, int size, DllCallback callback);

        public TestGeDouble(int size) : base(size)
        {
        }

        protected override unsafe void Call_Native(double* d, int size, DllCallback callback)
        {
            Run_d(d, size, callback);
        }
        
        protected override void Call_Managed(double[,] doubles, int getLength, TestBaseDouble.DllCallback callback)
        {
            RunManaged(doubles, getLength, callback);
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