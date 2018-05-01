using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal.Devices;
using NextGenSpice.Numerics;

namespace NextGenSpice.LargeSignal.Behaviors
{
    /// <summary>
    ///     Strategy class for piece-wise linear behavior of <see cref="LargeSignalVoltageSource" /> and
    ///     <see cref="LargeSignalCurrentSource" />.
    /// </summary>
    internal class PieceWiseLinearSourceBehavior : InputSourceBehavior<PieceWiseLinearBehaviorParams>
    {
        private readonly List<double> timepoints;
        private readonly List<double> values;

        public PieceWiseLinearSourceBehavior(PieceWiseLinearBehaviorParams parameters) : base(parameters)
        {
            timepoints = parameters.DefinitionPoints.Keys.ToList();
            values = timepoints.Select(i => parameters.DefinitionPoints[i]).ToList();
        }

        /// <summary>Gets input source value for given timepoint.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override double GetValue(ISimulationContext context)
        {
            var time = context.TimePoint;
            if (Parameters.RepeatStart.HasValue) // is periodic?
            {
                var rs = Parameters.RepeatStart.Value;
                var period = timepoints[timepoints.Count - 1] - rs;

                time = (time - rs) % period + rs;
            }

            var i = timepoints.BinarySearch(time);
            if (i < 0)
                i = ~i; // if not found, returned value of BinarySearch is bitwise negation of index where to insert the value (minimal greater device or one-after-last index)

            if (i >= timepoints.Count) return values[timepoints.Count - 1];
            if (i == 0)
            {
                if (Math.Abs(timepoints[0]) < double.Epsilon) return Parameters.InitialValue;
                return MathHelper.LinearInterpolation(Parameters.InitialValue, values[i], time / timepoints[i]);
            }

            return MathHelper.LinearInterpolation(values[i - 1], values[i],
                (time - timepoints[i - 1]) / (timepoints[i] - timepoints[i - 1]));
        }
    }
}