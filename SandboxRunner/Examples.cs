using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Extensions;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Devices;
using NextGenSpice.LargeSignal.NumIntegration;
using NextGenSpice.Parser;

namespace SandboxRunner
{
    public class Examples
    {
        public static void Simple()
        {
            var builder = new CircuitBuilder();
            builder
                .AddDevice(new[] { 1, 0 }, new VoltageSource(12, "VS"))
                .AddDevice(new[] { 0, 2 }, new Resistor(10))
                .AddDevice(new[] { 1, 2 }, new Resistor(10))
                .AddDevice(new[] { 2, 3 }, new Resistor(5))
                .AddDevice(new[] { 1, 3 }, new Resistor(5));
            var circuit = builder.BuildCircuit();

            //                        var builder = new CircuitBuilder();
            //                        builder
            //                            .AddVoltageSource(1, 0, 12)
            //                            .AddResistor(0, 2, 10)
            //                            .AddResistor(1, 2, 10)
            //                            .AddResistor(2, 3, 5)
            //                            .AddResistor(1, 3, 5);
            //                        var circuit = builder.BuildCircuit();

            var model = circuit.GetLargeSignalModel();
            // equivalent to
            var m = AnalysisModelCreator
                .Instance.GetModel<LargeSignalCircuitModel>(circuit);


            //            builder
            //                .AddVoltageSource(1, 0, 12, "VS")
            ////              .AddDevice(new[] { 1, 0 }, new VoltageSourceDevice(12, "VS"))
            ////                ...

            model.EstablishDcBias();
            Console.WriteLine(model.NodeVoltages[1]); // 12 V
            Console.WriteLine(model.NodeVoltages[2]); //  8 V
            Console.WriteLine(model.NodeVoltages[3]); // 10 V

            // equivalent to
            // var vsource = (ITwoTerminalLargeSignalDevice) model.Devices.Single(
            //    dev => dev.DefinitionDevice.Tag.Equals("VS"));
            var vsouce = (ITwoTerminalLargeSignalDevice)model.FindDevice("VS");

            Console.WriteLine(vsouce.Current); // -0.8 V
        }

        private static void Simple2()
        {
            var builder = new CircuitBuilder();
            builder
                .AddVoltageSource(1, 0, 12, "VS")
                .AddResistor(0, 2, 10)
                .AddResistor(1, 2, 10)
                .AddResistor(2, 3, 5)
                .AddResistor(1, 3, 5);
            var circuit = builder.BuildCircuit();

            var model = circuit.GetLargeSignalModel();

            model.EstablishDcBias();

            Console.WriteLine(model.NodeVoltages[1]); // 12 V
            Console.WriteLine(model.NodeVoltages[2]); //  8 V
            Console.WriteLine(model.NodeVoltages[3]); // 10 V

            var vsouce = (ITwoTerminalLargeSignalDevice)model.FindDevice("VS");

            Console.WriteLine(vsouce.Current); // -0.8 V
        }

        public static void SimpleRlc()
        {
            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0, new PulseBehaviorParams
                {
                    InitialLevel = 0,
                    PulseLevel = 5,
                    Delay = 5e-3, // 5 ms
                    PulseWidth = 25e-3 // 25 ms
                })
                .AddResistor(1, 2, 50)
                .AddInductor(2, 3, 0.125)
                .AddCapacitor(3, 0, 1e-6)
                .BuildCircuit();

            var model = circuit.GetLargeSignalModel();

            model.EstablishDcBias();

            Console.WriteLine("Time V(1) V(3)");

            var timestep = 0.2e-3; // use 0.2 ms timestep
            while (model.CurrentTimePoint <= 55e-3) // simulate for 55 ms
            {
                var time = model.CurrentTimePoint;
                var v1 = model.NodeVoltages[1];
                var v3 = model.NodeVoltages[3];

                Console.WriteLine($"{time} {v1} {v3}");

                model.AdvanceInTime(timestep);
            }

            model.CircuitParameters.IntegrationMethodFactory =
                new SimpleIntegrationMethodFactory(() => new TrapezoidalIntegrationMethod());
        }

        public static void SimpleSubcircuit()
        {
            var builder = new CircuitBuilder();
            var batteryDefinition = builder
                .AddVoltageSource(1, 2, 9)
                .AddResistor(2, 3, 1.5)
                .BuildSubcircuit(new[] { 1, 3 });

            builder.Clear();
            builder
                .AddDevice(new[] { 0, 1 }, new Subcircuit(batteryDefinition))
                .AddSubcircuit(new[] { 0, 1 }, batteryDefinition);
        }

        public static void LoadingSubcircuit()
        {
            var parser = SpiceNetlistParser.WithDefaults();
            var result = parser.Parse(new StreamReader("doubledouble.txt"));
            //            var result = parser.Parse(new StreamReader("circuit.cir"));
            var circuit = result.CircuitDefinition;

            var model = circuit.GetLargeSignalModel();
            var d1 = (ITwoTerminalLargeSignalDevice)model.FindDevice("D1");
            var inNode = result.NodeIndices["IN"];

            Console.WriteLine("Time V(IN) I(D1)");

            var timestep = 10e-6; // use 0.2 ms timestep
            while (model.CurrentTimePoint <= 10e-3) // simulate for 50 ms
            {
                var time = model.CurrentTimePoint;
                var vin = model.NodeVoltages[inNode];
                var id1 = d1.Current;

                Console.WriteLine($"{time} {vin} {id1}");

                model.AdvanceInTime(timestep);
            }
        }

        public static void LiveSimExample()
        {
            var builder = new CircuitBuilder();
            builder
                .AddVoltageSource(1, 0, new SinusoidalBehaviorParams()
                {
                    Amplitude = 5,
                    Frequency = 1e3
                })
                .AddCapacitor(1, 2, 0.125, null, "C1")
                .AddResistor(2, 0, 1);

            var m1 = builder.BuildCircuit().GetLargeSignalModel();
            m1.EstablishDcBias();


            Console.WriteLine("Time V_1 V_2");
            var timestep = 10e-6;
            while (m1.CurrentTimePoint <= 1e-3)
            {
                var time = m1.CurrentTimePoint;
                Console.WriteLine($"{time} {m1.NodeVoltages[1]} {m1.NodeVoltages[2]}");

                m1.AdvanceInTime(timestep);
            }

            builder.AddCapacitor(2, 0, 0.125, null, "C2");
            var m2 = builder.BuildCircuit().GetLargeSignalModel();

            var c1 = (LargeSignalCapacitor)m1.FindDevice("C1");
            var intprop = typeof(LargeSignalCapacitor).GetProperty("IntegrationMethod",
                BindingFlags.Instance | BindingFlags.NonPublic);
            intprop.SetValue(m2.FindDevice("C1"), intprop.GetValue(c1));

            var ctxprop =
                typeof(LargeSignalCircuitModel).GetField("context", BindingFlags.Instance | BindingFlags.NonPublic);
            var ctx = (ISimulationContext)ctxprop.GetValue(m1);

            // too complex not worth it

            var tpprop = ctx.GetType().GetProperty("TimePoint", BindingFlags.NonPublic | BindingFlags.Instance);
            tpprop.SetValue(ctxprop.GetValue(m2), ctx.TimePoint);
        }

        public static void ResistorSweep()
        {
            var builder = new CircuitBuilder();
            builder
                .AddVoltageSource(1, 0, 12, "VS")
                .AddResistor(0, 2, 10)
                .AddResistor(1, 2, 10)
                .AddResistor(2, 3, 5)
                .AddResistor(1, 3, 5, "R1");
            var circuit = builder.BuildCircuit();

            var model = circuit.GetLargeSignalModel();
            
            var vsouce = (ITwoTerminalLargeSignalDevice)model.FindDevice("VS");
            var res = (Resistor)circuit.FindDevice("R1");
            // sweep for values from 1 Ohm to 15 Ohm
            for (int i = 1; i <= 15; i++)
            {
                res.Resistance = i+1; // set resistance

                // calculate
                model.EstablishDcBias();
                var v1 = model.NodeVoltages[1];
                var v2 = model.NodeVoltages[2];
                var v3 = model.NodeVoltages[3];
                var iV = vsouce.Current;

                // print values
                Console.WriteLine($"{i+1}Ohm: {v1}V {v2}V {v3}V {iV}A");
            }
        }
    }
}