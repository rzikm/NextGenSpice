using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Models;
using Numerics;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for pulsing behavior of <see cref="LargeSignalVoltageSourceModel" /> and
    ///     <see cref="LargeSignalCurrentSourceModel" />.
    /// </summary>
    internal class PulseSourceBehavior : InputSourceBehavior<PulseBehaviorParams>
    {
        public PulseSourceBehavior(PulseBehaviorParams parameters) : base(parameters)
        {
        }

        /// <summary>
        ///     Specifies how often the model should be updated.
        /// </summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.TimePoint;

        /// <summary>
        ///     If true, the behavior is not constant over time and the value is refreshed every timestep.
        /// </summary>
        public override bool IsTimeDependent => true;

        /// <summary>
        ///     If true, the behavior depends on another element and the source value is updated in every iteration of
        ///     Newton-Raphson loop.
        /// </summary>
        public override bool HasDependency => false;

        /// <summary>
        ///     Gets input source value for given timepoint.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override double GetValue(ISimulationContext context)
        {
            // TODO: make more efficient (precompute boundaries)
            var phase = context.Time;
            if (Parameters.Period > 0)
                phase = context.Time % Parameters.Period;
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