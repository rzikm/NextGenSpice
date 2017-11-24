using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Elements
{
    public abstract class CircuitDefinitionElement : ICircuitDefinitionElement
    {
        protected CircuitDefinitionElement(int terminalCount, string name)
        {
            ConnectedNodes = new NodeConnectionSet(terminalCount);
            Name = name;
        }

        public NodeConnectionSet ConnectedNodes { get; protected set; }
        public string Name { get; internal set; }
        public virtual ICircuitDefinitionElement Clone()
        {
            var circuitDefinitionElement = (CircuitDefinitionElement) MemberwiseClone();
            circuitDefinitionElement.ConnectedNodes = ConnectedNodes.Clone();
            return circuitDefinitionElement;
        }
    }
}