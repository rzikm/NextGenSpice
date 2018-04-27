using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Devices
{
    /// <summary>Base class for representing a circuit device used in circuit definition.</summary>
    public abstract class CircuitDefinitionDevice : ICircuitDefinitionDevice
    {
        protected CircuitDefinitionDevice(int terminalCount, object tag)
        {
            ConnectedNodes = new NodeConnectionSet(terminalCount);
            Tag = tag;
        }

        /// <summary>Set of terminal connections.</summary>
        public NodeConnectionSet ConnectedNodes { get; private set; }

        /// <summary>Identifier of this device.</summary>
        public object Tag { get; }

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
        public virtual IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
        {
            return Enumerable.Empty<CircuitBranchMetadata>();
        }
    }
}