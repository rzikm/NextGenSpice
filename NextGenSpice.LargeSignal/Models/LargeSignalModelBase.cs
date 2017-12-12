using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public abstract class LargeSignalModelBase<TDefinitionElement> : ILargeSignalDeviceModel
        where TDefinitionElement : ICircuitDefinitionElement
    {
        protected LargeSignalModelBase(TDefinitionElement parent)
        {
            Parent = parent;
        }

        public string Tag { get; set; }

        protected TDefinitionElement Parent { get; }

        public string Name => Parent.Name;

        public virtual void RegisterAdditionalVariables(IEquationSystemBuilder builder, ISimulationContext context)
        {
        }

        public abstract void ApplyModelValues(IEquationEditor equations, ISimulationContext context);

        public virtual void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            ApplyModelValues(equations, context);
        }

        public virtual void OnDcBiasEstablished(ISimulationContext context)
        {
        }

        public abstract bool IsNonlinear { get; }
        public abstract bool IsTimeDependent { get; }
    }
}