using System.Linq;
using NextGenSpice.LargeSignal.Devices;
using NextGenSpice.Parser.Test;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.LargeSignal.Test
{
    public class SubcircuiTests : ParserTestBase
    {
        public SubcircuiTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ModelsInSubcircuitsAreIndependent()
        {
            var result = Parse(@"
.subckt vsource 1 2
v1 1 2 0v
.ends

v1 1 0 5v

r1 1 2 3
r2 1 3 2

x1 2 4 vsource
x2 3 4 vsource

r3 4 0 1
");
            var model = result.CircuitDefinition.GetLargeSignalModel();
            model.EstablishDcBias();

            var x1 = (LargeSignalSubcircuit) model.FindDevice("X1");
            var v1 = (ITwoTerminalLargeSignalDevice) x1.Devices.Single(); 

            var x2 = (LargeSignalSubcircuit)model.FindDevice("X2");
            var v2 = (ITwoTerminalLargeSignalDevice)x2.Devices.Single();

            Assert.NotEqual(v1, v2);
            Assert.NotEqual(v1.Current, v2.Current);
        }
    }
}