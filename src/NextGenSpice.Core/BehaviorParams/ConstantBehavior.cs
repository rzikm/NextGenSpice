namespace NextGenSpice.Core.BehaviorParams
{
	/// <summary>Specifies behavior parameters for input source with constant output.</summary>
	public class ConstantBehavior : InputSourceBehavior
	{
		/// <summary>Value of the source in volts or ampers.</summary>
		public double Value { get; set; }

		/// <summary>Gets input source value for given timepoint.</summary>
		/// <param name="timepoint">The time value for which to calculate the value.</param>
		/// <returns></returns>
		public override double GetValue(double timepoint)
		{
			return Value;
		}
	}
}