using System.IO;
using System.Linq;
using NextGenSpice.LargeSignal;
using NextGenSpice.Parser;
using NextGenSpiceTest;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpiceParserTest
{
    public class SubcircuitTests : TracedTestBase
    {
        public SubcircuitTests(ITestOutputHelper output) : base(output)
        {
        }

        public SpiceNetlistParserResult Parse(string code)
        {
            SpiceNetlistParser parser = SpiceNetlistParser.WithDefaults();
            var result = parser.Parse(new StringReader(code));
            Output.WriteLine(string.Join("\n", result.Errors));
            return result;
        }


        [Fact]
        public void AcceptsSimpleSubcircuit()
        {
            var result = Parse(@"
x1 1 0 subcircuit
r1 0 1 5OHM

.subckt subcircuit 1 2
v 1 2 5Volts
.ends

");
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void CanUseCustomModelInsideSubcircuit()
        {
            var result = Parse(@"
v1 1 0 5V
r1 1 2 5OHM
x1 0 1 diodeAlias

.model mydiode D

.subckt diodeAlias 1 2
d1 1 2 mydiode *mydiode is declared outside
.ends

");
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void CannotUseNestedSubcircuitOutside()
        {
            var result = Parse(@"
v1 1 0 5V
r1 1 2 5OHM
x1 0 1 subcircuit
x2 1 2 voltageAlias *this should fail

.subckt subcircuit 1 2
d1 1 2 D
x1 1 2 voltageAlias

.subckt voltageAlias 1 2
v1 1 2 5v
.ends

.ends

");
            Assert.Single(result.Errors);
            var error = result.Errors.Single();
            Assert.Equal(SpiceParserErrorCode.NoSuchSubcircuit, error.ErrorCode);
            Assert.Equal("VOLTAGEALIAS", error.Args[0]);
        }

        [Fact]
        public void CanUsedSubcktInNestedSubckt()
        {
            var result = Parse(@"
v1 1 0 5V
r1 1 2 5OHM
x1 0 1 sub2

.subckt sub2 1 2
d1 1 2 D
x1 1 2 sub1

.subckt sub1 1 2
i1 1 2 5v
.ends

.ends

");
            Assert.Empty(result.Errors);
        }


        [Fact]
        public void CanUseDefaultModelInsideSubcircuit()
        {
            var result = Parse(@"
v1 1 0 5V
r1 1 2 5OHM
x1 0 1 diodeAlias

.subckt diodeAlias 1 2
d1 1 2 D
.ends

");
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void DetectsCurrentCutsetAcrossSubcircuits()
        {
            var result = Parse(@"
i1 1 0 5a
r1 1 2 5OHM
x1 2 3 curAlias
r2 3 0 5Ohm

.subckt curAlias 1 2
r1 1 3 5
i2 3 4 5
r2 4 2 5
.ends


");
            Assert.Single(result.Errors);
            var error = result.Errors.Single();
            Assert.Equal(SpiceParserErrorCode.CurrentBranchCutset, error.ErrorCode);
        }

        [Fact]
        public void DetectsVoltageCycleAcrossSubcircuits()
        {
            var result = Parse(@"
v1 1 0 5V
r1 1 2 5OHM
x1 0 1 voltAlias

.subckt voltAlias 1 2
v1 1 3 5
v2 3 2 4
.ends


");
            Assert.Single(result.Errors);
            var error = result.Errors.Single();
            Assert.Equal(SpiceParserErrorCode.VoltageBranchCycle, error.ErrorCode);
        }

        [Fact]
        public void DoesNotAllowDuplicatesInSubcircuitTerminals()
        {
            var result = Parse(@"
v1 1 0 5V
r1 1 2 5OHM
x1 0 1 subcircuit

.subckt subcircuit 1 1
v1 1 0 5v
.ends
");
            Assert.Single(result.Errors);
            var error = result.Errors.Single();
            Assert.Equal(SpiceParserErrorCode.TerminalNamesNotUnique, error.ErrorCode);
        }

        [Fact]
        public void DoesNotAllowGroundInSubcircuitTerminals()
        {
            var result = Parse(@"
v1 1 0 5V
r1 1 2 5OHM
x1 0 1 subcircuit

.subckt subcircuit 1 0
v1 1 0 5v
.ends
");
            Assert.Single(result.Errors);
            var error = result.Errors.Single();
            Assert.Equal(SpiceParserErrorCode.TerminalToGround, error.ErrorCode);
        }


        [Fact]
        public void ReportsUnconnectedSubcircuit()
        {
            var result = Parse(@"
v1 1 0 5
x1 1 0 subckt

.subckt subckt 1 2
v 1 22 5         *oops forgot to connect to node 2
.ends
");
            Assert.Single(result.Errors);
            var error = result.Errors.Single();
            Assert.Equal(SpiceParserErrorCode.SubcircuitNotConnected, error.ErrorCode);
        }

        [Fact]
        public void SimpleNestedSubcircuit()
        {
            var result = Parse(@"
i1 1 0 5a
r1 1 2 5OHM
x1 0 1 subcircuit

.subckt subcircuit 1 2
d1 1 2 D
x1 1 2 voltageAlias

.subckt voltageAlias 1 2
v1 1 2 5v
.ends

.ends

");
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void TestSameSimulationAsWithoutSubcircuit()
        {
            var v1 = Parse(@"
i1 1 0 5a
r1 1 2 5OHM
x1 0 1 subcircuit

.subckt subcircuit 1 2
d1 1 2 D
x1 1 2 voltageAlias

.subckt voltageAlias 1 2
v1 1 2 5v
.ends

.ends
").CircuitDefinition.GetLargeSignalModel();
            v1.EstablishDcBias();

            var v2 = Parse(@"
i1 1 0 5a
r1 1 2 5OHM
d-x1.d1 0 1 D
v-x1.x1.v1 0 1 5v
").CircuitDefinition.GetLargeSignalModel();
            v2.EstablishDcBias();

            Assert.Equal(v2.NodeVoltages, v1.NodeVoltages);
        }
    }
}