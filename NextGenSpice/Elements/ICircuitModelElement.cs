using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public interface ICircuitModelElement
    {
        void Initialize();
    }

    public interface ILinearCircuitModelElement : ICircuitModelElement
    {
        void ApplyLinearModelValues(IEquationSystemBuilder equationSystem, SimulationContext context);
    }
}