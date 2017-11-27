using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalResistorModel : TwoNodeLargeSignalModel<ResistorElement>
    {
        public LargeSignalResistorModel(ResistorElement parent) : base(parent)
        {
        }

        public override bool IsNonlinear => false;
        public override bool IsTimeDependent => false;

        public double Resistance => Parent.Resistance;

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            equations.AddConductance(Anode, Kathode, 1 / Resistance);
        }
    }
}
