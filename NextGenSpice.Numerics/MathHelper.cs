namespace NextGenSpice.Numerics
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


        /// <summary>
        /// Returns whether the two values are in the tollerance range of each other
        /// </summary>
        /// <param name="v1">First value.</param>
        /// <param name="v2">Second value.</param>
        /// <param name="abstol">Absolute tolerance.</param>
        /// <param name="reltol">Relative tolerance.</param>
        /// <returns></returns>
        public static bool InTollerance(double v1, double v2, double abstol, double reltol)
        {
            var tol = reltol * Math.Max(Math.Abs(v1), Math.Abs(v2)) + abstol;
            return (Math.Abs(v1 - v2) < tol);
        }
    }
}