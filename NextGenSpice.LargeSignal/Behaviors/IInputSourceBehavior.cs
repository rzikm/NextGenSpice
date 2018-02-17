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
        /// <summary>
        ///     Specifies how often the model should be updated.
        /// </summary>
        ModelUpdateMode UpdateMode { get; }

        /// <summary>
        ///     Gets input source value for given timepoint.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        double GetValue(ISimulationContext context);
    }
}