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

        public SubcircuitTests(ITestOutputHelper output)
        {
            traceListener = new MyTraceListener(output);
            Trace.Listeners.Add(traceListener);

            builder = new CircuitBuilder();
        }

        [Fact]
        public void CanAddClonedElement()
        {
            var device = new ResistorElement(5);
            builder.AddElement(new[] { 1, 2 }, device);
            builder.AddElement(new[] { 1, 2 }, device.Clone());
        }

        [Fact]
        public void TestSimpleSubcircuit()
        {
            var subcircuit = builder
                .AddVoltageSource(2, 1, 4)
                .AddResistor(2, 3, 1)
                .BuildSubcircuit(new int[] {1, 2});

            var circuitWithSubcircuit = new CircuitBuilder()
                .AddElement(new[] {0, 1}, subcircuit)
                .AddResistor(1, 0, 5)
                .BuildCircuit().GetLargeSignalModel();
            circuitWithSubcircuit.EstablishDcBias();

            var originalCircuit = new CircuitBuilder()
                .AddVoltageSource(1, 0, 4)
                .AddResistor(1, 2, 1)
                .AddResistor(2, 0, 5)
                .BuildCircuit().GetLargeSignalModel();
            originalCircuit.EstablishDcBias();

            Assert.Equal(circuitWithSubcircuit.NodeVoltages[1], originalCircuit.NodeVoltages[2]);    
        }
    }
}