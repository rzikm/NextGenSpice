namespace NextGenSpice.Circuit
{
    public interface ICircuitElement
    {
        NodeConnectionSet ConnectedNodes { get; }

        string Name { get; }

        void Accept<T>(ICircuitVisitor<T> visitor);

        void ApplyToEquationsPermanent(IEquationEditor equationSystem, SimulationContext context);
        void ApplyToEquationsDynamic(IEquationSystem equationSystem, SimulationContext context);
    }
}