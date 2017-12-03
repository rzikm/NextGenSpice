using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    public class ModelInstantiationContext<TAnalysisModel> : IModelInstantiationContext<TAnalysisModel>
    {
        private readonly Dictionary<ICircuitDefinitionElement, IAnalysisDeviceModel<TAnalysisModel>> resolutionCache;
        private readonly Dictionary<string, ICircuitDefinitionElement> namedElements;
        private readonly Dictionary<Type, Func<object, IModelInstantiationContext<TAnalysisModel>, object>> paramCreators;
        private readonly Dictionary<Type, Func<ICircuitDefinitionElement, IModelInstantiationContext<TAnalysisModel>, IAnalysisDeviceModel<TAnalysisModel>>> modelCreators;

        public ModelInstantiationContext(
            Dictionary<Type, Func<ICircuitDefinitionElement, IModelInstantiationContext<TAnalysisModel>, IAnalysisDeviceModel<TAnalysisModel>>> modelCreators,
            Dictionary<Type, Func<object, IModelInstantiationContext<TAnalysisModel>, object>> paramCreators,
            ICircuitDefinition circuitDefinition)
        {
            this.paramCreators = paramCreators;
            this.modelCreators = modelCreators;
            CircuitDefinition = circuitDefinition;
            resolutionCache = new Dictionary<ICircuitDefinitionElement, IAnalysisDeviceModel<TAnalysisModel>>();
            namedElements = circuitDefinition.Elements.Where(e => e.Name != null).ToDictionary(e => e.Name);

        }

        public ICircuitDefinition CircuitDefinition { get; }
        public IAnalysisDeviceModel<TAnalysisModel> GetModel(ICircuitDefinitionElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            if (resolutionCache.TryGetValue(element, out var model))
                return model;

            return resolutionCache[element] = CreateModel(element);
        }

        private IAnalysisDeviceModel<TAnalysisModel> CreateModel(ICircuitDefinitionElement element)
        {
            if (modelCreators.TryGetValue(element.GetType(), out var factory))
                return factory(element, this);

            throw new InvalidOperationException($"No model creator for type {element.GetType()}");
        }

        public IAnalysisDeviceModel<TAnalysisModel> GetModel(string elementName)
        {
            if (elementName == null) throw new ArgumentNullException(nameof(elementName));

            if (!namedElements.TryGetValue(elementName, out var element))
                throw new ArgumentException($"Element with name {elementName} does not exist in given context.");

            return GetModel(element);
        }

        public object GetParam(object arg)
        {
            if (paramCreators.TryGetValue(arg.GetType(), out var factoryFunc))
            {
                return factoryFunc(arg, this);
            }

            throw new InvalidOperationException($"No param creator for type{arg.GetType()}");
        }
    }
}