using System.Collections.Generic;

namespace NextGenSpice.Core.Devices
{
    /// <summary>Class describing contents of a subcircuit.</summary>
    public class SubcircuitDefinition : ISubcircuitDefinition
    {
        internal SubcircuitDefinition(int innerNodeCount, int[] terminalNodes,
            IEnumerable<ICircuitDefinitionDevice> devices, string name)
        {
            TerminalNodes = terminalNodes;
            InnerNodeCount = innerNodeCount;
            Devices = devices;
            SubcircuitName = name;
        }

        /// <summary>Name of this subcircuit type</summary>
        public string SubcircuitName { get; }

        /// <summary>Ids from the subcircuit definition that are considered connected to the device terminals.</summary>
        public int[] TerminalNodes { get; }

        /// <summary>Number of inner nodes of this subcircuit.</summary>
        public int InnerNodeCount { get; }

        /// <summary>Inner devices that define behavior of this subcircuit.</summary>
        public IEnumerable<ICircuitDefinitionDevice> Devices { get; }
    }
}