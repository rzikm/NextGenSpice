namespace NextGenSpice.LargeSignal.NumIntegration
{
    public class TrapezoidalIntegrationMethod : IIntegrationMethod
    {

        private double derivative;
        private double state;

        public void SetState(double state, double derivative)
        {
            this.derivative = derivative;
            this.state = state;
        }

        public (double, double) GetEquivalents(double timeDerivative)
        {
            var geq = 2 * timeDerivative;
            var ieq = geq * derivative + state;

            return (geq, ieq);
        }
    }
}