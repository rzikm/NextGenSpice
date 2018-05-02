using System;
using NextGenSpice.LargeSignal.Devices;
using NextGenSpice.Parser.Test;
using NextGenSpice.Test;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.LargeSignal.Test
{
    public class CurrentControlledSourceCalculationTests : ParserTestBase
    {
        public CurrentControlledSourceCalculationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CurrentControlledCurrentSourceTest()
        {
            var result = Parse(@"TITLE
* specify it as the first device to make sure it parses in the unlikely cases
F1 2 22 V1 2
Vmeter 22 3 0
*
V1 1 0 5
R1 1 0 5
R2 1 2 5;
R3 3 0 5
");
            Assert.Empty(result.Errors);
            var model = result.CircuitDefinition.GetLargeSignalModel();
            model.EstablishDcBias();

            var vs = (ITwoTerminalLargeSignalDevice)model.FindDevice("V1");
            var vMeter = (ITwoTerminalLargeSignalDevice)model.FindDevice("VMETER");

            Assert.Equal(vs.Current * 2, vMeter.Current);
        }

        [Fact]
        public void CurrentControlledVoltageSourceTest()
        {
            var result = Parse(@"TITLE
* specify it as the first device to make sure it parses in the unlikely cases
H1 2 3 V1 2
*
V1 1 0 5
R1 1 0 5
R2 1 2 5;
R3 3 0 5
");
            Assert.Empty(result.Errors);
            var model = result.CircuitDefinition.GetLargeSignalModel();
            model.EstablishDcBias();

            var v2 = result.NodeIds["2"];
            var v3 = result.NodeIds["3"];

            var vs = (ITwoTerminalLargeSignalDevice)model.FindDevice("V1");

            Assert.Equal(vs.Current * 2, model.NodeVoltages[v2] - model.NodeVoltages[v3], new DoubleComparer(1e-10));
        }
    }
}
