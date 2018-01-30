using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.LargeSignal.Behaviors
{
    internal class SffmSourceBehavior : InputSourceBehavior<SffmBehaviorParams>
    {
        public SffmSourceBehavior(SffmBehaviorParams param) : base(param)
        {
        }

        public override double GetValue(ISimulationContext context)
        {
            var c = 2 * Math.PI * (context.Time);
            var phaseCarrier = c * param.FrequencyCarrier;
            var phaseSignal = c * param.FrequencySignal;

            return param.DcOffset +
                   param.Amplitude * Math.Sin(phaseCarrier + param.ModulationIndex * Math.Sin(phaseSignal));
        }

        public override bool IsTimeDependent => true;
        public override bool HasDependency => false;
    }
}