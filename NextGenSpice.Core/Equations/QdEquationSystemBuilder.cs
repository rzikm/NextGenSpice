using System.Collections.Generic;
using Numerics;

namespace NextGenSpice.Core.Equations
{
    /// <summary>
    ///     Class that is used to build equation system with qd_real coefficients.
    /// </summary>
    public class QdEquationSystemBuilder : IEquationSystemBuilder
    {
        private readonly List<List<qd_real>> matrix;
        private readonly List<qd_real> rhs;

        public QdEquationSystemBuilder()
        {
            matrix = new List<List<qd_real>>();
            rhs = new List<qd_real>();
        }

        /// <summary>
        ///     Adds a variable to the equation system. Returns the index of the variable.
        /// </summary>
        /// <returns></returns>
        public int AddVariable()
        {
            var newRow = new List<qd_real>();
            for (var i = 0; i < matrix.Count; i++)
            {
                matrix[i].Add(qd_real.Zero);
                newRow.Add(qd_real.Zero);
            }
            matrix.Add(newRow);
            newRow.Add(qd_real.Zero); // element on diagonal

            rhs.Add(qd_real.Zero);
            return rhs.Count - 1;
        }

        /// <summary>
        ///     Count of the variables in the equation.
        /// </summary>
        public int VariablesCount => rhs.Count;

        /// <summary>
        ///     Adds a value to coefficient on the given row and column of the equation matrix.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value to be added to the coefficients.</param>
        public void AddMatrixEntry(int row, int column, double value)
        {
            matrix[row][column] += value;
        }

        /// <summary>
        ///     Adds a value to coefficient on the given position of the right hand side of the equation matrix.
        /// </summary>
        /// <param name="index">Index of the position.</param>
        /// <param name="value">The value.</param>
        public void AddRightHandSideEntry(int index, double value)
        {
            rhs[index] += value;
        }

        /// <summary>
        ///     Creates equation system with fixed number of variables.
        /// </summary>
        /// <returns></returns>
        public QdEquationSystem Build()
        {
            var m = new Array2DWrapper<qd_real>(VariablesCount);


            for (var i = 0; i < VariablesCount; i++)
            for (var j = 0; j < VariablesCount; j++)
                m[i, j] = matrix[i][j];

            return new QdEquationSystem(m, rhs.ToArray());
        }
    }
}