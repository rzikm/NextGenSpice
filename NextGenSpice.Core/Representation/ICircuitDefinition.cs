using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    /// <summary>
    ///     Defines functions and properties that identify electric circuit definition.
    /// </summary>
    public interface ICircuitDefinition
    {
        /// <summary>
        ///     Number of the nodes in the circuit.
        /// </summary>
        int NodeCount { get; }

        /// <summary>
        ///     Initial voltages of nodes by their id.
        /// </summary>
        IReadOnlyList<double?> InitialVoltages { get; }

        /// <summary>
        ///     Set of elements that define this circuit.
        /// </summary>
        IReadOnlyList<ICircuitDefinitionElement> Elements { get; }

        /// <summary>
        ///     Sets factory for creating a model for specific analysis.
        /// </summary>
        /// <typeparam name="TAnalysisModel"></typeparam>
        /// <param name="factory">Instance of factory that creates the analysis-specific model instance.</param>
        void SetFactory<TAnalysisModel>(IAnalysisModelFactory<TAnalysisModel> factory);

        /// <summary>
        ///     Creates analysis-specific model of given type using registered factory instance.
        /// </summary>
        /// <typeparam name="TAnalysisModel">Analysis-specific model type.</typeparam>
        /// <returns></returns>
        TAnalysisModel GetModel<TAnalysisModel>();

        /// <summary>
        ///     Gets the instance of factory class responsible for creating analysis-specific model of givent type.
        /// </summary>
        /// <typeparam name="TAnalysisModel">Analysis-specific model type.</typeparam>
        /// <returns></returns>
        IAnalysisModelFactory<TAnalysisModel> GetFactory<TAnalysisModel>();
    }
}