using System.Collections.Generic;
using NextGenSpice.Core.Devices;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Base class for large signal device models that have exactly two terminals.</summary>
    /// <typeparam name="TDefinitionDevice">Class used for the device in the circuit definition that this class is model for.</typeparam>
    public abstract class TwoTerminalLargeSignalDevice<TDefinitionDevice> : LargeSignalDeviceBase<TDefinitionDevice>,
        ITwoTerminalLargeSignalDevice
        where TDefinitionDevice : TwoTerminalCircuitDevice
    {
        protected TwoTerminalLargeSignalDevice(TDefinitionDevice definitionDevice) : base(definitionDevice)
        {
        }

        /// <summary>Id of node connected to positive terminal of this device.</summary>
        public int Anode => DefinitionDevice.ConnectedNodes[0];

        /// <summary>Id of node connected to negative terminal of this device.</summary>
        public int Cathode => DefinitionDevice.ConnectedNodes[1];

        /// <summary>Current flowing from positive terminal to negative terminal through the device.</summary>
        public double Current { get; protected set; }

        /// <summary>Voltage across this device, difference of potential between positive and negative terminals.</summary>
        public double Voltage { get; protected set; }


        /// <summary>
        ///     Gets provider instance for specified attribute value or null if no provider for requested parameter exists.
        ///     For example "I" for the current flowing throught the two terminal device.
        /// </summary>
        /// <returns>IPrintValueProvider for specified attribute.</returns>
        public override IEnumerable<IDeviceStatsProvider> GetDeviceStatsProviders()
        {
            return new[]
            {
                new SimpleDeviceStatsProvider("I", () => Current),
                new SimpleDeviceStatsProvider("V", () => Voltage)
            };
        }
    }
}