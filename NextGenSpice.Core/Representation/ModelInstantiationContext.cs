using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    /// <summary>
    ///     Context in which a certain circuit definition is instantiated
    /// </summary>
    /// <typeparam name="TAnalysisModel"></typeparam>
    public class ModelInstantiationContext<TAnalysisModel> : IModelInstantiationContext<TAnalysisModel>
    {
        private readonly Dictionary<Type, Func<ICircuitDefinitionElement, IModelInstantiationContext<TAnalysisModel>,
            IAnalysisDeviceModel<TAnalysisModel>>> modelCreators;

        private readonly Dictionary<string, ICircuitDefinitionElement> namedElements;

        private readonly Dictionary<Type, Func<object, IModelInstantiationContext<TAnalysisModel>, object>>
            paramCreators;

        private readonly Dictionary<ICircuitDefinitionElement, IAnalysisDeviceModel<TAnalysisModel>> resolutionCache
            ; // cached models so we can compose arbitrary object graph.

        public ModelInstantiationContext(
            Dictionary<Type, Func<ICircuitDefinitionElement, IModelInstantiationContext<TAnalysisModel>,
                IAnalysisDeviceModel<TAnalysisModel>>> modelCreators,
            Dictionary<Type, Func<object, IModelInstantiationContext<TAnalysisModel>, object>> paramCreators,
            ICircuitDefinition circuitDefinition)
        {
            this.paramCreators = paramCreators;
            this.modelCreators = modelCreators;
            CircuitDefinition = circuitDefinition;
            resolutionCache = new Dictionary<ICircuitDefinitionElement, IAnalysisDeviceModel<TAnalysisModel>>();
            namedElements = circuitDefinition.Elements.Where(e => e.Name != null).ToDictionary(e => e.Name);
        }

        /// <summary>
        ///     Current circuit definition.
        /// </summary>
        public ICircuitDefinition CircuitDefinition { get; }

        /// <summary>
        ///     Gets model instance for a given device definition instance.
        /// </summary>
        /// <param name="element">The device definition.</param>
        /// <returns></returns>
        public IAnalysisDeviceModel<TAnalysisModel> GetModel(ICircuitDefinitionElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            if (resolutionCache.TryGetValue(element, out var model))
                return model;

            return resolutionCache[element] = CreateModel(element);
        }

        /// <summary>
        ///     Creates model instance for definition element with given name.
        /// </summary>
        /// <param name="elementName">Name of the element to be instantiated.</param>
        /// <returns></returns>
        public IAnalysisDeviceModel<TAnalysisModel> GetModel(string elementName)
        {
            if (elementName == null) throw new ArgumentNullException(nameof(elementName));

            if (!namedElements.TryGetValue(elementName, out var element))
                throw new ArgumentException($"Element with name {elementName} does not exist in given context.");

            return GetModel(element);
        }

        /// <summary>
        ///     Processes parameter using registered factory function for its type.
        /// </summary>
        /// <param name="arg">Argument to be processed.</param>
        /// <returns></returns>
        public object GetParam(object arg)
        {
            if (paramCreators.TryGetValue(arg.GetType(), out var factoryFunc))
                return factoryFunc(arg, this);

            throw new InvalidOperationException($"No param creator for type{arg.GetType()}");
        }

        /// <summary>
        ///     Creates model instance for given definition element.
        /// </summary>
        /// <param name="element">Element to be instantiated.</param>
        /// <returns></returns>
        private IAnalysisDeviceModel<TAnalysisModel> CreateModel(ICircuitDefinitionElement element)
        {
            if (modelCreators.TryGetValue(element.GetType(), out var factory))
                return factory(element, this);

            throw new InvalidOperationException($"No model creator for type {element.GetType()}");
        }
    }
}