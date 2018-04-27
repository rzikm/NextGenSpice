using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.Core.Extensions;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;

namespace NextGenSpiceTest
{
    public class CircuitGenerator
    {
        public static CircuitDefinition GetLinearCircuit()
        {
            //taken from Inside SPICE, pg. 16

            return new CircuitBuilder()
                .AddResistor(1, 2, 5, "R1")
                .AddResistor(2, 0, 10)
                .AddResistor(2, 3, 5)
                .AddResistor(3, 0, 10)
                .AddCurrentSource(0, 1, 3)
                .BuildCircuit();
        }

        public static CircuitDefinition GetNonlinearCircuit()
        {
            //taken from http://www.ecircuitcenter.com/SpiceTopics/Non-Linear%20Analysis/Non-Linear%20Analysis.htm
            return new CircuitBuilder()
                .AddResistor(1, 0, 100)
                .AddResistor(1, 2, 10000)
                .AddCurrentSource(0, 1, 0.1)
                .AddDiode(2, 0, p => { p.SaturationCurrent = 1e-15; })
                .BuildCircuit();
        }

        public static CircuitDefinition GetCircuitWithVoltageSource()
        {
            // taken from https://www.swarthmore.edu/NatSci/echeeve1/Ref/mna/MNA2.html, example 3

            return new CircuitBuilder()
                .AddResistor(1, 0, 1)
                .AddResistor(1, 2, 2)
                .AddResistor(0, 2, 3)
                .AddVoltageSource(1, 0, 5).BuildCircuit();
        }

        public static CircuitDefinition GetCircuitWithBasicDevices()
        {
            return new CircuitBuilder()
                .AddResistor(0, 1, 1)
                .AddCapacitor(1, 2, 5)
                .AddCurrentSource(0, 3, 5)
                .AddDiode(3, 4, DiodeParams.Default)
                .AddInductor(4, 2, 5)
                .AddVoltageSource(2, 0, 5)
                .BuildCircuit();
        }

        public static CircuitDefinition GetSimpleCircuitWithCapacitor()
        {
            return new CircuitBuilder()
                .AddVoltageSource(1, 0, 1)
                .AddResistor(1, 2, 5)
                .AddCapacitor(2, 0, 1e-6)
                .AddResistor(0, 2, 5)
                .BuildCircuit();
        }

        public static CircuitDefinition GetSimpleCircuitWithInductor()
        {
            return new CircuitBuilder()
                .AddCurrentSource(1, 0, 1)
                .AddResistor(1, 2, 5)
                .AddInductor(2, 3, 1e-6)
                .AddResistor(0, 3, 5)
                .BuildCircuit();
        }



        public static CircuitDefinition GetTruncationErrorModel()
        {
            return new CircuitBuilder()
                .AddVoltageSource(1, 0, new SinusoidalBehaviorParams
                {
                    Amplitude = 5,
                    Frequency = 100
                }, "VS")
//                .AddResistor(2, 3, 3 * 10000000000000e-6)
                .AddResistor(2, 3, 1e-6)
                .AddDiode(1, 2, DiodeParams.D1N4148, "D1")
                .AddDiode(0, 3, DiodeParams.D1N4148, "D2")
                //                .AddResistor(1,2, 1)
//                                .AddResistor(3,0, 10)
                .BuildCircuit();
        }

        public static CircuitDefinition GetCapacitorCircuit()
        {
            return new CircuitBuilder()
                //                .AddVoltageSource(1, 0, new PulseBehaviorParams()
                //                {
                //                    Value1 = 0,
                //                    Value2 = 5,
                //                    Duration = 1e-3,
                //                    Delay = 1e-4,
                //                    Period = 15e-4
                //                })
                .AddVoltageSource(1, 0, new SinusoidalBehaviorParams
                {
                    Amplitude = 5,
                    Frequency = 500
                })
                .AddResistor(2, 3, 1e-6)
                .AddCapacitor(1, 2, 1e-3)
                .AddResistor(3, 0, 1)
                .BuildCircuit();
        }
    }
}