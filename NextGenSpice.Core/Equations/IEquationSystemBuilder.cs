namespace NextGenSpice.Core.Equations
{
    /// <summary>
    /// Defines methods for building a linear equation system.
    /// </summary>
    public interface IEquationSystemBuilder : IEquationEditor
    {
        /// <summary>
        /// Adds a variable to the equation system. Returns the index of the variable.
        /// </summary>
        /// <returns></returns>
        int AddVariable();
    }
}