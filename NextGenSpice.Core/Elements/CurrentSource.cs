using System;
using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public class CurrentSource : SimpleTwoNodeElement
    {
        public double Current { get; set; }
        public CurrentSource(double current)
        {
            Current = current;
        }

        public override ILargeSignalDeviceModel GetLargeSignalModel()
        {
            return this;
        }

        public override ILargeSignalDeviceModel GetSmallSignalModel()
        {
            throw new NotImplementedException();
        }

        public override void ApplyLinearModelValues(IEquationEditor equationSystem, SimulationContext context)
        {
            equationSystem.AddRightHandSideEntry(Anode.Id, Current);
            equationSystem.AddRightHandSideEntry(Kathode.Id, -Current);
        }
    }
}