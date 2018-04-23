using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
    /// <summary>Helper class for stamping capacitor devices onto the equation system.</summary>
    public class CapacitorStamper
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

        /// <summary>Registeres the equation system coefficient proxies into the stamper.</summary>
        /// <param name="adapter">The equation system adapter.</param>
        /// <param name="anode">Index of anode terminal.</param>
        /// <param name="cathode">Index of cathode terminal.</param>
        public void Register(IEquationSystemAdapter adapter, int anode, int cathode)
        {
            BranchVariable = adapter.AddVariable();

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