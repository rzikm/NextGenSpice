using System;
using System.Collections.Generic;

#if dd_precision || qd_precision
using NextGenSpice.Numerics.Precision;
#endif

namespace NextGenSpice.Numerics.Equations
{

    /// <summary>Class that is used to build equation system with implementation specific coefficients.</summary>
    public class EquationSystemBuilder : IEquationSystemBuilder
    {
#if dd_precision
        private readonly List<List<dd_real>> matrix;
        private readonly List<dd_real> rhs;

        public EquationSystemBuilder()
        {
            matrix = new List<List<dd_real>>();
            rhs = new List<dd_real>();
        }

        /// <summary>Adds a variable to the equation system. Returns the index of the variable.</summary>
        /// <returns></returns>
        public int AddVariable()
        {
            var newRow = new List<dd_real>();
            for (var i = 0; i < matrix.Count; i++)
            {
                matrix[i].Add(dd_real.Zero);
                newRow.Add(dd_real.Zero);
            }

            matrix.Add(newRow);
            newRow.Add(dd_real.Zero); // device on diagonal

            rhs.Add(dd_real.Zero);
            return rhs.Count - 1;
        }
#elif qd_precision
        private readonly List<List<qd_real>> matrix;
        private readonly List<qd_real> rhs;

        public EquationSystemBuilder()
        {
            matrix = new List<List<qd_real>>();
            rhs = new List<qd_real>();
        }

        /// <summary>Aqds a variable to the equation system. Returns the index of the variable.</summary>
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
            newRow.Add(qd_real.Zero); // device on diagonal

            rhs.Add(qd_real.Zero);
            return rhs.Count - 1;
        }
#else
        private readonly List<List<double>> matrix;
        private readonly List<double> rhs;

        public EquationSystemBuilder()
        {
            matrix = new List<List<double>>();
            rhs = new List<double>();
        }

        /// <summary>Adds a variable to the equation system. Returns the index of the variable.</summary>
        /// <returns></returns>
        public int AddVariable()
        {
            var newRow = new List<double>();
            for (var i = 0; i < matrix.Count; i++)
            {
                matrix[i].Add(0);
                newRow.Add(0);
            }

            matrix.Add(newRow);
            newRow.Add(0); // device on diagonal

            rhs.Add(0);
            return rhs.Count - 1;
        }
#endif



        /// <summary>Count of the variables in the equation.</summary>
        public int VariablesCount => rhs.Count;

        /// <summary>Adds a value to coefficient on the given row and column of the equation matrix.</summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value to be added to the coefficients.</param>
        public void AddMatrixEntry(int row, int column, double value)
        {
#if DEBUG
            if (double.IsNaN(value)) throw new InvalidOperationException("Cannot insert NaN");
#endif
            matrix[row][column] += value;
        }

        /// <summary>Adds a value to coefficient on the given position of the right hand side of the equation matrix.</summary>
        /// <param name="index">Index of the position.</param>
        /// <param name="value">The value.</param>
        public void AddRightHandSideEntry(int index, double value)
        {
#if DEBUG
            if (double.IsNaN(value)) throw new InvalidOperationException("Cannot insert NaN");
#endif
            rhs[index] += value;
        }

        /// <summary>Creates equation system with fixed number of variables.</summary>
        /// <returns></returns>
        public EquationSystem Build()
        {
#if dd_precision
            var m = new Matrix<dd_real>(VariablesCount);
#elif qd_precision
            var m = new Matrix<qd_real>(VariablesCount);
#else
            var m = new Matrix<double>(VariablesCount);
#endif

            for (var i = 0; i < VariablesCount; i++)
                for (var j = 0; j < VariablesCount; j++)
                    m[i, j] = matrix[i][j];

            return new EquationSystem(m, rhs.ToArray());
        }
    }
}