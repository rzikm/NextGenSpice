using System.Diagnostics.Contracts;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.Core.Elements
{
    public class SimulationContext
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

        public double GetNodeVoltage(int id)
        {
            Contract.Assert(id >= 0 && id < NodeCount);

            return EquationSolution[id];
        }

        public double GetCurrent(int sourceId, int targetId)
        {
            return (GetNodeVoltage(sourceId) - GetNodeVoltage(targetId)) * GetConductance(sourceId, targetId);
        }

        public double GetConductance(int sourceId, int targetId)
        {
            Contract.Assert(sourceId >= 0 && sourceId < NodeCount);
            Contract.Assert(targetId >= 0 && targetId < NodeCount);

            return equationSystem.GetMatrixEntry(sourceId, targetId);
        }

    }
}