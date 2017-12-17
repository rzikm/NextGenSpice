using NextGenSpice.Core.Elements;

namespace NextGenSpice.LargeSignal.Models
{
    public abstract class TwoNodeLargeSignalModel<TDefinitionElement> : LargeSignalModelBase<TDefinitionElement>, ITwoTerminalLargeSignalDeviceModel
        where TDefinitionElement : TwoNodeCircuitElement
    {
        protected TwoNodeLargeSignalModel(TDefinitionElement parent) : base(parent)
        {
        }

        public int Anode => Parent.ConnectedNodes[0];

        public int Kathode => Parent.ConnectedNodes[1];

        public double Current { get; protected set; }
        public double Voltage { get; protected set; }
    }
}