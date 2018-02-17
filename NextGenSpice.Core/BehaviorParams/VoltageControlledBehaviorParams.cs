namespace NextGenSpice.Core.BehaviorParams
{
    /// <summary>
    ///     Class representing behavior simulating voltage controlled voltage source and voltage controlled current source.
    /// </summary>
    public class VoltageControlledBehaviorParams : SourceBehaviorParams
    {
        /// <summary>
        ///     Multiplying constant of the referential voltage.
        /// </summary>
        public double Gain { get; set; }

        /// <summary>
        ///     Positive node for measuring reference voltage.
        /// </summary>
        public int ReferenceAnode { get; set; }

        /// <summary>
        ///     Negative node for measuring reference voltage.
        /// </summary>
        public int ReferenceCathode { get; set; }
    }
}