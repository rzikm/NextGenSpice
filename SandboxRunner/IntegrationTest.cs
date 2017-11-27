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
            
            double x = 1;
            double t = 0;
            double h = 0.01;
            // forward euler - explicit method, no inner loop
            Console.WriteLine("Trapesoidal rule:");
            while (t < 0.2)
            {
                // iterate until fn = f + xn * h

                // initial guess (predictor)
                var xn = x;

                double oldxn;
                do
                {
                    // corrector step
                    oldxn = xn;
                    xn = x + h / 2 * (F(t+h, xn) + F(t, x));
                } while (Math.Abs(xn - oldxn) > 1e-10);

                t += h;
                x = xn;
            }

            Console.WriteLine(x);
        }
    }
}