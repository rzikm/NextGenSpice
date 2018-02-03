﻿using System.IO;
using System.Linq;
using NextGenSpice.Parser;
using NextGenSpice.Parser.Statements.Devices;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpiceParserTest
{
    public class SpiceCodeParserTests
    {
        public SpiceCodeParserTests(ITestOutputHelper output)
        {
            this.output = output;
            parser = new SpiceCodeParser();
            parser.RegisterElement(new ResistorStatementProcessor());
            parser.RegisterElement(new VoltageSourceStatementProcessor());
        }

        private readonly ITestOutputHelper output;
        private readonly SpiceCodeParser parser;

        private void ExpectErrors(string s)
        {
            var res = ParseString(s);
            foreach (var error in res.Errors)
                output.WriteLine(error.ToString());
        }

        private ParserResult ParseString(string s)
        {
            var sr = new StringReader(s);
            return parser.Parse(new TokenStream(sr));
        }

        [Fact]
        public void RecognisesInputSourceStatement()
        {
            var r = ParseString("V1 0 1 6V");
            Assert.True(!r.Errors.Any());
        }

        [Fact]
        public void RecognisesResistorStatement()
        {
            var r = ParseString("R1 0 1 6ohm");
            Assert.True(!r.Errors.Any());
        }

        [Fact]
        public void RecognisesTransientInputSourceStatement()
        {
            Assert.True(!ParseString("V1 0 1 Exp( 2 3 )").Errors.Any());
            Assert.True(!ParseString("V1 0 1 Exp ( 2 3 )").Errors.Any());
            Assert.True(!ParseString("V1 0 1 Exp (2 3 )").Errors.Any());
            Assert.True(!ParseString("V1 0 1 Exp(2 3)").Errors.Any());
            Assert.True(!ParseString("V1 0 1 Exp 2 3 ").Errors.Any());
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
    }
}