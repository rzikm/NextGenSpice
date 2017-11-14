using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Extensions;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpiceTest
{
    public class TransientAnalysisTests
    {
        private readonly ITestOutputHelper output;

        public TransientAnalysisTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestTimeSimulationDoesNotChangeResultWhenUsingInductor()
        {
            var circuit = CircuitGenerator.GetSimpleCircuitWithInductor();

            var model = circuit.GetLargeSignalModel();
            model.EstablishDcBias();
            output.PrintCircuitStats(model);

            var expected = model.NodeVoltages.ToArray();
            model.AdvanceInTime(model.MaxTimeStep);
            output.PrintCircuitStats(model);

            Assert.Equal(expected, model.NodeVoltages, new DoubleComparer(1e-4));
        }





//        [Fact]
        public void TestSimpleCapacitorTimeDependentCircuit()
        {
            // TODO expected values taken from LTSPICE, file capacitor_simple_time.asc - see docs.
            List<double> results = new List<double>();

            var model = CircuitGenerator.GetSimpleTimeDependentModelWithCapacitor(out var switchModel);

            model.EstablishDcBias();
            output.PrintCircuitStats(model);

            output.WriteLine("Voltages:");
            output.WriteLine(string.Join("\t", Enumerable.Range(0, model.NodeCount)));


            results.Add(model.NodeVoltages[2]);
            switchModel.IsOn = false;

            var capacitor = model.TimeDependentElements.OfType<LargeSignalCapacitorModel>().Single();

            for (int i = 0; i < 40; i++)
            {
                model.AdvanceInTime(1e-6); // 1us

                output.WriteLine(string.Join("\t", model.NodeVoltages.Concat(new[] { capacitor.Voltage }).Select(v => v.ToString("F"))));
                results.Add(model.NodeVoltages[2]);
            }

            Assert.Equal(0.77, results[4], new DoubleComparer(0.15));
            Assert.Equal(0.90, results[8], new DoubleComparer(0.15));
        }


        [Fact]
        public void TestSimpleInductorTimeDependentCircuit()
        {
            // TODO expected values taken from LTSPICE, file inductor_simple_time.asc - see docs.

            List<double> results = new List<double>();

            var model = CircuitGenerator.GetSimpleTimeDependentModelWithInductor(out var switchModel);
            var inductor = model.TimeDependentElements.OfType<LargeSignalInductorModel>().Single();

            switchModel.IsOn = false;
            model.EstablishDcBias();
            switchModel.IsOn = true;

            output.PrintCircuitStats(model);

            output.WriteLine("Voltages:");
            output.WriteLine(string.Join("\t", Enumerable.Range(0, model.NodeCount)));
            output.WriteLine(string.Join("\t", model.NodeVoltages.Concat(new[] { inductor.Current }).Select(v => v.ToString("F"))));

            results.Add(model.NodeVoltages[2]);


            for (int i = 0; i < 40; i++)
            {
                model.AdvanceInTime(1e-6); // 1us

                output.WriteLine(string.Join("\t", model.NodeVoltages.Concat(new[] { inductor.Current }).Select(v => v.ToString("F"))));
                results.Add(model.NodeVoltages[2]);
            }

//            Assert.Equal(0.77, results[4], new DoubleComparer(0.15));
//            Assert.Equal(0.90, results[8], new DoubleComparer(0.15));
        }

        [Fact]
        public void TestTimeSimulationDoesNotChangeResultWhenUsingCapacitor()
        {
            var circuit = CircuitGenerator.GetSimpleCircuitWithCapacitor();

            var model = circuit.GetLargeSignalModel();
            model.EstablishDcBias();
            output.PrintCircuitStats(model);

            var expected = model.NodeVoltages.ToArray();
            model.AdvanceInTime(model.MaxTimeStep);
            output.PrintCircuitStats(model);

            Assert.Equal(expected, model.NodeVoltages);
        }
    }
}