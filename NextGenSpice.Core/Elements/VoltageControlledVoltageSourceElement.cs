using System.Collections.Generic;

namespace NextGenSpice.Core.Elements
{
    /// <summary>Class representing voltage controlled voltage source element.</summary>
    public class VoltageControlledVoltageSourceElement : CircuitDefinitionElement
    {
        public VoltageControlledVoltageSourceElement(double gain, string name = null) : base(4, name)
        {
            Gain = gain;
        }

        /// <summary>Positive terminal of the device.</summary>
        public int Anode => ConnectedNodes[0];

        /// <summary>Negative terminal of the device.</summary>
        public int Cathode => ConnectedNodes[1];

        /// <summary>Positive terminal of the reference voltage.</summary>
        public int ReferenceAnode => ConnectedNodes[2];

        /// <summary>Negative terminal of the reference voltage.</summary>
        public int ReferenceCathode => ConnectedNodes[3];

        /// <summary>Multiplier of the reference voltage.</summary>
        public double Gain { get; set; }

        /// <summary>Gets metadata about this device interconnections in the circuit.</summary>
        /// <returns></returns>
        public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
        {
            return new[]
            {
                new CircuitBranchMetadata(Anode, Cathode, BranchType.VoltageDefined, this)
            };
        }
    }
}