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
        
        public void ApplyLinearModelValues(IEquationEditor equationSystem, SimulationContext context)
        {
            equationSystem.AddRightHandSideEntry(Anode, Current);
            equationSystem.AddRightHandSideEntry(Kathode, -Current);
        }

        public void Initialize()
        {
        }

        public void ApplyLinearModelValues(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            ApplyLinearModelValues((IEquationEditor) equationSystem, context);
        }
    }
}