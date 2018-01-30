using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using Numerics;

namespace NextGenSpice.LargeSignal.Behaviors
{
    internal class ExponentialSourceBehavior : InputSourceBehavior<ExponentialBehaviorParams>
    {
        public ExponentialSourceBehavior(ExponentialBehaviorParams param) : base(param)
        {
        }

        public override double GetValue(ISimulationContext context)
        {
            var time = context.Time;

            if (time <= param.RiseDelay)
                return param.InitialLevel;
            if (time <= param.FallDelay)
                return MathHelper.LinearInterpolation(
                    param.InitialLevel,
                    param.PulseLevel,
                    1 - Math.Exp(-(time - param.RiseDelay) / param.TauRise));
            return MathHelper.LinearInterpolation(
                param.InitialLevel,
                param.PulseLevel,
                (1 - Math.Exp(-(param.FallDelay - param.RiseDelay) / param.TauRise)) *
                Math.Exp(-(time - param.FallDelay) / param.TauFall));
        }

        public override bool IsTimeDependent => true;
        public override bool HasDependency => false;
    }
}