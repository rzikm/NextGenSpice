using System;
using System.Collections;

namespace NextGenSpiceTests
{
    public partial class CircuitCalculationTests
    {
        class DoubleComparer : IComparer
        {
            private double epsilon;
            public DoubleComparer(double epsilon)
            {
                this.epsilon = epsilon;
            }
            public int Compare(double x, double y)
            {
                return Math.Abs(x - y) < epsilon ? 0 : Math.Sign(x - y);
            }

            public int Compare(object x, object y)
            {
                return Compare((double)x, (double)y);
            }
        }
    }
}
