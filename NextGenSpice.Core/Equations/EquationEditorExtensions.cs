namespace NextGenSpice.Core.Equations
{
    /// <summary>
    ///     Defines extension methods for the IEquationEditor.
    /// </summary>
    public static class EquationEditorExtensions
    {
        /// <summary>
        ///     Adds values to the linear equation system corresponding to a conductance between specified nodes.
        /// </summary>
        /// <param name="editor">The equation editor.</param>
        /// <param name="anode">Id of the positive node of the device.</param>
        /// <param name="cathode">Id of the negative node of the device.</param>
        /// <param name="value">The added conductance between the given nodes.</param>
        /// <returns></returns>
        public static IEquationEditor AddConductance(this IEquationEditor editor, int anode, int cathode, double value)
        {
            editor.AddMatrixEntry(cathode, anode, -value);
            editor.AddMatrixEntry(anode, cathode, -value);
            editor.AddMatrixEntry(anode, anode, value);
            editor.AddMatrixEntry(cathode, cathode, value);

            return editor;
        }

        /// <summary>
        ///     Adds values to the linear equation system corresponding to an ideal voltage source between specified nodes.
        /// </summary>
        /// <param name="editor">The equation editor.</param>
        /// <param name="anode">Id of the positive node of the device.</param>
        /// <param name="cathode">Id of the negative node of the device.</param>
        /// <param name="branchVariable">Id of the variable representing the coresponding branch current.</param>
        /// <param name="value">The added voltage between the given nodes.</param>
        /// <returns></returns>
        public static IEquationEditor AddVoltage(this IEquationEditor editor, int anode, int cathode,
            int branchVariable,
            double value)
        {
            editor.AddMatrixEntry(branchVariable, anode, 1);
            editor.AddMatrixEntry(branchVariable, cathode, -1);
            editor.AddMatrixEntry(anode, branchVariable, 1);
            editor.AddMatrixEntry(cathode, branchVariable, -1);

            editor.AddRightHandSideEntry(branchVariable, value);

            return editor;
        }

        /// <summary>
        ///     Adds values to the linear equation system corresponding to an ideal current source between specified nodes.
        /// </summary>
        /// <param name="editor">The equation editor.</param>
        /// <param name="anode">Id of the positive node of the device.</param>
        /// <param name="cathode">Id of the negative node of the device.</param>
        /// <param name="value">The added current between the given nodes.</param>
        public static IEquationEditor AddCurrent(this IEquationEditor editor, int anode, int cathode, double value)
        {
            editor.AddRightHandSideEntry(anode, value);
            editor.AddRightHandSideEntry(cathode, -value);

            return editor;
        }
    }
}