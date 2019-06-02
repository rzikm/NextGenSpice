using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
	public class CapacitorStamper
	{
		private readonly ConductanceStamper cond = new ConductanceStamper();
		private readonly CurrentStamper current = new CurrentStamper();

		/// <summary>Registeres the equation system coefficient proxies into the stamper.</summary>
		/// <param name="adapter">The equation system adapter.</param>
		/// <param name="anode">Index of anode terminal.</param>
		/// <param name="cathode">Index of cathode terminal.</param>
		public void Register(IEquationSystemAdapter adapter, int anode, int cathode)
		{
			cond.Register(adapter, anode, cathode);
			current.Register(adapter, anode, cathode); // current faces the other way
		}

		/// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
		public void Stamp(double geq, double ieq)
		{
			cond.Stamp(geq);
			current.Stamp(ieq);
		}
	}

	/// <summary>Helper class for stamping capacitor devices onto the equation system.</summary>
	public class CapacitorStamperWithCurrent
	{
		private IEquationSystemCoefficientProxy nab;

		private IEquationSystemCoefficientProxy nb;
		private IEquationSystemCoefficientProxy nba;
		private IEquationSystemCoefficientProxy nbb;
		private IEquationSystemCoefficientProxy nbc;
		private IEquationSystemCoefficientProxy ncb;

		private IEquationSystemSolutionProxy sol;

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
		public void Register(IEquationSystemAdapter adapter, int anode, int cathode)
		{
			nba = adapter.GetMatrixCoefficientProxy(BranchVariable, anode);
			nbc = adapter.GetMatrixCoefficientProxy(BranchVariable, cathode);
			nab = adapter.GetMatrixCoefficientProxy(anode, BranchVariable);
			ncb = adapter.GetMatrixCoefficientProxy(cathode, BranchVariable);
			nbb = adapter.GetMatrixCoefficientProxy(BranchVariable, BranchVariable);

			nb = adapter.GetRightHandSideCoefficientProxy(BranchVariable);

			sol = adapter.GetSolutionProxy(BranchVariable);
		}

		/// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
		public void Stamp(double ieq, double geq)
		{
			nba.Add(geq);
			nbc.Add(-geq);
			nab.Add(1);
			ncb.Add(-1);
			nbb.Add(-1);

			nb.Add(ieq);
		}

		/// <summary>Gets the solution corresponding to the branch current variable</summary>
		public double GetCurrent()
		{
			return sol.GetValue();
		}
	}
}