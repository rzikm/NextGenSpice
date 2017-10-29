using System.Runtime.InteropServices;
using Numerics;

namespace NextGenSpiceTest.Misc
{
    public unsafe class TestQdGe : TestBaseQD
    {

        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void Run_qd(qd_real* matrix, int size, DllCallback callback);

        public TestQdGe(int size) : base(size)
        {
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

            // GaussElimSolve equation Ax=b for an upper triangular matrix A
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

        protected override unsafe void Call_Native(qd_real* qdReal, int getLength, DllCallback callback)
        {
            Run_qd(qdReal, getLength, callback);
        }

        protected override void Call_Managed(qd_real[,] qdReals, int getLength, DllCallback callback)
        {
            RunManaged(qdReals, getLength, callback);
        }
    }
}