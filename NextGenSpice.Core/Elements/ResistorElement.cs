namespace NextGenSpice.Core.Elements
{
    /// <summary>
    /// Class that represents a resistor device.
    /// </summary>
    public class ResistorElement : TwoNodeCircuitElement
    {
        /// <summary>
        /// Resistance of the device in ohms.
        /// </summary>
        public double Resistance { get; set; }

        public ResistorElement(double resistance, string name = null) : base(name)
        {
            this.Resistance = resistance;
        }
    }
}