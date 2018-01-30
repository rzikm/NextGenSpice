using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Representation;

namespace NextGenSpice.Core.Circuit
{
    /// <summary>
    /// Main class for building electrical circuit representation.
    /// </summary>
    public class CircuitBuilder
    {
        private readonly List<double> nodes;
        private readonly List<ICircuitDefinitionElement> elements;
        private readonly Dictionary<string, ICircuitDefinitionElement> namedElements;
        
        /// <summary>
        /// Number of nodes in the current circuit.
        /// </summary>
        public int NodeCount => nodes.Count;

        /// <summary>
        /// Set of elements curently in the circuit.
        /// </summary>
        public IReadOnlyList<ICircuitDefinitionElement> Elements => elements;

        public CircuitBuilder()
        {
            nodes = new List<double>();
            elements = new List<ICircuitDefinitionElement>();
            namedElements = new Dictionary<string, ICircuitDefinitionElement>();
        }

        // TODO: remove this method? this feature is currently not supported
        /// <summary>
        /// Sets initial node voltage.
        /// </summary>
        /// <param name="id">Id of the node.</param>
        /// <param name="voltage">Target voltage value in volts</param>
        /// <returns></returns>
        public CircuitBuilder SetNodeVoltage(int id, double voltage)
        {
            if (voltage < 0) throw new ArgumentOutOfRangeException(nameof(voltage));

            EnsureHasNode(id);
            nodes[id] = voltage;
            return this;
        }

        /// <summary>
        /// Ensures that call to nodes[id] is valid and does not result in OutOfRangeExecption.
        /// </summary>
        /// <param name="id"></param>
        private void EnsureHasNode(int id)
        {
            if (id < 0) throw new ArgumentOutOfRangeException(nameof(id));

            while (NodeCount <= id)
            {
                nodes.Add(0);
            }
        }

        /// <summary>
        /// Adds element to the circuit and connects it to the specified nodes.
        /// </summary>
        /// <param name="nodeConnections">Ids of the nodes to which the element terminals should connect.</param>
        /// <param name="element">The element to be added.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Verifies correctness of the circuit topology, creates new instance of circuit representation and returns it.
        /// </summary>
        /// <returns></returns>
        public ElectricCircuitDefinition BuildCircuit()
        {
            VerifyCircuit();

            return new ElectricCircuitDefinition(nodes.ToArray(), elements.Select(e => e.Clone()).ToArray());
        }

        /// <summary>
        /// Verifies correctness of the circuit topology, creates new instance of subcircuit representation and returns it.
        /// </summary>
        /// <returns></returns>
        public SubcircuitElement BuildSubcircuit(int [] terminals)
        {
            VerifySubcircuit(terminals);

            // subtract ground node from total node count
            return new SubcircuitElement(NodeCount - 1, terminals, elements.Select(e => e.Clone()));
        }

        /// <summary>
        /// Verifies that current subcircuit with given nodes as terminals represents valid SPICE subcircuit. That is: there are no floating nodes and there is a DC path between any two nodes not going through ground.
        /// </summary>
        /// <param name="terminals"></param>
        private void VerifySubcircuit(int [] terminals)
        {
            // ground node must not be external terminal
            if (terminals == null) throw new ArgumentNullException(nameof(terminals));
            if (terminals.Length == 0) throw new ArgumentException("Subcircuit must have at least one terminal node.");
            if (terminals.Any(n => n <= 0)) throw new ArgumentOutOfRangeException("Terminals of Subcircuit must have positive ids.");
            if (terminals.Any(n => n >= NodeCount)) throw new ArgumentOutOfRangeException("There is no node with given id.");

            var neighbourghs = GetNeighbourghs();
            neighbourghs[0].Clear(); // ignore connections to the ground node

            var components = GetComponents(neighbourghs);
            if (components.Count != 1) // incorrectly connected
            {
                throw new NotConnectedSubcircuit(components);
            }
        }

        /// <summary>
        /// Verifies that current subcircuit with given nodes as terminals represents valid SPICE circuit. That is: there are no floating nodes and there is a DC path between any two nodes not going through ground.
        /// </summary>
        /// <param name="terminals"></param>
        private void VerifyCircuit()
        {
            // every node must be transitively connected to 0 (ground)
            var neighbourghs = GetNeighbourghs();

            var visited = GetIdsInSameComponent(0, neighbourghs);

            // some nodes are not reachable from ground
            if (visited.Count != NodeCount) throw new NoDcPathToGroundException(Enumerable.Range(0, NodeCount).Except(visited));
        }

        /// <summary>
        /// Gets connectivity components of the circuit graph using simple graph search.
        /// </summary>
        /// <param name="neighbourghs">Vertex-neighbourghs representation of the graph.</param>
        /// <returns></returns>
        private List<int[]> GetComponents(Dictionary<int, HashSet<int>> neighbourghs)
        {
            var nodes = new HashSet<int>(Enumerable.Range(1, NodeCount - 1));
            List<int[]> components = new List<int[]>();

            while (nodes.Count > 0) // while some node is uncertain
            {
                var visited = GetIdsInSameComponent(nodes.First(), neighbourghs); // get component containing first node in the set
                visited.Remove(0); // connections to the ground are ignored

                components.Add(visited.ToArray());
                nodes.ExceptWith(visited); // remove nodes from component found
            }
            return components;
        }

        /// <summary>
        /// Creates vertex-neighbourghs representation of the circuit graph.
        /// </summary>
        private Dictionary<int, HashSet<int>> GetNeighbourghs()
        {
            var neighbourghs = Enumerable.Range(0, NodeCount).ToDictionary(n => n, n => new HashSet<int>());

            foreach (var element in elements)
            {
                var ids = element.ConnectedNodes; // consider the element to be a hyperedge - all pairs in the hyperedge are connected
                for (int i = 0; i < ids.Count; i++)
                {
                    for (int j = 0; j < ids.Count; j++)
                    {
                        if (i == j) continue; // do not add connection to itself
                        neighbourghs[ids[i]].Add(ids[j]);
                    }
                }
            }

            return neighbourghs;
        }

        /// <summary>
        /// Finds all nodes that can be reached from the given start node using graph.
        /// </summary>
        /// <param name="startId">Start node id.</param>
        /// <param name="neighbourghs">Vertex-neighbourghs representation of the graph.</param>
        /// <returns></returns>
        private static HashSet<int> GetIdsInSameComponent(int startId, Dictionary<int, HashSet<int>> neighbourghs)
        {
            // use breadth-first search
            Queue<int> q = new Queue<int>();
            q.Enqueue(startId);
            HashSet<int> visited = new HashSet<int>();
            while (q.Count > 0) // while fringe is nonempty
            {
                var current = q.Dequeue();
                visited.Add(current); // close current node.
                foreach (var n in neighbourghs[current])
                {
                    if (visited.Contains(n)) continue; // already visited node, do not open again.
                    q.Enqueue(n); // open new node
                }
            }
            return visited;
        }
    }
}