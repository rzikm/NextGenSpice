using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
    /// <summary>Helper class for stamping inductor evices onto the equation system.</summary>
    public class InductorStamper
    {
        private readonly CurrentStamper currentStamper;
        private readonly VoltageStamper voltage;

        private IEquationSystemCoefficientProxy n13;
        private IEquationSystemCoefficientProxy n23;
        private IEquationSystemCoefficientProxy n33;

        private IEquationSystemCoefficientProxy r3;


        public InductorStamper()
        {
            voltage = new VoltageStamper();
            currentStamper = new CurrentStamper();
        }


        /// <summary>Index of the branch variable.</summary>
        public int BranchVariable => voltage.BranchVariable;

        /// <summary>Registeres the equation system coefficient proxies into the stamper.</summary>
        /// <param name="adapter">The equation system adapter.</param>
        /// <param name="anode">Index of anode terminal.</param>
        /// <param name="cathode">Index of cathode terminal.</param>
        public void Register(IEquationSystemAdapter adapter, int anode, int cathode)
        {
            voltage.Register(adapter, anode, cathode);
            currentStamper.Register(adapter, anode, cathode);
            n13 = adapter.GetMatrixCoefficientProxy(anode, BranchVariable);
            n23 = adapter.GetMatrixCoefficientProxy(cathode, BranchVariable);
            n33 = adapter.GetMatrixCoefficientProxy(BranchVariable, BranchVariable);

            r3 = adapter.GetRightHandSideCoefficientProxy(BranchVariable);
        }

        /// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
        public void Stamp(double veq, double req)
        {
            voltage.Stamp(veq);
            n33.Add(-req);
        }

        /// <summary>Adds entries to the equation system that correspond to inductor with given initial condition.</summary>
        /// <param name="equations">The equation system.</param>
        /// <param name="current">The initial current in ampers for the inductor or null for equilibrium current.</param>
        public void StampInitialCondition(double? current)
        {
            if (current.HasValue)
            {
                voltage.Stamp(0);
//                n33.Add(1);

//                r3.Add(current.Value);
                currentStamper.Stamp(-current.Value);
            }
            else
                voltage.Stamp(0); // closed circuit
        }

        /// <summary>Gets the solution corresponding to the branch current variable</summary>
        public double GetCurrent()
        {
            return voltage.GetCurrent();
        }
    }
}