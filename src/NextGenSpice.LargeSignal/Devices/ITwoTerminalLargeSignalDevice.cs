namespace NextGenSpice.LargeSignal.Devices
{
	/// <summary>Defines basic methods and properties for large signal models of devices that have only two terminals.</summary>
	public interface ITwoTerminalLargeSignalDevice : ILargeSignalDevice
	{
		/// <summary>Voltage across this device, difference of potential between positive and negative terminals.</summary>
		double Voltage { get; }

		/// <summary>Current flowing from positive terminal to negative terminal through the device.</summary>
		double Current { get; }
	}
}