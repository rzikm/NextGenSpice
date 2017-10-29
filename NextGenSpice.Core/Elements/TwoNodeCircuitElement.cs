using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Elements
{
    public abstract class TwoNodeCircuitElement : ICircuitDefinitionElement
    {
        public int Anode
        {
            get => ConnectedNodes[0];
            set => ConnectedNodes[0] = value;
        }

        public int Kathode
        {
            get => ConnectedNodes[1];
            set => ConnectedNodes[1] = value;
        }

        public TwoNodeCircuitElement()
        {
            ConnectedNodes = new NodeConnectionSet(2);
        }

        public NodeConnectionSet ConnectedNodes { get; protected internal set; }
    }
}