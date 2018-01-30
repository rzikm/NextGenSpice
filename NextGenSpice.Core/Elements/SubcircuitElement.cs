using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Elements
{
    /// <summary>
    /// Class that represents a composite element from a set of simple ones.
    /// </summary>
    public class SubcircuitElement : CircuitDefinitionElement
    {
        /// <summary>
        /// Ids from the subcircuit definition that are considered connected to the device terminals.
        /// </summary>
        public int[] TerminalNodes { get; }

        /// <summary>
        /// Number of inner nodes of this subcircuit.
        /// </summary>
        public int InnerNodeCount { get; }

        /// <summary>
        /// Inner elements that define behavior of this subcircuit.
        /// </summary>
        public IEnumerable<ICircuitDefinitionElement> Elements { get; private set; }


        /// <summary>
        /// Creates a copy of this device.
        /// </summary>
        /// <returns></returns>
        public override ICircuitDefinitionElement Clone()
        {
            var clone = (SubcircuitElement) base.Clone();
            clone.Elements.Select(e => e.Clone()).ToArray();
            return clone;
        }

        protected internal SubcircuitElement(int innerNodeCount, int[] terminalNodes, IEnumerable<ICircuitDefinitionElement> elements, string name = null) : base(terminalNodes.Length, name)
        {
            TerminalNodes = terminalNodes;
            InnerNodeCount = innerNodeCount;
            Elements = elements;
        }
    }
}