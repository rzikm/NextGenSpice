using NextGenSpice.Core.Test;
using NextGenSpice.LargeSignal.Devices;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.LargeSignal.Test
{
	public class ControlledSourcesCalculationTests : CalculationTestBase
	{
		public ControlledSourcesCalculationTests(ITestOutputHelper output) : base(output)
		{
		}

		[Fact]
		public void CurrentControlledCurrentSourceTest()
		{
			Parse(@"TITLE
F1 2 22 V1 2
Vmeter 22 3 0
*
V1 1 0 5
R1 1 0 5
R2 1 2 5;
R3 3 0 5
");
			Model.EstablishDcBias();

			var vs = (ITwoTerminalLargeSignalDevice) Model.FindDevice("V1");
			var vMeter = (ITwoTerminalLargeSignalDevice) Model.FindDevice("VMETER");

			Assert.Equal(vs.Current * 2, vMeter.Current);
		}


		[Fact]
		public void CurrentControlledVoltageSourceTest()
		{
			Parse(@"TITLE
H1 2 3 V1 2
*
V1 1 0 5
R1 1 0 5
R2 1 2 5;
R3 3 0 5
");
			Model.EstablishDcBias();

			var v2 = Result.NodeIds["2"];
			var v3 = Result.NodeIds["3"];

			var vs = (ITwoTerminalLargeSignalDevice) Model.FindDevice("V1");

			Assert.Equal(vs.Current * 2, Model.NodeVoltages[v2] - Model.NodeVoltages[v3], new DoubleComparer(1e-10));
		}


		[Fact]
		public void VoltageControlledCurrentSourceTest()
		{
			Parse(@"TITLE
G1 2 22 1 0 2
Vmeter 22 3 0
*
V1 1 0 5
R1 1 0 5
R2 1 2 5;
R3 3 0 5
");
			Model.EstablishDcBias();


			var v1 = Result.NodeIds["1"];

			var vMeter = (ITwoTerminalLargeSignalDevice) Model.FindDevice("VMETER");

			Assert.Equal(Model.NodeVoltages[v1] * 2, vMeter.Current);
		}

		[Fact]
		public void VoltageControlledVoltageSourceTest()
		{
			Parse(@"TITLE
E1 2 3 1 0 3
*
V1 1 0 5
R1 1 0 5
R2 1 2 5;
R3 3 0 5
");
			Model.EstablishDcBias();

			var v2 = Result.NodeIds["2"];
			var v3 = Result.NodeIds["3"];


			var v1 = Result.NodeIds["1"];

			Assert.Equal(Model.NodeVoltages[v1] * 3, Model.NodeVoltages[v2] - Model.NodeVoltages[v3],
				new DoubleComparer(1e-10));
		}
	}
}