using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NextGenSpice.Test;
using Xunit.Abstractions;

namespace NextGenSpice.Parser.Test
{
    public class ParserTestBase : TracedTestBase
    {
        protected ParserTestBase(ITestOutputHelper output) : base(output)
        {
            DoTrace = false;
            Parser = SpiceNetlistParser.WithDefaults();
        }

        protected SpiceNetlistParser Parser { get; }

        protected void ExpectErrors(Action<IParserCaseBuilder> caseBuilder)
        {
            var p = new ParserCaseBuilder();
            caseBuilder(p);
            p.Assert(Output);
        }

        protected interface IParserCaseBuilder
        {
            void On(string line, params SpiceParserErrorCode[] expectedErrors);
        }

        class ParserCaseBuilder : IParserCaseBuilder
        {
            private readonly StringBuilder sb;
            private readonly List<(string line, SpiceParserErrorCode[] errors)> specs;

            public ParserCaseBuilder()
            {
                specs = new List<(string line, SpiceParserErrorCode[] errors)> {("", new SpiceParserErrorCode[0])};
                sb = new StringBuilder();
            }

            public void On(string line, params SpiceParserErrorCode[] expectedErrors)
            {
                Array.Sort(expectedErrors);
                specs.Add((line, expectedErrors));
                sb.AppendLine(line);
            }

            public void Assert(ITestOutputHelper output)
            {
                var parser = SpiceNetlistParser.WithDefaults();
                var result = parser.Parse(new StringReader(sb.ToString())).Errors
                    .ToLookup(i => i.LineNumber, i => i.ErrorCode);

                for (int i = 0; i < specs.Count; i++)
                {
                    var expected = specs[i].errors;
                    var actual = result[i].OrderBy(e => e).ToArray();

                    Xunit.Assert.True(expected.SequenceEqual(actual),
                        $@"line {i}: {specs[i].line}
Expected: [{string.Join(", ", expected)}]
Actual: [{string.Join(", ", actual)}]
");
                }
            }
        }
    }
}