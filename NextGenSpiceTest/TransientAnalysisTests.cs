using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Extensions;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpiceTest
{
    public class TransientAnalysisTests : TracedTestBase
    {
        public TransientAnalysisTests(ITestOutputHelper output) : base(output)
        {
            DoTrace = false;
        }


        [Fact]
        public void TestSimpleCapacitorTimeDependentCircuit()
        {
            // TODO expected values taken from LTSPICE, file capacitor_simple_time.asc - see docs.
            var results = new List<double>();

            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0, new PulseBehaviorParams
                {
                    Delay = 1e-6,
                    PulseWidth = 1,
                    PulseLevel = 15
                })
                .AddResistor(1, 2, 1)
                .AddCapacitor(2, 0, 1e-6, 0)
                .BuildCircuit();

            var model = circuit.GetLargeSignalModel();
            model.EstablishInitialDcBias();

            var device = model.Elements.OfType<LargeSignalCapacitorModel>().Single();

            Output.WriteLine("Voltages:");
            Output.WriteLine(string.Join("\t", Enumerable.Range(0, model.NodeCount)));
            Output.WriteLine(string.Join("\t",
                model.NodeVoltages.Concat(new[] {device.Current}).Select(v => v.ToString("F"))));

            for (var i = 0; i < 40; i++)
            {
                model.AdvanceInTime(1e-6); // 1us

                Output.WriteLine(string.Join("\t",
                    model.NodeVoltages.Concat(new[] {device.Current}).Select(v => v.ToString("F"))));
//                results.Add(model.NodeVoltages[2]);
            }
        }

        [Fact]
        public void TestSimpleInductorTimeDependentCircuit()
        {
            // TODO expected values taken from LTSPICE, file inductor_simple_time.asc - see docs.

            var results = new List<double>();

            var model = CircuitGenerator.GetSimpleTimeDependentModelWithInductor(out var switchModel);
            var inductor = model.Elements.OfType<LargeSignalInductorModel>().Single();

            switchModel.IsOn = false;
            model.EstablishInitialDcBias();
            switchModel.IsOn = true;

            Output.WriteLine("Voltages:");
            Output.WriteLine(string.Join("\t", Enumerable.Range(0, model.NodeCount)));
            Output.WriteLine(string.Join("\t",
                model.NodeVoltages.Concat(new[] {inductor.Current}).Select(v => v.ToString("F"))));

            results.Add(model.NodeVoltages[2]);


            for (var i = 0; i < 40; i++)
            {
                model.AdvanceInTime(1e-6); // 1us

                Output.WriteLine(string.Join("\t",
                    model.NodeVoltages.Concat(new[] {inductor.Current}).Select(v => v.ToString("F"))));
                results.Add(model.NodeVoltages[2]);
            }

        }

        [Fact]
        public void TestTimeSimulationDoesNotChangeResultWhenUsingCapacitor()
        {
            var circuit = CircuitGenerator.GetSimpleCircuitWithCapacitor();

            var model = circuit.GetLargeSignalModel();
            model.EstablishInitialDcBias();
            Output.PrintCircuitStats(model);

            var expected = model.NodeVoltages.ToArray();
            model.AdvanceInTime(model.MaxTimeStep);
            Output.PrintCircuitStats(model);

            Assert.Equal(expected, model.NodeVoltages);
        }

        [Fact]
        public void TestTimeSimulationDoesNotChangeResultWhenUsingInductor()
        {
            var circuit = CircuitGenerator.GetSimpleCircuitWithInductor();

            var model = circuit.GetLargeSignalModel();
            model.EstablishInitialDcBias();
            Output.PrintCircuitStats(model);

            var expected = model.NodeVoltages.ToArray();
            model.AdvanceInTime(model.MaxTimeStep);
            Output.PrintCircuitStats(model);

            Assert.Equal(expected, model.NodeVoltages, new DoubleComparer(1e-4));
        }
    }
}