using System.Collections.Generic;
using NextGenSpice.Core.BehaviorParams;

namespace NextGenSpice.Core.Devices
{
	/// <summary>Class that represents a current source device.</summary>
	public class VoltageSource : TwoTerminalCircuitDevice
	{
		public VoltageSource(InputSourceBehavior behavior, object tag = null) : base(tag)
		{
			Behavior = behavior;
		}

		public VoltageSource(double voltage, object tag = null) : this(
			new ConstantBehavior {Value = voltage}, tag)
		{
		}

		/// <summary>Behavior parameters of the input source.</summary>
		public InputSourceBehavior Behavior { get; set; }

		/// <summary>Creates a deep copy of this device.</summary>
		/// <returns></returns>
		public override ICircuitDefinitionDevice Clone()
		{
			var clone = (VoltageSource) base.Clone();
			clone.Behavior = (InputSourceBehavior) clone.Behavior.Clone();
			return clone;
		}

		/// <summary>Gets metadata about this device interconnections in the circuit.</summary>
		/// <returns></returns>
		public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
		{
			return new[]
			{
				new CircuitBranchMetadata(Anode, Cathode, BranchType.VoltageDefined, this)
			};
		}
	}
}