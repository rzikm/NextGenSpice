using System.Collections.Generic;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Elements
{
    /// <summary>Base class for representing a circuit device used in circuit definition.</summary>
    public abstract class CircuitDefinitionElement : ICircuitDefinitionElement
    {
        protected CircuitDefinitionElement(int terminalCount, string name)
        {
            ConnectedNodes = new NodeConnectionSet(terminalCount);
            Name = name;
        }

        /// <summary>Set of terminal connections.</summary>
        public NodeConnectionSet ConnectedNodes { get; protected set; }

        /// <summary>Name identifier of this device.</summary>
        public string Name { get; protected set; }

        /// <summary>Creates a deep copy of this device.</summary>
        /// <returns></returns>
        public virtual ICircuitDefinitionElement Clone()
        {
            var circuitDefinitionElement = (CircuitDefinitionElement) MemberwiseClone();
            circuitDefinitionElement.ConnectedNodes = ConnectedNodes.Clone();
            return circuitDefinitionElement;
        }

        /// <summary>Gets metadata about this device interconnections in the circuit.</summary>
        /// <returns></returns>
        public abstract IEnumerable<CircuitBranchMetadata> GetBranchMetadata();
    }
}