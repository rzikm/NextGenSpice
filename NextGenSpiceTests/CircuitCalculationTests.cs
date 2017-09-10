using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using NextGenSpice;
using NextGenSpice.Circuit;

namespace NextGenSpiceTests
{
    [TestFixture]
    public partial class CircuitCalculationTests
    {
        [Test]
        public void TestLinearCircuit()
        {
            var circuit = GetTestLinearCircuit();
            CircuitSimulator sim = new CircuitSimulator(circuit);
            sim.EstablishDcBias();
            CollectionAssert.AreEqual(new double[4] { 0, 33, 18, 12 }, circuit.Nodes.Select(n => n.Voltage),
                new DoubleComparer(1e-10));
        }

        [Test]
        public void TestNonlinearCircuit()
        {
            var circuit = GetTestNonlinearCircuit();

            CircuitSimulator sim = new CircuitSimulator(circuit);
            sim.EstablishDcBias();
            CollectionAssert.AreEqual(new double[] { 0, 9.90804734507935, 0.712781853012352 },
                circuit.Nodes.Select(n => n.Voltage), new DoubleComparer(1e-10));
        }

        [Test]
        public void TestVoltageSource()
        {
            var circuit = GetTestCircuitWithVoltageSource();

            CircuitSimulator sim = new CircuitSimulator(circuit);
            sim.EstablishDcBias();
            //            CollectionAssert.AreEqual(new double[] { 0, 9.90804734507935, 0.712781853012352 },
            //                circuit.Nodes.Select(n => n.Voltage), new DoubleComparer(1e-10));
        }

        [Test]
        public void TestCapacitorAsOpenCircuit()
        {
            CircuitSimulator sim;

            Console.WriteLine("With capacitor:");
            {
                CircuitBuilder builder = new CircuitBuilder();
                for (int i = 0; i < 3; i++)
                    builder.AddNode(i);
                builder.AddElement(new VoltageSourceElement(5), 1, 0);
                builder.AddElement(new RezistorElement(2), 1, 0);
                builder.AddElement(new RezistorElement(5), 2, 0);
                builder.AddElement(new CapacitorElement(1), 1, 2);

                sim = new CircuitSimulator(builder.Build());
            }
            sim.EstablishDcBias();
            var withCapacitorVoltages = sim.CircuitDefinition.Nodes.Select(node => node.Voltage).ToArray();

            Console.WriteLine("\nWithout capacitor:");
            {
                CircuitBuilder builder = new CircuitBuilder();
                for (int i = 0; i < 3; i++)
                    builder.AddNode(i);
                builder.AddElement(new VoltageSourceElement(5), 1, 0);
                builder.AddElement(new RezistorElement(2), 1, 0);
                builder.AddElement(new RezistorElement(5), 2, 0);

                sim = new CircuitSimulator(builder.Build());
            }
            sim.EstablishDcBias();
            var withoutCapacitorVoltages = sim.CircuitDefinition.Nodes.Select(node => node.Voltage).ToArray();

            CollectionAssert.AreEqual(withoutCapacitorVoltages, withCapacitorVoltages, new DoubleComparer(1e-10));
        }

        [Test]
        public void TestInductorAsShortCircuit()
        {
            CircuitSimulator sim;
            Console.WriteLine("With inductor:");
            {
                CircuitBuilder builder = new CircuitBuilder();
                for (int i = 0; i < 3; i++)
                    builder.AddNode(i);
                builder.AddElement(new VoltageSourceElement(5), 1, 0);
                builder.AddElement(new RezistorElement(2), 1, 0);
                builder.AddElement(new RezistorElement(5), 2, 0);
                builder.AddElement(new InductorElement(1), 1, 2);

                sim = new CircuitSimulator(builder.Build());
            }
            sim.EstablishDcBias();
            var withInductorVoltages = sim.CircuitDefinition.Nodes.Select(node => node.Voltage).ToArray();

            Console.WriteLine("\nWithout inductor:");
            {
                CircuitBuilder builder = new CircuitBuilder();
                for (int i = 0; i < 2; i++)
                    builder.AddNode(i);
                builder.AddElement(new VoltageSourceElement(5), 1, 0);
                builder.AddElement(new RezistorElement(2), 1, 0);
                builder.AddElement(new RezistorElement(5), 1, 0);

                sim = new CircuitSimulator(builder.Build());
            }
            sim.EstablishDcBias();

            var withoutInductorVoltages = sim.CircuitDefinition.Nodes.Select(node => node.Voltage).ToList();
            withoutInductorVoltages.Insert(1, withoutInductorVoltages[1]);
            CollectionAssert.AreEqual(withoutInductorVoltages, withInductorVoltages, new DoubleComparer(1e-10));
        }

        private static ElectricCircuitDefinition GetTestLinearCircuit()
        {
            //taken from Inside SPICE, pg. 16
            CircuitBuilder builder = new CircuitBuilder();
            builder.AddNode(0);
            builder.AddNode(1);
            builder.AddNode(2);
            builder.AddNode(3);

            builder.AddElement(new RezistorElement(5), 1, 2);
            builder.AddElement(new RezistorElement(10), 2, 0);
            builder.AddElement(new RezistorElement(5), 2, 3);
            builder.AddElement(new RezistorElement(10), 3, 0);

            builder.AddElement(new CurrentSourceElement(3), 1, 0);

            var circuit = builder.Build();
            return circuit;
        }

        private static ElectricCircuitDefinition GetTestNonlinearCircuit()
        {
            //taken from http://www.ecircuitcenter.com/SpiceTopics/Non-Linear%20Analysis/Non-Linear%20Analysis.htm
            CircuitBuilder builder = new CircuitBuilder();
            builder.AddNode(0);
            builder.AddNode(1);
            builder.AddNode(2);

            builder.AddElement(new RezistorElement(100), 1, 0);
            builder.AddElement(new RezistorElement(10000), 1, 2);

            builder.AddElement(new CurrentSourceElement(0.1), 1, 0);

            builder.AddElement(new DiodeElement(new DiodeModelParams()), 2, 0);

            var circuit = builder.Build();
            return circuit;
        }

        private static ElectricCircuitDefinition GetTestCircuitWithVoltageSource()
        {
            // taken from https://www.swarthmore.edu/NatSci/echeeve1/Ref/mna/MNA2.html, example 3
            CircuitBuilder builder = new CircuitBuilder();
            builder.AddNode(0);
            builder.AddNode(1);
            builder.AddNode(2);

            builder.AddElement(new RezistorElement(1), 1, 0);
            builder.AddElement(new RezistorElement(2), 1, 2);
            builder.AddElement(new RezistorElement(3), 0, 2);

            //            builder.AddElement(new CurrentSourceElement(1), 1, 0);
            builder.AddElement(new VoltageSourceElement(5), 1, 0);

            var circuit = builder.Build();
            return circuit;
        }

        private static CircuitBuilder PartialBuildTestCircuit()
        {
            CircuitBuilder builder = new CircuitBuilder();
            for (int i = 0; i < 3; i++)
            {
                builder.AddNode(i);
            }

            builder.AddElement(new VoltageSourceElement(5), 1, 0);
            builder.AddElement(new RezistorElement(2), 1, 0);
            builder.AddElement(new RezistorElement(5), 2, 0);

            return builder;
        }
    }
}
