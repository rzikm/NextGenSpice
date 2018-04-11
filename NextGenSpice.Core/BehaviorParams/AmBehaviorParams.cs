namespace NextGenSpice.Core.BehaviorParams
{
    /// <summary>Specifies behavior parameters for input source with amplitude modulation.</summary>
    public class AmBehaviorParams : SourceBehaviorParams
    {
        /// <summary>
        ///     Peak amplitude of the unmodulated signal in volts or ampers. The real maximum amplitude is given by
        ///     SignalAmplitude * (1 + ModulationIndex).
        /// </summary>
        public double SignalAmplitude { get; set; }

        /// <summary>Frequency of the carrier wave in hertz.</summary>
        public double FrequencyCarrier { get; set; }

        /// <summary>Frequency with which the carrier is modulated in hertz.</summary>
        public double FrequencyModulation { get; set; }

        /// <summary>Indicates by how much the value varies around its unmodulated level.</summary>
        public double ModulationIndex { get; set; } = 1;

        /// <summary>Phase offset of the modulation at the start of the simulation in radians.</summary>
        public double PhaseOffset { get; set; }

        /// <summary>Time offset of the signal modulation.</summary>
        public double Delay { get; set; }
    }
}