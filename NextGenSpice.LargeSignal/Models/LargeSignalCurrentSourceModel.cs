using System;
using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public class LargeSignalCurrentSourceModel : TwoNodeCircuitElement, ILinearLargeSignalDeviceModel
    {
        public double Current { get; set; }
        public LargeSignalCurrentSourceModel(double current)
        {
            Current = current;
        }
        
        public void ApplyLinearModelValues(IEquationEditor equationSystem, SimulationContext context)
        {
            equationSystem.AddRightHandSideEntry(Anode.Id, Current);
            equationSystem.AddRightHandSideEntry(Kathode.Id, -Current);
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void ApplyLinearModelValues(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            throw new NotImplementedException();
        }

        public override ILargeSignalDeviceModel GetLargeSignalModel()
        {
            throw new NotImplementedException();
        }

        public override ILargeSignalDeviceModel GetSmallSignalModel()
        {
            throw new NotImplementedException();
        }
    }
}