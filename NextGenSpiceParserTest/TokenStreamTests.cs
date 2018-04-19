using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Parser;
using Xunit;

namespace NextGenSpiceParserTest
{
    public class TokenStreamTests
    {
        private TokenStream TokenStream { get; set; }

        private void InitInput(string input)
        {
            TokenStream = new TokenStream(new StringReader(input), 0);
        }

        private class TokenComparer : IEqualityComparer<Token>
        {
            public bool Equals(Token x, Token y)
            {
                return Compare(x, y) == 0;
            }

            public int GetHashCode(Token obj)
            {
                return obj.LineColumn.GetHashCode() * obj.LineNumber.GetHashCode() * obj.Value.GetHashCode();
            }

            public int Compare(Token x, Token y)
            {
                var res = 0;
                if ((res = x.LineColumn.CompareTo(y.LineColumn)) != 0)
                    return res;
                if ((res = x.LineNumber.CompareTo(y.LineNumber)) != 0)
                    return res;
                return string.Compare(x.Value, y.Value, StringComparison.Ordinal);
            }
        }

        [Fact]
        public void HandlesMoreLines()
        {
            const string token1 = "First";
            const string token2 = "Second";
            InitInput($"{token1}  \n  {token2}");

            var expected = new Token
            {
                Value = token1.ToUpperInvariant(),
                LineColumn = 1,
                LineNumber = 1
            };

            Assert.Equal(expected, TokenStream.Read(), new TokenComparer());

            expected = new Token
            {
                Value = token2.ToUpperInvariant(),
                LineColumn = 3,
                LineNumber = 2
            };

            Assert.Equal(expected, TokenStream.Read(), new TokenComparer());
        }

        [Fact]
        public void HandlesSplitLine()
        {
            InitInput(@"first second   
* next line should be connected to the previous one
+third fourth");
            Assert.Equal(new[] {"FIRST", "SECOND", "THIRD", "FOURTH"},
                TokenStream.ReadStatement().Select(t => t.Value));
        }

        [Fact]
        public void ReturnsEmptyOnEof()
        {
            InitInput("first  second     third");

            Assert.Equal(new[] {"FIRST", "SECOND", "THIRD"}, TokenStream.ReadStatement().Select(t => t.Value));
            Assert.Equal(0, TokenStream.ReadStatement().Count());
            Assert.Equal(0, TokenStream.ReadStatement().Count());
        }

        [Fact]
        public void ReturnsNullOnEof()
        {
            const string token1 = "First";
            const string token2 = "Second";
            InitInput($"{token1}    {token2}");

            var expected = new Token
            {
                Value = token1.ToUpperInvariant(),
                LineColumn = 1,
                LineNumber = 1
            };

            Assert.Equal(expected, TokenStream.Read(), new TokenComparer());

            expected = new Token
            {
                Value = token2.ToUpperInvariant(),
                LineColumn = token1.Length + 5,
                LineNumber = 1
            };

            Assert.Equal(expected, TokenStream.Read(), new TokenComparer());
            Assert.Equal(null, TokenStream.Read());
        }

        [Fact]
        public void SkipsCommentsOnBegginingOfLine()
        {
            InitInput(@"first second   
* this is a comment

last*comment in the middle
     *comment on last line of file");
            Assert.Equal(new[] {"FIRST", "SECOND"}, TokenStream.ReadStatement().Select(t => t.Value));
            var expected = new Token
            {
                Value = "LAST",
                LineColumn = 1,
                LineNumber = 4
            };
            var nextLine = TokenStream.ReadStatement();
            Assert.Equal(expected, nextLine.Single(), new TokenComparer());
            Assert.Equal(0, TokenStream.ReadStatement().Count());
        }

        [Fact]
        public void SkipsEmptyLine()
        {
            InitInput(@"first second   

last");
            Assert.Equal(new[] {"FIRST", "SECOND"}, TokenStream.ReadStatement().Select(t => t.Value));
            var expected = new Token
            {
                Value = "LAST",
                LineColumn = 1,
                LineNumber = 3
            };
            Assert.Equal(expected, TokenStream.ReadStatement().Single(), new TokenComparer());
            Assert.Equal(0, TokenStream.ReadStatement().Count());
        }
    }
}