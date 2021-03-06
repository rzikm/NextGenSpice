﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace NextGenSpice.Parser.Test
{
	public class RetokenizerTests
	{
		private IEnumerable<string> Retokenize(string line, int startPos = 0)
		{
			var stream = new TokenStream(new StringReader(line), 0);
			return Utils.Parser.Retokenize(stream.ReadStatement().ToArray(), startPos).Select(c => c.Value);
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