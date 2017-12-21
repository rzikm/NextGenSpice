using System.IO;
using System.Linq;
using NextGenSpice;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpiceParserTest
{
    public class SpiceCodeParserTests
    {
        private readonly ITestOutputHelper output;
        private readonly SpiceCodeParser parser;


        public SpiceCodeParserTests(ITestOutputHelper output)
        {
            this.output = output;
            parser = new SpiceCodeParser();
            parser.Register(new ResistorStatementProcessor());
            parser.Register(new VoltageSourceStatementProcessor());
        }

        [Fact]
        public void RecognisesResistorStatement()
        {
            var r = ParseString("R1 0 1 6ohm");
            Assert.NotEqual(typeof(ErrorElementStatement), r.ElementStatements.Single().GetType());
        }

        [Fact]
        public void RecognisesInputSourceStatement()
        {
            var r = ParseString("V1 0 1 6V");
            Assert.NotEqual(typeof(ErrorElementStatement), r.ElementStatements.Single().GetType());
        }

        [Fact]
        public void RecognisesTransientInputSourceStatement()
        {
            Assert.NotEqual(typeof(ErrorElementStatement), ParseString("V1 0 1 Exp( 2 3 )").ElementStatements.Single().GetType());
            Assert.NotEqual(typeof(ErrorElementStatement), ParseString("V1 0 1 Exp ( 2 3 )").ElementStatements.Single().GetType());
            Assert.NotEqual(typeof(ErrorElementStatement), ParseString("V1 0 1 Exp (2 3 )").ElementStatements.Single().GetType());
            Assert.NotEqual(typeof(ErrorElementStatement), ParseString("V1 0 1 Exp(2 3)").ElementStatements.Single().GetType());
            Assert.NotEqual(typeof(ErrorElementStatement), ParseString("V1 0 1 Exp 2 3 ").ElementStatements.Single().GetType());
        }

        [Fact]
        public void ReportsErrorOnTransientInputSource()
        {
            ExpectErrors(@"
v 0 1 exp( 2 3 4 4G 9 3 2 04 39 )     * too many
v1 0 1 exp( 2 3 4 4G 9 3 2 04 39      * unterminated
v3 1 0                                * too few arguments
v4 1 0    exp 3                       * too few arguments
v5 1 0    fuc 3                       * unknown function
v6 1 0    pwl 0 1 1 3 R 4             * repeat not on breakpoint
v7 1 0    pwl 0 1 -1 3 R              * negative timepoint
v7 1 0    pwl 0 1 -1 3 5              * odd number of pairs
");
        }
    

        [Fact]
        public void ReportsErrorOnResistor()
        {
            ExpectErrors(@"
R1 0 1 6.96.wef     * nan
R1 0 1 5ohm         * duplicate
r2 0 R1 5           * not a node
wA R1 R2 42Meg4     * not implemented
R2 R1 R2 42Meg4     * multiple errors");
        }

        private void ExpectErrors(string s)
        {
            var res = ParseString(s);
            foreach (var error in res.Errors)
            {
                output.WriteLine(error.ToString());
            }
        }

        private ParserResult ParseString(string s)
        {
            var sr = new StringReader(s);
            return parser.ParseInputFile(new TokenStream(sr));
        }
    }
}