using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpiceTest
{
    public class SwitchModel : TwoNodeLargeSignalModel<SwitchElement>, ITimeDependentLargeSignalDeviceModel
    {
        public bool IsOn { get; set; } = true;

        public SwitchModel(SwitchElement parent) : base(parent)
        {
        }

        public void AdvanceTimeDependentModel(SimulationContext context)
        {
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, SimulationContext context)
        {
            if (IsOn)
                equation.BindEquivalent(Anode, Kathode);
            else
                equation.AddConductance(Anode, Kathode, 1e-12);
        }

        public void RollbackTimeDependentModel()
        {
        }
    }
}