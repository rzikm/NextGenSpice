using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.LargeSignal.Behaviors;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalVoltageSourceModel : TwoNodeLargeSignalModel<VoltageSourceElement>
    {
        private int branchVariable = -1;

        public LargeSignalVoltageSourceModel(VoltageSourceElement parent, IInputSourceBehavior behavior) : base(parent)
        {
            Behavior = behavior;
        }

        private IInputSourceBehavior Behavior { get; }


        public override bool IsNonlinear => Behavior.HasDependency;
        public override bool IsTimeDependent => Behavior.IsTimeDependent;
        
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Current = context.GetSolutionForVariable(branchVariable);
        }

        public override void RegisterAdditionalVariables(IEquationSystemBuilder builder, ISimulationContext context)
        {
            base.RegisterAdditionalVariables(builder, context);
            branchVariable = builder.AddVariable();
        }

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            Voltage = Behavior.GetValue(context);
            equations.AddVoltage(Anode, Kathode, branchVariable, Voltage);
        }
    }
}