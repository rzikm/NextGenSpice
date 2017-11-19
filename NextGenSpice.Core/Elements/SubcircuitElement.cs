using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Elements
{
    public class SubcircuitElement : ICircuitDefinitionElement
    {
        public int[] TerminalNodes { get; }
        public int InnerNodeCount { get; }
        public IEnumerable<ICircuitDefinitionElement> Elements { get; }
        public NodeConnectionSet ConnectedNodes { get; }
        public ICircuitDefinitionElement Clone()
        {
            return new SubcircuitElement(InnerNodeCount, TerminalNodes, Elements.Select(e => e.Clone()));
        }

        protected internal SubcircuitElement(int innerNodeCount, int[] terminalNodes, IEnumerable<ICircuitDefinitionElement> elements)
        {
            TerminalNodes = terminalNodes;
            InnerNodeCount = innerNodeCount;
            Elements = elements;
            ConnectedNodes = new NodeConnectionSet(terminalNodes.Length);
        }
    }
}