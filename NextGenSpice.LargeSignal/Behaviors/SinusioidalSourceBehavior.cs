﻿using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.LargeSignal.Behaviors
{
    internal class SinusioidalSourceBehavior : InputSourceBehavior<SinusoidalBehaviorParams>
    {
        public SinusioidalSourceBehavior(SinusoidalBehaviorParams param) : base(param)
        {
        }

        public override double GetValue(ISimulationContext context)
        {
            var phase = param.Phase;
            var amplitude = param.Amplitude;
            var elapsedTime = context.Time - param.Delay;

            if (elapsedTime > 0)
            {
                phase += elapsedTime * param.Frequency;
                amplitude *= Math.Exp(-elapsedTime * param.DampingFactor);
            }

            return param.BaseValue + Math.Sin(phase) * amplitude;
        }

        public override bool IsTimeDependent => true;
        public override bool HasDependency => false;
    }
}