using System.Runtime.InteropServices;
using NextGenSpice.Numerics;

namespace NextGenSpice
{
    public unsafe class TestCjGdaDouble : TestGdaDouble
    {

        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void Run_d_cjgda(double* matrix, int size, double* solutions, DllCallback callback);
        public TestCjGdaDouble(int size) : base(size)
        {
        }

        protected override unsafe void Call_Native(double* d, int getLength, DllCallback callback)
        {
            fixed(double* sol = solutions)
            Run_d_cjgda(d, getLength, sol, callback);
        }

    }

    public unsafe class TestCjGdaQd : TestGdaQd
    {

        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void Run_qd_cjgda(qd_real* matrix, int size, qd_real* solutions, DllCallback callback);
        public TestCjGdaQd(int size) : base(size)
        {
        }

        protected override unsafe void Call_Native(qd_real* d, int getLength, DllCallback callback)
        {
            fixed (qd_real* sol = solutions)
                Run_qd_cjgda(d, getLength, sol, callback);
        }

    }
}