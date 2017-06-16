using System;
using NextGenSpice.Numerics;

namespace NextGenSpice
{
    public class Misc
    {
        public static void HilbertMatrixStabilityTest()
        {
            const int testSize = 20;

            Console.WriteLine("Gauss-Elimination:");
            RunTest(new TestGeDouble(testSize), new TestQdGe(testSize));
            Console.WriteLine("\n\nGradient Descent:");
            RunTest(new TestGdaDouble(testSize), new TestGdaQd(testSize));
            Console.WriteLine("\n\nConjugate Gradient Descent:");
            RunTest(new TestCjGdaDouble(testSize), new TestCjGdaQd(testSize));
        }

        private static void RunTest(TestBaseDouble t, TestBaseQD tqd)
        {
            Console.WriteLine("Double:");
            var a = t.RunTest_Native();
            var am = t.RunTest_Managed();
            t.RunTest_Native();
            t.RunTest_Managed();

            Console.WriteLine("\nQuad-Double:");
            var b = tqd.RunTest_Native();
            var bm = tqd.RunTest_Managed();
            tqd.RunTest_Native();
            tqd.RunTest_Managed();

            var maxDiff_d = 0d;
            Console.WriteLine("\nResults double:");
            for (int i = 0; i < a.Length; i++)
            {
                maxDiff_d = Math.Max(maxDiff_d, Math.Abs(a[i] - 1));
//                Console.WriteLine($"{a[i]}\t{am[i]}");
            }
            Console.WriteLine($"Max difference: {maxDiff_d}");

            var maxDiff_qd = qd_real.Zero;
            Console.WriteLine("\n\nResults quad-double:");
            for (int i = 0; i < b.Length; i++)
            {
                var diff = (b[i] - 1).Abs();
                maxDiff_qd = diff > maxDiff_qd ? diff : maxDiff_qd;
//                Console.WriteLine($"{b[i]}\t{bm[i]}");
            }
            Console.WriteLine($"Max difference: {maxDiff_qd}");
        }
    }
}