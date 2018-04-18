using System;
using NextGenSpice.Core.Devices;

namespace NextGenSpice.Core.Representation
{
    /// <summary>
    ///     Defines methods for registering analysis models for individual circuit devices and building whole analysis
    ///     models from circuit representation.
    /// </summary>
    /// <typeparam name="TAnalysisModel"></typeparam>
    public interface IAnalysisModelFactory<TAnalysisModel>
    {
        /// <summary>Creates new instance of analysis model of type <see cref="TAnalysisModel" /> for given circuit.</summary>
        /// <param name="circuitDefinition">Definition of the circuit.</param>
        /// <returns></returns>
        TAnalysisModel Create(ICircuitDefinition circuitDefinition);

        /// <summary>Registers a factory method for creating analysis-specific device model from the representation.</summary>
        /// <typeparam name="TRepresentation">Class representing the device in circuit definition</typeparam>
        /// <typeparam name="TModel">Analysis-specific class for the device</typeparam>
        /// <param name="factoryFunc">The factory function</param>
        void SetModel<TRepresentation, TModel>(Func<TRepresentation, TModel> factoryFunc)
            where TRepresentation : ICircuitDefinitionDevice
            where TModel : IAnalysisDeviceModel<TAnalysisModel>;

        /// <summary>Registers a factory method for creating analysis-specific device model from the representation.</summary>
        /// <typeparam name="TRepresentation">Class representing the device in circuit definition</typeparam>
        /// <typeparam name="TModel">Analysis-specific class for the device</typeparam>
        /// <param name="factoryFunc">The factory function</param>
        void SetModel<TRepresentation, TModel>(
            Func<TRepresentation, IModelInstantiationContext<TAnalysisModel>, TModel> factoryFunc)
            where TRepresentation : ICircuitDefinitionDevice
            where TModel : IAnalysisDeviceModel<TAnalysisModel>;

        /// <summary>Registers a function for custom parameter processing (e.g. input source behaviors).</summary>
        /// <typeparam name="TParam">Defining type of the parameter.</typeparam>
        /// <param name="factoryFunc">Processing function of the parameter.</param>
        void SetParam<TParam>(Func<TParam, object> factoryFunc);

        /// <summary>Registers a function for custom parameter processing (e.g. input source behaviors).</summary>
        /// <typeparam name="TParam">Defining type of the parameter.</typeparam>
        /// <param name="factoryFunc">Processing function of the parameter.</param>
        void SetParam<TParam>(Func<TParam, IModelInstantiationContext<TAnalysisModel>, object> factoryFunc);
    }
}