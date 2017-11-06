using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Representation;

namespace NextGenSpice.Core.Circuit
{
    public class CircuitBuilder
    {
        private readonly List<double> nodes;

        public int NodeCount => nodes.Count;
        private List<ICircuitDefinitionElement> CircuitElements { get; set; }

        public CircuitBuilder()
        {
            nodes = new List<double>();
            CircuitElements = new List<ICircuitDefinitionElement>();
        }

        public CircuitBuilder SetNodeVoltage(int id, double voltage)
        {
            if (voltage < 0) throw new ArgumentOutOfRangeException(nameof(voltage));

            EnsureHasNode(id);
            nodes[id] = voltage;
            return this;
        }

        private void EnsureHasNode(int id)
        {
            if (id < 0) throw new ArgumentOutOfRangeException(nameof(id));

            while (NodeCount <= id)
            {
                nodes.Add(0);
            }
        }
        public CircuitBuilder AddElement(int[] nodeConnections, ICircuitDefinitionElement element)
        {
            if (element.ConnectedNodes.Count != nodeConnections.Length)
                throw new ArgumentException("Wrong number of connections");

            // connect to nodes
            for (var i = 0; i < nodeConnections.Length; i++)
            {
                var id = nodeConnections[i];
                EnsureHasNode(id);
                element.ConnectedNodes[i] = id;
            }

            CircuitElements.Add(element);
            return this;
        }

        public ElectricCircuitDefinition Build()
        {
            VerifyCircuit();

            var electricCircuitDefinition = new ElectricCircuitDefinition(nodes, CircuitElements);
            return electricCircuitDefinition;
        }

        private void VerifyCircuit()
        {
            // every node must be transitively connected to 0 (ground)
            var neighbourghs = Enumerable.Range(0, NodeCount).ToDictionary(n => n, n => new HashSet<int>());
            foreach (var element in CircuitElements)
            {
                var ids = element.ConnectedNodes;
                for (int i = 0; i < ids.Count; i++)
                {
                    for (int j = 0; j < ids.Count; j++)
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

            if (visited.Count != NodeCount) throw new InvalidOperationException("Some nodes are not connected to ground");
        }


    }
}