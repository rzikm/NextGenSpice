using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
    /// <summary>Class for stamping capacitor elements for large signal circuit model.</summary>
    public class LargeSignalCapacitorStamper
    {
        private readonly int anode;
        private readonly int branch;
        private readonly int cathode;

        /// <summary>Initializes a new instance of the <see cref="LargeSignalCapacitorStamper"></see> class.</summary>
        public LargeSignalCapacitorStamper(int anode, int cathode, int branch)
        {
            this.anode = anode;
            this.cathode = cathode;
            this.branch = branch;
        }

        /// <summary>
        ///     Adds entries to the equation system that correspond to capacitor with given equivalent current and
        ///     conductance.
        /// </summary>
        /// <param name="equations">The equation system.</param>
        /// <param name="ieq">Equivalent current of the capacitor in ampers.</param>
        /// <param name="geq">Equivalent conductance of the capacitor in 1/ohms.</param>
        public void Stamp(IEquationEditor equations, double ieq, double geq)
        {
            equations.AddMatrixEntry(branch, anode, geq);
            equations.AddMatrixEntry(branch, cathode, -geq);

            AddBranchCurrent(equations, ieq);
        }

        private void AddBranchCurrent(IEquationEditor equations, double ieq)
        {
            equations.AddMatrixEntry(anode, branch, 1);
            equations.AddMatrixEntry(cathode, branch, -1);

            equations.AddMatrixEntry(branch, branch, -1);

            equations.AddRightHandSideEntry(branch, ieq);
        }

        /// <summary>Adds entries to the equation system that correspond to capacitor with given initial condition.</summary>
        /// <param name="equations">The equation system.</param>
        /// <param name="voltage">The initial voltage in volts for the capacitor or null for equilibrium voltage.</param>
        public void StampInitialCondition(IEquationEditor equations, double? voltage)
        {
            if (voltage.HasValue) equations.AddVoltage(anode, cathode, branch, voltage.Value); // fixed voltage
            else AddBranchCurrent(equations, 0); // model as open circuit
        }
    }
}