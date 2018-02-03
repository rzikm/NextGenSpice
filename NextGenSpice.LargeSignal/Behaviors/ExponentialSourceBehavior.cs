using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Models;
using Numerics;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for behavior with exponential edges for <see cref="LargeSignalVoltageSourceModel" /> and
    ///     <see cref="LargeSignalCurrentSourceModel" />.
    /// </summary>
    internal class ExponentialSourceBehavior : InputSourceBehavior<ExponentialBehaviorParams>
    {
        public ExponentialSourceBehavior(ExponentialBehaviorParams parameters) : base(parameters)
        {
        }

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
            var time = context.Time;

            if (time <= Parameters.RiseDelay)
                return Parameters.InitialLevel;
            if (time <= Parameters.FallDelay)
                return MathHelper.LinearInterpolation(
                    Parameters.InitialLevel,
                    Parameters.PulseLevel,
                    1 - Math.Exp(-(time - Parameters.RiseDelay) / Parameters.TauRise));
            return MathHelper.LinearInterpolation(
                Parameters.InitialLevel,
                Parameters.PulseLevel,
                (1 - Math.Exp(-(Parameters.FallDelay - Parameters.RiseDelay) / Parameters.TauRise)) *
                Math.Exp(-(time - Parameters.FallDelay) / Parameters.TauFall));
        }
    }
}