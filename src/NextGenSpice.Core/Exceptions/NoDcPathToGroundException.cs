using System;
using System.Collections.Generic;

namespace NextGenSpice.Core.Exceptions
{
    [Serializable]
    public class NoDcPathToGroundException : CircuitTopologyException
    {
        public NoDcPathToGroundException(IEnumerable<int> nodes) : base(
            $"Some nodes are not connected to the ground node ({string.Join(", ", nodes)})")
        {
            Nodes = nodes;
        }

        /// <summary>Nodes having no DC path to the ground.</summary>
        public IEnumerable<int> Nodes { get; }
    }
}