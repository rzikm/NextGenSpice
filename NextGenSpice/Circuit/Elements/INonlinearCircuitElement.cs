namespace NextGenSpice.Circuit
{
    public interface INonlinearCircuitElement : ICircuitElement
    {
        void UpdateLinearizedModel(SimulationContext context);
    }
}