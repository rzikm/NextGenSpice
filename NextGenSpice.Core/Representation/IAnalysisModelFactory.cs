using System;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    public interface IAnalysisModelFactory<TAnalysisModel>
    {
        TAnalysisModel Create(ICircuitDefinition circuitDefinition);

        void SetModel<TRepresentation, TModel>(TModel model) where TRepresentation : ICircuitDefinitionElement
            where TModel : IAnalysisDeviceModel<TAnalysisModel>;

        void SetModel<TRepresentation, TModel>(Func<TRepresentation, TModel> factoryFunc)
            where TRepresentation : ICircuitDefinitionElement
            where TModel : IAnalysisDeviceModel<TAnalysisModel>;

        void SetModel<TRepresentation, TModel>(Func<TRepresentation, IAnalysisModelFactory<TAnalysisModel>, TModel> factoryFunc)
            where TRepresentation : ICircuitDefinitionElement
            where TModel : IAnalysisDeviceModel<TAnalysisModel>;

        IAnalysisDeviceModel<TAnalysisModel> GetModel(ICircuitDefinitionElement element);
    }
}