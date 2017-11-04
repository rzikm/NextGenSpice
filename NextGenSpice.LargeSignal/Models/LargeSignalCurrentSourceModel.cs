using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalCurrentSourceModel : TwoNodeLargeSignalModel<CurrentSourceElement>, ILinearLargeSignalDeviceModel
    {
        public double Current => Parent.Current;
        public LargeSignalCurrentSourceModel(CurrentSourceElement parent) : base(parent)
        {
        }
        
        public void ApplyLinearModelValues(IEquationEditor equation, SimulationContext context)
        {
            equation.AddCurrent(Anode, Kathode, Current);
        }
    }
}