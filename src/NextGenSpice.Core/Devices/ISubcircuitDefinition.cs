using System.Collections.Generic;

namespace NextGenSpice.Core.Devices
{
	public interface ISubcircuitDefinition
	{
		/// <summary>Tag for identifying this subcircuit type</summary>
		object Tag { get; }

		/// <summary>Ids from the subcircuit definition that are considered connected to the device terminals.</summary>
		int[] TerminalNodes { get; }

		/// <summary>Number of inner nodes of this subcircuit.</summary>
		int InnerNodeCount { get; }

		/// <summary>Inner devices that define behavior of this subcircuit.</summary>
		IEnumerable<ICircuitDefinitionDevice> Devices { get; }
	}
}