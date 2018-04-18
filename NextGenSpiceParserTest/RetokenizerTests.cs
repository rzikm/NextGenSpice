using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Parser;
using NextGenSpice.Core.Parser.Utils;
using Xunit;

namespace NextGenSpiceParserTest
{
    public class RetokenizerTests
    {
        private IEnumerable<string> Retokenize(string line, int startPos = 0)
        {
            TokenStream stream = new TokenStream(new StringReader(line));
            return Helper.Retokenize(stream.ReadLogicalLine().ToArray(), startPos).Select(c => c.Value);
        }

        [Fact]
        public void HandlesWithoutParentheses()
        {
            Assert.Equal(new[] {"SIN", "1", "(1", "0)"}, Retokenize("SIN 1 (1 0)"));
            Assert.Equal(new[] {"SIN", "(1", "0"}, Retokenize("SIN (1 0"));
            Assert.Equal(new[] {"SIN", "1", "0"}, Retokenize("SIN 1 0"));
            Assert.Equal(new[] {"EXP", "2", "3"}, Retokenize("V1 0 1 Exp 2 3 ", 3));
        }

        [Fact]
        public void HandlesWithParentheses()
        {
            Assert.Equal(new[] {"SIN", "1"}, Retokenize("SIN(1)"));
            Assert.Equal(new[] {"SIN", "1"}, Retokenize("SIN (1)"));
            Assert.Equal(new[] {"SIN", "1", "0"}, Retokenize("SIN(1 0)"));
            Assert.Equal(new[] {"SIN", "1", "0"}, Retokenize("SIN (1 0)"));
            Assert.Equal(new[] {"SIN", "1", "0"}, Retokenize("SIN( 1 0 )"));
            Assert.Equal(new[] {"SIN", "1", "0"}, Retokenize("SIN ( 1 0 )"));
        }
    }
}