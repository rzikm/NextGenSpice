using NextGenSpice.Parser.Test;
using NextGenSpice.Test;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.LargeSignal.Test
{
    public class BjtTests : ParserTestBase
    {
        public BjtTests(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public void SimpleNpn()
        {
            var result = Parse(@"
q1 1 2 0 qmod
rc 2 3 200k
rb 1 3 1k
vcc 3 0 5

.model qmod npn is=1e-16 bf=100

.end");
            var model = result.CircuitDefinition.GetLargeSignalModel();
            model.EstablishDcBias();

            var v1 = model.NodeVoltages[result.NodeIds["1"]];
            var v2 = model.NodeVoltages[result.NodeIds["2"]];
            var v3 = model.NodeVoltages[result.NodeIds["3"]];

            Assert.Equal(2.89653326135722, v1, new DoubleComparer(1e-12));
            Assert.Equal(0.79306652271697, v2, new DoubleComparer(1e-12));
            Assert.Equal(5, v3, new DoubleComparer(1e-12));
        }
    }
}