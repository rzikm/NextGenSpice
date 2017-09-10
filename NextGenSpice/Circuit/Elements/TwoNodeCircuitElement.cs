namespace NextGenSpice.Circuit
{
    public abstract class TwoNodeCircuitElement : ICircuitDefinitionElement
    {
        public string Name { get; }

        public virtual CircuitNode Anode
        {
            get => ConnectedNodes[0];
            set => ConnectedNodes[0] = value;
        }

        public virtual CircuitNode Kathode
        {
            get => ConnectedNodes[1];
            set => ConnectedNodes[1] = value;
        }

        public TwoNodeCircuitElement()
        {
            ConnectedNodes = new NodeConnectionSet(2);
        }

        public NodeConnectionSet ConnectedNodes { get; protected set; }
        public abstract void Accept<T>(ICircuitVisitor<T> visitor);
        public abstract ICircuitModelElement GetDcOperatingPointModel();
        public abstract ICircuitModelElement GetTransientModel();
    }
}