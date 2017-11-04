using System;
using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    public abstract class AnalysisModelFactory<TAnalysis> : IAnalysisModelFactory<TAnalysis>
    {
        private readonly Dictionary<Type, Func<ICircuitDefinitionElement, IAnalysisDeviceModel<TAnalysis>>>
            modelCreators;

        protected AnalysisModelFactory()
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
            if (modelCreators.TryGetValue(element.GetType(), out var creator))
            {
                return creator(element);
            }
            throw new InvalidOperationException($"No device model set for device {element.GetType().FullName} in factory for {typeof(TAnalysis).FullName}.");
        }
    }
}