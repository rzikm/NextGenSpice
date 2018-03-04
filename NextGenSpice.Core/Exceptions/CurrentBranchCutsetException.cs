using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Exceptions
{
    [Serializable]
    public class CurrentBranchCutsetException : CircuitTopologyException
    {
        /// <summary>
        ///     Set of elements that participated in the current branch cutset.
        /// </summary>
        public IEnumerable<ICircuitDefinitionElement> Elements { get; }

        public CurrentBranchCutsetException(IEnumerable<ICircuitDefinitionElement> elements) : base("Circuit contains cutset of current defined elements.")
        {
            Elements = elements;
        }

        protected CurrentBranchCutsetException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}