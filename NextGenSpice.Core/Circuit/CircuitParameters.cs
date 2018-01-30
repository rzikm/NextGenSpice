namespace NextGenSpice.Core.Elements
{
    /// <summary>
    /// Class for aggregating device independent parameters for the simulation.
    /// </summary>
    public class CircuitParameters
    {
        /// <summary>
        /// Convergence aid for some devices.
        /// </summary>
        public double MinimalResistance { get; set; } = 1e-12;
    }
}