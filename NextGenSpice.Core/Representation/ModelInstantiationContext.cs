using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    public class ModelInstantiationContext<TAnalysisModel> : IModelInstantiationContext<TAnalysisModel>
    {
        private readonly IAnalysisModelFactory<TAnalysisModel> factory;
        private readonly Dictionary<ICircuitDefinitionElement, IAnalysisDeviceModel<TAnalysisModel>> resolutionCache;
        private readonly Dictionary<string, ICircuitDefinitionElement> namedElements;

        public ModelInstantiationContext(AnalysisModelFactory<TAnalysisModel> factory, ICircuitDefinition circuitDefinition)
        {
            this.factory = factory;
            CircuitDefinition = circuitDefinition;

            namedElements = circuitDefinition.Elements.Where(e => e.Name != null).ToDictionary(e => e.Name);

            resolutionCache = new Dictionary<ICircuitDefinitionElement, IAnalysisDeviceModel<TAnalysisModel>>();
        }

        public ICircuitDefinition CircuitDefinition { get; }
        public IAnalysisDeviceModel<TAnalysisModel> GetModel(ICircuitDefinitionElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            if (resolutionCache.TryGetValue(element, out var model))
                return model;

            return resolutionCache[element] = factory.GetModel(element);
        }

        public IAnalysisDeviceModel<TAnalysisModel> GetModel(string elementName)
        {
            if (elementName == null) throw new ArgumentNullException(nameof(elementName));

            if (!namedElements.TryGetValue(elementName, out var element))
                throw new ArgumentException($"Element with name {elementName} does not exist in given context.");

            return GetModel(element);
        }
    }
}