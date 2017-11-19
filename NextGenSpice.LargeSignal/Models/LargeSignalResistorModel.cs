using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalResistorModel : TwoNodeLargeSignalModel<ResistorElement>, ILinearLargeSignalDeviceModel
    {
        public LargeSignalResistorModel(ResistorElement parent) : base(parent)
        {
        }

        public double Resistance => Parent.Resistance;

        public void ApplyLinearModelValues(IEquationEditor equation, ISimulationContext context)
        {
            equation.AddConductance(Anode, Kathode, 1 / Resistance);
        }
    }
}