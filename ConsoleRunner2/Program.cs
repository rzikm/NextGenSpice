using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Extensions;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using NextGenSpiceTest;

namespace ConsoleRunner2
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
            Console.WriteLine($"{(time*1e6) , +5 :##.## 'us'}\t|{string.Join("\t|", model.NodeVoltages.Select(v => v.ToString("F")))}\t|{val:F}");
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


        static void Main(string[] args)
        {
            PrintFileSizes();
            
            var model = CircuitGenerator.GetSimpleTimeDependentModelWithCapacitor(out var switchModel); 
//            var model = CircuitGenerator.GetSimpleTimeDependentModelWithInductor(out var switchModel);
            switchModel.IsOn = false;
//            var model = CircuitGenerator.GetSimpleCircuitWithInductor().GetLargeSignalModel();
            model.EstablishDcBias();
            switchModel.IsOn = true;

            SimulateAndPrint(model, 15e-6, 1e-6);
        }
    }
}
