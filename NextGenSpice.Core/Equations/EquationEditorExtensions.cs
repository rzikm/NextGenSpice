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

        public static IEquationEditor AddVoltage(this IEquationEditor editor, int anode, int kathode, int binder,
            double value)
        {
            editor.AddMatrixEntry(binder, anode, 1);
            editor.AddMatrixEntry(binder, kathode, -1);
            editor.AddMatrixEntry(anode, binder, 1);
            editor.AddMatrixEntry(kathode, binder, -1);

            editor.AddRightHandSideEntry(binder, value);

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
