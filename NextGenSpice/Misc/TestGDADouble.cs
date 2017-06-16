using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace NextGenSpice
{
    public unsafe class TestGdaDouble : TestBaseDouble
    {
        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void Run_d_gda(double* matrix, int size, double* solutions, DllCallback callback);

        private int c;

        private double[] partial;
        protected double[] solutions;

        public TestGdaDouble(int size) : base(size)
        {
            solutions = new double[size];
        }

        protected override unsafe void Call_Native(double* d, int getLength, DllCallback callback)
        {
            solutions = new double[getLength];

            fixed (double* sol = solutions)
            { Run_d_gda(d, getLength, sol, callback); }
        }


        protected override double[] GetResults()
        {
            return solutions;
        }

        protected override void Call_Managed(double[,] doubles, int getLength, DllCallback callback)
        {
            solutions = new double[getLength];
            Managed(doubles, getLength, callback);
        }

        void Managed(double[,] m, int size, DllCallback callback)
        {
            partial = new double[size];
            var it = 0;
            do
            {
                UpdateResult(m, size);
//                Console.WriteLine(string.Join(" ", partial.Select(d => d.ToString())));
                UpdateSolutions(m, size);
                callback();
                ++it;
            } while (partial.Sum(d => d * d) > 0.00000001);
            Console.WriteLine($"Iterations: {it}");
        }

        private void UpdateResult(double[,] doubles, int size)
        {
            for (int i = 0; i < size; i++)
            {
                partial[i] = -doubles[i, size];
                for (int j = 0; j < size; j++)
                {
                    partial[i] += solutions[i] * doubles[i, j];
                }
            }
        }

        private void UpdateSolutions(double[,] doubles, int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    solutions[i] -= doubles[j, i] * partial[i] * 0.07;
                }
            }
        }
    }
}