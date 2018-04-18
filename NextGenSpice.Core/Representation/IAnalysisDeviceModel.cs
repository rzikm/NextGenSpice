using NextGenSpice.Core.Devices;

namespace NextGenSpice.Core.Representation
{
    /// <summary>
    ///     Metadata interface used to denote to which circuit analysis model this device belongs. All models for given
    ///     analysis type must implement this interface.
    /// </summary>
    /// <typeparam name="TAnalysisModel"></typeparam>
    public interface IAnalysisDeviceModel<TAnalysisModel>
    {
        /// <summary>Instance of definition device that corresponds to this device analysis model.</summary>
        ICircuitDefinitionDevice DefinitionDevice { get; }
    }
}