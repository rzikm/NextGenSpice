using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
    /// <summary>Helper class for stamping voltage source devices onto the equation system.</summary>
    public class VoltageStamper
    {
        private IEquationSystemCoefficientProxy br;
        private IEquationSystemCoefficientProxy n13;
        private IEquationSystemCoefficientProxy n23;
        private IEquationSystemCoefficientProxy n31;
        private IEquationSystemCoefficientProxy n32;

        private IEquationSystemSolutionProxy solution;

        /// <summary>Index of the branch variable.</summary>
        public int BranchVariable { get; private set; }

        /// <summary>Registeres the equation system coefficient proxies into the stamper.</summary>
        /// <param name="adapter">The equation system adapter.</param>
        /// <param name="anode">Index of anode terminal.</param>
        /// <param name="cathode">Index of cathode terminal.</param>
        public void Register(IEquationSystemAdapter adapter, int anode, int cathode)
        {
            BranchVariable = adapter.AddVariable();
            n13 = adapter.GetMatrixCoefficientProxy(anode, BranchVariable);
            n23 = adapter.GetMatrixCoefficientProxy(cathode, BranchVariable);
            n31 = adapter.GetMatrixCoefficientProxy(BranchVariable, anode);
            n32 = adapter.GetMatrixCoefficientProxy(BranchVariable, cathode);

            br = adapter.GetRightHandSideCoefficientProxy(BranchVariable);

            solution = adapter.GetSolutionProxy(BranchVariable);
        }

        /// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
        public void Stamp(double voltage)
        {
            n13.Add(1);
            n23.Add(-1);
            n31.Add(1);
            n32.Add(-1);

            br.Add(voltage);
        }

        /// <summary>Gets the solution corresponding to the branch current variable</summary>
        public double GetCurrent()
        {
            return solution.GetValue();
        }
    }
}