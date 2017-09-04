using System;
using System.Diagnostics;

namespace NextGenSpice.Circuit
{
    public class CircuitEquationSystem : ICircuitEquationSystem, IEquationSolver
    {
        private Array2DWrapper conductance;
        //        private Array2DWrapper conductance;
        private readonly double[] current;
        private readonly double[] voltage;
        public CircuitEquationSystem(int count)
        {
            conductance = new Array2DWrapper(count);
            current = new double[count];
            voltage = new double[count];
        }

        public void AddConductance(int sourceId, int targetId, double value)
        {
            conductance[sourceId, targetId] += value;
            if (sourceId != targetId)
                conductance[targetId, sourceId] += value;
        }

        public void AddCurrent(int nodeId, double value)
        {
            current[nodeId] += value;
        }

        public void MergeNodes(int n1, int n2)
        {
            throw new System.NotImplementedException();
        }

        public double[] NodeVoltages => voltage;

        public void Clear()
        {
            for (var i = 0; i < conductance.RawData.Length; i++)
            {
                conductance.RawData[i] = 0;
            }
            for (var i = 0; i < current.Length; i++)
            {
                current[i] = 0;
            }
        }

        public void Solve()
        {
            UpdateValues();

            PrintMatrix();
        }

        private void PrintMatrix()
        {
            Console.WriteLine("EquationSystem:");
            for (int i = 0; i < conductance.SizeLength; i++)
            {
                for (int j = 0; j < conductance.SizeLength; j++)
                {
                    Console.Write($"{conductance[i, j]:00.0000}\t");
                }

                Console.WriteLine($" | {current[i]:00.0000}");
            }
            Console.WriteLine();
        }

        public void UpdateValues()
        {
            var m = conductance.Clone();
            var b = (double[])current.Clone();
            var size = m.SizeLength;

            // we start from node 1, because 0 is the ground/reference (0V)
            for (int i = 1; i < size; i++)
            {
                // Search for maximum in this column
                double maxEl = Math.Abs(m[i, i]);
                int maxRow = i;
                for (int k = i + 1; k < size; k++)
                {
                    if (Math.Abs(m[k, i]) > maxEl)
                    {
                        maxEl = Math.Abs(m[k, i]);
                        maxRow = k;
                    }
                }

                // Swap maximum row with current row (column by column)
                for (int k = i; k < size; k++)
                {
                    double tmp = m[maxRow, k];
                    m[maxRow, k] = m[i, k];
                    m[i, k] = tmp;
                }
                // swap in b vector
                {
                    double tmp = b[maxRow];
                    b[maxRow] = b[i];
                    b[i] = tmp;
                }


                // eliminate current variable in all columns
                for (int k = i + 1; k < size; k++)
                {
                    double c = -m[k, i] / m[i, i];
                    for (int j = i; j < size; j++)
                    {
                        if (i == j)
                            m[k, j] = 0;
                        else
                            m[k, j] += c * m[i, j];
                    }
                    // b vector
                    b[k] += c * b[i];
                }
            }

            // Solve equation Ax=b for an upper triangular matrix A
            for (int i = size - 1; i >= 1; i--)
            {
                // normalize
                b[i] /= m[i, i];

                // backward elimination
                for (int k = i - 1; k >= 0; k--)
                    b[k] -= m[k, i] * b[i];
            }

            b.CopyTo(voltage, 0);
            voltage[0] = 0;
        }
    }
}