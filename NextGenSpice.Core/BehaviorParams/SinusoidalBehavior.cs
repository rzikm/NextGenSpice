namespace NextGenSpice.Core.BehaviorParams
{
    /// <summary>Specifies behavior parameters for sinusoidal input source.</summary>
    public class SinusoidalBehavior : InputSourceBehavior
    {
        /// <summary>Offset of the signal in volts or ampers.</summary>
        public double DcOffset { get; set; }

        /// <summary>Amplitude of the waveform in volts or ampers.</summary>
        public double Amplitude { get; set; }

        /// <summary>Frequency of the signal in hertz.</summary>
        public double Frequency { get; set; }

        /// <summary>Time delay before start of sinusoidal wave. The source is constant during the delay.</summary>
        public double Delay { get; set; }

        /// <summary>Time constant of exponential decay in seconds.</summary>
        public double DampingFactor { get; set; }

        /// <summary>Phase offset of the waveform in radians.</summary>
        public double PhaseOffset { get; set; }
    }
}