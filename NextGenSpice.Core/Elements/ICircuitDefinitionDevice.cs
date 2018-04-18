using System.Collections.Generic;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Devices
{
    /// <summary>
    ///     Defines basic properties and methods for every class that represents a definition of electrical circuit
    ///     device.
    /// </summary>
    public interface ICircuitDefinitionDevice
    {
        /// <summary>Set of terminal connections of this device.</summary>
        NodeConnectionSet ConnectedNodes { get; }

        /// <summary>Name identifier of this device.</summary>
        string Name { get; }

        /// <summary>Creates a deep copy of this device.</summary>
        /// <returns></returns>
        ICircuitDefinitionDevice Clone();

        /// <summary>Gets metadata about this device interconnections in the circuit.</summary>
        /// <returns></returns>
        IEnumerable<CircuitBranchMetadata> GetBranchMetadata();
    }
}