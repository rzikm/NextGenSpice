using System;
using NextGenSpice.Circuit;
using NextGenSpice.Helpers;

namespace NextGenSpice.Numerics
{
    public static class NumericMethods
    {
        public static void GaussElimSolve(Array2DWrapper m, double[] b, double[] x)
        {
            var size = m.SideLength;

            // we start from node 1, because 0 is the ground/reference (0V)
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
            }

            // GaussElimSolve equation Ax=b for an upper triangular matrix A
            for (int i = size - 1; i >= 0; i--)
            {
                // normalize
                b[i] /= m[i, i];

                // backward elimination
                for (int k = i - 1; k >= 0; k--)
                    b[k] -= m[k, i] * b[i];
            }

            b.CopyTo(x, 0);
        }
    }
}