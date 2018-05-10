using System;
using NextGenSpice.Core.Representation;

namespace NextGenSpice.Core
{
    public static class CircuitDefinitionExtensions
    {
        /// <summary>Creates instance of <see cref="TAnalysisModel" /> from given circuit definition.</summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        public static TAnalysisModel GetAnalysisModel<TAnalysisModel>(this ICircuitDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            return AnalysisModelCreator.Instance.Create<TAnalysisModel>(definition);
        }
    }
}