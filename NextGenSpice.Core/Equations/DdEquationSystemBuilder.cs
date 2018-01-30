using System.Collections.Generic;
using Numerics;

namespace NextGenSpice.Core.Equations
{
    /// <summary>
    /// Class that is used to build equation system with dd_real coefficients.
    /// </summary>
    public class DdEquationSystemBuilder : IEquationSystemBuilder
    {
        private readonly List<List<dd_real>> matrix;
        readonly List<dd_real> rhs;

        public DdEquationSystemBuilder()
        {
            this.matrix = new List<List<dd_real>>();
            this.rhs = new List<dd_real>();
        }

        /// <summary>
        /// Adds a variable to the equation system. Returns the index of the variable.
        /// </summary>
        /// <returns></returns>
        public int AddVariable()
        {
            var newRow = new List<dd_real>();
            for (int i = 0; i < matrix.Count; i++)
            {
                matrix[i].Add(dd_real.Zero);
                newRow.Add(dd_real.Zero);
            }
            matrix.Add(newRow);
            newRow.Add(dd_real.Zero); // element on diagonal

            rhs.Add(dd_real.Zero);
            return rhs.Count - 1;
        }

        /// <summary>
        /// Count of the variables in the equation.
        /// </summary>
        public int VariablesCount => rhs.Count;

        /// <summary>
        /// Adds a value to coefficient on the given row and column of the equation matrix.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value to be added to the coefficients.</param>
        public void AddMatrixEntry(int row, int column, double value)
        {
            matrix[row][column] += value;
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
        /// Creates equation system with fixed number of variables.
        /// </summary>
        /// <returns></returns>
        public DdEquationSystem Build()
        {
            Array2DWrapper<dd_real> m = new Array2DWrapper<dd_real>(VariablesCount);


            for (int i = 0; i < VariablesCount; i++)
            for (int j = 0; j < VariablesCount; j++)
                m[i, j] = matrix[i][j];

            return new DdEquationSystem(m, rhs.ToArray());
        }
    }
}