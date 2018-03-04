using System;
using System.Runtime.Serialization;

namespace NextGenSpice.Core.Exceptions
{
    [Serializable]
    public abstract class CircuitTopologyException : Exception
    {
        protected CircuitTopologyException()
        {
        }

        protected CircuitTopologyException(string message) : base(message)
        {
        }

        protected CircuitTopologyException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CircuitTopologyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}