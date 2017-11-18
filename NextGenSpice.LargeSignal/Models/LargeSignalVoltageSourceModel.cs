using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalVoltageSourceModel : TwoNodeLargeSignalModel<VoltageSourceElement>, ILinearLargeSignalDeviceModel
    {
        public double Voltage => Parent.Voltage;

        public double Current { get; private set; }

        private int branchVariable = -1;
        public LargeSignalVoltageSourceModel(VoltageSourceElement parent) : base(parent)
        {
        }

        public override void PostProcess(SimulationContext context)
        {
            base.PostProcess(context);
            Current = context.EquationSolution[branchVariable];
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            base.Initialize(builder);
            branchVariable = builder.AddVariable();
        }

        public void ApplyLinearModelValues(IEquationEditor equation, SimulationContext context)
        {
            equation.AddVoltage(Anode, Kathode, branchVariable, Voltage);
        }
    }

    public class PulsingLargeSignalVoltageSourceModel : TwoNodeLargeSignalModel<VoltageSourceElement>, ITimeDependentLargeSignalDeviceModel
    {
        public double Voltage => Parent.Voltage;

        public double Current { get; private set; }

        private int branchVariable = -1;
        public PulsingLargeSignalVoltageSourceModel(VoltageSourceElement parent) : base(parent)
        {
        }

        public override void PostProcess(SimulationContext context)
        {
            base.PostProcess(context);
            Current = context.EquationSolution[branchVariable];
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            base.Initialize(builder);
            branchVariable = builder.AddVariable();
        }

        public void ApplyLinearModelValues(IEquationEditor equation, SimulationContext context)
        {
            equation.AddVoltage(Anode, Kathode, branchVariable, Voltage);
        }

        public void AdvanceTimeDependentModel(SimulationContext context)
        {
            
        }

        public void RollbackTimeDependentModel()
        {
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, SimulationContext context)
        {
            equation.AddVoltage(Anode, Kathode, branchVariable, Voltage);
        }
    }
}