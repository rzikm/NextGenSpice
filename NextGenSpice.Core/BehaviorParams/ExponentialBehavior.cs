using System;
using NextGenSpice.Numerics;

namespace NextGenSpice.Core.BehaviorParams
{
    /// <summary>Specifies behavior parameters for input source with exponential rising and falling edges.</summary>
    public class ExponentialBehavior : InputSourceBehavior
    {
        /// <summary>Value on the start of the simulation in volts or ampers.</summary>
        public double InitialLevel { get; set; }

        /// <summary>Value in volts or ampers to which source exponentially converges after RiseDelay.</summary>
        public double PulseLevel { get; set; }

        /// <summary>Time delay in seconds before start of leading edge.</summary>
        public double RiseDelay { get; set; }

        /// <summary>Leading edge rise time constant in seconds.</summary>
        public double RiseTau { get; set; }

        /// <summary>Time delay in seconds before start of trailing edge.</summary>
        public double FallDelay { get; set; }

        /// <summary>Trailing edge fall time constant in seconds.</summary>
        public double FallTau { get; set; }

        /// <summary>Gets input source value for given timepoint.</summary>
        /// <param name="timepoint">The time value for which to calculate the value.</param>
        /// <returns></returns>
        public override double GetValue(double timepoint)
        {
            if (timepoint <= RiseDelay)
                return InitialLevel;
            if (timepoint <= FallDelay)
                return MathHelper.LinearInterpolation(
                    InitialLevel,
                    PulseLevel,
                    1 - Math.Exp(-(timepoint - RiseDelay) / RiseTau));
            return MathHelper.LinearInterpolation(
                InitialLevel,
                PulseLevel,
                (1 - Math.Exp(-(FallDelay - RiseDelay) / RiseTau)) *
                Math.Exp(-(timepoint - FallDelay) / FallTau));
        }
    }
}