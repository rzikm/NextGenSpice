namespace NextGenSpice.Core.Equations
{
    public static class EquationEditorExtensions
    {
        public static IEquationEditor AddConductance(this IEquationEditor editor, int anode, int kathode, double value)
        {
            editor.AddMatrixEntry(kathode, anode, -value);
            editor.AddMatrixEntry(anode, kathode, -value);
            editor.AddMatrixEntry(anode, anode, value);
            editor.AddMatrixEntry(kathode, kathode, value);

            return editor;
        }

        public static IEquationEditor AddVoltage(this IEquationEditor editor, int anode, int kathode, int branchVariable,
            double value)
        {
            editor.AddMatrixEntry(branchVariable, anode, 1);
            editor.AddMatrixEntry(branchVariable, kathode, -1);
            editor.AddMatrixEntry(anode, branchVariable, 1);
            editor.AddMatrixEntry(kathode, branchVariable, -1);

            editor.AddRightHandSideEntry(branchVariable, value);

            return editor;
        }

        public static IEquationEditor AddCurrent(this IEquationEditor editor, int anode, int kathode, double value)
        {
            editor.AddRightHandSideEntry(anode, value);
            editor.AddRightHandSideEntry(kathode, -value);

            return editor;
        }
    }
}
