using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
	/// <summary>Helper class for stamping current controlled voltage source devices onto the equation system.</summary>
	public class CcvsStamper
	{
		private IEquationSystemCoefficientProxy n14;
		private IEquationSystemCoefficientProxy n24;
		private IEquationSystemCoefficientProxy n41;
		private IEquationSystemCoefficientProxy n42;
		private IEquationSystemCoefficientProxy n43;

		private IEquationSystemSolutionProxy solution;

		/// <summary>Index of the branch variable.</summary>
		public int BranchVariable { get; private set; }

		/// <summary>
		///   Registers the branch variable for the voltage source device
		/// </summary>
		/// <param name="adapter"></param>
		public void RegisterVariable(IEquationSystemAdapter adapter)
		{
			BranchVariable = adapter.AddVariable();
		}

		/// <summary>Registeres the equation system coefficient proxies into the stamper.</summary>
		/// <param name="adapter">The equation system adapter.</param>
		/// <param name="anode">Index of anode terminal.</param>
		/// <param name="cathode">Index of cathode terminal.</param>
		public void Register(IEquationSystemAdapter adapter, int anode, int cathode, int refBranch)
		{
			n14 = adapter.GetMatrixCoefficientProxy(anode, BranchVariable);
			n24 = adapter.GetMatrixCoefficientProxy(cathode, BranchVariable);
			n41 = adapter.GetMatrixCoefficientProxy(BranchVariable, anode);
			n42 = adapter.GetMatrixCoefficientProxy(BranchVariable, cathode);
			n43 = adapter.GetMatrixCoefficientProxy(BranchVariable, refBranch);

			solution = adapter.GetSolutionProxy(BranchVariable);
		}

		/// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
		public void Stamp(double gain)
		{
			n14.Add(1);
			n24.Add(-1);
			n41.Add(+1);
			n42.Add(-1);
			n43.Add(-gain);
		}

		/// <summary>Gets the solution corresponding to the branch current variable</summary>
		public double GetCurrent()
		{
			return solution.GetValue();
		}
	}
}