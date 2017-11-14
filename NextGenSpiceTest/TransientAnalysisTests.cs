using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
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

        [Fact]
        public void TestSimpleCapacitorTimeDependentCircuit()
        {
            // TODO expected values taken from LTSPICE, file capacitor_simple_time.asc - see docs.

            SwitchModel switchModel = null;

            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0, 1)
                .AddResistor(1, 2, 5)
                .AddCapacitor(2, 0, 1e-6)
                .AddElement(new int[] { 2, 3 }, new SwitchElement())
                .AddResistor(0, 3, 5)
                .Build();

            List<double> results = new List<double>(40);

            circuit.GetFactory<LargeSignalCircuitModel>().SetModel<SwitchElement, SwitchModel>(m =>
            {
                switchModel = new SwitchModel(m);
                return switchModel;
            });

            var model = circuit.GetLargeSignalModel();

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
            // TODO expected values taken from LTSPICE, file capacitor_simple_time.asc - see docs.

            SwitchModel switchModel = null;

            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0, 1)
                .AddResistor(1, 2, 5)
                .AddInductor(2, 0, 1e-6)
                .AddElement(new int[] { 2, 3 }, new SwitchElement())
                .AddResistor(0, 3, 5)
                .Build();

            List<double> results = new List<double>(40);

            circuit.GetFactory<LargeSignalCircuitModel>().SetModel<SwitchElement, SwitchModel>(m =>
            {
                switchModel = new SwitchModel(m);
                return switchModel;
            });

            var model = circuit.GetLargeSignalModel();

            model.EstablishDcBias();
            output.PrintCircuitStats(model);

            output.WriteLine("Voltages:");
            output.WriteLine(string.Join("\t", Enumerable.Range(0, model.NodeCount)));

            results.Add(model.NodeVoltages[2]);
            switchModel.IsOn = false;

            var inductor = model.TimeDependentElements.OfType<LargeSignalInductorModel>().Single();

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