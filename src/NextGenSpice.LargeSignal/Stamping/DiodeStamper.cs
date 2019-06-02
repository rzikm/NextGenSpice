using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Stamping
{
    /// <summary>Helper class for stamping diode devices onto the equation system.</summary>
    internal class DiodeStamper
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
            current.Register(adapter, cathode, anode); // current faces the other way
        }

        /// <summary>Stamps the device characteristics onto the equation system through the registered proxies.</summary>
        public void Stamp(double geq, double ieq)
        {
            cond.Stamp(geq);
            current.Stamp(ieq);
        }
    }
}