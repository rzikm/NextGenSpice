using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Helpers;
using NextGenSpice.Core.Numerics;

namespace NextGenSpice.Core.Equations
{
    public class EquationSystem : IEquationSystem
    {
        private readonly Array2DWrapper matrixBackup;
        private readonly double[] rhsBackup;
        private Array2DWrapper matrix;
        private double[] rhs;

        private ISet<ISet<int>> equivalences;

        public EquationSystem(Array2DWrapper matrix, double[] rhs)
        {
            if (matrix.SideLength != rhs.Length) throw new ArgumentException($"Matrix side length ({matrix.SideLength}) is different from right hand side vector length ({rhs.Length})");
            this.matrixBackup = matrix;
            this.rhsBackup = rhs;
            Solution = new double[rhs.Length];

            Clear();
        }

        public int VariablesCount => Solution.Length;

        public void AddMatrixEntry(int row, int column, double value)
        {
            matrix[row, column] += value;
        }

        public void AddRightHandSideEntry(int index, double value)
        {
            rhs[index] += value;
        }

        public void BindEquivalent(params int[] vars)
        {
            // check input
            if (vars.Max() >= rhs.Length || vars.Min() < 0) throw new ArgumentOutOfRangeException();

            var toMerge = equivalences.Where(e => e.Overlaps(vars)).ToList();
            equivalences.ExceptWith(toMerge);
            equivalences.Add(new HashSet<int>(toMerge.SelectMany(set => set)));
        }

        public double[] Solution { get; }

        public void Clear()
        {
            matrix = matrixBackup.Clone();
            rhs = (double[])rhsBackup.Clone();
            equivalences = new HashSet<ISet<int>>(Enumerable.Range(0, rhs.Length).Select(i => new HashSet<int>(new[] { i })));
        }

        public double GetMatrixEntry(int row, int column)
        {
            if (row < 0 || row >= rhs.Length) throw new ArgumentOutOfRangeException(nameof(row));
            if (column < 0 || column >= rhs.Length) throw new ArgumentOutOfRangeException(nameof(column));

            return matrix[row, column];
        }

        public double GetRightHandSideEntry(int row)
        {
            if (row < 0 || row >= rhs.Length) throw new ArgumentOutOfRangeException(nameof(row));

            return rhs[row];
        }

        public double[] Solve()
        {
            var m = matrix.Clone();
            var b = (double[])rhs.Clone();
            
            EnforceBoundNodeEquivalence(m, b);
            EnforceGroundHasZeroVoltage(m, b);

            NumericMethods.GaussElimSolve(m, b, Solution);

            //DistributeEquivalentVoltages(m);
            return Solution;
        }

//        private void DistributeEquivalentVoltages(Array2DWrapper m)
//        {
//            foreach (var grp in equivalences)
//            {
//                var representative = grp.First();
//                foreach (var other in grp.Skip(1))
//                {
//                    Solution[other] = Solution[representative];
//                }
//            }
//        }

        private void EnforceBoundNodeEquivalence(Array2DWrapper m, double[] b)
        {
            foreach (var grp in equivalences)
            {
                var representative = grp.First();
                foreach (var other in grp.Skip(1))
                {

                    b[representative] += b[other];
                    b[other] = 0;
                    for (int i = 0; i < m.SideLength; i++)
                    {
                        // move others into representative
                        m[representative, i] += m[other, i];
                        m[i, representative] += m[i, other];

                        m[other, i] = 0;
                        m[i, other] = 0;
                    }

                    // force equality
                    m[other, representative] = 1;
                    m[representative,representative] += m[representative, other];
                    m[representative, other] = 0;
                    m[other, other] = -1;
                }
            }
        }

        private static void EnforceGroundHasZeroVoltage(Array2DWrapper m, double[] b)
        {
            // workaround for grounding 0th node (Voltage = 0)
            for (int i = 0; i < b.Length; i++)
            {
                m[i, 0] = 0;
                m[0, i] = 0;
            }

            m[0, 0] = 1;
            b[0] = 0;
        }
    }
}