using System.Linq;
using NextGenSpice.Core.Equations;
using Numerics;

namespace NextGenSpice.LargeSignal.NumIntegration
{
    public class AdamsMoultonIntegrationMethod : IIntegrationMethod
    {
        private readonly double[] coefficients;
        private readonly double derivativeCoeff;
        private readonly double[] states;
        private double derivative;

        private int stateCount;

        private int baseIndex;
        public AdamsMoultonIntegrationMethod(int order)
        {
            var coef = GetCoefficients(order);
            coefficients = coef.Skip(1).ToArray();
            derivativeCoeff = coef[0];

            states = new double[order - 1];
        }

        public void SetState(double state, double derivative)
        {
            this.derivative = derivative;
            baseIndex = (baseIndex - 1 + states.Length) % states.Length;
            states[baseIndex] = state;
            stateCount++;
        }

        public (double, double) GetEquivalents(double timeDerivative)
        {
            if (stateCount < states.Length)
            {
                var rec = new AdamsMoultonIntegrationMethod(stateCount + 1);
                for (int i = 0; i < stateCount; i++)
                {
                    rec.SetState(states[states.Length - 1 - i], derivative);
                }
                return rec.GetEquivalents(timeDerivative);
            }

            var geq = timeDerivative / derivativeCoeff;
            var state = derivative * timeDerivative;
            for (int i = 0; i < states.Length; i++)
            {
                state += coefficients[i] * states[(baseIndex + i) % states.Length];
            }
            state /= derivativeCoeff;

            return (geq, state);
        }

        public static double[] GetCoefficients(int order)
        {
            // see http://qucs.sourceforge.net/tech/node24.html#eq:MoultonInt for details
            EquationSystem es = new EquationSystem(new Array2DWrapper<double>(order), new double[order]);
            es.AddMatrixEntry(0, 0, 1);
            es.AddRightHandSideEntry(0, 1);
            for (int i = 1; i < order; i++)
            {
                var parity = i % 2 > 0 ? -1 : 1;

                es.AddMatrixEntry(0, i, 1);
                es.AddMatrixEntry(i, 0, parity);
                es.AddRightHandSideEntry(i, parity / (i + 1.0));

                var b = i - 1;
                for (int row = 1; row < order; row++)
                {
                    es.AddMatrixEntry(row, i, b);
                    b *= (i - 1);
                }
            }

            return es.Solve();
        }
    }
}