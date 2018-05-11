using NextGenSpice.Numerics;

namespace NextGenSpice.Core.BehaviorParams
{
    /// <summary>Specifies behavior parameters for input source with alternating pulses.</summary>
    public class PulseBehavior : InputSourceBehavior
    {
        /// <summary>Value in volts or ampers on the beginning of the simulation.</summary>
        public double InitialLevel { get; set; }

        /// <summary>Valule in volts or ampers during the ON state.</summary>
        public double PulseLevel { get; set; }

        /// <summary>Delay in seconds before the rising edge of the pulse.</summary>
        public double Delay { get; set; }

        /// <summary>Duration of the rising edge in seconds.</summary>
        public double TimeRise { get; set; }

        /// <summary>Duration of the ON state of the pulse source in seconds.</summary>
        public double PulseWidth { get; set; }

        /// <summary>Duration of the falling edge in seconds.</summary>
        public double TimeFall { get; set; }

        /// <summary>Period of the waveform in seconds.</summary>
        public double Period { get; set; }

        /// <summary>Gets input source value for given timepoint.</summary>
        /// <param name="timepoint">The time value for which to calculate the value.</param>
        /// <returns></returns>
        public override double GetValue(double timepoint)
        {
            var phase = timepoint;
            if (Period > 0)
                phase = timepoint % Period;
            if (phase < Delay) return InitialLevel;
            phase -= Delay;
            if (phase < TimeRise)
                return MathHelper.LinearInterpolation(InitialLevel, PulseLevel,
                    phase / TimeRise);
            phase -= TimeRise;
            if (phase < PulseWidth)
                return PulseLevel;
            phase -= PulseWidth;
            if (phase < TimeFall)
                return MathHelper.LinearInterpolation(PulseLevel, InitialLevel,
                    phase / TimeFall);
            return InitialLevel;
        }
    }
}