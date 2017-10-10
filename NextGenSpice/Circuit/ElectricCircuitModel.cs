using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Elements;

namespace NextGenSpice.Circuit
{
    public class ElectricCircuitModel : ICircuitModel
    {
        public ElectricCircuitModel(CircuitNode[] nodes, IEnumerable<ICircuitModelElement> elements)
        {
            this.Nodes = nodes;
            this.Elements = elements.ToArray();
            LinearElements = Elements.OfType<ILinearCircuitModelElement>().ToArray();
            NonlinearElements = Elements.OfType<INonlinearCircuitModelElement>().ToArray();
            TimeDependentElements = Elements.OfType<ITimeDependentCircuitModelElement>().ToArray();
        }
        public CircuitNode[] Nodes { get; }
        public ICircuitModelElement[] Elements { get; }

        public ILinearCircuitModelElement[] LinearElements { get; }
        public INonlinearCircuitModelElement[] NonlinearElements { get; }
        public ITimeDependentCircuitModelElement[] TimeDependentElements { get; }
        public bool IsLinear => NonlinearElements.Length == 0;

    }
}