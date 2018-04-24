using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Core.Representation;

namespace NextGenSpice.Core.Circuit
{
    /// <summary>Main class for building electrical circuit representation.</summary>
    public class CircuitBuilder
    {
        private readonly List<ICircuitDefinitionDevice> devices;
        private readonly Dictionary<object, ICircuitDefinitionDevice> namedDevices;
        private readonly List<double?> nodes;
        private CircuitTopologyException circuitException;
        private bool validatedCircuit;

        public CircuitBuilder()
        {
            nodes = new List<double?>();
            devices = new List<ICircuitDefinitionDevice>();
            namedDevices = new Dictionary<object, ICircuitDefinitionDevice>();
            EnsureHasNode(0);
        }

        /// <summary>Number of nodes in the current circuit.</summary>
        public int NodeCount => nodes.Count;

        /// <summary>Set of devices curently in the circuit.</summary>
        public IReadOnlyList<ICircuitDefinitionDevice> Devices => devices;

        /// <summary>Sets initial node voltage.</summary>
        /// <param name="id">Id of the node.</param>
        /// <param name="voltage">Target voltage value in volts</param>
        /// <returns></returns>
        public CircuitBuilder SetNodeVoltage(int id, double? voltage)
        {
            if (voltage < 0) throw new ArgumentOutOfRangeException(nameof(voltage));

            EnsureHasNode(id);
            nodes[id] = voltage;
            return this;
        }

        /// <summary>Ensures that call to nodes[id] is valid and does not result in OutOfRangeExecption.</summary>
        /// <param name="id"></param>
        private void EnsureHasNode(int id)
        {
            if (id < 0) throw new ArgumentOutOfRangeException(nameof(id));

            while (NodeCount <= id)
                nodes.Add(null);
        }

        /// <summary>Adds device to the circuit and connects it to the specified nodes.</summary>
        /// <param name="nodeConnections">Ids of the nodes to which the device terminals should connect.</param>
        /// <param name="device">The device to be added.</param>
        /// <returns></returns>
        public CircuitBuilder AddDevice(int[] nodeConnections, ICircuitDefinitionDevice device)
        {
            if (device.Tag != null && namedDevices.ContainsKey(device.Tag))
                throw new InvalidOperationException($"Circuit already contains device with name '{device.Tag}'");
            if (device.ConnectedNodes.Count != nodeConnections.Length)
                throw new ArgumentException("Wrong number of connections.");
            if (devices.Contains(device))
                throw new InvalidOperationException("Cannot insert same device twice more than once.");

            // connect to nodes
            for (var i = 0; i < nodeConnections.Length; i++)
            {
                var id = nodeConnections[i];
                EnsureHasNode(id);
                device.ConnectedNodes[i] = id;
            }

            devices.Add(device);
            if (device.Tag != null)
                namedDevices[device.Tag] = device;

            // invalidate cached validation result
            validatedCircuit = false;
            circuitException = null;
            return this;
        }

        /// <summary>Verifies correctness of the circuit topology, creates new instance of circuit representation and returns it.</summary>
        /// <returns></returns>
        public CircuitDefinition BuildCircuit()
        {
            if (!ValidateCircuit()) throw circuitException;
            return new CircuitDefinition(nodes.ToArray(), devices.Select(e => e.Clone()).ToArray());
        }

        /// <summary>
        ///     Verifies correctness of the circuit topology, creates new instance of subcircuit representation and returns
        ///     it.
        /// </summary>
        /// <returns></returns>
        public SubcircuitDefinition BuildSubcircuit(int[] terminals, object tag = null)
        {
            circuitException = ValidateSubcircuit_Internal(terminals);
            if (circuitException != null) throw circuitException;

            // subtract ground node from total node count
            return new SubcircuitDefinition(NodeCount - 1, terminals, devices.Select(e => e.Clone()), tag);
        }

        /// <summary>
        ///     Verifies that current subcircuit with given nodes as terminals represents valid SPICE subcircuit. That is:
        ///     there are no floating nodes and there is a DC path between any two nodes not going through ground.
        /// </summary>
        /// <param name="terminals"></param>
        public bool ValidateSubcircuit(int[] terminals)
        {
            return ValidateSubcircuit_Internal(terminals) == null;
        }

        /// <summary>
        ///     Verifies that current subcircuit with given nodes as terminals represents valid SPICE circuit. That is: there
        ///     are no floating nodes and there is a DC path between any two nodes not going through ground.
        /// </summary>
        public bool ValidateCircuit()
        {
            if (!validatedCircuit) circuitException = ValidateCircuit_Internal();
            return circuitException == null;
        }

        /// <summary>Clears the circuit builder to allow new circuit to be built.</summary>
        public void Clear()
        {
            devices.Clear();
            namedDevices.Clear();
            nodes.Clear();
        }

        /// <summary>
        ///     Verifies that current subcircuit with given nodes as terminals represents valid SPICE subcircuit. That is:
        ///     there are no floating nodes and there is a DC path between any two nodes not going through ground.
        /// </summary>
        /// <param name="terminals"></param>
        private CircuitTopologyException ValidateSubcircuit_Internal(int[] terminals)
        {
            // ground node must not be external terminal
            if (terminals == null) throw new ArgumentNullException(nameof(terminals));
            if (terminals.Length == 0) throw new ArgumentException("Subcircuit must have at least one terminal node.");
            if (terminals.Any(n => n <= 0))
                throw new ArgumentOutOfRangeException("Terminals of Subcircuit must have positive ids.");
            if (terminals.Any(n => n >= NodeCount))
                throw new ArgumentOutOfRangeException("There is no node with given id.");

            if (validatedCircuit) return circuitException;

            var neighbourghs = CircuitBuilderHelpers.GetNeighbourghs(NodeCount, Devices);
            neighbourghs[0].Clear(); // ignore connections to the ground node

            var components = CircuitBuilderHelpers.GetComponents(neighbourghs);
            components.RemoveAll(c => c[0] == 0); // remove ground component

            if (components.Count != 1) // incorrectly connected
                return new NotConnectedSubcircuitException(components);

            var branches = devices.SelectMany(e => e.GetBranchMetadata()).ToArray();

            var cycle = GetVoltageCicrle(branches);
            return cycle != null ? new VoltageBranchCycleException(cycle) : null;
        }

        /// <summary>
        ///     Verifies that current subcircuit with given nodes as terminals represents valid SPICE circuit. That is: there
        ///     are no floating nodes and there is a DC path between any two nodes not going through ground.
        /// </summary>
        private CircuitTopologyException ValidateCircuit_Internal()
        {
            if (validatedCircuit) return circuitException;
            // every node must be transitively connected to 0 (ground)
            var neighbourghs = CircuitBuilderHelpers.GetNeighbourghs(NodeCount, Devices);

            var visited = CircuitBuilderHelpers.GetIdsInSameComponent(0, neighbourghs);

            // some nodes are not reachable from ground
            if (visited.Count != NodeCount)
                return new NoDcPathToGroundException(Enumerable.Range(0, NodeCount).Except(visited));

            var branches = devices.SelectMany(e => e.GetBranchMetadata()).ToArray();

            var cycle = GetVoltageCicrle(branches);
            if (cycle != null) return new VoltageBranchCycleException(cycle);

            var cutset = GetCurrentCutset(branches);
            return cutset != null ? new CurrentBranchCutsetException(cutset) : null;
        }


        private IEnumerable<ICircuitDefinitionDevice> GetCurrentCutset(CircuitBranchMetadata[] branches)
        {
            var currentBranches = branches.Where(b => b.BranchType == BranchType.CurrentDefined).ToArray();

            var neighbourghs = CircuitBuilderHelpers.GetNeighbourghs(NodeCount, devices);

            // remove current defined branches from the graph
            var nonCurrentElems = new HashSet<ICircuitDefinitionDevice>(devices);
            foreach (var e in currentBranches.Select(b => b.Device)) nonCurrentElems.Remove(e);
            foreach (var branch in currentBranches)
            {
                if (nonCurrentElems.Any(e =>
                    e.ConnectedNodes.Contains(branch.N1) && e.ConnectedNodes.Contains(branch.N2)))
                    continue; // some node bridges the same connection as this branch
                neighbourghs[branch.N1].Remove(branch.N2);
                neighbourghs[branch.N2].Remove(branch.N1);
            }

            var components = CircuitBuilderHelpers.GetComponents(neighbourghs);

            // get indexes of components for faster lookup
            var componentIndexes = new int[NodeCount];
            for (var i = 0; i < components.Count; i++)
                foreach (var n in components[i])
                    componentIndexes[n] = i;

            // throw away branches that do not connect nodes from different components
            var result = currentBranches
                .Where(b => componentIndexes[b.N1] != componentIndexes[b.N2]).Select(b => b.Device).ToArray();

            return result.Length > 0 ? result : null;
        }

        private IEnumerable<ICircuitDefinitionDevice> GetVoltageCicrle(CircuitBranchMetadata[] branches)
        {
            // get neighbourghs for the graph
            var neighbourghs = new Dictionary<int, HashSet<(int target, ICircuitDefinitionDevice device)>>();
            foreach (var branch in branches.Where(b => b.BranchType == BranchType.VoltageDefined))
            {
                if (!neighbourghs.TryGetValue(branch.N1, out var ne))
                    neighbourghs[branch.N1] = ne = new HashSet<(int target, ICircuitDefinitionDevice device)>();
                ne.Add((branch.N2, branch.Device));

                if (!neighbourghs.TryGetValue(branch.N2, out ne))
                    neighbourghs[branch.N2] = ne = new HashSet<(int target, ICircuitDefinitionDevice device)>();
                ne.Add((branch.N1, branch.Device));
            }

            // use depth first search to get a cycle
            var deviceStack = new Stack<ICircuitDefinitionDevice>();
            var nodeStack = new Stack<int>();
            var visited = new bool[NodeCount];

            IEnumerable<ICircuitDefinitionDevice> Recurse(int i)
            {
                if (visited[i]) return null; // already visited this path

                if (nodeStack.Contains(i)) // success
                {
                    // skip prefix (handles situations when the cycle found is a -> b -> c -> b)
                    // stack is enumerated from top
                    var cycleLength = nodeStack.TakeWhile(n => n != i).Count() + 1;
                    return deviceStack.Take(cycleLength);
                }

                nodeStack.Push(i);
                foreach ((var target, var device) in neighbourghs[i])
                {
                    if (deviceStack.Count > 0 && device == deviceStack.Peek())
                        continue; // prevent recursing indefinitely

                    deviceStack.Push(device);


                    var res = Recurse(target);
                    if (res != null) return res; // propagate success

                    deviceStack.Pop(); // backtrack
                }

                nodeStack.Pop();
                return null; // fail
            }

            IEnumerable<ICircuitDefinitionDevice> result = null;
            foreach (var i in neighbourghs.Keys)
            {
                result = Recurse(i);
                if (result != null) break;
            }

            return result;
        }
    }
}