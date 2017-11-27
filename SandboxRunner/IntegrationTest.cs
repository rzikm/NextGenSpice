using System;

namespace SandboxRunner
{
    public class IntegrationTest
    {
        public static double F(double t, double x)
        {
            return 2* x - 3 * t;
        }
   
        public static void Run()
        {
            // see wolfram alpha:  " y' = 2y - 3t, y(0)=1, solve for y(0.2) "
            // should print out something like 1.42296
            Console.WriteLine(Integrate(0, 0.2, 1, 0.01, (t, y) => 2 * y - 3 * t));
            Console.WriteLine(Integrate_LTE_timestep_control(0, 0.2, 1, 0.01, 1e-15, 1 + 1e-15, (t, y) => 2 * y - 3 * t));
            Console.WriteLine($"Should be: {1/4.0 * (6 * 0.2 + Math.Exp(2 * 0.2) + 3)}");
            Console.WriteLine();

            // exponential: sollution is y(t) = y0 * exp(ty)
            Console.WriteLine(Integrate(0, 0.2, 1, 0.01, (t, y) => y));
            Console.WriteLine(Integrate_LTE_timestep_control(0, 0.2, 1, 0.01, 1e-8, 1+1e-6, (t, y) => y));
            Console.WriteLine($"Should be: {Math.Exp(0.2)}");
        }

        private static double Integrate(double initPoint, double stopPoint, double initValue, double stepsize, Func<double, double, double> func)
        {
            var iterOuter = 0;
            var iterInner = 0;
            while (initPoint < stopPoint)
            {
                ++iterOuter;
                // iterate until fn = f + xn * stepsize

                // initial guess (predictor)
                var xn = initValue;

                double oldxn;
                do
                {
                    ++iterInner;
                    // corrector step
                    oldxn = xn;
                    xn = initValue + stepsize / 2 * (func(initPoint + stepsize, xn) + func(initPoint, initValue));
                } while (Math.Abs(xn - oldxn) > 1e-10);

                initPoint += stepsize;
                initValue = xn;
            }

            Console.WriteLine($"OuterIter: {iterOuter}, InnerIter: {iterInner}");
            return initValue;
        }

        private static double Integrate_LTE_timestep_control(double initPoint, double stopPoint, double initValue, double stepsize, double maxAbsTruncErr, double maxRelTruncErr, Func<double, double, double> func)
        {

            var iterOuter = 0;
            var iterInner = 0;

            double dx = 0;

            var x = initValue;
            var t = initPoint;
            var h = stepsize;
            var hn = h;

            while (t < stopPoint)
            {
                ++iterOuter;

                // initial guess (predictor)
                var xn = x;

                double oldxn;
                do
                {
                    ++iterInner;

                    // corrector step
                    oldxn = xn;
                    xn = x + hn / 2 * (func(t + hn, xn) + func(t, x));
                } while (Math.Abs(xn - oldxn) > 1e-15);

                var dxn = (xn - x) / hn;
                var ddx = (dxn - dx) / (h + hn); // second derivative of x

                var lte = 2*ddx;
                var q = lte/(Math.Abs(xn) * maxRelTruncErr + maxAbsTruncErr);

                bool converged = (x == initValue) || 1 > q;
                if (converged)
                {
                    t += hn;
                    x = xn;
                    dx = dxn;
                    hn = h;
                }
                else
                {
                    hn *= Math.Pow(0.8/q, 1/3.0);
                }
            }

            Console.WriteLine($"OuterIter: {iterOuter}, InnerIter: {iterInner}");
            return x;
        }
    }
}