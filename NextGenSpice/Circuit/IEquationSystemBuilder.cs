namespace NextGenSpice.Circuit
{
    public interface IEquationSystemBuilder : IEquationEditor
    {
        int AddVariable();
//        int VariablesCount { get; }
//        void AddMatrixEntry(int row, int column, double value);
//        void AddRightHandSideEntry(int index, double value);

        IEquationSystem Build();
    }
}