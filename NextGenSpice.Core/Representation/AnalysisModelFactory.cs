using System;
using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    public abstract class AnalysisModelFactory<TAnalysisModel> : IAnalysisModelFactory<TAnalysisModel>
    {
        private readonly Dictionary<Type, Func<ICircuitDefinitionElement, IModelInstantiationContext<TAnalysisModel>, IAnalysisDeviceModel<TAnalysisModel>>>
            modelCreators;

        private readonly Dictionary<Type, Func<object, IModelInstantiationContext<TAnalysisModel>, object>>
            paramCreators;
        
        protected AnalysisModelFactory()
        {
            paramCreators = new Dictionary<Type, Func<object, IModelInstantiationContext<TAnalysisModel>, object>>();
            modelCreators = new Dictionary<Type, Func<ICircuitDefinitionElement, IModelInstantiationContext<TAnalysisModel>, IAnalysisDeviceModel<TAnalysisModel>>>();
        }   

        protected abstract TAnalysisModel Instantiate(IModelInstantiationContext<TAnalysisModel> context);

        public TAnalysisModel Create(ICircuitDefinition circuitDefinition)
        {
            var instantiationContext = new ModelInstantiationContext<TAnalysisModel>(modelCreators, paramCreators, circuitDefinition);

            var analysisModel = Instantiate(instantiationContext);

            return analysisModel;
        }
        public void SetModel<TRepresentation, TModel>(TModel model)
            where TRepresentation : ICircuitDefinitionElement
            where TModel : IAnalysisDeviceModel<TAnalysisModel>
        {
            SetModel<TRepresentation, TModel>(m => model);
        }
        
        public void SetModel<TRepresentation, TModel>(Func<TRepresentation, TModel> factoryFunc)
            where TRepresentation : ICircuitDefinitionElement
            where TModel : IAnalysisDeviceModel<TAnalysisModel>
        {
            modelCreators[typeof(TRepresentation)] = (model, context) => factoryFunc((TRepresentation)model);
        }

        public void SetModel<TRepresentation, TModel>(Func<TRepresentation, IModelInstantiationContext<TAnalysisModel>, TModel> factoryFunc)
            where TRepresentation : ICircuitDefinitionElement
            where TModel : IAnalysisDeviceModel<TAnalysisModel>
        {
            modelCreators[typeof(TRepresentation)] = (model, context) => factoryFunc((TRepresentation)model, context);
        }

        public void SetParam<TParam>(Func<TParam, object> factoryFunc)
        {
            paramCreators[typeof(TParam)] = (param, context) => factoryFunc((TParam) param);
        }

        public void SetParam<TParam>(Func<TParam, IModelInstantiationContext<TAnalysisModel>, object> factoryFunc)
        {
            paramCreators[typeof(TParam)] = (param, context) => factoryFunc((TParam) param, context);
        }
    }
}