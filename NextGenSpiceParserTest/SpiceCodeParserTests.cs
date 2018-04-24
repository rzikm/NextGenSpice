using System.IO;
using System.Linq;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
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
        public void DoesNotReturnSubcircuitOnError()
        {
            var res = SpiceNetlistParser.WithDefaults()
                .Parse(new StringReader(@"
.subckt mysub 1 2
v1 1 0 10v
r1 0 2 10ohm
d1 1 1 dmod         * intentionally wrong, nodes 1 and 2 not connected
.model dmod D
.ends
"));
            Assert.Single(res.Errors);
            Assert.Empty(res.Subcircuits);
        }

        [Fact]
        public void RecognisesInputSourceStatement()
        {
            ExpectErrors(c =>
            {
                c.On("TITLE");
                c.On("V1 0 1 5v");
            });
        }

        [Fact]
        public void RecognisesResistorStatement()
        {
            ExpectErrors(c =>
            {
                c.On("TITLE");
                c.On("R1 0 1 6ohm");
            });
        }

        [Fact]
        public void RecognisesTransientInputSourceStatement()
        {
            ExpectErrors(c =>
            {
                c.On("TITLE");
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
                c.On("TITLE");
                c.On("R1 0 1 6.96.wef", SpiceParserErrorCode.NotANumber);
                c.On("R1 0 1 5ohm", SpiceParserErrorCode.DeviceAlreadyDefined);
                c.On("r2 0 R1 5", SpiceParserErrorCode.NotANode);
                c.On("wA R1 R2 42Meg4", SpiceParserErrorCode.UnknownDevice);
                c.On("R2 R1 R2 42Meg4", SpiceParserErrorCode.DeviceAlreadyDefined, SpiceParserErrorCode.NotANode,
                    SpiceParserErrorCode.NotANode, SpiceParserErrorCode.NotANumber);
            });
        }

        [Fact]
        public void ReportsErrorOnTransientInputSource()
        {
            ExpectErrors(c =>
            {
                c.On("TITLE");
                c.On("v 0 1 exp( 2 3 4 4G 9 3 2 04 39 )     * too many", SpiceParserErrorCode.InvalidNumberOfArguments);
                c.On("v3 1 0                                * too few arguments",
                    SpiceParserErrorCode.InvalidNumberOfArguments);
                c.On("v4 1 0    exp 3                       * too few arguments",
                    SpiceParserErrorCode.InvalidNumberOfArguments);
                c.On("v5 1 0    fuc 3                       * unknown function",
                    SpiceParserErrorCode.UnknownTransientFunction);
                c.On("v6 1 0    pwl 0 1 1 3 R 4             * repeat not on breakpoint",
                    SpiceParserErrorCode.NoBreakpointRepetition);
                c.On("v7 1 0    pwl 0 1 -1 3 R              * negative timepoint", SpiceParserErrorCode.NegativeTimepoint);
                c.On("v8 1 0    pwl 0 1 1 3 5              * odd number of pairs",
                    SpiceParserErrorCode.TimePointWithoutValue);
            });
        }

        [Fact]
        public void ReturnsNamesOfNodes()
        {
            var res = SpiceNetlistParser.WithDefaults()
                .Parse(new StringReader(@"
v1 0 stop 5
r1 start stop 5
r2 0 start 10
"));
            Assert.Empty(res.Errors);
            var r1 = res.CircuitDefinition.Devices.Single(d => d.Tag as string == "R1") as ResistorDevice;
            Assert.Equal("START", res.NodeNames[r1.Anode]);
            Assert.Equal("STOP", res.NodeNames[r1.Cathode]);
        }

        [Fact]
        public void ReturnsSubcircuitWhenCorrect()
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
            Assert.Equal("MYSUB", subckt.Tag);
        }

        [Fact]
        public void ReturnsModels()
        {
            var res = SpiceNetlistParser.WithDefaults()
                .Parse(new StringReader($@"
v1 1 0 10v
r1 0 2 10ohm
d1 1 2 dmod
.model dmod D(IS=1)
"));
            Assert.Empty(res.Errors);
            var model = res.Models[typeof(DiodeModelParams)]["DMOD"] as DiodeModelParams;
            Assert.NotNull(model);
            Assert.Equal(1, model.SaturationCurrent);
        }

        [Fact]
        public void ReturnsTitle()
        {
            var title = "my title string";
            var res = SpiceNetlistParser.WithDefaults()
                .Parse(new StringReader($@"{title}
v1 1 0 10v
r1 0 2 10ohm
d1 1 2 dmod
.model dmod D
"));
            Assert.Equal(title, res.Title);
        }
    }
}