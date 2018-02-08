using NextGenSpice.Core.Elements.Parameters;

namespace NextGenSpice.Core.Elements
{
    /// <summary>
    ///     Class that represents the diode device.
    /// </summary>
    public class DiodeElement : TwoNodeCircuitElement
    {
        public DiodeElement(DiodeModelParams parameters, string name = null, double voltageHint = 0) : base(name)
        {
            Parameters = parameters;
            VoltageHint = voltageHint;
        }

        /// <summary>
        ///     Diode model parameters.
        /// </summary>
        public DiodeModelParams Parameters { get; }


        /// <summary>
        ///     Hint for initial voltage across the diode in volts for faster first dc-bias calculation.
        /// </summary>
        public double VoltageHint { get; }
    }
}