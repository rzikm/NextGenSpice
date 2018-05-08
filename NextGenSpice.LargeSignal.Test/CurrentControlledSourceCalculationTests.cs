using System;
using NextGenSpice.LargeSignal.Devices;
using NextGenSpice.Parser.Test;
using NextGenSpice.Test;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.LargeSignal.Test
{
    public class CurrentControlledSourceCalculationTests : CalculationTestBase
    {
        public CurrentControlledSourceCalculationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CurrentControlledCurrentSourceTest()
        {
            Parse(@"TITLE
* specify it as the first device to make sure it parses in the unlikely cases
F1 2 22 V1 2
Vmeter 22 3 0
*
V1 1 0 5
R1 1 0 5
R2 1 2 5;
R3 3 0 5
");
            Model.EstablishDcBias();

            var vs = (ITwoTerminalLargeSignalDevice)Model.FindDevice("V1");
            var vMeter = (ITwoTerminalLargeSignalDevice)Model.FindDevice("VMETER");

            Assert.Equal(vs.Current * 2, vMeter.Current);
        }

        [Fact]
        public void CurrentControlledVoltageSourceTest()
        {
            Parse(@"TITLE
* specify it as the first device to make sure it parses in the unlikely cases
H1 2 3 V1 2
*
V1 1 0 5
R1 1 0 5
R2 1 2 5;
R3 3 0 5
");
            Model.EstablishDcBias();

            var v2 = Result.NodeIds["2"];
            var v3 = Result.NodeIds["3"];

            var vs = (ITwoTerminalLargeSignalDevice)Model.FindDevice("V1");

            Assert.Equal(vs.Current * 2, Model.NodeVoltages[v2] - Model.NodeVoltages[v3], new DoubleComparer(1e-10));
        }
    }
}
