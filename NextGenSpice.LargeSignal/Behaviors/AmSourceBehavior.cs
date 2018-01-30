using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.LargeSignal.Behaviors
{
    internal class AmSourceBehavior : InputSourceBehavior<AmBehaviorParams>
    {
        public AmSourceBehavior(AmBehaviorParams param) : base(param)
        {
        }

        public override double GetValue(ISimulationContext context)
        {
            var time = context.Time - param.Delay;
            var c = 2 * Math.PI * time;

            var phaseCarrier = c * param.FrequencyCarrier;
            var phaseModulation = c * param.FrequencyModulation;

            //            return param.SignalAmplitude * (param.Offset + Math.Sin(phaseModulation)) * Math.Sin(phaseCarrier);

            var m = param.ModulationIndex;

            return param.SignalAmplitude * Math.Sin(phaseCarrier) * (1 + m * Math.Cos(phaseModulation));

        }

        public override bool IsTimeDependent => true;
        public override bool HasDependency => false;
    }
}