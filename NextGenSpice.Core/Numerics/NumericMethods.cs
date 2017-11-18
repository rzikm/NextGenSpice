using System;
using System.Diagnostics;
using NextGenSpice.Core.Helpers;

namespace NextGenSpice.Core.Numerics
{
    public static class NumericMethods
    {
        [Conditional("DEBUG")]
        public static void PrintSystem(Array2DWrapper m, double[] b)
        {
            var size = m.SideLength;
            Trace.WriteLine("-----------------------------------------------------");

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Trace.Write($"{m[i,j] :F}");
                    Trace.Write("\t");
                }

                Trace.WriteLine($"|\t{b[i]}");
            }

            Trace.WriteLine("-----------------------------------------------------");
        }

        public static void GaussElimSolve(Array2DWrapper m, double[] b, double[] x)
        {
            var size = m.SideLength;

            PrintSystem(m,b);

            // we start from node 1, because 0 is the ground/reference (0V)
            for (int i = 1; i < size - 1; i++)
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
                for (int k = i; k < size; k++)
                {
                    double tmp = m[maxRow, k];
                    m[maxRow, k] = m[i, k];
                    m[i, k] = tmp;
                }
                // swap in b vector
                {
                    double tmp = b[maxRow];
                    b[maxRow] = b[i];
                    b[i] = tmp;
                }

//                PrintSystem(m, b);



                // eliminate current variable in all columns
                for (int k = i + 1; k < size; k++)
                {
                    double c = -m[k, i] / m[i, i];
                    for (int j = i; j < size; j++)
                    {
                        if (i == j)
                            m[k, j] = 0;
                        else
                            m[k, j] += c * m[i, j];
                    }
                    // b vector
                    b[k] += c * b[i];
                }

//                PrintSystem(m, b);

            }


            // GaussElimSolve equation Ax=b for an upper triangular matrix A
            for (int i = size - 1; i > 0; i--)
            {
                if (b[i] == 0)
                {
                    continue;
                }
                // normalize
                b[i] /= m[i, i];
                //m[i, i] = 1;
                // backward elimination
                for (int k = i - 1; k >= 0; k--)
                {
                    b[k] -= m[k, i] * b[i];
                    //m[k, i] = 0;
                }

            }

            b.CopyTo(x, 0);
        }
    }
}