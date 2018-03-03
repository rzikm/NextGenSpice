using System.Collections.Generic;

namespace NextGenSpice.Core.Elements
{
    /// <summary>
    ///     Class that represents a resistor device.
    /// </summary>
    public class ResistorElement : TwoNodeCircuitElement
    {
        public ResistorElement(double resistance, string name = null) : base(name)
        {
            Resistance = resistance;
        }

        /// <summary>
        ///     Resistance of the device in ohms.
        /// </summary>
        public double Resistance { get; set; }

        /// <summary>
        ///     Gets metadata about this device interconnections in the circuit.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
        {
            return new[]
            {
                new CircuitBranchMetadata
                {
                    N1 = Anode,
                    N2 = Cathode,
                    BranchType = BranchType.Mixed
                }
            };
        }
    }
}