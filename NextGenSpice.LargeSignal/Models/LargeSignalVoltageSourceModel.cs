using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalVoltageSourceModel : TwoNodeLargeSignalModel<VoltageSourceElement>
    {
        private int branchVariable = -1;

        public LargeSignalVoltageSourceModel(VoltageSourceElement parent) : base(parent)
        {
        }

        public double Voltage => Parent.Voltage;

        public double Current { get; private set; }

        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Current = context.GetSolutionForVariable(branchVariable);
        }

        public override void RegisterAdditionalVariables(IEquationSystemBuilder builder)
        {
            base.RegisterAdditionalVariables(builder);
            branchVariable = builder.AddVariable();
        }

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            equations.AddVoltage(Anode, Kathode, branchVariable, Voltage);
        }
    }

    public class PulsingLargeSignalVoltageSourceModel : TwoNodeLargeSignalModel<VoltageSourceElement>
    {
        private int branchVariable = -1;

        public PulsingLargeSignalVoltageSourceModel(VoltageSourceElement parent) : base(parent)
        {
        }

        public double Voltage => Parent.Voltage;

        public double Current { get; private set; }

        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Current = context.GetSolutionForVariable(branchVariable);
        }

        public override void RegisterAdditionalVariables(IEquationSystemBuilder builder)
        {
            base.RegisterAdditionalVariables(builder);
            branchVariable = builder.AddVariable();
        }

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            equations.AddVoltage(Anode, Kathode, branchVariable, Voltage);
        }
    }
}