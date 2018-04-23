using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;
using NextGenSpice.Numerics.Equations.Eq;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Class for stamping inductor devices for large signal circuit model.</summary>
    public class InductorStamper
    {
        private VoltageStamper voltage;
        private CurrentStamper currentStamper;

        private IEquationSystemCoefficientProxy n33;

        public InductorStamper()
        {
            voltage = new VoltageStamper();
            currentStamper = new CurrentStamper();
        }

        int BranchVariable => voltage.BranchVariable;

        public void Register(IEquationSystemAdapter adapter, int anode, int cathode)
        {
            voltage.Register(adapter, anode, cathode);
            currentStamper.Register(adapter, anode, cathode);
            n33 = adapter.GetMatrixCoefficientProxy(BranchVariable, BranchVariable);
        }

        /// <summary>Adds entries to the equation system that correspond to inductor with given equivalent current and conductance.</summary>
        /// <param name="equations">The equation system.</param>
        /// <param name="ieq">Equivalent current of the inductor in ampers.</param>
        /// <param name="req">Equivalent resistance of the inductor in ohms.</param>
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
                currentStamper.Stamp(current.Value);
            else
                voltage.Stamp(0); // closed circuit
        }

        public double GetCurrent()
        {
            return voltage.GetCurrent();
        }
    }

    public class VoltageStamper
    {
        private IEquationSystemCoefficientProxy n13;
        private IEquationSystemCoefficientProxy n23;
        private IEquationSystemCoefficientProxy n31;
        private IEquationSystemCoefficientProxy n32;

        private IEquationSystemCoefficientProxy br;

        private IEquationSystemSolutionProxy solution;
        public int BranchVariable { get; private set; }
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

        public void Stamp(double voltage)
        {
            n13.Add(1);
            n23.Add(-1);
            n31.Add(1);
            n32.Add(-1);

            br.Add(voltage);
        }

        public double GetCurrent()
        {
            return solution.GetValue();
        }
    }
}