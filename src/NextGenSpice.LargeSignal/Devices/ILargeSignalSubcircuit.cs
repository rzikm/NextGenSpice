using System.Collections.Generic;

namespace NextGenSpice.LargeSignal.Devices
{
    /// <summary>Defines mebers for Subcircuit pseudodevice implementation for large-signal analysis.</summary>
    public interface ILargeSignalSubcircuit : ILargeSignalDevice
    {
        /// <summary>Set of classes that model this subcircuit.</summary>
        IReadOnlyList<ILargeSignalDevice> Devices { get; }
    }
}