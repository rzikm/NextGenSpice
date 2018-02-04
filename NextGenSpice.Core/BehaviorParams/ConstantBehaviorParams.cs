namespace NextGenSpice.Core.BehaviorParams
{
    /// <summary>
    ///     Specifies behavior parameters for input source with constant output.
    /// </summary>
    public class ConstantBehaviorParams : SourceBehaviorParams
    {
        /// <summary>
        ///     Value of the source in volts or ampers.
        /// </summary>
        public double Value { get; set; }
    }
}