using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.LargeSignal.Behaviors
{
    public interface IInputSourceBehavior
    {
        double GetValue(ISimulationContext context);

        bool IsTimeDependent { get; }

        bool HasDependency { get; }
    }
}