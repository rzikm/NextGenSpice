using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using NextGenSpice.Numerics;
using NextGenSpice.Numerics.Equations;

namespace SandboxRunner
{
    [CoreJob]
    public class GaussianEliminationTests
    {
        [Params(20, 200)] public int N;


        private EquationSystem system;

        [GlobalSetup]
        public void Setup()
        {
            system = new EquationSystem(N);

        }


        [Benchmark(Baseline = true)]
        public void Managed()
        {
            GaussJordanElimination.Solve_Managed_double(system.Matrix, system.RightHandSide, system.Solution);
        }

        [Benchmark]
        public void Managed_Wrapped()
        {
            var m = new Matrix<d_managed>(N);
            var b = new d_managed[N];
            var x = new d_managed[N];

            var size = N;
            var zero = new d_managed(0);
            var one = new d_managed(1);

            d_managed Abs(d_managed val)
            {
                return val.val > 0 ? val : new d_managed(-val.val);
            }


            for (var i = 0; i < size - 1; i++)
            {
                // Search for maximum in this column
                var maxEl = Abs(m[i, i]);
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
                    var c = zero - m[k, i] / m[i, i];
                    for (var j = i; j < size; j++)
                        if (i == j)
                            m[k, j] = zero;
                        else
                            m[k, j] += c * m[i, j];
                    // b vector
                    b[k] += c * b[i];
                }

            }


            // Solve equation Ax=b for an upper triangular matrix A
            for (var i = size - 1; i >= 0; i--)
            {
                if (b[i] == zero)
                    continue;
                // normalize
                b[i] /= m[i, i];
                m[i, i] = one;
                // backward elimination
                for (var k = i - 1; k >= 0; k--)
                {
                    b[k] -= m[k, i] * b[i];
                    m[k, i] = zero;
                }

            }

            b.CopyTo(x, 0);
        }


        [Benchmark]
        public void Managed_Pinvoke()
        {
            var m = new Matrix<d_native>(N);
            var b = new d_native[N];
            var x = new d_native[N];

            var size = N;
            var zero = new d_native(0);
            var one = new d_native(1);

            d_native Abs(d_native val)
            {
                return val.val > 0 ? val : new d_native(-val.val);
            }


            for (var i = 0; i < size - 1; i++)
            {
                // Search for maximum in this column
                var maxEl = Abs(m[i, i]);
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
                    var c = zero - m[k, i] / m[i, i];
                    for (var j = i; j < size; j++)
                        if (i == j)
                            m[k, j] = zero;
                        else
                            m[k, j] += c * m[i, j];
                    // b vector
                    b[k] += c * b[i];
                }

            }


            // Solve equation Ax=b for an upper triangular matrix A
            for (var i = size - 1; i >= 0; i--)
            {
                if (b[i] == zero)
                    continue;
                // normalize
                b[i] /= m[i, i];
                m[i, i] = one;
                // backward elimination
                for (var k = i - 1; k >= 0; k--)
                {
                    b[k] -= m[k, i] * b[i];
                    m[k, i] = zero;
                }

            }

            b.CopyTo(x, 0);
        }


        [Benchmark]
        public void Native()
        {
            GaussJordanElimination.Solve_Native_double(system.Matrix, system.RightHandSide, system.Solution);
        }
    }
}