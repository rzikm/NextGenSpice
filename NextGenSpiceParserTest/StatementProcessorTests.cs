using NextGenSpice.Core.Devices;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.Parser.Test
{
    public class StatementProcessorTests : ParserTestBase
    {
        public StatementProcessorTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ParsesInitialConditions()
        {
            var result = Parse(@"
L1 1 0 1M IC=-1M
C1 1 0 1N IC=2M
");
            Assert.Empty(result.Errors);
            var l1 = (Inductor) result.CircuitDefinition.FindDevice("L1");
            var c1 = (Capacitor) result.CircuitDefinition.FindDevice("C1");

            Assert.Equal(-1e-3, l1.InitialCurrent);
            Assert.Equal(2e-3, c1.InitialVoltage);
        }
    }
}