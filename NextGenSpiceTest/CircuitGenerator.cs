using System.Collections.Generic;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Extensions;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpiceTest
{
    public class CircuitGenerator
    {
        public static ElectricCircuitDefinition GetLinearCircuit()
        {
            //taken from Inside SPICE, pg. 16

            return new CircuitBuilder()
                .AddResistor(1, 2, 5, "R1")
                .AddResistor(2, 0, 10)
                .AddResistor(2, 3, 5)
                .AddResistor(3, 0, 10)
                .AddCurrentSource(1, 0, 3)
                .BuildCircuit();
        }

        public static ElectricCircuitDefinition GetNonlinearCircuit()
        {
            //taken from http://www.ecircuitcenter.com/SpiceTopics/Non-Linear%20Analysis/Non-Linear%20Analysis.htm
            return new CircuitBuilder()
                .AddResistor(1, 0, 100)
                .AddResistor(1, 2, 10000)
                .AddCurrentSource(1, 0, 0.1)
                .AddDiode(2, 0, p =>
                {
                    p.Vd = 0.7;
                    p.SaturationCurrent = 1e-15;
                })
                .BuildCircuit();
        }

        public static ElectricCircuitDefinition GetCircuitWithVoltageSource()
        {
            // taken from https://www.swarthmore.edu/NatSci/echeeve1/Ref/mna/MNA2.html, example 3

            return new CircuitBuilder()
                .AddResistor(1, 0, 1)
                .AddResistor(1, 2, 2)
                .AddResistor(0, 2, 3)
                .AddVoltageSource(1, 0, 5).BuildCircuit();
        }

        public static ElectricCircuitDefinition GetCircuitWithBasicDevices()
        {
            return new CircuitBuilder()
                .AddResistor(0, 1, 1)
                .AddCapacitor(1, 2, 5)
                .AddCurrentSource(2, 3, 5)
                .AddDiode(3, 4, DiodeModelParams.Default)
                .AddInductor(4, 5, 5)
                .AddVoltageSource(5, 0, 5)
                .BuildCircuit();
        }

        public static ElectricCircuitDefinition GetSimpleCircuitWithCapacitor()
        {
            return new CircuitBuilder()
                .AddVoltageSource(1, 0, 1)
                .AddResistor(1, 2, 5)
                .AddCapacitor(2, 0, 1e-6)
                .AddResistor(0, 2, 5)
                .BuildCircuit();
        }

        public static ElectricCircuitDefinition GetSimpleCircuitWithInductor()
        {
            return new CircuitBuilder()
                .AddCurrentSource(1, 0, 1)
                .AddResistor(1, 2, 5)
                .AddInductor(2, 3, 1e-6)
                .AddResistor(0, 3, 5)
                .BuildCircuit();
        }

        public static LargeSignalCircuitModel GetSimpleTimeDependentModelWithCapacitor(out SwitchModel switchModel)
        {
            SwitchModel sw = null;

            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0, new PulseBehaviorParams())
                .AddElement(new int[] { 1, 2 }, new SwitchElement())
                .AddResistor(2, 3, 1)
                .AddCapacitor(3, 0, 1e-6)
                .BuildCircuit();

            circuit.GetFactory<LargeSignalCircuitModel>().SetModel<SwitchElement, SwitchModel>(m =>
            {
                sw = new SwitchModel(m);
                return sw;
            });
            circuit.GetFactory<LargeSignalCircuitModel>();

            var model = circuit.GetLargeSignalModel();
            switchModel = sw;
            return model;
        }

        public static LargeSignalCircuitModel GetSimpleTimeDependentModelWithInductor(out SwitchModel switchModel)
        {
            SwitchModel sw = null;
            var circuit = new CircuitBuilder()
                .AddVoltageSource(1, 0, 15)
                .AddElement(new int[] { 1, 2 }, new SwitchElement())
                .AddResistor(2, 3, 1)
                .AddInductor(3, 0, 1e-6)
                .BuildCircuit();


            circuit.GetFactory<LargeSignalCircuitModel>().SetModel<SwitchElement, SwitchModel>(m =>
            {
                sw = new SwitchModel(m);
                return sw;
            });

            var model = circuit.GetLargeSignalModel();
            switchModel = sw;
            return model;
        }

        public static ElectricCircuitDefinition GetTruncationErrorModel()
        {
            return new CircuitBuilder()
                .AddVoltageSource(1, 0, new SinusoidalBehaviorParams()
                {
                    Amplitude = 5,
                    Frequency = 100,
                })
                .AddResistor(2,3,1e-6)
                .AddDiode(1,2, DiodeModelParams.D1N4148, "D1")
                .AddDiode(0,3, DiodeModelParams.D1N4148)
//                .AddResistor(1,2, 1)
//                .AddResistor(3,0, 1)
                .BuildCircuit();
        }
    }
}