using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalCurrentSourceModel : TwoNodeLargeSignalModel<CurrentSourceElement>
    {
        public LargeSignalCurrentSourceModel(CurrentSourceElement parent) : base(parent)
        {
        }

        public double Current => Parent.Current;
        
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            equations.AddCurrent(Anode, Kathode, Current);
        }
    }
}