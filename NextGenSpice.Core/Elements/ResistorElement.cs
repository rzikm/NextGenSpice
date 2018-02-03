namespace NextGenSpice.Core.Elements
{
    /// <summary>
    ///     Class that represents a resistor device.
    /// </summary>
    public class ResistorElement : TwoNodeCircuitElement
    {
        public ResistorElement(double resistance, string name = null) : base(name)
        {
            Resistance = resistance;
        }

        /// <summary>
        ///     Resistance of the device in ohms.
        /// </summary>
        public double Resistance { get; set; }
    }
}