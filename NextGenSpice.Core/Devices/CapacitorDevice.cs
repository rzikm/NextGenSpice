using System.Collections.Generic;

namespace NextGenSpice.Core.Devices
{
    /// <summary>Class that represents a capacitor device.</summary>
    public class CapacitorDevice : TwoTerminalCircuitDevice
    {
        public CapacitorDevice(double capacity, double? initialVoltage = null, object tag = null) : base(tag)
        {
            Capacity = capacity;
            InitialVoltage = initialVoltage;
        }

        /// <summary>Capacity in farads.</summary>
        public double Capacity { get; set; }

        /// <summary>Initial voltage across the capacitor in volts.</summary>
        public double? InitialVoltage { get; set; }

        /// <summary>Gets metadata about this device interconnections in the circuit.</summary>
        /// <returns></returns>
        public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
        {
            return new[]
            {
                new CircuitBranchMetadata(Anode, Cathode, BranchType.CurrentDefined, this)
            };
        }
    }
}