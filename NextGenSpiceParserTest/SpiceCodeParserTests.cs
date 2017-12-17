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
        }

        [Fact]
        public void RecognisesResistorStatement()
        {
            var r = ParseString("R1 0 1 6ohm");
            Assert.NotEqual(typeof(ErrorElementStatement), r.ElementStatements.Single().GetType());
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
            foreach (var st in res.ElementStatements)
            {
                Assert.Equal(typeof(ErrorElementStatement), st.GetType());
                foreach (var error in st.GetErrors())
                {
                    output.WriteLine(error.ToString());
                }
            }
        }

        private ParserResult ParseString(string s)
        {
            var sr = new StringReader(s);
            return parser.ParseInputFile(new TokenStream(sr));
        }
    }
}