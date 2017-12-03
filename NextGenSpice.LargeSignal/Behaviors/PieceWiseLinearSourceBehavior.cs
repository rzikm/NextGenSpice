using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.LargeSignal.Behaviors
{
    internal class PieceWiseLinearSourceBehavior : InputSourceBehavior<PieceWiseLinearBehaviorParams>
    {
        public PieceWiseLinearSourceBehavior(PieceWiseLinearBehaviorParams param) : base(param)
        {
        }

        public override double GetValue(ISimulationContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsTimeDependent => true;
        public override bool HasDependency => false;
    }
}