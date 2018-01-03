namespace NextGenSpice.Core.Elements
{
    public class CircuitParameters
    {
        /// <summary>
        /// Convergence aid for some devices.
        /// </summary>
        public double MinimalResistance { get; set; } = 1e-12;
    }
}