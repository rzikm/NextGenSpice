using NextGenSpice.Elements;

namespace NextGenSpice.Circuit
{
    public interface ICircuitModel
    {
        CircuitNode[] Nodes { get; }
        ILargeSignalDeviceModel[] Elements { get; }
        INonlinearLargeSignalDeviceModel[] NonlinearElements { get; }
        ILinearLargeSignalDeviceModel[] LinearElements { get; }
        ITimeDependentLargeSignalDeviceModel[] TimeDependentElements { get; }
        bool IsLinear { get; }
    }
}