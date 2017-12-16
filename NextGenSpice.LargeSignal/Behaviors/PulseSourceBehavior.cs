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
            if (phase < param.Delay) return param.Value1;
            phase -= param.Delay;
            if (phase < param.TimeRise)
                return MathHelper.LinearInterpolation(param.Value1, param.Value2, phase / param.TimeRise);
            phase -= param.TimeRise;
            if (phase < param.Duration)
                return param.Value2;
            phase -= param.Duration;
            if (phase < param.TimeFall)
                return MathHelper.LinearInterpolation(param.Value2, param.Value1, phase / param.TimeFall);
            return param.Value1;
        }

        public override bool IsTimeDependent => true;
        public override bool HasDependency => false;
    }
}