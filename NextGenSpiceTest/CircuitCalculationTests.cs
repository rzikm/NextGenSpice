using System;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Extensions;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpiceTest
{
    public class CircuitCalculationTests
    {
        private readonly ITestOutputHelper output;

        public CircuitCalculationTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private void PrintStats(LargeSignalCircuitModel sim)
        {
            
            output.WriteLine($"Iterations: {sim.IterationCount}");
            output.WriteLine($"Delta^2: {sim.DeltaSquared}");
            output.WriteLine("Voltages:");
            for (var id = 0; id < sim.NodeVoltages.Length; id++)
            {
                output.WriteLine($"[{id}]:\t{sim.NodeVoltages[id]}");
            }
            output.WriteLine("");
        }

        [Fact]
        public void TestLinearCircuit()
        {
            var circuit = CircuitGenerator.GetLinearCircuit();
            var sim = circuit.GetModel<LargeSignalCircuitModel>();
            sim.EstablishDcBias();

            PrintStats(sim);

            Assert.Equal(new double[4] { 0, 33, 18, 12 }, sim.NodeVoltages,
                new DoubleComparer(1e-10));
        }

        [Fact]
        public void TestNonlinearCircuit()
        {
            var circuit = CircuitGenerator.GetNonlinearCircuit();

            var sim = circuit.GetModel<LargeSignalCircuitModel>();
            sim.EstablishDcBias();
            PrintStats(sim);

            Assert.Equal(new double[] { 0, 9.90804734507935, 0.712781853012352 },
                sim.NodeVoltages, new DoubleComparer(1e-10));
        }

        [Fact]
        public void TestVoltageSource()
        {
            var circuit = CircuitGenerator.GetCircuitWithVoltageSource();

            var sim = circuit.GetModel<LargeSignalCircuitModel>();
            sim.EstablishDcBias();
            PrintStats(sim);

            Assert.Equal(new double[] { 0, 5, 3 },
                sim.NodeVoltages, new DoubleComparer(1e-10));
        }

        [Fact]
        public void TestCapacitorAsOpenCircuit()
        {
            LargeSignalCircuitModel sim;

            Console.WriteLine("With capacitor:");
            {
                var circuit = new CircuitBuilder()
                    .AddVoltageSource(5, 1, 0)
                    .AddResistor(2, 1, 0)
                    .AddResistor(5, 2, 0)
                    .AddCapacitor(1, 1, 2).Build();
                Assert.Equal(3, circuit.NodeCount);
                sim = circuit.GetModel<LargeSignalCircuitModel>();
            }
            sim.EstablishDcBias();
            PrintStats(sim);

            var withCapacitorVoltages = sim.NodeVoltages.ToArray();

            Console.WriteLine("\nWithout capacitor:");
            {
                var circuit = new CircuitBuilder()
                    .AddVoltageSource(5, 1, 0)
                    .AddResistor(2, 1, 0)
                    .AddResistor(5, 2, 0).Build();
                sim = circuit.GetModel<LargeSignalCircuitModel>();
            }
            sim.EstablishDcBias();
            PrintStats(sim);

            var withoutCapacitorVoltages = sim.NodeVoltages.ToArray();

            Assert.Equal(withoutCapacitorVoltages, withCapacitorVoltages, new DoubleComparer(1e-10));
        }

        [Fact]
        public void TestInductorAsShortCircuit()
        {
            LargeSignalCircuitModel sim;
            Console.WriteLine("With inductor:");
            {
                var circuit = new CircuitBuilder()
                    .AddVoltageSource(5, 1, 0)
                    .AddResistor(2, 1, 0)
                    .AddResistor(5, 2, 0)
                    .AddInductor(1, 1, 2).Build();
                sim = circuit.GetModel<LargeSignalCircuitModel>();
            }
            sim.EstablishDcBias();
            PrintStats(sim);

            var withInductorVoltages = sim.NodeVoltages.ToArray();

            Console.WriteLine("\nWithout inductor:");
            {
                var circuit = new CircuitBuilder()
                    .AddVoltageSource(5, 1, 0)
                    .AddResistor(2, 1, 0)
                    .AddResistor(5, 1, 0).Build();
                sim = circuit.GetModel<LargeSignalCircuitModel>();
            }
            sim.EstablishDcBias();
            PrintStats(sim);

            var withoutInductorVoltages = sim.NodeVoltages.ToList();
            withoutInductorVoltages.Insert(1, withoutInductorVoltages[1]);

            Assert.Equal(withoutInductorVoltages, withInductorVoltages, new DoubleComparer(1e-10));
        }
    }
}
