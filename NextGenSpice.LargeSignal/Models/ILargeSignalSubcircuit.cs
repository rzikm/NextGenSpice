using System.Collections.Generic;

namespace NextGenSpice.LargeSignal.Models
{
    public interface ILargeSignalSubcircuit : ILargeSignalDevice
    {
        /// <summary>Set of classes that model this subcircuit.</summary>
        IReadOnlyList<ILargeSignalDevice> Devices { get; }
    }
}