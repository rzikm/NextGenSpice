using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    /// <summary>
    /// Defines methods for resolving analysis-specific device model instances.
    /// </summary>
    /// <typeparam name="TAnalysisModel"></typeparam>
    public interface IModelInstantiationContext<TAnalysisModel>
    {
        /// <summary>
        /// Current circuit definition.
        /// </summary>
        ICircuitDefinition CircuitDefinition { get; }

        /// <summary>
        /// Gets model instance for a given device definition instance.
        /// </summary>
        /// <param name="element">The device definition.</param>
        /// <returns></returns>
        IAnalysisDeviceModel<TAnalysisModel> GetModel(ICircuitDefinitionElement element);

        /// <summary>
        /// Gets model instance for device definition instance identified by given name.
        /// </summary>
        /// <param name="element">The name of the device.</param>
        /// <returns></returns>
        IAnalysisDeviceModel<TAnalysisModel> GetModel(string name);

        /// <summary>
        /// Processes parameter using registered factory function for its type.
        /// </summary>
        /// <param name="arg">Argument to be processed.</param>
        /// <returns></returns>
        object GetParam(object arg);
    }
}