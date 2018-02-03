using System;
using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    /// <summary>
    ///     Class implementig basic functionality for creating circuit analysis models from circuit representation.
    /// </summary>
    /// <typeparam name="TAnalysisModel"></typeparam>
    public abstract class AnalysisModelFactory<TAnalysisModel> : IAnalysisModelFactory<TAnalysisModel>
    {
        private readonly Dictionary<Type, Func<ICircuitDefinitionElement, IModelInstantiationContext<TAnalysisModel>,
                IAnalysisDeviceModel<TAnalysisModel>>>
            modelCreators;

        private readonly Dictionary<Type, Func<object, IModelInstantiationContext<TAnalysisModel>, object>>
            paramCreators;

        protected AnalysisModelFactory()
        {
            paramCreators = new Dictionary<Type, Func<object, IModelInstantiationContext<TAnalysisModel>, object>>();
            modelCreators =
                new Dictionary<Type, Func<ICircuitDefinitionElement, IModelInstantiationContext<TAnalysisModel>,
                    IAnalysisDeviceModel<TAnalysisModel>>>();
        }

        /// <summary>
        ///     Creates new instance of analysis model of type <see cref="TAnalysisModel" /> for given circuit.
        /// </summary>
        /// <param name="circuitDefinition">Definition of the circuit.</param>
        /// <returns></returns>
        public TAnalysisModel Create(ICircuitDefinition circuitDefinition)
        {
            var instantiationContext =
                new ModelInstantiationContext<TAnalysisModel>(modelCreators, paramCreators, circuitDefinition);

            var analysisModel = Instantiate(instantiationContext);

            return analysisModel;
        }

        /// <summary>
        ///     Registers a factory method for creating analysis-specific device model from the representation.
        /// </summary>
        /// <typeparam name="TRepresentation">Class representing the device in circuit definition</typeparam>
        /// <typeparam name="TModel">Analysis-specific class for the device</typeparam>
        /// <param name="factoryFunc">The factory function</param>
        public void SetModel<TRepresentation, TModel>(Func<TRepresentation, TModel> factoryFunc)
            where TRepresentation : ICircuitDefinitionElement
            where TModel : IAnalysisDeviceModel<TAnalysisModel>
        {
            modelCreators[typeof(TRepresentation)] = (model, context) => factoryFunc((TRepresentation) model);
        }

        /// <summary>
        ///     Registers a factory method for creating analysis-specific device model from the representation.
        /// </summary>
        /// <typeparam name="TRepresentation">Class representing the device in circuit definition</typeparam>
        /// <typeparam name="TModel">Analysis-specific class for the device</typeparam>
        /// <param name="factoryFunc">The factory function</param>
        public void SetModel<TRepresentation, TModel>(
            Func<TRepresentation, IModelInstantiationContext<TAnalysisModel>, TModel> factoryFunc)
            where TRepresentation : ICircuitDefinitionElement
            where TModel : IAnalysisDeviceModel<TAnalysisModel>
        {
            modelCreators[typeof(TRepresentation)] = (model, context) => factoryFunc((TRepresentation) model, context);
        }

        /// <summary>
        ///     Registers a function for custom parameter processing (e.g. input source behaviors).
        /// </summary>
        /// <typeparam name="TParam">Defining type of the parameter.</typeparam>
        /// <param name="factoryFunc">Processing function of the parameter.</param>
        public void SetParam<TParam>(Func<TParam, object> factoryFunc)
        {
            paramCreators[typeof(TParam)] = (param, context) => factoryFunc((TParam) param);
        }

        /// <summary>
        ///     Registers a function for custom parameter processing (e.g. input source behaviors).
        /// </summary>
        /// <typeparam name="TParam">Defining type of the parameter.</typeparam>
        /// <param name="factoryFunc">Processing function of the parameter.</param>
        public void SetParam<TParam>(Func<TParam, IModelInstantiationContext<TAnalysisModel>, object> factoryFunc)
        {
            paramCreators[typeof(TParam)] = (param, context) => factoryFunc((TParam) param, context);
        }

        /// <summary>
        ///     Factory method for creating the actual instance of the analysis model.
        /// </summary>
        /// <param name="context">Current instantiation context.</param>
        /// <returns></returns>
        protected abstract TAnalysisModel Instantiate(IModelInstantiationContext<TAnalysisModel> context);
    }
}