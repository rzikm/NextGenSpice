using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Helpers;
using Numerics;

namespace NextGenSpice.Core.Equations
{
    public class EquationSystem : IEquationSystem
    {
        private readonly Array2DWrapper matrixBackup;
        private readonly double[] rhsBackup;
        private Array2DWrapper matrix;
        private double[] rhs;
        
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
        public double[] Solution { get; }

        public Array2DWrapper Matrix => matrix;
        public double[] RightHandSide => rhs;

        public void Clear()
        {
            matrix = matrixBackup.Clone();
            rhs = (double[])rhsBackup.Clone();
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

    }
}