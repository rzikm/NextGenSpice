using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalResistorModel : TwoNodeLargeSignalModel<ResistorElement>, ILinearLargeSignalDeviceModel
    {
        public double Resistance => Parent.Resistance;

        public LargeSignalResistorModel(ResistorElement parent) : base(parent)
        {
        }

        public void ApplyLinearModelValues(IEquationEditor equationSystem, SimulationContext context)
        {
            equationSystem.AddMatrixEntry(Kathode, Anode, -1 / Resistance);
            equationSystem.AddMatrixEntry(Anode, Kathode, -1 / Resistance);
            equationSystem.AddMatrixEntry(Anode, Anode, 1 / Resistance);
            equationSystem.AddMatrixEntry(Kathode, Kathode, 1 / Resistance);
        }
    }

    public abstract class TwoNodeLargeSignalModel<TDefinitionElement> : LargeSignalModelBase<TDefinitionElement> where TDefinitionElement : TwoNodeCircuitElement
    {
        public int Anode => Parent.ConnectedNodes[0];

        public int Kathode => Parent.ConnectedNodes[1];

        protected TwoNodeLargeSignalModel(TDefinitionElement parent) : base(parent)
        {
        }
    }

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