using NextGenSpice.LargeSignal.Devices;
using Xunit;
using Xunit.Abstractions;

namespace NextGenSpice.LargeSignal.Test
{
	public class DiodeTests : CalculationTestBase
	{
		public DiodeTests(ITestOutputHelper output) : base(output)
		{
		}

		[Fact]
		private void DoubleDoublePrecisionCircuitTest()
		{
			Parse(@"
V1 IN 0 SIN(0 5 100 0 0 0)
D1 IN A 1n4148
R1 A B 1e-6
D2 0 B 1n4148

*.model D1N4148 D (IS=2.52e-9 N=1.752 TT=2e-8 CJO=9e-13 M=0.25 VJ=20 BV=75 RS=0.568)

.model 1N4148 D(Is=2.52n Rs=.568 N=1.752 Cjo=4p M=.4 tt=20n VJ=20 BV=75)
*.print tran V(IN) I(D1)
");
			Model.EstablishDcBias();

			var v1 = (ITwoTerminalLargeSignalDevice) Model.FindDevice("V1");
			var d1 = (ITwoTerminalLargeSignalDevice) Model.FindDevice("D1");
		}
	}
}