using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using NextGenSpice;
using NextGenSpice.Circuit;

namespace NextGenSpiceTests
{
    [TestFixture]
    public partial class UnitTest1
    {
        [Test]
        public void TestLinearCircuit()
        {
            CircuitEquationSystem sys = new CircuitEquationSystem(4);

            var circuit = GetTestLinearCircuit();

            foreach (var e in circuit.Elements)
            {
                e.ApplyToEquations(sys);
            }

            sys.Solve();
            for (var i = 0; i < sys.NodeVoltages.Length; i++)
            {
                var v = sys.NodeVoltages[i];
                Console.WriteLine($"node {i}: {v:##.000}");
            }
            CollectionAssert.AreEqual(new double[4] { 0, 33, 18, 12 }, sys.NodeVoltages, new DoubleComparer(1e-10));

        }

        [Test]
        public void TestNonlinearCircuit()
        {
            CircuitSimulator sim = new CircuitSimulator(GetTestNonlinearCircuit());
            sim.EstablishDcBias();
        }

        private static ElectricCircuit GetTestLinearCircuit()
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

        private static ElectricCircuit GetTestNonlinearCircuit()
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
    }
}
