using NextGenSpice.Core.Devices;

namespace NextGenSpice.Core.Representation
{
	/// <summary>Defines methods for resolving analysis-specific device model instances.</summary>
	/// <typeparam name="TAnalysisModel"></typeparam>
	public interface IModelInstantiationContext<TAnalysisModel>
	{
		/// <summary>Current circuit definition.</summary>
		ICircuitDefinition CircuitDefinition { get; }

		/// <summary>Gets model instance for a given device definition instance.</summary>
		/// <param name="device">The device definition.</param>
		/// <returns></returns>
		IAnalysisDeviceModel<TAnalysisModel> GetModel(ICircuitDefinitionDevice device);

		/// <summary>Gets model instance for device definition instance identified by given name.</summary>
		/// <param name="tag">Tag of the device to be instantiated.</param>
		/// <returns></returns>
		IAnalysisDeviceModel<TAnalysisModel> GetModel(object tag);

		/// <summary>Gets nested instantiation context (for handling subcircuits).</summary>
		/// <returns></returns>
		IModelInstantiationContext<TAnalysisModel> CreateSubcontext();
	}
}