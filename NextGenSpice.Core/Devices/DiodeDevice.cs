using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Devices.Parameters;

namespace NextGenSpice.Core.Devices
{
    /// <summary>Class that represents the diode device.</summary>
    public class DiodeDevice : TwoTerminalCircuitDevice
    {
        public DiodeDevice(DiodeParams parameters, object tag = null, double voltageHint = 0) : base(tag)
        {
            Parameters = parameters;
            VoltageHint = voltageHint;
        }

        /// <summary>Diode model parameters.</summary>
        public DiodeParams Parameters { get; set; }


        /// <summary>Hint for initial voltage across the diode in volts for faster first dc-bias calculation.</summary>
        public double VoltageHint { get; set; }

        /// <summary>Creates a deep copy of this device.</summary>
        /// <returns></returns>
        public override ICircuitDefinitionDevice Clone()
        {
            var clone = (DiodeDevice) base.Clone();
            clone.Parameters = (DiodeParams) clone.Parameters.Clone();
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