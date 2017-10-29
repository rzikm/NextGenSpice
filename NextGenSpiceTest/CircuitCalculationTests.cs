using System;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Extensions;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using Xunit;

namespace NextGenSpiceTest
{
   
    public class CircuitCalculationTests
    {
        public CircuitCalculationTests()
        {
            //ElectricCircuitDefinition.SetGlobalFactory(new largefa);
        }

        private static void PrintStats(CircuitSimulator sim)
        {
            Console.WriteLine($"Iterations: {sim.IterationCount}");
            Console.WriteLine($"Delta^2: {sim.DeltaSquared}");
            Console.WriteLine("Voltages:");
            for (var id = 0; id < sim.Model.NodeVoltages.Length; id++)
            {
                Console.WriteLine($"[{id}]:\t{sim.Model.NodeVoltages[id]}");
            }
        }

        [Fact]
        public void TestLinearCircuit()
        {
            var circuit = GetTestLinearCircuit();
            CircuitSimulator sim = new CircuitSimulator(circuit.GetModel<LargeSignalCircuitModel>());
            sim.EstablishDcBias();

            PrintStats(sim);

            Assert.Equal(new double[4] { 0, 33, 18, 12 }, sim.Model.NodeVoltages,
                new DoubleComparer(1e-10));
        }

        [Fact]
        public void TestNonlinearCircuit()
        {
            var circuit = GetTestNonlinearCircuit();

            CircuitSimulator sim = new CircuitSimulator(circuit.GetModel<LargeSignalCircuitModel>());
            sim.EstablishDcBias();
            PrintStats(sim);

            Assert.Equal(new double[] { 0, 9.90804734507935, 0.712781853012352 },
                sim.Model.NodeVoltages, new DoubleComparer(1e-10));
        }

        [Fact]
        public void TestVoltageSource()
        {
            var circuit = GetTestCircuitWithVoltageSource();

            CircuitSimulator sim = new CircuitSimulator(circuit.GetModel<LargeSignalCircuitModel>());
            sim.EstablishDcBias();
            PrintStats(sim);

            Assert.Equal(new double[] { 0, 5, 3 },
                sim.Model.NodeVoltages, new DoubleComparer(1e-10));
        }

        [Fact]
        public void TestCapacitorAsOpenCircuit()
        {
            CircuitSimulator sim;

            Console.WriteLine("With capacitor:");
            {
                var circuit = new CircuitBuilder()
                    .AddVoltageSource(5, 1, 0)
                    .AddResistor(2, 1, 0)
                    .AddResistor(5, 2, 0)
                    .AddCapacitor(1, 1, 2).Build();
                sim = new CircuitSimulator(circuit.GetModel<LargeSignalCircuitModel>());
            }
            sim.EstablishDcBias();
            PrintStats(sim);

            var withCapacitorVoltages = sim.Model.NodeVoltages.ToArray();

            Console.WriteLine("\nWithout capacitor:");
            {
                var circuit = new CircuitBuilder()
                    .AddVoltageSource(5, 1, 0)
                    .AddResistor(2, 1, 0)
                    .AddResistor(5, 2, 0).Build();
                sim = new CircuitSimulator(circuit.GetModel<LargeSignalCircuitModel>());
            }
            sim.EstablishDcBias();
            PrintStats(sim);

            var withoutCapacitorVoltages = sim.Model.NodeVoltages.ToArray();

            Assert.Equal(withoutCapacitorVoltages, withCapacitorVoltages, new DoubleComparer(1e-10));
        }

        [Fact]
        public void TestInductorAsShortCircuit()
        {
            CircuitSimulator sim;
            Console.WriteLine("With inductor:");
            {
                var circuit = new CircuitBuilder()
                    .AddVoltageSource(5, 1, 0)
                    .AddResistor(2, 1, 0)
                    .AddResistor(5, 2, 0)
                    .AddInductor(1, 1, 2).Build();
                sim = new CircuitSimulator(circuit.GetModel<LargeSignalCircuitModel>());
            }
            sim.EstablishDcBias();
            PrintStats(sim);

            var withInductorVoltages = sim.Model.NodeVoltages.ToArray();

            Console.WriteLine("\nWithout inductor:");
            {
                var circuit = new CircuitBuilder()
                    .AddVoltageSource(5, 1, 0)
                    .AddResistor(2, 1, 0)
                    .AddResistor(5, 1, 0).Build();
                sim = new CircuitSimulator(circuit.GetModel<LargeSignalCircuitModel>());
            }
            sim.EstablishDcBias();
            PrintStats(sim);

            var withoutInductorVoltages = sim.Model.NodeVoltages.ToList();
            withoutInductorVoltages.Insert(1, withoutInductorVoltages[1]);

            Assert.Equal(withoutInductorVoltages, withInductorVoltages, new DoubleComparer(1e-10));
        }

        private static ElectricCircuitDefinition GetTestLinearCircuit()
        {
            //taken from Inside SPICE, pg. 16

            var circuit = new CircuitBuilder()
                .AddResistor(5, 1, 2)
                .AddResistor(10, 2, 0)
                .AddResistor(5, 2, 3)
                .AddResistor(10, 3, 0)
                .AddCurrentSource(3, 1, 0)
                .Build();
            return circuit;
        }

        private static ElectricCircuitDefinition GetTestNonlinearCircuit()
        {
            //taken from http://www.ecircuitcenter.com/SpiceTopics/Non-Linear%20Analysis/Non-Linear%20Analysis.htm
            var circuit = new CircuitBuilder()
                .AddResistor(100, 1, 0)
                .AddResistor(10000, 1, 2)
                .AddCurrentSource(0.1, 1, 0)
                .AddDiode(p => { p.Vd = 0.2; }, 2, 0)
                .Build();
            return circuit;
        }

        private static ElectricCircuitDefinition GetTestCircuitWithVoltageSource()
        {
            // taken from https://www.swarthmore.edu/NatSci/echeeve1/Ref/mna/MNA2.html, example 3

            var circuit = new CircuitBuilder()
                .AddResistor(1, 1, 0)
                .AddResistor(2, 1, 2)
                .AddResistor(3, 0, 2)
                .AddVoltageSource(5, 1, 0).Build();
            return circuit;
        }
    }
}
