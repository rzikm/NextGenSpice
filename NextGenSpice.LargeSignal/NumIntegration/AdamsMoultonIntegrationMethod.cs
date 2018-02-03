using System;
using System.Linq;
using NextGenSpice.Core.Equations;
using Numerics;

namespace NextGenSpice.LargeSignal.NumIntegration
{
    /// <summary>
    ///     Class performing Adams-Moulton integration method of given order.
    /// </summary>
    public class AdamsMoultonIntegrationMethod : IIntegrationMethod
    {
        private readonly double[] coefficients;
        private readonly double derivativeCoeff;
        private readonly double[] states;

        private int baseIndex;
        private double derivative;

        private int stateCount;

        public AdamsMoultonIntegrationMethod(int order)
        {
            var coef = GetCoefficients(order);
            coefficients = coef.Skip(1).ToArray();
            derivativeCoeff = coef[0];

            states = new double[order - 1];
        }

        /// <summary>
        ///     Adds state and derivative of current timepoint to history.
        /// </summary>
        /// <param name="state">Value of current state variable</param>
        /// <param name="derivative">Derivative of current state variable</param>
        public void SetState(double state, double derivative)
        {
            this.derivative = derivative;
            baseIndex = (baseIndex - 1 + states.Length) % states.Length;
            states[baseIndex] = state;
            stateCount++;
        }


        /// <summary>
        ///     Gets next values of state and derivative based on history and current timepoint.
        /// </summary>
        /// <param name="dx">How far to predict values of state and derivative.</param>
        /// <returns></returns>
        public (double state, double derivative) GetEquivalents(double dx)
        {
            if (dx <= 0) throw new ArgumentOutOfRangeException(nameof(dx));

            if (stateCount < states.Length)
            {
                var rec = new AdamsMoultonIntegrationMethod(stateCount + 1);
                for (var i = 0; i < stateCount; i++)
                    rec.SetState(states[states.Length - 1 - i], derivative);
                return rec.GetEquivalents(dx);
            }

            var dy = dx / derivativeCoeff;
            var y = derivative * dx;
            for (var i = 0; i < states.Length; i++)
                y += coefficients[i] * states[(baseIndex + i) % states.Length];
            y /= derivativeCoeff;

            return (y, dy);
        }

        /// <summary>
        ///     Gets coefficients for the Adams-Moulton integration of given order.
        /// </summary>
        /// <param name="order">Order of the integration method</param>
        /// <returns></returns>
        public static double[] GetCoefficients(int order)
        {
            if (order <= 0) throw new ArgumentOutOfRangeException(nameof(order));

            // see http://qucs.sourceforge.net/tech/node24.html#eq:MoultonInt for details

            var es = new EquationSystem(new Matrix<double>(order), new double[order]);
            es.AddMatrixEntry(0, 0, 1);
            es.AddRightHandSideEntry(0, 1);
            for (var i = 1; i < order; i++)
            {
                var parity = i % 2 > 0 ? -1 : 1;

                es.AddMatrixEntry(0, i, 1);
                es.AddMatrixEntry(i, 0, parity);
                es.AddRightHandSideEntry(i, parity / (i + 1.0));

                var b = i - 1;
                for (var row = 1; row < order; row++)
                {
                    es.AddMatrixEntry(row, i, b);
                    b *= i - 1;
                }
            }

            return es.Solve();
        }
    }
}