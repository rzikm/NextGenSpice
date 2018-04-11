using System.Collections.Generic;
using System.Linq;
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
            return Enumerable.Empty<CircuitBranchMetadata>();
        }
    }
}