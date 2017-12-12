using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Helpers;
using Numerics;

namespace NextGenSpice.Core.Equations
{
    public class EquationSystemBuilder : IEquationSystemBuilder
    {
        private readonly List<List<double>> matrix;
        readonly List<double> rhs;
        private ISet<ISet<int>> equivalences;

        public EquationSystemBuilder()
        {
            this.matrix = new List<List<double>>();
            this.rhs = new List<double>();
            equivalences = new HashSet<ISet<int>>();
        }

        public int AddVariable()
        {
            var newRow = new List<double>();
            for (int i = 0; i < matrix.Count; i++)
            {
                matrix[i].Add(0);
                newRow.Add(0);
            }
            matrix.Add(newRow);
            newRow.Add(0); // element on diagonal

            rhs.Add(0);
            return rhs.Count - 1;
        }

        public int VariablesCount => rhs.Count;

        public void AddMatrixEntry(int row, int column, double value)
        {
            matrix[row][column] += value;
        }

        public void AddRightHandSideEntry(int index, double value)
        {
            rhs[index] += value;
        }
       
        public EquationSystem Build()
        {
            Array2DWrapper m = new Array2DWrapper(VariablesCount);


            for (int i = 0; i < VariablesCount; i++)
            for (int j = 0; j < VariablesCount; j++)
                m[i, j] = matrix[i][j];

            return new EquationSystem(m, rhs.ToArray());
        }
    }

    public class QdEquationSystemBuilder : IEquationSystemBuilder
    {
        private readonly List<List<double>> matrix;
        readonly List<double> rhs;
        private ISet<ISet<int>> equivalences;

        public QdEquationSystemBuilder()
        {
            this.matrix = new List<List<double>>();
            this.rhs = new List<double>();
            equivalences = new HashSet<ISet<int>>();
        }

        public int AddVariable()
        {
            var newRow = new List<double>();
            for (int i = 0; i < matrix.Count; i++)
            {
                matrix[i].Add(0);
                newRow.Add(0);
            }
            matrix.Add(newRow);
            newRow.Add(0); // element on diagonal

            rhs.Add(0);
            return rhs.Count - 1;
        }

        public int VariablesCount => rhs.Count;

        public void AddMatrixEntry(int row, int column, double value)
        {
            matrix[row][column] += value;
        }

        public void AddRightHandSideEntry(int index, double value)
        {
            rhs[index] += value;
        }

        public QdEquationSystem Build()
        {
            QdArray2DWrapper m = new QdArray2DWrapper(VariablesCount);


            for (int i = 0; i < VariablesCount; i++)
            for (int j = 0; j < VariablesCount; j++)
                m[i, j] = new qd_real(matrix[i][j]);

            return new QdEquationSystem(m, rhs.Select(d => new qd_real(d)).ToArray());
        }
    }
}