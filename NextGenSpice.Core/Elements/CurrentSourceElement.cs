using NextGenSpice.Core.BehaviorParams;

namespace NextGenSpice.Core.Elements
{
    public class CurrentSourceElement : TwoNodeCircuitElement
    {
        public SourceBehaviorParams BehaviorParams { get; }

        public CurrentSourceElement(SourceBehaviorParams behavior, string name = null) : base(name)
        {
            BehaviorParams = behavior;
        }
        public CurrentSourceElement(double current, string name = null) : this(new ConstantBehaviorParams { Value = current }, name)
        {
        }
    }
}