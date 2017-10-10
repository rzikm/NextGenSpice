using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public interface ITimeDependentCircuitModelElement : ICircuitModelElement
    {
        void UpdateTimeDependentModel(SimulationContext context);

        void ApplyTimeDependentModelValues(IEquationSystem equationSystem, SimulationContext context);
    }
}