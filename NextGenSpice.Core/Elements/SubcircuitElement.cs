using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Elements
{
    /// <summary>Class that represents a composite element from a set of simple ones.</summary>
    public class SubcircuitElement : CircuitDefinitionElement
    {
        public SubcircuitElement(int innerNodeCount, int[] terminalNodes,
            IEnumerable<ICircuitDefinitionElement> elements, string name = null) : base(terminalNodes.Length, name)
        {
            TerminalNodes = terminalNodes;
            InnerNodeCount = innerNodeCount;
            Elements = elements;
        }

        /// <summary>Ids from the subcircuit definition that are considered connected to the device terminals.</summary>
        public int[] TerminalNodes { get; }

        /// <summary>Number of inner nodes of this subcircuit.</summary>
        public int InnerNodeCount { get; }

        /// <summary>Inner elements that define behavior of this subcircuit.</summary>
        public IEnumerable<ICircuitDefinitionElement> Elements { get; }

        /// <summary>Creates a copy of this device.</summary>
        /// <returns></returns>
        public override ICircuitDefinitionElement Clone()
        {
            var clone = (SubcircuitElement) base.Clone();
            clone.Elements.Select(e => e.Clone()).ToArray();
            return clone;
        }

        /// <summary>Gets metadata about this device interconnections in the circuit.</summary>
        /// <returns></returns>
        public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
        {
            // return only meaningful branches to be used on level above
            var branches = Elements.SelectMany(e => e.GetBranchMetadata()).ToArray();

            return GetVoltageBranches(branches).Concat(GetCurrentBranches(branches));
        }

        private IEnumerable<CircuitBranchMetadata> GetCurrentBranches(CircuitBranchMetadata[] branches)
        {
            var neighbourghs = CircuitBuilderHelpers.GetNeighbourghs(InnerNodeCount + 1, Elements);

            // remove current defined branches
            foreach (var branch in branches.Where(b => b.BranchType == BranchType.CurrentDefined))
            {
                neighbourghs[branch.N1].Remove(branch.N2);
                neighbourghs[branch.N2].Remove(branch.N1);
            }

            var terminals = new HashSet<int>(TerminalNodes);
            var connections = new Dictionary<int, int>();
            for (int i = 0; i < ConnectedNodes.Count; i++)
            {
                connections[TerminalNodes[i]] = ConnectedNodes[i];
            }

            var components = CircuitBuilderHelpers.GetComponents(neighbourghs);

            // return branch for all pairs of terminals that are not in the same component
            for (var i = 0; i < components.Count; i++)
            {
                var c1 = components[i].Where(n => terminals.Contains(n)).ToArray();
                if (c1.Length == 0) continue;

                for (var j = i + 1; j < components.Count; j++)
                {
                    var c2 = components[j].Where(n => terminals.Contains(n)).ToArray();
                    foreach (var n1 in c1)
                    {
                        foreach (var n2 in c2)
                        {
                            yield return new CircuitBranchMetadata(connections[n1], connections[n2],
                                BranchType.CurrentDefined, this);
                        }
                    }
                }
            }
        }

        private IEnumerable<CircuitBranchMetadata> GetVoltageBranches(CircuitBranchMetadata[] branches)
        {
            // use union find structure for graph connected components
            var representatives = Enumerable.Range(0, InnerNodeCount + 1).ToArray();
            foreach (var branch in branches.Where(b => b.BranchType == BranchType.VoltageDefined).ToList())
            {
                var rep1 = GetRepresentant(representatives, branch.N1);
                var rep2 = GetRepresentant(representatives, branch.N2);

                representatives[rep1] = rep2;
            }

            // contract all
            for (int i = 0; i < representatives.Length; i++)
            {
                representatives[i] = GetRepresentant(representatives, i);
            }

            // get mapping of local nodes to outer nodes
            var connections = new Dictionary<int, int>();
            for (int i = 0; i < ConnectedNodes.Count; i++)
            {
                connections[TerminalNodes[i]] = ConnectedNodes[i];
            }

            for (int i = 0; i < TerminalNodes.Length - 1; i++)
            {
                var n1 = TerminalNodes[i];
                for (int j = i + 1; j < TerminalNodes.Length; j++)
                {
                    var n2 = TerminalNodes[j];
                    if (representatives[n1] == representatives[n2])
                    {
                        // same component, therefore a path of voltage defined branches
                        yield return new CircuitBranchMetadata(connections[n1], connections[n2],
                            BranchType.VoltageDefined, this);
                    }
                }
            }
        }

        /// <summary>Gets representative from union-find structure while contracting paths to make next query faster.</summary>
        /// <param name="representatives"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static int GetRepresentant(int[] representatives, int index)
        {
            var rep = representatives[index];
            while (index != rep)
            {
                // get representative one level up
                var newRep = representatives[rep];
                // make chain one link shorter for the lowes node
                representatives[index] = newRep;
                // advance one level up
                index = rep;
                rep = newRep;
                //repeat
            }

            return rep;
        }
    }
}