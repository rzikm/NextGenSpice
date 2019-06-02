using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
	/// <summary>Helper class for stamping current controlled current source devices onto the equation system.</summary>
	public class CccsStamper
	{
		private IEquationSystemCoefficientProxy na;
		private IEquationSystemCoefficientProxy nc;

		/// <summary>Registeres the equation system coefficient proxies into the stamper.</summary>
		/// <param name="adapter">The equation system adapter.</param>
		/// <param name="anode">Index of anode terminal.</param>
		/// <param name="cathode">Index of cathode terminal.</param>
		/// <param name="branch">Index of variable containing the reference current.</param>
		public void Register(IEquationSystemAdapter adapter, int anode, int cathode, int branch)
		{
			na = adapter.GetMatrixCoefficientProxy(anode, branch);
			nc = adapter.GetMatrixCoefficientProxy(cathode, branch);
		}

		/// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
		public void Stamp(double gain)
		{
			na.Add(gain);
			nc.Add(-gain);
		}
	}
}