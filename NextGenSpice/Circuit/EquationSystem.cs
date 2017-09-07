using System;
using System.Diagnostics;
using System.Globalization;

namespace NextGenSpice.Circuit
{
    public class EquationSystem : IEquationSystem
    {
        private readonly Array2DWrapper matrixBackup;
        private readonly double[] rhsBackup;
        private Array2DWrapper matrix;
        private double[] rhs;

        public EquationSystem(Array2DWrapper matrix, double[] rhs)
        {
            if (matrix.SideLength != rhs.Length) throw new ArgumentException($"Matrix side length ({matrix.SideLength}) is different from right hand side vector length ({rhs.Length})");
            this.matrixBackup = matrix;
            this.rhsBackup = rhs;
            Solution = new double[rhs.Length];

            Clear();
        }

        public int VariablesCount => Solution.Length;

        public void AddMatrixEntry(int row, int column, double value)
        {
            matrix[row, column] += value;
            if (row != column)
                matrix[column, row] += value;
        }

        public void AddRightHandSideEntry(int index, double value)
        {
            rhs[index] += value;
        }

        public double[] Solution { get; }

        public void Clear()
        {
            matrix = matrixBackup.Clone();
            rhs = (double[])rhsBackup.Clone();
        }

        public double[] Solve()
        {
            PrintMatrix();

            var m = matrix.Clone();
            var b = (double[])rhs.Clone();

            // workaround for grounding 0th node (Voltage = 0)
            for (int i = 0; i < b.Length; i++)
            {
                m[i, 0] = 0;
                m[0, i] = 0;
            }

            m[0, 0] = 1;
            b[0] = 0;

            NumericMethods.GaussElimSolve(m, b, Solution);
            return Solution;
        }

        private void PrintMatrix()
        {
            Console.WriteLine("EquationSystem:");
            for (int i = 0; i < matrix.SideLength; i++)
            {
                for (int j = 0; j < matrix.SideLength; j++)
                {
                    Console.Write($"{matrix[i, j]:00.0000}\t");
                }

                Console.WriteLine($" | {rhs[i]:00.0000}");
            }
            Console.WriteLine();
        }
    }
}