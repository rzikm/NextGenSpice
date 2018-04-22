using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Numerics;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for behavior with exponential edges for <see cref="LargeSignalVoltageSource" /> and
    ///     <see cref="LargeSignalCurrentSource" />.
    /// </summary>
    internal class ExponentialSourceBehavior : InputSourceBehavior<ExponentialBehaviorParams>
    {
        public ExponentialSourceBehavior(ExponentialBehaviorParams parameters) : base(parameters)
        {
        }

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.TimePoint;

        /// <summary>Gets input source value for given timepoint.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override double GetValue(ISimulationContext context)
        {
            var time = context.TimePoint;

            if (time <= Parameters.RiseDelay)
                return Parameters.InitialLevel;
            if (time <= Parameters.FallDelay)
                return MathHelper.LinearInterpolation(
                    Parameters.InitialLevel,
                    Parameters.PulseLevel,
                    1 - Math.Exp(-(time - Parameters.RiseDelay) / Parameters.RiseTau));
            return MathHelper.LinearInterpolation(
                Parameters.InitialLevel,
                Parameters.PulseLevel,
                (1 - Math.Exp(-(Parameters.FallDelay - Parameters.RiseDelay) / Parameters.RiseTau)) *
                Math.Exp(-(time - Parameters.FallDelay) / Parameters.FallTau));
        }
    }
}