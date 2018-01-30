namespace NextGenSpice.Core.Elements
{
    /// <summary>
    /// Class that represents the diode device.
    /// </summary>
    public class DiodeElement : TwoNodeCircuitElement
    {
        /// <summary>
        /// Diode model parameters.
        /// </summary>
        public DiodeModelParams Param { get; }
        
        public DiodeElement(DiodeModelParams param, string name = null) : base(name)
        {
            Param = param;
        }
    }
}