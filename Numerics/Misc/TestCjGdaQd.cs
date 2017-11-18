using System;
using System.Runtime.InteropServices;

namespace Numerics.Misc
{
    public unsafe class TestCjGdaQd : TestGdaQd
    {

        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void Run_qd_cjgda(qd_real* matrix, int size, qd_real* solutions, DllCallback callback);
        public TestCjGdaQd(int size) : base(size)
        {
        }

        protected override unsafe void Call_Native(qd_real* d, int getLength, DllCallback callback)
        {
            fixed (qd_real* sol = solutions)
                Run_qd_cjgda(d, getLength, sol, callback);
        }

        protected override void Call_Managed(qd_real[,] mat, int size, DllCallback callback)
        {
            int it = 0;
            qd_real[] r = new qd_real[(size)];
            qd_real[] p = new qd_real[(size)];
            qd_real[] p2 = new qd_real[(size)];
            qd_real[] x = new qd_real[(size)];
            qd_real[] x2 = new qd_real[(size)];

            for (int i = 0; i < size; i++)
            {
                r[i] = mat[i, size];
                for (int j = 0; j < size; j++)
                {
                    r[i] -= mat[i, j] * x[j];
                }
                p[i] = r[i];
            }

            qd_real rdot = dot_product(size, r, r);

            for (it = 0; it < size; ++it)
            {
                qd_real alpha = rdot / matrix_form(size, p, p, mat);

                for (int i = 0; i < size; ++i)
                {
                    x2[i] = x[i] + alpha * p[i];
                    for (int j = 0; j < size; j++)
                    {
                        r[i] -= alpha * mat[i, j] * p[j];
                    }
                }

                var r2dot = dot_product(size, r, r);
                if (r2dot < 0.0000000001) break;

                qd_real beta = r2dot / rdot;

                for (int i = 0; i < size; ++i)
                {
                    p2[i] = r[i] + beta * p[i];
                }

                rdot = r2dot;
                var tmpp = p;
                p = p2;
                p2 = tmpp;

                var tmpx = x;
                x = x2;
                x2 = tmpx;
            }
            x2.CopyTo(solutions, 0);
            Console.WriteLine($"Iterations: {it + 1}");
        }

        private qd_real matrix_form(int size, qd_real[] left, qd_real[] right, qd_real[,] mat)
        {
            qd_real ret = qd_real.Zero;
            for (int i = 0; i < size; i++)
            {
                qd_real tmp = qd_real.Zero;
                for (int j = 0; j < size; j++)
                {
                    tmp += right[j] * mat[i, j];
                }
                ret += tmp * left[i];
            }
            return ret;
        }

        private qd_real dot_product(int size, qd_real[] p1, qd_real[] p2)
        {
            qd_real acc = qd_real.Zero;
            for (int i = 0; i < size; i++)
            {
                acc += p1[i] * p2[i];
            }
            return acc;
        }
    }
}