using System;
using System.CodeDom;
using NextGenSpice.Elements;

namespace NextGenSpice.Representation
{
    public interface IAnalysisModelFactory<TAnalysisModel>
    {
        TAnalysisModel Create(ICircuitDefinition2 circuitDefinition);

        void SetModel<TRepresentation, TModel>() where TRepresentation : ICircuitDefinitionElement
            where TModel : IAnalysisDeviceModel<TAnalysisModel>, new();

        void SetModel<TRepresentation, TModel>(Func<TModel, TRepresentation> factoryFunc)
            where TModel : IAnalysisDeviceModel<TAnalysisModel>;
    }
}