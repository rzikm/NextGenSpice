using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
    /// <summary>Helper class for stamping voltage controlled voltage sources devices onto the equation system.</summary>
    public class VcvsStamper
    {
        private IEquationSystemSolutionProxy cur;

        private IEquationSystemCoefficientProxy nab;
        private IEquationSystemCoefficientProxy nba;
        private IEquationSystemCoefficientProxy nbc;
        private IEquationSystemCoefficientProxy nbra;
        private IEquationSystemCoefficientProxy nbrc;
        private IEquationSystemCoefficientProxy ncb;

        public int BranchVariable { get; private set; }

        /// <summary>Registeres the equation system coefficient proxies into the stamper.</summary>
        /// <param name="adapter">The equation system adapter.</param>
        /// <param name="anode">Index of anode terminal.</param>
        /// <param name="cathode">Index of cathode terminal.</param>
        /// <param name="ranode">Index of reference anode terminal.</param>
        /// <param name="rcathode">Index of reference cathode terminal.</param>
        public void Register(IEquationSystemAdapter adapter, int anode, int cathode, int ranode, int rcathode)
        {
            BranchVariable = adapter.AddVariable();
            nab = adapter.GetMatrixCoefficientProxy(anode, BranchVariable);
            ncb = adapter.GetMatrixCoefficientProxy(cathode, BranchVariable);
            nba = adapter.GetMatrixCoefficientProxy(BranchVariable, anode);
            nbc = adapter.GetMatrixCoefficientProxy(BranchVariable, cathode);
            nbra = adapter.GetMatrixCoefficientProxy(BranchVariable, ranode);
            nbrc = adapter.GetMatrixCoefficientProxy(BranchVariable, rcathode);

            cur = adapter.GetSolutionProxy(BranchVariable);
        }

        /// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
        public void Stamp(double gain)
        {
            nab.Add(1);
            ncb.Add(-1);
            nba.Add(1);
            nbc.Add(-1);
            nbra.Add(gain);
            nbrc.Add(gain);
        }

        /// <summary>Gets the solution corresponding to the branch current variable</summary>
        public double GetCurrent()
        {
            return cur.GetValue();
        }
    }
}