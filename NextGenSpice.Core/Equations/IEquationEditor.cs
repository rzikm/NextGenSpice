namespace NextGenSpice.Equations
{
    public interface IEquationEditor
    {
        int VariablesCount { get; }
        void AddMatrixEntry(int row, int column, double value);
        void AddRightHandSideEntry(int index, double value);
    }
}