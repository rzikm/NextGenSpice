using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using NextGenSpiceTest;
using Numerics.Misc;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Extensions;

namespace SandboxRunner
{
    class Program
    {
        const string projectPath = @"D:\Visual Studio 2017\Projects\NextGen Spice\";

        static string GetProjectName(string filePath)
        {
            return filePath.Substring(projectPath.Length, filePath.IndexOf('\\', projectPath.Length) - projectPath.Length);
        }

        private static void PrintFileSizes()
        {
            var files = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(projectPath, "*.cpp", SearchOption.AllDirectories)).GroupBy(GetProjectName);


            long sizeTotal = 0;

            foreach (var grp in files)
            {
                long sizeGrp = 0;
                var group = (IEnumerable<string>)grp;

                if (grp.Key == "NumericCore")
                    @group = grp.Where(name => !name.Contains("src")); // ignore qd library


                foreach (var file in @group)
                {
                    FileInfo info = new FileInfo(file);
                    sizeGrp += info.Length;
                }
                Console.WriteLine($"{(sizeGrp / 1000.0).ToString("F").PadLeft(10)}kb\t- {grp.Key}");
                sizeTotal += sizeGrp;
            }

            Console.WriteLine("------------------------------------");
            Console.WriteLine($"{(sizeTotal / 1000.0).ToString("F").PadLeft(10)}kb\t- TOTAL");
            Console.WriteLine();
        }

        private static void PrintStats(LargeSignalCircuitModel model, double time, double val)
        {
            Trace.WriteLine($"{(time * 1e6),+5:##.## 'us'}\t|{string.Join("\t|", model.NodeVoltages.Select(v => v.ToString("F")))}\t|{val:F}");
        }
        private static void SimulateAndPrint(LargeSignalCircuitModel model, double time, double step)
        {
            var elapsed = 0.0;

            //model.EstablishDcBias();
            Console.WriteLine("Voltages:");
            Console.WriteLine($"Time\t|{string.Join("\t|", Enumerable.Range(0, model.NodeCount))}\t|Il");
            Console.WriteLine("-------------------------------------------------------------------------");
            //            var device = model.TimeDependentElements.OfType<LargeSignalInductorModel>().Single();
            var device = model.Elements.OfType<LargeSignalCapacitorModel>().Single();

            PrintStats(model, elapsed, device.Current);
            //            PrintStats(model, elapsed, device.Voltage);


            while (elapsed < time)
            {
                model.AdvanceInTime(step);
                elapsed += step;
                PrintStats(model, elapsed, device.Current);
                //                PrintStats(model, elapsed, device.Voltage);
            }
        }


        private static void RunModel()
        {
            SwitchModel sw = null;

            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0, 0)
                .AddElement(new int[] { 1, 2 }, new NextGenSpiceTest.SwitchElement())
                .AddResistor(2, 3, 1)
                .AddCapacitor(3, 0, 1e-6)
                .BuildCircuit();

            circuit.GetFactory<LargeSignalCircuitModel>().SetModel<NextGenSpiceTest.SwitchElement, SwitchModel>(m =>
            {
                sw = new SwitchModel(m);
                return sw;
            });
            circuit.GetFactory<LargeSignalCircuitModel>()
                .SetModel<VoltageSourceElement, PulsingLargeSignalVoltageSourceModel>(m =>
                {
                    return new PulsingLargeSignalVoltageSourceModel(m);
                });


            var model = circuit.GetLargeSignalModel();
            //            var model = CircuitGenerator.GetSimpleTimeDependentModelWithInductor(out var switchModel);
            sw.IsOn = false;
            //            var model = CircuitGenerator.GetSimpleCircuitWithInductor().GetLargeSignalModel();
            model.EstablishDcBias();

            sw.IsOn = true;

            var elapsed = 0.0;

            //model.EstablishDcBias();
            Trace.WriteLine("Voltages:");
            Trace.WriteLine($"Time\t|{string.Join("\t|", Enumerable.Range(0, model.NodeCount))}\t|Il");
            Trace.WriteLine("-------------------------------------------------------------------------");
            //            var device = model.TimeDependentElements.OfType<LargeSignalInductorModel>().Single();
            var device = model.Elements.OfType<LargeSignalCapacitorModel>().Single();

            PrintStats(model, elapsed, device.Current);
            //            PrintStats(model, elapsed, device.Voltage);

            circuit.Elements.OfType<VoltageSourceElement>().Single().Voltage = 15;

            while (elapsed < 15e-6)
            {
                model.AdvanceInTime(0.1e-7);
                elapsed += 0.1e-7;
                PrintStats(model, elapsed, device.Current);
                //                PrintStats(model, elapsed, device.Voltage);
            }
        }

        static void Main(string[] args)
        {
            PrintFileSizes();
            //            SetListeners();
            Stopwatch sw = Stopwatch.StartNew();
            RunModel();
            sw.Stop();

            Console.WriteLine(sw.Elapsed);

//            Misc.HilbertMatrixStabilityTest();
        }


        [Conditional("DEBUG")]
        private static void SetListeners()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }
    }
}
