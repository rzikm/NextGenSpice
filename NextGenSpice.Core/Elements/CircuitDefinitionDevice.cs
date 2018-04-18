using System.Collections.Generic;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Devices
{
    /// <summary>Base class for representing a circuit device used in circuit definition.</summary>
    public abstract class CircuitDefinitionDevice : ICircuitDefinitionDevice
    {
        protected CircuitDefinitionDevice(int terminalCount, string name)
        {
            ConnectedNodes = new NodeConnectionSet(terminalCount);
            Name = name;
        }

        /// <summary>Set of terminal connections.</summary>
        public NodeConnectionSet ConnectedNodes { get; protected set; }

        /// <summary>Name identifier of this device.</summary>
        public string Name { get; protected set; }

        /// <summary>Creates a deep copy of this device.</summary>
        /// <returns></returns>
        public virtual ICircuitDefinitionDevice Clone()
        {
            var circuitDefinitionDevice = (CircuitDefinitionDevice) MemberwiseClone();
            circuitDefinitionDevice.ConnectedNodes = ConnectedNodes.Clone();
            return circuitDefinitionDevice;
        }

        /// <summary>Gets metadata about this device interconnections in the circuit.</summary>
        /// <returns></returns>
        public abstract IEnumerable<CircuitBranchMetadata> GetBranchMetadata();
    }
}