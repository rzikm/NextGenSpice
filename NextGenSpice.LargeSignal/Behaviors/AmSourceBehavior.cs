using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Devices;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for single frequency AM behavior of <see cref="LargeSignalVoltageSource" /> and
    ///     <see cref="LargeSignalCurrentSource" />.
    /// </summary>
    internal class AmSourceBehavior : InputSourceBehavior<AmBehavior>
    {
        public AmSourceBehavior(AmBehavior parameters) : base(parameters)
        {
        }

        /// <summary>Gets input source value for given timepoint.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override double GetValue(ISimulationContext context)
        {
            var time = context.TimePoint - Parameters.Delay;
            var c = 2 * Math.PI * time;

            var phaseCarrier = c * Parameters.FrequencyCarrier;
            var phaseModulation = c * Parameters.FrequencyModulation;


            return time < 0? 0 : Parameters.Amplitude * (Parameters.DcOffset + Math.Sin(phaseModulation) + Parameters.PhaseOffset) *
                   Math.Sin(phaseCarrier + Parameters.PhaseOffset);
        }
    }
}