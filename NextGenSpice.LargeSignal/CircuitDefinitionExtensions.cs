using System;
using NextGenSpice.Core.Representation;

namespace NextGenSpice.LargeSignal
{
    public static class CircuitDefinitionExtensions
    {
        /// <summary>Creates instance of <see cref="LargeSignalCircuitModel" /> from given circuit definition.</summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        public static LargeSignalCircuitModel GetLargeSignalModel(this ICircuitDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            return AnalysisModelCreator.Instance.GetModel<LargeSignalCircuitModel>(definition);
        }

        /// <summary>Creates instance of <see cref="LargeSignalCircuitModel" /> from given circuit definition.</summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        public static TAnalysisModel GetAnalysisModel<TAnalysisModel>(this ICircuitDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            return AnalysisModelCreator.Instance.GetModel<TAnalysisModel>(definition);
        }
    }
}