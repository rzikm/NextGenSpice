using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Devices;

namespace SandboxRunner
{
    public class SwitchDevice : TwoTerminalCircuitDevice
    {
        public SwitchDevice(string name = null) : base(name)
        {
        }

        public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
        {
            return Enumerable.Empty<CircuitBranchMetadata>();
        }
    }
}