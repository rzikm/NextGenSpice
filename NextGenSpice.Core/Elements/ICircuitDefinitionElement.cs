using NextGenSpice.Core.Circuit;

namespace NextGenSpice.Core.Elements
{
    /// <summary>
    ///     Defines basic properties and methods for every class that represents a definition of electrical circuit device.
    /// </summary>
    public interface ICircuitDefinitionElement
    {
        /// <summary>
        ///     Set of terminal connections of this device.
        /// </summary>
        NodeConnectionSet ConnectedNodes { get; }

        /// <summary>
        ///     Name identifier of this device.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Creates a deep copy of this device.
        /// </summary>
        /// <returns></returns>
        ICircuitDefinitionElement Clone();
    }
}