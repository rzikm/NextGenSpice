namespace NextGenSpice.Circuit
{
    public interface INonlinearCircuitModelElement : ICircuitModelElement
    {
        void UpdateLinearizedModel(SimulationContext context);

        void ApplyToEquationsDynamic(IEquationSystem equationSystem, SimulationContext context);
    }
}