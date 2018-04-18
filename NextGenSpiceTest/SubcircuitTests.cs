using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Extensions;
using NextGenSpice.LargeSignal;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpiceTest
{
    public class SubcircuitTests : TracedTestBase
    {
        public SubcircuitTests(ITestOutputHelper output) : base(output)
        {
            builder = new CircuitBuilder();
        }

        private readonly CircuitBuilder builder;

        [Fact]
        public void SubcircuitCannotReferenceOuterDevices()
        {
            var subcircuit = builder
                .AddVoltageSource(2, 1, 4)
                .AddResistor(2, 3, 1)
                .BuildSubcircuit(new[] {1, 2});

            var circuitWithSubcircuit = new CircuitBuilder()
                .AddSubcircuit(new[] {0, 1}, subcircuit)
                .AddResistor(1, 0, 5, "R1")
                .BuildCircuit();
        }

        [Fact]
        public void SubcircuitCanReferenceInnerDevices()
        {
            var subcircuit = builder
                .AddVoltageSource(2, 1, 4)
                .AddResistor(2, 3, 1)
                .BuildSubcircuit(new[] {1, 2});
        }


        [Fact]
        public void TestSimpleSubcircuit()
        {
            var subcircuit = builder
                .AddVoltageSource(2, 1, 4)
                .AddResistor(2, 3, 1)
                .BuildSubcircuit(new[] {1, 2});


            Output.WriteLine("With subcircuit:");
            var circuitWithSubcircuit = new CircuitBuilder()
                .AddSubcircuit(new[] {0, 1}, subcircuit)
                .AddResistor(1, 0, 5)
                .BuildCircuit().GetLargeSignalModel();
            circuitWithSubcircuit.EstablishInitialDcBias();

            Output.WriteLine("Without subcircuit:");
            var originalCircuit = new CircuitBuilder()
                .AddVoltageSource(1, 0, 4)
                .AddResistor(1, 2, 1)
                .AddResistor(2, 0, 5)
                .BuildCircuit().GetLargeSignalModel();
            originalCircuit.EstablishInitialDcBias();

//            Assert.Equal(circuitWithSubcircuit.NodeVoltages[1], originalCircuit.NodeVoltages[2]);    
        }
    }
}