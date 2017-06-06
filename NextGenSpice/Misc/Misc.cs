using System;
using NextGenSpice.Numerics;

namespace NextGenSpice
{
    public class Misc
    {
        public static void HilbertMatrixStabilityTest()
        {
            const int testSize = 500;
            Test t = new Test(testSize);
            TestQD tqd = new TestQD(testSize);

            Console.WriteLine("Double:");
            t.RunTest_Native();
            t.RunTest_Managed();
            var a = t.RunTest_Native();
            t.RunTest_Managed();

            Console.WriteLine("\nQuad-Double:");
            tqd.RunTest_Native();
            tqd.RunTest_Managed();
            var b = tqd.RunTest_Native();
            tqd.RunTest_Managed();

            var maxDiff_d = 0d;
            Console.WriteLine("\nResults double:");
            for (int i = 0; i < b.Length; i++)
            {
                maxDiff_d = Math.Max(maxDiff_d, Math.Abs(a[i] - 1));
//                Console.WriteLine(a[i]);
            }
            Console.WriteLine($"Max difference: {maxDiff_d}");

            var maxDiff_qd = qd_real.Zero;
            Console.WriteLine("\n\nResults quad-double:");
            for (int i = 0; i < b.Length; i++)
            {
                var diff = (b[i] - 1).Abs();
                maxDiff_qd = diff > maxDiff_qd ? diff : maxDiff_qd;
//                Console.WriteLine(b[i]);
            }
            Console.WriteLine($"Max difference: {maxDiff_qd}");
        }
    }
}