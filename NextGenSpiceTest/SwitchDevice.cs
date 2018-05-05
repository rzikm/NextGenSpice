using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Devices;

namespace NextGenSpice.Test
{
    public class SwitchDevice : TwoTerminalCircuitDevice
    {
        public SwitchDevice(string name = null) : base(name)
        {
        }
    }
}