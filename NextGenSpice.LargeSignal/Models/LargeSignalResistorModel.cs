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

        public void ApplyLinearModelValues(IEquationEditor equation, SimulationContext context)
        {
            equation.AddConductance(Anode, Kathode, 1 / Resistance);
        }
    }
}