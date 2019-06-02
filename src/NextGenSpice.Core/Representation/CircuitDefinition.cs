using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Devices;

namespace NextGenSpice.Core.Representation
{
	/// <summary>Class that represents definition of an electric circuit.</summary>
	public class CircuitDefinition : ICircuitDefinition
	{
		internal CircuitDefinition(IReadOnlyList<double?> initialVoltages,
			IReadOnlyList<ICircuitDefinitionDevice> devices)
		{
			InitialVoltages = initialVoltages;
			Devices = devices;
		}

		/// <summary>Number of the nodes in the circuit.</summary>
		public int NodeCount => InitialVoltages.Count;

		/// <summary>Initial voltages of nodes by their id.</summary>
		public IReadOnlyList<double?> InitialVoltages { get; }

		/// <summary>Set of devices that define this circuit.</summary>
		public IReadOnlyList<ICircuitDefinitionDevice> Devices { get; }

		/// <summary>Returns contained device with given tag or null if no such device is found.</summary>
		/// <param name="tag">The tag of the device.</param>
		public ICircuitDefinitionDevice FindDevice(object tag)
		{
			if (tag == null) throw new ArgumentNullException(nameof(tag));

			return Devices.FirstOrDefault(dev => Equals(dev.Tag, tag));
		}
	}
}