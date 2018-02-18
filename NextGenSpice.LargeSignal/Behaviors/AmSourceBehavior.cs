using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for single frequency AM behavior of <see cref="LargeSignalVoltageSourceModel" /> and
    ///     <see cref="LargeSignalCurrentSourceModel" />.
    /// </summary>
    internal class AmSourceBehavior : InputSourceBehavior<AmBehaviorParams>
    {
        public AmSourceBehavior(AmBehaviorParams parameters) : base(parameters)
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
            var time = context.TimePoint - Parameters.Delay;
            var c = 2 * Math.PI * time;

            var phaseCarrier = c * Parameters.FrequencyCarrier;
            var phaseModulation = c * Parameters.FrequencyModulation;

            var m = Parameters.ModulationIndex;

            return Parameters.SignalAmplitude * Math.Sin(phaseCarrier) * (1 + m * Math.Cos(phaseModulation));
        }
    }
}