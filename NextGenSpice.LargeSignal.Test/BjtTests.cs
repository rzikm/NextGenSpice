using NextGenSpice.Parser.Test;
using NextGenSpice.Test;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.LargeSignal.Test
{
    public class BjtTests : CalculationTestBase 
    {
        public BjtTests(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public void SimpleNpn()
        {
            Parse(@"
q1 1 2 0 qmod
rc 2 3 200k
rb 1 3 1k
vcc 3 0 5

.Model qmod npn is=1e-16 bf=100

.end");
            Model.EstablishDcBias();

            var v1 = Model.NodeVoltages[Result.NodeIds["1"]];
            var v2 = Model.NodeVoltages[Result.NodeIds["2"]];
            var v3 = Model.NodeVoltages[Result.NodeIds["3"]];

            AssertEqual(2.89653326135722, v1);
            AssertEqual(0.79306652271697, v2);
            AssertEqual(5, v3);

            PrintStatistics(Model, Result);
        }

        [Fact]
        public void SimplePnp()
        {
            Parse(@"
.Model mybjt pnp is=1e-15

vcc 1 0  6V
vin 3 0  sin(0.705 50mV 1kHz 0 0)
rc 1 2 5kOhm
q1 2 3 0 mybjt


.end");
            Model.EstablishDcBias();

            AssertEqual(6, Model.NodeVoltages[1]);
            AssertEqual(2.51299401496814, Model.NodeVoltages[3]);
            AssertEqual(0.705, Model.NodeVoltages[2]);

            PrintStatistics(Model, Result);
        }
    }
}