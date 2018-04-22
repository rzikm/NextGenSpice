using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Numerics.Equations;
using NextGenSpice.Numerics.Equations.Eq;

namespace NextGenSpice.LargeSignal.Stamping
{
    public class CapacitorStamper
    {
        private IEquationSystemCoefficientProxy nba;
        private IEquationSystemCoefficientProxy nbc;
        private IEquationSystemCoefficientProxy nab;
        private IEquationSystemCoefficientProxy ncb;
        private IEquationSystemCoefficientProxy nbb;

        private IEquationSystemCoefficientProxy nb;

        private IEquationSystemSolutionProxy sol;

        public int BranchVariable { get; private set; }

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

        public void Stamp(double ieq, double geq)
        {
            nba.Add(geq);
            nbc.Add(-geq);
            nab.Add(1);
            ncb.Add(-1);
            nbb.Add(-1);

            nb.Add(ieq);
        }

        public double GetCurrent()
        {
            return sol.GetValue();
        }
    }
}