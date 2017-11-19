﻿using NextGenSpice.Core.Elements;

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

        public double Time
        {
            get => TrueContext.Time;
            set => TrueContext.Time = value;
        }

        public double Timestep
        {
            get => TrueContext.Timestep;
            set => TrueContext.Timestep = value;
        }

        public double GetSolutionForVariable(int index)
        {
            return TrueContext.GetSolutionForVariable(GetMappedIndex(index));
        }

        private int GetMappedIndex(int i)
        {
            return i < nodeMap.Length ? nodeMap[i] : i;
        }
    }
}