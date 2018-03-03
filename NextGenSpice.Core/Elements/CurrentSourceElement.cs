using NextGenSpice.Core.BehaviorParams;

namespace NextGenSpice.Core.Elements
{
    /// <summary>
    ///     Class that represents a current source device.
    /// </summary>
    public class CurrentSourceElement : TwoNodeCircuitElement
    {
        public CurrentSourceElement(SourceBehaviorParams behavior, string name = null) : base(name)
        {
            BehaviorParams = behavior;
        }

        public CurrentSourceElement(double current, string name = null) : this(
            new ConstantBehaviorParams {Value = current}, name)
        {
        }

        /// <summary>
        ///     Behavior parameters of the input source.
        /// </summary>
        public SourceBehaviorParams BehaviorParams { get; set; }
    }
}