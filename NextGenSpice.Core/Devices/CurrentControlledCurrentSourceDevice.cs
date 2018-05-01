using System.Collections.Generic;

namespace NextGenSpice.Core.Devices
{
    /// <summary>Class representing current controlled current source device.</summary>
    public class CurrentControlledCurrentSourceDevice : TwoTerminalCircuitDevice
    {
        public CurrentControlledCurrentSourceDevice(VoltageSourceDevice ampermeter, double gain, string tag = null) :
            base(tag)
        {
            Ampermeter = ampermeter;
            Gain = gain;
        }

        /// <summary>Voltage sourced device that is used as an ampermeter</summary>
        public VoltageSourceDevice Ampermeter { get; }

        /// <summary>Multiplier of the reference current to current output.</summary>
        public double Gain { get; set; }

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