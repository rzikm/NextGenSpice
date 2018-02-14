using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for constant behavior of <see cref="LargeSignalVoltageSourceModel" /> and
    ///     <see cref="LargeSignalCurrentSourceModel" />.
    /// </summary>
    internal class ConstantSourceBehavior : InputSourceBehavior<ConstantBehaviorParams>
    {
        public ConstantSourceBehavior(ConstantBehaviorParams parameters) : base(parameters)
        {
        }

        /// <summary>
        ///     Specifies how often the model should be updated.
        /// </summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.NoUpdate;

        /// <summary>
        ///     If true, the behavior is not constant over time and the value is refreshed every timestep.
        /// </summary>
        public override bool IsTimeDependent => false;

        /// <summary>
        ///     If true, the behavior depends on another element and the source value is updated in every iteration of
        ///     Newton-Raphson loop.
        /// </summary>
        public override bool HasDependency => false;

        /// <summary>
        ///     Gets input source value for given timepoint.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override double GetValue(ISimulationContext context)
        {
            return Parameters.Value;
        }
    }
}