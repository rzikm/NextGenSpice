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

        public ICircuitModel GetLargeSignalModel()
        {
            return new ElectricCircuitModel(Nodes.ToArray(), Elements.Select(e => e.GetLargeSignalModel()));
        }

        public ICircuitModel GetSmallSignalModel()
        {
            return new ElectricCircuitModel(Nodes.ToArray(), Elements.Select(e => e.GetSmallSignalModel()));
        }
    }
}