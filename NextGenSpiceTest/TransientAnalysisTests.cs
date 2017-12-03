using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
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
                .AddVoltageSource(1, 0, 15)
                .AddResistor(1, 2, 1)
                .AddCapacitor(2, 0, 1e-6, 0)
                .BuildCircuit();

            var model = circuit.GetLargeSignalModel();
            model.EstablishDcBias();

            var device = model.Elements.OfType<LargeSignalCapacitorModel>().Single();

            Output.WriteLine("Voltages:");
            Output.WriteLine(string.Join("\t", Enumerable.Range(0, model.NodeCount)));
            Output.WriteLine(string.Join("\t",
                model.NodeVoltages.Concat(new[] { device.Current }).Select(v => v.ToString("F"))));

            for (var i = 0; i < 40; i++)
            {
                model.AdvanceInTime(1e-6); // 1us

                Output.WriteLine(string.Join("\t",
                    model.NodeVoltages.Concat(new[] { device.Current }).Select(v => v.ToString("F"))));
                results.Add(model.NodeVoltages[2]);
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
            model.EstablishDcBias();
            switchModel.IsOn = true;

            Output.WriteLine("Voltages:");
            Output.WriteLine(string.Join("\t", Enumerable.Range(0, model.NodeCount)));
            Output.WriteLine(string.Join("\t",
                model.NodeVoltages.Concat(new[] { inductor.Current }).Select(v => v.ToString("F"))));

            results.Add(model.NodeVoltages[2]);


            for (var i = 0; i < 40; i++)
            {
                model.AdvanceInTime(1e-6); // 1us

                Output.WriteLine(string.Join("\t",
                    model.NodeVoltages.Concat(new[] { inductor.Current }).Select(v => v.ToString("F"))));
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
            model.EstablishDcBias();
            Output.PrintCircuitStats(model);

            var expected = model.NodeVoltages.ToArray();
            model.AdvanceInTime(model.MaxTimeStep);
            Output.PrintCircuitStats(model);

            Assert.Equal(expected, model.NodeVoltages, new DoubleComparer(1e-4));
        }

        [Fact]
        public void TestPulseSource()
        {
            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0,
                    new PulseBehaviorParams()
                    {
                        Value1 = 1,
                        Value2 = 5,
                        Delay = 3e-6,
                        TimeRise = 4e-6,
                        Duration = 5e-6,
                        TimeFall = 2e-6,
                        Period = 20e-6
                    })
                .AddResistor(1, 0, 1)
                .BuildCircuit()
                .GetLargeSignalModel();

            var voltages = SimulateFor(circuit, 30e-6, 1e-6);
        }

        [Fact]
        public void TestSinusoidalSource()
        {
            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0,
                    new SinusoidalBehaviorParams()
                    {
                        BaseValue = 1,
                        Amplitude = 1,
                        Frequency = (2* Math.PI / 10e-6),
                        Delay = 5e-6,
                        DampingFactor = 10000,
                        Phase = Math.PI/2
                    })
                .AddResistor(1, 0, 1)
                .BuildCircuit()
                .GetLargeSignalModel();

            var voltages = SimulateFor(circuit, 30e-6, 1e-6);
        }


        public List<double> SimulateFor(LargeSignalCircuitModel model, double time, double timestep)
        {
            List<double> voltages = new List<double>();
            Output.WriteLine("Voltages:");
            Output.WriteLine($"Time      |{string.Join("|", Enumerable.Range(0, model.NodeCount).Select(c => c.ToString().PadLeft(10)))}");
            var elapsed = 0.0;

            model.EstablishDcBias();
            while (true)
            {
                Output.WriteLine($"{elapsed, 10:G4}|{string.Join("|", model.NodeVoltages.Select(c => c.ToString("F").PadLeft(10)))}");
                voltages.Add(model.NodeVoltages[1]);

                if (elapsed > time) break;

                model.AdvanceInTime(timestep);
                elapsed += timestep;
            }

            return voltages;
        }
    }
}