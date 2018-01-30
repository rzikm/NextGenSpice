using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Elements
{
    /// <summary>
    /// Base class for elements that have exactly two terminals.
    /// </summary>
    public abstract class TwoNodeCircuitElement : CircuitDefinitionElement
    {
        /// <summary>
        /// Positive terminal of the device.
        /// </summary>
        public int Anode => ConnectedNodes[0];

        /// <summary>
        /// Negative terminal of the device.
        /// </summary>
        public int Kathode => ConnectedNodes[1];

        protected TwoNodeCircuitElement(string name) : base(2, name)
        {
        }
    }
}