using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    public interface IModelInstantiationContext<TAnalysisModel>
    {
        ICircuitDefinition CircuitDefinition { get; }
        IAnalysisDeviceModel<TAnalysisModel> GetModel(ICircuitDefinitionElement element);
    }
}