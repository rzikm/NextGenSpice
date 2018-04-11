namespace NextGenSpice.Core.BehaviorParams
{
    /// <summary>Specifies behavior parameters for input source with frequency modulation.</summary>
    public class SffmBehaviorParams : SourceBehaviorParams
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
    }
}