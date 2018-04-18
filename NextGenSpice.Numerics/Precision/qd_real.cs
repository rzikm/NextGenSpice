namespace NextGenSpice.Numerics.Precision
{
#if qd_precision
    [StructLayout(LayoutKind.Sequential)]
    public struct qd_real : IConvertible, IComparable<qd_real>, IEquatable<qd_real>
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
            return lhs.CompareTo(Zero + rhs) < 0;
        }

        public static bool operator >(qd_real lhs, double rhs)
        {
            return lhs.CompareTo(Zero + rhs) > 0;
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


        [DllImport(Constants.DllPath, CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        private static extern void qd_add(ref qd_real self, ref qd_real b);

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

        [DllImport(Constants.DllPath, CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        private static extern void qd_sub(ref qd_real self, ref qd_real b);

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

        [DllImport(Constants.DllPath, CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        private static extern void qd_mul(ref qd_real self, ref qd_real b);

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

        [DllImport(Constants.DllPath, CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        private static extern void qd_div(ref qd_real self, ref qd_real b);

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
            var a = new qd_real();
            a.x0 = rhs;
            return a / lhs;
        }


        [DllImport(Constants.DllPath, CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        private static extern void qd_sqrt(ref qd_real self);

        public qd_real Sqrt()
        {
            var d = this;
            qd_sqrt(ref d);
            return d;
        }

        [DllImport(Constants.DllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        [SuppressUnmanagedCodeSecurity]
        private static extern string qd_to_string(ref qd_real self);

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
#endif
}