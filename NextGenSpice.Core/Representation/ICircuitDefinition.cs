using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    public interface ICircuitDefinition
    {
        int NodeCount { get; }
        IReadOnlyList<double> InitialVoltages { get; }
        IReadOnlyList<ICircuitDefinitionElement> Elements { get; }
        void SetFactory<TAnalysisModel>(IAnalysisModelFactory<TAnalysisModel> factory);
        TAnalysisModel GetModel<TAnalysisModel>();
        IAnalysisModelFactory<TAnalysisModel> GetFactory<TAnalysisModel>();
    }
}