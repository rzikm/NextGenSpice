using System;
using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice.Circuit
{
    public class CircuitBuilder
    {
        private Dictionary<int, CircuitNode> NodeLookup { get; set; }
        public IEnumerable<CircuitNode> Nodes => NodeLookup.Values;
        public List<ICircuitElement> CircuitElements { get; set; }
        public List<INonlinearCircuitElement> NonlinearCircuitElements { get; set; }

        public CircuitBuilder()
        {
            NodeLookup = new Dictionary<int, CircuitNode>();
            CircuitElements = new List<ICircuitElement>();
            NonlinearCircuitElements = new List<INonlinearCircuitElement>();
        }

        public CircuitNode AddNode(int id)
        {
            if (NodeLookup.ContainsKey(id))
                throw new ArgumentException("Node with given id is already present");

            return NodeLookup[id] = new CircuitNode { Id = id };
        }
        public void AddElement(ICircuitElement element, params int[] nodeConnections)
        {
            if (element.ConnectedNodes.Count != nodeConnections.Length)
                throw new ArgumentException("Wrong number of connections");

            // connect to nodes
            for (var i = 0; i < nodeConnections.Length; i++)
            {
                var id = nodeConnections[i];
                if (NodeLookup.TryGetValue(id, out var node))
                    element.ConnectedNodes[i] = node;
                else throw new ArgumentException($"Node {id} does not exist");
            }

            CircuitElements.Add(element);

            var nonlinear = element as INonlinearCircuitElement;

            if (nonlinear != null)
                NonlinearCircuitElements.Add(nonlinear);
        }

        public ElectricCircuit Build()
        {
            return new ElectricCircuit
            {
                Elements = CircuitElements,
                Nodes = Nodes.ToList(),
                NonlinearCircuitElements = NonlinearCircuitElements
            };
        }
    }
}