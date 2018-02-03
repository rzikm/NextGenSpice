using System;
using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice.Core.Circuit
{
    [Serializable]
    public class NotConnectedSubcircuit : CircuitTopologyException
    {
        public NotConnectedSubcircuit(IEnumerable<int[]> components) : base(
            $"No path connecting node sets {string.Join(", ", components.Select(c => $"({string.Join(", ", c.Select(i => i.ToString()))})"))}.")
        {
            Components = components;
        }

        /// <summary>
        ///     Non-connected components of the circuit graph.
        /// </summary>
        public IEnumerable<int[]> Components { get; }
    }
}