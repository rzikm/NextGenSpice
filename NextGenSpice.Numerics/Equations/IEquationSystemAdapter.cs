namespace NextGenSpice.Numerics.Equations
{
    /// <summary>Defines methods for creating bindings on the equation system.</summary>
    public interface IEquationSystemAdapter
    {
        /// <summary>Adds a new variable to the equation system and returns the index of the variable;</summary>
        /// <returns></returns>
        int AddVariable();

        /// <summary>Returns proxy class for coefficient at given coordinates in the equation matrix.</summary>
        /// <param name="row">Row coordinate.</param>
        /// <param name="column">Column coordinate.</param>
        /// <returns></returns>
        IEquationSystemCoefficientProxy GetMatrixCoefficientProxy(int row, int column);

        /// <summary>Returns proxy class for coefficient at given row in the right hand side vector.</summary>
        /// <param name="row">Row coordinate.</param>
        /// <returns></returns>
        IEquationSystemCoefficientProxy GetRightHandSideCoefficientProxy(int row);

        /// <summary>Returns proxy class for the i-th variable of the solution.</summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IEquationSystemSolutionProxy GetSolutionProxy(int index);
    }
}