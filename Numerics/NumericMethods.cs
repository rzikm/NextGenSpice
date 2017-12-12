using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Numerics
{
    public static unsafe class NumericMethods
    {
        private const string DllPath = "D:\\Visual Studio 2017\\Projects\\NextGen Spice\\Debug\\NumericCore.dll";
//        private const string DllPath = "NumericCore.dll";

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern void gauss_solve_double(double* mat, double* b, uint size);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern void gauss_solve_qd(qd_real* mat, qd_real* b, uint size);

        [Conditional("DEBUG")]
        public static void PrintSystem(Array2DWrapper m, double[] b)
        {
            var size = m.SideLength;
            Trace.WriteLine("-----------------------------------------------------");

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    Trace.Write($"{m[i, j],10:G4} ");
                }

                Trace.WriteLine($"| {b[i],10:G4}");
            }

            Trace.WriteLine("-----------------------------------------------------");
        }

        [Conditional("DEBUG")]
        private static void PrintSolution(double[] b)
        {
            Trace.WriteLine($"Solution: {string.Join(" ", b.Select(d => d.ToString("F")))}");
        }

        public static void GaussElimSolve(Array2DWrapper m, double[] b, double[] x)
        {
            GaussElimSolve_Managed(m, b, x);
//            GaussElimSolve_Native_qd(m, b, x);
//                                    GaussElimSolve_Native(m, b, x);
        }

        public static void GaussElimSolve_qd(QdArray2DWrapper m, qd_real[] b, qd_real[] x)
        {
                        GaussElimSolve_Managed_qd(m, b, x);
//            GaussElimSolve_Native_qd2(m, b, x);
            //                                    GaussElimSolve_Native(m, b, x);
        }

        private static void GaussElimSolve_Managed_qd(QdArray2DWrapper m, qd_real[] b, qd_real[] x)
        {
            qd_real Abs(qd_real val)
            {
                return val > qd_real.Zero ? val : -val;
            }

            var size = m.SideLength;

//            PrintSystem(m, b);

            // we start from node 1, because 0 is the ground/reference (0V)
            for (var i = 1; i < size - 1; i++)
            {
                // Search for maximum in this column
                var maxEl =  Abs(m[i, i]);
                var maxRow = i;
                for (var k = i + 1; k < size; k++)
                    if (Abs(m[k, i]) > maxEl)
                    {
                        maxEl = Abs(m[k, i]);
                        maxRow = k;
                    }

                // Swap maximum row with current row (column by column)
                for (var k = i; k < size; k++)
                {
                    var tmp = m[maxRow, k];
                    m[maxRow, k] = m[i, k];
                    m[i, k] = tmp;
                }
                // swap in b vector
                {
                    var tmp = b[maxRow];
                    b[maxRow] = b[i];
                    b[i] = tmp;
                }

                //                PrintSystem(m, b);


                // eliminate current variable in all columns
                for (var k = i + 1; k < size; k++)
                {
                    var c = -m[k, i] / m[i, i];
                    for (var j = i; j < size; j++)
                        if (i == j)
                            m[k, j] = qd_real.Zero;
                        else
                            m[k, j] += c * m[i, j];
                    // b vector
                    b[k] += c * b[i];
                }

                //                PrintSystem(m, b);
            }


            // GaussElimSolve equation Ax=b for an upper triangular matrix A
            for (var i = size - 1; i > 0; i--)
            {
                if (b[i] == qd_real.Zero)
                    continue;
                // normalize
                b[i] /= m[i, i];
                //m[i, i] = 1;
                // backward elimination
                for (var k = i - 1; k >= 0; k--)
                    b[k] -= m[k, i] * b[i];
                //m[k, i] = 0;
            }

            b.CopyTo(x, 0);
            PrintSolution(b.Select(e => (double) e).ToArray());
        }

        private static unsafe void GaussElimSolve_Native_qd2(QdArray2DWrapper m, qd_real[] b, qd_real[] x)
        {
            fixed (qd_real* mat = m.RawData)
            fixed (qd_real* rhs = b)
            {
                gauss_solve_qd(mat, rhs, (uint) x.Length);
            }

            b.CopyTo(x, 0);
        }

        private static void GaussElimSolve_Native_qd(Array2DWrapper m, double[] b, double[] x)
        {
            PrintSystem(m, b);


            qd_real[] b2 = b.Select(d => new qd_real(d)).ToArray();
            qd_real[] mat = m.RawData.Select(d => new qd_real(d)).ToArray();
            qd_real[] sol = new qd_real[x.Length];

            fixed (qd_real* m_ = mat)
            fixed (qd_real* rhs = b2)
            {
                gauss_solve_qd(m_, rhs, (uint)x.Length);
            }

            for (var i = 0; i < b2.Length; i++)
            {
                x[i] = (double) b2[i];
            }
        }

        private static void GaussElimSolve_Managed(Array2DWrapper m, double[] b, double[] x)
        {
            var size = m.SideLength;

            PrintSystem(m, b);

            // we start from node 1, because 0 is the ground/reference (0V)
            for (var i = 1; i < size - 1; i++)
            {
                // Search for maximum in this column
                var maxEl = Math.Abs(m[i, i]);
                var maxRow = i;
                for (var k = i + 1; k < size; k++)
                    if (Math.Abs(m[k, i]) > maxEl)
                    {
                        maxEl = Math.Abs(m[k, i]);
                        maxRow = k;
                    }

                // Swap maximum row with current row (column by column)
                for (var k = i; k < size; k++)
                {
                    var tmp = m[maxRow, k];
                    m[maxRow, k] = m[i, k];
                    m[i, k] = tmp;
                }
                // swap in b vector
                {
                    var tmp = b[maxRow];
                    b[maxRow] = b[i];
                    b[i] = tmp;
                }

//                PrintSystem(m, b);


                // eliminate current variable in all columns
                for (var k = i + 1; k < size; k++)
                {
                    var c = -m[k, i] / m[i, i];
                    for (var j = i; j < size; j++)
                        if (i == j)
                            m[k, j] = 0;
                        else
                            m[k, j] += c * m[i, j];
                    // b vector
                    b[k] += c * b[i];
                }

//                PrintSystem(m, b);
            }


            // GaussElimSolve equation Ax=b for an upper triangular matrix A
            for (var i = size - 1; i > 0; i--)
            {
                if (b[i] == 0)
                    continue;
                // normalize
                b[i] /= m[i, i];
                //m[i, i] = 1;
                // backward elimination
                for (var k = i - 1; k >= 0; k--)
                    b[k] -= m[k, i] * b[i];
                //m[k, i] = 0;
            }

            b.CopyTo(x, 0);
            PrintSolution(b);
        }

        private static void GaussElimSolve_Native(Array2DWrapper m, double[] b, double[] x)
        {
            fixed (double* mat = m.RawData)
            fixed (double* rhs = b)
            {
                gauss_solve_double(mat, rhs, (uint)x.Length);
            }

            b.CopyTo(x, 0);
        }
    }
}