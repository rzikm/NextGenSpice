using System;
using System.Diagnostics;

namespace NextGenSpiceTest.Misc
{
    public abstract class TestBaseDouble
    {
        protected double[,] matrix;
        protected double[,] matrix2;

        public TestBaseDouble(int size)
        {
            matrix2 = new double[size, size + 1];
            Random rnd = new Random(30);
            for (var i = 0; i < size; i++)
                for (var j = 0; j < size; j++)
                {
                    //              Hilbert Matrix
//                                    matrix2[i, j] = 1f / (i + j + 1);
//                                    matrix2[i, size] += matrix2[i, j];

                    var val = rnd.NextDouble();
                    matrix2[i, j] += val;
                    matrix2[j, i] += val;
                    matrix2[i, size] += val;
                    matrix2[j, size] += val;
                }
        }

        protected delegate void DllCallback();

        public virtual unsafe double[] RunTest_Native()
        {
            matrix = (double[,])matrix2.Clone();
            Stopwatch sw = Stopwatch.StartNew();
            fixed (double* m = matrix)
            {
                Call_Native(m, matrix.GetLength(0), Callback);
            }
            sw.Stop();
            Console.WriteLine($"Native:  {sw.Elapsed}");
            return GetResults();
        }

        protected abstract unsafe void Call_Native(double* d, int size, DllCallback callback);

        protected virtual double[] GetResults()
        {
            double[] ret = new double[matrix.GetLength(0)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = matrix[i, ret.Length];
            }
            return ret;
        }

        private long i;
        public void Callback()
        {
            i++;
        }

        public virtual double[] RunTest_Managed()
        {
            matrix = (double[,])matrix2.Clone();
            Stopwatch sw = Stopwatch.StartNew();
            {
                Call_Managed(matrix, matrix.GetLength(0), Callback);
            }
            sw.Stop();
            Console.WriteLine($"Managed: {sw.Elapsed}");
            return GetResults();
        }

        protected abstract void Call_Managed(double[,] doubles, int getLength, DllCallback callback);
    }
}