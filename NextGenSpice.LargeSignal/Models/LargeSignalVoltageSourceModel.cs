using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalVoltageSourceModel : TwoNodeLargeSignalModel<VoltageSourceElement>, ILinearLargeSignalDeviceModel
    {
        public double Voltage => Parent.Voltage;

        private int additionalVariable = -1;
        public LargeSignalVoltageSourceModel(VoltageSourceElement parent) : base(parent)
        {
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            base.Initialize(builder);
            additionalVariable = builder.AddVariable();
        }

        public void ApplyLinearModelValues(IEquationEditor equation, SimulationContext context)
        {
            equation.AddVoltage(Anode, Kathode, additionalVariable, Voltage);
        }
    }
}