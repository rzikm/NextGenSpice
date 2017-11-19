using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Elements
{
    public interface ICircuitDefinitionElement
    {
        NodeConnectionSet ConnectedNodes { get; }
        ICircuitDefinitionElement Clone();
    }
}