namespace NextGenSpice.Equations
{
    public interface IEquationSystemBuilder : IEquationEditor
    {
        int AddVariable();
        IEquationSystem Build();
    }
}