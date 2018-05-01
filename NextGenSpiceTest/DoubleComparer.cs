using System;
using System.Collections.Generic;

namespace NextGenSpice.Test
{
    internal class DoubleComparer : IEqualityComparer<double>
    {
        private readonly double epsilon;

        public DoubleComparer(double epsilon)
        {
            this.epsilon = epsilon;
        }

        public bool Equals(double x, double y)
        {
            return Math.Abs(x - y) < epsilon;
        }

        public int GetHashCode(double obj)
        {
            return obj.GetHashCode();
        }

        public int Compare(double x, double y)
        {
            return Math.Abs(x - y) < epsilon ? 0 : Math.Sign(x - y);
        }

        public int Compare(object x, object y)
        {
            return Compare((double) x, (double) y);
        }
    }
}