namespace NextGenSpice.Core.Equations
{
    public interface IEquationSystemBuilder : IEquationEditor
    {
        int AddVariable();
        IEquationSystem Build();
    }
}