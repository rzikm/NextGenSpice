using System.Collections.Generic;

namespace NextGenSpice.LargeSignal.NumIntegration
{
    public interface IIntegrationMethod
    {
        void SetState(double state, double derivative);
        (double, double) GetEquivalents(double timeDerivative);
    }
}