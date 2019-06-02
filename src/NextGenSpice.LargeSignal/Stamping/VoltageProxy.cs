using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
	/// <summary>Class wrapping calls to the equation system solution proxies to simplify getting voltage across the device.</summary>
	public class VoltageProxy
	{
		private IEquationSystemSolutionProxy anode;
		private IEquationSystemSolutionProxy cathode;

		/// <summary>Registeres the equation system solution proxies.</summary>
		/// <param name="adapter">The equation system adapter.</param>
		/// <param name="anode">Index of anode terminal.</param>
		/// <param name="cathode">Index of cathode terminal.</param>
		public void Register(IEquationSystemAdapter adapter, int anode, int cathode)
		{
			this.anode = adapter.GetSolutionProxy(anode);
			this.cathode = adapter.GetSolutionProxy(cathode);
		}

		/// <summary>Gets the voltage accross the registered terminals.</summary>
		/// <returns></returns>
		public double GetValue()
		{
			return anode.GetValue() - cathode.GetValue();
		}
	}
}