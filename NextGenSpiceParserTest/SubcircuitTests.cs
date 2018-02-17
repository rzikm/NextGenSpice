using System.IO;
using System.Linq;
using NextGenSpice.LargeSignal;
using NextGenSpice.Parser;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpiceParserTest
{
    public class SubcircuitTests
    {
        private ITestOutputHelper output;

        public SubcircuitTests(ITestOutputHelper output)
        {
            this.output = output;
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
        public void CannotUseCustomModelInsideSubcircuit()
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
            Assert.Single(result.Errors);
            var message = result.Errors.Single().Messsage;
            Assert.Contains("MYDIODE", message);
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
        public void SimpleNestedSubcircuit()
        {
            var result = Parse(@"
v1 1 0 5V
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
            var message = result.Errors.Single().Messsage;
            Assert.Contains("VOLTAGEALIAS", message);
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
            var message = result.Errors.Single().Messsage;
            Assert.Contains("connecting node sets", message);
        }

        public ParserResult Parse(string code)
        {
            SpiceCodeParser parser = new SpiceCodeParser();
            var result = parser.Parse(new TokenStream(new StringReader(code)));
            output.WriteLine(string.Join("\n", result.Errors));
            return result;
        }

        [Fact]
        public void TestSameSimulationAsWithoutSubcircuit()
        {
            var v1 = Parse(@"
v1 1 0 5V
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
v1 1 0 5V
r1 1 2 5OHM
d-x1.d1 0 1 D
v-x1.x1.v1 0 1 5v
").CircuitDefinition.GetLargeSignalModel();
            v2.EstablishDcBias();

            Assert.Equal(v2.NodeVoltages, v1.NodeVoltages);
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
            var message = result.Errors.Single().Messsage;
            Assert.Contains("ground node", message);
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
            var message = result.Errors.Single().Messsage;
            Assert.Contains("must be unique", message);
        }
    }
}