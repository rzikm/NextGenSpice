using System.Collections.Generic;

namespace NextGenSpice.Core.Elements
{
    /// <summary>Class representing voltage controlled current source element.</summary>
    public class VoltageControlledCurrentSourceElement : CircuitDefinitionElement
    {
        public VoltageControlledCurrentSourceElement(double transConductance, string name = null) : base(4, name)
        {
            TransConductance = transConductance;
        }

        /// <summary>Positive terminal of the device.</summary>
        public int Anode => ConnectedNodes[0];

        /// <summary>Negative terminal of the device.</summary>
        public int Cathode => ConnectedNodes[1];

        /// <summary>Positive terminal of the reference voltage.</summary>
        public int ReferenceAnode => ConnectedNodes[2];

        /// <summary>Negative terminal of the reference voltage.</summary>
        public int ReferenceCathode => ConnectedNodes[3];

        /// <summary>Multiplier of the reference voltage to current output.</summary>
        public double TransConductance { get; set; }

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