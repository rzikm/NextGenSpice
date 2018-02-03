using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Base class for defining strategy classes for behavior of <see cref="LargeSignalVoltageSourceModel" /> and
    ///     <see cref="LargeSignalCurrentSourceModel" />.
    /// </summary>
    /// <typeparam name="TParams">Type defining the set of parameters for the behavior.</typeparam>
    internal abstract class InputSourceBehavior<TParams> : IInputSourceBehavior where TParams : SourceBehaviorParams
    {
        protected InputSourceBehavior(TParams parameters)
        {
            Parameters = parameters;
        }

        /// <summary>
        ///     Set of parameters for this behavior class.
        /// </summary>
        protected TParams Parameters { get; }

        /// <summary>
        ///     Gets input source value for given timepoint.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract double GetValue(ISimulationContext context);

        /// <summary>
        ///     If true, the behavior is not constant over time and the value is refreshed every timestep.
        /// </summary>
        public abstract bool IsTimeDependent { get; }

        /// <summary>
        ///     If true, the behavior depends on another element and the source value is updated in every iteration of
        ///     Newton-Raphson loop.
        /// </summary>
        public abstract bool HasDependency { get; }
    }
}