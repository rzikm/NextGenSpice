using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Extensions;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Devices;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.Test
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
            model.EstablishDcBias();

            var device = model.Devices.OfType<LargeSignalCapacitor>().Single();

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
        public void TestTimeSimulationDoesNotChangeResultWhenUsingCapacitor()
        {
            var circuit = CircuitGenerator.GetSimpleCircuitWithCapacitor();

            var model = circuit.GetLargeSignalModel();
            model.EstablishDcBias();
            Output.PrintCircuitStats(model);

            var expected = model.NodeVoltages.ToArray();
            model.AdvanceInTime(1e-6);
            Output.PrintCircuitStats(model);

            Assert.Equal(expected, model.NodeVoltages);
        }

        [Fact]
        public void TestTimeSimulationDoesNotChangeResultWhenUsingInductor()
        {
            var circuit = CircuitGenerator.GetSimpleCircuitWithInductor();

            var model = circuit.GetLargeSignalModel();
            model.EstablishDcBias();
            Output.PrintCircuitStats(model);

            var expected = model.NodeVoltages.ToArray();
            model.AdvanceInTime(1e-6);
            Output.PrintCircuitStats(model);

            Assert.Equal(expected, model.NodeVoltages, new DoubleComparer(1e-4));
        }
    }
}