namespace NextGenSpice.Core.Representation
{
    public interface IAnalysisModelCreator
    {
        /// <summary>Sets factory for creating a model for specific analysis.</summary>
        /// <typeparam name="TAnalysisModel"></typeparam>
        /// <param name="factory">Instance of factory that creates the analysis-specific model instance.</param>
        void SetFactory<TAnalysisModel>(IAnalysisModelFactory<TAnalysisModel> factory);

        /// <summary>Creates analysis-specific model of given type using registered factory instance.</summary>
        /// <typeparam name="TAnalysisModel">Analysis-specific model type.</typeparam>
        /// <param name="circuitDefinition">Definition of the circuit, whose analysis model should be created</param>
        /// <returns></returns>
        TAnalysisModel GetModel<TAnalysisModel>(ICircuitDefinition circuitDefinition);

        /// <summary>Gets the instance of factory class responsible for creating analysis-specific model of givent type.</summary>
        /// <typeparam name="TAnalysisModel">Analysis-specific model type.</typeparam>
        /// <returns></returns>
        IAnalysisModelFactory<TAnalysisModel> GetFactory<TAnalysisModel>();
    }
}