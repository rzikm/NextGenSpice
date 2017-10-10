using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public interface INonlinearCircuitModelElement : ICircuitModelElement
    {
        void UpdateNonlinearModel(SimulationContext context);

        void ApplyNonlinearModelValues(IEquationSystem equationSystem, SimulationContext context);
    }
}