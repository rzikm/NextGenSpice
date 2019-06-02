namespace NextGenSpice.Core.Devices
{
    /// <summary>Base class for devices that have exactly two terminals.</summary>
    public abstract class TwoTerminalCircuitDevice : CircuitDefinitionDevice
    {
        protected TwoTerminalCircuitDevice(object tag) : base(2, tag)
        {
        }

        /// <summary>Positive terminal of the device.</summary>
        public int Anode => ConnectedNodes[0];

        /// <summary>Negative terminal of the device.</summary>
        public int Cathode => ConnectedNodes[1];
    }
}