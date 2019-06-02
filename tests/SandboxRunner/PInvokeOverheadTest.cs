using System.Runtime.InteropServices;
using System.Security;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace SandboxRunner
{
	[CoreJob]
	//    [RPlotExporter, RankColumn]
	public class PInvokeOverheadTest
	{
		[Params(10000, 1000000)] public int N;

		[GlobalSetup]
		public void Setup()
		{
			//            data = new byte[N];
			//            new Random(42).NextBytes(data);
		}

		//                [Benchmark]
		//                public byte[] Sha256() => sha256.ComputeHash(data);

		//        [Benchmark]
		//        public byte[] Md5() => md5.ComputeHash(data);

		[Benchmark]
		public double TestSpeedNative()
		{
			d_native a = new d_native(1), b = new d_native(2), c = new d_native(3);
			var sum = new d_native(0);
			for (var i = 0; i < N; i++)
			{
				sum += a * b + c;
				a = b + c;
				b = c + a;
			}

			return sum.val;
		}

		//        [MethodImpl(MethodImplOptions.NoInlining)]
		[Benchmark]
		public double TestSpeedManaged_wrapped()
		{
			d_managed a = new d_managed(1), b = new d_managed(2), c = new d_managed(3);

			var sum = new d_managed(0);
			for (var i = 0; i < N; i++)
			{
				sum += a * b + c;
				a = b + c;
				b = c + a;
			}

			return sum.val;
		}

		//        [MethodImpl(MethodImplOptions.NoInlining)]
		[Benchmark(Baseline = true)]
		public double TestSpeedManaged()
		{
			double a = 1, b = 2, c = 3;
			double sum = 0;
			for (var i = 0; i < N; i++)
			{
				sum += a * b + c;
				a = b + c;
				b = c + a;
			}

			return sum;
		}
	}

	internal struct d_managed
	{
		public double val;

		public d_managed(double d)
		{
			val = d;
		}

		public static d_managed operator +(d_managed self, d_managed other)
		{
			return new d_managed(self.val + other.val);
		}

		public static d_managed operator -(d_managed self, d_managed other)
		{
			return new d_managed(self.val - other.val);
		}

		public static d_managed operator *(d_managed self, d_managed other)
		{
			return new d_managed(self.val * other.val);
		}

		public static bool operator <(d_managed self, d_managed other)
		{
			return self.val < other.val;
		}

		public static bool operator >(d_managed self, d_managed other)
		{
			return self.val > other.val;
		}

		public static bool operator ==(d_managed self, d_managed other)
		{
			return self.val == other.val;
		}

		public static bool operator !=(d_managed self, d_managed other)
		{
			return self.val != other.val;
		}

		public static d_managed operator /(d_managed self, d_managed other)
		{
			return new d_managed(self.val / other.val);
		}
	}

	internal struct d_native
	{
		public double val;

		public d_native(double d)
		{
			val = d;
		}

		[DllImport("D:\\Visual Studio 2017\\Projects\\NextGen Spice\\Release\\NextGenSpice.Numerics.Native.dll",
			CallingConvention = CallingConvention.StdCall)]
		[SuppressUnmanagedCodeSecurity]
		private static extern void d_add(ref d_native self, ref d_native val);

		public static d_native operator +(d_native self, d_native other)
		{
			d_add(ref self, ref other);
			return self;
		}

		[DllImport("D:\\Visual Studio 2017\\Projects\\NextGen Spice\\Release\\NextGenSpice.Numerics.Native.dll",
			CallingConvention = CallingConvention.StdCall)]
		[SuppressUnmanagedCodeSecurity]
		private static extern void d_mul(ref d_native self, ref d_native val);

		public static d_native operator *(d_native self, d_native other)
		{
			d_mul(ref self, ref other);
			return self;
		}

		[DllImport("D:\\Visual Studio 2017\\Projects\\NextGen Spice\\Release\\NextGenSpice.Numerics.Native.dll",
			CallingConvention = CallingConvention.StdCall)]
		[SuppressUnmanagedCodeSecurity]
		private static extern void d_sub(ref d_native self, ref d_native val);

		public static d_native operator -(d_native self, d_native other)
		{
			return new d_native(self.val - other.val);
		}

		public static bool operator <(d_native self, d_native other)
		{
			return self.val < other.val;
		}

		public static bool operator >(d_native self, d_native other)
		{
			return self.val > other.val;
		}

		public static bool operator ==(d_native self, d_native other)
		{
			return self.val == other.val;
		}

		public static bool operator !=(d_native self, d_native other)
		{
			return self.val != other.val;
		}

		[DllImport("D:\\Visual Studio 2017\\Projects\\NextGen Spice\\Release\\NextGenSpice.Numerics.Native.dll",
			CallingConvention = CallingConvention.StdCall)]
		[SuppressUnmanagedCodeSecurity]
		private static extern void d_div(ref d_native self, ref d_native val);

		public static d_native operator /(d_native self, d_native other)
		{
			return new d_native(self.val / other.val);
		}
	}
}