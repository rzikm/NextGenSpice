using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Circuit
{
    public class CircuitBuilderHelpers
    {
        /// <summary>Gets connectivity components of the circuit graph using simple graph search.</summary>
        /// <param name="neighbourghs">Vertex-neighbourghs representation of the graph.</param>
        /// <returns></returns>
        public static List<int[]> GetComponents(Dictionary<int, HashSet<int>> neighbourghs)
        {
            var nodes = new HashSet<int>(Enumerable.Range(0, neighbourghs.Count));
            var components = new List<int[]>();

            while (nodes.Count > 0) // while some node is uncertain
            {
                var visited =
                    GetIdsInSameComponent(nodes.First(),
                        neighbourghs); // get component containing first node in the set
                //visited.Remove(0); // connections to the ground are ignored

                components.Add(Enumerable.ToArray<int>(visited));
                nodes.ExceptWith(visited); // remove nodes from component found
            }

            return components;
        }

        /// <summary>Creates vertex-neighbourghs representation of the circuit graph.</summary>
        public static Dictionary<int, HashSet<int>> GetNeighbourghs(int nodeCount,
            IEnumerable<ICircuitDefinitionElement> elements)
        {
            var neighbourghs = Enumerable.Range(0, nodeCount).ToDictionary(n => n, n => new HashSet<int>());

            foreach (var element in elements)
            {
                var ids = element
                    .ConnectedNodes; // consider the element to be a hyperedge - all pairs in the hyperedge are connected
                for (var i = 0; i < ids.Count; i++)
                for (var j = 0; j < ids.Count; j++)
                {
                    if (i == j) continue; // do not add connection to itself
                    neighbourghs[ids[i]].Add(ids[j]);
                }
            }

            return neighbourghs;
        }

        /// <summary>Finds all nodes that can be reached from the given start node using graph.</summary>
        /// <param name="startId">Start node id.</param>
        /// <param name="neighbourghs">Vertex-neighbourghs representation of the graph.</param>
        /// <returns></returns>
        public static HashSet<int> GetIdsInSameComponent(int startId, Dictionary<int, HashSet<int>> neighbourghs)
        {
            // use breadth-first search
            var q = new Queue<int>();
            q.Enqueue(startId);
            var visited = new HashSet<int>();
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