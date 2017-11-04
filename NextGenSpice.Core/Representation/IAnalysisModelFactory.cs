using System;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    public interface IAnalysisModelFactory<TAnalysisModel>
    {
        TAnalysisModel Create(ICircuitDefinition circuitDefinition);

        void SetModel<TRepresentation, TModel>() where TRepresentation : ICircuitDefinitionElement
            where TModel : IAnalysisDeviceModel<TAnalysisModel>, new();

        void SetModel<TRepresentation, TModel>(Func<TRepresentation, TModel> factoryFunc)
            where TRepresentation : ICircuitDefinitionElement
            where TModel : IAnalysisDeviceModel<TAnalysisModel>;
    }
}