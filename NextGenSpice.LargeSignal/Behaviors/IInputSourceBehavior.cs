using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Defines methods for strategy classes directing the behavior of <see cref="LargeSignalVoltageSourceModel" /> and
    ///     <see cref="LargeSignalCurrentSourceModel" />
    /// </summary>
    public interface IInputSourceBehavior
    {
        //TODO: consider using an enum like ModelUpdateMode.

        /// <summary>
        ///     If true, the behavior is not constant over time and the value is refreshed every timestep.
        /// </summary>
        bool IsTimeDependent { get; }

        /// <summary>
        ///     If true, the behavior depends on another element and the source value is updated in every iteration of
        ///     Newton-Raphson loop.
        /// </summary>
        bool HasDependency { get; }

        /// <summary>
        ///     Gets input source value for given timepoint.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        double GetValue(ISimulationContext context);
    }
}