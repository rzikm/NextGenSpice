namespace NextGenSpice.LargeSignal.NumIntegration
{
	/// <summary>Class implementing basic backward euler integration method.</summary>
	public class BackwardEulerIntegrationMethod : IIntegrationMethod
	{
		private double derivative;

		/// <summary>Adds state and derivative of current timepoint to history.</summary>
		/// <param name="state">Value of current state variable</param>
		/// <param name="derivative">Derivative of current state variable</param>
		public void SetState(double state, double derivative)
		{
			this.derivative = derivative;
		}

		/// <summary>Gets next values of state and derivative based on history and current timepoint.</summary>
		/// <param name="dx">How far to predict values of state and derivative.</param>
		/// <returns></returns>
		public (double state, double derivative) GetEquivalents(double dx)
		{
			var dy = dx;
			var y = dx * derivative;

			return (y, dy);
		}
	}
}