namespace NextGenSpice.Core.Elements
{
    /// <summary>Base class for elements that have exactly two terminals.</summary>
    public abstract class TwoNodeCircuitElement : CircuitDefinitionElement
    {
        protected TwoNodeCircuitElement(string name) : base(2, name)
        {
        }

        /// <summary>Positive terminal of the device.</summary>
        public int Anode => ConnectedNodes[0];

        /// <summary>Negative terminal of the device.</summary>
        public int Cathode => ConnectedNodes[1];
    }
}