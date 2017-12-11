using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;
using Numerics;

namespace NextGenSpice.LargeSignal.Behaviors
{
    internal class PieceWiseLinearSourceBehavior : InputSourceBehavior<PieceWiseLinearBehaviorParams>
    {
        private readonly List<double> timepoints;
        private readonly List<double> values;
        public PieceWiseLinearSourceBehavior(PieceWiseLinearBehaviorParams param) : base(param)
        {
            timepoints = param.DefinitionPoints.Keys.ToList();
            values = timepoints.Select(i => param.DefinitionPoints[i]).ToList();
        }

        public override double GetValue(ISimulationContext context)
        {
            var time = context.Time;
            if (param.RepeatStart.HasValue)
            {
                var rs = param.RepeatStart.Value;
                var period = timepoints[timepoints.Count - 1] - rs;

                time = (time - rs) % period + rs;
            }

            var i = timepoints.BinarySearch(time);
            if (i < 0) i = ~i;

            if (i >= timepoints.Count) return values[timepoints.Count - 1];
            if (i == 0) return MathHelper.LinearInterpolation(param.InitialValue, values[i], time / timepoints[i]);
            return MathHelper.LinearInterpolation(values[i - 1], values[i],
                (time - timepoints[i - 1]) / (timepoints[i] - timepoints[i - 1]));
        }

        public override bool IsTimeDependent => true;
        public override bool HasDependency => false;
    }
}