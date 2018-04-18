namespace NextGenSpice.Core.Devices
{
    /// <summary>Base class for devices that have exactly two terminals.</summary>
    public abstract class TwoTerminalCircuitDevice : CircuitDefinitionDevice
    {
        protected TwoTerminalCircuitDevice(string name) : base(2, name)
        {
        }

        /// <summary>Positive terminal of the device.</summary>
        public int Anode => ConnectedNodes[0];

        /// <summary>Negative terminal of the device.</summary>
        public int Cathode => ConnectedNodes[1];
    }
}