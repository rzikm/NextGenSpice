using NextGenSpice.Core.Devices.Parameters;

namespace NextGenSpice.Core.Devices
{
	/// <summary>Class that represents the diode device.</summary>
	public class Diode : TwoTerminalCircuitDevice
	{
		public Diode(DiodeParams parameters, object tag = null, double? voltageHint = null) : base(tag)
		{
			Parameters = parameters;
			VoltageHint = voltageHint;
		}

		/// <summary>Diode model parameters.</summary>
		public DiodeParams Parameters { get; set; }


		/// <summary>Hint for initial voltage across the diode in volts for faster first dc-bias calculation.</summary>
		public double? VoltageHint { get; set; }

		/// <summary>Creates a deep copy of this device.</summary>
		/// <returns></returns>
		public override ICircuitDefinitionDevice Clone()
		{
			var clone = (Diode) base.Clone();
			clone.Parameters = (DiodeParams) clone.Parameters.Clone();
			return clone;
		}
	}
}