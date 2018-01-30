using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;
using Numerics;

namespace NextGenSpice.LargeSignal.Behaviors
{
    internal class PulseSourceBehavior : InputSourceBehavior<PulseBehaviorParams>
    {

        public PulseSourceBehavior(PulseBehaviorParams param) : base(param)
        {
        }

        public override double GetValue(ISimulationContext context)
        {
            // TODO: make more efficient
            var phase = context.Time;
            if (param.Period > 0)
             phase = context.Time % param.Period;
            if (phase < param.Delay) return param.InitialLevel;
            phase -= param.Delay;
            if (phase < param.TimeRise)
                return MathHelper.LinearInterpolation(param.InitialLevel, param.PulseLevel, phase / param.TimeRise);
            phase -= param.TimeRise;
            if (phase < param.PulseWidth)
                return param.PulseLevel;
            phase -= param.PulseWidth;
            if (phase < param.TimeFall)
                return MathHelper.LinearInterpolation(param.PulseLevel, param.InitialLevel, phase / param.TimeFall);
            return param.InitialLevel;
        }

        public override bool IsTimeDependent => true;
        public override bool HasDependency => false;
    }
}