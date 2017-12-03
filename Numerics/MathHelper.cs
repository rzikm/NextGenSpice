namespace Numerics
{
    public static class MathHelper
    {
        public static double LinearInterpolation(double val1, double val2, double x)
        {
            return val1 + (val2 - val1) * x;
        }
    }
}