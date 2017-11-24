using System.Diagnostics;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Extensions;
using NextGenSpice.LargeSignal;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpiceTest
{
    public class SubcircuitTests : TracedTestBase
    {
        private readonly CircuitBuilder builder;

        public SubcircuitTests(ITestOutputHelper output) : base(output)
        {
            builder = new CircuitBuilder();
        }

  

        [Fact]
        public void TestSimpleSubcircuit()
        {
            var subcircuit = builder
                .AddVoltageSource(2, 1, 4)
                .AddResistor(2, 3, 1)
                .BuildSubcircuit(new int[] {1, 2});

            
            Output.WriteLine("With subcircuit:");
            var circuitWithSubcircuit = new CircuitBuilder()
                .AddElement(new[] {0, 1}, subcircuit)
                .AddResistor(1, 0, 5)
                .BuildCircuit().GetLargeSignalModel();
            circuitWithSubcircuit.EstablishDcBias();

            Output.WriteLine("Without subcircuit:");
            var originalCircuit = new CircuitBuilder()
                .AddVoltageSource(1, 0, 4)
                .AddResistor(1, 2, 1)
                .AddResistor(2, 0, 5)
                .BuildCircuit().GetLargeSignalModel();
            originalCircuit.EstablishDcBias();

//            Assert.Equal(circuitWithSubcircuit.NodeVoltages[1], originalCircuit.NodeVoltages[2]);    
        }

        [Fact]
        public void SubcircuitCannotReferenceOuterElements()
        {
            var subcircuit = builder
                .AddVoltageSource(2, 1, 4)
                .AddResistor(2, 3, 1)
                .BuildSubcircuit(new int[] { 1, 2 });

            var circuitWithSubcircuit = new CircuitBuilder()
                .AddElement(new[] { 0, 1 }, subcircuit)
                .AddResistor(1, 0, 5, "R1")
                .BuildCircuit();
            

            
        }

        [Fact]
        public void SubcircuitCanReferenceInnerElements()
        {
            var subcircuit = builder
                .AddVoltageSource(2, 1, 4)
                .AddResistor(2, 3, 1)
                .BuildSubcircuit(new int[] { 1, 2 });
        }
    }
}