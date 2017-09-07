using System.Collections.Generic;

namespace NextGenSpice.Circuit
{
    class EquationSystemBuilder : IEquationSystemBuilder
    {
        private List<List<double>> matrix;
        List<double> rhs;

        public EquationSystemBuilder()
        {
            this.matrix = new List<List<double>>();
            this.rhs = new List<double>();
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

        public IEquationSystem Build()
        {
            Array2DWrapper m = new Array2DWrapper(VariablesCount);


            for (int i = 0; i < VariablesCount; i++)
            for (int j = 0; j < VariablesCount; j++)
                m[i, j] = matrix[i][j];

            return new EquationSystem(m, rhs.ToArray());
        }
    }
}