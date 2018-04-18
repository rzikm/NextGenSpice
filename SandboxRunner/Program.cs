using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Extensions;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using NextGenSpiceTest;

namespace SandboxRunner
{
    internal class Program
    {
        private const string projectPath = @"D:\Visual Studio 2017\Projects\NextGen Spice\";

        private static void Main(string[] args)
        {
            PrintFileSizes(); return;

//            TestSimulationSpeed(); return;
            //            IntegrationTest.Run();

            //            SetListeners();
            var sw = Stopwatch.StartNew();
//            RunModel();
            //
            RunTruncationModel();
            sw.Stop();
//            Console.WriteLine(sw.Elapsed);
        }

        private static void TestSimulationSpeed()
        {
            // dry run
            TestSpeedManaged(1,2,3);
            TestSpeedNative(new d_native(1), new d_native(2), new d_native(3));
            TestSpeedManaged_wrapped(new d_managed(1), new d_managed(2), new d_managed(3));


            Console.WriteLine($"Managed - double:\t{TestSpeedManaged(1,2,3)}");
            Console.WriteLine($"Managed - wrapper:\t{TestSpeedManaged_wrapped(new d_managed(1), new d_managed(2), new d_managed(3))}");
            Console.WriteLine($"Native - PInvoke:\t{TestSpeedNative(new d_native(1), new d_native(2), new d_native(3))}");
//            Console.WriteLine($"Native:\t{TestSpeedNative(new d_native(1), new d_native(2), new d_native(3))}");
        }

        private const int reps = 10000000;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static double TestSpeedNative(d_native a, d_native b, d_native c)
        {
            d_native sum = new d_native(0);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < reps; i++)
            {
                sum += a * b + c;
                a = b + c;
                b = c + a;
            }
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static double TestSpeedManaged_wrapped(d_managed a, d_managed b, d_managed c)
        {
            d_managed sum = new d_managed(0);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < reps; i++)
            {
                sum += a * b + c;
                a = b + c;
                b = c + a;
            }
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static double TestSpeedManaged(double a, double b, double c)
        {
            double sum = 0;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < reps; i++)
            {
                sum += a * b + c;
                a = b + c;
                b = c + a;
            }
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        struct d_managed
        {
            private double val;

            public d_managed(double d)
            {
                val = d;
            }

            public static d_managed operator +(d_managed self, d_managed other)
            {
                return new d_managed(self.val + other.val);
            }

            public static d_managed operator *(d_managed self, d_managed other)
            {
                return new d_managed(self.val * other.val);
            }
        }

        struct d_native
        {
            private double val;

            public d_native(double d)
            {
                val = d;
            }

            [DllImport("NumericCore.dll", CallingConvention = CallingConvention.StdCall)]
            [SuppressUnmanagedCodeSecurity]
            private static extern void d_add(ref d_native self, ref d_native val);

            public static d_native operator +(d_native self, d_native other)
            {
                d_add(ref self, ref other);
                return self;
            }

            [DllImport("NumericCore.dll", CallingConvention = CallingConvention.StdCall)]
            [SuppressUnmanagedCodeSecurity]
            private static extern void d_mul(ref d_native self, ref d_native val);

            public static d_native operator *(d_native self, d_native other)
            {
                d_mul(ref self, ref other);
                return self;
            }
        }


        private static void RunTruncationModel()
        {
            var model = CircuitGenerator.GetTruncationErrorModel().GetLargeSignalModel();


            model.NonlinearIterationEpsilon = 1e-10;
            model.MaxDcPointIterations = 100;
            model.MaxTimeStep = 10e-6;
            model.EstablishInitialDcBias();
            SimulateAndPrint(model, 10e-3, model.MaxTimeStep);
        }

        private static string GetProjectName(string filePath)
        {
            return filePath.Substring(projectPath.Length,
                filePath.IndexOf('\\', projectPath.Length) - projectPath.Length);
        }

        private static void PrintFileSizes()
        {
            var files = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(projectPath, "*.cpp", SearchOption.AllDirectories)).GroupBy(GetProjectName);


            long sizeTotal = 0;

            foreach (var grp in files)
            {
                long sizeGrp = 0;
                var group = (IEnumerable<string>) grp;

                if (grp.Key.Contains("Native"))
                    group = grp.Where(name => !name.Contains("src")); // ignore qd library


                foreach (var file in group)
                {
                    var info = new FileInfo(file);
                    sizeGrp += info.Length;
                }

                Console.WriteLine($"{(sizeGrp / 1000.0).ToString("F").PadLeft(10)}kb\t- {grp.Key}");
                sizeTotal += sizeGrp;
            }

            Console.WriteLine("------------------------------------");
            Console.WriteLine($"{(sizeTotal / 1000.0).ToString("F").PadLeft(10)}kb\t- TOTAL");
            Console.WriteLine();
        }

        private static void PrintStats(LargeSignalCircuitModel model, double time, params double[] vals)
        {
            //            Console.WriteLine($"{(time * 1e6),+20:#0.## 'us'}|{string.Join("|", model.NodeVoltages.Select(v => $"{v,20:G10}"))}|{-val,20:G10}");
//            Console.WriteLine($"{time} {-val}");
            Console.WriteLine(
                $"{time} {model.NodeVoltages[2]} {model.NodeVoltages[2] - model.NodeVoltages[3]} {string.Join(" ", vals.Select(v => v.ToString(CultureInfo.InvariantCulture)))}");
//            Console.WriteLine(model.IterationCount);
//            Console.WriteLine($"{model.NodeVoltages[1]} {-val}");
        }

        private static void SimulateAndPrint(LargeSignalCircuitModel model, double time, double step)
        {
            var elapsed = 0.0;

            //model.EstablishDcBias();
            //            Console.WriteLine("Voltages:");
            //            Console.WriteLine($"Time                |{string.Join("|", Enumerable.Range(0, model.NodeCount).Select(i => $"{i,20}"))}|Il");
            //            Console.WriteLine("-------------------------------------------------------------------------------------------------");
            var voltageSource = model.Devices.OfType<LargeSignalVoltageSourceModel>().Single(d => d.Name == "VS");
            var device = model.Devices.OfType<LargeSignalDiodeModel>().Single(d => d.Name == "D1");
//            var device2 = model.Devices.OfType<LargeSignalDiodeModel>().Single(d => d.Name == "D2");
//            var device = model.Devices.OfType<LargeSignalCapacitorModel>().Single();

            //            PrintStats(model, elapsed, device.Current);
            while (elapsed < time)
            {
                model.AdvanceInTime(step);
                elapsed += step;
                PrintStats(model, elapsed, voltageSource.Voltage, device.Current);
                //                PrintStats(model, elapsed, device.Voltage);
            }
        }

        private static void RunModel()
        {
            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0, new PieceWiseLinearBehaviorParams
                {
                    DefinitionPoints = new Dictionary<double, double>
                    {
                        [100e-6] = 1
                    },
                    InitialValue = 0
                }, "V")
                .AddDiode(1, 0, d => { d.ReverseBreakdownVoltage = 2; }, "D")
                .BuildCircuit();

            var model = circuit.GetLargeSignalModel();

            model.NonlinearIterationEpsilon = 1e-10;
            model.MaxDcPointIterations = 10000;

            model.EstablishInitialDcBias();
            SimulateAndPrint(model, 1000e-6, 1e-6);
        }


        [Conditional("DEBUG")]
        private static void SetListeners()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }
    }
}