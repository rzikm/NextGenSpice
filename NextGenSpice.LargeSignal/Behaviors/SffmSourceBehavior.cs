using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for single frequency FM behavior of <see cref="LargeSignalVoltageSource" /> and
    ///     <see cref="LargeSignalCurrentSource" />.
    /// </summary>
    internal class SffmSourceBehavior : InputSourceBehavior<SffmBehaviorParams>
    {
        public SffmSourceBehavior(SffmBehaviorParams parameters) : base(parameters)
        {
        }

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.TimePoint;

        /// <summary>Gets input source value for given timepoint.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override double GetValue(ISimulationContext context)
        {
            var c = 2 * Math.PI * context.TimePoint;
            var phaseCarrier = c * Parameters.FrequencyCarrier;
            var phaseSignal = c * Parameters.FrequencySignal;

            return Parameters.DcOffset +
                   Parameters.Amplitude * Math.Sin(phaseCarrier + Parameters.ModulationIndex * Math.Sin(phaseSignal));
        }
    }
}