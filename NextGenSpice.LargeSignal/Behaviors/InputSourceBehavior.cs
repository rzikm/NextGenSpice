using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.LargeSignal.Behaviors
{
    abstract class InputSourceBehavior<TParams> : IInputSourceBehavior where TParams : SourceBehaviorParams
    {
        protected InputSourceBehavior(TParams param)
        {
            this.param = param;
        }

        public SourceBehaviorParams Params => param;
        protected TParams param { get; }
        public abstract double GetValue(ISimulationContext context);
        public abstract bool IsTimeDependent { get; }
        public abstract bool HasDependency { get; }
    }
}