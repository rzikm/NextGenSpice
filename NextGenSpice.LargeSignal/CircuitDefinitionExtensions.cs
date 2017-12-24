using System;
using NextGenSpice.Core.Representation;

namespace NextGenSpice.LargeSignal
{
    public static class CircuitDefinitionExtensions
    {
        public static LargeSignalCircuitModel GetLargeSignalModel(this ICircuitDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            return definition.GetModel<LargeSignalCircuitModel>();
        }
    }
}