using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Numerics;

namespace NextGenSpice.Core.BehaviorParams
{
	public class PieceWiseLinearBehavior : InputSourceBehavior
	{
		private IReadOnlyDictionary<double, double> definitionPoints = new Dictionary<double, double>();
		private List<double> timepoints;
		private List<double> values;

		public IReadOnlyDictionary<double, double> DefinitionPoints
		{
			get => definitionPoints;
			set
			{
				definitionPoints = value;
				timepoints = value.Keys.ToList();
				values = timepoints.Select(i => definitionPoints[i]).ToList();
			}
		}

		public double InitialValue { get; set; }
		public double? RepeatStart { get; set; }


		/// <summary>Gets input source value for given timepoint.</summary>
		/// <param name="timepoint">The time value for which to calculate the value.</param>
		/// <returns></returns>
		public override double GetValue(double timepoint)
		{
			var time = timepoint;
			if (RepeatStart.HasValue) // is periodic?
			{
				var rs = RepeatStart.Value;
				var period = timepoints[timepoints.Count - 1] - rs;

				time = (time - rs) % period + rs;
			}

			var i = timepoints.BinarySearch(time);
			if (i < 0)
				i = ~i; // if not found, returned value of BinarySearch is bitwise negation of index where to insert the value (minimal greater device or one-after-last index)

			if (i >= timepoints.Count) return values[timepoints.Count - 1];
			if (i == 0)
			{
				if (Math.Abs(timepoints[0]) < double.Epsilon) return InitialValue;
				return MathHelper.LinearInterpolation(InitialValue, values[i], time / timepoints[i]);
			}

			return MathHelper.LinearInterpolation(values[i - 1], values[i],
				(time - timepoints[i - 1]) / (timepoints[i] - timepoints[i - 1]));
		}
	}
}