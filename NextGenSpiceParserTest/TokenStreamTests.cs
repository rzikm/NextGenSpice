using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice;
using Xunit;

namespace NextGenSpiceParserTest
{
    public class TokenStreamTests
    {
        public TokenStream TokenStream { get; set; }

        private void InitInput(string input)
        {
            TokenStream = new TokenStream(new StringReader(input));
        }

        [Fact]
        public void SkipsCommentsOnBegginingOfLine()
        {
            InitInput(@"first second   
* this is a comment

last*comment in the middle
     *comment on last line of file");
            Assert.Equal(new[] { "first", "second" }, TokenStream.ReadLogicalLine().Select(t => t.Value));
            var expected = new Token
            {
                Value = "last",
                Char = 1,
                Line = 4
            };
            var nextLine = TokenStream.ReadLogicalLine();
            Assert.Equal(expected, nextLine.Single(), new TokenComparer());
            Assert.Equal(0, TokenStream.ReadLogicalLine().Count());
        }

        [Fact]
        public void ReturnsEmptyOnEof()
        {
            InitInput("first  second     third");

            Assert.Equal(new[]{"first", "second", "third"}, TokenStream.ReadLogicalLine().Select(t => t.Value));
            Assert.Equal(0, TokenStream.ReadLogicalLine().Count());
            Assert.Equal(0, TokenStream.ReadLogicalLine().Count());
        }

        [Fact]
        public void SkipsEmptyLine()
        {
            InitInput(@"first second   

last");
            Assert.Equal(new[] { "first", "second" }, TokenStream.ReadLogicalLine().Select(t => t.Value));
            var expected = new Token
            {
                Value = "last",
                Char = 1,
                Line = 3
            };
            Assert.Equal(expected, TokenStream.ReadLogicalLine().Single(), new TokenComparer());
            Assert.Equal(0, TokenStream.ReadLogicalLine().Count());
        }

        [Fact]
        public void ReturnsNullOnEof()
        {
            const string token1 = "First";
            const string token2 = "Second";
            InitInput($"{token1}    {token2}");

            var expected = new Token
            {
                Value = token1,
                Char = 1,
                Line = 1
            };

            Assert.Equal(expected, TokenStream.Read(), new TokenComparer());

            expected = new Token
            {
                Value = token2,
                Char = token1.Length + 5,
                Line = 1
            };

            Assert.Equal(expected, TokenStream.Read(), new TokenComparer());
            Assert.Equal(null, TokenStream.Read());
        }

        [Fact]
        public void HandlesMoreLines()
        {
            const string token1 = "First";
            const string token2 = "Second";
            InitInput($"{token1}  \n  {token2}");

            var expected = new Token
            {
                Value = token1,
                Char = 1,
                Line = 1
            };

            Assert.Equal(expected, TokenStream.Read(), new TokenComparer());

            expected = new Token
            {
                Value = token2,
                Char = 3,
                Line = 2
            };

            Assert.Equal(expected, TokenStream.Read(), new TokenComparer());

        }

        class TokenComparer : IEqualityComparer<Token>
        {
            public int Compare(Token x, Token y)
            {
                var res = 0;
                if ((res = x.Char.CompareTo(y.Char)) != 0)
                    return res;
                if ((res = x.Line.CompareTo(y.Line)) != 0)
                    return res;
                return string.Compare(x.Value, y.Value, StringComparison.Ordinal);
            }

            public bool Equals(Token x, Token y)
            {
                return Compare(x, y) == 0;
            }

            public int GetHashCode(Token obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
