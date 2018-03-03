using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace SandboxRunner
{
    public class SwitchElement : TwoNodeCircuitElement
    {
        public SwitchElement(string name = null) : base(name)
        {
        }
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