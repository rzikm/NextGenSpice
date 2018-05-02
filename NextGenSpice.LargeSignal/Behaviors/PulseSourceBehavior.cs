using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Devices;
using NextGenSpice.Numerics;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for pulsing behavior of <see cref="LargeSignalVoltageSource" /> and
    ///     <see cref="LargeSignalCurrentSource" />.
    /// </summary>
    internal class PulseSourceBehavior : InputSourceBehavior<PulseBehavior>
    {
        public PulseSourceBehavior(PulseBehavior parameters) : base(parameters)
        {
        }

        /// <summary>Gets input source value for given timepoint.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override double GetValue(ISimulationContext context)
        {
            var phase = context.TimePoint;
            if (Parameters.Period > 0)
                phase = context.TimePoint % Parameters.Period;
            if (phase < Parameters.Delay) return Parameters.InitialLevel;
            phase -= Parameters.Delay;
            if (phase < Parameters.TimeRise)
                return MathHelper.LinearInterpolation(Parameters.InitialLevel, Parameters.PulseLevel,
                    phase / Parameters.TimeRise);
            phase -= Parameters.TimeRise;
            if (phase < Parameters.PulseWidth)
                return Parameters.PulseLevel;
            phase -= Parameters.PulseWidth;
            if (phase < Parameters.TimeFall)
                return MathHelper.LinearInterpolation(Parameters.PulseLevel, Parameters.InitialLevel,
                    phase / Parameters.TimeFall);
            return Parameters.InitialLevel;
        }
    }
}