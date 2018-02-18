using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for sinusoidal behavior of <see cref="LargeSignalVoltageSourceModel" /> and
    ///     <see cref="LargeSignalCurrentSourceModel" />.
    /// </summary>
    internal class SinusioidalSourceBehavior : InputSourceBehavior<SinusoidalBehaviorParams>
    {
        public SinusioidalSourceBehavior(SinusoidalBehaviorParams parameters) : base(parameters)
        {
        }

        /// <summary>
        ///     Specifies how often the model should be updated.
        /// </summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.TimePoint;

        /// <summary>
        ///     Gets input source value for given timepoint.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override double GetValue(ISimulationContext context)
        {
            var phase = Parameters.PhaseOffset;
            var amplitude = Parameters.Amplitude;
            var elapsedTime = context.TimePoint - Parameters.Delay;

            if (elapsedTime > 0) // source is constant during Delay time
            {
                phase += elapsedTime * Parameters.Frequency * 2 * Math.PI;
                amplitude *= Math.Exp(-elapsedTime * Parameters.DampingFactor);
            }

            return Parameters.DcOffset + Math.Sin(phase) * amplitude;
        }
    }
}