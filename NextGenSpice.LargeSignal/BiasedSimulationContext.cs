using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.LargeSignal
{
    internal class BiasedSimulationContext : ISimulationContext
    {
        private readonly int[] nodeMap;

        public BiasedSimulationContext(int[] nodeMap)
        {
            this.nodeMap = nodeMap;
        }

        public ISimulationContext TrueContext { get; set; }
        public double NodeCount => TrueContext.NodeCount;

        public double Time => TrueContext.Time;

        public double TimeStep => TrueContext.TimeStep;

        public double GetSolutionForVariable(int index)
        {
            return TrueContext.GetSolutionForVariable(GetMappedIndex(index));
        }

        public CircuitParameters CircuitParameters => TrueContext.CircuitParameters;

        private int GetMappedIndex(int i)
        {
            return i < nodeMap.Length ? nodeMap[i] : i;
        }
    }
}