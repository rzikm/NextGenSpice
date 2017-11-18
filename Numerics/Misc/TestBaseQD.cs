using System;
using System.Diagnostics;

namespace Numerics.Misc
{
    public abstract class TestBaseQD
    {
        protected delegate void DllCallback();

        private qd_real[,] matrix;
        private readonly qd_real[,] matrix2;
        private long i = 0;

        public TestBaseQD(int size)
        {
            matrix2 = new qd_real[size, size + 1];
            Random rnd = new Random(30);
            for (var i = 0; i < size; i++)
            for (var j = 0; j < size; j++)
            {
//              Hilbert matrix
//                matrix2[i, j] = 1 / (qd_real.Zero + i + j + 1);
//                matrix2[i, size] += matrix2[i, j];

                var val = rnd.NextDouble();
                matrix2[i, j] += val;
                matrix2[j, i] += val;
                matrix2[i, size] += val;
                matrix2[j, size] += val;
            }
        }

        public unsafe qd_real[] RunTest_Native()
        {
            matrix = (qd_real[,]) matrix2.Clone();
            Stopwatch sw = Stopwatch.StartNew();
            fixed (qd_real* m = matrix)
            {
                Call_Native(m, matrix.GetLength(0), Callback);
            }
            sw.Stop();
            Console.WriteLine($"Native:  {sw.Elapsed}");
            return GetResults();
        }

        protected abstract unsafe void Call_Native(qd_real* qdReal, int getLength, DllCallback callback);

        protected virtual qd_real[] GetResults()
        {
            qd_real[] ret = new qd_real[matrix.GetLength(0)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = matrix[i, ret.Length];
            }
            return ret;
        }

        public void Callback()
        {
            i++;
        }

        public qd_real[] RunTest_Managed()
        {
            matrix = (qd_real[,]) matrix2.Clone();
            Stopwatch sw = Stopwatch.StartNew();
            {
                Call_Managed(matrix, matrix.GetLength(0), Callback);
            }
            sw.Stop();
            Console.WriteLine($"Managed: {sw.Elapsed}");
            return GetResults();
        }

        protected abstract void Call_Managed(qd_real[,] qdReals, int getLength, DllCallback callback);
    }
}