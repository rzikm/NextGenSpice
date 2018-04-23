namespace NextGenSpice.Numerics.Equations
{
    /// <summary>Defines method for modifying a coefficient in the equation system representation.</summary>
    public interface IEquationSystemCoefficientProxy
    {
        /// <summary>Adds a specified value to the target coefficient.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        void Add(double value);
    }
}