using System.Collections.Generic;
using System.Linq;
using Numerics;

namespace NextGenSpice.Core.Equations
{
    public class QdEquationSystemBuilder : IEquationSystemBuilder
    {
        private readonly List<List<qd_real>> matrix;
        readonly List<qd_real> rhs;
        private ISet<ISet<int>> equivalences;

        public QdEquationSystemBuilder()
        {
            this.matrix = new List<List<qd_real>>();
            this.rhs = new List<qd_real>();
            equivalences = new HashSet<ISet<int>>();
        }

        public int AddVariable()
        {
            var newRow = new List<qd_real>();
            for (int i = 0; i < matrix.Count; i++)
            {
                matrix[i].Add(qd_real.Zero);
                newRow.Add(qd_real.Zero);
            }
            matrix.Add(newRow);
            newRow.Add(qd_real.Zero); // element on diagonal

            rhs.Add(qd_real.Zero);
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
            Array2DWrapper<qd_real> m = new Array2DWrapper<qd_real>(VariablesCount);


            for (int i = 0; i < VariablesCount; i++)
            for (int j = 0; j < VariablesCount; j++)
                m[i, j] = matrix[i][j];

            return new QdEquationSystem(m, rhs.ToArray());
        }
    }
}