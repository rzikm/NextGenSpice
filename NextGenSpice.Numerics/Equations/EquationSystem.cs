namespace NextGenSpice.Numerics.Equations
{
    /// <summary>Simple equation system with double precision coefficient.</summary>
    public class EquationSystem : IEquationSystem
    {
        public EquationSystem(int size)
        {
            // init backup space
            Matrix = new Matrix<double>(size);
            Solution = new double[size];
            RightHandSide = new double[size];
        }

        /// <summary>Result of the latest call to the Solve() method.</summary>
        public double[] Solution { get; }

        /// <summary>Matrix part of the equation system.</summary>
        public Matrix<double> Matrix { get; }

        /// <summary>Right hand side vector of the equation system.</summary>
        public double[] RightHandSide { get; }

        /// <summary>Count of the variables in the equation.</summary>
        public int VariablesCount => Solution.Length;

        /// <summary>Returns solution for the given variable.</summary>
        /// <param name="variable">Index of the variable in the equation system.</param>
        /// <returns></returns>
        public double GetSolution(int variable)
        {
            return Solution[variable];
        }

        /// <summary>Solves the linear equation system. If the system has no solution, the result is undefined.</summary>
        public void Solve()
        {
            GaussJordanElimination.Solve(Matrix, RightHandSide, Solution);
        }
    }

#if dd_precision
    /// <summary>Simple equation system with dd_real precision coefficient.</summary>
    public class DdEquationSystem : IEquationSystem
    {
        public DdEquationSystem(int size)
        {
            // init backup space
            Matrix = new Matrix<dd_real>(size);
            Solution = new dd_real[size];
            RightHandSide = new dd_real[size];
        }

        /// <summary>Result of the latest call to the Solve() method.</summary>
        public dd_real[] Solution { get; }

        /// <summary>Matrix part of the equation system.</summary>
        public Matrix<dd_real> Matrix { get; }

        /// <summary>Right hand side vector of the equation system.</summary>
        public dd_real[] RightHandSide { get; }

        /// <summary>Count of the variables in the equation.</summary>
        public int VariablesCount => Solution.Length;

        /// <summary>Returns solution for the given variable.</summary>
        /// <param name="variable">Index of the variable in the equation system.</param>
        /// <returns></returns>
        public double GetSolution(int variable)
        {
            return Solution[variable].x0;
        }

        /// <summary>Solves the linear equation system. If the system has no solution, the result is undefined.</summary>
        public void Solve()
        {
            GaussJordanElimination.Solve(Matrix, RightHandSide, Solution);
        }
    }
#endif

#if qd_precision
    /// <summary>Simple equation system with qd_real precision coefficient.</summary>
    public class QdEquationSystem : IEquationSystem
    {
        public QdEquationSystem(int size)
        {
            // init backup space
            Matrix = new Matrix<qd_real>(size);
            Solution = new qd_real[size];
            RightHandSide = new qd_real[size];
        }

        /// <summary>Result of the latest call to the Solve() method.</summary>
        public qd_real[] Solution { get; }

        /// <summary>Matrix part of the equation system.</summary>
        public Matrix<qd_real> Matrix { get; }

        /// <summary>Right hand side vector of the equation system.</summary>
        public qd_real[] RightHandSide { get; }

        /// <summary>Count of the variables in the equation.</summary>
        public int VariablesCount => Solution.Length;

        /// <summary>Returns solution for the given variable.</summary>
        /// <param name="variable">Index of the variable in the equation system.</param>
        /// <returns></returns>
        public double GetSolution(int variable)
        {
            return Solution[variable].x0;
        }

        /// <summary>Solves the linear equation system. If the system has no solution, the result is undefined.</summary>
        public void Solve()
        {
            GaussJordanElimination.Solve(Matrix, RightHandSide, Solution);
        }
    }
#endif
}