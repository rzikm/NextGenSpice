using System;
using System.Runtime.InteropServices;

namespace Numerics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct dd_real : IConvertible, IComparable<dd_real>, IEquatable<dd_real>
    {
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
            return obj is dd_real && Equals((dd_real) obj);
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
            return lhs.CompareTo(Zero + rhs) < 0;
        }

        public static bool operator >(dd_real lhs, double rhs)
        {
            return lhs.CompareTo(Zero + rhs) > 0;
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


        [DllImport(Constants.DllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern void dd_add(ref dd_real self, ref dd_real b);

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

        [DllImport(Constants.DllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern void dd_sub(ref dd_real self, ref dd_real b);

        public static dd_real operator -(dd_real lhs, dd_real rhs)
        {
            dd_sub(ref lhs, ref rhs);
            return lhs;
        }

        public static dd_real operator -(dd_real lhs, double rhs)
        {
            var dd = new dd_real {x0 = rhs};
            return lhs - dd;
        }

        public static dd_real operator -(double lhs, dd_real rhs)
        {
            var dd = new dd_real {x0 = lhs};
            return dd - rhs;
        }

        public static dd_real operator -(dd_real self)
        {
            return Zero - self;
        }

        [DllImport(Constants.DllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern void dd_mul(ref dd_real self, ref dd_real b);

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

        [DllImport(Constants.DllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern void dd_div(ref dd_real self, ref dd_real b);

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
            var a = new dd_real();
            a.x0 = rhs;
            return a / lhs;
        }


        [DllImport(Constants.DllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern void dd_sqrt(ref dd_real self);

        public dd_real Sqrt()
        {
            var d = this;
            dd_sqrt(ref d);
            return d;
        }

        [DllImport(Constants.DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        private static extern string dd_to_string(ref dd_real self);

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
            return (int) dd.x0;
        }

        public static explicit operator double(dd_real dd)
        {
            return dd.x0;
        }

        public static explicit operator float(dd_real dd)
        {
            return (float) dd.x0;
        }

        public string ComponentString()
        {
            return $"{x0} {x1}";
        }

        /// <summary>Returns the <see cref="T:System.TypeCode"></see> for this instance.</summary>
        /// <returns>
        ///     The enumerated constant that is the <see cref="T:System.TypeCode"></see> of the class or value type that
        ///     implements this interface.
        /// </returns>
        TypeCode IConvertible.GetTypeCode()
        {
            return x0.GetTypeCode();
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent Boolean value using the specified culture-specific
        ///     formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>A Boolean value equivalent to the value of this instance.</returns>
        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToBoolean(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified
        ///     culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>An 8-bit unsigned integer equivalent to the value of this instance.</returns>
        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToByte(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent Unicode character using the specified culture-specific
        ///     formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>A Unicode character equivalent to the value of this instance.</returns>
        char IConvertible.ToChar(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToChar(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent <see cref="T:System.DateTime"></see> using the specified
        ///     culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>A <see cref="T:System.DateTime"></see> instance equivalent to the value of this instance.</returns>
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToDateTime(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent <see cref="T:System.Decimal"></see> number using the
        ///     specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>A <see cref="T:System.Decimal"></see> number equivalent to the value of this instance.</returns>
        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToDecimal(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent double-precision floating-point number using the
        ///     specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>A double-precision floating-point number equivalent to the value of this instance.</returns>
        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToDouble(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent 16-bit signed integer using the specified
        ///     culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>An 16-bit signed integer equivalent to the value of this instance.</returns>
        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToInt16(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent 32-bit signed integer using the specified
        ///     culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToInt32(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent 64-bit signed integer using the specified
        ///     culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToInt64(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific
        ///     formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>An 8-bit signed integer equivalent to the value of this instance.</returns>
        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToSByte(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent single-precision floating-point number using the
        ///     specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>A single-precision floating-point number equivalent to the value of this instance.</returns>
        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToSingle(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent <see cref="T:System.String"></see> using the specified
        ///     culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>A <see cref="T:System.String"></see> instance equivalent to the value of this instance.</returns>
        string IConvertible.ToString(IFormatProvider provider)
        {
            return x0.ToString(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an <see cref="T:System.Object"></see> of the specified
        ///     <see cref="T:System.Type"></see> that has an equivalent value, using the specified culture-specific formatting
        ///     information.
        /// </summary>
        /// <param name="conversionType">The <see cref="T:System.Type"></see> to which the value of this instance is converted.</param>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>
        ///     An <see cref="T:System.Object"></see> instance of type
        ///     <paramref name="conversionType">conversionType</paramref> whose value is equivalent to the value of this instance.
        /// </returns>
        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible) x0).ToType(conversionType, provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified
        ///     culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>An 16-bit unsigned integer equivalent to the value of this instance.</returns>
        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToUInt16(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified
        ///     culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>An 32-bit unsigned integer equivalent to the value of this instance.</returns>
        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToUInt32(provider);
        }

        /// <summary>
        ///     Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified
        ///     culture-specific formatting information.
        /// </summary>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider"></see> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <returns>An 64-bit unsigned integer equivalent to the value of this instance.</returns>
        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible) x0).ToUInt64(provider);
        }
    }
}