using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Devices;

namespace NextGenSpiceTest
{
    public class SwitchDevice : TwoTerminalCircuitDevice
    {
        public SwitchDevice(string name = null) : base(name)
        {
        }

        /// <summary>Gets metadata about this device interconnections in the circuit.</summary>
        /// <returns></returns>
        public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
        {
            return Enumerable.Empty<CircuitBranchMetadata>();
        }
    }
}