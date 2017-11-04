using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Extensions;
using NextGenSpice.Core.Representation;

namespace NextGenSpiceTest
{
    public class CircuitGenerator
    {
        public static ElectricCircuitDefinition GetLinearCircuit()
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

        public static ElectricCircuitDefinition GetNonlinearCircuit()
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

        public static ElectricCircuitDefinition GetCircuitWithVoltageSource()
        {
            // taken from https://www.swarthmore.edu/NatSci/echeeve1/Ref/mna/MNA2.html, example 3

            var circuit = new CircuitBuilder()
                .AddResistor(1, 1, 0)
                .AddResistor(2, 1, 2)
                .AddResistor(3, 0, 2)
                .AddVoltageSource(5, 1, 0).Build();
            return circuit;
        }

        public static ElectricCircuitDefinition GetCircuitWithBasicDevices()
        {
            var circuit = new CircuitBuilder()
                .AddResistor(1, 0, 1)
                .AddCapacitor(5, 1, 2)
                .AddCurrentSource(5, 2, 3)
                .AddDiode(DiodeModelParams.Default, 3,4)
                .AddInductor(5, 4, 5)
                .AddVoltageSource(5, 5, 0)
                .Build();
            return circuit;
        }
    }
}