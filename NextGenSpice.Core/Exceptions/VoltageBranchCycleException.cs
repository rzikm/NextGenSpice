using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Exceptions
{
    [Serializable]
    public class VoltageBranchCycleException : CircuitTopologyException
    {
        public VoltageBranchCycleException(IEnumerable<ICircuitDefinitionElement> elements) : base(
            "Circuit contains a cycle of voltage defined elements.")
        {
            Elements = elements;
        }

        protected VoltageBranchCycleException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        /// <summary>Set of elements that participated in the voltage branch cycle.</summary>
        public IEnumerable<ICircuitDefinitionElement> Elements { get; }
    }
}