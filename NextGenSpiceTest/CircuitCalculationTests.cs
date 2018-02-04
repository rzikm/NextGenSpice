﻿using System;
using System.Linq;
using NextGenSpice.Core;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Core.Extensions;
using NextGenSpice.LargeSignal;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpiceTest
{
    public class CircuitCalculationTests : TracedTestBase
    {
        public CircuitCalculationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SimpleSwitchCircuit()
        {
            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0, 1)
                .AddResistor(1, 2, 1)
                .AddResistor(1, 3, 1)
                .AddElement(new[] {2, 3}, new SwitchElement())
                .AddResistor(3, 0, 0.5)
                .BuildCircuit();

            circuit.GetFactory<LargeSignalCircuitModel>().SetModel<SwitchElement, SwitchModel>(e => new SwitchModel(e));

            var model = circuit.GetModel<LargeSignalCircuitModel>();

            var sw = model.Elements.OfType<SwitchModel>().Single();

            Output.WriteLine("Switch is on");
            sw.IsOn = true;
            model.EstablishDcBias();
            Output.PrintCircuitStats(model);

            Output.WriteLine("Switch is off");
            sw.IsOn = false;
            model.EstablishDcBias();
            Output.PrintCircuitStats(model);
        }

        [Fact]
        public void TestCapacitorAsOpenCircuit()
        {
            LargeSignalCircuitModel model;

            Console.WriteLine("With capacitor:");
            {
                var circuit = new CircuitBuilder()
                    .AddVoltageSource(1, 0, 5)
                    .AddResistor(1, 0, 2)
                    .AddResistor(2, 0, 5)
                    .AddCapacitor(1, 2, 1).BuildCircuit();
                Assert.Equal(3, circuit.NodeCount);
                model = circuit.GetModel<LargeSignalCircuitModel>();
            }
            model.EstablishDcBias();
            Output.PrintCircuitStats(model);

            var withCapacitorVoltages = model.NodeVoltages.ToArray();

            Console.WriteLine("\nWithout capacitor:");
            {
                var circuit = new CircuitBuilder()
                    .AddVoltageSource(1, 0, 5)
                    .AddResistor(1, 0, 2)
                    .AddResistor(2, 0, 5).BuildCircuit();
                model = circuit.GetModel<LargeSignalCircuitModel>();
            }
            model.EstablishDcBias();
            Output.PrintCircuitStats(model);

            var withoutCapacitorVoltages = model.NodeVoltages.ToArray();

            Assert.Equal(withoutCapacitorVoltages, withCapacitorVoltages);
        }

        [Fact]
        public void TestInductorAsShortCircuit()
        {
            LargeSignalCircuitModel model;
            Console.WriteLine("With inductor:");
            {
                var circuit = new CircuitBuilder()
                    .AddVoltageSource(1, 0, 5)
                    .AddResistor(1, 0, 2)
                    .AddResistor(2, 0, 5)
                    .AddInductor(1, 2, 1).BuildCircuit();
                model = circuit.GetModel<LargeSignalCircuitModel>();
            }
            model.EstablishDcBias();
            Output.PrintCircuitStats(model);

            var withInductorVoltages = model.NodeVoltages.ToArray();

            Console.WriteLine("\nWithout inductor:");
            {
                var circuit = new CircuitBuilder()
                    .AddVoltageSource(1, 0, 5)
                    .AddResistor(1, 0, 2)
                    .AddResistor(1, 0, 5).BuildCircuit();
                model = circuit.GetModel<LargeSignalCircuitModel>();
            }
            model.EstablishDcBias();
            Output.PrintCircuitStats(model);

            var withoutInductorVoltages = model.NodeVoltages.ToList();
            withoutInductorVoltages.Insert(1, withoutInductorVoltages[1]);

            Assert.Equal(withoutInductorVoltages, withInductorVoltages, new DoubleComparer(1e-10));
        }


        [Fact]
        public void TestLinearCircuit()
        {
            var circuit = CircuitGenerator.GetLinearCircuit();
            var model = circuit.GetModel<LargeSignalCircuitModel>();
            model.EstablishDcBias();

            Output.PrintCircuitStats(model);

            Assert.Equal(new double[4] {0, 33, 18, 12}, model.NodeVoltages,
                new DoubleComparer(1e-10));
        }

        [Fact]
        public void TestNonlinearCircuit()
        {
            var circuit = CircuitGenerator.GetNonlinearCircuit();

            var model = circuit.GetModel<LargeSignalCircuitModel>();
            model.EstablishDcBias();
            Output.PrintCircuitStats(model);

            Assert.Equal(new[] {0, 9.90804460268287, 0.71250487096788},
                model.NodeVoltages, new DoubleComparer(1e-10));
        }

        [Fact]
        public void TestVoltageSource()
        {
            var circuit = CircuitGenerator.GetCircuitWithVoltageSource();

            var model = circuit.GetModel<LargeSignalCircuitModel>();
            model.EstablishDcBias();
            Output.PrintCircuitStats(model);

            Assert.Equal(new double[] {0, 5, 3},
                model.NodeVoltages, new DoubleComparer(1e-10));
        }

        [Fact]
        public void ThrowsWhenCannotConverge()
        {
            var circuit = CircuitGenerator.GetNonlinearCircuit();

            var model = circuit.GetModel<LargeSignalCircuitModel>();
            model.MaxDcPointIterations = 3; // some unrealistic low bound

            Assert.Throws<NonConvergenceException>(() => model.EstablishDcBias());
            Output.PrintCircuitStats(model);
        }
    }
}