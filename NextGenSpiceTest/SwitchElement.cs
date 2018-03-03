using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace NextGenSpiceTest
{
    public class SwitchElement : TwoNodeCircuitElement
    {
        public SwitchElement(string name = null) : base(name)
        {
        }

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