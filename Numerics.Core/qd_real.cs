using System.Runtime.InteropServices;

namespace Numerics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct qd_real
    {
        public qd_real(double x0 = 0, double x1 = 0, double x2 = 0, double x3 = 0)
        {
            this.x0 = x0;
            this.x1 = x1;
            this.x2 = x2;
            this.x3 = x3;
        }


        public bool Equals(qd_real other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is qd_real && Equals((qd_real) obj);
        }

        public static qd_real Zero => new qd_real();

        public double x0;
        public double x1;
        public double x2;
        public double x3;

        public static bool operator <(qd_real lhs, qd_real rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }

        public static bool operator <(qd_real lhs, double rhs)
        {
            return lhs.CompareTo(qd_real.Zero + rhs) < 0;
        }

        public static bool operator >(qd_real lhs, double rhs)
        {
            return lhs.CompareTo(qd_real.Zero + rhs) > 0;
        }

        public static bool operator >(qd_real lhs, qd_real rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        public int CompareTo(qd_real rhs)
        {
            int comp;
            if ((comp = x0.CompareTo(rhs.x0)) != 0) return comp;
            if ((comp = x1.CompareTo(rhs.x1)) != 0) return comp;
            if ((comp = x2.CompareTo(rhs.x2)) != 0) return comp;
            return x3.CompareTo(rhs.x3);
        }

        public static bool operator ==(qd_real lhs, qd_real rhs)
        {
            return lhs.CompareTo(rhs) == 0;
        }

        public static bool operator !=(qd_real lhs, qd_real rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return x0.GetHashCode();
        }


        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void qd_add(ref qd_real self, ref qd_real b);

        public static qd_real operator +(qd_real lhs, qd_real rhs)
        {
            qd_add(ref lhs, ref rhs);
            return lhs;
        }

        public static qd_real operator +(qd_real lhs, double rhs)
        {
            var qd = new qd_real();
            qd.x0 = rhs;
            return lhs + qd;
        }

        public static qd_real operator +(double rhs, qd_real lhs)
        {
            return lhs + rhs;
        }

        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void qd_sub(ref qd_real self, ref qd_real b);

        public static qd_real operator -(qd_real lhs, qd_real rhs)
        {
            qd_sub(ref lhs, ref rhs);
            return lhs;
        }

        public static qd_real operator -(qd_real lhs, double rhs)
        {
            var qd = new qd_real {x0 = rhs};
            return lhs - qd;
        }

        public static qd_real operator -(double lhs, qd_real rhs)
        {
            var qd = new qd_real {x0 = lhs};
            return qd - rhs;
        }

        public static qd_real operator -(qd_real self)
        {
            return Zero - self;
        }

        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void qd_mul(ref qd_real self, ref qd_real b);

        public static qd_real operator *(qd_real lhs, qd_real rhs)
        {
            qd_mul(ref lhs, ref rhs);
            return lhs;
        }

        public static qd_real operator *(qd_real lhs, double rhs)
        {
            var qd = new qd_real();
            qd.x0 = rhs;
            return lhs * qd;
        }

        public static qd_real operator *(double rhs, qd_real lhs)
        {
            return lhs * rhs;
        }

        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void qd_div(ref qd_real self, ref qd_real b);

        public static qd_real operator /(qd_real lhs, qd_real rhs)
        {
            qd_div(ref lhs, ref rhs);
            return lhs;
        }

        public static qd_real operator /(qd_real lhs, double rhs)
        {
            var qd = new qd_real();
            qd.x0 = rhs;
            return lhs / qd;
        }

        public static qd_real operator /(double rhs, qd_real lhs)
        {
            qd_real a = new qd_real();
            a.x0 = rhs;
            return a / lhs;
        }


        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void qd_sqrt(ref qd_real self);

        public qd_real Sqrt()
        {
            qd_real d = this;
            qd_sqrt(ref d);
            return d;
        }

        [DllImport("NumericCore.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        static extern string qd_to_string(ref qd_real self);

        public qd_real Abs()
        {
            if (x0 < 0)
                return -this;
            return this;
        }

        public override string ToString()
        {
            return qd_to_string(ref this);
        }

        public static explicit operator int(qd_real qd)
        {
            return (int) qd.x0;
        }

        public static explicit operator double(qd_real qd)
        {
            return qd.x0;
        }

        public static explicit operator float(qd_real qd)
        {
            return (float) qd.x0;
        }

        public string ComponentString()
        {
            return $"{x0} {x1} {x2} {x3}";
        }
    }
}