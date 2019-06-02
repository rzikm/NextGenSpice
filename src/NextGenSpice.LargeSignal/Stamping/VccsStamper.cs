using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
	/// <summary>Helper class for stamping voltage controlled current source devices onto the equation system.</summary>
	public class VccsStamper
	{
		private IEquationSystemCoefficientProxy nara;
		private IEquationSystemCoefficientProxy narc;
		private IEquationSystemCoefficientProxy ncra;
		private IEquationSystemCoefficientProxy ncrc;

		/// <summary>Registeres the equation system coefficient proxies into the stamper.</summary>
		/// <param name="adapter">The equation system adapter.</param>
		/// <param name="anode">Index of anode terminal.</param>
		/// <param name="cathode">Index of cathode terminal.</param>
		/// <param name="ranode">Index of reference anode terminal.</param>
		/// <param name="rcathode">Index of reference cathode terminal.</param>
		public void Register(IEquationSystemAdapter adapter, int anode, int cathode, int ranode, int rcathode)
		{
			nara = adapter.GetMatrixCoefficientProxy(anode, ranode);
			ncra = adapter.GetMatrixCoefficientProxy(cathode, ranode);
			narc = adapter.GetMatrixCoefficientProxy(anode, rcathode);
			ncrc = adapter.GetMatrixCoefficientProxy(cathode, rcathode);
		}

		/// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
		public void Stamp(double gain)
		{
			nara.Add(gain);
			ncra.Add(-gain);
			narc.Add(-gain);
			ncrc.Add(gain);
		}
	}
}