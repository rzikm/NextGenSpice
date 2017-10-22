using System.Collections.Generic;
using NextGenSpice.Circuit;
using NextGenSpice.Elements;

namespace NextGenSpice.Representation
{
    public interface ICircuitDefinition2
    {
        List<CircuitNode> Nodes { get; set; }
        List<ICircuitDefinitionElement> Elements { get; set; }
        void SetFactory<TAnalysisModel>(IAnalysisModelFactory<TAnalysisModel> factory);
        TAnalysisModel GetModel<TAnalysisModel>();
        IAnalysisModelFactory<TAnalysisModel> GetFactory<TAnalysisModel>();
    }
}