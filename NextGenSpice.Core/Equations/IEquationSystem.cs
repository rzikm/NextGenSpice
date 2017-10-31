namespace NextGenSpice.Core.Equations
{
    public interface IEquationSystem : IEquationEditor
    {
//        void Clear();
//        double[] Solution { get; }
//        double[] Solve();

        void BindEquivalent(params int[] vars);
    }
}