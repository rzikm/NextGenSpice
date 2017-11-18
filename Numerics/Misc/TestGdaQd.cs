using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Numerics.Misc
{
    public unsafe class TestGdaQd : TestBaseQD
    {
        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void Run_qd_gda(qd_real* matrix, int size, qd_real* solutions, DllCallback callback);

        private int c;

        private qd_real[] partial;
        protected qd_real[] solutions;

        public TestGdaQd(int size) : base(size)
        {
            solutions = new qd_real[size];
        }

        protected override unsafe void Call_Native(qd_real* d, int getLength, DllCallback callback)
        {
            solutions = new qd_real[getLength];
            fixed (qd_real* sol = solutions)
            { Run_qd_gda(d, getLength, sol, callback); }
        }


        protected override qd_real[] GetResults()
        {
            return solutions;
        }

        protected override void Call_Managed(qd_real[,] doubles, int getLength, DllCallback callback)
        {
            solutions = new qd_real[getLength];
            Managed(doubles, getLength, callback);
        }

        void Managed(qd_real[,] m, int size, DllCallback callback)
        {
            partial = new qd_real[size];
            var it = 0;
            do
            {
                UpdateResult(m, size);
//                Console.WriteLine(string.Join(" ", partial.Select(d => d.ToString())));

                UpdateSolutions(m, size);
                callback();
                ++it;
            } while (partial.Aggregate(qd_real.Zero,(acc,d) => { return acc + d * d; }) > 0.00000001);
            Console.WriteLine($"Iterations: {it}");
        }

        private void UpdateSolutions(qd_real[,] doubles, int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    solutions[i] -= doubles[j, i] * partial[i] * 0.07;
                }
            }
        }

        private void UpdateResult(qd_real[,] doubles, int size)
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
    }
}