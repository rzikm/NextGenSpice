using System;
using System.Linq;
using NextGenSpice.Numerics;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.NumIntegration
{
    /// <summary>Class implementing the Gear integration method of given order.</summary>
    public class GearIntegrationMethod : IIntegrationMethod
    {
        private readonly double[] coefficients;
        private readonly double[] derivatives;
        private readonly double normalizingCoeff;
        private int baseIndex;

        private int stateCount;

        public GearIntegrationMethod(int order)
        {
            var coef = GetCoefficients(order);
            coefficients = coef.Skip(1).ToArray();
            normalizingCoeff = coef[0];

            derivatives = new double[order];
        }

        /// <summary>Adds state and derivative of current timepoint to history.</summary>
        /// <param name="state">Value of current state variable</param>
        /// <param name="derivative">Derivative of current state variable</param>
        public void SetState(double state, double derivative)
        {
            baseIndex = (baseIndex - 1 + derivatives.Length) % derivatives.Length;
            derivatives[baseIndex] = derivative;
            stateCount++;
        }

        /// <summary>Gets next values of state and derivative based on history and current timepoint.</summary>
        /// <param name="dx">How far to predict values of state and derivative.</param>
        /// <returns></returns>
        public (double state, double derivative) GetEquivalents(double dx)
        {
            if (stateCount < derivatives.Length)
            {
                var rec = new GearIntegrationMethod(stateCount);
                for (var i = 0; i < stateCount; i++)
                    rec.SetState(0, derivatives[derivatives.Length - 1 - i]);
                return rec.GetEquivalents(dx);
            }

            var dy = dx / normalizingCoeff;
            var y = 0.0;
            for (var i = 0; i < derivatives.Length; i++)
                y += coefficients[i] * dx * derivatives[(baseIndex + i) % derivatives.Length];
            y /= normalizingCoeff;

            return (y, dy);
        }

        /// <summary>Gets coeffitients for Gear integration method of given order.</summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static double[] GetCoefficients(int order)
        {
            if (order < 0) throw new ArgumentOutOfRangeException(nameof(order));

            // see http://qucs.sourceforge.net/tech/node24.html#SECTION00713100000000000000 for details
            var es = new EquationSystem(order + 1);
            es.RightHandSide[0] = 1;
            for (var i = 1; i < es.VariablesCount; i++)
            {
                es.RightHandSide[i] = 1;
                es.Matrix[0, i] = 1;
                es.Matrix[i, 0] = i;


                var b = -(i - 1);
                for (var row = 1; row < es.VariablesCount; row++)
                {
                    es.Matrix[row, i] = b;
                    b *= -(i - 1);
                }
            }

            es.Solve();
            return es.Solution;
        }
    }
}