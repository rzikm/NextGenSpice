namespace NextGenSpice.Circuit
{
    public interface ICircuitDefinitionElement
    {
        NodeConnectionSet ConnectedNodes { get; }

        string Name { get; }

        void Accept<T>(ICircuitVisitor<T> visitor);

        ICircuitModelElement GetDcOperatingPointModel();
        ICircuitModelElement GetTransientModel();
    }

    public interface ICircuitModelElement
    {
        void ApplyToEquationsPermanent(IEquationSystemBuilder equationSystem, SimulationContext context);
    }
}