namespace NextGenSpice.Core.Equations
{
    /// <summary>Defines methods for adding coeffitients to linear equation system.</summary>
    public interface IEquationEditor
    {
        /// <summary>Count of the variables in the equation.</summary>
        int VariablesCount { get; }

        /// <summary>Adds a value to coefficient on the given row and column of the equation matrix.</summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value to be added to the coefficients.</param>
        void AddMatrixEntry(int row, int column, double value);

        /// <summary>Adds a value to coefficient on the given position of the right hand side of the equation matrix.</summary>
        /// <param name="index">Index of the position.</param>
        /// <param name="value">The value.</param>
        void AddRightHandSideEntry(int index, double value);
    }
}