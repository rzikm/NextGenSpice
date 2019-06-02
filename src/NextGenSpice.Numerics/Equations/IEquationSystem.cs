namespace NextGenSpice.Numerics.Equations
{
	/// <summary>Defines necessary interface for equation system implementation.</summary>
	public interface IEquationSystem
	{
		/// <summary>Count of the variables in the equation.</summary>
		int VariablesCount { get; }

		/// <summary>Returns solution for the given variable.</summary>
		/// <param name="variable">Index of the variable in the equation system.</param>
		/// <returns></returns>
		double GetSolution(int variable);

		/// <summary>Solves the linear equation system. If the system has no solution, the result is undefined.</summary>
		void Solve();
	}
}