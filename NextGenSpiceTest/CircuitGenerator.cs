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
                .AddResistor(1, 2, 5)
                .AddResistor(2, 0, 10)
                .AddResistor(2, 3, 5)
                .AddResistor(3, 0, 10)
                .AddCurrentSource(1, 0, 3)
                .Build();
            return circuit;
        }

        public static ElectricCircuitDefinition GetNonlinearCircuit()
        {
            //taken from http://www.ecircuitcenter.com/SpiceTopics/Non-Linear%20Analysis/Non-Linear%20Analysis.htm
            var circuit = new CircuitBuilder()
                .AddResistor(1, 0, 100)
                .AddResistor(1, 2, 10000)
                .AddCurrentSource(1, 0, 0.1)
                .AddDiode(2, 0, p => { p.Vd = 0.2; })
                .Build();
            return circuit;
        }

        public static ElectricCircuitDefinition GetCircuitWithVoltageSource()
        {
            // taken from https://www.swarthmore.edu/NatSci/echeeve1/Ref/mna/MNA2.html, example 3

            var circuit = new CircuitBuilder()
                .AddResistor(1, 0, 1)
                .AddResistor(1, 2, 2)
                .AddResistor(0, 2, 3)
                .AddVoltageSource(1, 0, 5).Build();
            return circuit;
        }

        public static ElectricCircuitDefinition GetCircuitWithBasicDevices()
        {
            var circuit = new CircuitBuilder()
                .AddResistor(0, 1, 1)
                .AddCapacitor(1, 2, 5)
                .AddCurrentSource(2, 3, 5)
                .AddDiode(3, 4, DiodeModelParams.Default)
                .AddInductor(4, 5, 5)
                .AddVoltageSource(5, 0, 5)
                .Build();
            return circuit;
        }
    }
}