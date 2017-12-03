using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.BehaviorParams
{
    public abstract class BehavioredSourceElement : TwoNodeCircuitElement
    {
        protected BehavioredSourceElement(string name) : base(name)
        {
        }
    }
}