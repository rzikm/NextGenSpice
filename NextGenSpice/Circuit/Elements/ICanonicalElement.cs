namespace NextGenSpice.Circuit
{
    public interface ICanonicalElement
    {
        void ApplyToEquationsPermanent(IEquationEditor equationSystem, SimulationContext context);
    }
}