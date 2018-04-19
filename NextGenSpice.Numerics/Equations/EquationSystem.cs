using System;

#if dd_precision || qd_precision
using NextGenSpice.Numerics.Precision;
#endif

namespace NextGenSpice.Numerics.Equations
{
    /// <summary>Class representing linear equation system with implementation specific precision.</summary>
    public class EquationSystem : IEquationEditor
    {
#if dd_precision
        private readonly (Matrix<dd_real> m, dd_real[] v)[] backup;

        private readonly dd_real[] solution;

        public EquationSystem(Matrix<dd_real> matrix, dd_real[] rhs, int backupDepth = 2)
        {
            // init backup space
            backup = new(Matrix<dd_real> m, dd_real[] v)[backupDepth];
            for (int i = 0; i < backupDepth; i++)
            {
                backup[i] = (matrix.Clone(), (dd_real[])rhs.Clone());
            }

            if (matrix.Size != rhs.Length)
                throw new ArgumentException(
                    $"Matrix side length ({matrix.Size}) is different from right hand side vector length ({rhs.Length})");
            Solution = new double[rhs.Length];
            solution = new dd_real[rhs.Length];

            Matrix = matrix;
            RightHandSide = rhs;
        }

        /// <summary>Matrix part of the equation system.</summary>
        public Matrix<dd_real> Matrix { get; private set; }

        /// <summary>Right hand side vector of the equation system.</summary>
        public dd_real[] RightHandSide { get; private set; }

        private void CopyData(Matrix<dd_real> msrc, Matrix<dd_real> mdest, dd_real[] rhssrc, dd_real[] rhsdest)
        {
            msrc.RawData.CopyTo(mdest.RawData, 0);
            rhssrc.CopyTo(rhsdest, 0);
        }

#elif qd_precision
        private readonly (Matrix<qd_real> m, qd_real[] v)[] backup;

        private readonly qd_real[] solution;

        public EquationSystem(Matrix<qd_real> matrix, qd_real[] rhs, int backupDepth = 2)
        {
            // init backup space
            backup = new (Matrix<qd_real> m, qd_real[] v)[backupDepth];
            for (int i = 0; i < backupDepth; i++)
            {
                backup[i] = (matrix.Clone(), (qd_real[]) rhs.Clone());
            }

            if (matrix.Size != rhs.Length)
                throw new ArgumentException(
                    $"Matrix side length ({matrix.Size}) is different from right hand side vector length ({rhs.Length})");
            Solution = new double[rhs.Length];
            solution = new qd_real[rhs.Length];

            Matrix = matrix;
            RightHandSide = rhs;
        }

        /// <summary>Matrix part of the equation system.</summary>
        public Matrix<qd_real> Matrix { get; private set; }

        /// <summary>Right hand side vector of the equation system.</summary>
        public qd_real[] RightHandSide { get; private set; }

        private void CopyData(Matrix<qd_real> msrc, Matrix<qd_real> mdest, qd_real[] rhssrc, qd_real[] rhsdest)
        {
            msrc.RawData.CopyTo(mdest.RawData, 0);
            rhssrc.CopyTo(rhsdest, 0);
        }
#else

        private readonly (Matrix<double> m, double[] v)[] backup;

        private readonly double[] solution;

        public EquationSystem(Matrix<double> matrix, double[] rhs, int backupDepth = 2)
        {
            // init backup space
            backup = new(Matrix<double> m, double[] v)[backupDepth];
            for (int i = 0; i < backupDepth; i++)
            {
                backup[i] = (matrix.Clone(), (double[])rhs.Clone());
            }

            if (matrix.Size != rhs.Length)
                throw new ArgumentException(
                    $"Matrix side length ({matrix.Size}) is different from right hand side vector length ({rhs.Length})");
            Solution = new double[rhs.Length];
            solution = new double[rhs.Length];

            Matrix = matrix;
            RightHandSide = rhs;
        }



        /// <summary>Matrix part of the equation system.</summary>
        public Matrix<double> Matrix { get; private set; }

        /// <summary>Right hand side vector of the equation system.</summary>
        public double[] RightHandSide { get; private set; }

        private void CopyData(Matrix<double> msrc, Matrix<double> mdest, double[] rhssrc, double[] rhsdest)
        {
            msrc.RawData.CopyTo(mdest.RawData, 0);
            rhssrc.CopyTo(rhsdest, 0);
        }
        
#endif

        /// <summary>Result of the latest call to the Solve() method.</summary>
        public double[] Solution { get; private set; }

        /// <summary>Count of the variables in the equation.</summary>
        public int VariablesCount => Solution.Length;

        /// <summary>Adds a value to coefficient on the given row and column of the equation matrix.</summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value to be added to the coefficients.</param>
        public void AddMatrixEntry(int row, int column, double value)
        {
#if DEBUG
            if (double.IsNaN(value)) throw new InvalidOperationException("Cannot insert NaN");
#endif
            Matrix[row, column] += value;
        }

        /// <summary>Adds a value to coefficient on the given position of the right hand side of the equation matrix.</summary>
        /// <param name="index">Index of the position.</param>
        /// <param name="value">The value.</param>
        public void AddRightHandSideEntry(int index, double value)
        {
#if DEBUG
            if (double.IsNaN(value)) throw new InvalidOperationException("Cannot insert NaN");
#endif
            RightHandSide[index] += value;
        }

        /// <summary>Creates a restore point for the equation system.</summary>
        public void Backup(int index)
        {
            var tup = backup[index];

            CopyData(Matrix, tup.Item1, RightHandSide, tup.Item2);
        }

        /// <summary>
        ///     Restores the equation system to the previous bacup or the state that it was when it was build by the equation
        ///     system builder.
        /// </summary>
        public void Restore(int index)
        {
            var tup = backup[index];

            CopyData(tup.Item1, Matrix, tup.Item2, RightHandSide);
        }



        /// <summary>Solves the linear equation system. If the system has no solution, the result is undefined.</summary>
        /// <returns></returns>
        public void Solve()
        {
            var m = Matrix;
            var b = RightHandSide;
            var x = solution;

            GaussJordanElimination.Solve(m, b, x);

            for (int i = 0; i < solution.Length; i++)
            {
                Solution[i] = (double) solution[i];
            }
        }
    }
}