namespace NextGenSpice.Core.Elements
{
    /// <summary>
    /// Class that represents a capacitor device.
    /// </summary>
    public class CapacitorElement : TwoNodeCircuitElement
    {
        /// <summary>
        /// Capacity in farads.
        /// </summary>
        public double Capacity { get; }

        /// <summary>
        /// Initial voltage across the capacitor in volts.
        /// </summary>
        public double? InitialVoltage { get; }

        public CapacitorElement(double capacity, double? initialVoltage = null, string name = null) : base(name)
        {
            this.Capacity = capacity;
            InitialVoltage = initialVoltage;
        }
    }
}