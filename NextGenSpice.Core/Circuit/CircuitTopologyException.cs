using System;
using System.Collections.Generic;

namespace NextGenSpice.Core.Circuit
{
    public class CircuitTopologyException : Exception
    {
        public CircuitTopologyException(IEnumerable<int> nodes) : base($"Some nodes are not connected to the ground node ({string.Join(", ", nodes)})")
        {
            this.Nodes = nodes;
        }

        public IEnumerable<int> Nodes { get; set; }
    }
}