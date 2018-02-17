using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for voltage controlled behavior of <see cref="LargeSignalVoltageSourceModel" /> and
    ///     <see cref="LargeSignalCurrentSourceModel" />.
    /// </summary>
    internal class VoltageControlledSourceBehavior : InputSourceBehavior<VoltageControlledBehaviorParams>
    {
        public VoltageControlledSourceBehavior(VoltageControlledBehaviorParams parameters) : base(parameters)
        {
        }

        /// <summary>
        ///     Gets input source value for given timepoint.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override double GetValue(ISimulationContext context)
        {
            return (context.GetSolutionForVariable(Parameters.ReferenceAnode) -
                    context.GetSolutionForVariable(Parameters.ReferenceCathode)) * Parameters.Gain;
        }

        /// <summary>
        ///     Specifies how often the model should be updated.
        /// </summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.Always;
    }
}