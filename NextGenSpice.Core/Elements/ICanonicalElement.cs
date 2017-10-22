using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public interface ICanonicalElement
    {
        void ApplyLinearModelValues(IEquationEditor equationSystem, SimulationContext context);
    }
}