using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public abstract class SimpleTwoNodeElement : TwoNodeCircuitElement, ICanonicalElement, ILinearCircuitModelElement
    {
        public virtual void Initialize()
        {
        }

        public void ApplyLinearModelValues(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            ApplyLinearModelValues(equationSystem as IEquationEditor, context);
        }

        public abstract void ApplyLinearModelValues(IEquationEditor equationSystem, SimulationContext context);
    }
}