using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    public class ModelInstantiationContext<TAnalysisModel> : IModelInstantiationContext<TAnalysisModel>
    {
        private readonly IAnalysisModelFactory<TAnalysisModel> factory;
        private readonly Dictionary<ICircuitDefinitionElement, IAnalysisDeviceModel<TAnalysisModel>> resolutionCache;

        public ModelInstantiationContext(AnalysisModelFactory<TAnalysisModel> factory, ICircuitDefinition circuitDefinition)
        {
            this.factory = factory;
            CircuitDefinition = circuitDefinition;

            resolutionCache = new Dictionary<ICircuitDefinitionElement, IAnalysisDeviceModel<TAnalysisModel>>();
        }

        public ICircuitDefinition CircuitDefinition { get; }
        public IAnalysisDeviceModel<TAnalysisModel> GetModel(ICircuitDefinitionElement element)
        {
            if (resolutionCache.TryGetValue(element, out var model))
                return model;

            return resolutionCache[element] = factory.GetModel(element);
        }
    }
}