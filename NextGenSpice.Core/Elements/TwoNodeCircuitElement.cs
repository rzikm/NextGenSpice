using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Elements
{
    public abstract class TwoNodeCircuitElement : CircuitDefinitionElement
    {
        public int Anode => ConnectedNodes[0];

        public int Kathode => ConnectedNodes[1];

        protected TwoNodeCircuitElement(string name) : base(2, name)
        {
        }
    }
}