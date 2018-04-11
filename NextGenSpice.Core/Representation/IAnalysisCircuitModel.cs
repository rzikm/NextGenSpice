using System.Collections.Generic;

namespace NextGenSpice.Core.Representation
{
    /// <summary>Defines Property for accessing analysis-specific device model instances of the circuit.</summary>
    /// <typeparam name="TElement"></typeparam>
    public interface IAnalysisCircuitModel<out TElement>
    {
        /// <summary>Devices of this circuit.</summary>
        IReadOnlyList<TElement> Elements { get; }
    }
}