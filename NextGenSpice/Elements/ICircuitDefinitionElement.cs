using NextGenSpice.Circuit;

namespace NextGenSpice.Elements
{
    public interface ICircuitDefinitionElement
    {
        NodeConnectionSet ConnectedNodes { get; }

        void Accept<T>(ICircuitVisitor<T> visitor);

        ICircuitModelElement GetLargeSignalModel();
        ICircuitModelElement GetSmallSignalModel();
    }
}