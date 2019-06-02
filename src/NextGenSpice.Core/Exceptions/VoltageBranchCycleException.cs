using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NextGenSpice.Core.Devices;

namespace NextGenSpice.Core.Exceptions
{
    [Serializable]
    public class VoltageBranchCycleException : CircuitTopologyException
    {
        public VoltageBranchCycleException(IEnumerable<ICircuitDefinitionDevice> devices) : base(
            "Circuit contains a cycle of voltage defined devices.")
        {
            Devices = devices;
        }

        protected VoltageBranchCycleException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        /// <summary>Set of devices that participated in the voltage branch cycle.</summary>
        public IEnumerable<ICircuitDefinitionDevice> Devices { get; }
    }
}