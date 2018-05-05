using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice.Core.Devices
{
    /// <summary>Class that represents a resistor device.</summary>
    public class Resistor : TwoTerminalCircuitDevice
    {
        public Resistor(double resistance, object tag = null) : base(tag)
        {
            Resistance = resistance;
        }

        /// <summary>Resistance of the device in ohms.</summary>
        public double Resistance { get; set; }

        /// <summary>Gets metadata about this device interconnections in the circuit.</summary>
        /// <returns></returns>
        public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
        {
            return Enumerable.Empty<CircuitBranchMetadata>();
        }
    }
}