using NextGenSpice.Core.Devices;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.Parser.Test
{
	public class CurrentControlledSources : ParserTestBase
	{
		public CurrentControlledSources(ITestOutputHelper output) : base(output)
		{
		}

		[Fact]
		public void ParsesCccs()
		{
			var result = Parse(@"TITLE
* specify it as the first device to make sure it parses in the unlikely cases
F1 2 3 V1 1
*
V1 1 0 5
R1 1 0 5
R2 1 2 5;
R3 3 0 5
");
			Assert.Empty(result.Errors);

			var f1 = (Cccs) result.CircuitDefinition.FindDevice("F1");

			Assert.Equal(1, f1.Gain);
		}

		[Fact]
		public void ParsesCcvs()
		{
			var result = Parse(@"TITLE
* specify it as the first device to make sure it parses in the unlikely cases
H1 2 3 V1 1
*
V1 1 0 5
R1 1 0 5
R2 1 2 5;
R3 3 0 5
");

			Assert.Empty(result.Errors);

			var f1 = (Ccvs) result.CircuitDefinition.FindDevice("H1");

			Assert.Equal(1, f1.Gain);
			;
		}

		[Fact]
		public void ReturnErrorWhenIsNotSource()
		{
			ExpectErrors(c =>
			{
				c.On("TITLE");
				c.On("F1 2 3 R1 1", SpiceParserErrorCode.NotVoltageSource); // Resistor
				c.On("H1 2 3 1 1", SpiceParserErrorCode.NotVoltageSource); // not a device
				c.On("*");
				c.On("V1 1 0 5");
				c.On("R1 1 0 5");
				c.On("R2 1 2 5;");
				c.On("R3 3 0 5");
			});
		}
	}
}