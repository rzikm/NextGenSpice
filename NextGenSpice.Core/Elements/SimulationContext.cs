using NextGenSpice.Core.Equations;

namespace NextGenSpice.Core.Elements
{
    public class SimulationContext : ISimulationContext
    {
        private readonly EquationSystem equationSystem;

        public SimulationContext(int nodeCount, EquationSystem equationSystem)
        {
            NodeCount = nodeCount;
            this.equationSystem = equationSystem;
        }

        public double NodeCount { get; }
        public double Time { get; set; }
        public double Timestep { get; set; }
        public double[] EquationSolution => equationSystem.Solution;

        public double GetSolutionForVariable(int index)
        {
            return equationSystem.Solution[index];
        }
    }
}