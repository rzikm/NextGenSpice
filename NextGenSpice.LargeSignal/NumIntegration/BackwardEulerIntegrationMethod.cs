namespace NextGenSpice.LargeSignal.NumIntegration
{
    public class BackwardEulerIntegrationMethod : IIntegrationMethod
    {
        private double derivative;

        public void SetState(double state, double derivative)
        {
            this.derivative = derivative;
        }

        public (double, double) GetEquivalents(double timeDerivative)
        {
            var geq = timeDerivative;
            var ieq = timeDerivative * derivative;

            return (geq, ieq);
        }
    }
}