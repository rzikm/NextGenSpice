namespace NextGenSpice.Numerics.Equations
{
    /// <summary>Defines method for reading the value of variable from equation system solution.</summary>
    public interface IEquationSystemSolutionProxy
    {
        /// <summary>Gets the value of the target solution variable.</summary>
        /// <returns></returns>
        double GetValue();
    }
}