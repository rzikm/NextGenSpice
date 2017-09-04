using System.Collections.Generic;

namespace NextGenSpice.Circuit
{
    public class ElectricCircuit
    {
        public List<CircuitNode> Nodes { get; internal set; }
        public List<ICircuitElement> Elements { get; internal set; }
        public List<INonlinearCircuitElement> NonlinearCircuitElements { get; internal set; }

        public bool IsLinear => NonlinearCircuitElements.Count == 0;
    }
}