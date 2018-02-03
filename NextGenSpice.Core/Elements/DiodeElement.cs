namespace NextGenSpice.Core.Elements
{
    /// <summary>
    ///     Class that represents the diode device.
    /// </summary>
    public class DiodeElement : TwoNodeCircuitElement
    {
        public DiodeElement(DiodeModelParams parameters, string name = null) : base(name)
        {
            Parameters = parameters;
        }

        /// <summary>
        ///     Diode model parameters.
        /// </summary>
        public DiodeModelParams Parameters { get; }
    }
}