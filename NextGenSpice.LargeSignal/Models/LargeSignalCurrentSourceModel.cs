using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.LargeSignal.Behaviors;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalCurrentSourceModel : TwoNodeLargeSignalModel<CurrentSourceElement>
    {
        public LargeSignalCurrentSourceModel(CurrentSourceElement parent, IInputSourceBehavior behavior) : base(parent)
        {
            Behavior = behavior;
        }

        public IInputSourceBehavior Behavior { get; }

        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Voltage = context.GetSolutionForVariable(Anode) - context.GetSolutionForVariable(Kathode);
        }

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            Current = Behavior.GetValue(context);
            equations.AddCurrent(Anode, Kathode, -Current);
        }

        public override bool IsNonlinear => Behavior.HasDependency;
        public override bool IsTimeDependent => Behavior.IsTimeDependent;
    }
}