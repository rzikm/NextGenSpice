namespace NextGenSpice.Core.NumIntegration
{
    /// <summary>
    ///     Defines basic interface for numeric integration methods to be used in model classes.
    /// </summary>
    public interface IIntegrationMethod
    {
        /// <summary>
        ///     Adds state and derivative of current timepoint to history.
        /// </summary>
        /// <param name="state">Value of current state variable</param>
        /// <param name="derivative">Derivative of current state variable</param>
        void SetState(double state, double derivative);

        /// <summary>
        ///     Gets next values of state and derivative based on history and current timepoint.
        /// </summary>
        /// <param name="dx">How far to predict values of state and derivative.</param>
        /// <returns></returns>
        (double state, double derivative) GetEquivalents(double dx);
    }
}