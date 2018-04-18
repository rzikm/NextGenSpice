using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NextGenSpice.Core.Devices;

namespace NextGenSpice.Core.Exceptions
{
    [Serializable]
    public class CurrentBranchCutsetException : CircuitTopologyException
    {
        public CurrentBranchCutsetException(IEnumerable<ICircuitDefinitionDevice> devices) : base(
            "Circuit contains cutset of current defined devices.")
        {
            Devices = devices;
        }

        protected CurrentBranchCutsetException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        /// <summary>Set of devices that participated in the current branch cutset.</summary>
        public IEnumerable<ICircuitDefinitionDevice> Devices { get; }
    }
}