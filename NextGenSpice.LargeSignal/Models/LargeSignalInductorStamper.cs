using NextGenSpice.Core.Equations;
using NextGenSpice.LargeSignal.Stamping;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Class for stamping inductor elements for large signal circuit model.</summary>
    public class LargeSignalInductorStamper
    {
        private readonly int anode;
        private readonly int branch;
        private readonly int cathode;

        /// <summary>Initializes a new instance of the <see cref="LargeSignalCapacitorStamper"></see> class.</summary>
        public LargeSignalInductorStamper(int anode, int cathode, int branch)
        {
            this.anode = anode;
            this.cathode = cathode;
            this.branch = branch;
        }

        /// <summary>Adds entries to the equation system that correspond to inductor with given equivalent current and conductance.</summary>
        /// <param name="equations">The equation system.</param>
        /// <param name="ieq">Equivalent current of the inductor in ampers.</param>
        /// <param name="req">Equivalent resistance of the inductor in ohms.</param>
        public void Stamp(IEquationEditor equations, double veq, double req)
        {
            equations.AddVoltage(anode, cathode, branch, -veq);
            equations.AddMatrixEntry(branch, branch, -req);
        }

        /// <summary>Adds entries to the equation system that correspond to inductor with given initial condition.</summary>
        /// <param name="equations">The equation system.</param>
        /// <param name="current">The initial current in ampers for the inductor or null for equilibrium current.</param>
        public void StampInitialCondition(IEquationEditor equations, double? current)
        {
            if (current.HasValue)
                equations.AddCurrent(anode, cathode, current.Value);
            else
                equations.AddVoltage(anode, cathode, branch, 0);
        }
    }
}