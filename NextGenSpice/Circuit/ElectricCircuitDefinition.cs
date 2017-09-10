using System;
using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice.Circuit
{


    public interface ICircuitDefinition
    {
        ICircuitModel GetDcOperatingPointAnalysisModel();
        ICircuitModel GetTransientAnalysisModel();
    }

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
    
    public interface ICircuitModel
    {
        CircuitNode[] Nodes { get; }
        ICircuitModelElement[] Elements { get; }
        INonlinearCircuitModelElement[] NonlinearElements { get; }
        bool IsLinear { get; }
    }

    public class ElectricCircuitModel : ICircuitModel
    {
        public ElectricCircuitModel(CircuitNode[] nodes, IEnumerable<ICircuitModelElement> elements)
        {
            this.Nodes = nodes;
            this.Elements = elements.ToArray();
            NonlinearElements = Elements.OfType<INonlinearCircuitModelElement>().ToArray();
        }
        public CircuitNode[] Nodes { get; internal set; }
        public ICircuitModelElement[] Elements { get; internal set; }

        public INonlinearCircuitModelElement[] NonlinearElements { get; internal set; }
        public bool IsLinear => NonlinearElements.Length == 0;

    }
}