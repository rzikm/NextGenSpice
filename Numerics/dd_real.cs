using System.Runtime.InteropServices;

namespace Numerics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct dd_real
    {
        //        private const string DllPath = "NumericCore.dll";
        private const string DllPath = "D:\\Visual Studio 2017\\Projects\\NextGen Spice\\Debug\\NumericCore.dll";

        public dd_real(double x0 = 0, double x1 = 0)
        {
            this.x0 = x0;
            this.x1 = x1;
        }


        public bool Equals(dd_real other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is dd_real && Equals((dd_real)obj);
        }

        public static dd_real Zero => new dd_real();

        public double x0;
        public double x1;

        public static bool operator <(dd_real lhs, dd_real rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }

        public static bool operator <(dd_real lhs, double rhs)
        {
            return lhs.CompareTo(dd_real.Zero + rhs) < 0;
        }

        public static bool operator >(dd_real lhs, double rhs)
        {
            return lhs.CompareTo(dd_real.Zero + rhs) > 0;
        }

        public static bool operator >(dd_real lhs, dd_real rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        public int CompareTo(dd_real rhs)
        {
            int comp;
            if ((comp = x0.CompareTo(rhs.x0)) != 0) return comp;
            return x1.CompareTo(rhs.x1);
        }

        public static bool operator ==(dd_real lhs, dd_real rhs)
        {
            return lhs.CompareTo(rhs) == 0;
        }

        public static bool operator !=(dd_real lhs, dd_real rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return x0.GetHashCode();
        }


        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void dd_add(ref dd_real self, ref dd_real b);

        public static dd_real operator +(dd_real lhs, dd_real rhs)
        {
            dd_add(ref lhs, ref rhs);
            return lhs;
        }

        public static dd_real operator +(dd_real lhs, double rhs)
        {
            var dd = new dd_real();
            dd.x0 = rhs;
            return lhs + dd;
        }

        public static dd_real operator +(double rhs, dd_real lhs)
        {
            return lhs + rhs;
        }

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void dd_sub(ref dd_real self, ref dd_real b);

        public static dd_real operator -(dd_real lhs, dd_real rhs)
        {
            dd_sub(ref lhs, ref rhs);
            return lhs;
        }

        public static dd_real operator -(dd_real lhs, double rhs)
        {
            var dd = new dd_real { x0 = rhs };
            return lhs - dd;
        }

        public static dd_real operator -(double lhs, dd_real rhs)
        {
            var dd = new dd_real { x0 = lhs };
            return dd - rhs;
        }

        public static dd_real operator -(dd_real self)
        {
            return Zero - self;
        }

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void dd_mul(ref dd_real self, ref dd_real b);

        public static dd_real operator *(dd_real lhs, dd_real rhs)
        {
            dd_mul(ref lhs, ref rhs);
            return lhs;
        }

        public static dd_real operator *(dd_real lhs, double rhs)
        {
            var dd = new dd_real();
            dd.x0 = rhs;
            return lhs * dd;
        }

        public static dd_real operator *(double rhs, dd_real lhs)
        {
            return lhs * rhs;
        }

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void dd_div(ref dd_real self, ref dd_real b);

        public static dd_real operator /(dd_real lhs, dd_real rhs)
        {
            dd_div(ref lhs, ref rhs);
            return lhs;
        }

        public static dd_real operator /(dd_real lhs, double rhs)
        {
            var dd = new dd_real();
            dd.x0 = rhs;
            return lhs / dd;
        }

        public static dd_real operator /(double rhs, dd_real lhs)
        {
            dd_real a = new dd_real();
            a.x0 = rhs;
            return a / lhs;
        }


        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void dd_sqrt(ref dd_real self);

        public dd_real Sqrt()
        {
            dd_real d = this;
            dd_sqrt(ref d);
            return d;
        }

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        static extern string dd_to_string(ref dd_real self);

        public dd_real Abs()
        {
            if (x0 < 0)
                return -this;
            return this;
        }

        public override string ToString()
        {
            return dd_to_string(ref this);
        }

        public static explicit operator int(dd_real dd)
        {
            return (int)dd.x0;
        }

        public static explicit operator double(dd_real dd)
        {
            return dd.x0;
        }

        public static explicit operator float(dd_real dd)
        {
            return (float)dd.x0;
        }

        public string ComponentString()
        {
            return $"{x0} {x1}";
        }
    }
}