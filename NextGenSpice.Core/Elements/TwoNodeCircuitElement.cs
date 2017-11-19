using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Elements
{
    public abstract class TwoNodeCircuitElement : ICircuitDefinitionElement
    {
        public int Anode => ConnectedNodes[0];

        public int Kathode => ConnectedNodes[1];

        protected TwoNodeCircuitElement()
        {
            ConnectedNodes = new NodeConnectionSet(2);
        }

        public NodeConnectionSet ConnectedNodes { get; private set; }
        public virtual ICircuitDefinitionElement Clone()
        {
            var clone = (TwoNodeCircuitElement)MemberwiseClone();
            clone.ConnectedNodes = ConnectedNodes.Clone();

            return clone;
        }
    }
}