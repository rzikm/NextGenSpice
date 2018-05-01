using System.Collections.Generic;
using NextGenSpice.Core.Devices;

namespace NextGenSpice.Core.Representation
{
    /// <summary>Defines Property for accessing analysis-specific device model instances of the circuit.</summary>
    /// <typeparam name="TDevice"></typeparam>
    public interface IAnalysisCircuitModel<out TDevice>
    {
        /// <summary>Devices of this circuit.</summary>
        IReadOnlyList<TDevice> Devices { get; }

        /// <summary>Returns device with given tag or null if no such device exists.</summary>
        /// <param name="tag">The tag of the queried device.</param>
        /// <returns></returns>
        TDevice FindDevice(object tag);


        /// <summary>Returns device implementation for corresponding circuit definition device.</summary>
        /// <param name="device">The tag of the queried device.</param>
        /// <returns></returns>
        TDevice FindDevice(ICircuitDefinitionDevice device);
    }
}