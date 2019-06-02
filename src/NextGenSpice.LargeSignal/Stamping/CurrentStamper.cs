using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
	/// <summary>Helper class for stamping current source devices onto the equation system.</summary>
	public class CurrentStamper
	{
		private IEquationSystemCoefficientProxy anode;
		private IEquationSystemCoefficientProxy cathode;

		/// <summary>Registeres the equation system coefficient proxies into the stamper.</summary>
		/// <param name="adapter">The equation system adapter.</param>
		/// <param name="anode">Index of anode terminal.</param>
		/// <param name="cathode">Index of cathode terminal.</param>
		public void Register(IEquationSystemAdapter adapter, int anode, int cathode)
		{
			this.anode = adapter.GetRightHandSideCoefficientProxy(anode);
			this.cathode = adapter.GetRightHandSideCoefficientProxy(cathode);
		}

		/// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
		public void Stamp(double current)
		{
			anode.Add(-current);
			cathode.Add(current);
		}
	}
}