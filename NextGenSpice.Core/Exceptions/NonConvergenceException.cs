using System;
using System.Runtime.Serialization;

namespace NextGenSpice.Core.Exceptions
{
    [Serializable]
    public class NonConvergenceException : SimulationException
    {
        public NonConvergenceException()
        {
        }

        public NonConvergenceException(string message) : base(message)
        {
        }

        public NonConvergenceException(string message, Exception inner) : base(message, inner)
        {
        }

        protected NonConvergenceException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}