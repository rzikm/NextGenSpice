using System.Linq;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal
{
    internal class BiasedEquationEditor : IEquationSystem, IEquationSystemBuilder
    {
        private readonly int[] nodeMap;

        public BiasedEquationEditor(int[] nodeMap)
        {
            this.nodeMap = nodeMap;
        }

        public IEquationEditor TrueEquationEditor { get; set; }

        public int VariablesCount { get; }

        public void AddMatrixEntry(int row, int column, double value)
        {
            TrueEquationEditor.AddMatrixEntry(GetMappedIndex(row), GetMappedIndex(column), value);
        }

        public void AddRightHandSideEntry(int index, double value)
        {
            TrueEquationEditor.AddRightHandSideEntry(GetMappedIndex(index), value);
        }

        public void BindEquivalent(params int[] vars)
        {
            (TrueEquationEditor as IEquationSystem).BindEquivalent(vars.Select(GetMappedIndex).ToArray());
        }

        public int AddVariable()
        {
            return (TrueEquationEditor as IEquationSystemBuilder).AddVariable();
        }

        private int GetMappedIndex(int i)
        {
            return i < nodeMap.Length ? nodeMap[i] : i;
        }
    }
}