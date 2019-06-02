using System;
using System.Runtime.Serialization;

namespace NextGenSpice.Core.Exceptions
{
    [Serializable]
    public class IterationCountExceededException : SimulationException
    {
        public IterationCountExceededException() : base("Newton-Raphson iterations did not converge.")
        {
        }
        protected IterationCountExceededException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class NaNInEquationSystemSolutionException : SimulationException
    {
        public NaNInEquationSystemSolutionException() : this(null)
        {
        }

        public NaNInEquationSystemSolutionException(Exception inner) : base("NaN in equation system solution.", inner)
        {
        }
        protected NaNInEquationSystemSolutionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}