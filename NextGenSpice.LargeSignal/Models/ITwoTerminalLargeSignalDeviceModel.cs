namespace NextGenSpice.LargeSignal.Models
{
    public interface ITwoTerminalLargeSignalDeviceModel : ILargeSignalDeviceModel
    {
        double Voltage { get; }
        double Current { get; }
    }
}