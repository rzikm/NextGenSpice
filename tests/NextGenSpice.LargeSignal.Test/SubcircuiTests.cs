using System.Linq;
using NextGenSpice.LargeSignal.Devices;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.LargeSignal.Test
{
	public class SubcircuiTests : CalculationTestBase
	{
		public SubcircuiTests(ITestOutputHelper output) : base(output)
		{
		}

		[Fact]
		public void ModelsInSubcircuitsAreIndependent()
		{
			Parse(@"
.subckt vsource 1 2
v1 1 2 0v
.ends

v1 1 0 5v

r1 1 2 3
r2 1 3 2

x1 2 4 vsource
x2 3 4 vsource

r3 4 0 1
");
			Model.EstablishDcBias();

			var x1 = (LargeSignalSubcircuit) Model.FindDevice("X1");
			var v1 = (ITwoTerminalLargeSignalDevice) x1.Devices.Single();

			var x2 = (LargeSignalSubcircuit) Model.FindDevice("X2");
			var v2 = (ITwoTerminalLargeSignalDevice) x2.Devices.Single();

			Assert.NotEqual(v1, v2);
			Assert.NotEqual(v1.Current, v2.Current);
		}

		[Fact]
		public void TestSameSimulationAsWithoutSubcircuit()
		{
			Parse(@"
i1 1 0 5a
r1 1 2 5OHM
x1 0 1 subcircuit

.subckt subcircuit 1 2
d1 1 2 D
x1 1 2 voltageAlias

.subckt voltageAlias 1 2
v1 1 2 5v
.ends

.ends
");
			Model.EstablishDcBias();
			var v1 = Model.NodeVoltages;

			Parse(@"
i1 1 0 5a
r1 1 2 5OHM
d-x1.d1 0 1 D
v-x1.x1.v1 0 1 5v
");
			Model.EstablishDcBias();
			var v2 = Model.NodeVoltages;

			Assert.Equal(v2, v1);
		}
	}
}