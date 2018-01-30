using System;
using System.Collections.Generic;
using NextGenSpice.Core.Helpers;
using Numerics;

namespace NextGenSpice.Core.Equations
{
    /// <summary>
    /// Class representing linear equation system with inner double coeffitient precision.
    /// </summary>
    public class EquationSystem : IEquationEditor
    {
        private readonly Stack<Tuple<Array2DWrapper<double>, double[]>> backups;

        private Array2DWrapper<double> matrix;
        private double[] rhs;
        
        public EquationSystem(Array2DWrapper<double> matrix, double[] rhs)
        {
            if (matrix.SideLength != rhs.Length) throw new ArgumentException($"Matrix side length ({matrix.SideLength}) is different from right hand side vector length ({rhs.Length})");
            Solution = new double[rhs.Length];

            backups = new Stack<Tuple<Array2DWrapper<double>, double[]>>();

            backups.Push(Tuple.Create(matrix, rhs));
            Clear();
        }

        /// <summary>
        /// Count of the variables in the equation.
        /// </summary>
        public int VariablesCount => Solution.Length;

        /// <summary>
        /// Adds a value to coefficient on the given row and column of the equation matrix.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value to be added to the coefficients.</param>
        public void AddMatrixEntry(int row, int column, double value)
        {
            matrix[row, column] += value;
        }

        /// <summary>
        /// Adds a value to coefficient on the given position of the right hand side of the equation matrix.
        /// </summary>
        /// <param name="index">Index of the position.</param>
        /// <param name="value">The value.</param>
        public void AddRightHandSideEntry(int index, double value)
        {
            rhs[index] += value;
        }

        /// <summary>
        /// Result of the latest call to the Solve() method.
        /// </summary>
        public double[] Solution { get; }

        /// <summary>
        /// Matrix part of the equation system.
        /// </summary>
        public Array2DWrapper<double> Matrix => matrix;

        /// <summary>
        /// Right hand side vector of the equation system.
        /// </summary>
        public double[] RightHandSide => rhs;

        /// <summary>
        /// Restores the equation system to the state that it was when it was build by the equation system builder.
        /// </summary>
        public void Clear()
        {
            while (backups.Count > 1) backups.Pop();

            var tup = backups.Peek();

            matrix = tup.Item1.Clone();
            rhs = (double[])tup.Item2.Clone();
        }

        /// <summary>
        /// Creates a restore point for the equation system.
        /// </summary>
        public void Backup()
        {
            backups.Push(Tuple.Create(matrix.Clone(), (double[])rhs.Clone()));
        }

        /// <summary>
        /// Restores the equation system to the previous bacup or the state that it was when it was build by the equation system builder.
        /// </summary>
        public void Restore()
        {
            var tup = backups.Peek();

            matrix = tup.Item1.Clone();
            rhs = (double[])tup.Item2.Clone();
        }

        /// <summary>
        /// Solves the linear equation system. If the system has no solution, the result is undefined.
        /// </summary>
        /// <returns></returns>
        public double[] Solve()
        {
            var m = matrix.Clone();
            var b = (double[])rhs.Clone();
            

            NumericMethods.GaussElimSolve(m, b, Solution);

            return Solution;
        }
    }
}