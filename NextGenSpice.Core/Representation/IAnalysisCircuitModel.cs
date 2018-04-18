using System.Collections.Generic;

namespace NextGenSpice.Core.Representation
{
    /// <summary>Defines Property for accessing analysis-specific device model instances of the circuit.</summary>
    /// <typeparam name="TDevice"></typeparam>
    public interface IAnalysisCircuitModel<out TDevice>
    {
        /// <summary>Devices of this circuit.</summary>
        IReadOnlyList<TDevice> Devices { get; }
    }
}