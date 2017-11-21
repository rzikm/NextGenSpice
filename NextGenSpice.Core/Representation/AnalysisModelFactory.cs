using System;
using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    public abstract class AnalysisModelFactory<TAnalysisModel> : IAnalysisModelFactory<TAnalysisModel>
    {
        private class NullModelInstantiationContext : IModelInstantiationContext<TAnalysisModel> {
            public ICircuitDefinition CircuitDefinition { get; }

            private readonly IAnalysisModelFactory<TAnalysisModel> factory;

            public NullModelInstantiationContext(IAnalysisModelFactory<TAnalysisModel> factory)
            {
                this.factory = factory;
            }

            public IAnalysisDeviceModel<TAnalysisModel> GetModel(ICircuitDefinitionElement element)
            {
                return factory.GetModel(element);
            }
        }

        private readonly Dictionary<Type, Func<ICircuitDefinitionElement, IModelInstantiationContext<TAnalysisModel>, IAnalysisDeviceModel<TAnalysisModel>>>
            modelCreators;

        private ModelInstantiationContext<TAnalysisModel> instantiationContext;
        private readonly IModelInstantiationContext<TAnalysisModel> nullInstantiationContext;
        
        protected AnalysisModelFactory()
        {
            modelCreators = new Dictionary<Type, Func<ICircuitDefinitionElement, IModelInstantiationContext<TAnalysisModel>, IAnalysisDeviceModel<TAnalysisModel>>>();
            nullInstantiationContext = new NullModelInstantiationContext(this);
        }   

        protected abstract TAnalysisModel Instantiate(ModelInstantiationContext<TAnalysisModel> context);

        public TAnalysisModel Create(ICircuitDefinition circuitDefinition)
        {
            instantiationContext = new ModelInstantiationContext<TAnalysisModel>(this, circuitDefinition);

            var analysisModel = Instantiate(instantiationContext);
            instantiationContext = null;

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

        //todo: consider removing this method
        public IAnalysisDeviceModel<TAnalysisModel> GetModel(ICircuitDefinitionElement element)
        {
            return InstantiationContext.GetModel(element);
        }

        IAnalysisDeviceModel<TAnalysisModel> IAnalysisModelFactory<TAnalysisModel>.GetModel(ICircuitDefinitionElement element)
        {
            if (modelCreators.TryGetValue(element.GetType(), out var creator))
            {
                return creator(element, InstantiationContext);
            }
            throw new InvalidOperationException($"No device model set for device {element.GetType().FullName} in factory for {typeof(TAnalysisModel).FullName}.");
        }

        private IModelInstantiationContext<TAnalysisModel> InstantiationContext => instantiationContext ?? nullInstantiationContext;
    }
}