namespace NextGenSpice.Core.BehaviorParams
{
    /// <summary>
    ///     Specifies behavior parameters for input source with exponential rising and falling edges.
    /// </summary>
    public class ExponentialBehaviorParams : SourceBehaviorParams
    {
        /// <summary>
        ///     Value on the start of the simulation in volts or ampers.
        /// </summary>
        public double InitialLevel { get; set; }

        /// <summary>
        ///     Value in volts or ampers to which source exponentially converges after RiseDelay.
        /// </summary>
        public double PulseLevel { get; set; }

        /// <summary>
        ///     Time delay in seconds before start of leading edge.
        /// </summary>
        public double RiseDelay { get; set; }

        /// <summary>
        ///     Leading edge rise time constant in seconds.
        /// </summary>
        public double TauRise { get; set; }

        /// <summary>
        ///     Time delay in seconds before start of trailing edge.
        /// </summary>
        public double FallDelay { get; set; }

        /// <summary>
        ///     Trailing edge fall time constant in seconds.
        /// </summary>
        public double TauFall { get; set; }
    }
}