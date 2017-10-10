using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Elements;

namespace NextGenSpice.Circuit
{
    public class ElectricCircuitDefinition : ICircuitDefinition
    {
        public List<CircuitNode> Nodes { get; internal set; }
        public List<ICircuitDefinitionElement> Elements { get; internal set; }

        public ICircuitModel GetDcOperatingPointAnalysisModel()
        {
            return new ElectricCircuitModel(Nodes.ToArray(), Elements.Select(e => e.GetDcOperatingPointModel()));
        }

        public ICircuitModel GetTransientAnalysisModel()
        {
            return new ElectricCircuitModel(Nodes.ToArray(), Elements.Select(e => e.GetTransientModel()));
        }
    }
}