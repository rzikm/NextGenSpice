using System.Collections.Generic;

namespace NextGenSpice.Circuit
{
    public interface IEquationSystemBuilder : IEquationEditor
    {
        int AddVariable();
        IEquationSystem Build();
    }
}