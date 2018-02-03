using NextGenSpice.Core.BehaviorParams;

namespace NextGenSpice.Core.Elements
{
    /// <summary>
    ///     Class that represents a current source device.
    /// </summary>
    public class VoltageSourceElement : TwoNodeCircuitElement
    {
        public VoltageSourceElement(SourceBehaviorParams behavior, string name = null) : base(name)
        {
            BehaviorParams = behavior;
        }

        public VoltageSourceElement(double voltage, string name = null) : this(
            new ConstantBehaviorParams {Value = voltage}, name)
        {
        }

        /// <summary>
        ///     Behavior parameters of the input source.
        /// </summary>
        public SourceBehaviorParams BehaviorParams { get; }
    }
}