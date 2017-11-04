using System;
using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    public abstract class ModelFactory<TAnalysis> : ModelFactoryBase, IAnalysisModelFactory<TAnalysis>
    {
        private readonly Dictionary<Type, Func<ICircuitDefinitionElement, IAnalysisDeviceModel<TAnalysis>>>
            modelCreators;

        protected ModelFactory() : base(typeof(TAnalysis))
        {
            modelCreators = new Dictionary<Type, Func<ICircuitDefinitionElement, IAnalysisDeviceModel<TAnalysis>>>();
        }

        public abstract TAnalysis Create(ICircuitDefinition circuitDefinition);

        public void SetModel<TRepresentation, TModel>() where TRepresentation : ICircuitDefinitionElement where TModel : IAnalysisDeviceModel<TAnalysis>, new()
        {
            SetModel<TRepresentation, TModel>(m => new TModel());
        }

        public void SetModel<TRepresentation, TModel>(Func<TRepresentation, TModel> factoryFunc) where TRepresentation : ICircuitDefinitionElement where TModel : IAnalysisDeviceModel<TAnalysis>
        {
            modelCreators[typeof(TRepresentation)] = model => factoryFunc((TRepresentation)model);
        }

        protected IAnalysisDeviceModel<TAnalysis> GetModel(ICircuitDefinitionElement element)
        {
            return modelCreators[element.GetType()](element);
        }
    }
}