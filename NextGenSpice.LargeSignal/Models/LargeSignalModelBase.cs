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

        public virtual void Initialize(IEquationSystemBuilder builder)
        {
        }

        public virtual void PostProcess(ISimulationContext context)
        {
        }
    }
}