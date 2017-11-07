using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Extensions;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;

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

        class SwitchElement : TwoNodeCircuitElement
        {
        }

        class SwitchModel : TwoNodeLargeSignalModel<SwitchElement>, ITimeDependentLargeSignalDeviceModel
        {
            public bool IsOn { get; set; } = true;

            public SwitchModel(SwitchElement parent) : base(parent)
            {
            }

            public void AdvanceTimeDependentModel(SimulationContext context)
            {
            }

            public void ApplyTimeDependentModelValues(IEquationSystem equation, SimulationContext context)
            {
                if (IsOn)
                    equation.BindEquivalent(Anode, Kathode);
            }

            public void RollbackTimeDependentModel()
            {
            }
        }


        static void Main(string[] args)
        {
            PrintFileSizes();

            SwitchModel switchModel = null;

            var circuit = new CircuitBuilder()
                    .AddVoltageSource(1, 0, 1)
                    .AddResistor(1, 2, 5)
                    .AddCapacitor(2, 0, 3)
                    .AddElement(new int[] { 2, 3 }, new SwitchElement())
                    .AddResistor(0, 3, 5)
                    .Build();

            circuit.GetFactory<LargeSignalCircuitModel>().SetModel<SwitchElement, SwitchModel>(m =>
            {
                switchModel = new SwitchModel(m);
                return switchModel;
            });

            var model = circuit.GetLargeSignalModel();

            Console.WriteLine("Voltages:");
            Console.WriteLine(string.Join("\t", Enumerable.Range(0, model.NodeCount)));
            model.EstablishDcBias();


            Console.WriteLine(string.Join("\t", model.NodeVoltages.Select(v => v.ToString("F"))));

            for (int i = 0; i < 10; i++)
            {
//                if (i > 0) switchModel.IsOn = false;

                model.AdvanceInTime(model.MaxTimeStepMilliseconds * 30);
                Console.WriteLine(string.Join("\t", model.NodeVoltages.Select(v => v.ToString("F"))));
            }
        }
    }
}
