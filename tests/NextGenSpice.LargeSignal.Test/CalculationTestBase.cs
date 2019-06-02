using NextGenSpice.Core.Test;
using NextGenSpice.Parser;
using NextGenSpice.Parser.Test;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.LargeSignal.Test
{
	public class CalculationTestBase : ParserTestBase
	{
		private readonly DoubleComparer comparer = new DoubleComparer(1e-5);

		public CalculationTestBase(ITestOutputHelper output) : base(output)
		{
		}

		protected SpiceNetlistParserResult Result { get; private set; }

		protected LargeSignalCircuitModel Model { get; private set; }

		protected void PrintStatistics(LargeSignalCircuitModel model, SpiceNetlistParserResult result)
		{
			Output.WriteLine($"Iterations: {model.LastNonLinearIterationCount}");
			for (var i = 0; i < result.NodeNames.Count; i++)
				Output.WriteLine($"V({result.NodeNames[i]}) = {model.NodeVoltages[i]}");
		}

		protected void AssertEqual(double expected, double actual)
		{
			Assert.Equal(expected, actual, comparer);
		}

		protected new void Parse(string netlist)
		{
			Result = base.Parse(netlist);

			Assert.Empty(Result.Errors);

			Model = Result.CircuitDefinition.GetLargeSignalModel();
		}

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public override void Dispose()
		{
			base.Dispose();
			if (Model != null)
				PrintStatistics(Model, Result);
		}
	}
}