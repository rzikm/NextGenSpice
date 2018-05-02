using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Devices;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for sinusoidal behavior of <see cref="LargeSignalVoltageSource" /> and
    ///     <see cref="LargeSignalCurrentSource" />.
    /// </summary>
    internal class SinusioidalSourceBehavior : InputSourceBehavior<SinusoidalBehavior>
    {
        public SinusioidalSourceBehavior(SinusoidalBehavior parameters) : base(parameters)
        {
        }

        /// <summary>Gets input source value for given timepoint.</summary>
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