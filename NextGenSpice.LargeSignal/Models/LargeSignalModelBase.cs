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

        public virtual void RegisterAdditionalVariables(IEquationSystemBuilder builder)
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
    }
}