using NextGenSpice.Core.Equations;

namespace NextGenSpice.Core.Elements
{
    public class SimulationContext : ISimulationContext
    {
        public SimulationContext(int nodeCount)
        {
            NodeCount = nodeCount;
            CircuitParameters = new CircuitParameters();
        }

        public double NodeCount { get; }
        public double Time { get; set; }
        public double Timestep { get; set; }
        public double[] EquationSolution => EquationSystem.Solution;

        public double GetSolutionForVariable(int index)
        {
            return EquationSystem.Solution[index];
        }

        public CircuitParameters CircuitParameters { get; }

        public EquationSystem EquationSystem { get; set; }
    }
}