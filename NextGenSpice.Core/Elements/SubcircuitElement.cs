using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Elements
{
    public class SubcircuitElement : CircuitDefinitionElement
    {
        public int[] TerminalNodes { get; }
        public int InnerNodeCount { get; }
        public IEnumerable<ICircuitDefinitionElement> Elements { get; private set; }

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