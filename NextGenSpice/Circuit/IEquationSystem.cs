namespace NextGenSpice.Circuit
{
    public interface IEquationSystem : IEquationEditor
    {
        void Clear();
        double[] Solution { get; }
        double[] Solve();
    }

    public interface IEquationEditor
    {
        int VariablesCount { get; }
        void AddMatrixEntry(int row, int column, double value);
        void AddRightHandSideEntry(int index, double value);
    }
}