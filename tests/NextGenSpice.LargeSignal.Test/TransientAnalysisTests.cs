using System.Linq;
using NextGenSpice.Core.Test;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.LargeSignal.Test
{
	public class TransientAnalysisTests : TracedTestBase
	{
		public TransientAnalysisTests(ITestOutputHelper output) : base(output)
		{
			DoTrace = false;
		}


		[Fact]
		public void TestTimeSimulationDoesNotChangeResultWhenUsingCapacitor()
		{
			var circuit = CircuitGenerator.GetSimpleCircuitWithCapacitor();

			var model = circuit.GetLargeSignalModel();
			model.EstablishDcBias();
			Output.PrintCircuitStats(model);

			var expected = model.NodeVoltages.ToArray();
			model.AdvanceInTime(1e-6);
			Output.PrintCircuitStats(model);

			Assert.Equal(expected, model.NodeVoltages);
		}

		[Fact]
		public void TestTimeSimulationDoesNotChangeResultWhenUsingInductor()
		{
			var circuit = CircuitGenerator.GetSimpleCircuitWithInductor();

			var model = circuit.GetLargeSignalModel();
			model.EstablishDcBias();
			Output.PrintCircuitStats(model);

			var expected = model.NodeVoltages.ToArray();
			model.AdvanceInTime(1e-6);
			Output.PrintCircuitStats(model);

			Assert.Equal(expected, model.NodeVoltages, new DoubleComparer(1e-4));
		}
	}
}