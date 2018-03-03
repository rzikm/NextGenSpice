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

        /// <summary>
        ///     Creates a deep copy of this device.
        /// </summary>
        /// <returns></returns>
        public override ICircuitDefinitionElement Clone()
        {
            var clone = (CurrentSourceElement) base.Clone();
            clone.BehaviorParams = (SourceBehaviorParams) clone.BehaviorParams.Clone();
            return clone;
        }
    }
}