using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Extensions;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Devices;

namespace SandboxRunner
{
    internal class Program
    {
        private const string projectPath = @"D:\Visual Studio 2017\Projects\NextGen Spice\";

        private static void Main(string[] args)
        {
//            PrintFileSizes(); return;
//            Examples.ResistorSweep(); return;
//            Examples.SimpleRlc(); return;
            var summary = BenchmarkRunner.Run<GaussianEliminationTests>(); return;
//            var summary = BenchmarkRunner.Run<PInvokeOverheadTest>(); return;
            //            IntegrationTest.Run();

//            Console.WriteLine(sw.Elapsed);
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
            var voltageSource = model.Devices.OfType<LargeSignalVoltageSource>().Single(d => d.DefinitionDevice.Tag as string == "VS");
            var device = model.Devices.OfType<LargeSignalDiode>().Single(d => d.DefinitionDevice.Tag as string == "D1");
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
                .AddVoltageSource(1, 0, new PieceWiseLinearBehavior
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

            model.EstablishDcBias();
            SimulateAndPrint(model, 1000e-6, 1e-6);
        }


        [Conditional("DEBUG")]
        private static void SetListeners()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }
    }
}