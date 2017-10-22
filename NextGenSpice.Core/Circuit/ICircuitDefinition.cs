using System.Collections.Generic;
using NextGenSpice.Elements;

namespace NextGenSpice.Circuit
{
    public interface ICircuitDefinition
    {
        List<CircuitNode> Nodes { get; }
        List<ICircuitDefinitionElement> Elements { get; }
        ICircuitModel GetLargeSignalModel();
        ICircuitModel GetSmallSignalModel();
    }
}