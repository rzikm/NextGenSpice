using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Devices.Parameters;

namespace NextGenSpice.Core.Devices
{
    /// <summary>Class that represents Homo-Junction Bipolar Transistor device.</summary>
    public class BjtDevice : CircuitDefinitionDevice
    {
        public BjtDevice(BjtParams parameters, string tag = null) : base(4, tag)
        {
            Parameters = parameters;
        }

        /// <summary>Node connected to collector terminal of the transistor.</summary>
        public int Collector => ConnectedNodes[0];

        /// <summary>Node connected to base terminal of the transistor.</summary>
        public int Base => ConnectedNodes[1];

        /// <summary>Node connected to emitter terminal of the transistor.</summary>
        public int Emitter => ConnectedNodes[2];

        /// <summary>Node connected to substrate terminal of the transistor.</summary>
        public int Substrate => ConnectedNodes[3];

        /// <summary>Set of model parameters for this device.</summary>
        public BjtParams Parameters { get; set; }

        /// <summary>Creates a deep copy of this device.</summary>
        /// <returns></returns>
        public override ICircuitDefinitionDevice Clone()
        {
            var clone = (BjtDevice) base.Clone();
            clone.Parameters = (BjtParams) clone.Parameters.Clone();
            return clone;
        }

        /// <summary>Gets metadata about this device interconnections in the circuit.</summary>
        /// <returns></returns>
        public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
        {
            return Enumerable.Empty<CircuitBranchMetadata>();
        }
    }
}