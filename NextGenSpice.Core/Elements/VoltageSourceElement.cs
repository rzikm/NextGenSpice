using NextGenSpice.Core.BehaviorParams;

namespace NextGenSpice.Core.Elements
{
    public class VoltageSourceElement : TwoNodeCircuitElement
    {
        public SourceBehaviorParams BehaviorParams { get; }

        public VoltageSourceElement(SourceBehaviorParams behavior, string name = null) : base(name)
        {
            BehaviorParams = behavior;
        }

        public VoltageSourceElement(double voltage, string name = null) : this(new ConstantBehaviorParams { Value = voltage }, name)
        {
        }
    }
}