namespace NextGenSpice.Circuit
{
    public interface ICircuitElement
    {
        NodeConnectionSet ConnectedNodes { get; }

        void Accept<T>(ICircuitVisitor<T> visitor);

        void ApplyToEquations(ICircuitEquationSystem equationSystem);
    }
}