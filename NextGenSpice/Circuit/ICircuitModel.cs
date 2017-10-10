using NextGenSpice.Elements;

namespace NextGenSpice.Circuit
{
    public interface ICircuitModel
    {
        CircuitNode[] Nodes { get; }
        ICircuitModelElement[] Elements { get; }
        INonlinearCircuitModelElement[] NonlinearElements { get; }
        ILinearCircuitModelElement[] LinearElements { get; }
        ITimeDependentCircuitModelElement[] TimeDependentElements { get; }
        bool IsLinear { get; }
    }
}