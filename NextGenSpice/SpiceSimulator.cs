using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextGenSpice.Circuit;

namespace NextGenSpice
{
    public class SpiceSimulator
    {
        public static void TestSimple()
        {
           
        }
        public static void TestRun()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());

            const int testSize = 50;

            RunTests(new TestGeDouble(testSize), new TestQdGe(testSize));
            RunTests(new TestGdaDouble(testSize), new TestQdGe(testSize));
        }

        private static void RunTests(TestBaseDouble t, TestBaseQD tqd)
        {
            Console.WriteLine("Double:");
            t.RunTest_Native();
            t.RunTest_Managed();
            t.RunTest_Native();
            t.RunTest_Managed();

            Console.WriteLine("Quad-Double:");
            tqd.RunTest_Native();
            tqd.RunTest_Managed();
            tqd.RunTest_Native();
            tqd.RunTest_Managed();
        }
    }
}
