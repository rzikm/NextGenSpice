using System;

namespace NextGenSpice.Core.BehaviorParams
{
    /// <summary>Specifies behavior parameters for input source with amplitude modulation.</summary>
    public class AmBehavior : InputSourceBehavior
    {
        /// <summary>
        ///     Peak amplitude of the unmodulated signal in volts or ampers. The real maximum amplitude is given by
        ///     SignalAmplitude * (1 + ModulationIndex).
        /// </summary>
        public double Amplitude { get; set; }

        /// <summary>Offset of the signal in volts or ampers.</summary>
        public double DcOffset { get; set; }

        /// <summary>Frequency with which the carrier is modulated in hertz.</summary>
        public double FrequencyModulation { get; set; }

        /// <summary>Frequency of the carrier wave in hertz.</summary>
        public double FrequencyCarrier { get; set; }

        /// <summary>Time offset of the signal modulation.</summary>
        public double Delay { get; set; }

        /// <summary>Phase offset of the modulation at the start of the simulation in radians.</summary>
        public double PhaseOffset { get; set; }

        /// <summary>Gets input source value for given timepoint.</summary>
        /// <param name="timepoint">The time value for which to calculate the value.</param>
        /// <returns></returns>
        public override double GetValue(double timepoint)
        {
            timepoint -=  Delay;
            var c = 2 * Math.PI * timepoint;

            var phaseCarrier = c * FrequencyCarrier;
            var phaseModulation = c * FrequencyModulation;


            return timepoint < 0 ? 0 : Amplitude * (DcOffset + Math.Sin(phaseModulation) + PhaseOffset) *
                                  Math.Sin(phaseCarrier + PhaseOffset);

        }
    }
}