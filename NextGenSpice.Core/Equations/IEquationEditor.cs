using System.Collections.Generic;

namespace NextGenSpice.Core.Equations
{
    public interface IEquationEditor
    {
        int VariablesCount { get; }
        void AddMatrixEntry(int row, int column, double value);
        void AddRightHandSideEntry(int index, double value);
    }
}