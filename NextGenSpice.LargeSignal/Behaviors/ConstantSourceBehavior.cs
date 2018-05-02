using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Devices;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for constant behavior of <see cref="LargeSignalVoltageSource" /> and
    ///     <see cref="LargeSignalCurrentSource" />.
    /// </summary>
    internal class ConstantSourceBehavior : InputSourceBehavior<ConstantBehavior>
    {
        public ConstantSourceBehavior(ConstantBehavior parameters) : base(parameters)
        {
        }

        /// <summary>Gets input source value for given timepoint.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override double GetValue(ISimulationContext context)
        {
            return Parameters.Value;
        }
    }
}