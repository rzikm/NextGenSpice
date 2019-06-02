using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
	/// <summary>Helper class for stamping resistor devices onto the equation system.</summary>
	public class ConductanceStamper
	{
		private IEquationSystemCoefficientProxy n11;
		private IEquationSystemCoefficientProxy n12;
		private IEquationSystemCoefficientProxy n21;
		private IEquationSystemCoefficientProxy n22;

		/// <summary>Registeres the equation system coefficient proxies into the stamper.</summary>
		/// <param name="adapter">The equation system adapter.</param>
		/// <param name="anode">Index of anode terminal.</param>
		/// <param name="cathode">Index of cathode terminal.</param>
		public void Register(IEquationSystemAdapter adapter, int anode, int cathode)
		{
			n11 = adapter.GetMatrixCoefficientProxy(anode, anode);
			n12 = adapter.GetMatrixCoefficientProxy(anode, cathode);
			n21 = adapter.GetMatrixCoefficientProxy(cathode, anode);
			n22 = adapter.GetMatrixCoefficientProxy(cathode, cathode);
		}

		/// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
		public void Stamp(double value)
		{
			n11.Add(value);
			n12.Add(-value);
			n22.Add(value);
			n21.Add(-value);
		}
	}
}