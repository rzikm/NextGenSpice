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
        private readonly List<ICircuitDefinitionElement> elements;
        private readonly Dictionary<string, ICircuitDefinitionElement> namedElements;

        public IReadOnlyList<ICircuitDefinitionElement> Elements => elements;

        public CircuitBuilder()
        {
            nodes = new List<double>();
            elements = new List<ICircuitDefinitionElement>();
            namedElements = new Dictionary<string, ICircuitDefinitionElement>();
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
            if (element.Name != null && namedElements.ContainsKey(element.Name))
                throw new InvalidOperationException($"Circuit already contains element with name '{element.Name}'");
            if (element.ConnectedNodes.Count != nodeConnections.Length)
                throw new ArgumentException("Wrong number of connections.");
            if (elements.Contains(element))
                throw new InvalidOperationException("Cannot insert same device twice more than once.");

            // connect to nodes
            for (var i = 0; i < nodeConnections.Length; i++)
            {
                var id = nodeConnections[i];
                EnsureHasNode(id);
                element.ConnectedNodes[i] = id;
            }

            elements.Add(element);
            if (element.Name != null)
                namedElements[element.Name] = element;
            return this;
        }

        public ElectricCircuitDefinition BuildCircuit()
        {
            VerifyCircuit();

            return new ElectricCircuitDefinition(nodes.ToArray(), elements.Select(e => e.Clone()).ToArray());
        }

        public SubcircuitElement BuildSubcircuit(int [] terminals)
        {
            VerifySubcircuit(terminals);

            // subtract ground node from total node count
            return new SubcircuitElement(NodeCount - 1, terminals, elements.Select(e => e.Clone()));
        }

        private void VerifySubcircuit(int [] terminals)
        {
            // ground node must not be external terminal
            if (terminals == null) throw new ArgumentNullException(nameof(terminals));
            if (terminals.Length == 0) throw new ArgumentException("Subcircuit must have at least one terminal node.");
            if (terminals.Any(n => n <= 0)) throw new ArgumentOutOfRangeException("Terminals of Subcircuit must have positive ids.");
            if (terminals.Any(n => n >= NodeCount)) throw new ArgumentOutOfRangeException("There is no node with given id.");
            
            var neighbourghs = Enumerable.Range(0, NodeCount).ToDictionary(n => n, n => new HashSet<int>());
            GetNeighbourghs(neighbourghs);
            neighbourghs[0].Clear(); // ignore connections to the ground node

            var components = GetComponents(neighbourghs);
            if (components.Count != 1) // incorrectly connected
            {
                throw new NotConnectedSubcircuit(components);
            }
        }

        private List<int[]> GetComponents(Dictionary<int, HashSet<int>> neighbourghs)
        {
            var nodes = new HashSet<int>(Enumerable.Range(1, NodeCount - 1));
            List<int[]> components = new List<int[]>();
            while (true)
            {
                var visited = GetIdsInSameComponent(nodes.First(), neighbourghs);
                visited.Remove(0);
                components.Add(visited.ToArray());
                nodes.ExceptWith(visited);

                if (nodes.Count == 0) break;
            }
            return components;
        }

        private void VerifyCircuit()
        {
            // every node must be transitively connected to 0 (ground)
            var neighbourghs = Enumerable.Range(0, NodeCount).ToDictionary(n => n, n => new HashSet<int>());
            GetNeighbourghs(neighbourghs);

            var visited = GetIdsInSameComponent(0, neighbourghs);

            // some nodes are not reachable from ground
            if (visited.Count != NodeCount) throw new NoDcPathToGroundException(Enumerable.Range(0, NodeCount).Except(visited));
        }

        private void GetNeighbourghs(Dictionary<int, HashSet<int>> neighbourghs)
        {
            foreach (var element in elements)
            {
                var ids = element.ConnectedNodes;
                for (int i = 0; i < ids.Count; i++)
                {
                    for (int j = 0; j < ids.Count; j++)
                    {
                        if (i == j) continue; // skip connections to ground
                        neighbourghs[ids[i]].Add(ids[j]);
                    }
                }
            }
        }

        private static HashSet<int> GetIdsInSameComponent(int startId, Dictionary<int, HashSet<int>> neighbourghs)
        {
            Queue<int> q = new Queue<int>();
            q.Enqueue(startId);
            HashSet<int> visited = new HashSet<int>();
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
            return visited;
        }
    }
}