using System;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.LargeSignal.Behaviors
{
    internal class ConstantSourceBehavior : InputSourceBehavior<ConstantBehaviorParams>
    {
        public override double GetValue(ISimulationContext context)
        {
            return param.Value;
        }

        public override bool IsTimeDependent => false;
        public override bool HasDependency => false;

        public ConstantSourceBehavior(ConstantBehaviorParams param) : base(param)
        {
        }
    }
}