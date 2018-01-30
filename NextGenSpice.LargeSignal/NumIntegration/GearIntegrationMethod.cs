using System.Linq;
using NextGenSpice.Core.Equations;
using Numerics;

namespace NextGenSpice.LargeSignal.NumIntegration
{
    public class GearIntegrationMethod : IIntegrationMethod
    {
        private readonly double[] coefficients;
        private readonly double normalizingCoeff;
        private readonly double[] derivatives;

        private int stateCount;
        private int baseIndex;

        public GearIntegrationMethod(int order)
        {
            var coef = GetCoefficients(order);
            coefficients = coef.Skip(1).ToArray();
            normalizingCoeff = coef[0];

            derivatives = new double[order];
        }

        public void SetState(double state, double derivative)
        {
            baseIndex = (baseIndex - 1 + derivatives.Length) % derivatives.Length;
            derivatives[baseIndex] = derivative;
            stateCount++;
        }

        public (double, double) GetEquivalents(double timeDerivative)
        {
            if (stateCount < derivatives.Length)
            {
                var rec = new GearIntegrationMethod(stateCount);
                for (int i = 0; i < stateCount; i++)
                {
                    rec.SetState(0,derivatives[derivatives.Length - 1 - i]);
                }
                return rec.GetEquivalents(timeDerivative);
            }

            var geq = timeDerivative / normalizingCoeff;
            var state = 0.0;
            for (int i = 0; i < derivatives.Length; i++)
            {
                state += coefficients[i] * timeDerivative * derivatives[(baseIndex + i) % derivatives.Length];
            }
            state /= normalizingCoeff;

            return (geq, state);
        }

        public static double[] GetCoefficients(int order)
        {
            // see http://qucs.sourceforge.net/tech/node24.html#eq:MoultonInt for details
            EquationSystem es = new EquationSystem(new Array2DWrapper<double>(order + 1), new double[order + 1]);
            es.AddRightHandSideEntry(0, 1);
            for (int i = 1; i < es.VariablesCount; i++)
            {
                es.AddRightHandSideEntry(i, 1);
                es.AddMatrixEntry(0, i, 1);
                es.AddMatrixEntry(i, 0, i);


                var b = -(i - 1);
                for (int row = 1; row < es.VariablesCount; row++)
                {
                    es.AddMatrixEntry(row, i, b);
                    b *= -(i - 1);
                }
            }

            return es.Solve();
        }
    }
}