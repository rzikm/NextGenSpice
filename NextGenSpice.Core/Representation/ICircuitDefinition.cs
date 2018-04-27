using System.Collections.Generic;
using NextGenSpice.Core.Devices;

namespace NextGenSpice.Core.Representation
{
    /// <summary>Defines functions and properties that identify electric circuit definition.</summary>
    public interface ICircuitDefinition
    {
        /// <summary>Number of the nodes in the circuit.</summary>
        int NodeCount { get; }

        /// <summary>Initial voltages of nodes by their id.</summary>
        IReadOnlyList<double?> InitialVoltages { get; }

        /// <summary>Set of devices that define this circuit.</summary>
        IReadOnlyList<ICircuitDefinitionDevice> Devices { get; }

        /// <summary>Returns contained device with given tag or null if no such device is found.</summary>
        /// <param name="tag">The tag of the device.</param>
        ICircuitDefinitionDevice FindDevice(object tag);
    }
}