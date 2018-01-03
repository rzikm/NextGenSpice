using System;
using System.Collections.Generic;

namespace NextGenSpice.Core.Circuit
{
    [Serializable]
    public class NoDcPathToGroundException : CircuitTopologyException
    {
        public NoDcPathToGroundException(IEnumerable<int> nodes) : base($"Some nodes are not connected to the ground node ({string.Join(", ", nodes)})")
        {
            this.Nodes = nodes;
        }

        public IEnumerable<int> Nodes { get; }
    }
}