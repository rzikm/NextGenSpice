using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalVoltageSourceModel : TwoNodeLargeSignalModel<VoltageSourceElement>, ILinearLargeSignalDeviceModel
    {
        public double Voltage => Parent.Voltage;
        public LargeSignalVoltageSourceModel(VoltageSourceElement parent) : base(parent)
        {
        }

        public void Initialize()
        {
            
        }

        public void ApplyLinearModelValues(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            var i = equationSystem.AddVariable();
            equationSystem.AddMatrixEntry(i, Anode, 1);
            equationSystem.AddMatrixEntry(i, Kathode, -1);

            equationSystem.AddMatrixEntry(Anode, i, 1);
            equationSystem.AddMatrixEntry(Kathode, i, -1);

            equationSystem.AddRightHandSideEntry(i, Voltage);
        }
    }
}