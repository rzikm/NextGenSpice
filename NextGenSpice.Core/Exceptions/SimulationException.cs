using System;
using System.Runtime.Serialization;

namespace NextGenSpice.Core
{
    [Serializable]
    public class SimulationException : Exception
    {
        public SimulationException()
        {
        }

        public SimulationException(string message) : base(message)
        {
        }

        public SimulationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SimulationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}