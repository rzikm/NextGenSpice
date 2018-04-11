namespace Numerics
{
    /// <summary>Defines helper methods for mathematic operations.</summary>
    public static class MathHelper
    {
        /// <summary>Finds  value that is proportionally between given values</summary>
        /// <param name="val1">First value.</param>
        /// <param name="val2">Second value.</param>
        /// <param name="x">Interpolation parameter from range [0,1]</param>
        /// <returns></returns>
        public static double LinearInterpolation(double val1, double val2, double x)
        {
            return val1 + (val2 - val1) * x;
        }
    }
}