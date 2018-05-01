using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Extensions;
using NextGenSpice.LargeSignal;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.Test
{
    public class InputSourceBehaviorTests : TracedTestBase
    {
        public InputSourceBehaviorTests(ITestOutputHelper output) : base(output)
        {
            DoTrace = false;
        }

        public List<double> SimulateFor(LargeSignalCircuitModel model, double time, double timestep)
        {
            var voltages = new List<double>();
            Output.WriteLine("Voltages:");
            Output.WriteLine(
                $"Time      |{string.Join("|", Enumerable.Range(0, model.NodeCount).Select(c => c.ToString().PadLeft(10)))}");
            var elapsed = 0.0;

            model.EstablishDcBias();
            while (true)
            {
                Output.WriteLine(
                    $"{elapsed,10:G4}|{string.Join("|", model.NodeVoltages.Select(c => c.ToString("F").PadLeft(10)))}");
                voltages.Add(model.NodeVoltages[1]);

                if (elapsed > time) break;

                model.AdvanceInTime(timestep);
                elapsed += timestep;
            }

            return voltages;
        }

        [Fact]
        public void TestExponentialSource()
        {
            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0,
                    new ExponentialBehaviorParams
                    {
                        InitialLevel = 1,
                        PulseLevel = 5,
                        RiseDelay = 3e-6,
                        RiseTau = 5e-6,
                        FallDelay = 10e-6,
                        FallTau = 10e-6
                    })
                .AddResistor(1, 0, 1)
                .BuildCircuit()
                .GetLargeSignalModel();

            var voltages = SimulateFor(circuit, 30e-6, 1e-6);
        }

        [Fact]
        public void TestPieceWiseLinearSource()
        {
            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0,
                    new PieceWiseLinearBehaviorParams
                    {
                        InitialValue = 1,
                        DefinitionPoints = new Dictionary<double, double>
                        {
                            [1e-6] = 1,
                            [5e-6] = 2,
                            [7e-6] = 2,
                            [8e-6] = 5,
                            [12e-6] = 1,
                            [16e-6] = 2
                        },
                        RepeatStart = 6e-6
                    })
                .AddResistor(1, 0, 1)
                .BuildCircuit()
                .GetLargeSignalModel();

            var voltages = SimulateFor(circuit, 30e-6, 1e-6);
        }

        [Fact]
        public void TestPulseSource()
        {
            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0,
                    new PulseBehaviorParams
                    {
                        InitialLevel = 1,
                        PulseLevel = 5,
                        Delay = 3e-6,
                        TimeRise = 4e-6,
                        PulseWidth = 5e-6,
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
                    new SinusoidalBehaviorParams
                    {
                        DcOffset = 1,
                        Amplitude = 1,
                        Frequency = 2 * Math.PI / 10e-6,
                        Delay = 5e-6,
                        DampingFactor = 10000,
                        PhaseOffset = Math.PI / 2
                    })
                .AddResistor(1, 0, 1)
                .BuildCircuit()
                .GetLargeSignalModel();

            var voltages = SimulateFor(circuit, 30e-6, 1e-6);
        }
    }
}