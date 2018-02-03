namespace NextGenSpice.LargeSignal.NumIntegration
{
    /// <summary>
    ///     Class implementing implicit trapezoidal integration method.
    /// </summary>
    public class TrapezoidalIntegrationMethod : IIntegrationMethod
    {
        private double derivative;
        private double state;

        /// <summary>
        ///     Adds state and derivative of current timepoint to history.
        /// </summary>
        /// <param name="state">Value of current state variable</param>
        /// <param name="derivative">Derivative of current state variable</param>
        public void SetState(double state, double derivative)
        {
            this.derivative = derivative;
            this.state = state;
        }

        /// <summary>
        ///     Gets next values of state and derivative based on history and current timepoint.
        /// </summary>
        /// <param name="timeStep">How far to predict values of state and derivative.</param>
        /// <returns></returns>
        public (double state, double derivative) GetEquivalents(double timeStep)
        {
            var dx = 2 * timeStep;
            var x = dx * derivative + state;

            return (x, dx);
        }
    }
}