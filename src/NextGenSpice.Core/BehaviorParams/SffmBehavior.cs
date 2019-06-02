using System;

namespace NextGenSpice.Core.BehaviorParams
{
    /// <summary>Specifies behavior parameters for input source with frequency modulation.</summary>
    public class SffmBehavior : InputSourceBehavior
    {
        /// <summary>Offset of the signal in volts or ampers.</summary>
        public double DcOffset { get; set; }

        /// <summary>Peak amplitude of the signal in volts or ampers.</summary>
        public double Amplitude { get; set; }

        /// <summary>Frequency of the carrier wave in hertz.</summary>
        public double FrequencyCarrier { get; set; }

        /// <summary>Frequency of the signal in hertz.</summary>
        public double FrequencySignal { get; set; }

        /// <summary>Indicates by how much the value varies around its unmodulated level.</summary>
        public double ModulationIndex { get; set; }

        /// <summary>Gets input source value for given timepoint.</summary>
        /// <param name="timepoint">The time value for which to calculate the value.</param>
        /// <returns></returns>
        public override double GetValue(double timepoint)
        {
            var c = 2 * Math.PI * timepoint;
            var phaseCarrier = c * FrequencyCarrier;
            var phaseSignal = c * FrequencySignal;

            return DcOffset +
                   Amplitude * Math.Sin(phaseCarrier + ModulationIndex * Math.Sin(phaseSignal));

        }
    }
}