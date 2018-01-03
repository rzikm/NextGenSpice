using System;
using System.Collections.Generic;
using System.Linq;
using Numerics;

namespace NextGenSpice.Core.Equations
{
    public class QdEquationSystem : IEquationSystem
    {
        private readonly Stack<Tuple<QdArray2DWrapper, qd_real[]>> backups;

        private readonly QdArray2DWrapper matrixBackup;
        private readonly qd_real[] rhsBackup;
        private QdArray2DWrapper matrix;
        private qd_real[] rhs;

        public QdEquationSystem(QdArray2DWrapper matrix, qd_real[] rhs)
        {
            if (matrix.SideLength != rhs.Length) throw new ArgumentException($"Matrix side length ({matrix.SideLength}) is different from right hand side vector length ({rhs.Length})");
            this.matrixBackup = matrix;
            this.rhsBackup = rhs;
            Solution = new double[rhs.Length];

            backups = new Stack<Tuple<QdArray2DWrapper, qd_real[]>>();

            backups.Push(Tuple.Create(matrix, rhs));
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
        public double[] Solution { get; private set; }

        public QdArray2DWrapper Matrix => matrix;
        public qd_real[] RightHandSide => rhs;

        public void Clear()
        {
            while (backups.Count > 1) backups.Pop();

            var tup = backups.Peek();

            matrix = tup.Item1.Clone();
            rhs = (qd_real[])tup.Item2.Clone();
        }

        public void Backup()
        {
            backups.Push(Tuple.Create(matrix.Clone(), (qd_real[])rhs.Clone()));
        }

        public void Restore()
        {
            var tup = backups.Peek();

            matrix = tup.Item1.Clone();
            rhs = (qd_real[])tup.Item2.Clone();
        }

        public qd_real GetMatrixEntry(int row, int column)
        {
            if (row < 0 || row >= rhs.Length) throw new ArgumentOutOfRangeException(nameof(row));
            if (column < 0 || column >= rhs.Length) throw new ArgumentOutOfRangeException(nameof(column));

            return matrix[row, column];
        }

        public qd_real GetRightHandSideEntry(int row)
        {
            if (row < 0 || row >= rhs.Length) throw new ArgumentOutOfRangeException(nameof(row));

            return rhs[row];
        }

        public double[] Solve()
        {
            var m = matrix.Clone();
            var b = (qd_real[])rhs.Clone();
            var x = new qd_real[b.Length];

            NumericMethods.GaussElimSolve_qd(m, b, x);

            //DistributeEquivalentVoltages(m);
            return Solution = x.Select(qd => (double) qd).ToArray();
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