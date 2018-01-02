using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NextGenSpice.Core.Circuit
{
    [Serializable]
    public abstract class CircuitTopologyException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public CircuitTopologyException()
        {
        }

        public CircuitTopologyException(string message) : base(message)
        {
        }

        public CircuitTopologyException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CircuitTopologyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class NoDcPathToGroundException : CircuitTopologyException
    {
        public NoDcPathToGroundException(IEnumerable<int> nodes) : base($"Some nodes are not connected to the ground node ({string.Join(", ", nodes)})")
        {
            this.Nodes = nodes;
        }

        public IEnumerable<int> Nodes { get; }
    }

    [Serializable]
    public class NotConnectedSubcircuit : CircuitTopologyException
    {
        public NotConnectedSubcircuit(IEnumerable<int[]> components) : base($"No path connecting node sets {string.Join(", ", components.Select(c => $"({String.Join(", ", c.Select(i => i.ToString()))})"))}.")
        {
            this.Components = components;
        }

        public IEnumerable<int[]> Components { get; }

    }
}