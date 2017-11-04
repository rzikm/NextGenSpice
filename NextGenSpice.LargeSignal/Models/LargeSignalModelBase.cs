using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public abstract class LargeSignalModelBase<TDefinitionElement> : ILargeSignalDeviceModel where TDefinitionElement : ICircuitDefinitionElement
    {
        protected LargeSignalModelBase(TDefinitionElement parent)
        {
            Parent = parent;
        }

        protected TDefinitionElement Parent { get; }

        public virtual void Initialize(IEquationSystemBuilder builder)
        {
        }
    }
}