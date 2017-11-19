using NextGenSpice.Core.Elements;

namespace NextGenSpice.LargeSignal.Models
{
    public abstract class TwoNodeLargeSignalModel<TDefinitionElement> : LargeSignalModelBase<TDefinitionElement>
        where TDefinitionElement : TwoNodeCircuitElement
    {
        protected TwoNodeLargeSignalModel(TDefinitionElement parent) : base(parent)
        {
        }

        public int Anode => Parent.ConnectedNodes[0];

        public int Kathode => Parent.ConnectedNodes[1];
    }
}