using System.IO;
using System.Linq;
using NextGenSpice.Parser;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpiceParserTest
{
    public class SpiceCodeParserTests : ParserTestBase
    {
        public SpiceCodeParserTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void RecognisesInputSourceStatement()
        {
            ExpectErrors(c => { c.On("V1 0 1 5v"); });
        }

        [Fact]
        public void RecognisesResistorStatement()
        {
            ExpectErrors(c => { c.On("R1 0 1 6ohm"); });
        }

        [Fact]
        public void RecognisesTransientInputSourceStatement()
        {
            ExpectErrors(c =>
            {
                c.On("V1 0 1 Exp( 2 3 )");
                c.On("V2 0 2 Exp ( 2 3 )");
                c.On("V3 0 3 Exp (2 3 )");
                c.On("V4 0 4 Exp(2 3)");
                c.On("V5 0 5 Exp 2 3 ");
            });
        }


        [Fact]
        public void ReportsErrorOnResistor()
        {
            ExpectErrors(c =>
            {
                c.On("R1 0 1 6.96.wef", SpiceParserError.NotANumber);
                c.On("R1 0 1 5ohm", SpiceParserError.DeviceAlreadyDefined);
                c.On("r2 0 R1 5", SpiceParserError.NotANode);
                c.On("wA R1 R2 42Meg4", SpiceParserError.UnknownDevice);
                c.On("R2 R1 R2 42Meg4", SpiceParserError.DeviceAlreadyDefined, SpiceParserError.NotANode,
                    SpiceParserError.NotANode, SpiceParserError.NotANumber);
            });
        }

        [Fact]
        public void ReportsErrorOnTransientInputSource()
        {
            ExpectErrors(c =>
            {
                c.On("v 0 1 exp( 2 3 4 4G 9 3 2 04 39 )     * too many", SpiceParserError.InvalidNumberOfArguments);
                c.On("v3 1 0                                * too few arguments",
                    SpiceParserError.InvalidNumberOfArguments);
                c.On("v4 1 0    exp 3                       * too few arguments",
                    SpiceParserError.InvalidNumberOfArguments);
                c.On("v5 1 0    fuc 3                       * unknown function",
                    SpiceParserError.UnknownTransientFunction);
                c.On("v6 1 0    pwl 0 1 1 3 R 4             * repeat not on breakpoint",
                    SpiceParserError.NoBreakpointRepetition);
                c.On("v7 1 0    pwl 0 1 -1 3 R              * negative timepoint", SpiceParserError.NegativeTimepoint);
                c.On("v8 1 0    pwl 0 1 1 3 5              * odd number of pairs",
                    SpiceParserError.TimePointWithoutValue);
            });
        }

        [Fact]
        public void ReturnsSubcircuits()
        {
            var res = SpiceNetlistParser.WithDefaults()
                .Parse(new StringReader(@"
.subckt mysub 1 2
v1 1 0 10v
r1 0 2 10ohm
d1 1 2 dmod
.model dmod D
.ends
"));
            Assert.Empty(res.Errors);
            var subckt = res.Subcircuits.Single();
            Assert.NotNull(subckt);
            Assert.Equal("MYSUB", subckt.SubcircuitName);
        }
    }
}