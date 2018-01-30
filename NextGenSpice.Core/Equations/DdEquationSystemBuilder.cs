using System.Collections.Generic;
using Numerics;

namespace NextGenSpice.Core.Equations
{
    public class DdEquationSystemBuilder : IEquationSystemBuilder
    {
        private readonly List<List<dd_real>> matrix;
        readonly List<dd_real> rhs;
        private ISet<ISet<int>> equivalences;

        public DdEquationSystemBuilder()
        {
            this.matrix = new List<List<dd_real>>();
            this.rhs = new List<dd_real>();
            equivalences = new HashSet<ISet<int>>();
        }

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

        public int VariablesCount => rhs.Count;

        public void AddMatrixEntry(int row, int column, double value)
        {
            matrix[row][column] += value;
        }

        public void AddRightHandSideEntry(int index, double value)
        {
            rhs[index] += value;
        }

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