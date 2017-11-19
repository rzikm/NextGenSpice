using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalVoltageSourceModel : TwoNodeLargeSignalModel<VoltageSourceElement>,
        ILinearLargeSignalDeviceModel
    {
        private int branchVariable = -1;

        public LargeSignalVoltageSourceModel(VoltageSourceElement parent) : base(parent)
        {
        }

        public double Voltage => Parent.Voltage;

        public double Current { get; private set; }

        public override void PostProcess(ISimulationContext context)
        {
            base.PostProcess(context);
            Current = context.GetSolutionForVariable(branchVariable);
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            base.Initialize(builder);
            branchVariable = builder.AddVariable();
        }

        public void ApplyLinearModelValues(IEquationEditor equation, ISimulationContext context)
        {
            equation.AddVoltage(Anode, Kathode, branchVariable, Voltage);
        }
    }

    public class PulsingLargeSignalVoltageSourceModel : TwoNodeLargeSignalModel<VoltageSourceElement>,
        ITimeDependentLargeSignalDeviceModel
    {
        private int branchVariable = -1;

        public PulsingLargeSignalVoltageSourceModel(VoltageSourceElement parent) : base(parent)
        {
        }

        public double Voltage => Parent.Voltage;

        public double Current { get; private set; }

        public override void PostProcess(ISimulationContext context)
        {
            base.PostProcess(context);
            Current = context.GetSolutionForVariable(branchVariable);
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            base.Initialize(builder);
            branchVariable = builder.AddVariable();
        }

        public void UpdateTimeDependentModel(ISimulationContext context)
        {
        }

        public void RollbackTimeDependentModel()
        {
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, ISimulationContext context)
        {
            equation.AddVoltage(Anode, Kathode, branchVariable, Voltage);
        }

        public void ApplyLinearModelValues(IEquationEditor equation, ISimulationContext context)
        {
            equation.AddVoltage(Anode, Kathode, branchVariable, Voltage);
        }
    }
}