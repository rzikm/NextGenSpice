using System;
using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice.Circuit
{
    public class CircuitBuilder
    {
        private readonly List<CircuitNode> nodes;
        public IReadOnlyList<CircuitNode> Nodes => nodes;
        private List<ICircuitDefinitionElement> CircuitElements { get; set; }

        public CircuitBuilder()
        {
            nodes = new List<CircuitNode>();
            CircuitElements = new List<ICircuitDefinitionElement>();
        }

        public CircuitNode GetNode(int id)
        {
            if (id < 0) throw new ArgumentOutOfRangeException(nameof(id));

            while (nodes.Count <= id)
            {
                nodes.Add(new CircuitNode { Id = nodes.Count });
            }

            return nodes[id];
        }
        public void AddElement(ICircuitDefinitionElement element, params int[] nodeConnections)
        {
            if (element.ConnectedNodes.Count != nodeConnections.Length)
                throw new ArgumentException("Wrong number of connections");

            // connect to nodes
            for (var i = 0; i < nodeConnections.Length; i++)
            {
                var id = nodeConnections[i];
                element.ConnectedNodes[i] = GetNode(id);
            }

            CircuitElements.Add(element);
        }

        public ElectricCircuitDefinition Build()
        {
            VerifyCircuit();

            return new ElectricCircuitDefinition
            {
                Elements = CircuitElements,
                Nodes = Nodes.ToList(),
            };
        }

        private void VerifyCircuit()
        {
            // every node must be transitively connected to 0 (ground)
            var neighbourghs = Nodes.ToDictionary(n => n.Id, n => new HashSet<int>());
            foreach (var element in CircuitElements)
            {
                var ids = element.ConnectedNodes.Select(n => n.Id).ToArray();
                for (int i = 0; i < ids.Length; i++)
                {
                    for (int j = 0; j < ids.Length; j++)
                    {
                        if (i == j) continue;
                        neighbourghs[ids[i]].Add(ids[j]);
                    }
                }
            }

            Queue<int> q = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();
            q.Enqueue(0);

            while (q.Count > 0)
            {
                var current = q.Dequeue();
                visited.Add(current);
                foreach (var n in neighbourghs[current])
                {
                    if (visited.Contains(n)) continue;
                    q.Enqueue(n);
                }
            }

            if (visited.Count != nodes.Count) throw new InvalidOperationException("Some nodes are not connected to ground");
        }


    }
}