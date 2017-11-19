using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalCurrentSourceModel : TwoNodeLargeSignalModel<CurrentSourceElement>,
        ILinearLargeSignalDeviceModel
    {
        public LargeSignalCurrentSourceModel(CurrentSourceElement parent) : base(parent)
        {
        }

        public double Current => Parent.Current;

        public void ApplyLinearModelValues(IEquationEditor equation, ISimulationContext context)
        {
            equation.AddCurrent(Anode, Kathode, Current);
        }
    }
}