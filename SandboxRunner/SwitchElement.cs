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
                new CircuitBranchMetadata(Anode, Cathode, BranchType.Mixed, this)
            };
        }
    }
}